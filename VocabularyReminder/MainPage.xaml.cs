using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DataAccessLibrary;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using VocabularyReminder.Services;
using System.Diagnostics;
using Windows.Storage;

namespace VocabularyReminder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaPlayer Player => PlaybackService.Instance.Player;

        MediaPlaybackList PlaybackList
        {
            get { return Player.Source as MediaPlaybackList; }
            set { Player.Source = value; }
        }

        MediaList MediaList
        {
            get { return PlaybackService.Instance.CurrentPlaylist; }
            set { PlaybackService.Instance.CurrentPlaylist = value; }
        }

        private int WordId = 1;

        public MainPage()
        {
            this.InitializeComponent();

            Task.Factory.StartNew(() =>
            {
                // Create a new playback list
                if (PlaybackList == null)
                    PlaybackList = new MediaPlaybackList();
                this.WordId = DataAccess.GetFirstWordId();
            });
        }

        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            Helper.ShowToast("Start Importing...");
            string tempInp = this.inp_ListWord.Text;
            var ListWord = Regex.Split(tempInp, "\r\n|\r|\n");
            Task.Factory.StartNew(() =>
            {
                //try
                //{
                    int Count = 0;
                    int CountSuccess = 0;
                    foreach (var item in ListWord)
                    {
                        if (!String.IsNullOrEmpty(item))
                        {
                            Count++;
                            if (DataAccess.AddVocabulary(item) > 0)
                            {
                                CountSuccess++;
                            }
                        }
                    }
                    Helper.ShowToast("Imported Success " + CountSuccess + "/" + Count + " Entered Vocabulary.");
                //} catch (Exception ex)
                //{
                //    Helper.ShowToast("Import Fail: " + ex.Message);
                //}
            });
        }

        private async Task ProcessBackgroundTranslateAsync()
        {
            try
            {
                var ListVocabulary = DataAccess.GetListVocabularyToTranslate();

                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = Environment.ProcessorCount * 2;    // TODO
                await Task.Run(() => Parallel.ForEach(ListVocabulary, parallelOptions, async _item =>
                {
                    await TranslateService.goTranslateAsync(_item);
                }));

                Helper.ShowToast("Crawling: Process Background Translate Finished.");
            } catch (Exception ex)
            {
                Helper.ShowToast("Crawling: Process Background Translate Failed: " + ex.Message);
            }
        }

        private async Task ProcessBackgroundGetPlayURLAsync()
        {
            try
            {
                var ListVocabulary = DataAccess.GetListVocabularyToGetPlayURL();

                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = Environment.ProcessorCount * 2;    // TODO
                await Task.Run(() => Parallel.ForEach(ListVocabulary, parallelOptions, async _item =>
                {
                    await TranslateService.goGetPlayURLAsync(_item);
                }));

                Helper.ShowToast("Crawling: Process Background Get Play URL Finished.");

            } catch (Exception ex)
            {
                Helper.ShowToast("Crawling: Process Background Get Play URL Fail: " + ex.Message);
            }
            
        }

        private void ProcessBackgroundGetRelatedWords()
        {
            try
            {
                var ListVocabulary = DataAccess.GetListVocabularyToGetRelatedWords();
                foreach (var _item in ListVocabulary)
                {
                    TranslateService.goGetRelatedAsync(_item);
                }
                Helper.ShowToast("Crawling: Process Background Get Related Words Finished.");
            } catch (Exception ex)
            {
                Helper.ShowToast("Crawling: Process Background Get Related Words Fail: " + ex.Message);
            }
        }

        private void btn_Process_Translate_Mp3_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                Helper.ShowToast("Start Crawling...");
                await this.ProcessBackgroundTranslateAsync();
                await this.ProcessBackgroundGetPlayURLAsync();
                this.ProcessBackgroundGetRelatedWords();
                Helper.ShowToast("Crawling Finished.");
            });
        }

        private void btn_Start_Learning_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Helper.ClearToast();
                Vocabulary _item = DataAccess.GetVocabularyById(WordId);
                if (_item.Id == 0)
                {
                    WordId = DataAccess.GetFirstWordId();
                    _item = DataAccess.GetVocabularyById(WordId);
                }
                WordId++;
                VocabularyToast.loadByVocabulary(_item);
                
                //while (true)
                //{

                //    await Task.Delay(1000);
                //}
            });
        }

        private void btn_Delete_DB_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    Helper.ShowToast("Start Deleting...");
                    var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
                    foreach (var file in files)
                    {
                        await file.DeleteAsync();
                    }
                    await ApplicationData.Current.LocalFolder.CreateFileAsync("vocabulary.db", CreationCollisionOption.OpenIfExists);
                    DataAccess.InitializeDatabase();
                    Helper.ShowToast("Delete File DB Success");
                } catch (Exception ex)
                {
                    Helper.ShowToast("Delete File DB Fail: " + ex.Message);
                }
            });
        }

        private void btn_Test_Click(object sender, RoutedEventArgs e)
        {
            Vocabulary _item = DataAccess.GetVocabularyById(WordId);
            if (_item.Id == 0)
            {
                WordId = DataAccess.GetFirstWordId();
                _item = DataAccess.GetVocabularyById(WordId);
            }
            TranslateService.goGetRelatedAsync(_item);
        }
    }
}

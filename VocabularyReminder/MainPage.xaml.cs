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

            // Create a new playback list
            if (PlaybackList == null)
                PlaybackList = new MediaPlaybackList();

            this.WordId = DataAccess.GetFirstWordId();
        }

        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            string tempInp = this.inp_ListWord.Text;
            var ListWord = Regex.Split(tempInp, "\r\n|\r|\n");
            Task.Factory.StartNew(() =>
            {
                int Count = 0;
                foreach (var item in ListWord)
                {
                    DataAccess.AddVocabulary(item);
                    Count++;
                }
                Helper.ShowToast("Import Success " + Count + " Vocabulary.");
            });
        }

        private void ProcessBackgroundTranslate()
        {
            var ListVocabulary = DataAccess.GetListVocabularyToTranslate();
            foreach (var item in ListVocabulary)
            {
                TranslateService.goTranslateAsync(item);
            }
            Helper.ShowToast("Process Background Translate Finished.");
        }

        private void ProcessBackgroundGetPlayURL()
        {
            var ListVocabulary = DataAccess.GetListVocabularyToGetPlayURL();
            foreach (var item in ListVocabulary)
            {
                TranslateService.goGetPlayURLAsync(item);
            }
            Helper.ShowToast("Process Background Get Play URL Finished.");
        }

        private void btn_Process_Translate_Mp3_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                this.ProcessBackgroundTranslate();
                this.ProcessBackgroundGetPlayURL();
            });
        }

        private void btn_Start_Learning_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Helper.ClearToast();
                WordId++;
                Vocabulary _item = DataAccess.GetVocabularyById(WordId);
                VocabularyToast.loadByVocabulary(_item);
                //while (true)
                //{

                //    await Task.Delay(1000);
                //}
            });
        }

        private void btn_Delete_DB_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                DataAccess.ResetDatabase();
                Helper.ShowToast("Delete File DB Success");
            });
        }
    }
}

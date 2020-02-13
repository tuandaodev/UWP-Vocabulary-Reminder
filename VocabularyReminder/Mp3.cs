using DataAccessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabularyReminder.Services;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace VocabularyReminder
{

    public class Mp3
    {

        public static void play(MediaPlayer mediaPlayer, string rawUrl)
        {
            Task.Factory.StartNew(async () =>
            {
                var url = new Uri(rawUrl);
                string filename = System.IO.Path.GetFileName(url.LocalPath);
                if (await isFilePresent(filename))
                {
                    StorageFolder Folder = ApplicationData.Current.LocalFolder;
                    //StorageFile sf = await Folder.GetFileAsync(filename);
                    mediaPlayer.Source = MediaSource.CreateFromStorageFile(await ApplicationData.Current.LocalFolder.GetFileAsync(filename));
                    mediaPlayer.Play();
                } else
                {
                    var destinationFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                    var download = new BackgroundDownloader().CreateDownload(url, destinationFile);
                    //download.IsRandomAccessRequired = true;
                    mediaPlayer.Source = MediaSource.CreateFromDownloadOperation(download);
                    mediaPlayer.AutoPlay = true;
                    mediaPlayer.Play();
                }
            });
        }

        public static void preloadMp3File(Vocabulary _item)
        {
            Task.Factory.StartNew(() =>
            {
                if (!String.IsNullOrEmpty(_item.PlayURL))
                {
                    preLoadMp3Remote(_item.PlayURL);
                }
                if (!String.IsNullOrEmpty(_item.PlayURL2))
                {
                    preLoadMp3Remote(_item.PlayURL2);
                }
            });
        }

        private static void preLoadMp3Remote(string mp3RemoteUrl)
        {
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    var url = new Uri(mp3RemoteUrl);
                    string filename = System.IO.Path.GetFileName(url.LocalPath);
                    if (await isFilePresent(filename))
                    {
                        // do nothing
                    }
                    else
                    {
                        var destinationFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                        var download = new BackgroundDownloader().CreateDownload(url, destinationFile);
                        await download.StartAsync().AsTask();
                    }
                });
            }
            catch (Exception ex)
            {
                Helper.ShowToast("PreLoad Mp3 Error: " + ex.Message);
            }
        }

        public static async Task<bool> isFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }
    }
}

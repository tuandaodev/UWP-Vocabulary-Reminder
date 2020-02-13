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
using Windows.Storage.FileProperties;

namespace VocabularyReminder
{

    public class Mp3
    {
        public static void play(string rawUrl)
        {
            Task.Factory.StartNew(async () =>
            {
                var url = new Uri(rawUrl);
                string filename = System.IO.Path.GetFileName(url.LocalPath);
                if (await isFilePresent(filename))
                {
                    StorageFolder Folder = ApplicationData.Current.LocalFolder;
                    //StorageFile sf = await Folder.GetFileAsync(filename);
                    App.mediaPlayer.Source = MediaSource.CreateFromStorageFile(await ApplicationData.Current.LocalFolder.GetFileAsync(filename));
                    App.mediaPlayer.Play();
                } else
                {
                    var destinationFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                    var download = new BackgroundDownloader().CreateDownload(url, destinationFile);
                    //download.IsRandomAccessRequired = true;
                    App.mediaPlayer.Source = MediaSource.CreateFromDownloadOperation(download);
                    App.mediaPlayer.AutoPlay = true;
                    App.mediaPlayer.Play();
                }
            });
        }

        public static void preloadMp3FileSingle(Vocabulary _item)
        {
            Task.Factory.StartNew(() =>
            {
                if (!String.IsNullOrEmpty(_item.PlayURL))
                {
                    Task.Factory.StartNew(async () =>
                    {
                        await preloadMp3SingleAsync(_item.PlayURL);
                    });
                }
                if (!String.IsNullOrEmpty(_item.PlayURL2))
                {
                    Task.Factory.StartNew(async () =>
                    {
                        await preloadMp3SingleAsync(_item.PlayURL2);
                    });
                }
            });
        }

        public static async Task<bool> isFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            if (item != null)
            {
                BasicProperties props = await item.GetBasicPropertiesAsync();
                if (props.Size == 0)
                {
                    await item.DeleteAsync();
                    return false;
                }
            }
            return item != null;
        }

        public static async Task preloadMp3SingleAsync(string mp3RemoteUrl)
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
        }

        public static async Task preloadMp3Multiple(Vocabulary _item)
        {
            if (!String.IsNullOrEmpty(_item.PlayURL))
            {
                await preloadMp3SingleAsync(_item.PlayURL);
            }
            if (!String.IsNullOrEmpty(_item.PlayURL2))
            {
                await preloadMp3SingleAsync(_item.PlayURL2);
            }
        }
    }
}

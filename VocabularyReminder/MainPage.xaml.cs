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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

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


        public MainPage()
        {
            this.InitializeComponent();

            // Create a new playback list
            if (PlaybackList == null)
                PlaybackList = new MediaPlaybackList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Vocabulary.load("Education");
        }
    }
}

using MediaPlayer.Model;
using MediaPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MediaPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectPage : Page
    {
        private bool _isManipulating = false;
        private Playlist _playlist;
        public SelectPage()
        {
            this.InitializeComponent();
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // custom nút back
            HardwareButtons.BackPressed +=HardwareButtons_BackPressed;
            this._playlist = e.Parameter as Playlist;
            // bind;
            this.mediaElement.DataContext = _playlist;
            this.Textblock_Title.DataContext = _playlist;
            this.duration.DataContext = _playlist;
            this.progressBar.DataContext = _playlist;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            this._playlist = null;
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            //e.Handled = true;
            //if (Frame.CanGoBack)
            //{
            //    Frame.GoBack();
            //}
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {

        }

        private void timeBtn_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if(!_isManipulating)
                _isManipulating = true;

            var btn = sender as Ellipse;
            var nextPos = Canvas.GetLeft(btn) + e.Delta.Translation.X;
           
            if(nextPos < 0)
            {
                nextPos = 0;
                //setMusicPostionPercent(0);
            }
            else if (nextPos > 300)
            {
                nextPos = 300;
                //setMusicPostionPercent(1);
            }

            Canvas.SetLeft(btn, nextPos);
        }

        private void setMusicPostionPercent(float percent)
        {
            var fileInfo = mediaElement.DataContext as FilesViewModel;
            var time = fileInfo.Duration.TotalSeconds * percent;
            
            mediaElement.Position = new TimeSpan(0, 0, 0, (int)time);
        }

        private void timeBtn_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var timeBtn = sender as Ellipse;
            if (timeBtn == null)
                return;

            var curPos = Canvas.GetLeft(timeBtn);
            var percent = curPos / (timeCanvas.Width - 40);

            //setMusicPostionPercent((float)percent);
            
            progressBar.Value = progressBar.Maximum * percent;

            _isManipulating = false;
        }

        private void progressBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var progress = sender as ProgressBar;
            if (progress == null || _isManipulating)
                return;

            var percent = progress.Value / progress.Maximum;
            var curPos = (timeCanvas.Width - 40) * percent;
            Canvas.SetLeft(timeBtn, curPos);
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            _playlist.Next();
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            _playlist.Previous();
        }

        private void shufferBtn_Checked(object sender, RoutedEventArgs e)
        {
            _playlist.Shuffle();
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if ((sender as MediaElement).IsLooping)
                return;
            _playlist.Next();
        }

        private void repeatBtn_Checked(object sender, RoutedEventArgs e)
        {
            this.mediaElement.IsLooping = true;
        }

        private void repeatBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            this.mediaElement.IsLooping = false;
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

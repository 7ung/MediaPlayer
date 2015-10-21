using MediaPlayer.Model;
using MediaPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
using Windows.ApplicationModel.Core;
using System.Threading.Tasks;
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
        private AutoResetEvent _sererInitialized;
        private bool _isbackgroundtaskrunning = false;
        private bool isBackgroundTaskBegin;

        private DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        private bool Isbackgroundtaskrunning()
        {
            if (_isbackgroundtaskrunning)
                return true;
            object value = ApplicationSettingHelper.ReadResetSettingsValue(Constant.BackgroundTaskState);
            if (value == null)
                return false;
            return (_isbackgroundtaskrunning = (String.Equals(value, Constant.BackgroundTaskRunning))); // ASSIGN. not compare!!!
        }
        public SelectPage()
        {
            this.InitializeComponent();
            _sererInitialized = new AutoResetEvent(false);
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;

        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // custom nút back
            HardwareButtons.BackPressed +=HardwareButtons_BackPressed;
            this._playlist = e.Parameter as Playlist;
            // bind;
            //this.mediaElement.IsMuted = true;
            //this.mediaElement.DataContext = _playlist;
            // có thể bind textblock current với BackgroundMediaPlayer.Current.Position;
            this.Textblock_Title.DataContext = _playlist;
            this.duration.DataContext = _playlist;
            this.progressBar.DataContext = _playlist;

            // hard code

            //await CacheAccess.SaveToCache(Constant.PlaylistPath, _playlist);
            //_playlist = await CacheAccess.LoadFromCache(Constant.PlaylistPath);

            await CacheAccess.SaveCurrentPlaylist(Constant.CurrentPlaylist, _playlist);

            _playlist.CurrentIndex = (e.Parameter as Playlist).CurrentIndex;

            App.Current.Suspending += App_Suspending;
            App.Current.Resuming += App_Resuming;

            ApplicationSettingHelper.SaveSettingsValue(Constant.AppState, Constant.ForegroundAppActive);
            if (_isbackgroundtaskrunning || BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Closed)
            {
                AddMediaPlayerEventHandlers();
                var initeresult = this.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () =>       // lamda expression
                    {
                        bool rs = this._sererInitialized.WaitOne(2000); // why ???
                        //System.Diagnostics.Debug.Assert(rs == false, "BackgroundAudioTaskBegin WaitOne Failed!!!");
                        System.Diagnostics.Debug.WriteLine("send msg command to init setindex play");
                    }
                    );
                initeresult.Completed = new AsyncActionCompletedHandler(initBackgroundtaskCompleted);
            }

            // init timer
            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            if (BackgroundMediaPlayer.Current == null)
                return;

            progressBar.Value = BackgroundMediaPlayer.Current.Position.TotalSeconds;
        }

        /// <summary> 
        /// Unsubscribes to MediaPlayer events. SHOULD RUN ONLY ON SUSPEND 
        /// </summary> 
        private void RemoveMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged -= this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground -= this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        /// <summary> 
        /// Subscribes to MediaPlayer events 
        /// </summary> 
        private void AddMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged += this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground += this.BackgroundMediaPlayer_MessageReceivedFromBackground;
            
        }
        private void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
            foreach (var key in e.Data.Keys)
            {
                switch (key)
                {
                    case Constant.BackgroundTaskStarted:
                        //Wait for Background Task to be initialized before starting playback
                        _sererInitialized.Set();
                        break;
                }
            }
        }

        private async void MediaPlayer_CurrentStateChanged(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
               () =>
                   {
                       duration.Text = sender.NaturalDuration.ToString(@"mm\:ss");
                       currentsecond.Text = sender.Position.ToString(@"mm\:ss");

                       progressBar.Value = 0;
                       progressBar.Maximum = sender.NaturalDuration.TotalSeconds;
        }
               );
        }

        private void initBackgroundtaskCompleted(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                System.Diagnostics.Debug.WriteLine("Background Audio Task initialized");
                var msg = new ValueSet();
                //var list = this._playlist.ListFile.Select(file => file.File.Path).ToArray();
                
                msg.Add(Command.PlayWithIndex, _playlist.CurrentIndex.ToString());
                BackgroundMediaPlayer.SendMessageToBackground(msg);
                // Do something here
            }
            else if (asyncStatus == AsyncStatus.Error)
            {
                System.Diagnostics.Debug.WriteLine("Background Audio Task could not initialized due to an error ::" );
                // do somethinge here
            } 
        }
        // APP SUSPEND
        private void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferal = e.SuspendingOperation.GetDeferral(); // requests that the app suspending operation be delayed
            ValueSet valueset = new ValueSet();
            valueset.Add(Constant.AppSuspended, DateTime.Now.ToString());       // ???
            BackgroundMediaPlayer.SendMessageToBackground(valueset);
            this.RemoveMediaPlayerEventHandlers();
            ApplicationSettingHelper.SaveSettingsValue(Constant.AppState, Constant.AppSuspended);
            deferal.Complete();
         
        }

        // RESUME
        private void App_Resuming(object sender, object e)
        {
            ApplicationSettingHelper.SaveSettingsValue(Constant.AppState, Constant.AppResumed);
            if (Isbackgroundtaskrunning())
            {
                AddMediaPlayerEventHandlers();

                ValueSet messageDictionary = new ValueSet();
                messageDictionary.Add(Constant.AppResumed, DateTime.Now.ToString());
                BackgroundMediaPlayer.SendMessageToBackground(messageDictionary);
                if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing)
                {
                    // set nút play thành playing
                    playBtn.IsChecked = true;
                }
                else
                {
                    // set nút play thành pause
                    playBtn.IsChecked = false;
                }
            }
            else
            {
                // set nút play thàn playing 
                playBtn.IsChecked = true;
            }
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
            if (BackgroundMediaPlayer.Current == null)
                return;
            
            var time = BackgroundMediaPlayer.Current.NaturalDuration.TotalSeconds * percent;
            BackgroundMediaPlayer.Current.Position = new TimeSpan(0, 0, 0, (int)time);
        }

        private void timeBtn_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var timeBtn = sender as Ellipse;
            if (timeBtn == null)
                return;

            var curPos = Canvas.GetLeft(timeBtn);
            var percent = curPos / (timeCanvas.Width - 40);

            setMusicPostionPercent((float)percent);
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

            if (BackgroundMediaPlayer.Current == null)
                return;
            currentsecond.Text = BackgroundMediaPlayer.Current.Position.ToString(@"mm\:ss");
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
           // _playlist.Next();
            if (Isbackgroundtaskrunning())
            {
                var msg = new ValueSet();
                msg.Add(Command.Next, "");
                BackgroundMediaPlayer.SendMessageToBackground(msg);                
            }
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Isbackgroundtaskrunning())
            {
                var msg = new ValueSet();
                msg.Add(Command.Previous, "");
                BackgroundMediaPlayer.SendMessageToBackground(msg);
            }
        }

        private void shufferBtn_Checked(object sender, RoutedEventArgs e)
        {
           // _playlist.Shuffle();
            if (Isbackgroundtaskrunning())
            {
                var msg = new ValueSet();
                msg.Add(Command.Shuffle, "");
                BackgroundMediaPlayer.SendMessageToBackground(msg);
            }
        }


        private void repeatBtn_Checked(object sender, RoutedEventArgs e)
        {
            //if (Isbackgroundtaskrunning())
            //{
            //    var msg = new ValueSet();
            //    msg.Add(Command.LoopState, eLoopState.All);
            //    BackgroundMediaPlayer.SendMessageToBackground(msg);
            //}
        }

        private void repeatBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            //if (Isbackgroundtaskrunning())
            //{
            //    var msg = new ValueSet();
            //    msg.Add(Command.LoopState, eLoopState.One);
            //    BackgroundMediaPlayer.SendMessageToBackground(msg);
            //}
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Isbackgroundtaskrunning())
            {
                var msg = new ValueSet();
                
                if (playBtn.IsChecked == true)
                {
                    msg.Add(Command.Play, "");
                }
                else
                {
                    msg.Add(Command.Pause, "");
                }

                BackgroundMediaPlayer.SendMessageToBackground(msg);
            }
        }

        private void repeatBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Isbackgroundtaskrunning())
            {
                var x = sender as ToggleButton;
                var msg = new ValueSet();
                
                if (x.IsChecked == true)
                {
                    msg.Add(Command.LoopState, eLoopState.All);
                }
                else if (x.IsChecked == false)
                {
                    msg.Add(Command.LoopState, eLoopState.One);
                }
                else
                {
                    msg.Add(Command.LoopState, eLoopState.None);
                }
                
                BackgroundMediaPlayer.SendMessageToBackground(msg);
            }
        }
    }
}

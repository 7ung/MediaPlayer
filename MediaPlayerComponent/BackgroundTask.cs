using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using MediaPlayer;
namespace MediaPlayerComponent
{
    // ref : https://code.msdn.microsoft.com/windowsapps/BackgroundAudio-63bbc319/sourcecode?fileId=111812&pathId=1324331438

    public sealed class BackgroundTask : IBackgroundTask
    {
        enum eForegroundState
        {
            Active,
            Suspended,
            Unknown,
        }
        
        // giữ cho task background không bị terminate
        private BackgroundTaskDeferral _deferral;
        private SystemMediaTransportControls _smtc; // ông chủ của UVC
        private eForegroundState _foregroundState;
        private AutoResetEvent _backgroundstarted = new AutoResetEvent(false);
        private bool isbackgroundtaskrunning = false;
        //private PlaylistManager playlistManager;
        private BackgroundPlaylist Playlist;

        private eLoopState _loopState = eLoopState.All;

        /// <summary>
        /// Hệ thống gọi đến hàm này khi association backgroundtask được bật
        /// </summary>
        /// <param name="taskInstance"> hệ thống tự tạo và truyền vào đây</param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //System.Diagnostics.Debug.WriteLine("background run");
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(BackgroundTaskCanceled);
            taskInstance.Task.Completed += new BackgroundTaskCompletedEventHandler(BackgroundTaskCompleted);
            _backgroundstarted.Set();//
            _deferral = taskInstance.GetDeferral();

            //Playlist = new BackgroundPlaylist();
            _smtc = initSMTC();


            this._foregroundState = this.initForegroundState();

            BackgroundMediaPlayer.Current.CurrentStateChanged += BackgroundMediaPlayer_CurrentStateChanged;
            //Playlist = await BackgroundPlaylist.LoadBackgroundPlaylist("playlist.xml");
            Playlist = new BackgroundPlaylist();
            Playlist.ListPathsource = await BackgroundPlaylist.LoadCurrentPlaylist(Constant.CurrentPlaylist);
            
            if (_foregroundState != eForegroundState.Suspended)
            {
                ValueSet message = new ValueSet();
                message.Add(Constant.BackgroundTaskStarted, "");
                BackgroundMediaPlayer.SendMessageToForeground(message);
            }  
            Playlist.TrackChanged += Playlist_TrackChanged;
            BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;
            BackgroundMediaPlayer.Current.MediaEnded +=Current_MediaEnded;
            ApplicationSettingHelper.SaveSettingsValue(Constant.BackgroundTaskState, Constant.BackgroundTaskRunning);
            isbackgroundtaskrunning = true;
            _loopState = eLoopState.None;
        }

        private void Playlist_TrackChanged(BackgroundPlaylist sender, object args)
        {
            if (this._foregroundState != eForegroundState.Suspended)
            {
                var msg = new ValueSet();
                msg.Add(Command.Titte, Playlist.Name);
                BackgroundMediaPlayer.SendMessageToForeground(msg);     
            }

        }

        private void Current_MediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            switch (this._loopState)
            {
                case eLoopState.None:
                    //sender.AutoPlay = false;
                    // or do nothing
                    break;
                case eLoopState.One:
                    sender.Position = TimeSpan.FromSeconds(0);
                    break;
                case eLoopState.All:
                    // trong play list đã tự động vòng rồi.
                    this.Playlist.Next();
                    this._smtc.DisplayUpdater.MusicProperties.Title = Playlist.Name;
                    this._smtc.DisplayUpdater.Update();
                    break;
                default:
                    break;
            }

        }


        private void BackgroundMediaPlayer_CurrentStateChanged(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            if (sender.CurrentState == MediaPlayerState.Playing)
            {
                this._smtc.PlaybackStatus = MediaPlaybackStatus.Playing;
            }
            else if (sender.CurrentState == MediaPlayerState.Paused)
            {
                this._smtc.PlaybackStatus = MediaPlaybackStatus.Paused;
            }
        }

        private void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("message received");
            foreach (string key in e.Data.Keys)
            {
                switch (key)
                {
                    case Constant.AppResumed:
                        this._foregroundState = eForegroundState.Active;
                        break;
                    case Constant.AppSuspended:
                        this._foregroundState = eForegroundState.Suspended;
                        break;
                    case Command.InitList:
                        System.Diagnostics.Debug.WriteLine("message received Init list");
                        //Playlist.ListPathsource.Add(e.Data[Command.InitList])
                        //var listtrack = (string[])e.Data[Command.InitList];
                        //Playlist.ListPathsource = listtrack;
                        break;
                    case Command.SetCurrentIndex:
                        var currentindex = Convert.ToInt32(e.Data[Command.SetCurrentIndex]);
                        Playlist.Currentindex = currentindex;
                        break;
                    case Command.PlayWithIndex:
                        //Playlist.Play();
                        Playlist.Currentindex = Int32.Parse(e.Data[Command.PlayWithIndex].ToString());
                        System.Diagnostics.Debug.WriteLine(Playlist.Currentindex);
                        BackgroundMediaPlayer.Current.SetUriSource(new Uri(Playlist.CurrentItem));
                        updatenewstmc();
                        break;
                    case Command.Pause:
                        BackgroundMediaPlayer.Current.Pause();
                        break;
                    case Command.Play:
                        BackgroundMediaPlayer.Current.Play();
                        break;
                    case Command.Shuffle:
                        Playlist.Shuffle();
                        break;
                    case Command.Next:
                        Playlist.Next();
                        BackgroundMediaPlayer.Current.SetUriSource(new Uri(Playlist.CurrentItem));
                        _smtc.DisplayUpdater.MusicProperties.Title = Playlist.Name;
                        _smtc.DisplayUpdater.Update();
                        break;
                    case Command.Previous:
                        Playlist.Previous();
                        BackgroundMediaPlayer.Current.SetUriSource(new Uri(Playlist.CurrentItem));
                        _smtc.DisplayUpdater.MusicProperties.Title = Playlist.Name;
                        _smtc.DisplayUpdater.Update();
                        break;
                    case Command.LoopState:
                        this._loopState = (eLoopState)Enum.Parse(typeof(eLoopState), e.Data[Command.LoopState].ToString());
                        break;
                }
            }
        }
        #region SMTC 
        
        private SystemMediaTransportControls initSMTC()
        {
            var temp = SystemMediaTransportControls.GetForCurrentView();
            temp.ButtonPressed += SMTC_ButtonPressed;
            temp.PropertyChanged += SMTC_PropertyChanged;

            // cung cấp quyền kích hoạt các nút của UVC
            temp.IsEnabled = true;
            temp.IsPauseEnabled = true;
            temp.IsPlayEnabled = true;
            temp.IsNextEnabled = true;
            temp.IsPreviousEnabled = true;
            return temp;
        }

        private void updatenewstmc()
        {
            this._smtc.PlaybackStatus = MediaPlaybackStatus.Playing;
            this._smtc.DisplayUpdater.Type = MediaPlaybackType.Music;
            this._smtc.DisplayUpdater.MusicProperties.Title = Playlist.Name;
            this._smtc.DisplayUpdater.Update();
        }

        private void SMTC_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {

            //throw new NotImplementedException();
        }

        private void SMTC_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            //throw new NotImplementedException();
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Next:
                    this.Playlist.Next();
                    BackgroundMediaPlayer.Current.SetUriSource(new Uri(Playlist.CurrentItem));
                    _smtc.DisplayUpdater.MusicProperties.Title = Playlist.Name;
                    _smtc.DisplayUpdater.Update();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    this.Playlist.Previous();
                    BackgroundMediaPlayer.Current.SetUriSource(new Uri(Playlist.CurrentItem));
                    _smtc.DisplayUpdater.MusicProperties.Title = Playlist.Name;
                    _smtc.DisplayUpdater.Update();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    BackgroundMediaPlayer.Current.Pause();
                    sender.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case SystemMediaTransportControlsButton.Play:
                    BackgroundMediaPlayer.Current.Play();
                    break;

            }
        }
        #endregion
        private void BackgroundTaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            this._deferral.Complete();
        }

        private void BackgroundTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //throw new NotImplementedException();
        }

        private eForegroundState initForegroundState()
        {
            var temp = ApplicationSettingHelper.ReadResetSettingsValue(Constant.AppState);
            if (temp == null)
                return eForegroundState.Unknown;
            return (eForegroundState)Enum.Parse(typeof(eForegroundState), temp.ToString());
        }
    }
}

using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Media;
using Windows.Media.Playback;

namespace MediaComponent
{
    // ref : https://code.msdn.microsoft.com/windowsapps/BackgroundAudio-63bbc319/sourcecode?fileId=111812&pathId=1324331438

    public class BackgroundTask : IBackgroundTask
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

        /// <summary>
        /// Hệ thống gọi đến hàm này khi association backgroundtask được bật
        /// </summary>
        /// <param name="taskInstance"> hệ thống tự tạo và truyền vào đây</param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            System.Diagnostics.Debug.WriteLine("bg RUN");
            _smtc = initSMTC();
            taskInstance.Canceled += BackgroundTaskCanceled;
            taskInstance.Task.Completed += BackgroundTaskCompleted;

            this._foregroundState = this.initForegroundState();
            
            // Playlist.TrackedChanged += Playlist_TrackChanged;
            BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;
            BackgroundMediaPlayer.Current.CurrentStateChanged +=BackgroundMediaPlayer_CurrentStateChanged;
            _backgroundstarted.Set();
            ApplicationSettingHelper.SaveSettingsValue(Constant.BackgroundTaskStarted, Constant.BackgroundTaskRunning);
            isbackgroundtaskrunning = true;
            _deferral = taskInstance.GetDeferral();
        }

        private void BackgroundMediaPlayer_CurrentStateChanged(Windows.Media.Playback.MediaPlayer sender, object args)
        {

        }

        private void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            foreach (var item in e.Data)
            {
                if (item.Key == "uri")
                {
                   // BackgroundMediaPlayer.Current.(e.Data["uri"] as string);
                }
            }
        }

        private void BackgroundTaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            //throw new NotImplementedException();
        }

        private void BackgroundTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //throw new NotImplementedException();
        }

        private SystemMediaTransportControls initSMTC()
        {
            var temp = SystemMediaTransportControls.GetForCurrentView();
            temp.ButtonPressed +=SMTC_ButtonPressed;              
            temp.PropertyChanged +=SMTC_PropertyChanged;

            // cung cấp quyền kích hoạt các nút của UVC
            temp.IsEnabled = true;
            temp.IsPauseEnabled = true;
            temp.IsPlayEnabled = true;
            temp.IsNextEnabled = true;
            temp.IsPreviousEnabled = true;
            return temp;
        }

        private void SMTC_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            
            //throw new NotImplementedException();
        }

        private void SMTC_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
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

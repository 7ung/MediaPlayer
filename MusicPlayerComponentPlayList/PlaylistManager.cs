using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Playback;

namespace MusicPlayerComponentPlayList
{
    //public sealed class PlaylistManager
    //{
    //    private static PlaylistManager instance;
    //    public Playlist playlist;
    //    public PlaylistManager Current
    //    {
    //        get
    //        {
    //            if (instance == null)
    //            {
    //                instance = new PlaylistManager();
    //            }
    //            return instance;
    //        }
    //    }

    //    public void ClearPlaylist()
    //    {
    //        instance = null;
    //    } 
    //}
    public sealed class Playlist
    {
        public String[] ListPathsource ;
        private int currentindex;
        public int Currentindex
        {
            get { return currentindex; }
            set {
                if (ListPathsource == null)
                {
                    currentindex = -1;
                }
                else
                {
                    currentindex = (value + 1) % ListPathsource.Length;
                }
            }
        }
        private MediaPlayer mediaplayer;
        private TimeSpan startPosition = TimeSpan.FromSeconds(0);
        pul Playlist()
        {
            mediaplayer = BackgroundMediaPlayer.Current;
            mediaplayer.MediaOpened +=mediaplayer_MediaOpened;
            mediaplayer.MediaEnded +=mediaplayer_MediaEnded;
            mediaplayer.CurrentStateChanged +=mediaplayer_CurrentStateChanged;
            mediaplayer.MediaFailed +=mediaplayer_MediaFailed;
            mediaplayer.AutoPlay = false;
        }

        //public string CurrentTrackName
        //{
        //    get
        //    {
        //        if (currentindex == -1)
        //        {
        //            return String.Empty;
        //        }
        //        if (currentindex < Track.Count)
        //        {
        //            string fullUrl = Track[currentindex];
        //            return fullUrl.Split('/')[fullUrl.Split('/').Length - 1];
        //        }
        //        else
        //            throw new ArgumentOutOfRangeException("Track Id is higher than total number of tracks");
        //    }
        //}
        public event TypedEventHandler<Playlist, object> TrackChanged;

        private void mediaplayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
         //   throw new NotImplementedException();
        }

        private void mediaplayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            if (sender.CurrentState == MediaPlayerState.Playing && startPosition != TimeSpan.FromSeconds(0))
            {
                // if the start position is other than 0, then set it now
                sender.Position = startPosition;
                sender.Volume = 1.0;
                startPosition = TimeSpan.FromSeconds(0);
                sender.PlaybackMediaMarkers.Clear();
            }
        }

        private void mediaplayer_MediaEnded(MediaPlayer sender, object args)
        {
        }

        private void mediaplayer_MediaOpened(MediaPlayer sender, object args)
        {
            // wait for media to be ready
            sender.Play();
           // Debug.WriteLine("New Track" + this.CurrentTrackName);
            //TrackChanged.Invoke(this, CurrentTrackName);
        }

        public void Play()
        {
            this.mediaplayer.SetUriSource(new Uri(ListPathsource[Currentindex]));
        }
    }
}

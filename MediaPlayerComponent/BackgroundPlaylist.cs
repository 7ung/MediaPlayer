using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;
namespace MediaPlayerComponent
{
    class BackgroundPlaylist
    {
        private List<string> listpahtsource ;
        private int currentindex;
        public List<string> ListPathsource
        {
            get { return listpahtsource; }
            set { listpahtsource = value; }
        }
        public int Currentindex
        {
            get { return currentindex; }
            set
            {
                if (currentindex != value)
                {
                    if (ListPathsource == null)
                    {
                        currentindex = -1;
                    }
                    else if (value < 0)
                    {
                        currentindex = ListPathsource.Count - 1;
                    }
                    else if (value >= ListPathsource.Count)
                    {
                        currentindex = 0;
                    }
                    else
                        currentindex = value;
                    TrackChanged(this, currentindex.ToString());
                }

            }
        }
        public string CurrentItem
        {
            get{
                if (Currentindex < 0)
                {
                    return null;   
                }
                return ListPathsource[Currentindex];}
        }
        public string Name
        {
            get
            {
                string temp = CurrentItem;
                int i = temp.Split('\\').Length - 1;
                temp = temp.Split('\\')[i];
                if (temp.Contains(".mp3"))
                {
                     temp= temp.Trim(".mp3".ToArray());
                }
                return temp;
            }
        }
        private Windows.Media.Playback.MediaPlayer mediaplayer;
        
        public BackgroundPlaylist()
        {
            ListPathsource = new List<string>();
            mediaplayer = BackgroundMediaPlayer.Current;
            mediaplayer.MediaOpened += mediaplayer_MediaOpened;
            mediaplayer.MediaEnded += mediaplayer_MediaEnded;
            mediaplayer.CurrentStateChanged += mediaplayer_CurrentStateChanged;
            mediaplayer.MediaFailed += mediaplayer_MediaFailed;
            mediaplayer.AutoPlay = false;
        }
        public void Next()
        {
            Currentindex++;
        }

        /// <summary>
        /// Bài hát trước
        /// </summary>
        public void Previous()
        {
            Currentindex--;
        }
        public event TypedEventHandler<BackgroundPlaylist, object> TrackChanged;

        private void mediaplayer_MediaFailed(Windows.Media.Playback.MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            //   throw new NotImplementedException();
        }

        private void mediaplayer_CurrentStateChanged(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            if (sender.CurrentState == MediaPlayerState.Playing)
            {
                // if the start position is other than 0, then set it now
                sender.Volume = 1.0;
                sender.PlaybackMediaMarkers.Clear();
            }
        }

        private void mediaplayer_MediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
        }

        private void mediaplayer_MediaOpened(Windows.Media.Playback.MediaPlayer sender, object args)
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
        public void Shuffle()
        {
            int tempA = 0;
            int tempB = 0;
            Random rand = new Random();
            for (int i = 0; i < ListPathsource.Count; i++)
            {
                tempA = rand.Next(0, ListPathsource.Count - 1);
                tempB = rand.Next(0, ListPathsource.Count - 1);

                // swap
                var temp = ListPathsource[tempA];
                ListPathsource[tempA] = ListPathsource[tempB];
                ListPathsource[tempB] = temp;
            }
            currentindex = 0;
        }

        public static async Task<BackgroundPlaylist> LoadBackgroundPlaylist(string path)
        {
            var file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync(path);
            Stream stream = await file.OpenStreamForReadAsync();
            List<string> filepath = null;
            int currentindex= -1;

            using (XmlReader reader = XmlReader.Create(stream))
            {
                //reader.ReadStartElement("Playlist");
                reader.Read();
                while (reader.NodeType != XmlNodeType.EndElement || reader.Name != "Playlist")
                {
                    reader.Read();

                    if (reader.IsStartElement("CurrentIndex"))
                    {
                        reader.Read();
                        currentindex = Int32.Parse(reader.Value);
                        continue;
                    }
                    if (reader.IsStartElement("List"))
                    {
                        filepath = readFilelist(reader);
                    }
                }
            }
            stream.Dispose();
            BackgroundPlaylist bgPlaylist = new BackgroundPlaylist();
            bgPlaylist.ListPathsource = filepath;
            bgPlaylist.Currentindex = currentindex;
            return bgPlaylist;
        }

        private static List<string> readFilelist(XmlReader reader)
        {
            //reader.ReadStartElement("List");
            //reader.Read();
            List<string> filepath = new List<string>();
            while (reader.NodeType != XmlNodeType.EndElement || reader.Name != "List")
            {
                reader.Read();
                if (reader.IsStartElement("Path"))
                {
                    reader.Read();
                    filepath.Add(reader.Value);
                }
            }
            return filepath;
        }

        public static async Task<List<string>> LoadCurrentPlaylist(string path)
        {
            List<string> listpath = new List<string>();
            StorageFile file;
            try
            {
                file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync(path);
            }
            catch
            {
                return null;
            }
            if (file == null)
                return null;
            Stream stream = await file.OpenStreamForReadAsync();
            using (XmlReader reader = XmlReader.Create(stream))
            {
                //reader.ReadStartElement("CurrentPlaylist");
                reader.Read();
                while (reader.NodeType != XmlNodeType.EndElement || reader.Name != "CurrentPlaylist")
                {
                    reader.Read();
                    if (reader.IsStartElement("Path"))
                    {
                        reader.Read();
                        listpath.Add(reader.Value);
                        continue;
                    }
                }
            }
            stream.Dispose();
            return listpath;
        }
    }
}

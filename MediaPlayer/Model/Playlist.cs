using MediaPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;

namespace MediaPlayer.Model
{
    public sealed class Playlist : INotifyCollectionChanged, INotifyPropertyChanged
    {
        //static private Windows.Media.Playback.MediaPlayer mediaplayer;
        private ObservableCollection<FilesViewModel> _listfile;

        private int _currentIndex;
        private FilesViewModel _currentItem;


        /// <summary>
        /// Thông tin bài hát hiện tại
        /// </summary>
        public FilesViewModel CurrentItem
        {
            get { return _currentItem; }
            private set { setProperty(ref _currentItem, value, "CurrentItem"); }
        }

        /// <summary>
        /// Chỉ số bài hát hiện tại
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { setProperty(ref _currentIndex, value, "CurrentIndex"); }
        }

        /// <summary>
        /// Tổng số bài hát
        /// </summary>
        public int Count { get { return ListFile.Count; } }

        /// <summary>
        /// Fired when any property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fired when any list or collection changed
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public Playlist(IList<FilesViewModel> listfile, int currentitem = 0)
        {
            ListFile = new ObservableCollection<FilesViewModel>(listfile.ToArray());
            ListFile.CollectionChanged += OnChanged;
            PropertyChanged +=Playlist_PropertyChanged;
            CurrentIndex = currentitem;
            CurrentItem = ListFile[CurrentIndex];


        }

        private void Playlist_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentIndex")
            {
                if (CurrentIndex >= Count)
                {
                    CurrentIndex = 0;
                }
                if (CurrentIndex < 0)
                {
                    CurrentIndex = Count - 1;
                }
                this.CurrentItem = ListFile[CurrentIndex];
            }
        }
        private void OnChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(sender, e);
            }
        }
        private void setProperty<T>(ref T obj, T value, string propertyName)
        {
            if (Object.Equals(obj, value))
                return;
            obj = value;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// List bài hát
        /// </summary>
        public ObservableCollection<FilesViewModel> ListFile
        {
            get { return _listfile; }
            set { _listfile = value; }
        }
        
        /// <summary>
        /// Bài hát kế tiếp
        /// </summary>
        public void Next()
        {
            CurrentIndex++;
        }

        /// <summary>
        /// Bài hát trước
        /// </summary>
        public void Previous()
        {
            CurrentIndex--;
        }

        /// <summary>
        /// Trộn
        /// </summary>
        public void Shuffle()
        {
            int tempA = 0;
            int tempB = 0;
            Random rand = new Random();
            for (int i = 0; i < Count; i++)
            {
                tempA = rand.Next(0, Count - 1);
                tempB = rand.Next(0, Count - 1);

                // swap
                var temp = ListFile[tempA];
                ListFile[tempA] = ListFile[tempB];
                ListFile[tempB] = temp;
            }
        }
    }
}

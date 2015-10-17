using MediaPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.Model
{
    class Playlist : INotifyCollectionChanged, INotifyPropertyChanged
    {
        public Playlist(IList<FilesViewModel> listfile, int currentitem)
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
        private ObservableCollection<FilesViewModel> _listfile;
        private int _currentIndex;
        private FilesViewModel _currentItem;

        public FilesViewModel CurrentItem
        {
            get { return _currentItem; }
            private set { setProperty(ref _currentItem, value, "CurrentItem"); }
        }
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { setProperty(ref _currentIndex, value, "CurrentIndex"); }
        }
        public int Count { get { return ListFile.Count; } }
        public ObservableCollection<FilesViewModel> ListFile
        {
            get { return _listfile; }
            set { _listfile = value; }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        
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

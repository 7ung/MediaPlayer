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
        public Playlist(IList<FilesViewModel> listfile, int currentitem = 0)
        {
            CurrentItem = currentitem;
            ListFile = new ObservableCollection<FilesViewModel>(listfile.ToArray());
            ListFile.CollectionChanged += OnChanged;
            PropertyChanged +=Playlist_PropertyChanged;
        }

        private void Playlist_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentItem")
            {
                if (CurrentItem >= Count)
                {
                    CurrentItem = 0;
                }
                if (CurrentItem <= 0)
                {
                    CurrentItem = Count - 1;
                }
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
        private int _currentItem;

        public int CurrentItem
        {
            get { return _currentItem; }
            set { setProperty(ref _currentItem, value, "CurrentItem"); }
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
            CurrentItem++;
        }
        public void Previous()
        {
            CurrentItem--;
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

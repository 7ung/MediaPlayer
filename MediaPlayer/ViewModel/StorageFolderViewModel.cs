using System;
using System.Collections.Generic;
using System.ComponentModel;       //for INotifyProertyChanged
using System.Linq;
using System.Runtime.CompilerServices;      // for CallerMemberName
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;// for storageFolder

namespace MediaPlayer.ViewModel
{
    public class StorageFolderViewModel : INotifyPropertyChanged, IComparable
    {
        private StorageFolder _folder;

        public StorageFolder Folder
        {
            get { return _folder; }
            set {
                SetProperty(ref _folder, value, "Folder");
            }
        }
        public StorageFolderViewModel(StorageFolder folder)
        {
            Folder = folder;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged(string propertyName)
        {
            var eventHanlder = this.PropertyChanged;
            if (eventHanlder != null)
                eventHanlder(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(object obj)
        {
            return String.Compare(this.Folder.Path, (obj as StorageFolderViewModel).Folder.Path);
        }
    }
}

using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MediaPlayer.ViewModel
{
    public class FilesViewModel : INotifyPropertyChanged, IComparable
    {
        private StorageFile _file;
        private string _album;
        private string _artist;
        private string _albumartist;
        private MusicProperties _musicproperties;
        private string _name;
        private string _title;
        private TimeSpan _duration;

        public TimeSpan Duration
        {
            get { return _duration; }
            set { SetProperty(ref _duration, value, "Duration"); }
        }
        public string Title
        {
            get { return _title; }
            set {
                SetProperty(ref _title, value, "Title");
            }
        }
        public string AlbumArtist
        {
            get { return _albumartist; }
            set { SetProperty(ref _albumartist, value, "AlbumArtist"); }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value, "Name");
            }
        }
        public string Artist
        {
            get { return _artist; }
            set { SetProperty(ref _artist, value, "Artist"); }
        }
        public string Album
        {
            get { return _album; }
            set { SetProperty(ref _album, value, "Album"); }
        }
        public StorageFile File
        {
            get { return _file; }
            set { 
                SetProperty(ref _file, value, "File"); 
            }
        }
        public MusicProperties Musicproperties
        {
            get { return _musicproperties; }
            set
            {
                SetProperty(ref _musicproperties, value, "MusicProperties");
                Title = (String.IsNullOrEmpty(value.Title)) ? File.DisplayName : value.Title;
                Artist = (String.IsNullOrEmpty(value.Artist)) ? "Unknown Artist" : value.Artist;
                Album = (String.IsNullOrEmpty(value.Album)) ? "Unknow Album" : value.Album;
                AlbumArtist = (String.IsNullOrEmpty(value.AlbumArtist)) ? "Unknow AlbumArtist" : value.AlbumArtist;
                Duration = value.Duration;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FilesViewModel(StorageFile file, MusicProperties properties)
        {
            File = file;
            Name = file.Name;
            Musicproperties = properties;
        }
        
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
            return String.Compare(this.File.Path, (obj as FilesViewModel).File.Path);
        }
    }

}

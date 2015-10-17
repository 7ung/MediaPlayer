using MediaPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;   // for ObservableCollection
using System.Collections.Specialized;   // for INotifyCollestionChanged
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MediaPlayer.Model
{
    public class FolderTracker : INotifyCollectionChanged
    {
        static private List<string> _listKnowfolder = new List<string>();
        private StorageFolder _sdCard;
        private StorageFolder _musicFolder;
        private ObservableCollection<StorageFolderViewModel> _folder;
        private ObservableCollection<FilesViewModel> _allfiles;

        public ObservableCollection<FilesViewModel> AllFiles
        {
            get { return _allfiles; }
            set {
                _allfiles = value;
            }
        }

        public ObservableCollection<StorageFolderViewModel> Folder
        {
            get { return _folder; }
            set
            {
                _folder = value;
            }
        }
        public FolderTracker()
        {
            Folder = new ObservableCollection<StorageFolderViewModel>();
            AllFiles = new ObservableCollection<FilesViewModel>();

            _musicFolder = KnownFolders.MusicLibrary;
            _sdCard = KnownFolders.RemovableDevices;
            _listKnowfolder.Add("Music");
            _listKnowfolder.Add("Videos");
            _listKnowfolder.Add("Pirtures");
            _listKnowfolder.Add("Downloads");
            _listKnowfolder.Add("Documents");
            _listKnowfolder.Add("Saved Pictures");
            _listKnowfolder.Add("Camera Roll");
            AllFiles.CollectionChanged +=this.CollectionChanged;
        }


        public event NotifyCollectionChangedEventHandler CollectionChanged;
        
        public async void fetchStorageInfo()
        {
            this.fetchFolders(_musicFolder);         //tìm file trong thư mục music library
            _sdCard = (await _sdCard.GetFoldersAsync()).FirstOrDefault();   // truy cập sd card
            var userfolders = await _sdCard.GetFoldersAsync();
            foreach (var folder in userfolders)
            {
                if (_listKnowfolder.Contains(folder.DisplayName) == true)       // bỏ qua các knownfolders
                    continue;
                fetchFolders(folder);
            }
        }
        public async void fetchFolders(StorageFolder parentFolder)
        {
            fetchFiles(parentFolder);
            var subfolders = await parentFolder.GetFoldersAsync();
            foreach (var item in subfolders)
            {
                Folder.Add(new StorageFolderViewModel(item));
                if (CollectionChanged != null)
                    this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                fetchFolders(item);
            }
        }

        private async void fetchFiles(StorageFolder _folder)
        {
            var subitems = await _folder.GetFilesAsync();
            foreach (var file in subitems)
            {
                if (file.ContentType != "audio/mpeg")
                    continue; 
                var properties = await file.Properties.GetMusicPropertiesAsync();

                //System.Diagnostics.Debug.WriteLine(artist);
                FilesViewModel viewmodel = new FilesViewModel(file);
                viewmodel.Musicproperties = properties;
                AllFiles.Add(viewmodel);
            }            
        }
    }
}

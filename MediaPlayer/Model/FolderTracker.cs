﻿using MediaPlayer.ViewModel;
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
using Windows.Storage.Search;

namespace MediaPlayer.Model
{
    public class FolderTracker: INotifyCollectionChanged
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
            Folder.CollectionChanged +=OnChanged;
            AllFiles.CollectionChanged += OnChanged;
            _musicFolder = KnownFolders.MusicLibrary;
            _sdCard = KnownFolders.RemovableDevices;
            _listKnowfolder.Add("Music");
            _listKnowfolder.Add("Videos");
            _listKnowfolder.Add("Pirtures");
            _listKnowfolder.Add("Downloads");
            _listKnowfolder.Add("Documents");
            _listKnowfolder.Add("Saved Pictures");
            _listKnowfolder.Add("Camera Roll");
        }

        private void OnChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(sender, e);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public async Task fetchStorageInfo()
        {
            // avoid async void. return Task instead.
            await this.fetchFolders(_musicFolder);         //tìm file trong thư mục music library
            _sdCard = (await _sdCard.GetFoldersAsync()).FirstOrDefault();   // truy cập sd card
            var userfolders = await _sdCard.GetFoldersAsync();
            foreach (var folder in userfolders)
            {
                if (_listKnowfolder.Contains(folder.DisplayName) == true)       // bỏ qua các knownfolders
                    continue;
                await fetchFolders(folder);
            }
        }
        public async Task fetchFolders(StorageFolder parentFolder)
        {
            // avoid async void. return Task instead
            await fetchFiles(parentFolder);
            var subfolders = await parentFolder.GetFoldersAsync();
            foreach (var item in subfolders)
            {
                Folder.Add(new StorageFolderViewModel(item));
                await fetchFolders(item);
            }
        }

        private async Task fetchFiles(StorageFolder _folder)
        {
            // avoid asyn void. return Task instead
            var subitems = await _folder.GetFilesAsync();
            foreach (var file in subitems)
            {
                if (file.ContentType != "audio/mpeg")
                    continue; 
                var properties = await file.Properties.GetMusicPropertiesAsync();
                FilesViewModel viewmodel = new FilesViewModel(file, properties);
                AllFiles.Add(viewmodel);
            }            
        }


    }
}

using MediaPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MediaPlayer.Model
{
    /*
     *cấu trúc file xml
     *<Docutment/>
     * <Playlist> 
     *          <CurrentIndex>
     *          </CurrentIndex>
     *          <List>
     *              <Path>
     *              </Path>     
     *              <Path>
     *              </Path>
     *              ....
     *              ....
     *              <Path>
     *              </Path>
     *             
     *          </List>
     * </Playlist>
     */
    class CacheAccess
    {
        private static StorageFolder _localecachefoler;
        public static StorageFolder LocalCacheFolder
        {
            get
            {
                if (_localecachefoler == null)
                    _localecachefoler = ApplicationData.Current.LocalCacheFolder;
                return _localecachefoler;
            }
        }

        public static async Task SaveToCache(string path, Playlist playlist)
        {
            StorageFile file = null;
            bool isFileExists = false;
            try
            {
                file = await LocalCacheFolder.GetFileAsync(path);
                isFileExists = (file != null);
            }
            catch
            {
                isFileExists = false;
            }
            if (isFileExists == false)
            {
                file = await LocalCacheFolder.CreateFileAsync(path);
            }
            Stream stream = await file.OpenStreamForWriteAsync();
            using (XmlWriter wr = XmlWriter.Create(stream))         // using sẽ tự dispose
            {
                wr.WriteStartDocument();
                wr.WriteStartElement("Playlist");
                {

                    wr.WriteElementString("CurrentIndex", playlist.CurrentIndex.ToString());
                    wr.WriteStartElement("List");
                    {
                        foreach (var fileitem in playlist.ListFile)
                        {
                            wr.WriteElementString("Path", fileitem.File.Path);
                        }
                    }
                    wr.WriteEndElement();
                }
                wr.WriteEndElement();
                wr.WriteEndDocument();
            }
        }

        public static async Task<Playlist> LoadFromCache(string path)
        {
            
            StorageFile file;
            try
            {
                file = await LocalCacheFolder.GetFileAsync(path);
            }
            catch
            {
                return null;
            }
            if (file == null)
                return null;
            Stream stream = await file.OpenStreamForReadAsync();
            List<string> filepath = null;
            int currentitem = -1;
            List<FilesViewModel> listfile = new List<FilesViewModel>();

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
                        currentitem = Int32.Parse(reader.Value);
                        continue;
                    }
                    if (reader.IsStartElement("List"))
                    {
                        filepath = readFilelist(reader);
                    }
                }
            }
            
            StorageFile storagefile;
            MusicProperties musicproperties;
            foreach (var item in filepath)
            {
                storagefile = await StorageFile.GetFileFromPathAsync(item);
                musicproperties = await storagefile.Properties.GetMusicPropertiesAsync();
                listfile.Add(new FilesViewModel(storagefile, musicproperties));
            }
            return new Playlist(listfile, currentitem);

        }


        // đọc elem <List> </List
        private static List<string> readFilelist(XmlReader reader)
        {
           // reader.ReadStartElement("List");
            List<string> filepath = new List<string>();
            //reader.Read();
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


        //--------------------------------------------------------------------------------
        public static async Task SaveCurrentPlaylist(string path, Playlist playlist)
        {
            StorageFile file = null;
            bool isFileExists = false;
            try
            {
                file = await LocalCacheFolder.GetFileAsync(path);
                isFileExists = (file != null);
            }
            catch
            {
                isFileExists = false;
            }
            if (isFileExists == false)
            {
                file = await LocalCacheFolder.CreateFileAsync(path);
            }

            Stream stream = await file.OpenStreamForWriteAsync();   // open stream
            using (XmlWriter wr = XmlWriter.Create(stream))         // using sẽ tự dispose
            {
                wr.WriteStartDocument();
                wr.WriteStartElement("CurrentPlaylist");
                {
                    foreach (var fileitem in playlist.ListFile)
                    {
                        wr.WriteElementString("Path", fileitem.File.Path);
                    }
                }
                wr.WriteEndElement();
                wr.WriteEndDocument();
            }
            stream.Dispose();
        }
        public static async Task<List<string>> LoadCurrentPlaylist(string path)
        {
            StorageFile file;
            try
            {
                file = await LocalCacheFolder.GetFileAsync(path);
            }
            catch
            {
                return null;
            }
            if (file == null)
                return null;
            Stream stream = await file.OpenStreamForReadAsync();
            List<string> listpath = new List<string>();
            using (XmlReader reader = XmlReader.Create(stream))
            {
                reader.ReadStartElement("CurrentPlaylist");
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

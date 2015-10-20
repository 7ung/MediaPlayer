using MediaPlayer.Common;
using MediaPlayer.Model;
using MediaPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MediaPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool isLoaded;
        public FolderTracker Tracker { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            Tracker = new FolderTracker();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Application.Current.Exit();
            }
        }



        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
            // to access sd card, must add association to manifest
            // http://stackoverflow.com/questions/24416244/how-do-i-add-a-file-type-association-in-a-windows-phone-8-1-app-manifest

            if (isLoaded == false)
            {
                await Tracker.fetchStorageInfo();
                // old
                //this.setListBinding(this.allmusic, Tracker.AllFiles.OrderBy(file => file.Title, StringComparer.OrdinalIgnoreCase).ToList());
                //this.setListBinding(this.artistcategory, Tracker.AllFiles.Select(file => file.Artist).Distinct().OrderBy(str => str, StringComparer.OrdinalIgnoreCase));
                //this.setListBinding(this.albumcategory, Tracker.AllFiles.Select(file => file.Album).Distinct().OrderBy(str => str, StringComparer.OrdinalIgnoreCase));
                //this.setListBinding(this.albumcartistategory, Tracker.AllFiles.Select(file => file.AlbumArtist).Distinct().OrderBy(str =>str, StringComparer.OrdinalIgnoreCase));
                
                // all item
                var allItems = AlphaKeyGroup<FilesViewModel>.CreatGroups(
                        Tracker.AllFiles,
                        file => file.Title, true);
                allItemsGrouped.Source = allItems;

                // artist
                var artist = AlphaKeyGroup<string>.CreatGroups(
                        Tracker.AllFiles.Select(file => file.Artist).Distinct(),
                        file => file, true);
                artistItemsGrouped.Source = artist;

                // album
                var album = AlphaKeyGroup<string>.CreatGroups(
                        Tracker.AllFiles.Select(file => file.Album).Distinct(),
                        file => file, true);
                albumItemsGrouped.Source = album;

                // album artist
                var albumArtist = AlphaKeyGroup<string>.CreatGroups(
                       Tracker.AllFiles.Select(file => file.AlbumArtist).Distinct(),
                       file => file, true);
                albumArtistItemsGrouped.Source = albumArtist;

                // load done!
                isLoaded = true;
                loadingProgress.IsActive = false;
                loadingProgress.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        ///gán bind từ một observe list vào một list control (Gridview, listview, listbox)
        /// </summary>
        private void setListBinding<T>(Selector selector, IEnumerable<T> observeList)
        {
            selector.ItemsSource = observeList;
        }

        private void allmusic_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickeditem = (e.ClickedItem as FilesViewModel);                // bài hát được click
            // bind kiểu CollectionViewSource cái này ko lấy được
            //var listfile = allmusic.ItemsSource as IList<FilesViewModel>;       // danh sách các bài hát
            //int index = listfile.IndexOf(e.ClickedItem as FilesViewModel);      // zero-base index bài hát được chọn

            int index = Tracker.AllFiles.IndexOf(e.ClickedItem as FilesViewModel);

            Playlist playlist = new Playlist(Tracker.AllFiles, index);
            Frame.Navigate(typeof(SelectPage), playlist);
        }

        private void artistcategory_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = Tracker.AllFiles.Where(file => file.Artist == (e.ClickedItem as string)).ToList();
            Playlist playlist = new Playlist(item);
            Frame.Navigate(typeof(SelectPage), playlist);
        }

        private void albumcategory_ItemClick(object sender, ItemClickEventArgs e)
        {
            string clickeditem = e.ClickedItem as string;                                       // tên album được chọn
            var album = Tracker.AllFiles.Where(file => file.Album == clickeditem).ToList();     // lọc ra các bài hát từ album
            Playlist playlist = new Playlist(album);
            Frame.Navigate(typeof(SelectPage), playlist);
        }

        private void albumartistcategory_ItemClick(object sender, ItemClickEventArgs e)
        {
            string clickeditem = e.ClickedItem as string;                               // tên album artist được chhọn
            var album = Tracker.AllFiles.Where(file => file.AlbumArtist == clickeditem).ToList();
            Playlist playlist = new Playlist(album);
            Frame.Navigate(typeof(SelectPage), playlist);
        }

    }
}

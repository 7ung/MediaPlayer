using MediaPlayer.Model;
using MediaPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
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
        public FolderTracker Tracker { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            Tracker = new FolderTracker();
            this.NavigationCacheMode = NavigationCacheMode.Required;

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            
            // to run followwed statement, must add association to manifest
            // http://stackoverflow.com/questions/24416244/how-do-i-add-a-file-type-association-in-a-windows-phone-8-1-app-manifest
            //var sdcard = (await KnownFolders.RemovableDevices.GetFoldersAsync()).FirstOrDefault();
            //var allfolder = await sdcard.GetFoldersAsync();
            //foreach (var folder in allfolder)
            //{
            //    Tracker.LoadLocalFolder(folder);
            //}
            Tracker.fetchStorageInfo();
            this.musicCategory.ItemsSource = Tracker.Folder;            // bind folder vô listitem musiccategory
            this.allmusic.ItemsSource = Tracker.AllFiles;               // bind files vô listitem allmusic
                //this.allmusic.ItemsSource  = AlphaKeyGroup<FilesViewModel>.CreatGroups(
                //    Tracker.AllFiles.ToList(), file => file.File.DisplayName, true);
            
        }

        private void allmusic_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (e.ClickedItem as FilesViewModel);
            Frame.Navigate(typeof(SelectPage), item);
        }
    }
}

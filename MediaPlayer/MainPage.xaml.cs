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

            await Tracker.fetchStorageInfo();
            this.setListBinding(this.allmusic, Tracker.AllFiles.OrderBy(file => file.Title).ToList());
            this.artistcategory.ItemsSource = Tracker.AllFiles.Select(file => file.Artist).Distinct();
            
        }

        /// <summary>
        ///gán bind từ một observe list vào một list control (Gridview, listview, listbox)
        /// </summary>
        private void setListBinding<T>(Selector selector, IList<T> observeList)
        {
            selector.ItemsSource = observeList;
        }

        private void allmusic_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (e.ClickedItem as FilesViewModel);
            Frame.Navigate(typeof(SelectPage), item);
        }

        private void artistcategory_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = Tracker.AllFiles.Where(file => file.Artist == (e.ClickedItem as string)).First();
            Frame.Navigate(typeof(SelectPage), item);
        }

    }
}

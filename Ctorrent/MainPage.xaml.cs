using BencodeNET.Objects;
using BencodeNET.Parsing;
using BencodeNET.Torrents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace Ctorrent
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<Torrent> download { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            download = new ObservableCollection<Torrent>();
        }

        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            picker.FileTypeFilter.Add(".torrent");
            
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                Debug.WriteLine(file.Name + " picked");
                Thread thread1 = new Thread(new ParameterizedThreadStart(AddFile));
                thread1.Start(file);
            }
            else
            {
                Debug.WriteLine("error while picking");
            }
        }

        private async void ChooseDownloadLocation_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                    FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                Debug.WriteLine(folder.Path + " picked");
            }
            else
            {
                Debug.WriteLine("error while picking download folder");
            }
        }

        private async void AddFile(object obj)
        {
            Windows.Storage.StorageFile file = obj as Windows.Storage.StorageFile;
            var fs = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            //var fs = File.OpenRead(file.Path);
            var parser = new BencodeParser();
            var torrent = parser.Parse<Torrent>(fs.AsStreamForRead());
            this.download.Add(torrent);
        }

        private void DownloadTorrent(Torrent torrent)
        {

        }
    }
}

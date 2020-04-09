﻿using DownloadExample;
using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;
using System;
using System.IO;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private BackgroundDownloader backgroundDownloader = new BackgroundDownloader();

        private string path;
        private Downloader foo;

        public MainPage()
        {
            this.InitializeComponent();

            //FolderPicker folderPicker = new FolderPicker();
            //folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            //folderPicker.ViewMode = PickerViewMode.Thumbnail;
            //folderPicker.FileTypeFilter.Add("*");
            //StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            // TODO: Find a way to set a custom destination ... I always got a permission-denied-exception ...
            // Maybe possible in combination with https://msdn.microsoft.com/library/windows/apps/br207457
            // See: https://docs.microsoft.com/en-us/windows/uwp/files/file-access-permissions
            //CrossDownloadManager.Current.PathNameForDownloadedFile = new Func<IDownloadFile, string> (file => path);

            foo = new Downloader();
        }

        public void Download()
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            path = Path.Combine(folder.Path, "data_200mb.test");

            // If already downloading, abort it.
            if (foo.IsDownloading())
            {
                foo.AbortDownloading();
                downloadBtn.Content = "Download aborted.";
                return;
            }

            downloadBtn.Content = "Start downloading ...";

            foo.InitializeDownload();

            foo.File.PropertyChanged += (sender, e) =>
            {
                // System.Diagnostics.Debug.WriteLine("[Property changed] " + e.PropertyName + " -> " + sender.GetType().GetProperty(e.PropertyName).GetValue(sender, null).ToString());

                // Update UI text-fields
                var downloadFile = ((IDownloadFile)sender);
                switch (e.PropertyName)
                {
                    case nameof(IDownloadFile.Status):
                        Statustext.Text = downloadFile.Status.ToString();
                        break;

                    case nameof(IDownloadFile.TotalBytesExpected):
                        BytesExpected.Text = (downloadFile.TotalBytesExpected / 1000).ToString();
                        break;

                    case nameof(IDownloadFile.TotalBytesWritten):
                        BytesReceived.Text = (downloadFile.TotalBytesWritten / 1000).ToString();
                        break;
                }

                // Update UI if download-status changed.
                if (e.PropertyName == "Status")
                {
                    switch (((IDownloadFile)sender).Status)
                    {
                        case DownloadFileStatus.COMPLETED:
                        case DownloadFileStatus.FAILED:
                        case DownloadFileStatus.CANCELED:
                            downloadBtn.Content = "Downloading finished.";
                            ((DownloadFileImplementation)downloadFile).DownloadOperation.ResultFile.DeleteAsync();
                            break;
                    }
                }

                // Update UI while donwloading.
                if (e.PropertyName == "TotalKBytesWritten" || e.PropertyName == "TotalKBytesExpected")
                {
                    var bytesExpected = ((IDownloadFile)sender).TotalBytesExpected;
                    var bytesWritten = ((IDownloadFile)sender).TotalBytesWritten;

                    if (bytesExpected > 0)
                    {
                        var percentage = Math.Round(bytesWritten / bytesExpected * 100);
                        downloadBtn.Content = "Downloading (" + percentage + "%)";
                    }
                }
            };

            foo.StartDownloading(CellularNetworkAllowed.IsChecked.HasValue && CellularNetworkAllowed.IsChecked.Value);
        }

        private void downloadBtn_Click(object sender, RoutedEventArgs e)
        {
            Download();
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }
    }
}
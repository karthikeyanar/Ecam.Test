using CefSharp;

namespace TorBrowser
{
    public class DownloadHandler : IDownloadHandler
    {
        MainForm myForm;

        public DownloadHandler(MainForm form)
        {
            myForm = form;
        }

        public void OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            string download_path = System.Configuration.ConfigurationManager.AppSettings["download_path"];
            if (!callback.IsDisposed)
            {
                using (callback) {
                    myForm.UpdateDownloadItem(downloadItem);
                    callback.Continue(System.IO.Path.Combine(download_path,downloadItem.SuggestedFileName), showDialog: false);
                }
            }
        }

        public void OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            myForm.UpdateDownloadItem(downloadItem);
            if (downloadItem.IsInProgress && myForm.CancelRequests.Contains(downloadItem.Id)) callback.Cancel();
            //Console.WriteLine(downloadItem.Url + " %" + downloadItem.PercentComplete + " complete");
        }
    }
}

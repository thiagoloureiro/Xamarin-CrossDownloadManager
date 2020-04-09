﻿using Foundation;
using Plugin.DownloadManager;

namespace DownloadExample.iOS
{
    // This class let you do interact with the downloading tasks.
    // See https://developer.apple.com/library/ios/documentation/Foundation/Reference/NSURLSessionDownloadDelegate_protocol/index.html
    public class ExtendedUrlSessionDownloadDelegate : UrlSessionDownloadDelegate
    {
        public override void DidCompleteWithError(Foundation.NSUrlSession session, Foundation.NSUrlSessionTask task, Foundation.NSError error)
        {
            // Add a breakpoint here if you encounter any errors.
        }

        public override void DidFinishEventsForBackgroundSession(Foundation.NSUrlSession session)
        {
            // If you want to notify the users, that all files are downloaded, do it here - before the base-method is called.

            base.DidFinishEventsForBackgroundSession(session);
        }

        public override void DidFinishDownloading(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
        {
            // In case you need to access the IDownloadFile implementation, you have to load it before calling the base-method.
            var file = GetDownloadFileByTask(downloadTask);
            if (file == null)
            {
                return;
            }

            // This base-method sets the state to "COMPLETED" and moves the file if `PathNameForDownloadedFile` is set.
            base.DidFinishDownloading(session, downloadTask, location);

            // If you don't set `PathNameForDownloadedFile`, you can do what you want with the file now.
            System.Diagnostics.Debug.WriteLine(location.AbsoluteString);
        }
    }
}
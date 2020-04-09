using Android.App;
using Android.Runtime;
using System;

namespace DownloadExample.Droid
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }
    }
}
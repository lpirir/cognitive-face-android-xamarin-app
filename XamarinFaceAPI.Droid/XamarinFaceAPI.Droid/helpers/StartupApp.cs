using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Cognitive.Face.Android;

namespace com.rcervantes.xamarinfaceapi_droid.helpers
{
    [Application]
    public class StartupApp : Application
    {
        private static FaceServiceRestClient sFaceServiceClient = null;

        public StartupApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            sFaceServiceClient = new FaceServiceRestClient(Context.Resources.GetString(Resource.String.subscription_key));
        }

        public static FaceServiceRestClient GetFaceServiceClient()
        {
            return sFaceServiceClient;
        }
    }
}

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

		public StartupApp(IntPtr javaReference, JniHandleOwnership transfer): base(javaReference, transfer)
        {
        }

		public override void OnCreate()
		{
			base.OnCreate();
			sFaceServiceClient = new FaceServiceRestClient("dd1bb5963dcd4d269e9cb296dde2dac5");
		}

		public static FaceServiceRestClient GetFaceServiceClient()
		{
			return sFaceServiceClient;
		}
	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace com.rcervantes.xamarinfaceapi_droid.ui
{
    [Activity(Name = "com.rcervantes.xamarinfaceapi_droid.ui.GroupingActivity", 
              Label = "@string/grouping", 
              ParentActivity = typeof(MainActivity),
			  LaunchMode = Android.Content.PM.LaunchMode.SingleTop,
			  ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class GroupingActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_grouping);
        }
    }
}

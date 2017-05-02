
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinFaceAPI.Droid
{
	[Activity(Name = "XamarinFaceAPI.Droid.Activities.UI.GroupingActivity")]
	public class GroupingActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
            SetContentView(Resource.Layout.activity_grouping);
		}
	}
}

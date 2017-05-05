
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
using Java.Lang;
using XamarinFaceAPI.Droid.Helpers;

namespace XamarinFaceAPI.Droid.Log
{
	[Activity(Name = "XamarinFaceAPI.Droid.Activities.Log.DetectionLogActivity")]
	public class DetectionLogActivity : Activity
	{
		private static TextView logTextView = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.activity_verification);

			logTextView = FindViewById<TextView>(Resource.Id.log);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		private class LogAdapter : BaseAdapter
		{
			private List<string> log = null;

			public LogAdapter()
			{
				log = LogHelper.GetDetectionLog();
			}

			public override bool IsEnabled(int position)
			{
				return false;
			}

			public override int Count
			{
				get
				{
					return log.Count;
				}
			}

			public override Java.Lang.Object GetItem(int position)
			{
				return log[position];
			}

			public override long GetItemId(int position)
			{
				return position;
			}

			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				if (convertView == null)
				{
					LayoutInflater layoutInflater = (LayoutInflater)Application.Context.GetSystemService(Context.LayoutInflaterService);
					convertView = layoutInflater.Inflate(Resource.Layout.item_log, parent, false);
				}
				convertView.Id = position;

				logTextView.Text = log[position];

				return convertView;
			}
		}
	}
}


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
using com.rcervantes.xamarinfaceapi_droid.helpers;
using com.rcervantes.xamarinfaceapi_droid.ui;

namespace com.rcervantes.xamarinfaceapi_droid.log
{
	[Activity(Name = "com.rcervantes.xamarinfaceapi_droid.log.GroupingLogActivity",
			  Label = "@string/grouping_log",
			  ParentActivity = typeof(GroupingActivity),
			  ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class GroupingLogActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.activity_grouping_log);

			LogAdapter logAdapter = new LogAdapter(this);
			ListView listView = (ListView)FindViewById(Resource.Id.log);
            listView.Adapter = logAdapter;
        }

		private class LogAdapter : BaseAdapter
		{
			private List<string> log;
			private GroupingLogActivity activity;

			public LogAdapter(GroupingLogActivity act)
			{
				this.log = LogHelper.GetGroupingLog();
				this.activity = act;
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

				((TextView)convertView.FindViewById(Resource.Id.log)).Text = log[position];

				return convertView;
			}
		}
    }
}

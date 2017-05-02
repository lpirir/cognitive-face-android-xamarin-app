using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;

namespace XamarinFaceAPI.Droid.Activities.UI
{
	[Activity(Name = "XamarinFaceAPI.Droid.Activities.UI.MainActivity")]
	public class MainActivity : Activity
	{
		Button detection, verification, grouping, findSimilarFace, identification = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);

			detection = FindViewById<Button>(Resource.Id.detection);
			detection.Click += Detection_Click;

			verification = FindViewById<Button>(Resource.Id.verification);
			verification.Click += Verification_Click;

			grouping = FindViewById<Button>(Resource.Id.grouping);
			grouping.Click += Grouping_Click;

			findSimilarFace = FindViewById<Button>(Resource.Id.findSimilarFace);
			findSimilarFace.Click += FindSimilarFace_Click;

			identification = FindViewById<Button>(Resource.Id.identification);
			identification.Click += Identification_Click;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			detection.Click -= Detection_Click;
			verification.Click -= Verification_Click;
			grouping.Click -= Grouping_Click;
			findSimilarFace.Click -= FindSimilarFace_Click;
			identification.Click -= Identification_Click;
		}

		void Detection_Click(object sender, System.EventArgs e)
		{
			Intent intent = new Intent(this, typeof(DetectionActivity));
			this.StartActivity(intent);
		}

		void Verification_Click(object sender, System.EventArgs e)
		{
			Intent intent = new Intent(this, typeof(VerificationMenuActivity));
			this.StartActivity(intent);
		}

		void Grouping_Click(object sender, System.EventArgs e)
		{
			Intent intent = new Intent(this, typeof(GroupingActivity));
			this.StartActivity(intent);
		}

		void FindSimilarFace_Click(object sender, System.EventArgs e)
		{
			Intent intent = new Intent(this, typeof(FindSimilarFaceActivity));
			this.StartActivity(intent);
		}

		void Identification_Click(object sender, System.EventArgs e)
		{
			Intent intent = new Intent(this, typeof(IdentificationActivity));
			this.StartActivity(intent);
		}
	}
}


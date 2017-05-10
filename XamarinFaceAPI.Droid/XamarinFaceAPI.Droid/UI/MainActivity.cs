using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Support.V7.App;

namespace com.rcervantes.xamarinfaceapi_droid.ui
{
	[Activity(Name="com.rcervantes.xamarinfaceapi_droid.ui.MainActivity", Label = "@string/app_name", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		private Button detection, verification, grouping, findSimilarFace, identification = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);

			detection = FindViewById<Button>(Resource.Id.detection);		
			verification = FindViewById<Button>(Resource.Id.verification);
			grouping = FindViewById<Button>(Resource.Id.grouping);
			findSimilarFace = FindViewById<Button>(Resource.Id.findSimilarFace);
			identification = FindViewById<Button>(Resource.Id.identification);
		}

        protected override void OnResume()
        {
            base.OnResume();
            detection.Click += Detection_Click;
            verification.Click += Verification_Click;
            grouping.Click += Grouping_Click;
            findSimilarFace.Click += FindSimilarFace_Click;
            identification.Click += Identification_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();
			detection.Click -= Detection_Click;
			verification.Click -= Verification_Click;
			grouping.Click -= Grouping_Click;
			findSimilarFace.Click -= FindSimilarFace_Click;
			identification.Click -= Identification_Click;
        }

		private void Detection_Click(object sender, System.EventArgs e)
		{
			Intent intent = new Intent(this, typeof(DetectionActivity));
			this.StartActivity(intent);
		}

		private void Verification_Click(object sender, System.EventArgs e)
		{
			Intent intent = new Intent(this, typeof(VerificationMenuActivity));
			this.StartActivity(intent);
		}

		private void Grouping_Click(object sender, System.EventArgs e)
		{
			Intent intent = new Intent(this, typeof(GroupingActivity));
			this.StartActivity(intent);
		}

		private void FindSimilarFace_Click(object sender, System.EventArgs e)
		{
			Intent intent = new Intent(this, typeof(FindSimilarFaceActivity));
			this.StartActivity(intent);
		}

		private void Identification_Click(object sender, System.EventArgs e)
		{
			Intent intent = new Intent(this, typeof(IdentificationActivity));
			this.StartActivity(intent);
		}
	}
}


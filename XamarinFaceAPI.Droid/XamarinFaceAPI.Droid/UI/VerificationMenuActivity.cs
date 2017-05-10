
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
	[Activity(Name = "com.rcervantes.xamarinfaceapi_droid.ui.VerificationMenuActivity", Label = "@string/verification", ParentActivity = typeof(MainActivity))]
	public class VerificationMenuActivity : AppCompatActivity
	{
		private Button select_face_face_verification, select_face_person_verification = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.activity_verification_menu);

			select_face_face_verification = FindViewById<Button>(Resource.Id.select_face_face_verification);
			select_face_face_verification.Click += Select_Face_Face_Verification_Click;

			select_face_person_verification = FindViewById<Button>(Resource.Id.select_face_person_verification);
			select_face_person_verification.Click += Select_Face_Person_Verification_Click;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			select_face_face_verification.Click -= Select_Face_Face_Verification_Click;
			select_face_person_verification.Click -= Select_Face_Person_Verification_Click;
		}

		void Select_Face_Face_Verification_Click(object sender, EventArgs e)
		{
			Intent intent = new Intent(this, typeof(FaceVerificationActivity));
			this.StartActivity(intent);
		}

		void Select_Face_Person_Verification_Click(object sender, EventArgs e)
		{
			Intent intent = new Intent(this, typeof(PersonVerificationActivity));
			this.StartActivity(intent);
		}
	}
}

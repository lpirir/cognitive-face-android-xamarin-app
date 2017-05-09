
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Text;
using Java.Util;
using Xamarin.Cognitive.Face.Android;
using Xamarin.Cognitive.Face.Android.Contract;
using com.rcervantes.xamarinfaceapi_droid.helpers;
using com.rcervantes.xamarinfaceapi_droid.log;

namespace com.rcervantes.xamarinfaceapi_droid.ui
{
	[Activity(Name="com.rcervantes.xamarinfaceapi_droid.ui.FaceVerificationActivity", Label = "@string/face_verification", ParentActivity = typeof(MainActivity))]
	public class FaceVerificationActivity : AppCompatActivity
	{
		private static int REQUEST_SELECT_IMAGE_0 = 0;
		private static int REQUEST_SELECT_IMAGE_1 = 1;

		private UUID mFaceId0 = null;
		private UUID mFaceId1 = null;

		private static Bitmap mBitmap0 = null;
		private static Bitmap mBitmap1 = null;

		private FaceListAdapter mFaceListAdapter0;
		private FaceListAdapter mFaceListAdapter1;

		private ProgressDialog mProgressDialog = null;

		private Button select_image_0, select_image_1, view_log, verify = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.activity_verification);

			mProgressDialog = new ProgressDialog(this);
			mProgressDialog.SetTitle(Resource.String.progress_dialog_title);

			select_image_0 = FindViewById<Button>(Resource.Id.select_image_0);
			select_image_0.Click += Select_Image_0_Click;

			select_image_1 = FindViewById<Button>(Resource.Id.select_image_1);
			select_image_1.Click += Select_Image_1_Click;

			view_log = FindViewById<Button>(Resource.Id.view_log);
			view_log.Click += View_Log_Click;

			verify = FindViewById<Button>(Resource.Id.verify);
			verify.Click += Verify_Click;

			ClearDetectedFaces(0);

			ClearDetectedFaces(1);

			// Disable button "verify" as the two face IDs to verify are not ready.
			SetVerifyButtonEnabledStatus(false);

			LogHelper.ClearVerificationLog();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			select_image_0.Click -= Select_Image_0_Click;
			select_image_1.Click -= Select_Image_1_Click;
			view_log.Click -= View_Log_Click;
			verify.Click -= Verify_Click;
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			int index;
			if (requestCode == REQUEST_SELECT_IMAGE_0)
			{
				index = 0;
			}
			else if (requestCode == REQUEST_SELECT_IMAGE_1)
			{
				index = 1;
			}
			else
			{
				return;
			}

			if (resultCode == Result.Ok)
			{
				// If image is selected successfully, set the image URI and bitmap.
				Bitmap bitmap = ImageHelper.LoadSizeLimitedBitmapFromUri(data.Data, this.ContentResolver);
				if (bitmap != null)
				{
					// Image is select but not detected, disable verification button.
					SetVerifyButtonEnabledStatus(false);

					ClearDetectedFaces(index);

					// Set the image to detect.
					if (index == 0)
					{
						mBitmap0 = bitmap;
						mFaceId0 = null;
					}
					else
					{
						mBitmap1 = bitmap;
						mFaceId1 = null;
					}

					// Add verification log.
					AddLog("Image" + index + ": " + data.Data + " resized to " + bitmap.Width + "x" + bitmap.Height);

					// Start detecting in image.
					Detect(bitmap, index);
				}
			}
		}

		private void ClearDetectedFaces(int index)
		{
			ListView faceList = (ListView)FindViewById(index == 0 ? Resource.Id.list_faces_0 : Resource.Id.list_faces_1);
			faceList.Visibility = ViewStates.Gone;

			ImageView imageView = (ImageView)FindViewById(index == 0 ? Resource.Id.image_0 : Resource.Id.image_1);
			imageView.SetImageResource(Android.Resource.Color.Transparent);
		}

		void Select_Image_0_Click(object sender, EventArgs e)
		{
			SelectImage(0);
		}

		void Select_Image_1_Click(object sender, EventArgs e)
		{
			SelectImage(1);
		}

		void Verify_Click(object sender, EventArgs e)
		{
			SetAllButtonEnabledStatus(false);
			new VerificationTask(mFaceId0, mFaceId1, this).Execute();
		}

		void View_Log_Click(object sender, EventArgs e)
		{
			Intent intent = new Intent(this, typeof(VerificationLogActivity));
			StartActivity(Intent);
		}

		private void SelectImage(int index)
		{
			Intent intent = new Intent(this, typeof(SelectImageActivity));
			StartActivityForResult(intent, index == 0 ? REQUEST_SELECT_IMAGE_0 : REQUEST_SELECT_IMAGE_1);
		}

		private void SetSelectImageButtonEnabledStatus(bool isEnabled, int index)
		{
			Button button;

			if (index == 0)
			{
				button = (Button)FindViewById(Resource.Id.select_image_0);
			}
			else
			{
				button = (Button)FindViewById(Resource.Id.select_image_1);
			}

			button.Enabled = isEnabled;

			Button viewLog = (Button)FindViewById(Resource.Id.view_log);
			viewLog.Enabled = isEnabled;
		}

		private void SetVerifyButtonEnabledStatus(bool isEnabled)
		{
			Button button = (Button)FindViewById(Resource.Id.verify);
			button.Enabled = isEnabled;
		}

		private void SetAllButtonEnabledStatus(bool isEnabled)
		{
			Button selectImage0 = (Button)FindViewById(Resource.Id.select_image_0);
			selectImage0.Enabled = isEnabled;

			Button selectImage1 = (Button)FindViewById(Resource.Id.select_image_1);
			selectImage1.Enabled = isEnabled;

			Button verif = (Button)FindViewById(Resource.Id.verify);
			verif.Enabled = isEnabled;

			Button viewLog = (Button)FindViewById(Resource.Id.view_log);
			viewLog.Enabled = isEnabled;
		}

		private void InitializeFaceList(int index)
		{
			ListView listView = (ListView)FindViewById(index == 0 ? Resource.Id.list_faces_0 : Resource.Id.list_faces_1);

			listView.ItemClick += (sender, e) =>
			{

				FaceListAdapter faceListAdapter = index == 0 ? mFaceListAdapter0 : mFaceListAdapter1;

				if (!faceListAdapter.faces[e.Position].FaceId.Equals(
						index == 0 ? mFaceId0 : mFaceId1))
				{
					if (index == 0)
					{
						mFaceId0 = faceListAdapter.faces[e.Position].FaceId;
					}
					else
					{
						mFaceId1 = faceListAdapter.faces[e.Position].FaceId;
					}

					ImageView imageView = (ImageView)FindViewById(index == 0 ? Resource.Id.image_0 : Resource.Id.image_1);
					imageView.SetImageBitmap(faceListAdapter.faceThumbnails[e.Position]);

					SetInfo("");
				}

				// Show the list of detected face thumbnails.
				ListView lv = (ListView)FindViewById(index == 0 ? Resource.Id.list_faces_0 : Resource.Id.list_faces_1);
				lv.Adapter = faceListAdapter;
			};
		}

		private void SetUiAfterVerification(VerifyResult result)
		{
			mProgressDialog.Dismiss();

			SetAllButtonEnabledStatus(true);

			// Show verification result.
			if (result != null)
			{
				DecimalFormat formatter = new DecimalFormat("#0.00");
				Android.Resource.String verificationResult = (Android.Resource.String)((result.IsIdentical ? "The same person" : "Different persons") + ". The confidence is " + formatter.Format(result.Confidence));

				SetInfo((string)verificationResult);
			}
		}

		private void SetUiAfterDetection(Face[] result, int index, bool succeed)
		{
			SetSelectImageButtonEnabledStatus(true, index);

			if (succeed)
			{
				AddLog("Response: Success. Detected " + result.Length + " face(s) in image" + index);
				SetInfo(result.Length + " face" + (result.Length != 1 ? "s" : "") + " detected");

				FaceListAdapter faceListAdapter = new FaceListAdapter(result, index, this);

				if (faceListAdapter.faces.Count != 0)
				{
					if (index == 0)
					{
						mFaceId0 = faceListAdapter.faces[0].FaceId;
					}
					else
					{
						mFaceId1 = faceListAdapter.faces[0].FaceId;
					}
					ImageView imageView = (ImageView)FindViewById(index == 0 ? Resource.Id.image_0 : Resource.Id.image_1);
					imageView.SetImageBitmap(faceListAdapter.faceThumbnails[0]);
				}

				ListView listView = (ListView)FindViewById(index == 0 ? Resource.Id.list_faces_0 : Resource.Id.list_faces_1);
				listView.Adapter = faceListAdapter;
				listView.Visibility = ViewStates.Visible;

				if (index == 0)
				{
					mFaceListAdapter0 = faceListAdapter;
					mBitmap0 = null;
				}
				else
				{
					mFaceListAdapter1 = faceListAdapter;
					mBitmap1 = null;
				}
			}

			if (result != null && result.Length == 0)
			{
				SetInfo("No face detected!");
			}

			if ((index == 0 && mBitmap1 == null) || (index == 1 && mBitmap0 == null) || index == 2)
			{
				mProgressDialog.Dismiss();
			}

			if (mFaceId0 != null && mFaceId1 != null)
			{

				SetVerifyButtonEnabledStatus(true);
			}
		}

		private void Detect(Bitmap bitmap, int index)
		{
			MemoryStream output = new MemoryStream();
			bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, output);
			ByteArrayInputStream inputStream = new ByteArrayInputStream(output.ToArray());

			new DetectionTask(index, this).Execute(inputStream);

			SetSelectImageButtonEnabledStatus(false, index);

			SetInfo("Detecting...");
		}

		private void SetInfo(string _info)
		{
			TextView info = FindViewById<TextView>(Resource.Id.info);
			info.Text = _info;
		}

		private void AddLog(string _log)
		{
			LogHelper.AddDetectionLog(_log);
		}

		private class FaceListAdapter : BaseAdapter
		{
			public List<Face> faces;
			public List<Bitmap> faceThumbnails;
			private int mIndex;
			private FaceVerificationActivity activity;

			public FaceListAdapter(Face[] detectionResult, int index, FaceVerificationActivity act)
			{
				faces = new List<Face>();
				faceThumbnails = new List<Bitmap>();
				mIndex = index;
				activity = act;

				if (detectionResult != null)
				{
					faces = detectionResult.ToList();
					foreach (Face face in faces)
					{
						try
						{
							faceThumbnails.Add(ImageHelper.GenerateFaceThumbnail(index == 0 ? mBitmap0 : mBitmap1, face.FaceRectangle));
						}
						catch (Java.IO.IOException ex)
						{
							activity.SetInfo(ex.Message);
						}
					}
				}
			}

			public override bool IsEnabled(int position)
			{
				return false;
			}

			public override int Count
			{
				get
				{
					return faces.Count;
				}
			}

			public override Java.Lang.Object GetItem(int position)
			{
				return faces[position];
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
					convertView = layoutInflater.Inflate(Resource.Layout.item_face, parent, false);
				}
				convertView.Id = position;

				Bitmap thumbnailToShow = faceThumbnails[position];
				if (mIndex == 0 && faces[position].FaceId.Equals(activity.mFaceId0))
				{
					thumbnailToShow = ImageHelper.HighlightSelectedFaceThumbnail(thumbnailToShow);
				}
				else if (mIndex == 1 && faces[position].FaceId.Equals(activity.mFaceId1))
				{
					thumbnailToShow = ImageHelper.HighlightSelectedFaceThumbnail(thumbnailToShow);
				}

				// Show the face thumbnail.
				((ImageView)convertView.FindViewById(Resource.Id.image_face)).SetImageBitmap(thumbnailToShow);

				return convertView;
			}
		}

		private class VerificationTask : AsyncTask<Java.Lang.Void, Android.Resource.String, VerifyResult>
		{
			private UUID mFaceId0;
			private UUID mFaceId1;
			private FaceVerificationActivity activity;

			public VerificationTask(UUID faceId0, UUID faceId1, FaceVerificationActivity act)
			{
				this.mFaceId0 = faceId0;
				this.mFaceId1 = faceId1;
				this.activity = act;
			}

			protected override VerifyResult RunInBackground(params Java.Lang.Void[] @params)
			{
				// Get an instance of face service client to detect faces in image.
				FaceServiceRestClient faceServiceClient = StartupApp.GetFaceServiceClient();
				try
				{
					PublishProgress("Verifying...");

					return faceServiceClient.Verify(mFaceId0, mFaceId1);
				}
				catch (Java.Lang.Exception e)
				{
					PublishProgress(e.Message);
					activity.AddLog(e.Message);
					return null;
				}
			}

			protected override void OnPreExecute()
			{
				base.OnPreExecute();
				activity.mProgressDialog.Show();
				activity.AddLog("Request: Verifying face " + mFaceId0 + " and face " + mFaceId1);
			}

			protected override void OnProgressUpdate(params Android.Resource.String[] values)
			{
				base.OnProgressUpdate(values);
				activity.mProgressDialog.SetMessage((string)values[0]);
				activity.SetInfo((string)values[0]);
			}

			protected override void OnPostExecute(VerifyResult result)
			{
				base.OnPostExecute(result);

				if (result != null)
				{
					activity.AddLog("Response: Success. Face " + mFaceId0 + " and face "
							+ mFaceId1 + (result.IsIdentical ? " " : " don't ")
							+ "belong to the same person");
				}

				// Show the result on screen when verification is done.
				activity.SetUiAfterVerification(result);
			}
		}

		private class DetectionTask : AsyncTask<InputStream, Android.Resource.String, Face[]>
		{
			private int mIndex;
			private bool mSucceed = true;
			private FaceVerificationActivity activity;

			public DetectionTask(int index, FaceVerificationActivity act)
			{
				this.mIndex = index;
				this.activity = act;
			}

			protected override Face[] RunInBackground(params InputStream[] @params)
			{
				// Get an instance of face service client to detect faces in image.
				FaceServiceRestClient faceServiceClient = StartupApp.GetFaceServiceClient();
				try
				{
					PublishProgress("Detecting...");

					return null;
					//return faceServiceClient.Detect(@params[0], true, true, new[] {
					//	FaceServiceClientFaceAttributeType.Age,
					//	FaceServiceClientFaceAttributeType.Gender,
					//	FaceServiceClientFaceAttributeType.Smile,
					//	FaceServiceClientFaceAttributeType.Glasses,
					//	FaceServiceClientFaceAttributeType.FacialHair,
					//	FaceServiceClientFaceAttributeType.Emotion,
					//	FaceServiceClientFaceAttributeType.HeadPose
					//});
				}
				catch (Java.Lang.Exception e)
				{
					mSucceed = false;
					PublishProgress(e.Message);
					activity.AddLog(e.Message);
					return null;
				}
			}

			protected override void OnPreExecute()
			{
				base.OnPreExecute();
				activity.mProgressDialog.Show();
				activity.AddLog("Request: Detecting in image" + mIndex);
			}

			protected override void OnProgressUpdate(params Android.Resource.String[] values)
			{
				base.OnProgressUpdate(values);
				activity.mProgressDialog.SetMessage((string)values[0]);
				activity.SetInfo((string)values[0]);
			}

			protected override void OnPostExecute(Face[] result)
			{
				base.OnPostExecute(result);
				activity.SetUiAfterDetection(result, mIndex, mSucceed);
			}
		}
	}
}
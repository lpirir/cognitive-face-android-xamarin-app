
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Xamarin.Cognitive.Face.Android;
using Xamarin.Cognitive.Face.Android.Contract;

namespace XamarinFaceAPI.Droid
{
	[Activity(Name = "XamarinFaceAPI.Droid.Activities.UI.DetectionActivity")]
	public class DetectionActivity : Activity
	{
		private const int REQUEST_SELECT_IMAGE = 0;
		private FaceServiceRestClient faceServiceClient = null;
		private Button select_image, detect, view_log = null;
		private static Bitmap mBitmap = null;
		private ImageView image = null;
		private static TextView info, text_detected_face = null;
		private ListView list_detected_faces = null;
		private ProgressDialog mProgressDialog = null;
		private Android.Net.Uri mImageUri = null;


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			faceServiceClient = new FaceServiceRestClient("");

			// Create your application here
			SetContentView(Resource.Layout.activity_detection);

			mProgressDialog = new ProgressDialog(this);
			mProgressDialog.SetTitle("Please wait");

			select_image = FindViewById<Button>(Resource.Id.select_image);
			select_image.Click += Select_Image_Click;

			detect = FindViewById<Button>(Resource.Id.detect);
			detect.Click += Detect_Click;

			view_log = FindViewById<Button>(Resource.Id.view_log);
			view_log.Click += View_Log_Click;

			image = FindViewById<ImageView>(Resource.Id.image);

			info = FindViewById<TextView>(Resource.Id.info);

			text_detected_face = FindViewById<TextView>(Resource.Id.text_detected_face);

			list_detected_faces = FindViewById<ListView>(Resource.Id.list_detected_faces);

			SetDetectButtonEnabledStatus(false);

			LogHelper.ClearDetectionLog();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			select_image.Click -= Select_Image_Click;
			detect.Click -= Detect_Click;
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			outState.PutParcelable("ImageUri", mImageUri);
		}

		protected override void OnRestoreInstanceState(Bundle savedInstanceState)
		{
			base.OnRestoreInstanceState(savedInstanceState);
			mImageUri = (Android.Net.Uri)savedInstanceState.GetParcelable("ImageUri");
			if (mImageUri != null)
			{
				mBitmap = ImageHelper.LoadSizeLimitedBitmapFromUri(mImageUri, this.ContentResolver);
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			switch (requestCode)
			{
				case (int)REQUEST_SELECT_IMAGE:
					if (resultCode == Result.Ok)
					{
						mImageUri = data.Data;
						mBitmap = ImageHelper.LoadSizeLimitedBitmapFromUri(mImageUri, this.ContentResolver);
						if (mBitmap != null)
						{
							image.SetImageBitmap(mBitmap);
							AddLog("Image: " + mImageUri + " resized to " + mBitmap.Width + "x" + mBitmap.Height);
						}

						//FaceListAdapter

					}
					break;
				default:
					break;
			}
		}

		void Select_Image_Click(object sender, EventArgs e)
		{
			Intent intent = new Intent(this, typeof(SelectImageActivity));
			this.StartActivityForResult(intent, REQUEST_SELECT_IMAGE);
		}

		void Detect_Click(object sender, EventArgs e)
		{
			//// Put the image into an input stream for detection.
			//ByteArrayOutputStream output = new ByteArrayOutputStream();
			//mBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, output);
			//ByteArrayInputStream inputStream = new ByteArrayInputStream(output.ToByteArray());

			//// Start a background task to detect faces in the image.
			//new DetectionTask().execute(inputStream);

			//// Prevent button click during detecting.
			//SetAllButtonsEnabledStatus(false);
		}

		void View_Log_Click(object sender, EventArgs e)
		{
			//Intent intent = new Intent(this, typeof(SelectImageActivity));
			//this.StartActivityForResult(intent, REQUEST_SELECT_IMAGE);
		}

		private void SetUiAfterDetection(Face[] result, bool succeed)
		{
			mProgressDialog.Dismiss();
			SetAllButtonsEnabledStatus(true);
			SetDetectButtonEnabledStatus(false);

			if (succeed)
			{
				string detectionResult;
				if (result != null)
				{
					detectionResult = result.Length + " face"
					   + (result.Length != 1 ? "s" : "") + " detected";

					image.SetImageBitmap(ImageHelper.DrawFaceRectanglesOnBitmap(mBitmap, result, true));
					FaceListAdapter faceListAdapter = new FaceListAdapter(result);
					list_detected_faces.Adapter = faceListAdapter;
				}
				else
				{
					detectionResult = "0 face detected";
				}
				SetInfo(detectionResult);
			}

			mImageUri = null;
			mBitmap = null;
		}

		private void SetDetectButtonEnabledStatus(bool isEnabled)
		{
			detect.Enabled = isEnabled;
		}

		private void SetAllButtonsEnabledStatus(bool isEnabled)
		{
			select_image.Enabled = isEnabled;
			detect.Enabled = isEnabled;
			view_log.Enabled = isEnabled;
		}

		private static void SetInfo(string _info)
		{
			info.Text = _info;
		}

		private void AddLog(string _log)
		{
			LogHelper.AddDetectionLog(_log);
		}

		private class FaceListAdapter : BaseAdapter
		{
			List<Face> faces = null;
			List<Bitmap> faceThumbnails = null;

			public FaceListAdapter(Face[] detectionResult)
			{
				faces = new List<Face>();
				faceThumbnails = new List<Bitmap>();

				if (detectionResult != null)
				{
					faces = detectionResult.ToList();
					foreach (Face face in faces)
					{
						try
						{
							faceThumbnails.Add(ImageHelper.GenerateFaceThumbnail(DetectionActivity.mBitmap, face.FaceRectangle));
						}
						catch (IOException ex)
						{
							DetectionActivity.SetInfo(ex.Message);
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
					convertView = layoutInflater.Inflate(Resource.Layout.item_face_with_description, parent, false);
				}
				convertView.Id = position;

				((ImageView)convertView.FindViewById(Resource.Id.face_thumbnail)).SetImageBitmap(faceThumbnails[position]);

				DecimalFormat formatter = new DecimalFormat("#0.0");
				string face_description = string.Format("Age: {0}\nGender: {1}\nSmile: {2}\nGlasses: {3}\nFacialHair: {4}\nHeadPose: {5}",
						faces[position].FaceAttributes.Age,
						faces[position].FaceAttributes.Gender,
						faces[position].FaceAttributes.Smile,
						faces[position].FaceAttributes.Glasses,
						GetFacialHair(faces[position].FaceAttributes.FacialHair),
						GetEmotion(faces[position].FaceAttributes.Emotion),
						GetHeadPose(faces[position].FaceAttributes.HeadPose)
						);

				text_detected_face.Text = face_description;

				return convertView;
			}

			private string GetFacialHair(FacialHair facialHair)
			{
				return (facialHair.Moustache + facialHair.Beard + facialHair.Sideburns > 0) ? "Yes" : "No";
			}

			private string GetEmotion(Emotion emotion)
			{
				string emotionType = "";
				double emotionValue = 0.0;
				if (emotion.Anger > emotionValue)
				{
					emotionValue = emotion.Anger;
					emotionType = "Anger";
				}
				if (emotion.Contempt > emotionValue)
				{
					emotionValue = emotion.Contempt;
					emotionType = "Contempt";
				}
				if (emotion.Disgust > emotionValue)
				{
					emotionValue = emotion.Disgust;
					emotionType = "Disgust";
				}
				if (emotion.Fear > emotionValue)
				{
					emotionValue = emotion.Fear;
					emotionType = "Fear";
				}
				if (emotion.Happiness > emotionValue)
				{
					emotionValue = emotion.Happiness;
					emotionType = "Happiness";
				}
				if (emotion.Neutral > emotionValue)
				{
					emotionValue = emotion.Neutral;
					emotionType = "Neutral";
				}
				if (emotion.Sadness > emotionValue)
				{
					emotionValue = emotion.Sadness;
					emotionType = "Sadness";
				}
				if (emotion.Surprise > emotionValue)
				{
					emotionValue = emotion.Surprise;
					emotionType = "Surprise";
				}
				return string.Format("{0}: {1}", emotionType, emotionValue);
			}

			private string GetHeadPose(HeadPose headPose)
			{
				return string.Format("Pitch: {0}, Roll: {1}, Yaw: {2}", headPose.Pitch, headPose.Roll, headPose.Yaw);
			}

		}
	}
}

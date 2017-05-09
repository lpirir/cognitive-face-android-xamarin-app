
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Xamarin.Cognitive.Face.Android;
using Xamarin.Cognitive.Face.Android.Contract;
using com.rcervantes.xamarinfaceapi_droid.helpers;
using com.rcervantes.xamarinfaceapi_droid.log;
using Android.Graphics.Drawables;

namespace com.rcervantes.xamarinfaceapi_droid.ui
{
    [Activity(Name = "com.rcervantes.xamarinfaceapi_droid.ui.DetectionActivity", Label = "@string/detection", ParentActivity = typeof(MainActivity))]
    public class DetectionActivity : AppCompatActivity
    {
        private const int REQUEST_SELECT_IMAGE = 0;
        private Button select_image, detect, view_log = null;
        private Bitmap mBitmap = null;
        private ProgressDialog mProgressDialog = null;
        private Android.Net.Uri mImageUri = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

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

            SetDetectButtonEnabledStatus(false);

            LogHelper.ClearDetectionLog();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            select_image.Click -= Select_Image_Click;
            detect.Click -= Detect_Click;
            view_log.Click -= View_Log_Click;
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
                            ImageView image = FindViewById<ImageView>(Resource.Id.image);
                            image.SetImageBitmap(mBitmap);
                            AddLog("Image: " + mImageUri + " resized to " + mBitmap.Width + "x" + mBitmap.Height);
                        }

                        FaceListAdapter faceListAdapter = new FaceListAdapter(null, this);
                        ListView list_detected_faces = FindViewById<ListView>(Resource.Id.list_detected_faces);
                        list_detected_faces.Adapter = faceListAdapter;

                        SetInfo("");

                        SetDetectButtonEnabledStatus(true);
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
            new DetectionTask(this).Execute();
            SetAllButtonsEnabledStatus(false);
        }

        void View_Log_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(DetectionLogActivity));
            this.StartActivity(intent);
        }

        private void SetUiAfterDetection(Face[] result, bool succeed, ListView list_detected_faces)
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

                    ImageView image = FindViewById<ImageView>(Resource.Id.image);
                    image.SetImageBitmap(ImageHelper.DrawFaceRectanglesOnBitmap(mBitmap, result, true));
                    FaceListAdapter faceListAdapter = new FaceListAdapter(result, this);
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

        private void SetInfo(string inf)
        {
            TextView info = FindViewById<TextView>(Resource.Id.info);
            info.Text = inf;
        }

        private void AddLog(string _log)
        {
            LogHelper.AddDetectionLog(_log);
        }

        private class FaceListAdapter : BaseAdapter
        {
            private List<Face> faces;
            private List<Bitmap> faceThumbnails;
            private DetectionActivity activity;

            public FaceListAdapter(Face[] detectionResult, DetectionActivity act)
            {
                faces = new List<Face>();
                faceThumbnails = new List<Bitmap>();
                activity = act;

                if (detectionResult != null)
                {
                    faces = detectionResult.ToList();
                    foreach (Face face in faces)
                    {
                        try
                        {
                            faceThumbnails.Add(ImageHelper.GenerateFaceThumbnail(activity.mBitmap, face.FaceRectangle));
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

                TextView text_detected_face = convertView.FindViewById<TextView>(Resource.Id.text_detected_face);
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

        private class DetectionTask : AsyncTask<Java.Lang.Void, Java.Lang.String, bool>
        {
            private DetectionActivity activity;
            private Face[] faces = null;

            public DetectionTask(DetectionActivity act)
            {
                this.activity = act;
            }

            protected override bool RunInBackground(params Java.Lang.Void[] @params)
            {
                // Get an instance of face service client to detect faces in image.
                FaceServiceRestClient faceServiceClient = StartupApp.GetFaceServiceClient();

                bool mSucceed = true;

                try
                {
                    PublishProgress("Detecting...");

					var pre_output = new MemoryStream();
					activity.mBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, pre_output);
					ByteArrayInputStream inputStream = new ByteArrayInputStream(pre_output.ToArray());
					byte[] arr = new byte[inputStream.Available()];
					inputStream.Read(arr);
					var output = new MemoryStream(arr);

                    faces = faceServiceClient.Detect(output, true, true, new[] {
                                    FaceServiceClientFaceAttributeType.Age,
                                    FaceServiceClientFaceAttributeType.Gender,
                                    FaceServiceClientFaceAttributeType.Smile,
                                    FaceServiceClientFaceAttributeType.Glasses,
                                    FaceServiceClientFaceAttributeType.FacialHair,
                                    FaceServiceClientFaceAttributeType.Emotion,
                                    FaceServiceClientFaceAttributeType.HeadPose
                                });

                }
                catch (Java.Lang.Exception e)
                {
                    mSucceed = false;
                    PublishProgress(e.Message);
                    activity.AddLog(e.Message);
                }

                return mSucceed;
            }

            protected override void OnPreExecute()
            {
                base.OnPreExecute();
                activity.mProgressDialog.Show();
                activity.AddLog("Request: Detecting in image " + activity.mImageUri);
            }

            protected override void OnProgressUpdate(params Java.Lang.String[] values)
            {
                base.OnProgressUpdate(values);
                activity.mProgressDialog.SetMessage((string)values[0]);
                activity.SetInfo((string)values[0]);
            }

            protected override void OnPostExecute(bool result)
            {
                base.OnPostExecute(result);

                if (result)
                {
                    activity.AddLog("Response: Success. Detected " + (faces == null ? 0 : faces.Length) + " face(s) in " + activity.mImageUri);
                }

                // Show the result on screen when detection is done.
                ListView list_detected_faces = activity.FindViewById<ListView>(Resource.Id.list_detected_faces);
                activity.SetUiAfterDetection(faces, result, list_detected_faces);
            }

        }
    }
}

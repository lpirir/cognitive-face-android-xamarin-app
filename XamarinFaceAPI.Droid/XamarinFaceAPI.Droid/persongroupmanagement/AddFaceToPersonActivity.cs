
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
using com.rcervantes.xamarinfaceapi_droid.client;
using com.rcervantes.xamarinfaceapi_droid.helpers;
using Java.IO;
using Java.Util;
using Xamarin.Cognitive.Face.Android.Contract;

namespace com.rcervantes.xamarinfaceapi_droid.persongroupmanagement
{
    [Activity(Name = "com.rcervantes.xamarinfaceapi_droid.persongroupmanagement.AddFaceToPersonActivity", 
              Label = "@string/add_face_to_person", 
              ParentActivity = typeof(PersonActivity),
			  ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class AddFaceToPersonActivity : AppCompatActivity
    {
        private String mPersonGroupId, mPersonId, mImageUriStr = null;
        private Bitmap mBitmap = null;
        private FaceGridViewAdapter mFaceGridViewAdapter = null;
        private ProgressDialog mProgressDialog = null;
        private Button done_and_save = null;
        private GridView gridView = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_add_face_to_person);

            Bundle bundle = Intent.Extras;
            if (bundle != null)
            {
                mPersonId = bundle.GetString("PersonId");
                mPersonGroupId = bundle.GetString("PersonGroupId");
                mImageUriStr = bundle.GetString("ImageUriStr");
            }

            mProgressDialog = new ProgressDialog(this);
            mProgressDialog.SetTitle(Application.Context.GetString(Resource.String.progress_dialog_title));

            done_and_save = (Button)FindViewById(Resource.Id.done_and_save);

            gridView = (GridView)FindViewById(Resource.Id.gridView_faces_to_select);
        }

        protected override void OnResume()
        {
            base.OnResume();
            done_and_save.Click += Done_And_Save_Click;

            Android.Net.Uri imageUri = Android.Net.Uri.Parse(mImageUriStr);
            mBitmap = ImageHelper.LoadSizeLimitedBitmapFromUri(imageUri, this.ContentResolver);
            if (mBitmap != null)
            {
                ExecuteDetection();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            done_and_save.Click -= Done_And_Save_Click;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("PersonId", mPersonId);
            outState.PutString("PersonGroupId", mPersonGroupId);
            outState.PutString("ImageUriStr", mImageUriStr);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            mPersonId = savedInstanceState.GetString("PersonId");
            mPersonGroupId = savedInstanceState.GetString("PersonGroupId");
            mImageUriStr = savedInstanceState.GetString("ImageUriStr");
        }

        private async void ExecuteDetection()
        {
            Face[] faces = null;
            bool mSucceed = true;

            mProgressDialog.Show();
            AddLog("Request: Detecting " + mImageUriStr);

            try
            {
                var faceClient = new FaceClient();
                using (MemoryStream pre_output = new MemoryStream())
                {
                    mBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, pre_output);

                    using (ByteArrayInputStream inputStream = new ByteArrayInputStream(pre_output.ToArray()))
                    {
                        byte[] arr = new byte[inputStream.Available()];
                        inputStream.Read(arr);
                        var output = new MemoryStream(arr);

                        mProgressDialog.SetMessage("Detecting...");
                        SetInfo("Detecting...");
                        faces = await faceClient.Detect(output, true, false, null);
                    }
                }
            }
            catch (Java.Lang.Exception e)
            {
                mSucceed = false;
                AddLog(e.Message);
            }

            RunOnUiThread(() =>
            {
                if (mSucceed)
                {
                    int faces_count = (faces == null) ? 0 : faces.Count();
                    AddLog("Response: Success. Detected " + faces_count.ToString() + " Face(s)");
                }

                SetUiAfterDetection(faces, mSucceed);
            });
        }

        private void SetUiAfterDetection(Face[] result, bool succeed)
        {
            mProgressDialog.Dismiss();

            if (succeed)
            {
                if (result != null)
                {
                    SetInfo(result.Count().ToString() + " face"
                            + (result.Count() != 1 ? "s" : "") + " detected");
                }
                else
                {
                    SetInfo("0 face detected");
                }

                mFaceGridViewAdapter = new FaceGridViewAdapter(result, this);
                gridView.Adapter = mFaceGridViewAdapter;
            }
        }

        private void Done_And_Save_Click(object sender, EventArgs e)
        {
            if (mFaceGridViewAdapter != null)
            {
                List<int> faceIndices = new List<int>();

                for (int i = 0; i < mFaceGridViewAdapter.faceRectList.Count; ++i)
                {
                    if (mFaceGridViewAdapter.faceChecked[i])
                    {
                        faceIndices.Add(i);
                    }
                }

                if (faceIndices.Count > 0)
                {
                    ExecuteFaceTask(faceIndices);
                }
                else
                {
                    Finish();
                }
            }
        }

        private async void ExecuteFaceTask(List<int> mFaceIndices)
        {
            AddPersistedFaceResult result = null;
            bool mSucceed = true;

            mProgressDialog.Show();

            try
            {
                var faceClient = new FaceClient();
                UUID personId = UUID.FromString(mPersonId);
                using (MemoryStream pre_output = new MemoryStream())
                {
                    mBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, pre_output);
                    using (ByteArrayInputStream inputStream = new ByteArrayInputStream(pre_output.ToArray()))
                    {
                        byte[] arr = new byte[inputStream.Available()];
                        inputStream.Read(arr);
                        var output = new MemoryStream(arr);

                        mProgressDialog.SetMessage("Adding face...");
                        SetInfo("Adding face...");

                        foreach (int index in mFaceIndices)
                        {
                            FaceRectangle faceRect = mFaceGridViewAdapter.faceRectList[index];
                            AddLog("Request: Adding face to person " + mPersonId);

                            result = await faceClient.AddPersonFace(mPersonGroupId, personId, output, "User data", faceRect);

                            mFaceGridViewAdapter.faceIdList[index] = result.PersistedFaceId;
                        }
                    }
                }
            }
            catch (Java.Lang.Exception e)
            {
                mSucceed = false;
                AddLog(e.Message);
            }

            RunOnUiThread(() =>
            {
                mProgressDialog.Dismiss();

                if (mSucceed)
                {
                    String faceIds = "";
                    foreach (int index in mFaceIndices)
                    {
                        String faceId = mFaceGridViewAdapter.faceIdList[index].ToString();
                        faceIds += faceId + ", ";

                        try
                        {
                            var file = System.IO.Path.Combine(Application.Context.FilesDir.Path, faceId);
							using (var fs = new FileStream(file, FileMode.OpenOrCreate))
							{
								mFaceGridViewAdapter.faceThumbnails[index].Compress(Bitmap.CompressFormat.Jpeg, 100, fs);
							}

                            Android.Net.Uri uri = Android.Net.Uri.Parse(file);
							StorageHelper.SetFaceUri(faceId, uri.ToString(), mPersonId, this);
                        }
                        catch (Java.IO.IOException e)
                        {
                            SetInfo(e.Message);
                        }
                    }
                    AddLog("Response: Success. Face(s) " + faceIds + "added to person " + mPersonId);
                    Finish();
                }
            });
        }


        private void AddLog(String _log)
        {
            LogHelper.AddIdentificationLog(_log);
        }

        private void SetInfo(String info)
        {
            TextView textView = (TextView)FindViewById(Resource.Id.info);
            textView.Text = info;
        }

        private class FaceGridViewAdapter : BaseAdapter
        {
            public List<UUID> faceIdList;
            public List<FaceRectangle> faceRectList;
            public List<Bitmap> faceThumbnails;
            public List<Boolean> faceChecked;
            private AddFaceToPersonActivity activity;

            public FaceGridViewAdapter(Face[] detectionResult, AddFaceToPersonActivity act)
            {
                faceIdList = new List<UUID>();
                faceRectList = new List<FaceRectangle>();
                faceThumbnails = new List<Bitmap>();
                faceChecked = new List<Boolean>();
                activity = act;

                if (detectionResult != null)
                {
                    List<Face> faces = detectionResult.ToList();
                    foreach (Face face in faces)
                    {
                        try
                        {
                            faceThumbnails.Add(ImageHelper.GenerateFaceThumbnail(activity.mBitmap, face.FaceRectangle));

                            faceIdList.Add(null);
                            faceRectList.Add(face.FaceRectangle);

                            faceChecked.Add(false);
                        }
                        catch (Java.IO.IOException e)
                        {
                            activity.SetInfo(e.Message);
                        }
                    }
                }
            }

            public override int Count
            {
                get
                {
                    return faceRectList.Count;
                }
            }

            public override Java.Lang.Object GetItem(int position)
            {
                return faceRectList[position];
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
                    convertView = layoutInflater.Inflate(Resource.Layout.item_face_with_checkbox, parent, false);
                }
                convertView.Id = position;

                ((ImageView)convertView.FindViewById(Resource.Id.image_face)).SetImageBitmap(faceThumbnails[position]);

                CheckBox checkBox = (CheckBox)convertView.FindViewById(Resource.Id.checkbox_face);
                checkBox.Checked = faceChecked[position];
				checkBox.SetOnCheckedChangeListener(new SetOnCheckedChangeListener(this, position));

                return convertView;
            }

        }

		private class SetOnCheckedChangeListener : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
		{
			private FaceGridViewAdapter adapter;
			private int position;

			public SetOnCheckedChangeListener(FaceGridViewAdapter adap, int pos)
			{
				this.adapter = adap;
				this.position = pos;
			}

			public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
			{
				adapter.faceChecked[position] = isChecked;
			}
		}
    }
}

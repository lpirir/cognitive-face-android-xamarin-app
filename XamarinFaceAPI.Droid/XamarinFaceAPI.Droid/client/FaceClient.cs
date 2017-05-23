
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using com.rcervantes.xamarinfaceapi_droid.helpers;
using com.rcervantes.xamarinfaceapi_droid.utils;
using Java.IO;
using Java.Util;
using Xamarin.Cognitive.Face.Android;
using Xamarin.Cognitive.Face.Android.Contract;

namespace com.rcervantes.xamarinfaceapi_droid.client
{
    public class FaceClient
    {
        public FaceClient() { }

		public Task<Face[]> Detect(MemoryStream stream, bool returnFaceId, bool returnLandmarks, FaceServiceClientFaceAttributeType[] attributes)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
				return faceServiceClient.Detect(stream, true, true, attributes);

			});
		}

		public Task<VerifyResult> Verify(UUID mFaceId0, UUID mFaceId1)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
                return faceServiceClient.Verify(mFaceId0, mFaceId1);
			});
		}

		public Task<VerifyResult> Verify(UUID mFaceId, String mPersonGroupId, UUID mPersonId)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
                return faceServiceClient.Verify(mFaceId, mPersonGroupId, mPersonId);
			});
		}

		public Task DeletePersonGroup(string mPersonGroupId)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
				faceServiceClient.DeletePersonGroup(mPersonGroupId);
			});
		}

		public Task CreatePersonGroup(string mPersonGroupId, string name, string userData)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
                faceServiceClient.CreatePersonGroup(mPersonGroupId, name, userData);
			});
		}

		public Task DeletePerson(string mPersonGroupId, UUID mPersonId)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
				faceServiceClient.DeletePerson(mPersonGroupId, mPersonId);
			});
		}

		public Task TrainPersonGroup(string mPersonGroupId)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
                faceServiceClient.TrainPersonGroup(mPersonGroupId);
			});
		}

		public Task DeletePersonFace(string mPersonGroupId, UUID mPersonId, UUID mFaceId)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
				faceServiceClient.DeletePersonFace(mPersonGroupId, mPersonId, mFaceId);
			});
		}

		public Task<CreatePersonResult> CreatePerson(string mPersonGroupId, string name, string userData)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
                return faceServiceClient.CreatePerson(mPersonGroupId, name, userData);
			});
		}

		public Task<AddPersistedFaceResult> AddPersonFace(string mPersonGroupId, UUID mPersonId, Stream mImageStream, string userData, FaceRectangle targetFace)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
                return faceServiceClient.AddPersonFace(mPersonGroupId, mPersonId, mImageStream, userData, targetFace);
			});
		}

		public Task<GroupResult> Group(UUID[] mFaceIds)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
				return faceServiceClient.Group(mFaceIds);
			});
		}

		public Task<SimilarFace[]> FindSimilar(UUID mFaceId, UUID[] mFaceIds, int mMaxNumOfCandidatesReturned)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
				return faceServiceClient.FindSimilar(mFaceId, mFaceIds, mMaxNumOfCandidatesReturned);
			});
		}

		public Task<SimilarFace[]> FindSimilar(UUID mFaceId, UUID[] mFaceIds, int mMaxNumOfCandidatesReturned, FaceServiceClientFindSimilarMatchMode mMode)
		{
			var faceServiceClient = StartupApp.GetFaceServiceClient();

			return Task.Run(() =>
			{
				return faceServiceClient.FindSimilar(mFaceId, mFaceIds, mMaxNumOfCandidatesReturned, mMode);
			});
		}

    }
}

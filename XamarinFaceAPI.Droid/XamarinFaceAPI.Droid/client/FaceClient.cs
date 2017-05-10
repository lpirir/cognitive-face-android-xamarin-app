﻿
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

        public Task<Face[]> Detect(MemoryStream stream)
        {
            var faceServiceClient = StartupApp.GetFaceServiceClient();

            return Task.Run(() => {

				return faceServiceClient.Detect(stream, true, true, new[] {
									FaceServiceClientFaceAttributeType.Age,
									FaceServiceClientFaceAttributeType.Gender,
									FaceServiceClientFaceAttributeType.Smile,
									FaceServiceClientFaceAttributeType.Glasses,
									FaceServiceClientFaceAttributeType.FacialHair,
									FaceServiceClientFaceAttributeType.Emotion,
									FaceServiceClientFaceAttributeType.HeadPose
								});

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
    }
}

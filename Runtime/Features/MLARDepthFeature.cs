/*
*   NatMLX
*   Copyright (c) 2021 Yusuf Olokoba.
*/

namespace NatSuite.MLX.Features {

    using System;
    using UnityEngine;
    using UnityEngine.XR.ARSubsystems;
    using ML;
    using ML.Types;
    using static Unity.Mathematics.math;

    /// <summary>
    /// ML augmented reality depth feature.
    /// This feature cannot be used directly for inference.
    /// Instead, it is used by predictors that require depth data for their computations.
    /// </summary>
    public sealed class MLARDepthFeature : MLFeature {

        #region --Client API--
        /// <summary>
        /// Create an AR depth image feature.
        /// </summary>
        /// <param name="image"></param>
        public MLARDepthFeature (XRCpuImage image) : base(new MLImageType(image.width, image.height)) {
            // Check
            if (!image.valid)
                throw new ArgumentException(@"AR depth image is invalid", nameof(image));
            if (Array.IndexOf(SupportedFormats, image.format) < 0)
                throw new ArgumentException($"AR depth image has invalid format: {image.format}", nameof(image));
            // Save
            this.image = image;
            this.rotation = GetRotation();
        }

        /// <summary>
        /// Sample the depth value at a normalized pixel location.
        /// The x-y coordinates are in range [0, 1].
        /// </summary>
        /// <returns>Depth in meters.</returns>
        public float Sample (float x, float y) {
            var type = this.type as MLImageType;
            var s = float2(type.width, type.height);
            var uv = float2(x, y);
            var t = Mathf.Deg2Rad * rotation;
            var T = mul(float2x2(cos(t), -sin(t), sin(t), cos(t)), float2x2(1f, 0f, 0f, -1f));
            var uv_r = mul(T, uv - 0.5f) + 0.5f;
            var xy = int2(uv_r * s);
            var plane = image.GetPlane(0);
            var startIdx = xy.y * plane.rowStride + xy.x * plane.pixelStride;
            var pixelData = plane.data.GetSubArray(startIdx, plane.pixelStride).ToArray();
            switch (image.format) {
                case XRCpuImage.Format.DepthUint16: return BitConverter.ToUInt16(pixelData, 0) / 1000f;
                case XRCpuImage.Format.DepthFloat32: return BitConverter.ToSingle(pixelData, 0);
                default: return 0f;
            }
        }
        #endregion


        #region --Operations--
        private readonly XRCpuImage image;
        private readonly float rotation;
        private static readonly XRCpuImage.Format[] SupportedFormats = new [] {
            XRCpuImage.Format.DepthUint16,
            XRCpuImage.Format.DepthFloat32
        };

        private static float GetRotation () {
            switch (Screen.orientation) {
                case ScreenOrientation.LandscapeLeft: return 0f;
                case ScreenOrientation.Portrait: return -90f;
                case ScreenOrientation.LandscapeRight: return -180f;
                case ScreenOrientation.PortraitUpsideDown: return -270f;
                default: return 0f;
            }
        }
        #endregion
    }
}
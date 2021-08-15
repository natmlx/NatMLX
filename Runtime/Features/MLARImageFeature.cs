/* 
*   NatMLX
*   Copyright (c) 2021 Yusuf Olokoba.
*/

namespace NatSuite.MLX.Features {

    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.XR.ARSubsystems;
    using Unity.Collections.LowLevel.Unsafe;
    using ML;
    using ML.Features;
    using ML.Internal;
    using ML.Types;

    /// <summary>
    /// ML augmented reality image feature.
    /// This feature will perform any necessary conversions to a model's desired input feature type.
    /// </summary>
    public sealed class MLARImageFeature : MLFeature, IMLFeature { // CHECK // Async prediction support

        #region --Client API--
        /// <summary>
        /// Normalization mean.
        /// </summary>
        public Vector3 mean = Vector3.zero;

        /// <summary>
        /// Normalization standard deviation.
        /// </summary>
        public Vector3 std = Vector3.one;

        /// <summary>
        /// Aspect mode.
        /// </summary>
        public MLImageFeature.AspectMode aspectMode = 0;

        /// <summary>
        /// Create an augmented reality image feature.
        /// </summary>
        /// <param name="image">Augmented reality image.</param>
        /// <param name="world">Whether AR image is from world-facing camera.</param>
        public MLARImageFeature (XRCpuImage image, bool world = true) : base(new MLImageType(image.width, image.height)) {
            // Check
            if (!image.valid)
                throw new ArgumentException(@"AR image is invalid", nameof(image));
            if (Array.IndexOf(SupportedFormats, image.format) < 0)
                throw new ArgumentException($"AR image has invalid format: {image.format}", nameof(image));
            // Save
            this.image = image;
            this.world = world;
        }
        #endregion


        #region --Operations--
        private readonly XRCpuImage image;
        private readonly bool world;
        private static readonly XRCpuImage.Format[] SupportedFormats = new [] {
            XRCpuImage.Format.AndroidYuv420_888,
            XRCpuImage.Format.IosYpCbCr420_8BiPlanarFullRange
        };

        unsafe IntPtr IMLFeature.Create (in MLFeatureType type) {
            var planeData = Enumerable.Range(0, image.planeCount).Select(image.GetPlane).ToArray();
            var pixelBuffer = (void*)Marshal.AllocHGlobal(image.width * image.height * 4);
            var (width, height) = (0, 0);
            fixed (void* planes = planeData.Select(p => (IntPtr)p.data.GetUnsafeReadOnlyPtr()).ToArray())
                MLX.Internal.Bridge.CreateARImageFeatureData(
                    (void**)planes,
                    planeData.Select(p => p.rowStride).ToArray(),
                    planeData.Select(p => p.pixelStride).ToArray(),
                    image.width,
                    image.height,
                    (int)Screen.orientation,
                    world,
                    pixelBuffer,
                    out width,
                    out height
                );
            var feature = new MLImageFeature(pixelBuffer, width, height) {
                mean = mean,
                std = std,
                aspectMode = aspectMode
            };
            var nativeFeature = (feature as IMLFeature).Create(type);
            Marshal.FreeHGlobal((IntPtr)pixelBuffer);  
            return nativeFeature;
        }
        #endregion
    }
}
/* 
*   NatMLX
*   Copyright (c) 2021 Yusuf Olokoba.
*/

namespace NatSuite.MLX.Features {

    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Experimental.Rendering;
    using UnityEngine.XR.ARSubsystems;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using ML;
    using ML.Features;
    using ML.Hub;
    using ML.Internal;
    using ML.Types;
    using MLX.Internal;

    /// <summary>
    /// ML augmented reality image feature.
    /// This feature will perform any necessary conversions to a model's desired input feature type.
    /// </summary>
    #pragma warning disable 0618
    public sealed class MLARImageFeature : MLFeature, IMLEdgeFeature, IMLHubFeature, IMLFeature { // CHECK // Async prediction support
    #pragma warning restore 0618

        #region --Client API--
        /// <summary>
        /// Normalization mean.
        /// </summary>
        public Vector4 mean = Vector3.zero;

        /// <summary>
        /// Normalization standard deviation.
        /// </summary>
        public Vector4 std = Vector3.one;

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

        unsafe IntPtr IMLEdgeFeature.Create (in MLFeatureType type) {
            var planeData = Enumerable.Range(0, image.planeCount).Select(image.GetPlane).ToArray();
            var pixelBuffer = new NativeArray<byte>(
                image.width * image.height * 4,
                Allocator.Temp,
                NativeArrayOptions.UninitializedMemory
            );
            var (width, height) = (0, 0);
            fixed (void* planes = planeData.Select(p => (IntPtr)p.data.GetUnsafeReadOnlyPtr()).ToArray())
                NatMLX.CreateARImageFeatureData(
                    (void**)planes,
                    planeData.Select(p => p.rowStride).ToArray(),
                    planeData.Select(p => p.pixelStride).ToArray(),
                    image.width,
                    image.height,
                    (int)Screen.orientation,
                    world,
                    pixelBuffer.GetUnsafePtr(),
                    out width,
                    out height
                );
            var feature = new MLImageFeature(pixelBuffer.GetUnsafePtr(), width, height) {
                mean = mean,
                std = std,
                aspectMode = aspectMode
            };
            var nativeFeature = (feature as IMLEdgeFeature).Create(type);
            pixelBuffer.Dispose();
            return nativeFeature;
        }

        unsafe MLHubFeature IMLHubFeature.Serialize () { // CHECK // Takes ~66ms on iPhone 12 Pro // Optimize with native lib
            var planeData = Enumerable.Range(0, image.planeCount).Select(image.GetPlane).ToArray();
            var pixelBuffer = new NativeArray<byte>(
                image.width * image.height * 4,
                Allocator.Temp,
                NativeArrayOptions.UninitializedMemory
            );
            var (width, height) = (0, 0);
            fixed (void* planes = planeData.Select(p => (IntPtr)p.data.GetUnsafeReadOnlyPtr()).ToArray())
                NatMLX.CreateARImageFeatureData(
                    (void**)planes,
                    planeData.Select(p => p.rowStride).ToArray(),
                    planeData.Select(p => p.pixelStride).ToArray(),
                    image.width,
                    image.height,
                    (int)Screen.orientation,
                    world,
                    pixelBuffer.GetUnsafePtr(),
                    out width,
                    out height
                );
            var buffer = ImageConversion.EncodeNativeArrayToJPG(
                pixelBuffer,
                GraphicsFormat.R8G8B8A8_UNorm,
                (uint)width,
                (uint)height,
                quality: 80
            ).ToArray();
            pixelBuffer.Dispose();
            return new MLHubFeature {
                data = Convert.ToBase64String(buffer),
                type = HubDataType.Image
            };
        }
        #endregion
    }
}
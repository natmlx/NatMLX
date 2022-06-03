/* 
*   NatMLX
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Features {

    using System;
    using UnityEngine;
    using UnityEngine.XR.ARSubsystems;
    using Unity.Collections.LowLevel.Unsafe;
    using Internal;
    using Types;

    /// <summary>
    /// ML augmented reality image feature.
    /// This feature will perform any necessary conversions to a model's desired input feature type.
    /// </summary>
    public sealed class MLARImageFeature : MLImageFeature, IMLEdgeFeature, IMLCloudFeature {

        #region --Client API--
        /// <summary>
        /// Create an augmented reality image feature.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="world">Whether AR image is from world-facing camera.</param>
        /// <param name="orientation">Image orientation. If `Unknown`, this will default to the screen orientation.</param>
        public MLARImageFeature (
            int width,
            int height,
            bool world = true,
            ScreenOrientation orientation = 0
        ) : base(new byte[width * height * 4], 0, 0) {
            this.orientation = orientation != 0 ? orientation : Screen.orientation;
            this.mirror = world;
            this.type = new MLImageType(portrait ? height : width, portrait ? width : height, 4);
        }

        /// <summary>
        /// Create an augmented reality image feature from an ARFoundation `XRCpuImage`.
        /// </summary>
        /// <param name="image">Augmented reality image.</param>
        /// <param name="world">Whether AR image is from world-facing camera.</param>
        /// <param name="orientation">Image orientation. If `Unknown`, this will default to the screen orientation.</param>
        public MLARImageFeature (
            XRCpuImage image,
            bool world = true,
            ScreenOrientation orientation = 0
        ) : this(image.width, image.height, world, orientation) => CopyFrom(image);

        /// <summary>
        /// Copy feature data from an ARFoundation `XRCpuImage`.
        /// The size of the `XRCpuImage` MUST match that of the feature.
        /// </summary>
        /// <param name="image">XR image.</param>
        public unsafe void CopyFrom (XRCpuImage image) {
            // Check
            if (!image.valid)
                throw new ArgumentException(@"AR image is invalid", nameof(image));
            // Check format
            if (Array.IndexOf(SupportedFormats, image.format) < 0)
                throw new ArgumentException($"AR image has invalid format: {image.format}", nameof(image));
            // Check size
            if (image.width * image.height * 4 != pixelBuffer.Length)
                throw new ArgumentException($"AR image size does not match feature size", nameof(image));
            // Get plane data
            var planes = stackalloc void*[image.planeCount];
            var rows = stackalloc int[image.planeCount];
            var pixels = stackalloc int[image.planeCount];
            for (var i = 0; i < image.planeCount; ++i) {
                var plane = image.GetPlane(i);
                planes[i] = plane.data.GetUnsafeReadOnlyPtr();
                rows[i] = plane.rowStride;
                pixels[i] = plane.pixelStride;
            }
            // Copy
            NatMLX.CreateARImageFeatureData(
                planes,
                image.planeCount,
                image.width,
                image.height,
                rows,
                pixels,
                (int)orientation,
                mirror,
                pixelBuffer,
                out var _,
                out var _
            );
        }

        /// <summary>
        /// Transform a normalized region-of-interest rectangle from feature space into image space.
        /// </summary>
        /// <param name="rect">Input rectangle.</param>
        /// <param name="featureType">Feature type that defines the input rectangle space.</param>
        /// <returns>Normalized rectangle in image space.</returns>
        public override Rect TransformRect (Rect rect, MLImageType featureType) {            
            var size = portrait ? new Vector2(rect.size.y, rect.size.x) : rect.size;
            var rotatedRect = new Rect(rect.center - 0.5f * size, size);
            return base.TransformRect(rotatedRect, featureType);
        }
        #endregion


        #region --Operations--
        private readonly ScreenOrientation orientation;
        private readonly bool mirror;
        private bool portrait => orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown;
        private static readonly XRCpuImage.Format[] SupportedFormats = new [] {
            XRCpuImage.Format.AndroidYuv420_888,
            XRCpuImage.Format.IosYpCbCr420_8BiPlanarFullRange
        };
        #endregion
    }
}
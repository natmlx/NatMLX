/* 
*   NatMLX
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Internal {

    using System.Runtime.InteropServices;

    public static class NatMLX { // NatMLX.h

        public const string Assembly =
        #if UNITY_IOS && !UNITY_EDITOR
        @"__Internal";
        #else
        @"NatMLX";
        #endif


        #region --NMLFeature--
        [DllImport(Assembly, EntryPoint = @"NMLXCreateARImageFeatureData")]
        public static unsafe extern void CreateARImageFeatureData (
            void** planes,
            int planeCount,
            int width,
            int height,
            int* rows,
            int* pixels,
            int orientation,
            bool world,
            [Out] byte[] pixelBuffer,
            out int dstWidth,
            out int dstHeight
        );
        #endregion
    }
}
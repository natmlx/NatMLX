/* 
*   NatMLX
*   Copyright (c) 2021 Yusuf Olokoba.
*/

namespace NatSuite.MLX.Internal {

    using System.Runtime.InteropServices;

    public static class Bridge {

        private const string Assembly =
        #if UNITY_IOS && !UNITY_EDITOR
        @"__Internal";
        #else
        @"NatMLX";
        #endif


        #region --NMLFeature--
        [DllImport(Assembly, EntryPoint = @"NMLXCreateARImageFeatureData")]
        public static unsafe extern void CreateARImageFeatureData (
            void** planes,
            [In] int[] rows,
            [In] int[] pixels,
            int srcWidth,
            int srcHeight,
            int orientation,
            bool world,
            void* pixelBuffer,
            out int dstWidth,
            out int dstHeight
        );
        #endregion
    }
}
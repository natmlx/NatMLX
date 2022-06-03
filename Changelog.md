## 1.0.7
+ Added support for NatML 1.0.11.
+ Added support for ARFoundation Remote in the Windows Editor.
+ Added `MLARImageFeature` constructor which accepts image `width` and `height`.
+ Added `MLARImageFeature.CopyFrom` method to copy feature data from `XRCpuImage` without making new memory allocations.
+ Added `orientation` parameter to `MLARImageFeature` constructor for specifying desired image orientation.
+ Added `orientation` parameter to `MLARDepthFeature` constructor for specifying depth image orientation.
+ Added `MLARImageFeature.TransformRect` method for transforming detection rectangles from feature space to image space.
+ Added `MLPredictorExtensions.ToAsync` extension method to convert a predictor to an async predictor.
+ Added `MLAsyncPredictor` class for running predictors on a worker thread.
+ Refactored top-level namespace from `NatSuite.MLX` to `NatML`.

## 1.0.6
+ Added background thread prediction support to `MLARImageFeature`.
+ Removed `Tokenizers` namespace along with all tokenizer implementations.

## 1.0.5
+ Added support for NatML 1.0.9.
+ Fixed `MLARImageFeature` default normalization standard deviation having `0` for alpha channel.

## 1.0.4
+ Added support for Hub predictions to `MLARImageFeature`.
+ Added support for ARFoundation Remote when making predictions with AR camera images on macOS.

## 1.0.3
+ Changed `MLARImageFeature.mean` and `std` types to `Vector4` to support normalization for alpha channel.
+ Fixed bitcode not being generated for iOS `NatMLX.framework`.

## 1.0.2
+ Added `MLARDepthFeature` for predictors that need depth data to compute outputs.
+ Added `Tokenizers` namespace with `BERTTokenizer` for natural language processing.

## 1.0.0
+ First release.
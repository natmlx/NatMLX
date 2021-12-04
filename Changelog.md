## 1.0.4
+ Added support for Hub predictions to `MLARImageFeature`.
+ Added support for ARFoundation Remote when making predictions with camera images on macOS.

## 1.0.3
+ Changed `MLARImageFeature.mean` and `std` types to `Vector4` to support normalization for alpha channel.
+ Fixed bitcode not being generated for iOS `NatMLX.framework`.

## 1.0.2
+ Added `MLARDepthFeature` for predictors that need depth data to compute outputs.
+ Added `Tokenizers` namespace with `BERTTokenizer` for natural language processing.

## 1.0.0
+ First release.
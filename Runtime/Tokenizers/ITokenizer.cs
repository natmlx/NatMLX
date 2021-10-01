/* 
*   NatMLX
*   Copyright (c) 2021 Yusuf Olokoba.
*/

namespace NatSuite.MLX.Tokenizers {

    using System.Collections.Generic;

    /// <summary>
    /// Primitive for tokenizing text features and encoding them for use with natural language models.
    /// </summary>
    public interface ITokenizer {

        /// <summary>
        /// Beginning of sentence token.
        /// </summary>
        string beginningToken { get; }

        /// <summary>
        /// End of sentence token.
        /// </summary>
        string endToken { get; }

        /// <summary>
        /// Classifier token.
        /// </summary>
        string classifierToken { get; }

        /// <summary>
        /// Mask token for masked language modeling.
        /// </summary>
        string maskToken { get; }

        /// <summary>
        /// Pad token for batching.
        /// </summary>
        string padToken { get; }

        /// <summary>
        /// Separator token for delineating sentences.
        /// </summary>
        string separatorToken { get; }

        /// <summary>
        /// Unknown token for out-of-vocabulary tokens.
        /// </summary>
        string unknownToken { get; }

        /// <summary>
        /// Tokenize a piece of text into tokens.
        /// </summary>
        /// <param name="text">Input text.</param>
        /// <returns>Array of tokens.</returns>
        string[] Tokenize (string text);

        /// <summary>
        /// Encode a series of tokens into their model-specific ID's.
        /// </summary>
        /// <param name="tokens">Input tokens.</param>
        /// <returns>Model-specific ID's.</returns>
        int[] Encode (IEnumerable<string> tokens);

        /// <summary>
        /// Decode a series of model-specific ID's into tokens.
        /// </summary>
        /// <param name="encodings">Model-specific ID's.</param>
        /// <returns>Result tokens.</returns>
        string[] Decode (IEnumerable<int> encodings);

        /// <summary>
        /// Detokenize a series of tokens into a plain string.
        /// </summary>
        /// <param name="tokens">Input tokens.</param>
        /// <returns>Detokenized string.</returns>
        string Detokenize (IEnumerable<string> tokens);
    }
}
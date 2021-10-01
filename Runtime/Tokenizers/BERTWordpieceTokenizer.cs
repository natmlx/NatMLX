/*
*   Copyright 2021 The TensorFlow Authors and Yusuf Olokoba. All Rights Reserved.
*    
*   Licensed under the Apache License, Version 2.0 (the "License");
*   you may not use this file except in compliance with the License.
*   You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
*
*   Unless required by applicable law or agreed to in writing, software
*   distributed under the License is distributed on an "AS IS" BASIS,
*   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*   See the License for the specific language governing permissions and
*   limitations under the License.
*/

namespace NatSuite.MLX.Tokenizers {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// BERT wordpiece tokenizer.
    /// This tokenizer tokenizes tokens that have been pre-tokenized by the `BERTBasicTokenizer`.
    /// This uses a greedy longest-match-first algorithm to perform tokenization using the given vocabulary.
    /// </summary>
    public sealed class BERTWordpieceTokenizer : ITokenizer {

        #region --Client API--
        /// <summary>
        /// BERT unknown token.
        /// </summary>
        public string unknownToken { get; private set; }

        /// <summary>
        /// Create the BERT wordpiece tokenizer.
        /// </summary>
        /// <param name="vocabulary">BERT vocabulary encoding dictionary.</param>
        /// <param name="unknownToken">BERT unknown token.</param>
        /// <param name="maxCharsPerWord">Maximum characters per word.</param>
        public BERTWordpieceTokenizer (
            IReadOnlyDictionary<string, int> vocabulary,
            string unknownToken = "[UNK]",
            int maxCharsPerWord = 200
        ) {
            this.vocabulary = vocabulary;
            this.unknownToken = unknownToken;
            this.maxCharsPerWord = maxCharsPerWord;
        }

        /// <summary>
        /// Tokenize a piece of text into its word pieces.
        /// For example: input = "unaffable", output = ["una", "##ffa", "##ble"].
        /// </summary>
        /// <param name="text">A single token or whitespace separated tokens.</param>
        /// <returns>Wordpiece tokens.</returns>
        public string[] Tokenize (string text) {
            // Check
            if (text == default)
                throw new ArgumentNullException(nameof(text));
            // Enumerate
            var tokens = text.Trim().Split(' ');
            var result = new List<string>();
            foreach (var token in tokens) {
                // Check length
                if (token.Length > maxCharsPerWord) {
                    result.Add(unknownToken);
                    continue;
                }
                // Enumerate
                var subTokens = new List<string>();
                var start = 0;
                var isBad = false;
                while (start < token.Length) {
                    var currentSubStr = string.Empty;
                    var end = token.Length;
                    while (start < end) {
                        var prefix = start > 0 ? "##" : string.Empty;
                        var subStr = $"{prefix}{token.Substring(start, end - start)}";
                        if (vocabulary.ContainsKey(subStr)) {
                            currentSubStr = subStr;
                            break;
                        }
                        end--;
                    }
                    // The word doesn't contain any known subwords.
                    if (currentSubStr == string.Empty) {
                        isBad = true;
                        break;
                    }
                    // Move to next substring
                    subTokens.Add(currentSubStr);
                    start = end;
                }
                // Add
                if (!isBad)
                    result.AddRange(subTokens);
                else
                    result.Add(unknownToken);
            }
            return result.ToArray();
        }
        #endregion

        
        #region --Operations--
        private readonly IReadOnlyDictionary<string, int> vocabulary;
        private readonly int maxCharsPerWord;

        string ITokenizer.beginningToken => throw new NotImplementedException(@"BERT tokenizer does not use beginning of sentence token");

        string ITokenizer.endToken => throw new NotImplementedException(@"BERT tokenizer does not use end of sentence token");

        string ITokenizer.classifierToken => throw new NotImplementedException(@"BERT wordpiece tokenizer does not use classifier token");

        string ITokenizer.maskToken => throw new NotImplementedException(@"BERT wordpiece tokenizer does not use mask token");

        string ITokenizer.padToken => throw new NotImplementedException(@"BERT wordpiece tokenizer does not use pad token");

        string ITokenizer.separatorToken => throw new NotImplementedException(@"BERT wordpiece tokenizer does not use separator token");

        int[] ITokenizer.Encode (IEnumerable<string> tokens) => throw new NotImplementedException(@"BERT wordpiece tokenizer does not support encoding");

        string[] ITokenizer.Decode (IEnumerable<int> encodings) => throw new NotImplementedException(@"BERT wordpiece tokenizer does not support decoding");

        string ITokenizer.Detokenize (IEnumerable<string> tokens) => throw new NotImplementedException(@"BERT wordpiece tokenizer does not support detokenizing");
        #endregion
    }
}
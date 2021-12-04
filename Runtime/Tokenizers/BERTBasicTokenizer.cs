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
    using System.Linq;
    using System.Text;

    /// <summary>
    /// BERT basic tokenizer.
    /// This tokenizer does basic whitespace and punctuation tokenization.
    /// </summary>
    public sealed class BERTBasicTokenizer : ITokenizer {

        #region --Client API--
        /// <summary>
        /// Create the BERT basic tokenizer.
        /// </summary>
        /// <param name="lowercase">Lowercase all tokens.</param>
        /// <param name="neverSplit">Tokens to never split.</param>
        public BERTBasicTokenizer (bool lowercase = true, string[] neverSplit = null) {
            this.lowercase = lowercase;
            this.neverSplit = neverSplit ?? new string[0];
        }

        /// <summary>
        /// Tokenize a piece of text by white spaces and punctuations.
        /// </summary>
        /// <param name="text">Input text.</param>
        /// <returns>Result tokens.</returns>
        public string[] Tokenize (string text) {
            // Check
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));
            // Whitespace tokenize
            text = Clean(text);
            var tokens = text.Split(' ');
            var buffer = new StringBuilder();
            foreach (var t in tokens) {
                var token = lowercase && !neverSplit.Contains(t) ? t.ToLower() : t;
                var subTokens = SplitOnPunctuation(token, neverSplit);
                foreach (var st in subTokens) {
                    buffer.Append(st);
                    buffer.Append(" ");
                }
            }
            // Join
            var result = buffer.ToString().Trim().Split(' ');
            return result;
        }
        #endregion


        #region --Operations--
        private readonly bool lowercase;
        private readonly string[] neverSplit;

        private static string Clean (string text) {
            var buffer = new StringBuilder();
            foreach (var c in text) {
                if (c == 0x0 || c == 0xfffd || Char.IsControl(c))
                    continue;
                buffer.Append(c);
            }
            return buffer.ToString();
        }

        private static string[] SplitOnPunctuation (string text, string[] neverSplit) {
            // Shortcut
            if (neverSplit.Contains(text))
                return new [] { text };
            // Splot
            var tokens = new List<string>();
            var buffer = new StringBuilder();
            foreach (var t in text) {
                if (char.IsPunctuation(t)) {
                    tokens.Add(buffer.ToString());
                    tokens.Add($"{t}");
                    buffer.Clear();
                }
                else
                    buffer.Append(t);
            }
            if (buffer.Length > 0)
                tokens.Add(buffer.ToString());
            return tokens.ToArray();
        }

        string ITokenizer.beginningToken => throw new NotImplementedException(@"BERT tokenizer does not use beginning of sentence token");

        string ITokenizer.endToken => throw new NotImplementedException(@"BERT tokenizer does not use end of sentence token");

        string ITokenizer.classifierToken => throw new NotImplementedException(@"BERT basic tokenizer does not use classifier token");

        string ITokenizer.maskToken => throw new NotImplementedException(@"BERT basic tokenizer does not use mask token");

        string ITokenizer.padToken => throw new NotImplementedException(@"BERT basic tokenizer does not use pad token");

        string ITokenizer.separatorToken => throw new NotImplementedException(@"BERT basic tokenizer does not use separator token");

        string ITokenizer.unknownToken => throw new NotImplementedException(@"BERT basic tokenizer does not use unknown token");

        int[] ITokenizer.Encode (IEnumerable<string> tokens) => throw new NotImplementedException(@"BERT basic tokenizer does not support encoding");

        string[] ITokenizer.Decode (IEnumerable<int> encodings) => throw new NotImplementedException(@"BERT basic tokenizer does not support decoding");

        string ITokenizer.Detokenize (IEnumerable<string> tokens) => throw new NotImplementedException(@"BERT basic tokenizer does not support detokenizing");
        #endregion
    }
}
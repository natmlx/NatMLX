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

    /// <summary>
    /// BERT tokenizer.
    /// </summary>
    public sealed class BERTTokenizer : ITokenizer {

        #region --Client API--
        /// <summary>
        /// Vocabulary mapping tokens to encodings.
        /// </summary>
        public readonly IReadOnlyDictionary<string, int> vocabulary;

        /// <summary>
        /// BERT classifier token.
        /// </summary>
        public string classifierToken { get; private set; }

        /// <summary>
        /// BERT mask token.
        /// </summary>
        public string maskToken { get; private set; }

        /// <summary>
        /// BERT pad token.
        /// </summary>
        public string padToken { get; private set; }

        /// <summary>
        /// BERT separator token.
        /// </summary>
        public string separatorToken { get; private set; }

        /// <summary>
        /// BERT unknown token.
        /// </summary>
        public string unknownToken { get; private set; }

        /// <summary>
        /// Create the BERT tokenizer.
        /// </summary>
        /// <param name="tokens">BERT vocabulary tokens.</param>
        /// <param name="lowercase">Lowercase all tokens.</param>
        public BERTTokenizer (string[] tokens, bool lowercase = true) : this(CreateVocabullary(tokens), lowercase) { }

        /// <summary>
        /// Create the BERT tokenizer.
        /// </summary>
        /// <param name="vocabulary">BERT vocabulary encoding dictionary.</param>
        /// <param name="lowercase">Lowercase all tokens.</param>
        /// <param name="classifierToken">BERT classifier token.</param>
        /// <param name="maskToken">BERT mask token.</param>
        /// <param name="padToken">BERT pad token.</param>
        /// <param name="separatorToken">BERT separator token.</param>
        /// <param name="unknownToken">BERT unknown token.</param>
        public BERTTokenizer (
            IReadOnlyDictionary<string, int> vocabulary,
            bool lowercase = true,
            string classifierToken = "[CLS]",
            string maskToken = "[MASK]",
            string padToken = "[PAD]",
            string separatorToken = "[SEP]",
            string unknownToken = "[UNK]"
        ) {
            this.vocabulary = vocabulary;
            this.inverseVocabulary = vocabulary.ToDictionary(p => p.Value, p => p.Key);
            this.specialTokens = new [] { classifierToken, maskToken, padToken, separatorToken, unknownToken };
            this.basicTokenizer = new BERTBasicTokenizer(lowercase, specialTokens);
            this.wordpieceTokenizer = new BERTWordpieceTokenizer(vocabulary, unknownToken);
            this.classifierToken = classifierToken;
            this.maskToken = maskToken;
            this.separatorToken = separatorToken;
            this.unknownToken = unknownToken;
        }

        /// <summary>
        /// Tokenize a piece of text into its BERT tokens.
        /// </summary>
        /// <param name="text">Input text.</param>
        /// <returns>BERT tokens.</returns>
        public string[] Tokenize (string text) {
            // Check
            if (string.IsNullOrEmpty(text.Trim()))
                return new string[0];
            // Split on tokens
            var pending = new List<string> { text };
            var subTexts = new List<string>();
            foreach (var token in specialTokens) {
                subTexts = new List<string>();
                foreach (var pendingText in pending) {
                    if (specialTokens.Contains(pendingText))
                        subTexts.Add(pendingText);
                    else
                        subTexts.AddRange(SplitOnToken(pendingText, token));
                }
                pending = subTexts;
            }
            // Tokenize
            return subTexts
                .SelectMany(p => specialTokens.Contains(p) ? new [] { p } : TokenizeSubText(p))
                .ToArray();
        }

        /// <summary>
        /// Encode a series of tokens into their BERT vocabulary ID's.
        /// </summary>
        /// <param name="tokens">Input tokens.</param>
        /// <returns>BERT vocabulary ID's.</returns>
        public int[] Encode (IEnumerable<string> tokens) => tokens
            .Select(t => vocabulary.TryGetValue(t, out var e) ? e : vocabulary[unknownToken])
            .ToArray();

        /// <summary>
        /// Decode a series of BERT vocabulary ID's into tokens.
        /// </summary>
        /// <param name="encodings">BERT vocabulary ID's.</param>
        /// <returns>Result tokens.</returns>
        public string[] Decode (IEnumerable<int> encodings) => encodings
            .Select(e => inverseVocabulary.TryGetValue(e, out var t) ? t : unknownToken)
            .ToArray();

        /// <summary>
        /// Detokenize a series of tokens into a plain string.
        /// </summary>
        /// <param name="tokens">Input tokens.</param>
        /// <returns>Detokenized string.</returns>
        public string Detokenize (IEnumerable<string> tokens) => string
            .Join(" ", tokens)
            .Replace(" ##", string.Empty)
            .Trim();
        #endregion


        #region --Operations--
        private readonly Dictionary<int, string> inverseVocabulary;
        private readonly string[] specialTokens;
        private readonly BERTBasicTokenizer basicTokenizer;
        private readonly BERTWordpieceTokenizer wordpieceTokenizer;

        private string[] TokenizeSubText (string subText) => basicTokenizer
            .Tokenize(subText)
            .SelectMany(wordpieceTokenizer.Tokenize)
            .ToArray();

        private static string[] SplitOnToken (string text, string token) {
            var split = text.Split(new [] { token }, System.StringSplitOptions.RemoveEmptyEntries);
            if (split.Length > 0)
                return split.SelectMany((t, i) => i > 0 ? new [] { token, t.Trim() } : new [] { t.Trim() }).ToArray();
            else
                return new [] { token };
        }

        private static Dictionary<string, int> CreateVocabullary (string[] tokens) => tokens
            .Select((t, i) => (t, i))
            .ToDictionary(p => p.t, p => p.i);
        
        string ITokenizer.beginningToken => throw new NotImplementedException(@"BERT tokenizer does not use beginning of sentence token");

        string ITokenizer.endToken => throw new NotImplementedException(@"BERT tokenizer does not use end of sentence token");
        #endregion
    }
}
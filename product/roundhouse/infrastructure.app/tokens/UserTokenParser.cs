
using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace roundhouse.infrastructure.app.tokens
{
    public class UserTokenParser
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(UserTokenParser));
        public static Dictionary<string, string> Parse(string tokensInput)
        {
            _logger.Info("UserTokens: " + tokensInput);

            if (String.IsNullOrEmpty(tokensInput)) throw new ArgumentNullException("tokensInput");

            if (tokensInput.IndexOf("file", StringComparison.OrdinalIgnoreCase) > -1)
                return ParseFile(tokensInput);

            var dictionary = new Dictionary<string, string>();
            if (tokensInput.IndexOf("+") == -1)
            {
                var keyValue = tokensInput.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (keyValue.Length == 2)
                    dictionary[keyValue[0]] = keyValue[1];
            }
            else
            {
                foreach (var tokens in tokensInput.Split(new[] { "+" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var keyValue = tokens.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (keyValue.Length == 2)
                        dictionary[keyValue[0]] = keyValue[1];
                }
            }
            return dictionary;
        }

        private static Dictionary<string, string> ParseFile(string tokensInput)
        {
            var dictionary = new Dictionary<string, string>();
            var filePath = tokensInput.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1];
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            using (var reader = File.OpenText(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    var keyValue = line.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (keyValue.Length != 2) continue;
                    dictionary[keyValue[0]] = keyValue[1];
                }
            }
            return dictionary;
        }
    }
}
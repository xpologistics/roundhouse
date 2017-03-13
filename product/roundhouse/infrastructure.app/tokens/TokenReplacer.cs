using roundhouse.infrastructure.extensions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace roundhouse.infrastructure.app.tokens
{
    public class TokenReplacer
    {
        public static string replace_tokens(ConfigurationPropertyHolder configuration, string text_to_replace)
        {
            if (string.IsNullOrEmpty(text_to_replace))
                return string.Empty;

            var dictionary = create_dictionary_from_configuration(configuration);
            var regex = new Regex("{{(?<key>\\w+)}}");

            var output = regex.Replace(text_to_replace, m =>
                                                        {
                                                            var key = "";
                                                            key = m.Groups["key"].Value;
                                                            if (!dictionary.ContainsKey(key))
                                                                return "{{" + key + "}}";

                                                            var value = dictionary[key];
                                                            return value;
                                                        });

            return output;
        }

        private static IDictionary<string, string> create_dictionary_from_configuration(ConfigurationPropertyHolder configuration)
        {
            var property_dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in configuration.GetType().GetProperties())
                if (property.Name == "UserTokens")
                {
                    var userTokens = property.GetValue(configuration, null) as Dictionary<string, string>;
                    if (userTokens != null)
                        foreach (var userToken in userTokens)
                            property_dictionary[userToken.Key] = userToken.Value;
                }
                else
                {
                    property_dictionary.Add(property.Name, property.GetValue(configuration, null).to_string());
                }

            return property_dictionary;
        }
    }
}
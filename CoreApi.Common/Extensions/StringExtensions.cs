using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CoreApi.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToBase64Encode(this string str)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(plainTextBytes);
        }
        
        public static string Base64Decode(this string str) {
            var base64EncodedBytes = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static byte[] HexToByteArray(this string hex) {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
        
        public static string ToMaskedString(this string str)
        {
            int textLength = str.Length;
            if (textLength <= 1)
                return str;

            int startIndex;
            switch (textLength)
            {
                case 2:
                    startIndex = 1;
                    break;
                case 3:
                    startIndex = 2;
                    break;

                case 4:
                    startIndex = 3;
                    break;

                default:
                    startIndex = 4;
                    break;
            }

            string newString = str.Substring(0, startIndex);
            newString += "*********" + str.Substring(textLength - startIndex);

            return newString;
        }

        public static string ToMd5(this string str)
        {
            using var md5 = MD5.Create();
            var data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            var sBuilder = new StringBuilder();

            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static string ToLower(this string str, string culture)
        {
            return str.ToLower(new CultureInfo(culture));
        }

        public static string ToTitleCase(this string str, string culture = "en-US")
        {
            var textInfo = new CultureInfo(culture).TextInfo;
            return textInfo.ToTitleCase(str);
        }


        public static string[] ToArraySplitComma(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return new string[0];

            return str.Split(",").Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim()).ToArray();
        }

        public static string CantEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? null : str.Trim();
        }

        public static string FirstCharToUpper(this string input)
        {
            return input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
        }
        
        public static string FirstCharToLower(this string input)
        {
            return input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToLower() + input.Substring(1)
            };
        }

        public static string RemoveFirstNewLines(this string str)
        {
            while (true)
            {
                if (!str.StartsWith(Environment.NewLine)) 
                    return str;
                str = str.Substring(0, Environment.NewLine.Length);
            }
        }
        
        public static string RemoveFirstLines(this string text, int linesCount)
        {
            var lines = Regex.Split(text, "\r\n|\r|\n").Skip(linesCount);
            return string.Join(Environment.NewLine, lines.ToArray());
        }
        
        /// <summary>
        /// Creates a URL And SEO friendly slug
        /// </summary>
        /// <param name="text">Text to slugify</param>
        /// <param name="maxLength">Max length of slug</param>
        /// <returns>URL and SEO friendly string</returns>
        public static string UrlFriendly(this string text, char dash = '-', int maxLength = 0)
        {
            // Return empty value if text is null
            if (text == null) return "";
            var normalizedString = text
                // Make lowercase
                .ToLowerInvariant()
                // Normalize the text
                .Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            var stringLength = normalizedString.Length;
            var prevDash = false;
            var trueLength = 0;
            for (var i = 0; i < stringLength; i++)
            {
                var c = normalizedString[i];
                switch (CharUnicodeInfo.GetUnicodeCategory(c))
                {
                    // Check if the character is a letter or a digit if the character is a
                    // international character remap it to an ascii valid character
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (c < 128)
                            stringBuilder.Append(c);
                        else
                            stringBuilder.Append(RemapInternationalCharToAscii(c));
                        prevDash = false;
                        trueLength = stringBuilder.Length;
                        break;
                    // Check if the character is to be replaced by a hyphen but only if the last character wasn't
                    case UnicodeCategory.SpaceSeparator:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.DashPunctuation:
                    case UnicodeCategory.OtherPunctuation:
                    case UnicodeCategory.MathSymbol:
                        if (!prevDash)
                        {
                            stringBuilder.Append(dash);
                            prevDash = true;
                            trueLength = stringBuilder.Length;
                        }

                        break;
                }

                // If we are at max length, stop parsing
                if (maxLength > 0 && trueLength >= maxLength)
                    break;
            }

            // Trim excess hyphens
            var result = stringBuilder.ToString().Trim(dash);
            // Remove any excess character to meet maxlength criteria
            return maxLength <= 0 || result.Length <= maxLength ? result : result.Substring(0, maxLength);
        }
        
        private static string RemapInternationalCharToAscii(char c)
        {
            var s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }

            if ("èéêëę".Contains(s))
            {
                return "e";
            }

            if ("ìíîïı".Contains(s))
            {
                return "i";
            }

            if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }

            if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }

            if ("çćčĉ".Contains(s))
            {
                return "c";
            }

            if ("żźž".Contains(s))
            {
                return "z";
            }

            if ("śşšŝ".Contains(s))
            {
                return "s";
            }

            if ("ñń".Contains(s))
            {
                return "n";
            }

            if ("ýÿ".Contains(s))
            {
                return "y";
            }

            if ("ğĝ".Contains(s))
            {
                return "g";
            }

            return c switch
            {
                'ř' => "r",
                'ł' => "l",
                'đ' => "d",
                'ß' => "ss",
                'þ' => "th",
                'ĥ' => "h",
                'ĵ' => "j",
                _ => ""
            };
        }
    }
}

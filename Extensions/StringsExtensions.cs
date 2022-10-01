﻿using System.Globalization;
using System.Text.RegularExpressions;

namespace BlogApp.Extensions
{
    public static class StringsExtensions
    {
        public static string Slugify(this string phrase)
        {
            //Remove all accents and make the stirng lower case
            string output = phrase.RemoveAccents().ToLower();


            //Remove all special characters from the string
            output = Regex.Replace(output, @"[^A-Za-z0-9\s-]", "");


            //Remove all additional spaces in favour of just one
            output = Regex.Replace(output, @"\s+", " ").Trim();


            //Replace all spaces with hyphen
            output = Regex.Replace(output, @"\s", "-");

            //Return the slug
            return output;


        }

        private static string RemoveAccents(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
            {
                return phrase;
            }

            //Convert for unicode
            phrase = phrase.Normalize(System.Text.NormalizationForm.FormD);

            //Format unicode/ASCII
            char[] chars = phrase.Where(c => CharUnicodeInfo.GetUnicodeCategory(c)
            != UnicodeCategory.NonSpacingMark).ToArray();

            //Convert and return the new phrase
            return new string(chars).Normalize(System.Text.NormalizationForm.FormC);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imaar.Helper
{
    public static class QueryExtentions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) => self.Select((item, index) => (item, index));
        //public static string RemoveCountryCodeAndZero<T>(this string self)
        //{
        //    var phoneNumber = string.Empty;

        //    if (self.StartsWith(Helpers.GeneralConsts.UaeCode))
        //        phoneNumber = self.Substring(3);
        //    if (self.StartsWith($"+{Helpers.GeneralConsts.UaeCode}"))
        //        phoneNumber = self.Substring(4);
        //    if (self.StartsWith($"00{Helpers.GeneralConsts.UaeCode}"))
        //        phoneNumber = self.Substring(5);
        //    if (self.StartsWith("0"))
        //        phoneNumber = self.Substring(1);
        //    return phoneNumber;
        //}
        //public static string CompletePhoneNumber<T>(this string self)
        //{
        //    var phoneNumber = string.Empty;

        //    phoneNumber = $"{Helpers.GeneralConsts.UaeCode}{self}";

        //    return phoneNumber;
        //}
        //public static string CompletePhoneNumberWithPlus<T>(this string self)
        //{
        //    var phoneNumber = string.Empty;

        //    phoneNumber = $"+{Helpers.GeneralConsts.UaeCode}{self}";

        //    return phoneNumber;
        //}

        public static string ArabicNormalize(this string source)
        {
            // Tanween
            var arr = new List<string>();//remove Tanween
            arr.Add("\u064e");
            arr.Add("\u064b");
            arr.Add("\u064f");
            arr.Add("\u064c");
            arr.Add("\u0650");
            arr.Add("\u064d");
            arr.Add("\u0651");
            arr.Add("\u0652");
            arr.Add("\u007e");

            //remove Tanween
            foreach (var c in arr)
            {
                source = source.Replace(c, "");
            }

            //replace 3ella chars
            var alef = new List<char>();
            alef.Add('ا');
            alef.Add('ء');
            alef.Add('آ');
            alef.Add('أ');
            alef.Add('إ');
            alef.Add('ئ');
            alef.Add('ى');
            alef.Add('ؤ');
            alef.Add('ه');
            alef.Add('ة');
            foreach (var c in alef)
            {
                source = source.Replace(c.ToString(), "ي");
            }
            var skip = new List<string>();
            skip.Add("ال");

            foreach (var c in skip)
            {
                source = source.StartsWith(c) ? source.Substring(c.Length) : source;
            }

            return source;
        }
    }
}

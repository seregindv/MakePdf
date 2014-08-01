using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MakePdf.Galleries;
using MakePdf.Stuff;

namespace MakePdf.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GalleryAttribute : Attribute
    {
        public Type Type { set; get; }
        public string Regex { set; get; }
        public string Title { get; set; }

        public GalleryAttribute()
        {
        }

        private static Tuple<AddressType, GalleryAttribute>[] _decoratedMembers;
        private static Tuple<AddressType, GalleryAttribute>[] GetDecoratedInternal()
        {
            if (_decoratedMembers == null)
                _decoratedMembers = Utils.GetDecoratedEnumMembers<AddressType, GalleryAttribute>();
            return _decoratedMembers;
        }

        public static IEnumerable<AddressType> GetDecorated()
        {
            return GetDecoratedInternal().Select(attr => attr.Item1);
        }

        private static Tuple<AddressType, GalleryAttribute>[] _nonDecoratedMembers;
        public static Tuple<AddressType, GalleryAttribute>[] GetNonDecorated()
        {
            if (_nonDecoratedMembers == null)
                _nonDecoratedMembers = Utils.GetNonDecoratedEnumMembers<AddressType, GalleryAttribute>();
            return _nonDecoratedMembers;
        }

        public static Type[] GetTypes()
        {
            return GetDecoratedInternal()
                .Where(@item => @item.Item2.Type != null)
                .Select(@item => @item.Item2.Type).ToArray();
        }

        public static AddressType GetAddressType(string url)
        {
            var result = Utils.FindFirstField(GetDecoratedInternal(),
                                              @tuple => @tuple.Regex != null &&
                                              System.Text.RegularExpressions.Regex.Match(url, @tuple.Regex,
                                                System.Text.RegularExpressions.RegexOptions.IgnoreCase).Success);
            return result == null ? AddressType.TextGallery : result.Item1;
        }

        public static Gallery CreateGallery(AddressType addressType)
        {
            return (Gallery)Activator.CreateInstance(GetDecoratedInternal().First(@t => @t.Item1 == addressType).Item2.Type);
        }

        public static string GetTitle(AddressType addressType)
        {
            return GetDecoratedInternal().First(@t => @t.Item1 == addressType).Item2.Title;
        }

        public static bool IsGallery(AddressType addressType)
        {
            return GetDecoratedInternal().Any(@t => @t.Item1 == addressType && @t.Item2.Type != null);
        }

        public static IEnumerable<AddressType> GetRegexless()
        {
            return GetDecoratedInternal().Where(@item => @item.Item2.Regex == null).Select(@item => @item.Item1);
        }
    }
}

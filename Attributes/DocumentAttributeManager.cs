using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MakePdf.Stuff;

namespace MakePdf.Attributes
{
    public class DocumentAttributeManager
    {
        Dictionary<Type, Tuple<AddressType, DocumentAttribute>[]> _decoratedMembers;

        public Tuple<AddressType, DocumentAttribute>[] GetDecorated<TAttr>() where TAttr : DocumentAttribute
        {
            if (!_decoratedMembers.ContainsKey(typeof(TAttr)))
            {
                var decorated = Utils.GetDecoratedEnumMembers<AddressType, TAttr>()
                    .Select(@t => new Tuple<AddressType, DocumentAttribute>(@t.Item1, @t.Item2))
                    .ToArray();
                _decoratedMembers[typeof(TAttr)] = decorated;
                return _decoratedMembers[typeof(TAttr)];
            }
            return _decoratedMembers[typeof(TAttr)];
        }

        public Type[] GetTypes<TAttr>() where TAttr : DocumentAttribute
        {
            return GetDecorated<TAttr>().Select(@t => @t.Item2.Type).ToArray();
        }

        public AddressType GetAddressType(string url)
        {
            var result = Utils.FindFirstField(_decoratedMembers.Values.SelectMany(@t => @t).ToArray(),
                                              @docAttr =>
                                              System.Text.RegularExpressions.Regex.Match(url, @docAttr.Regex,
                                                System.Text.RegularExpressions.RegexOptions.IgnoreCase).Success);
            return result == null ? AddressType.Other : result.Item1;
        }
    }
}

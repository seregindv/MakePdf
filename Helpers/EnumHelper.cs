using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MakePdf.Helpers
{
    public static class EnumHelper
    {
        public static Tuple<TEnum, TAttr>[] GetDecoratedEnumMembers<TEnum, TAttr>()
        {
            return GetEnumMembers<TEnum, TAttr>(true);
        }

        public static Tuple<TEnum, TAttr>[] GetNonDecoratedEnumMembers<TEnum, TAttr>()
        {
            return GetEnumMembers<TEnum, TAttr>(false);
        }

        private static Tuple<TEnum, TAttr>[] GetEnumMembers<TEnum, TAttr>(bool? decorated)
        {
            var result =
                from field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static)
                let attrs = field.GetCustomAttributes(typeof(TAttr), true)
                where decorated == null || (decorated.Value && attrs.Length > 0) || (!decorated.Value && attrs.Length == 0)
                select new Tuple<TEnum, TAttr>((TEnum)Enum.Parse(typeof(TEnum), field.Name), (TAttr)attrs[0]);
            return result.ToArray();
        }

        public static Tuple<TEnum, TAttr> FindFirstField<TEnum, TAttr>(Tuple<TEnum, TAttr>[] fields, Func<TAttr, bool> criteria)
        {
            return fields.FirstOrDefault(@field => criteria(@field.Item2));
        }
    }
}

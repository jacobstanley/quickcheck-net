using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QuickCheck.Internal
{
    internal class ReflectionData
    {
        public static Data CreateFrom(object instance)
        {
            if (instance == null)
            {
                return null;
            }

            Type type = instance.GetType();

            if (type.IsPrimitive || instance is string)
            {
                return Data.Value(instance);
            }

            var list = instance as IEnumerable;

            if (list != null)
            {
                return Data.List(list.Cast<object>().Select(CreateFrom));
            }

            const BindingFlags flags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly;

            var fields = Enumerable.Empty<FieldInfo>();
            var ftype = type;

            while (ftype != null)
            {
                fields = fields.Concat(ftype.GetFields(flags));
                ftype = ftype.BaseType;
            }

            var fieldValues = fields
                .Select(x => new { key = x.Name, value = CreateFrom(x.GetValue(instance)) })
                .Select(x => new KeyValuePair<string, Data>(x.key, x.value));

            return Data.Object(type.Name, fieldValues);
        }
    }
}

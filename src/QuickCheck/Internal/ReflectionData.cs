using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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

    public abstract class Data
    {
        private Data()
        {
        }

        public override string ToString()
        {
            return AppendTo(new StringBuilder()).ToString();
        }

        public abstract StringBuilder AppendTo(StringBuilder sb);

        public static Data Value(object x)
        {
            return new DataValue(x);
        }

        public static Data List(IEnumerable<Data> xs)
        {
            return new DataList(xs.ToArray());
        }

        public static Data Object(string type, IEnumerable<KeyValuePair<string, Data>> xs)
        {
            return new DataObject(type, xs.ToArray());
        }

        private sealed class DataValue : Data
        {
            private readonly object m_Primitive;

            public DataValue(object primitive)
            {
                m_Primitive = primitive;
            }

            public override StringBuilder AppendTo(StringBuilder sb)
            {
                if (m_Primitive is string)
                {
                    return sb.Append('"').Append(m_Primitive).Append('"');
                }

                if (m_Primitive is char)
                {
                    return sb.Append('\'').Append(m_Primitive).Append('\'');
                }

                return sb.Append(m_Primitive);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is DataValue && Equals((DataValue)obj);
            }

            private bool Equals(DataValue other)
            {
                return m_Primitive.Equals(other.m_Primitive);
            }

            public override int GetHashCode()
            {
                return m_Primitive.GetHashCode();
            }
        }

        private sealed class DataList : Data
        {
            private readonly Data[] m_Values;

            public DataList(Data[] values)
            {
                m_Values = values;
            }

            public override StringBuilder AppendTo(StringBuilder sb)
            {
                sb.Append("[");

                bool comma = false;
                foreach (var item in m_Values)
                {
                    if (comma)
                    {
                        sb.Append(", ");
                    }

                    item.AppendTo(sb);

                    comma = true;
                }

                return sb.Append("]");
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is DataList && Equals((DataList)obj);
            }

            private bool Equals(DataList other)
            {
                return Sequence.Equals(m_Values, other.m_Values);
            }

            public override int GetHashCode()
            {
                return Sequence.GetHashCode(m_Values);
            }
        }

        private sealed class DataObject : Data
        {
            private readonly string m_Type;
            private readonly KeyValuePair<string, Data>[] m_Fields;

            public DataObject(string type, KeyValuePair<string, Data>[] fields)
            {
                m_Type = type;
                m_Fields = fields;
            }

            public override StringBuilder AppendTo(StringBuilder sb)
            {
                sb.Append(m_Type);

                if (m_Fields.Length == 0)
                {
                    return sb;
                }

                sb.Append(" {");

                bool comma = false;
                foreach (var field in m_Fields)
                {
                    if (comma)
                    {
                        sb.Append(", ");
                    }

                    sb.Append(field.Key);
                    sb.Append(" = ");

                    if (field.Value == null)
                    {
                        sb.Append("null");
                    }
                    else
                    {
                        sb.Append(field.Value);
                    }

                    comma = true;
                }

                return sb.Append("}");
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is DataObject && Equals((DataObject)obj);
            }

            private bool Equals(DataObject other)
            {
                return String.Equals(m_Type, other.m_Type)
                    && Sequence.Equals(m_Fields, other.m_Fields);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (m_Type.GetHashCode() * 397) ^ Sequence.GetHashCode(m_Fields);
                }
            }
        }
    }
}

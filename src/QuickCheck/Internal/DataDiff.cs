using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickCheck.Internal
{
    public abstract class DataDiff
    {
        public virtual bool IsEmpty
        {
            get
            {
                return false;
            }
        }

        public override string ToString()
        {
            return AppendTo(new StringBuilder()).ToString();
        }

        public abstract StringBuilder AppendTo(StringBuilder sb);

        private static readonly DataDiff s_Empty = new EmptyDiff();

        internal static DataDiff Empty
        {
            get
            {
                return s_Empty;
            }
        }

        public abstract DataDiff WithEpsilon(double epsilon);
    }

    internal class EmptyDiff : DataDiff
    {
        public override bool IsEmpty
        {
            get
            {
                return true;
            }
        }

        public override StringBuilder AppendTo(StringBuilder sb)
        {
            return sb.Append("<equal>");
        }

        public override DataDiff WithEpsilon(double epsilon)
        {
            return this;
        }
    }

    internal class GeneralDiff : DataDiff
    {
        private readonly Data m_Old;
        private readonly Data m_New;

        public GeneralDiff(Data old, Data new_)
        {
            m_Old = old;
            m_New = new_;
        }

        public override StringBuilder AppendTo(StringBuilder sb)
        {
            return sb.Append(m_Old).Append(" => ").Append(m_New);
        }

        public override DataDiff WithEpsilon(double epsilon)
        {
            return this;
        }
    }

    internal class ObjectDiff : DataDiff
    {
        private readonly KeyValuePair<string, DataDiff>[] m_Fields;

        public override bool IsEmpty
        {
            get
            {
                return m_Fields.Length == 0;
            }
        }

        private ObjectDiff(KeyValuePair<string, DataDiff>[] fields)
        {
            m_Fields = fields;
        }

        public ObjectDiff(
            IEnumerable<KeyValuePair<string, Data>> olds,
            IEnumerable<KeyValuePair<string, Data>> news)
        {
            var fields = new List<KeyValuePair<string, DataDiff>>();
            var newDict = news.ToDictionary(x => x.Key, x => x.Value);

            foreach (var old in olds)
            {
                Data newValue;

                if (!newDict.TryGetValue(old.Key, out newValue))
                {
                    continue;
                }

                DataDiff diff = old.Value.Diff(newValue);

                if (!diff.IsEmpty)
                {
                    fields.Add(new KeyValuePair<string, DataDiff>(old.Key, diff));
                }
            }

            m_Fields = fields.ToArray();
        }

        public override StringBuilder AppendTo(StringBuilder sb)
        {
            if (m_Fields.Length == 0)
            {
                return sb;
            }

            sb.Append("{");

            bool comma = false;
            foreach (var field in m_Fields)
            {
                if (comma)
                {
                    sb.Append(", ");
                }

                sb.Append(field.Key);
                sb.Append(": ");

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

        public override DataDiff WithEpsilon(double epsilon)
        {
            var fields = m_Fields
                .Select(x => new KeyValuePair<string, DataDiff>(x.Key, x.Value.WithEpsilon(epsilon)))
                .Where(x => !x.Value.IsEmpty)
                .ToArray();

            if (fields.Length == 0)
            {
                return Empty;
            }

            return new ObjectDiff(fields);
        }
    }

    internal class FloatDiff : DataDiff
    {
        private readonly float m_Old;
        private readonly float m_New;

        public FloatDiff(float old, float new_)
        {
            m_Old = old;
            m_New = new_;
        }

        public override StringBuilder AppendTo(StringBuilder sb)
        {
            return sb.Append(m_Old).Append(" => ").Append(m_New);
        }

        public override DataDiff WithEpsilon(double epsilon)
        {
            return Math.Abs(m_Old - m_New) < epsilon ? Empty : this;
        }
    }

    internal class DoubleDiff : DataDiff
    {
        private readonly double m_Old;
        private readonly double m_New;

        public DoubleDiff(double old, double new_)
        {
            m_Old = old;
            m_New = new_;
        }

        public override StringBuilder AppendTo(StringBuilder sb)
        {
            sb.Append(m_Old);

            double diff = m_New - m_Old;
            if (diff >= 0)
            {
                sb.Append(" + ").Append(diff);
            }
            else
            {
                sb.Append(" - ").Append(-diff);
            }

            return sb.Append(" => ").Append(m_New);
        }

        public override DataDiff WithEpsilon(double epsilon)
        {
            return Math.Abs(m_Old - m_New) < epsilon ? Empty : this;
        }
    }
}

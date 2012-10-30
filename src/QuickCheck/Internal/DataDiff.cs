using System;
using System.Collections.Generic;
using System.Linq;

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

        private static readonly DataDiff s_Empty = new EmptyDiff();

        internal static DataDiff Empty
        {
            get
            {
                return s_Empty;
            }
        }

        internal static DataDiff Incompatible(Data old, Data new_)
        {
            return new IncompatibleDiff(old, new_);
        }

        internal static DataDiff Value(object old, object new_)
        {
            Type oldType = old.GetType();
            Type newType = new_.GetType();

            if (oldType == newType)
            {
                if (oldType == typeof(float))
                {
                    return new FloatDiff((float)old, (float)new_);
                }
                if (oldType == typeof(double))
                {
                    return new DoubleDiff((double)old, (double)new_);
                }
            }

            return new ValueDiff(old, new_);
        }

        internal static DataDiff List(Data[] olds, Data[] news)
        {
            throw new NotImplementedException();
        }

        internal static DataDiff Object(
            KeyValuePair<string, Data>[] olds, KeyValuePair<string, Data>[] news)
        {
            var oldDict = olds.ToDictionary(x => x.Key, x => x.Value);
            var newDict = news.ToDictionary(x => x.Key, x => x.Value);

            var removed = new Dictionary<string, Data>();
            var modified = new Dictionary<string, DataDiff>();

            foreach (var old in oldDict)
            {
                Data newValue;
                if (newDict.TryGetValue(old.Key, out newValue))
                {
                    modified.Add(old.Key, old.Value.Diff(newValue));

                    // NOTE: By the end of the loop this will contain the "added" fields
                    newDict.Remove(old.Key);
                }
                else
                {
                    removed.Add(old.Key, old.Value);
                }
            }

            return new ObjectDiff(newDict, removed, modified);
        }

        private class EmptyDiff : DataDiff
        {
            public override bool IsEmpty
            {
                get
                {
                    return true;
                }
            }
        }

        private class IncompatibleDiff : DataDiff
        {
            private readonly Data m_Old;
            private readonly Data m_New;

            public IncompatibleDiff(Data old, Data new_)
            {
                m_Old = old;
                m_New = new_;
            }
        }

        private class ValueDiff : DataDiff
        {
            private readonly object m_Old;
            private readonly object m_New;

            public ValueDiff(object old, object new_)
            {
                m_Old = old;
                m_New = new_;
            }
        }

        private class FloatDiff : DataDiff
        {
            private readonly float m_Old;
            private readonly float m_New;

            public FloatDiff(float old, float new_)
            {
                m_Old = old;
                m_New = new_;
            }
        }

        private class DoubleDiff : DataDiff
        {
            private readonly double m_Old;
            private readonly double m_New;

            public DoubleDiff(double old, double new_)
            {
                m_Old = old;
                m_New = new_;
            }
        }
    }
}

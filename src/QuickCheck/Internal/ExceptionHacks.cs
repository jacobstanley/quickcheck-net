using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace QuickCheck.Internal
{
    public static class ExceptionHacks
    {
        private static readonly FieldInfo Message = typeof(Exception)
            .GetField("_message", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo StackTrace = typeof(Exception)
            .GetField("_stackTrace", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo RemoteStackTrace = typeof(Exception)
            .GetField("_remoteStackTraceString", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Rethrow(this Exception exception)
        {
            exception.RethrowWith(null);
        }

        public static void RethrowWith(this Exception exception, string message)
        {
            if (exception == null)
            {
                return;
            }

            if (message != null)
            {
                string originalMessage = (string)Message.GetValue(exception);
                Message.SetValue(exception, message + "\n" + originalMessage);
            }

            FixAndRethrow(exception);
        }

        private static void FixAndRethrow(Exception exception)
        {
            PreserveStackTrace(exception);

            string remoteStackTrace = (string)RemoteStackTrace.GetValue(exception);
            RemoteStackTrace.SetValue(exception, remoteStackTrace
                .Split('\n')
                .Where(x => !x.StartsWith("   at QuickCheck.") ||
                    x.StartsWith("   at QuickCheck.Examples"))
                .Aggregate((ys, y) => ys + "\n" + y));

            try
            {
                throw exception;
            }
            finally
            {
                StackTrace.SetValue(exception, null);
            }
        }

        private static void PreserveStackTrace(Exception exception)
        {
            var context = new StreamingContext(StreamingContextStates.CrossAppDomain);
            var manager = new ObjectManager(null, context);
            var info = new SerializationInfo(exception.GetType(), new FormatterConverter());

            exception.GetObjectData(info, context);
            manager.RegisterObject(exception, 1, info);
            manager.DoFixups();
        }
    }
}

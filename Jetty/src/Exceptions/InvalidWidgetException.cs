using System;

namespace Jetty.Exceptions
{
    [Serializable]
    public class InvalidWidgetException : Exception
    {
        public InvalidWidgetException() { }
        public InvalidWidgetException(string message) : base(message) { }
        public InvalidWidgetException(string message, Exception inner) : base(message, inner) { }
        protected InvalidWidgetException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

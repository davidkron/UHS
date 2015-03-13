using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    internal class UHSNotCppException : Exception
    {
        public UHSNotCppException()
        {
        }

        public UHSNotCppException(string message) : base(message)
        {
        }

        public UHSNotCppException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UHSNotCppException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
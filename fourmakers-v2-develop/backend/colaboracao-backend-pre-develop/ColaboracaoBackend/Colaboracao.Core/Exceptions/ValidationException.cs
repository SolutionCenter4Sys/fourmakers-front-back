using System;

namespace Colaboracao.Core.Exceptions
{
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationException() : base()
        {
        }
        public ValidationException(string message) : base(message)
        {
        }
        public ValidationException(string message, Exception inner) : base(message, inner)
        {
        }
        protected ValidationException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
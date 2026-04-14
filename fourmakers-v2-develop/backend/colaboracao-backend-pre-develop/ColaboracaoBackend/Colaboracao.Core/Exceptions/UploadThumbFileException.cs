using System;

namespace Colaboracao.Core.Exceptions
{
    [Serializable]
    public class UploadThumbFileException : Exception
    {
        public UploadThumbFileException() : base()
        {
        }
        public UploadThumbFileException(string message) : base(message)
        {
        }
        public UploadThumbFileException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
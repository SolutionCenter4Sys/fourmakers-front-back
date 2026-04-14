using System;

namespace Colaboracao.Core.Exceptions
{
    [Serializable]
    public class UploadFileException : Exception
    {
        public UploadFileException() : base()
        {
        }
        public UploadFileException(string message) : base(message)
        {
        }
        public UploadFileException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
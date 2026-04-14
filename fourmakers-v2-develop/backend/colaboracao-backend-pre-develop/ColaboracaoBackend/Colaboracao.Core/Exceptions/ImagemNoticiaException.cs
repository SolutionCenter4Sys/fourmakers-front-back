using System;

namespace Colaboracao.Core.Exceptions
{
    public class ImagemNoticiaException : Exception
    {
        public ImagemNoticiaException() : base()
        {
        }
        public ImagemNoticiaException(string message) : base(message)
        {
        }
        public ImagemNoticiaException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
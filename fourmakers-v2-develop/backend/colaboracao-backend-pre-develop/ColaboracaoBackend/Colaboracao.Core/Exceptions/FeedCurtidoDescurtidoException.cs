using System;

namespace Colaboracao.Core.Exceptions
{
    public class FeedCurtidoDescurtidoException : Exception
    {
        public FeedCurtidoDescurtidoException() : base()
        {
        }
        public FeedCurtidoDescurtidoException(string message) : base(message)
        {
        }
        public FeedCurtidoDescurtidoException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
using System;

namespace Colaboracao.Core.Exceptions
{
    public class UnidadeInexistenteException : Exception
    {
        public UnidadeInexistenteException() : base()
        {
        }
        public UnidadeInexistenteException(string message) : base(message)
        {
        }
        public UnidadeInexistenteException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
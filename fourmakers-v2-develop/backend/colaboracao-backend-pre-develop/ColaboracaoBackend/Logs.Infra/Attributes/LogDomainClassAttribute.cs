using System;

namespace Logs.Infra.Attributes
{
    /// <summary>
    /// Atributo para marcar classes de serviços da camada Domain que devem ter seus métodos logados automaticamente.
    /// Quando aplicado a uma classe, todos os métodos públicos serão interceptados e logados.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class LogDomainClassAttribute : Attribute
    {
    }
}


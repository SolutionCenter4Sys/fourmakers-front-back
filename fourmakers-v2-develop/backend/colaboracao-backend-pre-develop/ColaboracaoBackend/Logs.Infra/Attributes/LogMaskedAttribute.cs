using System;

namespace Logs.Infra.Attributes
{
    /// <summary>
    /// Atributo para marcar propriedades que devem ser mascaradas nos logs.
    /// Propriedades marcadas com este atributo terão seus valores substituídos por "***" nos logs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LogMaskedAttribute : Attribute
    {
        /// <summary>
        /// Texto de substituição para valores mascarados. Padrão: "***"
        /// </summary>
        public string MaskValue { get; set; } = "***";
    }
}


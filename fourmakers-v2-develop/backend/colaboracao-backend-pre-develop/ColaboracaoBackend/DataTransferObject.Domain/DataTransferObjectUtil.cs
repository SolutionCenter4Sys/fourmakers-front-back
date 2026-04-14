using System;
using System.Reflection;

namespace DataTransferObject.Domain
{
    public static class DataTransferObjectUtil
    {
        /// <summary>
        /// Copia as propriedades de um objeto para outro, mantendo o mesmo nome e tipo.
        /// </summary>
        /// <param name="source">Objeto de origem (cópia)</param>
        /// <param name="destination">Objeto de destino (para onde os valores serão copiados)</param>
        public static void CopiarPropriedades(object source, object destination)
        {
            if (source == null || destination == null)
                throw new ArgumentNullException("Source or destination object is null.");

            // Verifica se ambos os objetos são do tipo correto
            if (source.GetType() != destination.GetType())
                throw new ArgumentException("Source and destination objects must be of the same type.");

            // Obtém todas as propriedades da classe de origem
            var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // Verifica se a propriedade pode ser lida e escrita
                if (property.CanRead && property.CanWrite)
                {
                    var value = property.GetValue(source);
                    property.SetValue(destination, value);
                }
            }
        }
    }
}
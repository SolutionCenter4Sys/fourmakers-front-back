using Logs.Infra.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Logs.Infra.Helpers
{
    /// <summary>
    /// Helper para mascarar propriedades marcadas com LogMaskedAttribute durante a serialização de logs
    /// </summary>
    public static class LogMaskingHelper
    {
        /// <summary>
        /// Mascara propriedades marcadas com LogMaskedAttribute em um objeto antes da serialização
        /// </summary>
        public static object MaskSensitiveData(object obj)
        {
            if (obj == null)
                return null;

            var type = obj.GetType();

            // Se for um tipo primitivo, string ou DateTime, retorna como está
            if (IsSimpleType(type))
                return obj;

            // Se for um dicionário
            if (obj is IDictionary dictionary)
            {
                return MaskDictionary(dictionary);
            }

            // Se for uma coleção/enumerable
            if (obj is IEnumerable enumerable && !(obj is string))
            {
                return MaskEnumerable(enumerable);
            }

            // Se for um objeto complexo, criar um dicionário com propriedades mascaradas
            return MaskObject(obj);
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTime?) ||
                   type == typeof(decimal) ||
                   type == typeof(decimal?) ||
                   type == typeof(Guid) ||
                   type == typeof(Guid?) ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private static object MaskDictionary(IDictionary dictionary)
        {
            var maskedDict = new Dictionary<string, object>();
            
            foreach (DictionaryEntry entry in dictionary)
            {
                var key = entry.Key?.ToString() ?? "null";
                var value = entry.Value;

                // Verificar se a chave do dicionário indica um campo sensível
                if (IsSensitiveKey(key))
                {
                    maskedDict[key] = "***";
                }
                else
                {
                    maskedDict[key] = MaskSensitiveData(value);
                }
            }

            return maskedDict;
        }

        private static object MaskEnumerable(IEnumerable enumerable)
        {
            var list = new List<object>();
            foreach (var item in enumerable)
            {
                list.Add(MaskSensitiveData(item));
            }
            return list;
        }

        private static object MaskObject(object obj)
        {
            var type = obj.GetType();
            var maskedDict = new Dictionary<string, object>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                try
                {
                    var value = property.GetValue(obj);
                    var propertyName = property.Name;

                    // Verificar se a propriedade tem o atributo LogMasked
                    var maskedAttr = property.GetCustomAttribute<LogMaskedAttribute>();
                    if (maskedAttr != null)
                    {
                        maskedDict[propertyName] = maskedAttr.MaskValue;
                    }
                    else
                    {
                        // Verificar se o nome da propriedade indica dados sensíveis
                        if (IsSensitivePropertyName(propertyName))
                        {
                            maskedDict[propertyName] = "***";
                        }
                        else
                        {
                            maskedDict[propertyName] = MaskSensitiveData(value);
                        }
                    }
                }
                catch
                {
                    // Ignorar propriedades que não podem ser lidas
                    maskedDict[property.Name] = "[Não acessível]";
                }
            }

            return maskedDict;
        }

        private static bool IsSensitiveKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            var lowerKey = key.ToLowerInvariant();
            return lowerKey.Contains("senha") ||
                   lowerKey.Contains("password") ||
                   lowerKey.Contains("token") ||
                   lowerKey.Contains("secret") ||
                   lowerKey.Contains("key") ||
                   lowerKey.Contains("credential") ||
                   lowerKey.Contains("auth");
        }

        private static bool IsSensitivePropertyName(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return false;

            var lowerName = propertyName.ToLowerInvariant();
            return lowerName.Contains("senha") ||
                   lowerName.Contains("password") ||
                   lowerName.Contains("token") ||
                   lowerName.Contains("secret") ||
                   lowerName.Contains("key") ||
                   lowerName.Contains("credential") ||
                   lowerName.Contains("auth");
        }
    }
}


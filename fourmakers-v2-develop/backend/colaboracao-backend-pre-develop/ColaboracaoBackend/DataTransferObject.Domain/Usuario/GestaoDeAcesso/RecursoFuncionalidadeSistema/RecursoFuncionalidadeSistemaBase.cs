using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoFuncionalidadeSistema
{
    public class RecursoMenuFuncionalidadeSistemaBase
    {
        public int TbFuncionalidadeSistemaId { get; set; }
        [JsonIgnore]
        public string CodigoRecursoMenu { get; set; }
    }
}
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoFuncionalidadeSistema;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoMenu;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso
{
    public class RecursoResult : RecursoBase
    {
        public RecursoMenuResult Menu { get; set; }
        public List<RecursoFuncionalidadeSistemaResult> FuncionalidadesSistema { get; set; } = new();
        [JsonIgnore]
        public string CodigoRecursoMenuTemp { get; set; }

        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}
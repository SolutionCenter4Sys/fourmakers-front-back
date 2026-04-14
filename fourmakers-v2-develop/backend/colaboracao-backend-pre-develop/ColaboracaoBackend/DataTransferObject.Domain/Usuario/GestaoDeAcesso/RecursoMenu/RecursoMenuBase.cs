using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoMenu
{
    public class RecursoMenuBase
    {
        public string NomeMenu { get; set; }
        
        [JsonIgnore]
        public string CodigoRecurso { get; set; }
        public string CodigoRecursoMenu { get; set; }
        public string CodigoRecursoMenuPai { get; set; }
        public string CodigoIcone { get; set; }
        public string TipoMenu { get; set; }
        public string LinkExternoNovaPagina { get; set; }
        public float Ordenacao { get; set; }
        public bool EmBreve { get; set; }
        public bool Visivel { get; set; }
    }
}
using DataTransferObject.Domain.Projeto.GestorExterno;
using DataTransferObject.Domain.Util;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoMenu
{
    public class RecursoMenuInput : RecursoMenuBase
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public int OrgId { get; set; }
        [JsonIgnore]
        public bool Ativo { get; set; }
        [JsonIgnore]
        public string CodigoInternoColaboradorAlteracao { get; set; }

        public void AtualizarPropriedadesDaClasseBase(RecursoMenuBase recursoMenuBase)
        {
            this.AtualizarSafeComPropriedadesDe(recursoMenuBase); //Util padrão do DataTransferObject.Domain.Util
        }

        public void ConfigurarParaPersistencia(int orgId, string codigoInternoColaboradorAlteracao, Guid id)
        {
            Ativo = true;
            OrgId = orgId;
            CodigoInternoColaboradorAlteracao = codigoInternoColaboradorAlteracao;
            Id = id;
        }

    }
}

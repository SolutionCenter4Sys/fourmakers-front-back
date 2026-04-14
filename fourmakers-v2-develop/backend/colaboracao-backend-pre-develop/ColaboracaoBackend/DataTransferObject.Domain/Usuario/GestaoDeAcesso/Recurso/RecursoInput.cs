using DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoFuncionalidadeSistema;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoMenu;
using DataTransferObject.Domain.Util;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso
{
    public class RecursoInput : RecursoBase
    {

        [JsonIgnore]
        public bool Ativo { get; set; }
        [JsonIgnore]
        public string CodigoInternoColaboradorAlteracao { get; set; }

        public RecursoMenuInput Menu { get; set; }
        public List<RecursoMenuFuncionalidadeSistemaInput> FuncionalidadesSistema { get; set; } = new();

        public void AtualizarPropriedadesDaClasseBase(RecursoBase recursoBase)
        {
            this.AtualizarSafeComPropriedadesDe(recursoBase); //Util padrão do DataTransferObject.Domain.Util
        }

        public void ConfigurarParaPersistencia(string codigoInternoColaboradorAlteracao, string codigoRecurso)
        {
            Ativo = true;
            CodigoInternoColaboradorAlteracao = codigoInternoColaboradorAlteracao;
            CodigoRecurso = codigoRecurso;
            Menu.CodigoRecurso = codigoRecurso;
            FuncionalidadesSistema.ForEach(s => s.CodigoRecursoMenu = Menu.CodigoRecursoMenu);
        }

    }
}

using DataTransferObject.Domain.Projeto.GestorExterno;
using DataTransferObject.Domain.Util;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Banco.CadastroBanco
{
    public class CadastroBancoInput : CadastroBancoBase
    {

        [JsonIgnore]
        public bool Ativo { get; set; } = true;

        public void AtualizarPropriedadesDaClasseBase(CadastroBancoBase cadastroBancoBase)
        {
            this.AtualizarSafeComPropriedadesDe(cadastroBancoBase); //Util padrão do DataTransferObject.Domain.Util
        }

    }
}

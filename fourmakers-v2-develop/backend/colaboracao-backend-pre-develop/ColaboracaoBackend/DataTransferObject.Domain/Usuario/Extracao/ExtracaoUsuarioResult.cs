using System;

namespace DataTransferObject.Domain.Usuario.Extracao
{
    public class ExtracaoUsuarioResult
    {
        public string ColaboradorID { get; set; }
        public DateTime DataAdmissao { get; set; }
        public string NomeColaborador { get; set; }
        public string Cpf { get; set; }
        public string EmailCorporativo { get; set; }
        public string Diretoria { get; set; }
        public string Departamento { get; set; }
        public string CodigoGestorHierarquico { get; set; }
        public string NomeGestorHierarquico { get; set; }
    }
}
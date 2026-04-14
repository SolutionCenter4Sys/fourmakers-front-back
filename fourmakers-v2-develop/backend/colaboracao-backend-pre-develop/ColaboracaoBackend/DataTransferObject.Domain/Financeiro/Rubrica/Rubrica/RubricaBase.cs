using System;

namespace DataTransferObject.Domain.Financeiro.Rubrica.Rubrica
{
    public class RubricaBase
    {
        public string Descricao { get; set; }
        public string RubricaTipo { get; set; }
        public string CalculoTipo { get; set; }
        public string CodigoRubrica { get; set; }
        public int OrgId { get; set; }
        public bool RefletirContabil { get; set; } = true;
    }
}
using System;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ClienteOrg
{
    public class ClienteOrgDaGestaoAlocadosResult
    {
        public Guid Id { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public int QtdAlocados { get; set; }
        public int QtdGestoresSemPerfil { get; set; }
        public int QtdGestores { get; set; }
    }
}
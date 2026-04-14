using DataTransferObject.Domain.Colaborador;

namespace Colaborador.API.DTOs
{
    public class InserirDadosPCDParam
    {
        public EnumPCD PCD { get; set; }
        public bool grupoDeRisco { get; set; }
        public string DescricaoCondicaoDeSaude { get; set; }
    }
}
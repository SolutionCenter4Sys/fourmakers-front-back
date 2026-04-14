using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil
{
    public class CadastrarPerfisLegadosEmLoteDTO
    {
        public string TbGestorExternoPerfilId { get; set; }
        public bool? Processado { get; set; }
        public DateTime? DataInicioProcessamento { get; set; }
        public DateTime? DataFimProcessamento { get; set; }
        public string Obs { get; set; }
    }
}
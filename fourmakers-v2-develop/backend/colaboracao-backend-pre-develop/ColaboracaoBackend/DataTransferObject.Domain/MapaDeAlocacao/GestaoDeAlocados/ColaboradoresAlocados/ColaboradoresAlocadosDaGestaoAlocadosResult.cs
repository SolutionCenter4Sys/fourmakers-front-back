using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao.Aderencia;
using DataTransferObject.Domain.Match;
using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ColaboradoresAlocados
{
    public class ColaboradoresAlocadosDaGestaoAlocadosResult
    {
        public string CodigoInternoColaborador { get; set; }
        public string Colaborador { get; set; }
        public Guid? PerfilId { get; set; }
        public string Perfil { get; set; }
        public string CodGestorCliente { get; set; }
        public string GestorCliente { get; set; }
        public string CodigoInternoGestorAdm { get; set; }
        public string GestorAdm { get; set; }
        public string CodGestorOperacional { get; set; }
        public string GestorProjeto { get; set; }
        public CalculoAderenciaDTO Aderencia { get; set; }
        public CandidatosMatchResponse RetornoMatch { get; set; }
    }
}
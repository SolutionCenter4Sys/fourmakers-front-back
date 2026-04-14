using DataTransferObject.Domain.Match;
using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados
{
    // ──────────────── Minha Equipe ────────────────
    public class MinhaEquipeDTO
    {
        // Dados do gestor
        public string CodigoInternoColaboradorGestorAdm { get; set; }
        public string NomeGestorAdm { get; set; }

        // Dados do subordinado
        public string CodigoInternoColaborador { get; set; }
        public string NomeColaborador { get; set; }

        // Dados da alocação
        public string CodigoProjeto { get; set; }
        public int IdAlocacao { get; set; }

        public string CodigoGestorOperacional { get; set; }
        public string NomeGestorOperacional { get; set; }

        public List<MEClientesAlocacaoDTO> Clientes { get; set; } = new();
    }

    // ──────────────── Para uso em Listas Clientes ────────────────
    public class MEClientesAlocacaoDTO
    {
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string PerfilId { get; set; }
        public string Perfil { get; set; }
        public string CodGestorCliente { get; set; }
        public string NomeGestorCliente { get; set; }
        public List<MEHabilidadesDTO> Habilidades { get; set; } = new();
    }

    // ──────────────── Listas Auxiliares ────────────────
    public class MEHabilidadesDTO
    {
        public string CodigoCliente { get; set; }
        public int? TipoPerfilId { get; set; }
        public string TipoPerfil { get; set; }
        public int? SkillId { get; set; }
        public string Habilidade { get; set; }
        public int? SenioridadeId { get; set; }
        public string Senioridade { get; set; }
        public int? Interesse { get; set; }
    }


    public class MinhaEquipeAderenciaDTO
    {
        // Dados do subordinado
        public string CodigoInternoColaborador { get; set; }
        public string NomeColaborador { get; set; }

        // Dados da alocação
        public string CodigoProjeto { get; set; }
        public int IdAlocacao { get; set; }
        public List<ClientesAlocacaoAderenciaDTO> Clientes { get; set; } = new();
    }
    public class ClientesAlocacaoAderenciaDTO
    {
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string PerfilId { get; set; }
        public string Perfil { get; set; }
        public string CodGestorCliente { get; set; }
        public string NomeGestorCliente { get; set; }
        public List<ResultadoIndicadoresDTO> ResultadoHabilidades { get; set; } = new();
        public CandidatosMatchResponse RetornoMatch { get; set; }
    }

    public class ResultadoIndicadoresDTO
    {
        public bool Pendencia { get; set; }
        public int? PerfilTipoId { get; set; }
        public string TipoPerfil { get; set; }
        public string Habilidade { get; set; }
        public string ColaboradorNivel { get; set; }
        public string VagaNivel { get; set; }
        public int? Interesse { get; set; }
        public string Status { get; set; }

    }
    public class GestorCandidatosMatchResponse
    {
        public string CodigoGestor { get; set; }
        public string NomeGestor { get; set; }

        public List<CandidatosMatchResponse> RetornoMatch { get; set; } = new();
    }

    public class GestorCandidatosMatchResponsePerfil
    {
        public string CodigoGestorAdm { get; set; }
        public string NomeGestorAdm { get; set; }
        public string CodigoGestorOper { get; set; }
        public string NomeGestorOper { get; set; }

        public List<CandidatoComPerfilResponse> RetornoMatch { get; set; } = new();
    }

    public class GestorColaboradoresSkill
    {
        public string CodigoGestorAdm { get; set; }
        public string NomeGestorAdm { get; set; }
        public List<MinhaEquipeAderenciaDTO> Colaboradores { get; set; } = new();
    }

    public class GestorColaboradoresSkillAdmOper 
    {
        public string CodigoGestorAdm { get; set; }
        public string NomeGestorAdm { get; set; }
        public string CodigoGestorOperacional { get; set; }
        public string NomeGestorOperacional { get; set; }
        public List<GestorOperacionalDTO> GestoresOperacionais { get; set; } = new();
        public List<MinhaEquipeAderenciaDTO> Colaboradores { get; set; } = new();
    }
    public class GestorOperacionalDTO
    {
        public string CodigoGestorOperacional { get; set; }
        public string NomeGestorOperacional { get; set; }
    }
    public class TotalizacaoIndicadoresPorGestorAdmOper
    {
        public int TotColaboradores { get; set; }
        public int TotPendentesSkills { get; set; }
        public double MediaMatch { get; set; }
    }
}

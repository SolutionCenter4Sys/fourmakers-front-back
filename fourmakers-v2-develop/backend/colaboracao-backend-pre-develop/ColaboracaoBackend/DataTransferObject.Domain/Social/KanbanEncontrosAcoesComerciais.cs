using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Projeto.GestorExterno;
using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain
{
    public class KanbanEncontrosAcoesComerciais
    {
        public ReunioesAgendadas ReunioesAgendadas { get; set; } = new();
        public ReunioesRealizadas ReunioesRealizadas { get; set; } = new();
        public AcoesAndamento AcoesAndamento { get; set; } = new();
        public AcoesPendentes AcoesPendentes { get; set; } = new();
        public AcoesRealizadas AcoesRealizadas { get; set; } = new();
    }

    public class ReunioesAgendadas
    {
        public CabecalhoReunioesAcoes Cabecalho { get; set; }
        public List<AgendaComercial> Reunioes { get; set; }
    }

    public class ReunioesRealizadas
    {
        public CabecalhoReunioesAcoes Cabecalho { get; set; }
        public List<AgendaComercialId> Reunioes { get; set; }
    }

    public class AcoesAndamento
    {
        public CabecalhoReunioesAcoes Cabecalho { get; set; }
        public List<AcaoKanbanBody> Acoes { get; set; }

    }

    public class AcoesPendentes
    {
        public CabecalhoReunioesAcoes Cabecalho { get; set; }
        public List<AcaoKanbanBody> Acoes { get; set; }
    }

    public class AcoesRealizadas
    {
        public CabecalhoReunioesAcoes Cabecalho { get; set; }
        public List<AcaoKanbanBody> Acoes { get; set; }
    }

    public class CabecalhoReunioesAcoes
    {
        public int Total { get; set; }
        public double TotalExpirando { get; set; }
        public int TotalAtrasados { get; set; }
        public double Valor { get; set; }
    }

    public class AgendaComercial
    {
        public int AgendaId { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Responsavel { get; set; }
        public DateTime Prazo { get; set; }
        public int PendenciasAbertas { get; set; }
        public string Status { get; set; }
        public string ClienteId { get; set; }
        public string Cliente { get; set; }
        public string Localizacao { get; set; }
        //public int Vencendo { get; set; }
        public List<GestorCliente> GestoresCliente { get; set; }
        public ParticipantesAgenda ParticipantesAgenda { get; set; } = new();
        public List<InteracaoComCategoriaDTO> InteracaoCategoria { get; set; }

    }
    public class AgendaComercialId
    {
        public int Id { get; set; }
        public int AgendaId { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Responsavel { get; set; }
        public DateTime Prazo { get; set; }
        public int PendenciasAbertas { get; set; }
        public string Status { get; set; }
        public string ClienteId { get; set; }
        public string Cliente { get; set; }
        public string Localizacao { get; set; }
        public int Atrasado { get; set; }
        public int Vencendo { get; set; }
        public List<GestorCliente> GestoresCliente { get; set; }
        public ParticipantesAgenda ParticipantesAgenda { get; set; } = new();
        public List<InteracaoComCategoriaSubDTO> InteracaoCategoria { get; set; }
    }

    public class AcaoKanbanBody
    {
        public int Id { get; set; }
        public  int InteracaoAiId { get; set; }
        public int AgendaId { get; set; }
        public int InteracaoId { get; set; }
        public string Texto { get; set; }
        public string CodInternoColaborador { get; set; }
        public string NomeResponsavel { get; set; }
        public DateTime? DataLimite { get; set; }
        public int StatusAcoes { get; set; }
        //public string ComentarioAcao { get; set; }
        public bool Atrasado { get; set; }
        public bool Vencendo { get; set; }
        public List<InteracaoComCategoriaSubDTO> InteracaoCategoria { get; set; }
    }

    public class ParticipantesAgenda
    {
        public List<GestorCliente> GestoresExterno { get; set; }
        public List<ColaboradorAgenda> Colaboradores { get; set; }
        public List<ParticipanteExterno> ParticipantesExterno { get; set; }
    }

    public class GestorCliente
    {
        public string CodigoGestorExterno { get; set; }
        public string Nome { get; set; }
    }
} 
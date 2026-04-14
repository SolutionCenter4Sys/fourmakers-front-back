using System;

namespace DataTransferObject.Domain.Vaga
{
    /// <summary>
    /// Um período em que a vaga permaneceu em um status: data que chegou, data que saiu (se houver), descrição/código do status, tempo e usuário que alterou.
    /// </summary>
    public class HistoricoStatusVagaItemDTO
    {
        /// <summary>Data em que a vaga chegou neste status.</summary>
        public DateTime DataQueChegouNesteStatus { get; set; }

        /// <summary>Data em que a vaga saiu deste status. Nulo se ainda estiver neste status (último da lista).</summary>
        public DateTime? DataQueSaiuDesteStatus { get; set; }

        /// <summary>Descrição do status.</summary>
        public string StatusDescricao { get; set; }

        /// <summary>Código do status.</summary>
        public int StatusCod { get; set; }

        /// <summary>Tempo que a vaga ficou neste status.</summary>
        public TimeSpan TempoQueFicouNesteStatus { get; set; }

        /// <summary>Descrição legível do tempo (ex: "2d 5h 30m").</summary>
        public string TempoQueFicouNesteStatusFormatado { get; set; }

        /// <summary>Nome do colaborador que realizou a alteração (que levou a vaga a este status).</summary>
        public string NomeUsuarioAlterador { get; set; }
    }
}

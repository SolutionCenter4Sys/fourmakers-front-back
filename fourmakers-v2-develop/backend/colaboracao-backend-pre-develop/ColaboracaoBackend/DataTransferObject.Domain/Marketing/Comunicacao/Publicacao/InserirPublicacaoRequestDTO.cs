using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class InserirPublicacaoRequestDTO
    {
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        /// <summary>Subtítulo ou headline da publicação (segundo título).</summary>
        public string Subtitulo { get; set; }
        public string Conteudo { get; set; }
        /// <summary>Pasta lógica para agrupamento no feed (opcional).</summary>
        public string Pasta { get; set; }
        public List<string> Labels { get; set; } = new List<string>();
        /// <summary>Disponível apenas para publicações do tipo <c>documento</c> (equivalente a labels do documento).</summary>
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime? DataAgendamentoPublicacao { get; set; }
        public DateTime? DataValidadePublicacao { get; set; }
        /// <summary>Quando informado, a publicação fica restrita à comunidade. Não pode ser usado junto com Grupos.</summary>
        public string ComunidadeId { get; set; }
        public List<string> Grupos { get; set; } = new List<string>();
        public List<PublicacaoAnexoInputDTO> Anexos { get; set; } = new List<PublicacaoAnexoInputDTO>();
        public PublicacaoConfiguracaoInteracaoDTO ConfiguracaoInteracao { get; set; }
        /// <summary>Autoria da publicação: 'pessoal' = exibe criador; 'alternativo' = exibe nome em Autor.NomeAutorAlternativo (config da org). Padrão: pessoal.</summary>
        public string AutoriaTipo { get; set; } = "pessoal";
    }
}

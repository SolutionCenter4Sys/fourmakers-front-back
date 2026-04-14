using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class ObterListaPublicacaoGeralRequestDTO
    {
        [JsonPropertyName("somenteLeituraObrigatoria")]
        public bool SomenteLeituraObrigatoria { get; set; }
        public string Tipo { get; set; }
        public List<string> Labels { get; set; }
        /// <summary>Filtro por nomes de tag (publicações tipo documento).</summary>
        public List<string> Tags { get; set; }
        public List<string> Status { get; set; }
        public List<string> StatusAprovacao { get; set; }
        public string ComunidadeId { get; set; }
        /// <summary>Quando true, retorna apenas publicações não ocultas no feed (ocultar_no_feed = 0). Padrão: false.</summary>
        [JsonPropertyName("somenteNaoOcultoNoFeed")]
        public bool SomenteNaoOcultoNoFeed { get; set; }

        /// <summary>
        /// Quando true, retorna apenas publicações com publicacao_oficial = 1 (comunicados da organização sem comunidade vinculada).
        /// Visibilidade por grupo e demais filtros do feed continuam valendo. ComunidadeId não se aplica a essas publicações.
        /// </summary>
        [JsonPropertyName("somentePublicacaoOficial")]
        public bool SomentePublicacaoOficial { get; set; }
    }
}

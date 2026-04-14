using Competencia.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Competencia
{
    public class SkillsLog
    {
        public string Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string GestorExternoPerfil { get; set; }
        public string CodigoInternoColaboradorLogado { get; set; }
        public long SkillId { get; set; }
        public ItemPerfilEnum ItemPerfil { get; set; }
        public long? NivelId { get; set; }
        public EnumSkillsMovimentacao SkillsMovimentacaoId { get; set; }
        public bool LogAutomatico { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
    }

    public enum EnumSkillsMovimentacao
    {
        ADICIONADO_PERFIL = 1,
        NAO_INTERESSADO = 2,
        ADICIONADO_PDI = 3,
        SUGERIDA = 4,
        ATUALIZADO = 5,
        INTERESSADO = 6
    }
}

using System;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Linha de resultado da query do mural de reconhecimento (mapeamento interno do repositório).
    /// Id e códigos de colaborador como Guid para compatibilidade com o driver MySQL (CHAR(36)).
    /// </summary>
    public class Feedback360MuralCardRowDTO
    {
        public Guid Id { get; set; }
        public Guid CodigoInternoColaboradorRemetente { get; set; }
        public string NomeRemetente { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataInteracao { get; set; }
        public string Situacao { get; set; }
        public string Tarefa { get; set; }
        public string Acao { get; set; }
        public string Resultado { get; set; }
        public string Previa { get; set; }
        public string Relacionamento { get; set; }
    }
}

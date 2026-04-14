using System;

namespace DataTransferObject.Domain.Colaborador
{
    public class ErroProcessamentoCurriculoDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public string IdLote { get; set; }
        public string MessageReceiptHandle { get; set; }
        public string MensagemErro { get; set; }
        public string StackTrace { get; set; }
        public DateTime? DataOcorrenciaErro { get; set; }
        public string BodyMensagem { get; set; }
        public bool ProcessadoComErro { get; set; }
        public string NomeArquivoCv { get; set; }
    }
} 
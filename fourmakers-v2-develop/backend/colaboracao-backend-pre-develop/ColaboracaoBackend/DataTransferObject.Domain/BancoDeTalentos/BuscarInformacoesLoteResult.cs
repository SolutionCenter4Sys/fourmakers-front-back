using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Colaborador;

namespace DataTransferObject.Domain.BancoDeTalentos
{
    public class BuscarInformacoesLoteResult
    {
        public ProcessamentoCurriculoLoteDTO Lote { get; set; }
        public IEnumerable<BuscarPessoasQueEuCadastrei> PessoasCadastradasNesteLote { get; set; }
        public IEnumerable<ErroProcessamentoCurriculoDTO> Erros { get; set; }
        public IEnumerable<string> PdfsAProcessar { get; set; }
    }
}
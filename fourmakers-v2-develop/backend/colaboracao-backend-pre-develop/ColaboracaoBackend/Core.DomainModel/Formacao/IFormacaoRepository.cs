using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Formacao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Formacao
{
    public interface IFormacaoRepository
    {
        FormacaoDTO GetModel(FormacaoDTO formacao);
        FormacaoDTO GetFormacaoByCpf(string key);
        FormacaoDTO GetFormacaoById(int formacaoId);
        Task<List<FormacaoSumarioDTO>> ListarFormacaoSumario(int orgId);
        FormacaoDTO AddFormacao(FormacaoDTO formacao, string cpf);
        List<FormacaoDTO> ListarFormacao(string busca, int cursor, int limite);
        long? GetFormacaoColabRowId(long formacaoColaboradorId);
        long SaveCertificado(CertificadoDTO certificado, long formacaoColaboradorId);
        FormacaoColaboradorDTO AlterarFormacaoColaborador(FormacaoColaboradorDTO formacaoColab);
    }
}
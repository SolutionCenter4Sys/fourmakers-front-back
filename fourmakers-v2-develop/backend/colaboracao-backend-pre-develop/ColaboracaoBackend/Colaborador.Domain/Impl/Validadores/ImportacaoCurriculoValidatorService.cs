using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.Vaga;
using Logs.Infra.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Validadores
{
    [LogDomainClass]
    public class ImportacaoCurriculoValidatorService : IImportacaoCurriculoValidatorService
    {
        public async Task ValidaImportarColaboradorLinkedin(string perfilIN, int orgId, string codigoInternoColaboradorCadastrante)
        {
            var campos = new List<CampoValidacao>();

            campos.Add(new("perfilIN", perfilIN, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("orgId", orgId, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("codigoInternoColaboradorCadastrante", codigoInternoColaboradorCadastrante, TipoValidacaoEnum.Obrigatoriedade));

            await ValidadorCamposUtil.ValidaCampos(campos);
        }
    }
}
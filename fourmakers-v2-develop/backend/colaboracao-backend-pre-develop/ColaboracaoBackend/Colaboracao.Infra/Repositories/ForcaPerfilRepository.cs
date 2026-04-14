using Colaboracao.Infra.Context;
using Core.Domain;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class ForcaPerfilRepository : IForcaPerfilRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public ForcaPerfilRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public int GetNumeroCertificadosCompetencia(string cpf)
        {
            return _colaboradorContext.tb_colaborador_competencia.Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1 && x.tb_colaborador_competencia_certificado.Where(y => y.ativo == 1).Any()).Count();
        }

        public int GetQuantidadeCompetencia(string cpf)
        {
            return _colaboradorContext.tb_colaborador_competencia.Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1).Count();
        }

        public int GetQuantidadeExperienciaProfissional(string cpf)
        {
            return _colaboradorContext.tb_experiencia.Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1).Count();
        }

        public int GetQuantidadeSoftskill(string cpf)
        {
            return _colaboradorContext.tb_colaborador_softskill.Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1).Count();
        }

        public bool PossuiCurriculo(string cpf)
        {
            return false;
        }

        public bool PossuiDominioNegocio(string cpf)
        {
            return _colaboradorContext.tb_colaborador_dominionegocio.Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1).Any();
        }

        public bool PossuiEscolaridade(string cpf)
        {
            return _colaboradorContext.tb_escolaridade.Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1).Any();
        }

        public bool PossuiIdioma(string cpf)
        {
            return _colaboradorContext.tb_colaborador_idioma.Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1).Any();
        }

        public bool PossuiMetodologia(string cpf)
        {
            return _colaboradorContext.tb_colaborador_metodologia.Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1).Any();
        }
    }
}
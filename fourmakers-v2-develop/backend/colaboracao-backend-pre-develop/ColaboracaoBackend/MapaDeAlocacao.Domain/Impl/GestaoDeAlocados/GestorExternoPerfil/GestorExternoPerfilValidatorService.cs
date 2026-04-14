using System;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Competencia.Domain.Enums;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Core.Domain.SRS;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados.GestorExternoPerfil
{
    [LogDomainClass]
    public class GestorExternoPerfilValidatorService : IGestorExternoPerfilValidatorService
    {
        private readonly IGestorExternoPerfilRepository _gestorExternoPerfilRepository;
        private readonly IGestorExternoRepository _gestorExternoRepository;
        private readonly IGestaoAlocadosRepository _gestaoAlocadosRepository;
        private readonly IAdmissaoCargoRepository _admissaoCargoRepository;

        public GestorExternoPerfilValidatorService(IGestorExternoPerfilRepository gestorExternoPerfilRepository,
                                                   IGestorExternoRepository gestorExternoRepository,
                                                   IGestaoAlocadosRepository gestaoAlocadosRepository,
                                                   IAdmissaoCargoRepository admissaoCargoRepository)
        {
            _gestorExternoPerfilRepository = gestorExternoPerfilRepository;
            _gestorExternoRepository = gestorExternoRepository;
            _gestaoAlocadosRepository = gestaoAlocadosRepository;
            _admissaoCargoRepository = admissaoCargoRepository;
        }

        public async Task ValidaGestorExternoPerfil(GestorExternoPerfilInput input, CRUDEnum cRUDEnum)
        {
            if (cRUDEnum == CRUDEnum.Update || cRUDEnum == CRUDEnum.Delete)
            {
                await ValidaSeExisteGestorExternoPerfil(input.Id);
            }

            if (cRUDEnum == CRUDEnum.Create || cRUDEnum == CRUDEnum.Update)
            {
                await ValidarCamposDeEntrada(input, cRUDEnum); // faz as validações padrões

                await ValidaSeJaExisteGestorExternoPerfil(input.NomePerfil, input.CodGestorExterno, input.OrgId, input.Id);

                await ValidaSeExisteGestorExterno(input.CodGestorExterno, input.OrgId);

                this.ValidaCidadeEstado(input);
                this.ValidaSeExistePermanenciaInfornada(input);
                this.ValidaSeExisteModeloTrabalhoInfornado(input);
                this.ValidaSeExisteModeloProfissionalLocalidadeInfornado(input);
                this.ValidaParaNaoTerSkillDuplicada(input);
                this.ValidarSkills(input);
            }
        }

        private async Task ValidarCamposDeEntrada(GestorExternoPerfilInput input, CRUDEnum cRUDEnum)
        {
            var campos = new List<CampoValidacao>();

            if (cRUDEnum == CRUDEnum.Update)
            {
                campos.Add(new("Id", input.Id, TipoValidacaoEnum.Obrigatoriedade));
            }
            ;
            campos.Add(new("Nome Perfil", input.NomePerfil, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Código do  Gestor", input.CodGestorExterno, TipoValidacaoEnum.Obrigatoriedade));

            campos.Add(new("Código do Gestor", input.CodGestorExterno, TipoValidacaoEnum.ValidarFormatoCodigo));

            campos.Add(new("ModeloTrabalhoId", input.ModeloTrabalhoId, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("ModeloTrabalhoId", input.ModeloTrabalhoId, TipoValidacaoEnum.TamanhoExato) { TamanhoExato = 36 });
            campos.Add(new("PermanenciaId", input.PermanenciaId, TipoValidacaoEnum.TamanhoExato) { TamanhoExato = 36 });
            campos.Add(new("ProfissionalLocalidadeId", input.ProfissionalLocalidadeId, TipoValidacaoEnum.TamanhoExato) { TamanhoExato = 36 });

			campos.Add(new("Custo Perfil", input.CustoPerfil, TipoValidacaoEnum.ValorMaiorQueZero));

			await ValidadorCamposUtil.ValidaCampos(campos);
        }

        private void ValidaParaNaoTerSkillDuplicada(GestorExternoPerfilInput input)
        {
            var skills = input.GestorExternoPerfilSkills;

            if (skills == null || !skills.Any())
                return;

            var gruposDuplicados = skills
                .GroupBy(skill => new
                {
                    ItemPerfilId = skill.ItemPerfil?.Id,
                    SkillId = skill.Skill?.Id,
                    NivelId = skill.Nivel?.Id,
                    Descricao = skill.Skill.Descricao
                })
                .Where(group => group.Count() > 1);

            if (gruposDuplicados.Any())
            {
                throw new ApplicationException("Existem skills duplicadas na entrada. Verifique as combinações de ItemPerfil, Skill e Nivel.");
            }
        }

        private void ValidaSeExistePermanenciaInfornada(GestorExternoPerfilInput input)
        {
            var permanencias = _gestaoAlocadosRepository.ListarPermanenciasAsync().Result;
            var permaneciaId = input.PermanenciaId;

            if (permaneciaId.HasValue && !permanencias.Any(x => x.Id == permaneciaId))
            {
                throw new ApplicationException($"A permanência não existe na tabela de referência. ID não encontrado: {permaneciaId}.");
            }
        }

        private void ValidaSeExisteModeloTrabalhoInfornado(GestorExternoPerfilInput input)
        {
            var modelosDeTrabalho = _gestaoAlocadosRepository.ListarModelosTrabalhoAsync().Result;
            var modeloTrabalhoId = input.ModeloTrabalhoId;

            if (modeloTrabalhoId.HasValue && !modelosDeTrabalho.Any(x => x.Id == modeloTrabalhoId))
            {
                throw new ApplicationException($"O modelo de trabalho não existe na tabela de referência. ID não encontrado: {modeloTrabalhoId}.");
            }
        }

        private void ValidaSeExisteModeloProfissionalLocalidadeInfornado(GestorExternoPerfilInput input)
        {
            var modelosDeTrabalho = _gestaoAlocadosRepository.ListarProfissionaisLocalidadesAsync().Result;
            var localidadeId = input.ProfissionalLocalidadeId;

            if (localidadeId.HasValue && !modelosDeTrabalho.Any(x => x.Id == localidadeId))
            {
                throw new ApplicationException($"A localidade do profissional não existe na tabela de referência. ID não encontrado: {localidadeId}.");
            }
        }

        private async Task ValidaSeExisteGestorExternoPerfil(Guid id)
        {
            var gestorExternoPerfil = await _gestorExternoPerfilRepository.ObterGestorExternoPerfilPorIdAsync(id);

            if (gestorExternoPerfil.IsNull())
            {
                throw new ApplicationException($"Id do Perfil informado não foi encontrado.");
            }
        }

        private async Task ValidaSeExisteGestorExterno(string codGestorExterno, int orgId)
        {
            var gestorExternoPerfil = await _gestorExternoRepository.ObterGestorExternoPorCodigoAsync(codGestorExterno, orgId);

            if (gestorExternoPerfil.IsNull())
            {
                throw new ApplicationException($"Gestor com código: '{codGestorExterno.ToString()}' não encontrado.");
            }
        }

        private async Task ValidaSeJaExisteGestorExternoPerfil(string nomePerfil, string codGestorExterno, int orgId, Guid id)
        {
            var listaGestorExternoPerfil = await _gestorExternoPerfilRepository.ObterGestorExternoPerfilsPorCodigoGestorExternoAsync(codGestorExterno, orgId);

            if (listaGestorExternoPerfil.IsNotNull())
            {
                if (listaGestorExternoPerfil.Any(x => x.NomePerfil == nomePerfil && x.Id != id))
                {
                    throw new ApplicationException($"Nome de Perfil já cadastrado para este gestor.");
                }
            }
        }

        private void ValidarSkills(GestorExternoPerfilInput input)
        {
            var skills = input.GestorExternoPerfilSkills;

            var softSkills = skills.Where(x => x.ItemPerfil.Id == ItemPerfilEnum.SOFTSKILL.ToLong());

            var hardSkills = skills.Where(x => x.ItemPerfil.Id == ItemPerfilEnum.COMPETENCIA.ToLong());

            if (!softSkills.Any()) throw new ApplicationException("É necessário informar ao menos uma Habilidade Comportamental.");
            if (!hardSkills.Any()) throw new ApplicationException("É necessário informar ao menos uma Habilidade Técnica.");
        }

        private void ValidaCidadeEstado(GestorExternoPerfilInput input)
        {
	        if (input.Cidade?.ToUpper() == "CIDADE" || input.Estado?.ToUpper() == "ESTADO")
		        throw new ApplicationException($"Cidade ou Estado informados não existem.");
        }
	}
}
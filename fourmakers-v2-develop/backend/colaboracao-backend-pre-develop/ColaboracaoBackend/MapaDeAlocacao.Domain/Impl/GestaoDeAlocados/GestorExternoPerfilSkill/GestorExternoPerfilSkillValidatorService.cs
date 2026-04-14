using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Competencia.Domain.Enums;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfilSkill;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados.GestorExternoPerfilSkill
{
    [LogDomainClass]
    public class GestorExternoPerfilSkillValidatorService : IGestorExternoPerfilSkillValidatorService
    {
        private readonly IGestorExternoPerfilRepository _gestorExternoPerfilRepository;
        private readonly IGestorExternoPerfilSkillRepository _gestorExternoPerfilSkillRepository;

        public GestorExternoPerfilSkillValidatorService(IGestorExternoPerfilRepository gestorExternoPerfilRepository,
                                                        IGestorExternoPerfilSkillRepository gestorExternoPerfilSkillRepository)
        {
            _gestorExternoPerfilRepository = gestorExternoPerfilRepository;
            _gestorExternoPerfilSkillRepository = gestorExternoPerfilSkillRepository;
        }

        public async Task ValidaGestorExternoPerfilSkill(GestorExternoPerfilSkillInput input, CRUDEnum crudOperation)
        {
            if (crudOperation == CRUDEnum.Create)
            {
                await ValidarCamposDeEntrada(input, crudOperation);

                await ValidaItemPerfilId(input.ItemPerfil.Id);
                await ValidaSeExisteCombinacaoSkillItemTrabalhoENivel(input.Skill.Id, input.ItemPerfil.Id, input.Nivel.Id);
                await ValidaSeExisteGestorExternoPerfil(input.GestorExternoPerfilId);
            }
        }

        private async Task ValidarCamposDeEntrada(GestorExternoPerfilSkillInput input, CRUDEnum cRUDEnum)
        {
            var campos = new List<CampoValidacao>();

            campos.Add(new("Perfil", input.GestorExternoPerfilId, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Item Perfil", input.ItemPerfil.Id, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Skill", input.Skill.Id, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Nível", input.Nivel.Id, TipoValidacaoEnum.Obrigatoriedade));

            campos.Add(new("Gestor ID", input.GestorExternoPerfilId, TipoValidacaoEnum.TamanhoExato) { TamanhoExato = 36 });

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        private async Task ValidaItemPerfilId(long id)
        {
            if (!Enum.IsDefined(typeof(ItemPerfilEnum), (int)id))
            {
                throw new ApplicationException($"O ItemPerfilId '{id}' não é válido. Certifique-se de que corresponde a um valor definido para o Cadastro de Peril.");
            }
        }

        private async Task ValidaSeExisteGestorExternoPerfil(Guid gestorExternoPerfilId)
        {
            var gestorExternoPerfilSkill = await _gestorExternoPerfilRepository.ObterGestorExternoPerfilPorIdAsync(gestorExternoPerfilId);

            if (gestorExternoPerfilSkill.IsNull())
            {
                throw new ApplicationException($"Id do Perfil informado não foi encontrado.");
            }
        }

        private async Task ValidaSeExisteCombinacaoSkillItemTrabalhoENivel(long skillId, long itemPerfilId, long nivelId)
        {
            var nivelValido = await _gestorExternoPerfilSkillRepository.ValidarRelacaoNivelItemPerfilAsync(itemPerfilId, nivelId);
            if (!nivelValido)
            {
                var categoria = ObterNomeAmigavelCategoriaItemPerfil(itemPerfilId);
                throw new ApplicationException(
                    $"O nível informado não é aceito para a categoria «{categoria}». " +
                    "Perfis gerados por IA podem associar um nível de uma categoria a um item de outra (ex.: nível de Técnica em Metodologia). " +
                    "Níveis aceitos por categoria no sistema: Técnica — Trainee, Junior, Pleno, Sênior, Especialista; " +
                    "Socioemocionais — Iniciante, Intermediário, Avançado, Especialista; Metodologia — Iniciante, Intermediário, Avançado, Especialista; " +
                    "Domínio de Negócio — Iniciante, Intermediário, Avançado, Especialista; Idioma — Básico, Intermediário, Avançado, Fluente, Nativo.");
            }

            var skillValida = await _gestorExternoPerfilSkillRepository.ValidarSkillParaItemPerfilAsync(skillId, itemPerfilId);
            if (!skillValida)
            {
                var itemPerfilDescricao = ((ItemPerfilEnum)itemPerfilId).ToString();
                throw new ApplicationException($"O SkillId '{skillId}' não é existente na tabela relacionada ao ItemPerfil '{itemPerfilDescricao}'.");
            }
        }

        private static string ObterNomeAmigavelCategoriaItemPerfil(long itemPerfilId)
        {
            if (!Enum.IsDefined(typeof(ItemPerfilEnum), (int)itemPerfilId))
                return $"ItemPerfil {itemPerfilId}";

            return ((ItemPerfilEnum)itemPerfilId) switch
            {
                ItemPerfilEnum.COMPETENCIA => "Técnica",
                ItemPerfilEnum.SOFTSKILL => "Socioemocionais",
                ItemPerfilEnum.METODOLOGIA => "Metodologia",
                ItemPerfilEnum.DOMINIONEGOCIO => "Domínio de Negócio",
                ItemPerfilEnum.IDIOMA => "Idioma",
                _ => ((ItemPerfilEnum)itemPerfilId).ToString()
            };
        }
    }
}
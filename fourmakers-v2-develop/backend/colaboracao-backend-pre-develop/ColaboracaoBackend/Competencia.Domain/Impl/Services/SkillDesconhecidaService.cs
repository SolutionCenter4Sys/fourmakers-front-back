using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Competencia.Domain.Interfaces.Services;
using Core.Domain;
using Core.DomainModel;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.SkillDesconhecida;
using DataTransferObject.Domain.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Competencia.Domain.Impl.Services
{
    [LogDomainClass]
    public class SkillDesconhecidaService : ISkillDesconhecidaService
    {
        private readonly ISkillDesconhecidaColaboradorRepository _skillDesconhecidaColaboradorRepository;
        private readonly ISkillDesconhecidaRepository _skillDesconhecidaRepository;
        private readonly UsuarioLogadoDTO _usuarioLogadoDTO;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;

        public SkillDesconhecidaService
        (
            ISkillDesconhecidaColaboradorRepository skillDesconhecidaColaboradorRepository,
            ISkillDesconhecidaRepository skillDesconhecidaRepository,
            IAspNetUser aspnetUser,
            IVerificaSeCpfESistemico verificaCpfSistemico
        )
        {
            _skillDesconhecidaColaboradorRepository = skillDesconhecidaColaboradorRepository;
            _skillDesconhecidaRepository = skillDesconhecidaRepository;
            _usuarioLogadoDTO = aspnetUser.GetUsuarioLogado();
            _verificaCpfSistemico = verificaCpfSistemico;
        }

        public void InserirSkillDesconhecidaColaborador(int idSkillDesconhecida, long? nivelId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                var skillDTO = new SkillDesconhecidaColaboradorDTO();
                skillDTO.ColaboradorCpf = cpf;
                skillDTO.IdSkillDesconhecida = idSkillDesconhecida;
                skillDTO.IdNivel = nivelId;
                _skillDesconhecidaColaboradorRepository.InserirSkillDesconhecidaColaborador(skillDTO);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<SkillDesconhecidaDTO> InserirSkillDesconhecida(string descricao)
        {
            try
            {
                var cpf = _verificaCpfSistemico.VerificaCpfSistemico(_usuarioLogadoDTO.Cpf);
                var idUser = await _skillDesconhecidaRepository.GetUsuarioCriacaoId(cpf);
                if (idUser == 0)
                {
                    throw new ArgumentException("Usuário não encontrado.");
                }
                return await _skillDesconhecidaRepository.AddSkillDesconhecida(descricao, idUser);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<SkillDesconhecidaDTO>> ListarSkillsDesconhecidas(string busca, int limite)
        {
            return await _skillDesconhecidaRepository.ListSkillDesconhecida(busca, limite);
        }

        public async Task<List<SkillDesconhecidaColaboradorDTO>> ListarSkillDesconhecidasColaborador(string codInternoColaborador)
        {
            try
            {
                if (string.IsNullOrEmpty(codInternoColaborador))
                    throw new ArgumentException("O código interno do colaborador é obrigatório");

                return await _skillDesconhecidaColaboradorRepository.ListarSkillDesconhecidasColaboador(codInternoColaborador);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<KeyValuePair<string, int>>> GetSkillDesconhecidaInfoByDescricao(List<string> skills)
        {
            var ret = new List<KeyValuePair<string, int>>();
            try
            {
                foreach (var skill in skills)
                {
                    int skillId;
                    var strValue = StringUtil.RemoveDiacritics(skill);
                    var skillAux = (await ListarSkillsDesconhecidas(strValue, 999)).Where(x => StringUtil.RemoveDiacritics(x.Descricao).ToUpper().Equals(strValue, StringComparison.InvariantCultureIgnoreCase) == true);
                    if (!skillAux.Any())
                    {
                        var novaSkill = await InserirSkillDesconhecida(skill);
                        skillId = novaSkill.Id;
                    }
                    else
                        skillId = skillAux.First().Id;
                    ret.Add(new KeyValuePair<string, int>(strValue, skillId));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ret;
        }
    }
}
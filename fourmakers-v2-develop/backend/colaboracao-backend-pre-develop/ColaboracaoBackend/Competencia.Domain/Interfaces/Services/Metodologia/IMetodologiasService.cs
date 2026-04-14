using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Metodologia;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Competencia.Domain.Interfaces.Services.Metodologia
{
    public interface IMetodologiasService
    {
        List<long> ListarMetodologiasAtribuidas(string cpf);
        ListaMetodologiaResult ListarMetodologiasNaoAtribuidas(string busca, int cursor, int limite, string cpf);
        List<MetodologiaDTO> ListarMetodologias(string busca, int cursor, int limite);
        MetodologiaDTO ListarMetodologiasById(int codMetodologia);
        List<NivelDTO> ListarNivelMetodologia();
        List<ListaMetodologiaColaboradorResult> ListarMetodologiasColaborador(string cpf);
        MetodologiaColaboradorResult InserirMetodologiaColaborador(List<MetodologiaDTO> param, string cpf, string token, bool minhaJornada, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        bool AdicionarMetodologia(List<MetodologiaDTO> param, string cpf, string token, bool importacaoLote = false);
        void RemoverMetodologiaColaborador(int codMetodologia, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        void AtualizarNivelMetodologiaColaborador(MetodologiaDTO metodologia, string cpf, bool minhaJornada, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        /// <summary>
        /// Adiciona uma nova metodologia ao sistema
        /// </summary>
        /// <param name="descricao">Descrição da metodologia</param>
        /// <param name="cpf">CPF do usuário que está adicionando a metodologia</param>
        /// <returns>ID da metodologia criada</returns>
        public long AdicionarMetodologia(string descricao, string cpf, bool importacaoLote = false);
        Task<List<KeyValuePair<string, long>>> GetMetodologiaInfoByDescricao(List<string> metodologias);
        MetodologiaColaboradorResult InserirMetodologiaColaboradorLote(List<MetodologiaDTO> listaMetodologias, string cpf, string token, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        Task<ApiGenericResult<List<CompetenciaGroupDTO>>> GetGroupMetodologiaByIds(List<long> ids);
    }
}
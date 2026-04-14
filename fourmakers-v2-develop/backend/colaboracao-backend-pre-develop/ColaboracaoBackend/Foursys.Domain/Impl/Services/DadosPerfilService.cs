using ApiClient.Domain.Interfaces;
using Core.Domain.DadosPerfil;
using DataTransferObject.Domain.Cep;
using DataTransferObject.Domain.DadosPerfil;
using DataTransferObject.Domain.DadosPerfil.GrauParentesco;
using DataTransferObject.Domain.DadosPerfil.TipoContratacao;
using Foursys.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Foursys.Domain.Impl.Services
{
    [LogDomainClass]
    public class DadosPerfilService : IDadosPerfilService
    {
        private readonly IDadosPerfilRepository _repositoryDadosPerfil;
        private readonly ICepClient _cepClient;
        private readonly ICCHClient _cchClient;
        public DadosPerfilService(IDadosPerfilRepository repositoryDadosPerfil, ICepClient cepClient, ICCHClient cchClient)
        {
            _repositoryDadosPerfil = repositoryDadosPerfil;
            _cepClient = cepClient;
            _cchClient = cchClient;
        }
        public List<DisponibilidadeDTO> ListarDisponibilidade(string busca, int cursor, int limite)
        {
            try
            {
                return _repositoryDadosPerfil.ListarDisponibilidade(busca, cursor, limite);
            }
            catch { throw; }
        }

        public List<FonteOrigemDTO> ListarFonteOrigem(string busca, int cursor, int limite)
        {
            try
            {
                return _repositoryDadosPerfil.ListarFonteOrigem(busca, cursor, limite);
            }
            catch { throw; }
        }

        public List<TipoDeCargoDTO> ListarTipoDeCargo(string busca, int cursor, int limite)
        {
            try
            {
                return _repositoryDadosPerfil.ListarTipoDeCargo(busca, cursor, limite);
            }
            catch { throw; }
        }

        public List<TipoCargaHorariaDTO> ListarTipoCargaHoraria(string busca, int cursor, int limite)
        {
            try
            {
                return _repositoryDadosPerfil.ListarTipoCargaHoraria(busca, cursor, limite);
            }
            catch { throw; }
        }

        public List<ModalidadeContratacaoDTO> ListarModalidadeContratacao(string busca, int cursor, int limite)
        {
            try
            {
                return _repositoryDadosPerfil.ListarModalidadeContratacao(busca, cursor, limite);
            }
            catch { throw; }
        }

        public List<EstadoCivilDTO> ListarEstadoCivil(string busca, int cursor, int limite)
        {
            try
            {
                return _repositoryDadosPerfil.ListarEstadoCivil(busca, cursor, limite);
            }
            catch { throw; }
        }

        public List<EscolaridadeDTO> ListarEscolaridade(string busca, int cursor, int limite)
        {
            try
            {
                return _repositoryDadosPerfil.ListarEscolaridade(busca, cursor, limite);
            }
            catch { throw; }
        }

        public List<OrientacaoSexualDTO> ListarOrientacaoSexual(string busca, int cursor, int limite)
        {
            try
            {
                return _repositoryDadosPerfil.ListarOrientacaoSexual(busca, cursor, limite);
            }
            catch { throw; }
        }

        public List<IdentidadeDeGeneroDTO> ListarIdentidadeDeGenero(string busca, int cursor, int limite)
        {
            try
            {
                return _repositoryDadosPerfil.ListarIdentidadeDeGenero(busca, cursor, limite);
            }
            catch { throw; }
        }

        public async Task<ConsultaCepDTO> ConsultarCep(string cep)
        {
            try
            {
                var ret = await _cepClient.ConsultaCep(cep);
                if (ret == null || ret.Uf == null)
                {
                    throw new Exception("Cep não encontrado.");
                }
                return ret;
            }
            catch { throw; }
        }

        public async Task<TipoContratacaoDTO> BuscarTipoContratacao(string emailColaborador)
        {
            var retorno = new TipoContratacaoResponse();

            var token = await _cchClient.Autenticacao();
            retorno.Colaboradores = await _cchClient.Validacao(emailColaborador, token.token);

            if (retorno.Colaboradores == null || retorno.Colaboradores.Count == 0)
            {
                throw new Exception("Colaborador nao encontrado para este email");
            }

            retorno.Recurso = await _cchClient.Recurso(retorno.Colaboradores.OrderByDescending(c => c.flFuncionarioAtivo).FirstOrDefault().cdProfissional.ToString(), token.token);

            DateTime contratacao = retorno.Recurso.dtContratacao ?? DateTime.MinValue;
            TimeSpan tempoCalculado = DateTime.Now - contratacao;

            var dias = tempoCalculado.TotalDays;
            double ano = 0;

            ano = dias / 365.0;
            int anoInteiro = (int)ano;
            double diasTotaisDosAnos = (anoInteiro * 365);
            double diasRestantes = dias - diasTotaisDosAnos;
            double meses = diasRestantes / 30.0;
            int mesesInteiros = (int)meses;

            string tempoDeEmpresa = "";

            if (anoInteiro >= 2)
                tempoDeEmpresa = anoInteiro + " anos";
            if (anoInteiro == 1)
                tempoDeEmpresa = anoInteiro + " ano";

            if (mesesInteiros >= 2)
                tempoDeEmpresa = tempoDeEmpresa + " " + mesesInteiros + " meses";
            if (mesesInteiros == 1)
                tempoDeEmpresa = tempoDeEmpresa + " " + mesesInteiros + " mês";

            return new TipoContratacaoDTO
            {
                Recurso = retorno.Recurso,
                TempoNaEmpresa = tempoDeEmpresa,
            };
        }

        public List<GrauParentescoDTO> ListarGrauParentesco(string busca, int cursor, int limite)
        {
            try
            {
                return _repositoryDadosPerfil.ListarGrauParentesco(busca, cursor, limite);
            }
            catch
            {
                throw;
            }
        }

        public List<ListarTipoContratacaoDTO> ListarTipoContratacao(string busca, int cursor, int limite)
        {
            try
            {
                return _repositoryDadosPerfil.ListarTipoContratacao(busca, cursor, limite);
            }
            catch
            {
                throw;
            }
        }

        public List<EtniaDTO> ListarEtnia(string busca, int cursor, int limite)
        {
            try
            {
                return _repositoryDadosPerfil.ListarEtnia(busca, cursor, limite);
            }
            catch
            {
                throw;
            }
        }
    }
}
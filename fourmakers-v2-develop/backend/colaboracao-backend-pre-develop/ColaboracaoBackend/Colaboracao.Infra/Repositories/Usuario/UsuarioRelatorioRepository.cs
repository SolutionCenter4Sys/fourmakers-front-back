using ApiClient.Domain;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.DomainModel.Usuario;
using Dapper;
using DataTransferObject.Domain.Foursys;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Usuario
{
    public class UsuarioRelatorioRepository : IUsuarioRelatorioRepository
    {
        private const int ATIVO = 1;
        private readonly ColaboradorContext _colaboradorContext;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _confguration;
        private readonly ILogCore _log;
        private IConnectionStringCore _connectionString;
        private string serviceMidia = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);

        public UsuarioRelatorioRepository(ColaboradorContext colaboradorContext, Microsoft.Extensions.Configuration.IConfiguration configuration, IConnectionStringCore connectionString)
        {
            _colaboradorContext = colaboradorContext;
            _confguration = configuration;
            this._connectionString = connectionString;
        }

        public List<UsuarioAcessoDTO> GetRelatorioAcessoUsuarios(UsuarioLogadoDTO usuarioLogadoDTO)
        {
            var ret = new List<UsuarioAcessoDTO>();

            var retorno = _colaboradorContext.tb_colaborador
                 .Join(_colaboradorContext.tb_colaborador_org.Where(tco => tco.tb_org_id == usuarioLogadoDTO.OrgId),
                     colaborador => colaborador.codigo_interno_colaborador,
                     colaborador_org => colaborador_org.codigo_interno_colaborador,
                     (colaborador, colaborador_org) => new { Colaborador = colaborador, ColaboradorOrg = colaborador_org })
                 .Where(x => x.ColaboradorOrg.ativo == ATIVO)
                 .ToList()
                 .Select(c =>
                 {
                     var usuario = c.Colaborador.tb_usuario.Where(u => u.tb_org_id == usuarioLogadoDTO.OrgId).FirstOrDefault();

                     return new UsuarioAcessoDTO
                     {
                         Id = usuario?.id,
                         Cpf = c.Colaborador.codigo_interno_colaborador,
                         Email = usuario?.email,
                         Nome = c.Colaborador.nome_completo,
                         Cargo = c.ColaboradorOrg.cargo,
                         Diretoria = c.ColaboradorOrg.diretoria,
                         Status = ObterStatus(c.Colaborador),
                         ImagemPath = GetImagemPath(c.Colaborador.imagem.path, usuarioLogadoDTO.Token),
                         UltimoLogin = null
                     };
                 })
                 .ToList();

            return retorno.ToList();
        }

        private static string ObterStatus(tb_colaborador colaborador)
        {
            if (colaborador.data_nascimento == null ||
                string.IsNullOrWhiteSpace(colaborador.genero) ||
                string.IsNullOrWhiteSpace(colaborador.etnia) ||
                string.IsNullOrWhiteSpace(colaborador.orientacao_sexual) ||
                string.IsNullOrWhiteSpace(colaborador.escolaridade))
            {
                return "Incompleto";
            }
            else
            {
                return "Completo";
            }
        }

        public List<ColaboradoresOrgIdResult> ListaColaboradoresOrgId(int orgId, int page, int pageSize)
        {
            try
            {
                var listaColaboradoresOrgId = _colaboradorContext.tb_colaborador_org
                    .Where(x => orgId == 0 || x.tb_org_id == orgId)
                    .ToList()
                    .Select(colabOrg =>
                    {
                        var usuario = colabOrg.codigo_interno_colaboradorNavigation.tb_usuario.Where(u => u.tb_org_id == orgId).FirstOrDefault();
                        return new
                        {
                            Colaborador = _colaboradorContext.tb_colaborador
                                        .FirstOrDefault(c => c.codigo_interno_colaborador == colabOrg.codigo_interno_colaborador),
                            ColaboradorOrg = colabOrg,
                            Usuario = colabOrg.codigo_interno_colaboradorNavigation.tb_usuario,
                            Org = colabOrg.tb_org,
                            Imagem = usuario?.codigo_interno_colaboradorNavigation.imagem
                        };
                    }).ToList()
                    .Select(x =>
                    {
                        var usuario = x.Usuario.Where(u => u.tb_org_id == orgId).FirstOrDefault();
                        return new ColaboradoresOrgIdResult
                        {
                            Id_usuario = usuario?.id,
                            Cpf = x.Colaborador.codigo_interno_colaborador,
                            Ativo = Convert.ToBoolean(x.ColaboradorOrg.ativo),
                            Email = usuario?.email,
                            Nome = x.Colaborador.nome_completo,
                            Org = new OrgDTO
                            {
                                Id = x.Org.id,
                                Descricao = x.Org.descricao
                            },
                            ImagemPath = x.Imagem != null ? $"{serviceMidia}{x.Imagem.path}" : null,
                            DataCadastro = x.Colaborador.data_criacao.ToString("dd-MM-yyyy"),
                            Status = ObterStatus(x.Colaborador),
                        };
                    })
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return listaColaboradoresOrgId ?? new List<ColaboradoresOrgIdResult>();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public int GetColaboradoresOrgId(int orgId)
        {
            try
            {
                return _colaboradorContext.tb_colaborador_org
                    .Count(x => orgId == 0 || x.tb_org_id == orgId);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public string GetOrgDescricao(int orgId)
        {
            try
            {
                return _colaboradorContext.tb_org
                    .FirstOrDefault(x => x.id == orgId)?.descricao;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private static string GetImagemPath(string path, string token)
        {
            if (path != null)
            {
                string serviceMidia = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);
                return serviceMidia.Replace("$1", token) + path;
            }
            return null;
        }

        public async Task<List<dynamic>> RelatorioColaboradores(int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@orgId", orgId, DbType.Int32);

                    var extracaoAlocacoesList = await _connection.QueryAsync<dynamic>(
                        "spr_rpt_relatorio_colaboradores",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return extracaoAlocacoesList.ToList();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    }
}
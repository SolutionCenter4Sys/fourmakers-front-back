using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.Domain.MapaAlocacao;
using Dapper;
using DataTransferObject.Domain.Carga;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.CalculoMensal;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ProjetoProposta;
using DataTransferObject.Domain.MapaDeAlocacao.Projetos;
using DataTransferObject.Domain.Projeto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao
{
    public class ProjetoMapaDeAlocacaoRepository : IProjetoMapaDeAlocacaoRepository
    {
        private ColaboradorContext _colaboradorContext;
        private const sbyte ATIVO = 1;
        private IConnectionStringCore _connectionString;
        private readonly IDBConnection _dapperConnection;

        public ProjetoMapaDeAlocacaoRepository(ColaboradorContext colaboradorContext, IConnectionStringCore connectionString, IMapaAlocacaoRepository mapaAlocacaoRepository, IDBConnection dapperConnection)
        {
            _colaboradorContext = colaboradorContext;
            _connectionString = connectionString;
            _dapperConnection = dapperConnection;
        }

        public async Task<int> GetQuantidadeDeAlocadosAsync(string cdProjeto, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = @" SELECT
                                        COUNT(DISTINCT codigo_interno_colaborador)
                                    FROM
                                        tb_colaborador_periodo_alocacao
                                    WHERE
                                        codigo_projeto = @CodigoProjeto
                                    AND
                                        ativo = 1 AND tb_org_id = @OrgId";

                    var result = await _connection.ExecuteScalarAsync<int>(sql, new
                    {
                        CodigoProjeto = cdProjeto,
                        OrgId = orgId
                    });

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public async Task<List<PeriodoDTO>> GetPeriodoAlocadoProjetoAsync(string cdProjeto, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = @" SELECT
                                        data_inicio AS DataInicio,
                                        data_fim AS DataFim,
                                        inclui_fimdesemana AS IncluiFimDeSemana,
                                        id AS PeriodoAlocadoId,
                                        quantidade_horas AS QuantidadeHoras
                                    FROM
                                        tb_colaborador_periodo_alocacao
                                    WHERE
                                        codigo_projeto = @CodigoProjeto
                                        AND tb_org_id = @OrgId
                                        AND ativo = @Ativo";

                    var periodos = await _connection.QueryAsync<PeriodoDTO>(sql, new
                    {
                        CodigoProjeto = cdProjeto,
                        OrgId = orgId,
                        Ativo = ATIVO
                    });

                    return periodos.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public List<BuscaProjetoHorasDTO> ListarProjetoHoras(string busca, int cursor, int limite, int orgId)
        {
            var ret = new List<BuscaProjetoHorasDTO>();
            var aux = new List<tb_projeto_org>();
            try
            {
                if (busca != null)
                {
                    aux = _colaboradorContext.tb_projeto_org.Where(x => x.projeto.Contains(busca) && x.tb_org_id == orgId).ToList();

                    if (aux != null)
                    {
                        foreach (var item in aux)
                        {
                            ret.Add(new BuscaProjetoHorasDTO { NomeProjeto = item.projeto, CodigoProjeto = long.Parse(item.cod_projeto) });
                        }
                    }
                    else
                    {
                        throw new Exception("Projeto não encontrado");
                    }
                }
                else
                {
                    aux = _colaboradorContext.tb_projeto_org.Where(x => x.tb_org_id == orgId).OrderBy(x => x.projeto).Skip(cursor).Take(limite).ToList();

                    if (aux != null)
                    {
                        foreach (var item in aux)
                        {
                            ret.Add(new BuscaProjetoHorasDTO { NomeProjeto = item.projeto, CodigoProjeto = long.Parse(item.cod_projeto) });
                        }
                    }
                    else
                    {
                        throw new Exception("Não foi possível gerar uma lista de projetos");
                    }
                }

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StatusProjetosDTO> ListarStatusProjetos(string busca, int cursor, int limite, int orgId)
        {
            try
            {
                var ret = new List<StatusProjetosDTO>();

                if (busca != null)
                {
                    var aux = _colaboradorContext.tb_projeto_org
                        .Where(x => x.status.Contains(busca) && x.tb_org_id == orgId)
                        .GroupBy(y => new { y.status, y.cod_status })
                        .OrderBy(x => x.Key.status)
                        .Skip(cursor).Take(limite)
                        .Select(x => new { x.Key.status, x.Key.cod_status })
                        .ToList();

                    if (aux != null)
                    {
                        foreach (var item in aux)
                        {
                            ret.Add(new StatusProjetosDTO { NomeStatusProjeto = item.status, CodigoStatusProjeto = (long)item.cod_status });
                        }
                    }
                    else
                    {
                        throw new Exception("Status não encontrado");
                    }
                }
                else
                {
                    var aux = _colaboradorContext.tb_projeto_org.Where(x => x.tb_org_id == orgId).GroupBy(y => new { y.status, y.cod_status })
                        .Select(x => new { x.Key.status, x.Key.cod_status }).OrderBy(x => x.status).Take(limite).ToList();

                    if (aux != null)
                    {
                        foreach (var item in aux)
                        {
                            ret.Add(new StatusProjetosDTO { NomeStatusProjeto = item.status, CodigoStatusProjeto = (long)item.cod_status });
                        }
                    }
                    else
                    {
                        throw new Exception("Não foi possível gerar uma lista de projetos");
                    }
                }

                return ret;
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<ConsultaProjetoHorasDTO>> BuscarProjetosHoras(int cursor, int limite, string? cdProjeto, string? nomeProjeto, int? cdStatusProjeto, string cdCliente, string cliente, string gestorProjeto, int? codDiretoria, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
               SELECT
                    projeto.cod_projeto AS CodigoProjeto,
                    projeto.projeto AS NomeProjeto,
                    projeto.cod_cliente AS CodigoCliente,
                    clienteOrg.nome_cliente AS Cliente,
                    projeto.cod_proposta AS Proposta,
                    projeto.status AS StatusProjeto,
                    IFNULL(projeto.data_inicio, '1900-01-01') AS InicioProjeto,
                    IFNULL(projeto.data_fim, '1900-01-01') AS FimProjeto,
                    IFNULL(gerente.cod_colaborador_gerente, '-1') AS CodigoGerenteProjeto,
                    IFNULL(colaborador.nome_completo, '') AS NomeGerenteProjeto,
                    IFNULL(colaborador.codigo_interno_colaborador, '') AS CpfGerenteProjeto,
                    COUNT(DISTINCT gestores.cod_gestor_externo) AS QuantidadeDeGestores
                FROM
                    tb_projeto_org projeto
                LEFT JOIN
                    tb_projeto_gerente gerente ON projeto.cod_projeto = gerente.cod_projeto
                LEFT JOIN
                    tb_colaborador_org colaboradorOrg ON gerente.cod_colaborador_gerente = colaboradorOrg.cod_colaborador_externo
                LEFT JOIN
                    tb_colaborador colaborador ON colaboradorOrg.codigo_interno_colaborador = colaborador.codigo_interno_colaborador
                LEFT JOIN
                    tb_cliente_org clienteOrg ON projeto.cod_cliente = clienteOrg.codigo_cliente
                LEFT JOIN
                    tb_gestor_externo gestores ON gestores.codigo_cliente = clienteOrg.codigo_cliente
                WHERE
                    (@cdProjeto IS NULL OR projeto.cod_projeto = @cdProjeto) AND
                    (@nomeProjeto IS NULL OR projeto.projeto LIKE CONCAT('%', @nomeProjeto, '%')) AND
                    (@cdStatusProjeto IS NULL OR projeto.cod_status = @cdStatusProjeto) AND
                    (@codDiretoria IS NULL OR @codDiretoria = '0' OR projeto.cod_diretoria = @codDiretoria) AND
                    (@cliente IS NULL OR clienteOrg.nome_cliente LIKE CONCAT('%', @cliente, '%')) AND
                    (@cdCliente IS NULL OR projeto.cod_cliente = @cdCliente) AND
                    (@gestorProjeto IS NULL OR
                        (colaborador.nome_completo LIKE CONCAT('%', @gestorProjeto, '%') OR gerente.cod_colaborador_gerente = @gestorProjeto)
                    ) AND
                    projeto.tb_org_id = @orgId
                GROUP BY projeto.cod_projeto
                ORDER BY projeto.projeto DESC
                LIMIT @limite OFFSET @cursor;
            ";

            var parametros = new
            {
                cdProjeto,
                nomeProjeto,
                cdStatusProjeto,
                cdCliente,
                cliente,
                gestorProjeto,
                codDiretoria,
                orgId,
                cursor = cursor * limite,
                limite
            };

            var resultList = await connection.QueryAsync<ConsultaProjetoHorasDTO>(query, parametros);

            var projetos = resultList.ToList();

            var buscaFeriados = await GetFeriadosPorOrgId(orgId);
            var feriados = buscaFeriados.Select(x => x.Data).ToArray();

            foreach (var projeto in projetos)
            {
                var colaboradoresAlocados = BuscaColaboradoresAlocadosNoProjeto(projeto.CodigoProjeto, orgId);

                projeto.QuantidadeAlocados = colaboradoresAlocados.Where(x => x.CodTbdAlocado == null)
                    .GroupBy(x => x.CodigoColaborador)
                    .Count();

                projeto.QuantidadeDeTBDS = colaboradoresAlocados
                    .Where(x => x.CodTbdAlocado != null)
                    .GroupBy(x => x.CodigoColaborador)
                    .Count();

                var horasAlocadas = 0.0;

                foreach (var colaboradorAlocado in colaboradoresAlocados)
                {
                    horasAlocadas += DateTimeUtil.ConverteDiasUteisParaHoras(
                        colaboradorAlocado.DataInicio,
                        colaboradorAlocado.DataFim,
                        (int)colaboradorAlocado.QuantidadeHoras,
                        false,
                        feriados);

                    projeto.HorasAlocadas = (long)horasAlocadas;
                }
            }

            return projetos;
        }

        public ResumoConsultarColaboradorProjetoDTO ResumoHorasComerciais(string cdProjeto, int orgId)
        {
            var ret = new ResumoConsultarColaboradorProjetoDTO();
            ret.HorasComerciais = 0;

            var retService = _colaboradorContext.tb_projeto_org.Where(x => x.cod_projeto == cdProjeto && x.tb_org_id == orgId).FirstOrDefault();
            if (retService != null)
            {
                ret.HorasComerciais += (long)retService.qtd_horas_planejadas;
            }
            return ret;
        }

        public List<ColaboradorAlocadoProjetoDTO> ColaboradorAlocadoProjeto(long cdProjeto, string cpf)
        {
            if (cpf != null)
            {
                var colaborador = _colaboradorContext.tb_colaborador_periodo_alocacao
                    .Where(item => item.codigo_interno_colaborador == cpf && item.ativo == ATIVO && item.codigo_projeto == cdProjeto.ToString())
                    .Select(item => new ColaboradorAlocadoProjetoDTO
                    {
                        NomeColaborador = item.codigo_interno_colaboradorNavigation.nome_completo
                    }).ToList();
                return colaborador;
            }
            else
            {
                var colaborador = _colaboradorContext.tb_colaborador_periodo_alocacao
                    .Where(item => item.ativo == ATIVO && item.codigo_projeto == cdProjeto.ToString())
                    .Select(item => new ColaboradorAlocadoProjetoDTO
                    {
                        NomeColaborador = item.codigo_interno_colaboradorNavigation.nome_completo
                    }).ToList();
                return colaborador;
            }
        }

        public List<DataTransferObject.Domain.MapaDeAlocacao.ProjetoDTO> ColaboradorProjetoPorColaborador(long cdProjeto, string cpf, int orgId)
        {
            var projeto = _colaboradorContext.tb_colaborador_periodo_alocacao
            .Where(item => item.codigo_interno_colaborador == cpf
                && item.ativo == ATIVO
                && item.codigo_projeto == cdProjeto.ToString())
            .Select(item => new DataTransferObject.Domain.MapaDeAlocacao.ProjetoDTO
            {
                NomeProjeto = _colaboradorContext.tb_projeto_org.Where(x => x.cod_projeto == cdProjeto.ToString() && x.tb_org_id == orgId).FirstOrDefault().projeto ?? "",
                CodigoProjeto = cdProjeto
            })
            .ToList();

            return projeto;
        }

        public List<PeriodoDTO> ColaboradorPeriodoPorMesAnoProjeto(string mesAno, string colaboradorCPF, int? codigoTBD, string projetoId, int orgId)
        {
            var data = DateTime.MinValue;

            string[] partes = mesAno.Split('/');

            if (partes.Length == 2)
            {
                string mesString = partes[0];
                string anoString = partes[1];
                int mes = MapaUtil.ObterNumeroDoMes(mesString);

                if (mes != -1 && int.TryParse(anoString, out int ano))
                {
                    data = new DateTime(ano, mes, 1);
                }
                else
                {
                    throw new Exception("erro interno! dateFormat");
                }
            }

            var periodos = _colaboradorContext.tb_colaborador_periodo_alocacao
                            .Where(x => x.codigo_projeto == projetoId.ToString()
                                        && ((codigoTBD.ToIntOuZero() == 0 && x.codigo_interno_colaborador == colaboradorCPF)
                                            || (colaboradorCPF.IsNull() && x.cod_tbd_alocado == codigoTBD))
                                        && x.ativo == ATIVO
                                        && (x.data_inicio.Year < data.Year || (x.data_inicio.Year == data.Year && x.data_inicio.Month <= data.Month))
                                        && (x.data_fim.Year > data.Year || (x.data_fim.Year == data.Year && x.data_fim.Month >= data.Month))
                                        && x.tb_org_id == orgId)
                            .Select(item => new PeriodoDTO
                            {
                                QuantidadeHoras = item.quantidade_horas,
                                DataInicio = item.data_inicio,
                                DataFim = item.data_fim,
                                IncluiFimDeSemana = item.inclui_fimdesemana != 0
                            })
                            .ToList();

            return periodos;
        }

        public void UpsertProjetoOrg(ProjetoCargaDTO projetoCarga, TipoCadastroProjetoOrgEnum tipoCadastro, int orgId)
        {
            // colocar tipo cadastro
            var projetoOrgRow = _colaboradorContext.tb_projeto_org.Where(x => x.tb_org_id == orgId && x.cod_projeto == projetoCarga.IdentificadorProjeto).FirstOrDefault();
            if (projetoOrgRow == null)
            {
                _colaboradorContext.tb_projeto_org.Add(new tb_projeto_org
                {
                    cod_cliente_registro_carga = projetoCarga.IdentificadorCliente,
                    cod_cliente = projetoCarga.IdentificadorClienteRefatorado.ToStringOuNull() ?? projetoCarga.IdentificadorCliente,
                    nome_cliente_registro_carga = projetoCarga.Cliente,
                    cod_diretoria = projetoCarga.IdentificadorFilial,
                    cod_projeto = projetoCarga.IdentificadorProjeto,
                    cod_proposta = projetoCarga.IdentificadorProposta,
                    data_fim = projetoCarga.DataFim,
                    data_inicio = projetoCarga.DataInicio,
                    diretoria = projetoCarga.Filial,
                    projeto = projetoCarga.Projeto,
                    qtd_horas_executadas = projetoCarga.QtdHorasExecutadas.ToDecimalOuZero(),
                    qtd_horas_planejadas = projetoCarga.QtdHorasPlanejada.ToDecimalOuZero(),
                    status = projetoCarga.Status,
                    cod_status = projetoCarga.IndentificadorStatus,
                    tb_org_id = orgId,
                    tipo_cadastro = tipoCadastro.ToString()
                });
            }
            else
            {
                projetoOrgRow.cod_cliente_registro_carga = projetoCarga.IdentificadorCliente;
                projetoOrgRow.cod_cliente = projetoCarga.IdentificadorClienteRefatorado.ToStringOuNull() ?? projetoCarga.IdentificadorCliente;
                projetoOrgRow.nome_cliente_registro_carga = projetoCarga.Cliente;
                projetoOrgRow.cod_diretoria = projetoCarga.IdentificadorFilial;
                projetoOrgRow.cod_projeto = projetoCarga.IdentificadorProjeto;
                projetoOrgRow.cod_proposta = projetoCarga.IdentificadorProposta;
                projetoOrgRow.data_fim = projetoCarga.DataFim;
                projetoOrgRow.data_inicio = projetoCarga.DataInicio;
                projetoOrgRow.diretoria = projetoCarga.Filial;
                projetoOrgRow.projeto = projetoCarga.Projeto;
                projetoOrgRow.qtd_horas_executadas = projetoCarga.QtdHorasExecutadas.ToDecimalOuZero();
                projetoOrgRow.qtd_horas_planejadas = projetoCarga.QtdHorasPlanejada.ToDecimalOuZero();
                projetoOrgRow.status = projetoCarga.Status;
                projetoOrgRow.cod_status = projetoCarga.IndentificadorStatus;
                projetoOrgRow.tipo_cadastro = tipoCadastro.ToString();

                _colaboradorContext.tb_projeto_org.Update(projetoOrgRow);
            }
            _colaboradorContext.SaveChanges();
        }

        public void DeletaProjetosGerentes(string codProjeto, int orgId)
        {
            _colaboradorContext.tb_projeto_gerente.RemoveRange(_colaboradorContext.tb_projeto_gerente.Where(x => x.tb_org_id == orgId && x.cod_projeto == codProjeto));
            _colaboradorContext.SaveChanges();
        }

        public List<ProjetoProposta> GetPropostaProjeto(string codProjeto, int orgId)
        {
            return _colaboradorContext.tb_projeto_proposta.Where(x => x.cod_projeto == codProjeto && x.tb_org_id == orgId).ToList().Select(x => new ProjetoProposta
            {
                CodProjeto = x.cod_projeto,
                CodProposta = x.cod_proposta
            }).ToList();
        }
        public void InserePropostaProjeto(string proposta, string codProjeto, int orgId)
        {
            _colaboradorContext.tb_projeto_proposta.Add(new tb_projeto_proposta
            {
                cod_projeto = codProjeto,
                cod_proposta = proposta,
                tb_org_id = orgId
            });
            _colaboradorContext.SaveChanges();
        }

        public void RemovePropostaProjeto(string proposta, string codProjeto, int orgId)
        {
            var row = _colaboradorContext.tb_projeto_proposta.Where(x => x.cod_projeto == codProjeto && x.cod_proposta == proposta && x.tb_org_id == orgId).FirstOrDefault();
            if (row == null)
                return;
            _colaboradorContext.tb_projeto_proposta.Remove(row);
            _colaboradorContext.SaveChanges();
        }

        public void InsereProjetoGerente(ProjetoGerenteCargaDTO gerenteProjeto, int orgId)
        {
            _colaboradorContext.tb_projeto_gerente.Add(new tb_projeto_gerente
            {
                tb_org_id = orgId,
                cod_colaborador_gerente = gerenteProjeto.IdentificadorColaboradorGerente,
                cod_projeto = gerenteProjeto.IdentificadorProjeto,
                tipo_gerente = gerenteProjeto.IdentificadorTipoGerente,
            });
            _colaboradorContext.SaveChanges();
        }

        public GerenteProjetoDTO BuscaGerenteProjeto(int orgId, string codProjeto, string? codGerente)
        {
            var gerente = _colaboradorContext.tb_projeto_gerente.Where(x => x.cod_projeto == codProjeto.ToString() && x.tb_org_id == orgId && (codGerente == null || x.cod_colaborador_gerente == codGerente)).FirstOrDefault()?.cod_colaborador_gerente ?? null;
            if (string.IsNullOrEmpty(gerente))
            {
                return new GerenteProjetoDTO
                {
                    CodigoProjeto = "-1",
                    CodigoColaborador = "-1"
                };
            }
            var colaboradorGerente = _colaboradorContext.tb_colaborador_org.Where(x => x.tb_org_id == orgId && x.cod_colaborador_externo == gerente).FirstOrDefault();

            if (colaboradorGerente is null)
            {
                //throw new Exception("Busca por Gerente de projeto pelo Código Colaborador não foi efetuada com sucesso.");
                return new GerenteProjetoDTO
                {
                    CodigoProjeto = "-1",
                    CodigoColaborador = "-1"
                };
            }

            var cpfgerente = colaboradorGerente.codigo_interno_colaborador;

            if (string.IsNullOrEmpty(cpfgerente))
            {
                return new GerenteProjetoDTO
                {
                    CodigoProjeto = codProjeto,
                    CodigoColaborador = gerente
                };
            }
            var nomeGerente = _colaboradorContext.tb_colaborador.Where(x => x.codigo_interno_colaborador == cpfgerente).FirstOrDefault().nome_completo;

            return new GerenteProjetoDTO
            {
                CodigoProjeto = codProjeto,
                Cpf = cpfgerente,
                NomeColaborador = nomeGerente,
                CodigoColaborador = gerente
            };
        }

        public List<TbColaboradorPeriodoAlocacaoDTO> BuscaColaboradoresAlocadosNoProjeto(string cdProjeto, int orgId)
        {
            var colaboradores = _colaboradorContext.tb_colaborador_periodo_alocacao
                .Where(x => x.codigo_projeto == cdProjeto
                       && x.tb_org_id == orgId
                       && x.ativo == 1)
                .Select(x =>
                        new TbColaboradorPeriodoAlocacaoDTO()
                        {
                            Id = x.id,
                            DataInicio = x.data_inicio,
                            DataFim = x.data_fim,
                            QuantidadeHoras = x.quantidade_horas,
                            IncluiFimdesemana = x.inclui_fimdesemana,
                            Ativo = x.ativo,
                            DataCriacao = x.data_criacao,
                            DataAlteracao = x.data_alteracao,
                            Observacao = x.observacao,
                            Oportunidade = x.oportunidade,
                            Prioritario = x.prioritario,
                            Percentual = x.percentual,
                            CodigoColaborador = x.codigo_colaborador,
                            CodigoProjeto = x.codigo_projeto,
                            CodigoInternoColaborador = x.codigo_interno_colaborador,
                            TbOrgId = x.tb_org_id,
                            CodTbdAlocado = x.cod_tbd_alocado,
                            TbAtividadeId = x.tb_atividade_id
                        })
                .ToList();

            return colaboradores;
        }

        public async Task<IEnumerable<FeriadoAlocacaoDTO>> GetFeriadosPorOrgId(int orgId)
        {
            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    string query = @" SELECT
                                         id AS Id,
                                         data AS Data,
                                         nome AS Nome,
                                         tipo AS Tipo,
                                         ativo AS Ativo,
                                         data_criacao AS DataCriacao,
                                         data_atualizacao AS DataAtualizacao,
                                         tb_org_id AS TbOrgId
                                      FROM
                                         tb_feriado
                                      WHERE
                                         tb_org_id = @OrgId;";

                    var parameters = new { OrgId = orgId };

                    var feriados = await _connection.QueryAsync<FeriadoAlocacaoDTO>(query, parameters);

                    return feriados;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao executar GetFeriadosPorOrgId.", ex);
            }
        }
    }
}
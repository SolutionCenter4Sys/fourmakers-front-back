using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Infra.Context;
using Core.Domain.MapaAlocacao;
using Dapper;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Apontamento;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.CalculoMensal;
using DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.ConsultaAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador;
using DataTransferObject.Domain.MapaDeAlocacao.GetMapaAlocacao;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Usuario;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Helper.Extension;

namespace Colaboracao.Infra.Repositories.MapaAlocacao
{
    public class MapaAlocacaoRepository : IMapaAlocacaoRepository
    {
        private const sbyte ATIVO = 1;
        private const sbyte NAO_ATIVO = 0;
        private const int SIM = 1;
        private const int NAO = 0;
        private const int NUMEN = 4;
        private ColaboradorContext _colaboradorContext;
        private IConnectionStringCore _connectionString;
        private readonly IDBConnection _dapperConnection;

        public MapaAlocacaoRepository(ColaboradorContext colaboradorContext, IConnectionStringCore connectionString, IDBConnection dapperConnection)
        {
            _colaboradorContext = colaboradorContext;
            _connectionString = connectionString;
            _dapperConnection = dapperConnection;
        }

        public async Task<CadastroMapaAlocacaoDTO> CadastroMapaAlocacao(CadastroMapaAlocacaoDTO cadastroMapaAlocacaoDTO, int orgId, bool isTbd = false, bool isColaborador = true)
        {
            try
            {
                var listColabAloc = new List<tb_colaborador_periodo_alocacao>();

                DateTime dataInicio = new DateTime(cadastroMapaAlocacaoDTO.DataInicio.Year, cadastroMapaAlocacaoDTO.DataInicio.Month, cadastroMapaAlocacaoDTO.DataInicio.Day, 00, 00, 00);
                DateTime dataFim = new DateTime(cadastroMapaAlocacaoDTO.DataFim.Year, cadastroMapaAlocacaoDTO.DataFim.Month, cadastroMapaAlocacaoDTO.DataFim.Day, 23, 59, 59);

                var rowPeriodos = new tb_colaborador_periodo_alocacao();

                if (isColaborador)
                {
                    rowPeriodos.codigo_colaborador = cadastroMapaAlocacaoDTO.CodigoColaborador;
                    rowPeriodos.codigo_interno_colaborador = cadastroMapaAlocacaoDTO.CpfColaborador;
                }
                if (isTbd)
                {
                    rowPeriodos.cod_tbd_alocado = String.IsNullOrEmpty(cadastroMapaAlocacaoDTO.CodigoTbd) ? null : int.Parse(cadastroMapaAlocacaoDTO.CodigoTbd);
                }

                rowPeriodos.codigo_projeto = cadastroMapaAlocacaoDTO.CodigoProjeto.ToString();
                rowPeriodos.ativo = ATIVO;
                rowPeriodos.tb_org_id = orgId;
                rowPeriodos.ativo = ATIVO;
                rowPeriodos.inclui_fimdesemana = (sbyte)(cadastroMapaAlocacaoDTO.IncluiFimDeSemana ? SIM : NAO);
                rowPeriodos.data_inicio = dataInicio;
                rowPeriodos.data_fim = cadastroMapaAlocacaoDTO.DataFim;
                rowPeriodos.quantidade_horas = cadastroMapaAlocacaoDTO.QuantidadeHoras;
                rowPeriodos.retroalimenta_cv = (sbyte)((cadastroMapaAlocacaoDTO.FlagRetroalimentaCV == true) ? SIM : NAO);

                rowPeriodos.percentual = cadastroMapaAlocacaoDTO.Percentual;
                rowPeriodos.observacao = cadastroMapaAlocacaoDTO.Observacao;
                rowPeriodos.prioritario = cadastroMapaAlocacaoDTO.Prioritario;
                rowPeriodos.oportunidade = cadastroMapaAlocacaoDTO.Oportunidade;

                _colaboradorContext.tb_colaborador_periodo_alocacao.Add(rowPeriodos);

                await _colaboradorContext.SaveChangesAsync();
                cadastroMapaAlocacaoDTO.PeriodoAlocadoId = rowPeriodos.id;
                return cadastroMapaAlocacaoDTO;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro na CadastroMapaAlocacao");
                throw;
            }
        }

        public async Task CadastrarAssociacaoAutomaticaNoProjeto(string codigoColaborador, string codigoProjeto, int orgId)
        {
            if (!string.IsNullOrEmpty(codigoColaborador)
                && !string.IsNullOrEmpty(codigoProjeto))
            {
                var rowColabProjetoOrg = await _colaboradorContext.tb_colaborador_projeto_org
                    .Where(x => x.cod_colaborador == codigoColaborador
                                 && x.cod_projeto == codigoProjeto
                                 && x.tb_org_id == orgId).FirstOrDefaultAsync();

                if (rowColabProjetoOrg == null)
                {
                    var colabProjetoOrg = new tb_colaborador_projeto_org()
                    {
                        cod_colaborador = codigoColaborador,
                        cod_projeto = codigoProjeto,
                        nome_projeto = _colaboradorContext.tb_projeto_org.AsNoTracking()
                                                                         .Where(x => x.cod_projeto == codigoProjeto && x.tb_org_id == orgId)
                                                                         .Select(x => x.projeto).FirstOrDefault().ToString(),
                        tb_org_id = orgId,
                    };

                    _colaboradorContext.tb_colaborador_projeto_org.Add(colabProjetoOrg);
                    _colaboradorContext.SaveChanges();
                }
            }
        }

        public List<HorasMensalDTO> GetHorasTotalProjeto(List<HorasMensalDTO> mesesBase, string codProjeto, IEnumerable<FeriadoAlocacaoDTO> feriados, int org_id)
        {
            try
            {
                return GetHorasAlocadas(mesesBase, feriados, _colaboradorContext.tb_colaborador_periodo_alocacao
                    .Where(x => x.ativo == ATIVO
                                && x.codigo_projeto == codProjeto
                                && x.tb_org_id == org_id).ToList());
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<CalculoMensalDTO>> GetCalculosMensaisAsync(int orgId, string codigoColaborador = null, int? codTbdAlocado = null)
        {
            const string query = @"SELECT
                                        codigo_colaborador AS CodigoColaborador,
                                        cod_tbd_alocado AS CodTbdAlocado,
                                        mes AS Mes,
                                        ano AS Ano,
                                        horas AS Horas,
                                        status_colaborador_periodo_alocacao AS StatusColaboradorPeriodoAlocacao,
                                        tb_org_id AS TbOrgId
                                    FROM
                                        tb_colaborador_periodo_alocacao_calculo_mensal
                                    WHERE
                                        tb_org_id = @OrgId
                                        AND (@CodigoColaborador IS NULL OR codigo_colaborador = @CodigoColaborador)
                                        AND (@CodTbdAlocado IS NULL OR cod_tbd_alocado = @CodTbdAlocado);";

            var parameters = new { OrgId = orgId, CodigoColaborador = codigoColaborador, CodTbdAlocado = codTbdAlocado };

            using (var connection = _connectionString.CreateMySqlConnection())
            {
                return (await connection.QueryAsync<CalculoMensalDTO>(query, parameters)).ToList();
            }
        }

        private static List<HorasMensalDTO> GetHorasAlocadas(List<HorasMensalDTO> mesesBase, IEnumerable<FeriadoAlocacaoDTO> feriados, List<tb_colaborador_periodo_alocacao> periodosAlocados)
        {
            foreach (var dia in mesesBase)
            {
                var alocacaoNoMes = periodosAlocados.Where(x =>
                    long.Parse(x.data_inicio.ToString("yyyy") + x.data_inicio.ToString("MM")) <= long.Parse(dia.Ano.ToString() + dia.Mes.ToString("00"))
                 && long.Parse(x.data_fim.ToString("yyyy") + x.data_fim.ToString("MM")) >= long.Parse(dia.Ano.ToString() + dia.Mes.ToString("00"))
                 && x.codigo_projeto == x.codigo_projeto).ToList();

                if (alocacaoNoMes.Any())
                {
                    foreach (var periodoAlocacao in alocacaoNoMes)
                    {
                        var dataInicio = periodoAlocacao.data_inicio > new DateTime(dia.Ano, dia.Mes, 1) ? periodoAlocacao.data_inicio : new DateTime(dia.Ano, dia.Mes, 1);
                        var dataLimite = periodoAlocacao.data_fim < new DateTime(dia.Ano, dia.Mes, DateTime.DaysInMonth(dia.Ano, dia.Mes)) ?
                            periodoAlocacao.data_fim
                            : new DateTime(dia.Ano, dia.Mes, DateTime.DaysInMonth(dia.Ano, dia.Mes));

                        DateTime data = dataInicio;

                        while (data <= dataLimite)
                        {
                            if (feriados.FirstOrDefault(feriado => feriado.Data == data) is null)
                            {
                                if (periodoAlocacao.inclui_fimdesemana == NAO)
                                {
                                    if (data.DayOfWeek != DayOfWeek.Saturday && data.DayOfWeek != DayOfWeek.Sunday)
                                    {
                                        dia.Horas += periodoAlocacao.quantidade_horas;
                                    }
                                }
                                else
                                {
                                    dia.Horas += periodoAlocacao.quantidade_horas;
                                }
                            }

                            data = data.AddDays(1);
                        }
                    }
                }
            }
            return mesesBase;
        }

        public bool ValidaAcesso(string cpf, int orgId)
        {
            try
            {
                if (_colaboradorContext.tb_usuario_grupo_acesso.Where(
                    x =>
                        x.tb_usuario.codigo_interno_colaborador == cpf
                        && (x.tb_grupo_acesso.tb_org_id == orgId)
                        && x.ativo == ATIVO
                        && x.tb_grupo_acesso.nivel == 0
                        && x.tb_grupo_acesso.tb_grupo_acesso_funcionalidade_sistema.Where(y => y.tb_funcionalidade_sistema.descricao == FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO.ToString()).Any()
                    ).Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> GetQuantidadeColaboradoresAsync(int orgId)
        {
            try
            {
                using (var connection = _connectionString.CreateMySqlConnection())
                {
                    var sql = @"SELECT
                                    COUNT(*)
                                FROM
                                    tb_colaborador_org co
                                    INNER JOIN tb_colaborador c ON co.codigo_interno_colaborador = c.codigo_interno_colaborador
                                WHERE
                                    co.tb_org_id = @OrgId AND c.ativo = 1";

                    var parameters = new { OrgId = orgId };

                    var quantidadeColaboradores = await connection.ExecuteScalarAsync<int>(sql, parameters);

                    return quantidadeColaboradores;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter quantidade de colaboradores.", ex);
            }
        }

        public async Task RemoverAlocacaoPorIds(long[] idsPeriodoAlocacao)
        {
            const string updateQuery = @"   UPDATE
                                                tb_colaborador_periodo_alocacao
                                            SET
                                                ativo = @NaoAtivo
                                            WHERE
                                                id IN @Ids";

            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    await _connection.OpenAsync();

                    var rowsAffected = await _connection.ExecuteAsync(updateQuery, new { Ids = idsPeriodoAlocacao, NaoAtivo = NAO_ATIVO });

                    if (rowsAffected == 0)
                    {
                        throw new Exception("Nenhuma alocação foi atualizada. Verifique se as alocações já não estão excluídas ou se os IDs são válidos.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao remover alocações em lote", ex);
            }
        }

        public async Task<List<BuscarCargaMapaAlocacaoDTO>> BuscarCargaPeriodoAlocacaoFoursysBI()
        {
            var query = @"  SELECT
                                tpa.codigo_colaborador AS CdColaboradorCCH,
                                tpa.codigo_projeto AS CdProjeto,
                                COALESCE(tpg.cod_colaborador_gerente, '') AS codProjetoGerente,
                                COALESCE(tc_g.nome_completo, '') AS ManagerName,
                                COALESCE(tu_g.email, '') AS ManagerEmail,
                                COALESCE(tpo.projeto, '') AS Projectname,
                                tpa.quantidade_horas AS DailyHours,
                                DATE_FORMAT(tpa.data_inicio, '%Y-%m-%dT%H:%i:%s') AS DateStart,
                                DATE_FORMAT(tpa.data_fim, '%Y-%m-%dT%H:%i:%s') AS DateFinal,
                                CASE 
                                   WHEN tpa.cod_tbd_alocado IS NULL THEN 0 
                                   ELSE 1 
                                END AS ehTbd,
                                tgep.custo_perfil as custoPerfil,
                                tgep.ratecard_perfil as rateCard
                            FROM
                                tb_colaborador_periodo_alocacao tpa
                                LEFT JOIN tb_projeto_org tpo ON tpa.codigo_projeto = tpo.cod_projeto AND tpo.tb_org_id = 2
                                LEFT JOIN tb_projeto_gerente tpg ON tpa.codigo_projeto = tpg.cod_projeto AND tpg.tb_org_id = 2 AND tpg.tipo_gerente = 'GerenteProjeto'
                                LEFT JOIN tb_perfil_alocacao tbpa ON tbpa.tb_colaborador_periodo_alocacao_id = tpa.id
                                LEFT JOIN tb_gestor_externo_perfil tgep ON tgep.id = tbpa.tb_gestor_externo_perfil_id 
                                LEFT JOIN tb_colaborador_org tco_g ON tco_g.cod_colaborador_externo = tpg.cod_colaborador_gerente AND tco_g.tb_org_id = 2
                                LEFT JOIN tb_colaborador tc_g ON tc_g.codigo_interno_colaborador = tco_g.codigo_interno_colaborador
                                LEFT JOIN tb_usuario tu_g ON tc_g.codigo_interno_colaborador = tu_g.codigo_interno_colaborador
                            WHERE
                                tpa.tb_org_id = 2 AND tpa.ativo = 1";

            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    var result = await _connection.QueryAsync<BuscarCargaMapaAlocacaoDTO>(query);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consultar BI", ex);
            }
        }

        public async Task<List<PeriodoAlocadoDTO>> GetPeriodoAlocacaoColaboradorComFiltro(string cpf, int? tbdId, int mes, int ano, string codigoProjeto, int orgId, bool filtrarPorMesAtualEFuturas)
        {
            try
            {
                var dataInicial = new DateTime(ano, mes, 1);
                var dataFinal = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));

                var retPeriodo = _colaboradorContext.tb_colaborador_periodo_alocacao
                                                     .Where(x => (filtrarPorMesAtualEFuturas ? (x.data_fim >= dataInicial) : ((x.data_inicio >= dataInicial || x.data_fim >= dataInicial)
                                                                  && (x.data_inicio <= dataFinal && x.data_fim >= dataInicial)))
                                                                  && ((tbdId.ToIntOuZero() == 0 && x.codigo_interno_colaborador == cpf) || (cpf.IsNull() && x.cod_tbd_alocado == tbdId))
                                                                  && (codigoProjeto == null || x.codigo_projeto == codigoProjeto)
                                                                  && x.tb_org_id == orgId
                                                                  && x.ativo == ATIVO);

                var retResult = new List<PeriodoAlocadoDTO>();

                foreach (var periodoAlocacao in await retPeriodo.ToListAsync())
                {
                    var projetoOrg = _colaboradorContext.tb_projeto_org.Where(tpo => tpo.cod_projeto == periodoAlocacao.codigo_projeto && tpo.tb_org_id == orgId).SingleOrDefault();

                    var codProjetoGerente = _colaboradorContext.tb_projeto_gerente
                                                        .Where(x => x.cod_projeto == periodoAlocacao.codigo_projeto && x.tb_org_id == orgId && x.tipo_gerente == "GerenteProjeto").FirstOrDefault()?.cod_colaborador_gerente ?? null;
                    var gerenteNome = "";
                    if (codProjetoGerente != null)
                        gerenteNome = _colaboradorContext.tb_colaborador_org.Where(x => x.cod_colaborador_externo == codProjetoGerente && x.tb_org_id == orgId).FirstOrDefault()?.codigo_interno_colaboradorNavigation.nome_completo ?? "";

                    var clienteNome = _colaboradorContext.tb_cliente_org.Where(x => x.codigo_cliente == projetoOrg.cod_cliente && x.tb_org_id == orgId).FirstOrDefault()?.nome_cliente;

                    retResult.Add(new PeriodoAlocadoDTO
                    {
                        PeriodoAlocadoId = periodoAlocacao.id,
                        CpfColaborador = periodoAlocacao.codigo_interno_colaborador,
                        CodigoTBD = periodoAlocacao.cod_tbd_alocado,
                        DataInicio = periodoAlocacao.data_inicio,
                        DataFim = periodoAlocacao.data_fim,
                        DataAlteracao = periodoAlocacao.data_alteracao,
                        IncluiFimDeSemana = periodoAlocacao.inclui_fimdesemana == SIM,
                        CodigoProjeto = periodoAlocacao.codigo_projeto,
                        NomeProjeto = projetoOrg != null ? projetoOrg.projeto : "",
                        CodigoCliente = projetoOrg.cod_cliente,
                        NomeCliente = clienteNome,
                        LabelProjetoCliente = projetoOrg != null ? MapaDeAlocacaoConstants.RetornarLabelCliente(projetoOrg.cod_cliente, clienteNome, projetoOrg.cod_projeto, projetoOrg.projeto) : "",
                        CodigoGestor = codProjetoGerente,
                        NomeGestor = gerenteNome,
                        QuantidadeHoras = periodoAlocacao.quantidade_horas,
                        Observacao = periodoAlocacao.observacao,
                        Oportunidade = periodoAlocacao.oportunidade,
                        Percentual = periodoAlocacao.percentual,
                        Prioritario = (sbyte)periodoAlocacao.prioritario,
                    });
                }

                return retResult;
            }
            catch
            {
                throw;
            }
        }

        public async Task<AlocacaoColabETbdDTO> EditarAlocacao(long periodoAlocacaoId, DateTime? dataInicio, DateTime? dataFim, bool? incluiFimDeSemana, double? quantidadeDeHoras, sbyte? prioritario, string observacao, string oportunidade, double? percentual, string codigoColaborador, string colaboradorCpf, int? codTbdAlocado, string codigoProjeto, bool? flagRetroalimentaCV)
        {
            try
            {
                var dbrow = await _colaboradorContext.tb_colaborador_periodo_alocacao
                    .Where(x => x.id == periodoAlocacaoId && x.ativo == ATIVO).FirstOrDefaultAsync()
                    ?? throw new Exception("Alocação não encontrada");

                if (dataInicio != dbrow.data_inicio && dataInicio != null)
                {
                    DateTime dataInicioEditada = new DateTime(dataInicio.Value.Year, dataInicio.Value.Month, dataInicio.Value.Day, 00, 00, 00);
                    dbrow.data_inicio = dataInicioEditada;
                }
                if (dataFim != dbrow.data_fim && dataInicio != null)
                {
                    DateTime dataFimEditada = new DateTime(dataFim.Value.Year, dataFim.Value.Month, dataFim.Value.Day, 23, 59, 59);
                    dbrow.data_fim = dataFimEditada;
                }
                if (incluiFimDeSemana != null)
                {
                    dbrow.inclui_fimdesemana = (sbyte)((bool)incluiFimDeSemana ? 1 : 0);
                }
                if (quantidadeDeHoras != dbrow.quantidade_horas && quantidadeDeHoras != null)
                {
                    dbrow.quantidade_horas = (double)quantidadeDeHoras;
                }
                if ((prioritario != null) && (prioritario == 0 || prioritario == 1) && prioritario != dbrow.prioritario)
                {
                    dbrow.prioritario = prioritario;
                }
                if (observacao != null && (dbrow.observacao == null || observacao.ToUpper() != dbrow.observacao.ToUpper()))
                {
                    dbrow.observacao = observacao;
                }
                if (oportunidade != null && (dbrow.oportunidade == null || oportunidade.ToUpper() != dbrow.oportunidade.ToUpper()))
                {
                    dbrow.oportunidade = oportunidade;
                }
                if (percentual != null && percentual != dbrow.percentual)
                {
                    dbrow.percentual = percentual;
                }

                if (codigoColaborador != null && codigoColaborador != dbrow.codigo_colaborador)
                {
                    dbrow.codigo_colaborador = codigoColaborador;
                    dbrow.cod_tbd_alocado = null;

                    if (colaboradorCpf.IsEmpty())
                    {
                        throw new Exception("Para editar a alocação, colaborador e cpf devem ser informados.");
                    }
                }

                if (colaboradorCpf != null && colaboradorCpf != dbrow.codigo_interno_colaborador)
                {
                    dbrow.codigo_interno_colaborador = colaboradorCpf;

                    if (codigoColaborador.IsEmpty())
                    {
                        throw new Exception("Para editar a alocação, colaborador e cpf devem ser informados.");
                    }
                }

                if (codTbdAlocado != null && codTbdAlocado != dbrow.cod_tbd_alocado)
                {
                    dbrow.cod_tbd_alocado = codTbdAlocado;
                    dbrow.codigo_colaborador = null;
                    dbrow.codigo_interno_colaborador = null;
                }

                if (codigoProjeto != null && codigoProjeto != dbrow.codigo_projeto)
                {
                    dbrow.codigo_projeto = codigoProjeto;
                }

                if (flagRetroalimentaCV != null)
                {
                    dbrow.retroalimenta_cv = (sbyte)(flagRetroalimentaCV == true ? 1 : 0);
                }

                _colaboradorContext.SaveChanges();

                return (await ListarAlocacoesColaboradoresETbds(null, dbrow.id.ToIntOuZero(), dbrow.tb_org_id)).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public List<ColaboradorAlocadoDTO> ColaboradoresAlocadosNoProjeto(string projetoId, int orgId)
        {
            try
            {
                var colaboradoresAlocados = _colaboradorContext.tb_colaborador_periodo_alocacao
                                                               .Where(x => x.codigo_projeto == projetoId && x.ativo == ATIVO && x.tb_org_id == orgId)
                                                               .GroupBy(alocado => new
                                                               {
                                                                   alocado.codigo_interno_colaborador,
                                                                   NomeColaborador = alocado.codigo_interno_colaboradorNavigation.nome_completo ?? alocado.cod_tbd_alocadoNavigation.descricao,
                                                                   alocado.cod_tbd_alocado
                                                               })
                                                               .Select(g => new ColaboradorAlocadoDTO
                                                               {
                                                                   NomeColaborador = g.Key.NomeColaborador,
                                                                   CpfColaborador = g.Key.codigo_interno_colaborador,
                                                                   Codigo_tbd_Alocado = g.Key.cod_tbd_alocado,
                                                               })
                                                               .ToList();

                return colaboradoresAlocados;
            }
            catch
            {
                throw;
            }
        }

        public List<ColaboradorAlocadoDTO> ColaboradoresAlocadosNoProjetoFiltradoPorData(string projetoId, int orgId, DateTime inicioProjeto, DateTime fimProjeto)
        {
            try
            {
                var colaboradoresAlocados = _colaboradorContext.tb_colaborador_periodo_alocacao
                                                                       .Where(x => x.codigo_projeto == projetoId.ToString()
                                                                                   && x.ativo == ATIVO
                                                                                   && x.tb_org_id == orgId
                                                                                   && x.data_fim >= inicioProjeto && x.data_fim <= fimProjeto)
                                                                       .GroupBy(alocado => new
                                                                       {
                                                                           alocado.codigo_interno_colaboradorNavigation.nome_completo,
                                                                           alocado.codigo_interno_colaborador,
                                                                           alocado.cod_tbd_alocadoNavigation.descricao,
                                                                           alocado.cod_tbd_alocado
                                                                       })
                                                                       .Select(g => new ColaboradorAlocadoDTO
                                                                       {
                                                                           NomeColaborador = g.Key.nome_completo ?? g.Key.descricao,
                                                                           CpfColaborador = g.Key.codigo_interno_colaborador,
                                                                           Codigo_tbd_Alocado = g.Key.cod_tbd_alocado,
                                                                       })
                                                                       .ToList();

                return colaboradoresAlocados;
            }
            catch
            {
                throw;
            }
        }

        public List<ColaboradorCchDTO> ListarColaboradoresOrg(string busca, int cursor, int limite, int orgId, List<string>? restricaoDiretorias = null)
        {
            try
            {
                IQueryable<tb_colaborador_org> query = _colaboradorContext.tb_colaborador_org.Where(x => x.tb_org_id == orgId && x.cod_diretoria != "BANCO TALENTOS")
               .OrderBy(x => x.codigo_interno_colaboradorNavigation.nome_completo);

                if (restricaoDiretorias != null && restricaoDiretorias.Count > 0)
                    query = query.Where(x => restricaoDiretorias.Contains(x.cod_diretoria));

                if (!string.IsNullOrEmpty(busca))
                    query = query.Where(x => EF.Functions.Like(x.codigo_interno_colaboradorNavigation.nome_completo.ToUpper(), $"%{busca.ToUpper()}%"));

                return query
                    .Skip(cursor)
                    .Take(limite)
                    .Select(x => new ColaboradorCchDTO()
                    {
                        CdProfissional = x.cod_colaborador_externo,
                        NmProfissional = x.codigo_interno_colaboradorNavigation.nome_completo,
                        CodigoColaboradorInterno = x.codigo_interno_colaboradorNavigation.codigo_interno_colaborador
                    })
                    .ToList();
            }
            catch (Exception)
            {
                try
                {
                    IQueryable<tb_colaborador_org> query = _colaboradorContext.tb_colaborador_org.Where(x => x.tb_org_id == orgId)
                    .OrderBy(x => x.codigo_interno_colaboradorNavigation.nome_completo);

                    if (restricaoDiretorias != null && restricaoDiretorias.Count > 0)
                        query = query.Where(x => restricaoDiretorias.Contains(x.cod_diretoria));

                    if (!string.IsNullOrEmpty(busca))
                        query = query.Where(x => EF.Functions.Like(x.codigo_interno_colaboradorNavigation.nome_completo.ToUpper(), $"%{busca.ToUpper()}%"));

                    return query
                        .Skip(cursor)
                        .Take(limite)
                        .Select(x => new ColaboradorCchDTO()
                        {
                            CdProfissional = x.cod_colaborador_externo,
                            NmProfissional = x.codigo_interno_colaboradorNavigation.nome_completo,
                            CodigoColaboradorInterno = x.codigo_interno_colaboradorNavigation.codigo_interno_colaborador
                        })
                        .ToList();
                }
                catch (Exception)
                {
                    throw;
                }
                throw;
            }
        }
        public List<ProjetosCchDTO> ListarProjetosOrg(string busca, int cursor, int limite, int orgId)
        {
            try
            {
                IQueryable<tb_projeto_org> query = _colaboradorContext.tb_projeto_org.Where(x => x.tb_org_id == orgId)
               .OrderBy(x => x.projeto);

                if (!string.IsNullOrEmpty(busca))
                    query = query.Where(x => EF.Functions.Like(x.projeto.ToUpper(), $"%{busca.ToUpper()}%"));

                return query
                    .Skip(cursor)
                    .Take(limite)
                    .Select(x => new ProjetosCchDTO()
                    {
                        cdProjeto = int.Parse(x.cod_projeto),
                        nmProjeto = x.projeto
                    })
                    .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string BuscaNomeProjeto(string codigoProjeto, int orgId)
        {
            return _colaboradorContext.tb_projeto_org.Where(x => x.cod_projeto == codigoProjeto && x.tb_org_id == orgId).FirstOrDefault().projeto ?? null;
        }

        public bool ExisteCodigoProjetoParaOrgId(string codigoProjeto, int orgId)
        {
            return _colaboradorContext.tb_projeto_org.Where(x => x.cod_projeto == codigoProjeto && x.tb_org_id == orgId).Any();
        }

        public void DeletarPeriodo(long periodoId)
        {
            try
            {
                var item = _colaboradorContext.tb_colaborador_periodo_alocacao.Where(x => x.id == periodoId).FirstOrDefault();
                if (item is not null)
                {
                    _colaboradorContext.tb_colaborador_periodo_alocacao.Remove(item);
                }
            }
            catch
            {
                throw;
            }
        }

        private CadastroMapaAlocacaoDTO MontaCadastroMapaAlocacaoDTO(tb_colaborador_periodo_alocacao row)
        {
            try
            {
                return new CadastroMapaAlocacaoDTO()
                {
                    PeriodoAlocadoId = row.id,
                    CpfColaborador = row.codigo_interno_colaborador,
                    CodigoColaborador = row.codigo_colaborador,
                    CodigoTbd = row.cod_tbd_alocado.ToString(),
                    CodigoProjeto = row.codigo_projeto,
                    Ativo = row.ativo == 1,
                    DataCriacao = row.data_criacao,
                    OrgId = row.tb_org_id,
                    DataInicio = row.data_inicio,
                    DataFim = row.data_fim,
                    IncluiFimDeSemana = row.inclui_fimdesemana == 1,
                    QuantidadeHoras = row.quantidade_horas,
                    Observacao = row.observacao,
                    Oportunidade = row.oportunidade,
                    Percentual = row.percentual,
                    Prioritario = row.prioritario,
                    DataAlteracao = row.data_alteracao
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Erro no MontaCadastroMapaAlocacaoDTO");
                throw;
            }
        }

        public CadastroMapaAlocacaoDTO GetColaboradorPeriodoAlocacaoById(long id)
        {
            try
            {
                var tbdRow = _colaboradorContext.tb_colaborador_periodo_alocacao.AsNoTracking().Where(row => row.id == id).SingleOrDefault();
                return tbdRow != null ? MontaCadastroMapaAlocacaoDTO(tbdRow) : null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Erro no GetTbColaboradorAlocadoById");
                throw;
            }
        }

        public List<CadastroMapaAlocacaoDTO> GetColaboradorPeriodoAlocacaoByIds(long[] ids)
        {
            try
            {
                var periodosList = _colaboradorContext.tb_colaborador_periodo_alocacao.Where(row => ids.Contains(row.id)).ToList();
                return periodosList.Select(row => MontaCadastroMapaAlocacaoDTO(row)).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Erro no GetTbColaboradorAlocadoById");
                throw;
            }
        }

        public List<CadastroMapaAlocacaoDTO> GetColaboradorPeriodoAlocacaoTodos()
        {
            try
            {
                var periodosList = _colaboradorContext.tb_colaborador_periodo_alocacao.ToList();
                return periodosList.Select(row => MontaCadastroMapaAlocacaoDTO(row)).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Erro no GetTbColaboradorAlocadoById");
                throw;
            }
        }

        public async Task<(List<dynamic> alocacoes, List<dynamic> perfis)> ListarAlocacoesColaboradoresETbdsDynamicAsync(string pesquisa,
            int? periodoAlocacaoId,
            int orgId,
            List<string> codigoUnidade = null,
            string codigoDepartamento = null,
            string codigoGestorAdm = null,
            List<string> listaCodigoColabOuTbd = null,
            TipoProfissionalEnum filtroTipoProfissional = 0,
            string codigoGestorProjeto = null,
            List<string> listaCodigoClientes = null,
            bool apenasProjetosPrioritarios = false,
            List<string> listaCodigoProjetos = null,
            string codigoStatusProjeto = null,
            string qtdGerenteProjetoPrioridade = null,
            bool incluirInativos = false,
            string habilidades = null, // ignorado
            string perfis = null)
        {
            var connection = _dapperConnection.GetConnection();
                
            var inClauseProjetos = listaCodigoProjetos.BuildInClauseOrNull();
            var inClauseClientes = listaCodigoClientes.BuildInClauseOrNull();
            var inClauseUnidades = codigoUnidade.BuildInClauseOrNull();
            var inClauseColabsOuTbds = listaCodigoColabOuTbd.BuildInClauseOrNull();

            var query = $@"
               WITH alocacoes AS (
                    -- 1️⃣ Resultset principal: alocações
                    SELECT
                        tcpa.id AS periodo_alocado_id,
                        tco.cod_diretoria,
                        tco.diretoria,
                        CASE
                            WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN ttbd.cod_departamento
                            ELSE tco.cod_departamento
                        END AS cod_departamento,
                        CASE
                            WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN ttbd.departamento
                            ELSE tco.departamento
                        END AS departamento,
                        CASE
                            WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN tcpa.cod_tbd_alocado
                            ELSE tcpa.codigo_colaborador
                        END AS codigo_colaborador,
                        CASE
                            WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN ttbd.descricao
                            ELSE tc.nome_completo
                        END AS nome_completo,
                        vgct_gestor.nome_gestor_adm,
                        tcpa.codigo_interno_colaborador,
                        tcpa.codigo_projeto,
                        tpo.projeto,
                        tpo.cod_cliente,
                        tclo.nome_cliente AS cliente,
                        tpo.status,
                        tpg.cod_colaborador_gerente AS cod_gerente,
                        tg.nome_completo AS nome_gerente,
                        ROW_NUMBER() OVER (PARTITION BY tcpa.id ORDER BY tpg.cod_colaborador_gerente) AS gerente_projeto_prioridade__ND,
                        DATE_FORMAT(tcpa.data_inicio, '%d/%m/%Y') AS data_inicio,
                        DATE_FORMAT(tcpa.data_fim, '%d/%m/%Y') AS data_fim,
                        tcpa.quantidade_horas AS quantidade_horas__ND,
                        REPLACE(CAST(tcpa.quantidade_horas AS CHAR), '.', ',') AS quantidade_horas,
                        tcpa.percentual,
                        CASE WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN 1 ELSE 0 END AS tbd,
                        tcpa.observacao,
                        tcpa.oportunidade,
                        tcpa.prioritario,
                        tcpa.retroalimenta_cv
                    FROM tb_colaborador_periodo_alocacao tcpa
                    LEFT JOIN tb_colaborador_org tco 
                        ON tcpa.codigo_colaborador = tco.cod_colaborador_externo 
                        AND tco.tb_org_id = tcpa.tb_org_id
                    LEFT JOIN tb_colaborador tc 
                        ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    LEFT JOIN tb_tbd_alocado ttbd 
                        ON tcpa.cod_tbd_alocado = ttbd.cod_tbd_alocado 
                        AND tcpa.tb_org_id = ttbd.tb_org_id
                    LEFT JOIN vw_gestores_colaborador_tbd vgct_gestor 
                        ON (((CASE WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN 1 ELSE 0 END) = vgct_gestor.eh_tbd 
                        AND ttbd.cod_tbd_alocado = vgct_gestor.cod_colaborador_externo)
                        OR ((CASE WHEN tcpa.cod_tbd_alocado IS NOT NULL THEN 1 ELSE 0 END) = vgct_gestor.eh_tbd 
                        AND tco.cod_colaborador_externo = vgct_gestor.cod_colaborador_externo))
                        AND tcpa.tb_org_id = vgct_gestor.tb_org_id
                    LEFT JOIN tb_projeto_org tpo 
                        ON tcpa.codigo_projeto = tpo.cod_projeto 
                        AND tpo.tb_org_id = tcpa.tb_org_id
                    LEFT JOIN tb_cliente_org tclo 
                        ON tpo.cod_cliente = tclo.codigo_cliente 
                        AND tpo.tb_org_id = tclo.tb_org_id
                    LEFT JOIN tb_projeto_gerente tpg 
                        ON tpo.cod_projeto = tpg.cod_projeto 
                        AND tpg.tb_org_id = tcpa.tb_org_id
                    LEFT JOIN tb_colaborador_org tco_g 
                        ON tpg.cod_colaborador_gerente = tco_g.cod_colaborador_externo 
                        AND tco_g.tb_org_id = tcpa.tb_org_id
                    LEFT JOIN tb_colaborador tg 
                        ON tco_g.codigo_interno_colaborador = tg.codigo_interno_colaborador
                    WHERE
                        tcpa.ativo = 1
                        AND (COALESCE(tcpa.cod_tbd_alocado,'') <> '' OR (tco.ativo = 1 OR (tco.ativo = 0 AND @Incluir_inativos = TRUE)))
                        AND tcpa.tb_org_id = @Org_id
                        AND (@Periodo_alocado_id IS NULL OR tcpa.id = @Periodo_alocado_id)
                        AND (
                            @Pesquisa IS NULL OR
                            tcpa.codigo_colaborador = @Pesquisa OR
                            tcpa.cod_tbd_alocado = @Pesquisa OR
                            tc.nome_completo LIKE CONCAT('%', @Pesquisa, '%') OR
                            tpo.projeto LIKE CONCAT('%', @Pesquisa, '%') OR
                            tpo.cod_cliente = @Pesquisa OR
                            tclo.nome_cliente LIKE CONCAT('%', @Pesquisa, '%') OR
                            tg.nome_completo LIKE CONCAT('%', @Pesquisa, '%')
                        )
                        AND (@Codigo_departamento IS NULL OR ((tcpa.cod_tbd_alocado IS NOT NULL 
                            AND ttbd.cod_departamento = @Codigo_departamento) 
                            OR (tcpa.cod_tbd_alocado IS NULL AND tco.cod_departamento = @Codigo_departamento)))
                        AND (@Codigo_gestor_adm IS NULL OR vgct_gestor.cod_colaborador_superior = @Codigo_gestor_adm)
                        AND (@Codigo_gestor_projeto IS NULL OR tpg.cod_colaborador_gerente = @Codigo_gestor_projeto)
                        AND ((@Apenas_projetos_prioritarios IS NULL OR @Apenas_projetos_prioritarios = FALSE) OR tpo.prioritario = TRUE)
                        AND (@Codigo_status_projeto IS NULL OR tpo.cod_status = @Codigo_status_projeto)
                        {(inClauseProjetos == null ? "" : $"AND tpo.cod_projeto IN {inClauseProjetos}")}
                        {(inClauseClientes == null ? "" : $"AND tpo.cod_cliente IN {inClauseClientes}")}
                        {(inClauseUnidades == null ? "" : $"AND tco.cod_diretoria IN {inClauseUnidades}")}
                        {(inClauseColabsOuTbds == null ? "" : $"AND (tcpa.cod_tbd_alocado IN {inClauseColabsOuTbds} OR tco.cod_colaborador_externo IN {inClauseColabsOuTbds})")}
                        AND (
                            (@FiltroTipoProfissional = 0 OR @FiltroTipoProfissional IS NULL)
                            OR
                            (@FiltroTipoProfissional = 1 AND tcpa.cod_tbd_alocado IS NULL)
                            OR
                            (@FiltroTipoProfissional = 2 AND tcpa.cod_tbd_alocado IS NOT NULL)
                        )
                )
                SELECT *
                FROM alocacoes
                WHERE
                    COALESCE(@QtdGerenteProjetoPrioridade, 0) = 0
                    OR gerente_projeto_prioridade__ND = @QtdGerenteProjetoPrioridade
                ORDER BY nome_completo, data_fim DESC;

                -- 2️⃣ Resultset secundário: perfis
                SELECT
                    vp.id,
                    vp.perfil,
                    tpa.tb_colaborador_periodo_alocacao_id
                FROM tb_perfil_alocacao tpa
                INNER JOIN vw_perfis vp 
                    ON vp.tb_org_id = tpa.tb_org_id
                    AND (vp.id = CONCAT('1|', tpa.tb_perfil_id)
                    OR vp.id = CONCAT('2|', tpa.tb_gestor_externo_perfil_id))
                    AND vp.ativo = 1
                WHERE tpa.tb_org_id = @Org_id;
            ";

            await connection.ExecuteAsync("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");

            var dadosResult = await connection.QueryMultipleAsync(
                query,
                new
                {
                    Pesquisa = pesquisa,
                    Org_id = orgId,
                    Periodo_alocado_id = periodoAlocacaoId,
                    Codigo_departamento = codigoDepartamento,
                    Codigo_gestor_adm = codigoGestorAdm,
                    Codigo_gestor_projeto = codigoGestorProjeto,
                    Codigo_status_projeto = codigoStatusProjeto,
                    Apenas_projetos_prioritarios = apenasProjetosPrioritarios,
                    Incluir_inativos = incluirInativos,
                    FiltroTipoProfissional = (int)filtroTipoProfissional,
                    QtdGerenteProjetoPrioridade = qtdGerenteProjetoPrioridade
                }
            );

            var listaAlocacoesList = (await dadosResult.ReadAsync()).ToList();
            var listaPerfisList = (await dadosResult.ReadAsync()).ToList();

            return (
                listaAlocacoesList,
                listaPerfisList
            );
        }   

        public async Task<IEnumerable<AlocacaoColabETbdDTO>> ListarAlocacoesColaboradoresETbds(
            string pesquisa,
            int? periodoAlocacaoId,
            int orgId,
            List<string> codigoUnidade = null,
            string codigoDepartamento = null,
            string codigoGestorAdm = null,
            List<string> listaCodigoColabOuTbd = null,
            TipoProfissionalEnum filtroTipoProfissional = 0,
            string codigoGestorProjeto = null,
            List<string> listaCodigoClientes = null,
            bool apenasProjetosPrioritarios = false,
            List<string> listaCodigoProjetos = null,
            string codigoStatusProjeto = null,
            string qtdGerenteProjetoPrioridade = null,
            bool incluirInativos = false,
            string habilidades = null, // ignorado
            string perfis = null
        )
        {
            try
            {
                var (listaAlocacoesList, listaPerfisList) = await ListarAlocacoesColaboradoresETbdsDynamicAsync(
                    pesquisa,
                    periodoAlocacaoId,
                    orgId,
                    codigoUnidade,
                    codigoDepartamento,
                    codigoGestorAdm,
                    listaCodigoColabOuTbd,
                    filtroTipoProfissional,
                    codigoGestorProjeto,
                    listaCodigoClientes,
                    apenasProjetosPrioritarios,
                    listaCodigoProjetos,
                    codigoStatusProjeto,
                    qtdGerenteProjetoPrioridade,
                    incluirInativos,
                    habilidades,
                    perfis
                );

                var groupedResult = listaAlocacoesList
                    .GroupBy(x => new
                    {
                        x.codigo_interno_colaborador,
                        x.periodo_alocado_id,
                        x.cod_departamento,
                        x.departamento,
                        x.codigo_colaborador,
                        x.nome_completo,
                        x.codigo_projeto,
                        x.projeto,
                        x.cod_cliente,
                        x.cliente,
                        x.status,
                        x.data_inicio,
                        x.data_fim,
                        x.quantidade_horas__ND,
                        x.percentual,
                        x.tbd,
                        x.nome_gestor_adm,
                        x.prioritario,
                        x.oportunidade,
                        x.observacao,
                        x.retroalimenta_cv
                    })
                    .Select(group => new AlocacaoColabETbdDTO
                    {
                        PeriodoAlocadoId = group.Key.periodo_alocado_id,
                        CodDepartamento = group.Key.cod_departamento,
                        NomeDepartamento = group.Key.departamento,
                        CodColaborador = group.Key.codigo_colaborador,
                        NomeColaborador = group.Key.nome_completo,
                        Cpf = group.Key.codigo_interno_colaborador,
                        NomeGestorAdm = group.Key.nome_gestor_adm,
                        Projeto = new AlocacoesColabETbdProjetoDTO
                        {
                            CodProjeto = group.Key.codigo_projeto,
                            NomeProjeto = group.Key.projeto,
                            CodCliente = group.Key.cod_cliente,
                            NomeCliente = group.Key.cliente,
                            LabelClienteProjeto = MapaDeAlocacaoConstants.RetornarLabelCliente(
                                group.Key.cod_cliente, group.Key.cliente, group.Key.codigo_projeto, group.Key.projeto),
                            StatusProjeto = group.Key.status,
                            GestoresProjeto = group
                                .Select(x => new AlocacoesColabETbdGestorDTO
                                {
                                    NomeGestor = x.nome_gerente,
                                    CodGestor = x.cod_gerente,
                                })
                                .DistinctBy(x => x.CodGestor)
                                .ToList()
                        },
                        DataInicio = DateTime.ParseExact(group.Key.data_inicio, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        DataFim = DateTime.ParseExact(group.Key.data_fim, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        QuantidadeDeHoras = group.Key.quantidade_horas__ND,
                        Percentual = group.Key.percentual,
                        Tbd = group.Key.tbd == 1,
                        Prioridade = Convert.ToBoolean(group.Key.prioritario),
                        Oportunidade = group.Key.oportunidade,
                        Observacao = group.Key.observacao,
                        RetroalimentaCv = Convert.ToBoolean(group.Key.retroalimenta_cv),
                        Perfil = listaPerfisList
                            .Where(x => x.tb_colaborador_periodo_alocacao_id == group.Key.periodo_alocado_id)
                            .Select(x => new PerfilAlocacaoDTO
                            {
                                Id = x.id,
                                Perfil = x.perfil
                            })
                            .FirstOrDefault()
                    })
                    .OrderBy(dto => dto.NomeColaborador)
                    .ThenByDescending(dto => dto.DataFim)
                    .ToList();

                return groupedResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Erro ao listar alocações", ex);
            }
        }

        public async Task<IEnumerable<ColaboradorETbdDTO>> ListarColaboradoresETbds(int orgId, List<string>? diretorias, string codigoGestor, string codigoDepartamento, TipoProfissionalEnum filtroTipoProfissional)
        {
            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    try
                    {
                        _connection.Open();

                        var projetos = new List<ProjetoAtividadeDTO>();
                        var inClauseUnidades = diretorias.BuildInClauseOrNull();

                        var sqlSelectCondicional = "";
                        var sqlJoinCondicional = "";
                        var sqlWhereCondicional = "";

                        if (codigoGestor.EhStringValidaEDiferenteDeZero())
                        {
                            sqlSelectCondicional = ",tch_gest_colab.cod_colaborador_superior";
                            sqlJoinCondicional = $@" LEFT JOIN tb_colaborador_hierarquia tch_gest_colab ON vmact.cod_profisisonal = tch_gest_colab.cod_colaborador_externo
																										AND tch_gest_colab.tb_org_id = vmact.tb_org_id AND vmact.eh_tbd = FALSE
                                                     LEFT JOIN tb_colaborador_org tco_gestor_tbd ON tco_gestor_tbd.codigo_interno_colaborador = vmact.codigo_interno_colaborador_gestor_tbd
                                                                                            AND tco_gestor_tbd.tb_org_id = vmact.tb_org_id AND vmact.eh_tbd = TRUE";

                            sqlWhereCondicional = $@" AND (tch_gest_colab.cod_colaborador_superior = @CodigoGestor OR tco_gestor_tbd.cod_colaborador_externo = @CodigoGestor)";
                        }

                        string sql = $@"SELECT
                                        vmact.cod_profisisonal,
                                        vmact.nome_profissional,
                                        vmact.codigo_interno_colaborador_gestor_tbd,
                                        vmact.codigo_diretoria,
                                        vmact.codigo_interno_colaborador,
                                        vmact.eh_tbd,
                                        vmact.tb_org_id
                                        {sqlSelectCondicional}
                                    FROM
                                        vw_mapa_alocacao_colaborador_tbd vmact
                                        {sqlJoinCondicional}
                                    WHERE
                                        vmact.tb_org_id =  @OrgId
                                        AND (vmact.codigo_diretoria <> 'BANCO TALENTOS')
                                        {(inClauseUnidades == null ? "" : $"AND vmact.codigo_diretoria IN {inClauseUnidades}")}
                                        AND (@CodigoDepartamento IS NULL OR vmact.codigo_departamento = @CodigoDepartamento)
                                        AND
                                        (
                                            (@FiltroTipoProfissional = 0)
                                            OR
                                            (@FiltroTipoProfissional = 1 AND vmact.eh_tbd = false)
                                            OR
                                            (@FiltroTipoProfissional = 2 AND vmact.eh_tbd = true)
                                        )
                                        {sqlWhereCondicional}
                                    ORDER BY vmact.nome_profissional;";

                        var result = await _connection.QueryAsync<dynamic>(sql,
                            new
                            {
                                CodigoDepartamento = codigoDepartamento,
                                CodigoGestor = codigoGestor,
                                OrgId = orgId,
                                FiltroTipoProfissional = (int)filtroTipoProfissional
                            }
                        );

                        return result.Select(x => new ColaboradorETbdDTO
                        {
                            CodProfissional = x.cod_profisisonal,
                            NomeProfissional = x.nome_profissional,
                            Cpf = x.codigo_interno_colaborador,
                            LabelCodigoNome = MapaDeAlocacaoConstants.RetornarLabelProfissional(x.cod_profisisonal, x.nome_profissional),
                            EhTbd = Convert.ToBoolean(x.eh_tbd)
                        });
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
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar colaboradores", ex);
            }
        }

        public async Task<List<ListaConsultaDatasDTO>> ConsultaDatasPeriodoAlocacaoColabOuTbd(string codigoProjeto, string codigoColabOuTbd, long? periodoAlocacaoId, bool ehTbd, int orgId)
        {
            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    try
                    {
                        _connection.Open();
                        var parametros = new DynamicParameters();

                        string condicaoWhere = "";

                        if (ehTbd)
                        {
                            condicaoWhere += " AND tcpa.cod_tbd_alocado = @CodigoColabOuTbd";
                        }
                        else
                        {
                            condicaoWhere += " AND tcpa.codigo_colaborador = @CodigoColabOuTbd";
                        }

                        if (!string.IsNullOrEmpty(codigoProjeto))
                        {
                            condicaoWhere += " AND tcpa.codigo_projeto = @CodigoProjeto";
                            parametros.Add("@CodigoProjeto", codigoProjeto);
                        }

                        if (periodoAlocacaoId.HasValue && periodoAlocacaoId > 0)
                        {
                            condicaoWhere += " AND tcpa.id <> @PeriodoAlocacaoId";
                            parametros.Add("@PeriodoAlocacaoId", periodoAlocacaoId);
                        }

                        var sql = $@"SELECT
                                        id,
                                        data_inicio,
                                        data_fim,
                                        quantidade_horas,
                                        inclui_fimdesemana
                                     FROM
                                        tb_colaborador_periodo_alocacao tcpa
                                     WHERE
                                        tcpa.tb_org_id = @OrgId AND tcpa.ativo = 1
                                        {condicaoWhere};";

                        parametros.Add("@OrgId", orgId);
                        parametros.Add("@CodigoColabOuTbd", codigoColabOuTbd);

                        var result = await _connection.QueryAsync<dynamic>(sql, parametros);

                        var ret = result.Select(x => new ListaConsultaDatasDTO
                        {
                            DataFimPeriodo = x.data_fim,
                            DataInicioPeriodo = x.data_inicio,
                            PeriodoId = x.id,
                            IncluiFinalDeSemana = x.inclui_fimdesemana == 1,
                            QuantidadeHoras = x.quantidade_horas,
                        }).ToList();

                        return ret;
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
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar alocacoes do colaborador", ex);
            }
        }

        public async Task RemoverCalculosMensaisAsync(string codigoColaborador, int? codTbdAlocado, int orgId)
        {
            if ((codigoColaborador.HasValue() && codTbdAlocado.HasValue())
                || (codigoColaborador.IsEmpty() && codTbdAlocado.IsEmpty()))
            {
                throw new ArgumentException("Código de colaborador ou código de TBD deve ser fornecido.");
            }

            var condicaoWhere = "";

            var parametros = new DynamicParameters();
            parametros.Add("@OrgId", orgId);

            if (codigoColaborador.HasValue())
            {
                parametros.Add("@CodigoColaborador", codigoColaborador);
                condicaoWhere = " AND codigo_colaborador = @CodigoColaborador";
            }
            else if (codTbdAlocado.HasValue())
            {
                parametros.Add("@CodTbdAlocado", codTbdAlocado);
                condicaoWhere = " AND cod_tbd_alocado = @CodTbdAlocado";
            }

            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    _connection.Open();

                    var deleteQuery = @$" DELETE FROM
                                            tb_colaborador_periodo_alocacao_calculo_mensal
                                          WHERE
                                            tb_org_id = @OrgId
                                            {condicaoWhere}";

                    await _connection.ExecuteAsync(deleteQuery, parametros);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir cálculos mensais", ex);
            }
        }
        public async Task PersistirCalculosMensaisAsync(List<CalculoMensalDTO> calculosMensais)
        {
            if (calculosMensais.Count > 0)
            {
                const string insertQuery = @"
                INSERT INTO
                    tb_colaborador_periodo_alocacao_calculo_mensal
                    (codigo_colaborador, cod_tbd_alocado, mes, ano, horas, status_colaborador_periodo_alocacao, tb_org_id)
                VALUES
                    (@CodigoColaborador, @CodTbdAlocado, @Mes, @Ano, @Horas, @StatusColaboradorPeriodoAlocacao, @OrgId);";

                try
                {
                    using (var _connection = _connectionString.CreateMySqlConnection())
                    {
                        _connection.Open();
                        using (var transaction = _connection.BeginTransaction())
                        {
                            try
                            {
                                foreach (var calculoMensal in calculosMensais)
                                {
                                    await _connection.ExecuteAsync(insertQuery, calculoMensal, transaction);
                                }

                                transaction.Commit();
                            }
                            catch
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao persistir Cálculos Mensais", ex);
                }
            }
        }

        public async Task<List<RecursoMapaDTO>> FiltroMapaAlocacao(GetMapaAlocacaoInputDTO mapaAlocacaoInputDTO, int orgId)
        {
            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    string sql = @"CALL spr_get_alocacao_calculo_mensal_filtro(@mes_inicial,
                                                                               @ano_inicial,
                                                                               @mes_final,
                                                                               @ano_final,
                                                                               @eh_tbd,
                                                                               @status,
                                                                               @idioma,
                                                                               @hard_skill,
                                                                               @colaborador_ou_tbd,
                                                                               @gestor,
                                                                               @cod_diretoria,
                                                                               @nome_diretoria,
                                                                               @cursor_int,
                                                                               @limite_int,
                                                                               @org_id)";

                    var parameters = new DynamicParameters();
                    parameters.Add("mes_inicial", mapaAlocacaoInputDTO.MesInicial);
                    parameters.Add("ano_inicial", mapaAlocacaoInputDTO.AnoInicial);
                    parameters.Add("mes_final", mapaAlocacaoInputDTO.MesFinal);
                    parameters.Add("ano_final", mapaAlocacaoInputDTO.AnoFinal);
                    parameters.Add("eh_tbd", mapaAlocacaoInputDTO.EhTbd);
                    parameters.Add("status", mapaAlocacaoInputDTO.StatusHorasFiltroEnum.ToString());
                    parameters.Add("idioma", mapaAlocacaoInputDTO.Idioma);
                    parameters.Add("hard_skill", mapaAlocacaoInputDTO.HardSkill);
                    parameters.Add("gestor", mapaAlocacaoInputDTO.GestorFiltro);
                    parameters.Add("colaborador_ou_tbd", mapaAlocacaoInputDTO.ColaboradorOuTbdFiltro);
                    parameters.Add("cod_diretoria", mapaAlocacaoInputDTO.CodigoDiretoria);
                    parameters.Add("nome_diretoria", mapaAlocacaoInputDTO.NomeDiretoria);
                    parameters.Add("cursor_int", mapaAlocacaoInputDTO.Cursor);
                    parameters.Add("limite_int", mapaAlocacaoInputDTO.Limite);
                    parameters.Add("org_id", orgId);

                    var result = await _connection.QueryAsync<FiltroMapaAlocacaoProcedureResult>(sql, parameters);

                    var groupedResult = result
                        .GroupBy(r => new
                        {
                            r.codigo_colaborador,
                            r.eh_tbd,
                            r.codigo_interno_colaborador,
                            r.nome,
                            r.cod_diretoria,
                            r.diretoria,
                            r.idiomas,
                            r.hard_skills,
                            r.gestores,
                            r.gestores_nome
                        })
                        .Select(g => new RecursoMapaDTO
                        {
                            Nome = g.Key.nome,
                            CpfColaborador = g.Key.codigo_interno_colaborador,
                            CodigoColaborador = g.FirstOrDefault()?.eh_tbd == 0 ? g.Key.codigo_colaborador : null,
                            CodigoTbd = g.FirstOrDefault()?.eh_tbd == 1 ? int.Parse(g.Key.codigo_colaborador) : null,
                            HardSkills = g.Key.hard_skills,
                            EhTbd = g.Key.eh_tbd == 1,
                            Idioma = g.Key.idiomas,
                            Diretoria = g.Key.diretoria,
                            CodigoDiretoria = g.Key.cod_diretoria,
                            Alocacao = g.Select(a => new AlocacaoDTO
                            {
                                Mes = $"{MapaUtil.ObterNomeDoMes(a.mes)}/{a.ano}",
                                Horas = a.horas.ToStringHoraFormatada(),
                                StatusHorasRecursoFiltro = (StatusHorasEnum)Enum.Parse(typeof(StatusHorasEnum), a.status_colaborador_periodo_alocacao),
                                MesAlocacao = a.mes,
                                AnoAlocacao = a.ano
                            }).ToList()
                        }).ToList();

                    return groupedResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao executar FiltroMapaAlocacao.", ex);
            }
        }

        public async Task<int> ObterQuantidadeAlocacoesColaboradorNumProjeto(string cpfColaborador, string codProjeto, int orgId)
        {
            try
            {
                using (var _connection = _connectionString.CreateMySqlConnection())
                {
                    string query = @" SELECT
                                        COUNT(*) as qtd
                                      FROM
                                        tb_colaborador_periodo_alocacao
                                      WHERE
                                        ativo = 1
                                        AND codigo_interno_colaborador = @Cpf
                                        AND tb_org_id = @OrgId
                                        AND codigo_projeto = @CodProjeto;";

                    var parameters = new { OrgId = orgId, CodProjeto = codProjeto, Cpf = cpfColaborador };

                    var cont = await _connection.QueryAsync<int>(query, parameters);

                    return cont.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao executar GetFeriadosPorOrgId.", ex);
            }
        }

        public async Task<List<RelatorioAprovadoresDTO>> ListarProjetosEAprovadores(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT 
                    tpo.cod_projeto AS ProjetoId,
                    tpo.projeto AS NomeProjeto,
                    tpo.cod_cliente AS ClienteId,
                    tco.nome_cliente AS NomeCliente,
                    tc.nome_completo AS NomeColaborador
                FROM tb_projeto_org tpo
                LEFT JOIN tb_cliente_org tco
                    ON tco.codigo_cliente = tpo.cod_cliente
                    AND tco.tb_org_id = tpo.tb_org_id
                LEFT JOIN tb_projeto_gerente tpg
                    ON tpg.cod_projeto = tpo.cod_projeto
                LEFT JOIN tb_colaborador_org tco2
                    ON tco2.cod_colaborador_externo = tpg.cod_colaborador_gerente
                    AND tco2.tb_org_id = tpo.tb_org_id
                LEFT JOIN tb_colaborador tc 
                    ON tc.codigo_interno_colaborador = tco2.codigo_interno_colaborador
                WHERE tpo.tb_org_id = @OrgId
                AND DATE(tpo.data_fim) >= CURDATE();
            ";

            var projetoDict = new Dictionary<string, RelatorioAprovadoresDTO>();

            await connection.QueryAsync<RelatorioAprovadoresDTO, string, RelatorioAprovadoresDTO>(
                query,
                (projeto, aprovador) =>
                {
                    if (!projetoDict.TryGetValue(projeto.ProjetoId, out var projetoEntry))
                    {
                        projetoEntry = projeto;
                        projetoDict[projeto.ProjetoId] = projetoEntry;
                    }

                    if (!string.IsNullOrEmpty(aprovador) && 
                        !projetoEntry.Aprovadores.Contains(aprovador))
                    {
                        projetoEntry.Aprovadores.Add(aprovador);
                    }

                    return projetoEntry;
                },
                param: new { OrgId = orgId },
                splitOn: "NomeColaborador"
            );

            // Agora converte para o DTO final
            return projetoDict.Values.Select(p => new RelatorioAprovadoresDTO
            {
                ProjetoId = p.ProjetoId,
                NomeProjeto = p.NomeProjeto,
                ClienteId = p.ClienteId,
                NomeCliente = p.NomeCliente,
                Aprovadores = p.Aprovadores
            }).ToList();
        }
    }
}
using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain.Notificacao;
using Dapper;
using DataTransferObject.Domain.Notificacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Notificacao
{
    public class NotificacaoRepository : INotificacaoRepository
    {
        private IConnectionStringCore _connectionString;

        public NotificacaoRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<int> ContarNotificacoesNaoLidasColaborador(string cpf, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = $@"SELECT COUNT(*)
                                    FROM
                                        tb_notificacao
                                    WHERE
                                        codigo_interno_colaborador = @Cpf
                                    AND
                                        tb_org_id = @OrgId
                                    AND
                                        lida = false;";

                    return await _connection.QueryFirstAsync<int>(sql,
                        new
                        {
                            Cpf = cpf,
                            OrgId = orgId
                        }
                    );
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
        public async Task<NotificacaoDTO> InserirNotificacaoColaborador(NotificacaoDTO notificacaoDTO)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sqlInsert = @"
                                        INSERT INTO tb_notificacao (
                                            id,
                                            codigo_interno_colaborador,
                                            titulo,
                                            mensagem,
                                            mensagem_html,
                                            lida,
                                            data_envio,
                                            data_leitura,
                                            tb_funcionalidade_sistema_id,
                                            url_customizada,
                                            tb_org_id
                                        ) VALUES (
                                            @Id,
                                            @ColaboradorCpf,
                                            @Titulo,
                                            @Mensagem,
                                            @MensagemHtml,
                                            @Lida,
                                            @DataEnvio,
                                            @DataLeitura,
                                            @TbFuncionalidadeSistemaId,
                                            @UrlCustomizada,
                                            @OrgId
                                        );";

                    notificacaoDTO.Id = Guid.NewGuid().ToString();

                    var parameters = new
                    {
                        notificacaoDTO.Id,
                        notificacaoDTO.ColaboradorCpf,
                        notificacaoDTO.Titulo,
                        notificacaoDTO.Mensagem,
                        notificacaoDTO.MensagemHtml,
                        notificacaoDTO.Lida,
                        notificacaoDTO.DataEnvio,
                        notificacaoDTO.DataLeitura,
                        notificacaoDTO.TbFuncionalidadeSistemaId,
                        UrlCustomizada = notificacaoDTO.UrlCustomizada,
                        notificacaoDTO.OrgId
                    };

                    await _connection.ExecuteAsync(sqlInsert, parameters);

                    string sqlSelect = @"
                                        SELECT
                                            tn.*,
                                            tfr.rota AS rota_funcionalidade
                                        FROM
                                            tb_notificacao tn
                                        LEFT JOIN
                                            tb_funcionalidade_rota tfr
                                            ON
                                                tn.tb_funcionalidade_sistema_id = tfr.tb_funcionalidade_sistema_id
                                                AND tfr.tb_org_id = tn.tb_org_id
                                        WHERE
                                            tn.id = @Id;";

                    var result = await _connection.QueryAsync<dynamic>(sqlSelect, new { notificacaoDTO.Id });

                    return result.Select(x => MapRowToDto(x)).FirstOrDefault();
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

        public async Task<List<NotificacaoDTO>> ListarNotificacoesColaborador(string cpf, int orgId, bool apenasNaoLidas)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    var notificacoes = new List<NotificacaoDTO>();

                    string sql = $@"SELECT tn.*, tfr.rota AS rota_funcionalidade
                                    FROM
                                        tb_notificacao tn
                                    LEFT JOIN
                                        tb_funcionalidade_rota tfr
                                        ON tn.tb_funcionalidade_sistema_id = tfr.tb_funcionalidade_sistema_id
                                        AND tfr.tb_org_id = tn.tb_org_id
                                    WHERE
                                        tn.codigo_interno_colaborador = @Cpf
                                    AND
                                        tn.tb_org_id = @OrgId";

                    if (apenasNaoLidas)
                    {
                        sql += " AND tn.lida = false";
                    }

                    sql += @" ORDER BY
                                tn.data_envio DESC";

                    var result = await _connection.QueryAsync<dynamic>(sql,
                        new
                        {
                            Cpf = cpf,
                            OrgId = orgId
                        }
                    );

                    var resultListDB = result.ToList();
                    if (resultListDB != null && resultListDB.Any())
                    {
                        foreach (var row in resultListDB)
                            notificacoes.Add(MapRowToDto(row));
                    }

                    return notificacoes;
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

        /// <summary>
        /// url_customizada tem precedência: URL absoluta ou path relativo (composto com URL_BASE como em tb_funcionalidade_rota).
        /// </summary>
        private static NotificacaoDTO MapRowToDto(dynamic x)
        {
            string urlCustomizada = x.url_customizada;
            string rotaFuncionalidade = x.rota_funcionalidade;
            var (rota, rotaCompleta) = BuildRotaExibicao(urlCustomizada, rotaFuncionalidade);

            return new NotificacaoDTO
            {
                Id = x.id.ToString(),
                ColaboradorCpf = x.codigo_interno_colaborador,
                Titulo = x.titulo,
                Mensagem = x.mensagem,
                MensagemHtml = x.mensagem_html,
                Lida = x.lida,
                DataEnvio = ParseDataEnvio(x.data_envio),
                DataLeitura = x.data_leitura != null ? (DateTime?)ParseDataEnvio(x.data_leitura) : null,
                TbFuncionalidadeSistemaId = x.tb_funcionalidade_sistema_id,
                OrgId = x.tb_org_id,
                UrlCustomizada = urlCustomizada,
                Rota = rota,
                RotaCompleta = rotaCompleta
            };
        }

        private static DateTime ParseDataEnvio(object dataEnvio)
        {
            if (dataEnvio is DateTime dt)
                return dt;
            if (dataEnvio is DateTimeOffset dto)
                return dto.DateTime;
            return DateTime.Parse(dataEnvio.ToString());
        }

        private static (string Rota, string RotaCompleta) BuildRotaExibicao(string urlCustomizada, string rotaFuncionalidade)
        {
            var urlBase = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.URL_BASE);

            var custom = string.IsNullOrWhiteSpace(urlCustomizada) ? null : urlCustomizada.Trim();
            if (!string.IsNullOrEmpty(custom))
            {
                if (custom.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    custom.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var uri = new Uri(custom);
                        var path = uri.PathAndQuery.TrimStart('/');
                        return (string.IsNullOrEmpty(path) ? null : path, custom);
                    }
                    catch (UriFormatException)
                    {
                        return (null, custom);
                    }
                }

                var rel = custom.TrimStart('/');
                var completaRel = urlBase + "/" + rel;
                return (rel, completaRel);
            }

            var func = string.IsNullOrWhiteSpace(rotaFuncionalidade) ? null : rotaFuncionalidade.Trim();
            if (string.IsNullOrEmpty(func))
                return (null, null);

            var rotaCompletaFunc = urlBase + "/" + func;
            return (func, rotaCompletaFunc);
        }

        public async Task<bool> MarcarNotificacoesComoLidasColaborador(string cpf, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = $@"UPDATE
                                        tb_notificacao
                                    SET
                                        lida = true
                                    WHERE
                                        codigo_interno_colaborador = @Cpf
                                    AND
                                        tb_org_id = @OrgId
                                    AND
                                        lida = false;";

                    var result = await _connection.ExecuteAsync(sql,
                        new
                        {
                            Cpf = cpf,
                            OrgId = orgId
                        }
                    );

                    return result > 0;
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

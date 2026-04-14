using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using ApiClient.Domain;
using Core.Domain.Marketing.Comunicacao.Profissionais;
using Dapper;
using DataTransferObject.Domain.Marketing.Comunicacao.Profissionais;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Marketing.Comunicacao.Profissionais
{
    public class ComunicacaoProfissionaisRepository : IComunicacaoProfissionaisRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ComunicacaoProfissionaisRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<List<ProfissionalDTO>> ObterListaProfissionaisAsync(int orgId, string codigoColaboradorUsuarioLogado)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    tc.codigo_interno_colaborador AS CodigoColaboradorInterno,
                    tu.email AS Email,
                    tc.nome_completo AS NomeCompleto,
                    tco.codigo_cargo AS CodCargo,
                    tco.cargo AS Cargo,
                    tco.cod_departamento AS CodDepartamento,
                    tco.departamento AS Departamento,
                    tco.cod_diretoria AS CodDiretoria,
                    tco.diretoria AS Diretoria,
                    tc.contato_principal AS Telefone,
                    tcs.descricao AS Sobre,
                    DATE_FORMAT(tc.data_nascimento, '%d/%m') AS DataAniversario,
                    tc_sup.nome_completo AS Supervisor,
                    ti.path AS ImagemPath
                FROM
                	tb_colaborador tc
                JOIN
                	tb_colaborador_org tco ON tc.codigo_interno_colaborador  = tco.codigo_interno_colaborador AND tco.tb_org_id = @OrgId
                LEFT JOIN
                	tb_imagem ti ON tc.imagem_id = ti.id
                LEFT JOIN
                	tb_colaborador_sobre tcs on tco.codigo_interno_colaborador = tcs.codigo_interno_colaborador
                LEFT JOIN
                	tb_usuario tu ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador AND tu.tb_org_id = @OrgId
                LEFT JOIN
                	tb_colaborador_hierarquia tch ON tco.cod_colaborador_externo = tch.cod_colaborador_externo AND tco.tb_org_id = tch.tb_org_id
                LEFT JOIN
                	tb_colaborador_org tco_sup ON tch.cod_colaborador_superior = tco_sup.cod_colaborador_externo AND tch.tb_org_id = tco_sup.tb_org_id
                LEFT JOIN
                	tb_colaborador tc_sup ON tco_sup.codigo_interno_colaborador = tc_sup.codigo_interno_colaborador
                WHERE
                	tco.tb_org_id = @OrgId AND tco.ativo = 1 AND tc.ativo = 1 AND tco.cod_diretoria <> 'BANCO TALENTOS'
                ORDER BY
                	tc.nome_completo";

            var rows = (await connection.QueryAsync<(string CodigoColaboradorInterno, string Email, string NomeCompleto, string CodCargo, string Cargo, string CodDepartamento, string Departamento, string CodDiretoria, string Diretoria, string Telefone, string Sobre, string DataAniversario, string Supervisor, string ImagemPath)>(
                sql,
                new { OrgId = orgId }
            )).ToList();

            var profissionais = rows.Select(r => new ProfissionalDTO
            {
                CodigoColaboradorInterno = r.CodigoColaboradorInterno,
                Email = r.Email,
                NomeCompleto = r.NomeCompleto,
                CodCargo = r.CodCargo,
                Cargo = r.Cargo,
                CodDepartamento = r.CodDepartamento,
                Departamento = r.Departamento,
                CodDiretoria = r.CodDiretoria,
                Diretoria = r.Diretoria,
                Telefone = r.Telefone,
                Sobre = r.Sobre,
                DataAniversario = r.DataAniversario,
                Supervisor = r.Supervisor,
                UrlFoto = !string.IsNullOrWhiteSpace(r.ImagemPath)
                    ? VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + r.ImagemPath
                    : null
            }).ToList();

            if (!string.IsNullOrWhiteSpace(codigoColaboradorUsuarioLogado))
            {
                var codigosFavoritados = await ObterCodigosFavoritadosPorUsuarioAsync(connection, codigoColaboradorUsuarioLogado, orgId);
                foreach (var p in profissionais)
                    p.Favoritado = codigosFavoritados.Contains(p.CodigoColaboradorInterno);
                // Favoritados primeiro, depois ordem por nome
                return profissionais.OrderByDescending(p => p.Favoritado).ThenBy(p => p.NomeCompleto).ToList();
            }

            return profissionais;
        }

        public async Task<List<ProfissionalDTO>> ObterProfissionaisPorCodigosAsync(int orgId, IReadOnlyList<string> codigos, string codigoColaboradorUsuarioLogado)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<ProfissionalDTO>();

            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT
                    tc.codigo_interno_colaborador AS CodigoColaboradorInterno,
                    tu.email AS Email,
                    tc.nome_completo AS NomeCompleto,
                    tco.codigo_cargo AS CodCargo,
                    tco.cargo AS Cargo,
                    tco.cod_departamento AS CodDepartamento,
                    tco.departamento AS Departamento,
                    tco.cod_diretoria AS CodDiretoria,
                    tco.diretoria AS Diretoria,
                    tc.contato_principal AS Telefone,
                    tcs.descricao AS Sobre,
                    DATE_FORMAT(tc.data_nascimento, '%d/%m') AS DataAniversario,
                    tc_sup.nome_completo AS Supervisor,
                    ti.path AS ImagemPath
                FROM
                	tb_colaborador tc
                JOIN
                	tb_colaborador_org tco ON tc.codigo_interno_colaborador  = tco.codigo_interno_colaborador AND tco.tb_org_id = @OrgId
                LEFT JOIN
                	tb_imagem ti ON tc.imagem_id = ti.id
                LEFT JOIN
                	tb_colaborador_sobre tcs on tco.codigo_interno_colaborador = tcs.codigo_interno_colaborador
                LEFT JOIN
                	tb_usuario tu ON tc.codigo_interno_colaborador = tu.codigo_interno_colaborador AND tu.tb_org_id = @OrgId
                LEFT JOIN
                	tb_colaborador_hierarquia tch ON tco.cod_colaborador_externo = tch.cod_colaborador_externo AND tco.tb_org_id = tch.tb_org_id
                LEFT JOIN
                	tb_colaborador_org tco_sup ON tch.cod_colaborador_superior = tco_sup.cod_colaborador_externo AND tch.tb_org_id = tco_sup.tb_org_id
                LEFT JOIN
                	tb_colaborador tc_sup ON tco_sup.codigo_interno_colaborador = tc_sup.codigo_interno_colaborador
                WHERE
                	tco.tb_org_id = @OrgId AND tco.ativo = 1 AND tc.ativo = 1 AND tco.cod_diretoria <> 'BANCO TALENTOS'
                    AND tc.codigo_interno_colaborador IN @Codigos
                ORDER BY
                	tc.nome_completo";

            var rows = (await connection.QueryAsync<(string CodigoColaboradorInterno, string Email, string NomeCompleto, string CodCargo, string Cargo, string CodDepartamento, string Departamento, string CodDiretoria, string Diretoria, string Telefone, string Sobre, string DataAniversario, string Supervisor, string ImagemPath)>(
                sql,
                new { OrgId = orgId, Codigos = codigos }
            )).ToList();

            var profissionais = rows.Select(r => new ProfissionalDTO
            {
                CodigoColaboradorInterno = r.CodigoColaboradorInterno,
                Email = r.Email,
                NomeCompleto = r.NomeCompleto,
                CodCargo = r.CodCargo,
                Cargo = r.Cargo,
                CodDepartamento = r.CodDepartamento,
                Departamento = r.Departamento,
                CodDiretoria = r.CodDiretoria,
                Diretoria = r.Diretoria,
                Telefone = r.Telefone,
                Sobre = r.Sobre,
                DataAniversario = r.DataAniversario,
                Supervisor = r.Supervisor,
                UrlFoto = !string.IsNullOrWhiteSpace(r.ImagemPath)
                    ? VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + r.ImagemPath
                    : null
            }).ToList();

            if (!string.IsNullOrWhiteSpace(codigoColaboradorUsuarioLogado))
            {
                var codigosFavoritados = await ObterCodigosFavoritadosPorUsuarioAsync(connection, codigoColaboradorUsuarioLogado, orgId);
                foreach (var p in profissionais)
                    p.Favoritado = codigosFavoritados.Contains(p.CodigoColaboradorInterno);
            }

            return profissionais;
        }

        private async Task<HashSet<string>> ObterCodigosFavoritadosPorUsuarioAsync(System.Data.IDbConnection connection, string codigoColaboradorQuemFavoritou, int orgId)
        {
            var sql = @"
                SELECT codigo_interno_colaborador_favoritado
                FROM tb_mkt_profissional_favorito
                WHERE codigo_interno_colaborador = @CodigoQuemFavoritou AND tb_org_id = @OrgId";
            var list = (await connection.QueryAsync<string>(sql, new { CodigoQuemFavoritou = codigoColaboradorQuemFavoritou, OrgId = orgId })).ToList();
            return new HashSet<string>(list);
        }

        public async Task FavoritarProfissionalAsync(string codigoColaboradorQuemFavoritou, string codigoColaboradorFavoritado, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var existsSql = @"
                SELECT 1 FROM tb_mkt_profissional_favorito
                WHERE codigo_interno_colaborador = @CodigoQuem AND codigo_interno_colaborador_favoritado = @CodigoFavoritado AND tb_org_id = @OrgId LIMIT 1";
            var exists = await connection.ExecuteScalarAsync<int?>(existsSql, new { CodigoQuem = codigoColaboradorQuemFavoritou, CodigoFavoritado = codigoColaboradorFavoritado, OrgId = orgId });
            if (exists.HasValue && exists.Value == 1)
                throw new System.Exception("O profissional já está favoritado.");

            var sql = @"
                INSERT INTO tb_mkt_profissional_favorito (codigo_interno_colaborador, codigo_interno_colaborador_favoritado, tb_org_id)
                VALUES (@CodigoQuem, @CodigoFavoritado, @OrgId)";
            await connection.ExecuteAsync(sql, new { CodigoQuem = codigoColaboradorQuemFavoritou, CodigoFavoritado = codigoColaboradorFavoritado, OrgId = orgId });
        }

        public async Task DesfavoritarProfissionalAsync(string codigoColaboradorQuemFavoritou, string codigoColaboradorFavoritado, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                DELETE FROM tb_mkt_profissional_favorito
                WHERE codigo_interno_colaborador = @CodigoQuem AND codigo_interno_colaborador_favoritado = @CodigoFavoritado AND tb_org_id = @OrgId";
            var rows = await connection.ExecuteAsync(sql, new { CodigoQuem = codigoColaboradorQuemFavoritou, CodigoFavoritado = codigoColaboradorFavoritado, OrgId = orgId });
            if (rows == 0)
                throw new System.Exception("O profissional não está favoritado.");
        }

        public async Task<List<string>> ListarCodigosProfissionaisFavoritadosAsync(string codigoColaboradorQuemFavoritou, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var sql = @"
                SELECT codigo_interno_colaborador_favoritado
                FROM tb_mkt_profissional_favorito
                WHERE codigo_interno_colaborador = @CodigoQuemFavoritou AND tb_org_id = @OrgId";
            return (await connection.QueryAsync<string>(sql, new { CodigoQuemFavoritou = codigoColaboradorQuemFavoritou, OrgId = orgId })).ToList();
        }
    }
}

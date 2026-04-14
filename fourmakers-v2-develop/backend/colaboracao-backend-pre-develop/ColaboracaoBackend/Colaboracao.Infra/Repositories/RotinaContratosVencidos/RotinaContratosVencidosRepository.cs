using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.Domain.RotinaContratosVencidos;
using Dapper;
using DataTransferObject.Domain.RotinaNotificacaoContratosVencidos;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Util.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.RotinaContratosVencidos
{
    public class RotinaContratosVencidosRepository : IRotinaContratosVencidosRepository
    {
        private readonly IDBConnection _dapperConnection;

        public RotinaContratosVencidosRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<NotificacaoContratosVencidos> BuscarEmailContratosNotificacao()
        {
            var connection = _dapperConnection.GetConnection();
            NotificacaoContratosVencidos notificacaoContratosVencidos = new();

            var rowsAVencer = await connection.QueryAsync<ContratoNotificacaoEmailRow>(
                @"
                    SELECT
                        tncvd.email AS Email,
                        t1.id AS ContratoId,
                        COALESCE(p.nome_parceiro, '') AS NomeEmpresa,
                        COALESCE(NULLIF(TRIM(t1.cd_contrato), ''), '') AS NomeContrato,
                        t1.fim_contrato AS FimContrato
                    FROM tb_notificacao_contratos_vencidos_destinatarios tncvd
                    JOIN tb_parceiros_gestao_contratos t1
                        ON t1.id = tncvd.tb_parceiros_gestao_contratos_id
                    JOIN tb_parceiros p ON p.id = t1.tb_parceiros_id
                    WHERE t1.fim_contrato >= CURDATE()
                      AND t1.fim_contrato < DATE_ADD(CURDATE(), INTERVAL 151 DAY)

                    UNION ALL

                    SELECT
                        tnp.email AS Email,
                        tpgc.id AS ContratoId,
                        COALESCE(p.nome_parceiro, '') AS NomeEmpresa,
                        COALESCE(NULLIF(TRIM(tpgc.cd_contrato), ''), '') AS NomeContrato,
                        tpgc.fim_contrato AS FimContrato
                    FROM tb_notificacao_contratos_vencidos_destinatarios_padrao tnp
                    JOIN tb_parceiros_gestao_contratos tpgc
                        ON tpgc.fim_contrato >= CURDATE()
                       AND tpgc.fim_contrato < DATE_ADD(CURDATE(), INTERVAL 151 DAY)
                    JOIN tb_parceiros p ON p.id = tpgc.tb_parceiros_id;
                "
            );

            notificacaoContratosVencidos.ContratosAVencer.AddRange(AgruparPorEmail(rowsAVencer));

            var rowsVencidos = await connection.QueryAsync<ContratoNotificacaoEmailRow>(
                @"
                    SELECT
                        tncvd.email AS Email,
                        t1.id AS ContratoId,
                        COALESCE(p.nome_parceiro, '') AS NomeEmpresa,
                        COALESCE(NULLIF(TRIM(t1.cd_contrato), ''), '') AS NomeContrato,
                        t1.fim_contrato AS FimContrato
                    FROM tb_notificacao_contratos_vencidos_destinatarios tncvd
                    JOIN tb_parceiros_gestao_contratos t1
                        ON t1.id = tncvd.tb_parceiros_gestao_contratos_id
                    JOIN tb_parceiros p ON p.id = t1.tb_parceiros_id
                    WHERE t1.fim_contrato < CURDATE()

                    UNION ALL

                    SELECT
                        tnp.email AS Email,
                        tpgc.id AS ContratoId,
                        COALESCE(p.nome_parceiro, '') AS NomeEmpresa,
                        COALESCE(NULLIF(TRIM(tpgc.cd_contrato), ''), '') AS NomeContrato,
                        tpgc.fim_contrato AS FimContrato
                    FROM tb_notificacao_contratos_vencidos_destinatarios_padrao tnp
                    JOIN tb_parceiros_gestao_contratos tpgc
                        ON tpgc.fim_contrato < CURDATE()
                    JOIN tb_parceiros p ON p.id = tpgc.tb_parceiros_id;
                "
            );

            notificacaoContratosVencidos.ContratosVencidos.AddRange(AgruparPorEmail(rowsVencidos));

            return notificacaoContratosVencidos;
        }

        private static List<ContratosPorEmail> AgruparPorEmail(IEnumerable<ContratoNotificacaoEmailRow> rows)
        {
            return rows
                .Where(r => !string.IsNullOrWhiteSpace(r.Email))
                .GroupBy(r => r.Email.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => new ContratosPorEmail
                {
                    Email = g.Key,
                    Contratos = g
                        .GroupBy(r => r.ContratoId)
                        .Select(gg => gg.First())
                        .Select(r => new ContratoNotificacaoDetalhe
                        {
                            NomeEmpresa = r.NomeEmpresa ?? string.Empty,
                            NomeContrato = r.NomeContrato ?? string.Empty,
                            FimContrato = r.FimContrato
                        })
                        .OrderBy(c => c.FimContrato ?? DateTime.MaxValue)
                        .ToList()
                })
                .Where(x => x.Contratos.Count > 0)
                .ToList();
        }

        public async Task<UsuarioColaboradorDTO> BuscarDadosColaboradorPorEmail(string email)
        {
            var connection = _dapperConnection.GetConnection();

            var usuario = await connection.QuerySingleOrDefaultAsync<UsuarioColaboradorDTO>(
                $@" SELECT  
                            tc.codigo_interno_colaborador AS cpf,
                            tu.email, 
                            tc.nome_completo AS nomeColaborador,
                            tu.tb_org_id AS orgId
                    FROM tb_usuario tu
                    INNER JOIN tb_colaborador tc 
	                    ON tc.codigo_interno_colaborador =  tu.codigo_interno_colaborador 
                    WHERE tu.tb_org_id = @OrgId
                          AND tu.email = @email;", new { email, OrgId = EnumORG.FOURSYS_2 });

            return usuario;
        }
    }
}

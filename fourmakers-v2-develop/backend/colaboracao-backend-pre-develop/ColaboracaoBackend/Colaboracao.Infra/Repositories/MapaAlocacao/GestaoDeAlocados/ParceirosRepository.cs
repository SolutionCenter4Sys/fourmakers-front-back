using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.MapaAlocacao.GestaoDeAlocados
{

    public class ParceirosRepository : IParceirosRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ParceirosRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        // ───────────────────────────────────────────────
        // Parceiro CRUD
        // ───────────────────────────────────────────────

        public async Task<ParceiroDTO> BuscarParceiroPorId(string parceiroID)
        {
            var connection = _dapperConnection.GetConnection();
            const string q = @"
                        SELECT
                            tp.id as ID,
                            tp.tb_colaborador_codigo_interno_colaborador as CodigoInternoColaborador,
                            tc.nome_completo as NomeColaborador,
                            tp.nome_parceiro as NomeParceiro,
                            tp.descricao_curta as DescricaoCurta,
                            tp.descricao_longa as DescricaoLonga,
                            tp.tipo_parceria as TipoParceria,
                            tp.avaliacao as Avaliacao,
                            tp.url_site as UrlSite,
                            tp.url_linkedin as UrlLinkedin,
                            tp.url_logo as UrlLogo,
                            tp.url_nda as UrlNda,
                            tp.url_contrato as UrlContrato,
                            tp.url_aditivos as UrlAditivos,
                            tp.mais_info as MaisInfo,
                            tp.data_cadastro as DataCadastro,
                            tp.data_atualizacao as DataAtualizacao,
                            tp.tb_colaborador_codigo_colaborador_ultima_alteracao as CodigoColaboradorUltimaAtualizacao,
                            tp.tb_org_id as OrgId,
                            tp.unidade as Unidade
                        FROM 
                            tb_parceiros tp
                        LEFT JOIN
                            tb_colaborador tc
                                ON tc.codigo_interno_colaborador = tp.tb_colaborador_codigo_interno_colaborador
                        WHERE 
                            tp.id = @ParceiroID;";
            var parceiroTab = await connection.QueryFirstOrDefaultAsync<ParceiroDTO>(q, new { ParceiroID = parceiroID });

            if (parceiroTab == null)
                return null;

            const string qContato = @"
                        SELECT
                            id as ID,
                            tb_parceiros_id as ParceiroID,
                            nome as Nome,
                            telefone as Telefone,
                            email as Email
                        FROM 
                            tb_parcerios_contatos 
                        WHERE 
                            tb_parceiros_id = @ParceiroID;";

            var contatos = (await connection.QueryAsync<ParceiroContatoDTO>(
                    qContato, new { ParceiroID = parceiroID }))
                .AsList();

            if (contatos.Count > 0)
                parceiroTab.ParceirosContato = contatos;


            const string qCategoria = @"
                        SELECT
                            id as ID,
                            tb_parceiros_id as ParceiroID,
                            categoria as Categoria
                        FROM 
                            tb_parcerios_categorias 
                        WHERE 
                            tb_parceiros_id = @ParceiroID;";

            var categorias = (await connection.QueryAsync<ParceiroCategoriaDTO>(
                    qCategoria, new { ParceiroID = parceiroID }))
                .AsList();

            if (categorias.Count > 0)
                parceiroTab.ParceirosCategoria = categorias;

            const string qGestao = @"
                        SELECT
                            id as ID,
                            tb_parceiros_id as ParceiroID,
                            contrato_assinado as ContratoAssinado,
                            CASE WHEN inicio_contrato IS NOT NULL THEN DATE_FORMAT(inicio_contrato, '%d/%m/%Y') ELSE NULL END as InicioContrato,
                            CASE WHEN fim_contrato IS NOT NULL THEN DATE_FORMAT(fim_contrato, '%d/%m/%Y') ELSE NULL END as FimContrato,
                            clausula_penalidade as ClausulaPenalidade,
                            nr_pagina as NumeroPagina,
                            vr_contrato as ValorContrato,
                            cd_contrato as Contrato,
                            cd_contrato_anterior as ContratoAnterior,
                            cd_cotacao_relacionada as CotacaoRelacionada,
                            cd_status as Status,
                            cd_necessidade_adicional as NecessidadeAdicional,
                            cd_plataforma_digital as PlataformaDigital,
                            ds_reajuste_anual as ReajusteAnual,
                            cd_renovado as Renovado,
                            url_anexo as UrlAnexo
                        FROM 
                            tb_parceiros_gestao_contratos 
                        WHERE 
                            tb_parceiros_id = @ParceiroID;";

            //parceiroTab.ParceirosGestaoContrato =
            //    await connection.QueryFirstOrDefaultAsync<ParceiroGestaoContratoDTO>(qGestao,
            //            new { ParceiroID = parceiroID });
            parceiroTab.ParceirosGestaoContrato = (await connection.QueryAsync<ParceiroGestaoContratoDTO>(
            qGestao, new { ParceiroID = parceiroID })).AsList();

            return parceiroTab;
        }

        public async Task<ParceiroParamDTO> BuscarParceiroPorIdAux(string parceiroID)
        {
            var connection = _dapperConnection.GetConnection();
            const string q = @"
                        SELECT
                            tp.id as ID,
                            tp.tb_colaborador_codigo_interno_colaborador as CodigoInternoColaborador,
                            tc.nome_completo as NomeColaborador,
                            tp.nome_parceiro as NomeParceiro,
                            tp.descricao_curta as DescricaoCurta,
                            tp.descricao_longa as DescricaoLonga,
                            tp.tipo_parceria as TipoParceria,
                            tp.avaliacao as Avaliacao,
                            tp.url_site as UrlSite,
                            tp.url_linkedin as UrlLinkedin,
                            tp.url_logo as UrlLogo,
                            tp.url_nda as UrlNda,
                            tp.url_contrato as UrlContrato,
                            tp.url_aditivos as UrlAditivos,
                            tp.mais_info as MaisInfo,
                            tp.data_cadastro as DataCadastro,
                            tp.data_atualizacao as DataAtualizacao,
                            tp.tb_colaborador_codigo_colaborador_ultima_alteracao as CodigoColaboradorUltimaAtualizacao,
                            tp.tb_org_id as OrgId,
                            tp.unidade as Unidade
                        FROM 
                            tb_parceiros tp
                        LEFT JOIN
                            tb_colaborador tc
                                ON tc.codigo_interno_colaborador = tp.tb_colaborador_codigo_interno_colaborador
                        WHERE 
                            tp.id = @ParceiroID;";
            var parceiroTab = await connection.QueryFirstOrDefaultAsync<ParceiroParamDTO>(q, new { ParceiroID = parceiroID });

            if (parceiroTab == null)
                return null;

            const string qContato = @"
                        SELECT
                            id as ID,
                            tb_parceiros_id as ParceiroID,
                            nome as Nome,
                            telefone as Telefone,
                            email as Email
                        FROM 
                            tb_parcerios_contatos 
                        WHERE 
                            tb_parceiros_id = @ParceiroID;";

            var contatos = (await connection.QueryAsync<ParceiroContatoDTO>(
                    qContato, new { ParceiroID = parceiroID }))
                .AsList();

            if (contatos.Count > 0)
                parceiroTab.ParceirosContato = contatos;


            const string qCategoria = @"
                        SELECT
                            id as ID,
                            tb_parceiros_id as ParceiroID,
                            categoria as Categoria
                        FROM 
                            tb_parcerios_categorias 
                        WHERE 
                            tb_parceiros_id = @ParceiroID;";

            var categorias = (await connection.QueryAsync<ParceiroCategoriaDTO>(
                    qCategoria, new { ParceiroID = parceiroID }))
                .AsList();

            if (categorias.Count > 0)
                parceiroTab.ParceirosCategoria = categorias;

            return parceiroTab;
        }

        public async Task<List<ParceiroDTO>> BuscarTodosParceiros(int? orgId, string? filtro, string? bucket)
        {
            var connection = _dapperConnection.GetConnection();

            var filtrosVencimento = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "todos", "1 = 1" },
                { "proximos", $"tpgc.cd_status = 'Andamento' AND tpgc.fim_contrato BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 150 DAY)" },
                { "vencidos", "tpgc.cd_status = 'Andamento' AND tpgc.fim_contrato < CURDATE() AND tpgc.fim_contrato IS NOT NULL AND tpgc.fim_contrato != '0000-00-00' AND (tpgc.cd_renovado = false OR tpgc.cd_renovado IS NULL)" },
                { "indeterminado", "tpgc.cd_status = 'Andamento' AND (tpgc.fim_contrato IS NULL OR tpgc.fim_contrato = '0000-00-00')" }
            };

            // Buckets baseados na diferença de dias desde o vencimento
            var buckets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "1-7",  "DATEDIFF(CURDATE(), tpgc.fim_contrato) BETWEEN 1 AND 7" },
                { "8-30", "DATEDIFF(CURDATE(), tpgc.fim_contrato) BETWEEN 8 AND 30" },
                { "31-60","DATEDIFF(CURDATE(), tpgc.fim_contrato) BETWEEN 31 AND 60" },
                { "60",  "DATEDIFF(CURDATE(), tpgc.fim_contrato) > 60" }
            };

            filtro = filtro?.Trim().ToLower();
            bucket = bucket?.Trim().ToLower();

            // garante que sempre tenha um valor válido
            if (string.IsNullOrWhiteSpace(filtro) || !filtrosVencimento.ContainsKey(filtro))
                filtro = "todos";

            var condicaoBase = filtrosVencimento[filtro];

            // Se for "vencidos" e tiver bucket válido → aplica refinamento
            if (filtro == "vencidos" && !string.IsNullOrWhiteSpace(bucket) && buckets.ContainsKey(bucket))
                condicaoBase += $" AND {buckets[bucket]} ";

            string condicaoGestao = filtro == "todos"
            ? ""
            : $@"AND EXISTS (
                    SELECT 1
                    FROM tb_parceiros_gestao_contratos tpgc
                    WHERE tpgc.tb_parceiros_id = tp.id
                      AND {condicaoBase}
                )";

            string q = @$"
                        SELECT
                            tp.id as ID,
                            tp.tb_colaborador_codigo_interno_colaborador as CodigoInternoColaborador,
                            tc2.nome_completo as NomeColaborador,
                            tp.nome_parceiro as NomeParceiro,
                            tp.descricao_curta as DescricaoCurta,
                            tp.descricao_longa as DescricaoLonga,
                            tp.tipo_parceria as TipoParceria,
                            tp.avaliacao as Avaliacao,
                            tp.url_site as UrlSite,
                            tp.url_linkedin as UrlLinkedin,
                            tp.url_logo as UrlLogo,
                            tp.url_nda as UrlNda,
                            tp.url_contrato as UrlContrato,
                            tp.url_aditivos as UrlAditivos,
                            tp.mais_info as MaisInfo,
                            tp.data_cadastro as DataCadastro,
                            tp.data_atualizacao as DataAtualizacao,
                            tp.tb_colaborador_codigo_colaborador_ultima_alteracao as CodigoColaboradorUltimaAtualizacao,
                            tc.nome_completo as NomeColaboradorUltimaAtualizacao,
                            tp.tb_org_id as OrgId,
                            tp.unidade as Unidade
                        FROM tb_parceiros tp
                        LEFT JOIN
                            tb_colaborador tc
                                ON tc.codigo_interno_colaborador = tp.tb_colaborador_codigo_colaborador_ultima_alteracao
                        LEFT JOIN
                            tb_colaborador tc2
                                ON tc2.codigo_interno_colaborador = tp.tb_colaborador_codigo_interno_colaborador
                        WHERE 
                            (@OrgID IS NULL OR tp.tb_org_id = @OrgID)
                            {condicaoGestao};
                        ";

            var parametros = new { OrgID = orgId };
            var parceiros = (await connection.QueryAsync<ParceiroDTO>(q, parametros)).AsList();

            foreach (var parceiro in parceiros)
            {
                const string qContato = @"
                        SELECT
                            id as ID,
                            tb_parceiros_id as ParceiroID,
                            nome as Nome,
                            telefone as Telefone,
                            email as Email
                        FROM 
                            tb_parcerios_contatos 
                        WHERE 
                            tb_parceiros_id = @ParceiroID;";

                var contatos = (await connection.QueryAsync<ParceiroContatoDTO>(
                        qContato, new { ParceiroID = parceiro.ID }))
                    .AsList();

                parceiro.ParceirosContato = contatos;

                const string qCategoria = @"
                        SELECT
                            id as ID,
                            tb_parceiros_id as ParceiroID,
                            categoria as Categoria
                        FROM 
                            tb_parcerios_categorias 
                        WHERE 
                            tb_parceiros_id = @ParceiroID;";

                var categorias = (await connection.QueryAsync<ParceiroCategoriaDTO>(
                        qCategoria, new { ParceiroID = parceiro.ID }))
                    .AsList();

                parceiro.ParceirosCategoria = categorias;


                string qGestao = @$"
                        SELECT
                            tpgc.id AS ID,
                            tpgc.tb_parceiros_id AS ParceiroID,
                            tpgc.contrato_assinado AS ContratoAssinado,
                            CASE 
                                WHEN tpgc.inicio_contrato IS NOT NULL 
                                THEN DATE_FORMAT(tpgc.inicio_contrato, '%d/%m/%Y') 
                                ELSE NULL 
                            END AS InicioContrato,
                            CASE 
                                WHEN tpgc.fim_contrato IS NOT NULL 
                                THEN DATE_FORMAT(tpgc.fim_contrato, '%d/%m/%Y') 
                                ELSE NULL 
                            END AS FimContrato,
                            tpgc.clausula_penalidade AS ClausulaPenalidade,
                            tpgc.nr_pagina AS NumeroPagina,
                            tpgc.vr_contrato AS ValorContrato,
                            tpgc.cd_contrato AS Contrato,
                            tpgc.cd_contrato_anterior AS ContratoAnterior,
                            tpgc.cd_cotacao_relacionada AS CotacaoRelacionada,
                            tpgc.cd_status AS Status,
                            tpgc.cd_necessidade_adicional AS NecessidadeAdicional,
                            tpgc.cd_plataforma_digital AS PlataformaDigital,
                            tpgc.ds_reajuste_anual AS ReajusteAnual,
                            tpgc.cd_renovado AS Renovado,
                            tpgc.url_anexo AS UrlAnexo
                        FROM 
                            tb_parceiros_gestao_contratos tpgc
                        WHERE 
                            tpgc.tb_parceiros_id = @ParceiroID
                            {(filtro == "todos" ? "" : $"AND {condicaoBase}")};";

                //parceiro.ParceirosGestaoContrato =
                //    await connection.QueryFirstOrDefaultAsync<ParceiroGestaoContratoDTO>(qGestao,
                //        new { ParceiroID = parceiro.ID });
                var gestaoContratos = (await connection.QueryAsync<ParceiroGestaoContratoDTO>(
                                        qGestao, new { ParceiroID = parceiro.ID })
                                    ).AsList();

                var contratoIDs = gestaoContratos.Select(c => c.ID).ToList();

                var todosEmails = await connection.QueryAsync<EmailContratoDTO>(@"
                        SELECT
                            tncvd.email AS Email,
                            tncvd.tb_parceiros_gestao_contratos_id AS GestaoContratoID
                        FROM
                            tb_notificacao_contratos_vencidos_destinatarios tncvd
                        WHERE
                            tncvd.tb_parceiros_gestao_contratos_id IN @ContratoIDs;",
                    new { ContratoIDs = contratoIDs });

                // Separa os emails por contrato com base no id do contrato.
                var emailsPorContrato = todosEmails
                .GroupBy(e => e.GestaoContratoID)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Email).ToList());

                foreach (var contrato in gestaoContratos)
                {
                    if (emailsPorContrato.TryGetValue(contrato.ID, out var emails))
                    {
                        contrato.EmailsNotificacao = emails;
                    }
                }

                parceiro.ParceirosGestaoContrato = gestaoContratos;
            }

            return parceiros;

        }

        public async Task<ParceiroParamDTO> InserirParceiro(ParceiroInserirParam p, string cpfUsuarioLogado)
        {
            var connection = _dapperConnection.GetConnection();

            var tx = await connection.BeginTransactionAsync();

            try
            {

                var parceiroId = Guid.NewGuid();

                const string q = @"
                        INSERT INTO tb_parceiros
                        (id, tb_colaborador_codigo_interno_colaborador, nome_parceiro, descricao_curta, descricao_longa, tipo_parceria,
                         avaliacao, url_site, url_linkedin, url_logo, url_nda, url_contrato, url_aditivos, mais_info, data_cadastro, 
                         tb_colaborador_codigo_colaborador_ultima_alteracao, tb_org_id, unidade)
                        VALUES
                        (@Id, @CodigoInternoColaborador, @NomeParceiro, @DescricaoCurta, @DescricaoLonga, @TipoParceria,
                         @Avaliacao, @UrlSite, @UrlLinkedin, @UrlLogo, @UrlNda, @UrlContrato, @UrlAditivos, @MaisInfo, NOW(), 
                         @CodColaboradorUsuarioLogado, @OrgId, @Unidade);
                        ";

                var ok = await connection.ExecuteAsync(q, new
                {
                    Id = parceiroId,
                    p.CodigoInternoColaborador,
                    p.NomeParceiro,
                    p.DescricaoCurta,
                    p.DescricaoLonga,
                    p.TipoParceria,
                    p.Avaliacao,
                    p.UrlSite,
                    p.UrlLinkedin,
                    p.UrlLogo,
                    p.UrlNda,
                    p.UrlContrato,
                    p.UrlAditivos,
                    p.MaisInfo,
                    CodColaboradorUsuarioLogado = cpfUsuarioLogado,
                    p.OrgId,
                    p.Unidade
                }, tx);

                //Guid gestaoId = p.ParceirosGestaoContrato != null ? Guid.NewGuid() : Guid.Empty;

                if (p.ParceirosCategoria is { Count: > 0 })
                {
                    const string qCat = @"
                                    INSERT INTO tb_parcerios_categorias
                                    (id, tb_parceiros_id, categoria)
                                    VALUES
                                    (@Id, @ParceiroId, @Categoria);";

                    var dadosCat = new List<object>();
                    foreach (var cat in p.ParceirosCategoria)
                        dadosCat.Add(new
                        {
                            Id = Guid.NewGuid(),
                            ParceiroId = parceiroId,
                            Categoria = cat
                        });

                    await connection.ExecuteAsync(qCat, dadosCat, tx);
                }

                if (p.ParceirosContato is { Count: > 0 })
                {
                    const string qContato = @"
                                        INSERT INTO tb_parcerios_contatos
                                        (id, tb_parceiros_id, nome, telefone, email)
                                        VALUES
                                        (@Id, @ParceiroId, @Nome, @Telefone, @Email);";

                    var dadosContato = new List<object>();
                    foreach (var c in p.ParceirosContato)
                        dadosContato.Add(new
                        {
                            Id = Guid.NewGuid(),
                            ParceiroId = parceiroId,
                            c.Nome,
                            c.Telefone,
                            c.Email
                        });

                    await connection.ExecuteAsync(qContato, dadosContato, tx);
                }
                /*
                if (p.ParceirosGestaoContrato != null)
                {
                    var g = p.ParceirosGestaoContrato;

                    const string qGestao = @"
                                        INSERT INTO tb_parceiros_gestao_contratos
                                        (id, tb_parceiros_id, contrato_assinado, inicio_contrato,
                                         fim_contrato, clausula_penalidade, nr_pagina, vr_contrato, 
                                         cd_contrato, cd_contrato_anterior, cd_cotacao_relacionada, cd_status, 
                                         cd_necessidade_adicional, cd_plataforma_digital, ds_reajuste_anual, cd_renovado )
                                        VALUES
                                        (@Id, @ParceiroId, @ContratoAssinado, @InicioContrato,
                                         @FimContrato, @ClausulaPenalidade, @NumeroPagina, @ValorContrato,
                                         @Contrato, @ContratoAnterior, @CotacaoRelacionada, @Status, 
                                         @NecessidadeAdicional, @PlataformaDigital, @ReajusteAnual, @Renovado );";

                    await connection.ExecuteAsync(qGestao, new
                    {
                        Id = gestaoId,
                        ParceiroId = parceiroId,
                        g.ContratoAssinado,
                        g.InicioContrato,  // string → TIMESTAMP (formato 'YYYY-MM-DD HH:MM:SS')
                        g.FimContrato,
                        g.ClausulaPenalidade,
                        g.NumeroPagina,
                        g.ValorContrato,
                        g.Contrato,
                        g.ContratoAnterior,
                        g.CotacaoRelacionada,
                        g.Status,
                        g.NecessidadeAdicional,
                        g.PlataformaDigital,
                        g.ReajusteAnual,
                        g.Renovado
                    }, tx);
                }
                */
                await tx.CommitAsync();

                return ok > 0 ? await BuscarParceiroPorIdAux(parceiroId.ToString()) : null;

            }
            catch (Exception)
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<ParceiroParamDTO> AtualizarParceiro(string parceiroID, ParceiroIDParam p, string cpfUsuarioLogado)
        {
            var connection = _dapperConnection.GetConnection();

            const string q = @"
                    UPDATE tb_parceiros SET
                        tb_colaborador_codigo_interno_colaborador = @CodigoInternoColaborador,
                        nome_parceiro  = @NomeParceiro,
                        descricao_curta= @DescricaoCurta,
                        descricao_longa= @DescricaoLonga,
                        tipo_parceria  = @TipoParceria,
                        avaliacao      = @Avaliacao,
                        url_site       = @UrlSite,
                        url_linkedin   = @UrlLinkedin,
                        url_logo       = @UrlLogo,
                        url_nda        = @UrlNda,
                        url_contrato   = @UrlContrato,
                        url_aditivos   = @UrlAditivos,
                        mais_info      = @MaisInfo,
                        data_atualizacao = NOW(),
                        tb_colaborador_codigo_colaborador_ultima_alteracao = @CodColaboradorUsuarioLogado,
                        tb_org_id      = @OrgId,
                        unidade        = @Unidade
                    WHERE id = @ParceiroID;";

            var ok = await connection.ExecuteAsync(q, new
            {
                ParceiroID = parceiroID,
                p.CodigoInternoColaborador,
                p.NomeParceiro,
                p.DescricaoCurta,
                p.DescricaoLonga,
                p.TipoParceria,
                p.Avaliacao,
                p.UrlSite,
                p.UrlLinkedin,
                p.UrlLogo,
                p.UrlNda,
                p.UrlContrato,
                p.UrlAditivos,
                p.MaisInfo,
                CodColaboradorUsuarioLogado = cpfUsuarioLogado,
                p.OrgId,
                p.Unidade
            });

            // ─── Atualiza Categorias ───
            await connection.ExecuteAsync("DELETE FROM tb_parcerios_categorias WHERE tb_parceiros_id = @ParceiroID",
                new { ParceiroID = parceiroID });

            if (p.ParceirosCategoria?.Any() == true)
            {
                foreach (var categoria in p.ParceirosCategoria)
                {
                    await connection.ExecuteAsync(@"
                                            INSERT INTO tb_parcerios_categorias (id, tb_parceiros_id, categoria)
                                            VALUES (UUID(), @ParceiroID, @Categoria);",
                        new { ParceiroID = parceiroID, Categoria = categoria });
                }
            }

            // ─── Atualiza Contatos ───
            await connection.ExecuteAsync("DELETE FROM tb_parcerios_contatos WHERE tb_parceiros_id = @ParceiroID",
                new { ParceiroID = parceiroID });

            if (p.ParceirosContato?.Any() == true)
            {
                foreach (var contato in p.ParceirosContato)
                {
                    await connection.ExecuteAsync(@"
                                        INSERT INTO tb_parcerios_contatos (id, tb_parceiros_id, nome, telefone, email)
                                        VALUES (UUID(), @ParceiroID, @Nome, @Telefone, @Email);",
                        new
                        {
                            ParceiroID = parceiroID,
                            contato.Nome,
                            contato.Telefone,
                            contato.Email
                        });
                }
            }

            // ─── Atualiza Gestão de Contrato ───
            /*
             * if (p.ParceirosGestaoContrato != null)
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM tb_parceiros_gestao_contratos WHERE tb_parceiros_id = @ParceiroID",
                    new { ParceiroID = parceiroID });

                if (count > 0)
                {
                    await connection.ExecuteAsync(@"
                                            UPDATE tb_parceiros_gestao_contratos SET
                                                contrato_assinado = @ContratoAssinado,
                                                inicio_contrato = @InicioContrato,
                                                fim_contrato = @FimContrato,
                                                clausula_penalidade = @ClausulaPenalidade,
                                                nr_pagina = @NumeroPagina,
                                                vr_contrato = @ValorContrato,
                                                cd_contrato = @Contrato,
                                                cd_contrato_anterior = @ContratoAnterior,
                                                cd_cotacao_relacionada = @CotacaoRelacionada,
                                                cd_status = @Status,
                                                cd_necessidade_adicional = @NecessidadeAdicional,
                                                cd_plataforma_digital = @PlataformaDigital,
                                                ds_reajuste_anual = @ReajusteAnual,
                                                cd_renovado = @Renovado
                                            WHERE tb_parceiros_id = @ParceiroID;",
                        new
                        {
                            ParceiroID = parceiroID,
                            p.ParceirosGestaoContrato.ContratoAssinado,
                            p.ParceirosGestaoContrato.InicioContrato,
                            p.ParceirosGestaoContrato.FimContrato,
                            p.ParceirosGestaoContrato.ClausulaPenalidade,
                            p.ParceirosGestaoContrato.NumeroPagina,
                            p.ParceirosGestaoContrato.ValorContrato,
                            p.ParceirosGestaoContrato.Contrato,
                            p.ParceirosGestaoContrato.ContratoAnterior,
                            p.ParceirosGestaoContrato.CotacaoRelacionada,
                            p.ParceirosGestaoContrato.Status,
                            p.ParceirosGestaoContrato.NecessidadeAdicional,
                            p.ParceirosGestaoContrato.PlataformaDigital,
                            p.ParceirosGestaoContrato.ReajusteAnual,
                            p.ParceirosGestaoContrato.Renovado
                        });
                }
            }
            */
            return await BuscarParceiroPorIdAux(parceiroID);

        }

        public async Task<bool> DeletarParceiro(string parceiroID)
        {
            var connection = _dapperConnection.GetConnection();
            const string q = @"
                    DELETE FROM tb_parceiros 
                           WHERE id = @ParceiroID;";
            var ok = await connection.ExecuteAsync(q, new { ParceiroID = parceiroID });
            return ok > 0;
        }

        public async Task<ParceiroArchiveDTO> BuscarArquivoPorId(string arquivoId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT 
                        id as ID,
                        tb_parceiros_id as ParceiroID,
                        parceiro_archive_link as Link,
                        tb_parceiros_archive_type_id as ArquivoTipoID,
                        tb_parceiros_archive_origin_id as ArquivoOriginID
                    FROM tb_parceiros_archives 
                    WHERE id = @ArquivoId;
            ";

            var parametros = new
            {
                ArquivoId = arquivoId
            };

            var result = await connection.QueryFirstOrDefaultAsync<ParceiroArchiveDTO>(query, parametros);
            return result;
        }

        public async Task<List<ParceiroArchiveDTO>> BuscarArquivosPorParceiroId(string parceiroID)
        {

            var connection = _dapperConnection.GetConnection();

            var query = @"
                    SELECT 
                        id as ID,
                        tb_parceiros_id as ParceiroID,
                        parceiro_archive_link as Link,
                        tb_parceiros_archive_type_id as ArquivoTipoID,
                        tb_parceiros_archive_origin_id as ArquivoOriginID
                    FROM tb_parceiros_archives 
                    WHERE tb_parceiros_id = @ParceiroID;
            ";

            var param = new
            {
                ParceiroID = parceiroID
            };

            var result = await connection.QueryAsync<ParceiroArchiveDTO>(query, param);

            return result as List<ParceiroArchiveDTO>;
        }

        // ─── Início Gestão de Contratos ───
        public async Task<ParceiroGestaoContratoDTO> InserirContrato(ParceiroGestaoContratoParceiroIDParam p)
        {
            var connection = _dapperConnection.GetConnection();

            try
            {
                if (string.IsNullOrWhiteSpace(p.ParceiroID))
                    throw new ArgumentException("O ID do Parceiro é obrigatório.");

                const string qValidaParceiro = "SELECT COUNT(*) FROM tb_parceiros WHERE id = @ParceiroId;";
                var existeParceiro = await connection.ExecuteScalarAsync<int>(qValidaParceiro, new { ParceiroId = p.ParceiroID });

                if (existeParceiro == 0)
                    throw new Exception($"Parceiro com ID {p.ParceiroID} não encontrado na tabela tb_parceiros.");

                string gestaoContratoId = Guid.NewGuid().ToString();
                int ok = 0;

                if (p.ParceiroID != null)
                {
                    const string qGestao = @"
                                        INSERT INTO tb_parceiros_gestao_contratos
                                        (id, tb_parceiros_id, contrato_assinado, inicio_contrato,
                                         fim_contrato, clausula_penalidade, nr_pagina, vr_contrato, 
                                         cd_contrato, cd_contrato_anterior, cd_cotacao_relacionada, cd_status, 
                                         cd_necessidade_adicional, cd_plataforma_digital, ds_reajuste_anual, cd_renovado, url_anexo )
                                        VALUES
                                        (@Id, @ParceiroId, @ContratoAssinado, @InicioContrato,
                                         @FimContrato, @ClausulaPenalidade, @NumeroPagina, @ValorContrato,
                                         @Contrato, @ContratoAnterior, @CotacaoRelacionada, @Status, 
                                         @NecessidadeAdicional, @PlataformaDigital, @ReajusteAnual, @Renovado, @UrlAnexo );";

                    ok = await connection.ExecuteAsync(qGestao, new
                    {
                        Id = gestaoContratoId,
                        p.ParceiroID,
                        p.ContratoAssinado,
                        p.InicioContrato,
                        p.FimContrato,
                        p.ClausulaPenalidade,
                        p.NumeroPagina,
                        p.ValorContrato,
                        p.Contrato,
                        p.ContratoAnterior,
                        p.CotacaoRelacionada,
                        Status = (p.Status ?? EnumStatusContrato.Andamento).ToString(),
                        p.NecessidadeAdicional,
                        p.PlataformaDigital,
                        p.ReajusteAnual,
                        p.Renovado,
                        p.UrlAnexo
                    });

                    foreach (var email in p.EmailsNotificacao)
                    {
                        await connection.ExecuteAsync(
                        @"  INSERT INTO tb_notificacao_contratos_vencidos_destinatarios
                            (tb_parceiros_gestao_contratos_id, email)
                            VALUES
                            (@gestaoContratoId, @email)
                        ",
                        new { gestaoContratoId, email });
                    }
                }

                return ok > 0 ? await BuscarGestaoContratoPorId(gestaoContratoId) : null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ParceiroGestaoContratoDTO> AtualizarContrato(ParceiroGestaoContratoParam p)
        {
            var connection = _dapperConnection.GetConnection();

            // ─── Atualiza Gestão de Contrato ───
            if (p.ID != null)
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM tb_parceiros_gestao_contratos WHERE id = @ID",
                    new { ID = p.ID });

                if (count > 0)
                {
                    await connection.ExecuteAsync(@"
                                            UPDATE tb_parceiros_gestao_contratos SET
                                                contrato_assinado = @ContratoAssinado,
                                                inicio_contrato = @InicioContrato,
                                                fim_contrato = @FimContrato,
                                                clausula_penalidade = @ClausulaPenalidade,
                                                nr_pagina = @NumeroPagina,
                                                vr_contrato = @ValorContrato,
                                                cd_contrato = @Contrato,
                                                cd_contrato_anterior = @ContratoAnterior,
                                                cd_cotacao_relacionada = @CotacaoRelacionada,
                                                cd_status = @Status,
                                                cd_necessidade_adicional = @NecessidadeAdicional,
                                                cd_plataforma_digital = @PlataformaDigital,
                                                ds_reajuste_anual = @ReajusteAnual,
                                                cd_renovado = @Renovado,
                                                url_anexo   = @UrlAnexo
                                            WHERE id = @ID;",
                        new
                        {
                            p.ID,
                            p.ContratoAssinado,
                            p.InicioContrato,
                            p.FimContrato,
                            p.ClausulaPenalidade,
                            p.NumeroPagina,
                            p.ValorContrato,
                            p.Contrato,
                            p.ContratoAnterior,
                            p.CotacaoRelacionada,
                            Status = (p.Status ?? EnumStatusContrato.Andamento).ToString(),
                            p.NecessidadeAdicional,
                            p.PlataformaDigital,
                            p.ReajusteAnual,
                            p.Renovado,
                            p.UrlAnexo
                        });

                    await connection.ExecuteAsync(
                        @"  DELETE FROM tb_notificacao_contratos_vencidos_destinatarios 
                            WHERE tb_parceiros_gestao_contratos_id = @contratoId
                        ",
                        new { contratoId = p.ID }
                    );

                    foreach (var email in p.EmailsNotificacao)
                    {
                        await connection.ExecuteAsync(
                        @"  INSERT INTO tb_notificacao_contratos_vencidos_destinatarios
                            (tb_parceiros_gestao_contratos_id, email)
                            VALUES
                            (@contratoId, @email)
                        ",
                        new { contratoId = p.ID, email });
                    }
                }
            }

            return await BuscarGestaoContratoPorId(p.ID);
        }

        public async Task<bool> DeletarContrato(string ID)
        {
            var connection = _dapperConnection.GetConnection();
            const string q = @"
                    DELETE FROM tb_parceiros_gestao_contratos 
                           WHERE id = @ID;";
            var ok = await connection.ExecuteAsync(q, new { ID });

            return ok > 0;
        }
        public async Task<bool> DeletarArquivo(string ID)
        {
            var connection = _dapperConnection.GetConnection();
            const string q = @"
                    DELETE FROM tb_parceiros_archives 
                           WHERE id = @ID;";
            var ok = await connection.ExecuteAsync(q, new { ID = ID });
            return ok > 0;
        }

        public async Task<ParceiroGestaoContratoDTO> BuscarGestaoContratoPorId(string gestaoContratoID)
        {
            var connection = _dapperConnection.GetConnection();

            const string qGestao = @"
                              SELECT
                                tgc.id as ID,
                                tgc.tb_parceiros_id as ParceiroID,
                                tgc.contrato_assinado as ContratoAssinado,
                                CASE WHEN tgc.inicio_contrato IS NOT NULL THEN DATE_FORMAT(tgc.inicio_contrato, '%d/%m/%Y') ELSE NULL END as InicioContrato,
                                CASE WHEN tgc.fim_contrato IS NOT NULL THEN DATE_FORMAT(tgc.fim_contrato, '%d/%m/%Y') ELSE NULL END as FimContrato,
                                tgc.clausula_penalidade as ClausulaPenalidade,
                                tgc.nr_pagina as NumeroPagina,
                                tgc.vr_contrato as ValorContrato,
                                tgc.cd_contrato as Contrato,
                                tgc.cd_contrato_anterior as ContratoAnterior,
                                tgc.cd_cotacao_relacionada as CotacaoRelacionada,
                                tgc.cd_status as Status,
                                tgc.cd_necessidade_adicional as NecessidadeAdicional,
                                tgc.cd_plataforma_digital as PlataformaDigital,
                                tgc.ds_reajuste_anual as ReajusteAnual,
                                tgc.cd_renovado as Renovado,
                                tgc.url_anexo as UrlAnexo
                            FROM 
                                tb_parceiros_gestao_contratos tgc
                            WHERE
                                tgc.id = @GestaoContratoID;";

            var resultado = await connection.QueryFirstOrDefaultAsync<ParceiroGestaoContratoDTO>(qGestao,
                        new { GestaoContratoID = gestaoContratoID });

            resultado.EmailsNotificacao = (await connection.QueryAsync<string>(@"
                              SELECT
                                tncvd.email
                            FROM 
                                tb_notificacao_contratos_vencidos_destinatarios tncvd
                            WHERE
                                tncvd.tb_parceiros_gestao_contratos_id = @GestaoContratoID;",
                        new { GestaoContratoID = gestaoContratoID })).ToList();

            return resultado;
        }

        // ─── Fim Gestão de Contratos ───
        //
        // ─── Início Arquivo(Contratos) ───
        public async Task<ParceiroArchiveDTO> InserirArquivo(ParceiroArchiveParam param, string url)
        {
            var id = Guid.NewGuid();
            var connection = _dapperConnection.GetConnection();

            var archiveType = param.ArquivoTipoID.ToInt();
            var archiveOrigin = param.ArquivoOriginID.ToInt();

            var query = @"
            INSERT INTO 
                tb_parceiros_archives (id, tb_parceiros_id, parceiro_archive_link, tb_parceiros_archive_type_id, tb_parceiros_archive_origin_id)
            VALUES 
                (@Id, @ParceirosId, @ParceiroArchiveLink,  @ParceiroArchiveTypeId, @ParceiroArchiveOriginId);
            ";

            var parametros = new
            {
                Id = id,
                ParceirosId = param.ParceiroID,
                ParceiroArchiveLink = url,
                ParceiroArchiveTypeId = archiveType.ToString(),
                ParceiroArchiveOriginId = archiveOrigin.ToString()
                //ParceiroGestaoContratoId = param.ParceiroGestaoContratoId
            };

            var insert = await connection.ExecuteAsync(query, parametros);

            if (insert > 0)
            {
                var result = await BuscarArquivoPorId(id.ToString());

                return result;
            }

            return null;
        }

        // ─── Fim Arquivo(Contratos) ───

        public async Task<List<dynamic>> RelatorioParceriaAliancas(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
		                tp.unidade AS 'Unidade',
		                tp.nome_parceiro AS 'Nome Parceiro',
		                tp.tipo_parceria  AS 'Tipo Parceria',
		                tpgc.cd_contrato AS 'Número Contrato',
                        DATE_FORMAT(tpgc.inicio_contrato, '%d/%m/%Y') AS 'Data Início Contrato',
                        DATE_FORMAT(tpgc.fim_contrato, '%d/%m/%Y') AS 'Data Fim Contrato',
		                tpgc.cd_status AS 'Status',
		                tpgc.vr_contrato AS 'Valor Contrato',
		                tpgc.clausula_penalidade AS 'Cláusula Penalidades',
		                tpgc.ds_reajuste_anual AS 'Reajuste Anual'
                FROM tb_parceiros tp 
                LEFT JOIN
	                tb_parceiros_gestao_contratos tpgc 
		                ON tpgc.tb_parceiros_id = tp.id 
                WHERE tp.tb_org_id = @orgId
                ORDER BY tp.nome_parceiro;
            ";

            var resultado = await connection.QueryAsync<dynamic>(query, new { orgId });
            return resultado.ToList();
        }

        // ───────────────────────────────────────────────
        // Importação de Planilha de Contratos
        // ───────────────────────────────────────────────

        public async Task<(int Sucesso, int Erros, List<string> Mensagens)> ImportacaoPlanilhaContrato(
            string caminhoArquivo,
            string cpfUsuarioLogado,
            int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            int sucesso = 0;
            int erros = 0;
            var mensagens = new List<string>();

            try
            {
                // Configura a licença do EPPlus 8+ (NonCommercial)
                OfficeOpenXml.ExcelPackage.License.SetNonCommercialOrganization("Foursys");

                // Normaliza o caminho do arquivo (substitui / por \ no Windows)
                caminhoArquivo = System.IO.Path.GetFullPath(caminhoArquivo.Replace("/", "\\"));
                
                // Verifica se o arquivo existe
                if (!System.IO.File.Exists(caminhoArquivo))
                {
                    mensagens.Add($"Arquivo não encontrado: {caminhoArquivo}");
                    return (0, 1, mensagens);
                }

                // Lê a planilha Excel usando EPPlus
                using (var package = new ExcelPackage(new System.IO.FileInfo(caminhoArquivo)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Primeira aba
                    int rowCount = worksheet.Dimension?.Rows ?? 0;

                    if (rowCount <= 1)
                    {
                        mensagens.Add("Planilha vazia ou sem dados (somente cabeçalho).");
                        return (0, 1, mensagens);
                    }

                    // ─── VALIDAÇÃO DO CABEÇALHO ───
                    var cabecalhoEsperado = new[]
                    {
                        "Nome da Empresa",
                        "Descrição Curta",
                        "Descrição Longa",
                        "Mais Informações",
                        "Tipo Parceria",
                        "Website",
                        "Linkedin",
                        "Nome do Contato",
                        "Telefone",
                        "E-mail",
                        "Nome do Contrato",
                        "Início de Contrato",
                        "Fim do Contrato",
                        "Cláusula de Penalidades",
                        "Num. Contrato Anterior",
                        "Numero de Paginas",
                        "Reajuste Anual",
                        "Emails para Notificação"
                    };

                    var cabecalhoAtual = new List<string>();
                    for (int col = 1; col <= 18; col++)
                    {
                        var cellValue = worksheet.Cells[1, col].Value?.ToString()?.Trim() ?? "";
                        cabecalhoAtual.Add(cellValue);
                    }

                    // Verifica se o cabeçalho está correto
                    var errosCabecalho = new List<string>();
                    for (int i = 0; i < cabecalhoEsperado.Length; i++)
                    {
                        if (i < cabecalhoAtual.Count)
                        {
                            if (!cabecalhoAtual[i].Equals(cabecalhoEsperado[i], StringComparison.OrdinalIgnoreCase))
                            {
                                errosCabecalho.Add($"Coluna {i + 1}: Esperado '{cabecalhoEsperado[i]}', encontrado '{cabecalhoAtual[i]}'");
                            }
                        }
                        else
                        {
                            errosCabecalho.Add($"Coluna {i + 1}: Esperado '{cabecalhoEsperado[i]}', coluna não encontrada");
                        }
                    }

                    if (errosCabecalho.Any())
                    {
                        mensagens.Add("ERRO: O cabeçalho da planilha está incorreto. A importação não será processada.");
                        mensagens.Add("Cabeçalho esperado (na ordem):");
                        for (int i = 0; i < cabecalhoEsperado.Length; i++)
                        {
                            mensagens.Add($"  {i + 1}. {cabecalhoEsperado[i]}");
                        }
                        mensagens.Add("");
                        mensagens.Add("Erros encontrados:");
                        mensagens.AddRange(errosCabecalho);
                        return (0, 1, mensagens);
                    }

                    // Itera pelas linhas (pulando o cabeçalho na linha 1)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var tx = await connection.BeginTransactionAsync();

                        try
                        {
                            // Parceiros
                            var codUsuario = "00000000000";
                            var nomeEmpresa = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            var tipoParceria = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
                            var descCurta = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                            var descLonga = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                            var maisInfo = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                            var urlSite = worksheet.Cells[row, 6].Value?.ToString()?.Trim();
                            var urlLinkedin = worksheet.Cells[row, 7].Value?.ToString()?.Trim();

                            // Contatos
                            var nomeContato = worksheet.Cells[row, 8].Value?.ToString()?.Trim();
                            var telContato = worksheet.Cells[row, 9].Value?.ToString()?.Trim();
                            var emailContato = worksheet.Cells[row, 10].Value?.ToString()?.Trim();
                            var emailsNotificacao = worksheet.Cells[row, 18].Value?.ToString()?.Trim();

                            // Gestao Contratos
                            var nomeContrato = worksheet.Cells[row, 11].Value?.ToString()?.Trim();
                            var inicioContratoStr = worksheet.Cells[row, 12].Value?.ToString()?.Trim();
                            var fimContratoStr = worksheet.Cells[row, 13].Value?.ToString()?.Trim();
                            var numeroPaginasStr = worksheet.Cells[row, 16].Value?.ToString()?.Trim();
                            var cdContratoAnt = worksheet.Cells[row, 15].Value?.ToString()?.Trim();
                            var clausulaPenal = worksheet.Cells[row, 14].Value?.ToString()?.Trim();
                            var reajusteAnual = worksheet.Cells[row, 17].Value?.ToString()?.Trim();

                            // Validação mínima: Nome da Empresa é obrigatório
                            if (string.IsNullOrWhiteSpace(nomeEmpresa))
                            {
                                mensagens.Add($"Linha {row}: Nome da Empresa é obrigatório. Registro ignorado.");
                                erros++;
                                await tx.RollbackAsync();
                                continue;
                            }

                            // ─── VALIDAÇÕES DE TAMANHO MÁXIMO ───
                            var validacoes = new List<string>();

                            // tb_parceiros
                            if (nomeEmpresa?.Length > 150)
                                validacoes.Add($"Nome da Empresa ({nomeEmpresa.Length} caracteres) excede 150");
                            if (tipoParceria?.Length > 150)
                                validacoes.Add($"Tipo Parceria ({tipoParceria.Length} caracteres) excede 150");
                            // descricao_curta e descricao_longa são TEXT (65535 bytes), mais_info também é TEXT
                            if (urlSite?.Length > 1500)
                                validacoes.Add($"URL Site ({urlSite.Length} caracteres) excede 1500");
                            if (urlLinkedin?.Length > 1500)
                                validacoes.Add($"URL LinkedIn ({urlLinkedin.Length} caracteres) excede 1500");

                            // tb_parcerios_contatos
                            if (nomeContato?.Length > 150)
                                validacoes.Add($"Nome Contato ({nomeContato.Length} caracteres) excede 150");
                            if (telContato?.Length > 20)
                                validacoes.Add($"Telefone ({telContato.Length} caracteres) excede 20");
                            if (emailContato?.Length > 150)
                                validacoes.Add($"Email ({emailContato.Length} caracteres) excede 150");

                            // tb_parceiros_gestao_contratos
                            if (nomeContrato?.Length > 150)
                                validacoes.Add($"Nome Contrato ({nomeContrato.Length} caracteres) excede 150");
                            if (cdContratoAnt?.Length > 150)
                                validacoes.Add($"Contrato Anterior ({cdContratoAnt.Length} caracteres) excede 150");
                            // clausula_penalidade e ds_reajuste_anual são TEXT (65535 bytes)

                            if (validacoes.Any())
                            {
                                mensagens.Add($"Linha {row}: Campos excedem tamanho máximo: {string.Join(", ", validacoes)}. Registro ignorado.");
                                erros++;
                                await tx.RollbackAsync();
                                continue;
                            }

                            // ─── 1. INSERIR PARCEIRO ───
                            var parceiroId = Guid.NewGuid().ToString();

                            const string qParceiro = @"
                                INSERT INTO tb_parceiros
                                (id, tb_colaborador_codigo_interno_colaborador, nome_parceiro, 
                                 descricao_curta, descricao_longa,tipo_parceria, 
                                 data_cadastro, url_site, url_linkedin, mais_info,
                                 tb_colaborador_codigo_colaborador_ultima_alteracao, tb_org_id)
                                VALUES
                                (@Id, @CodigoInternoColaborador, @NomeParceiro, 
                                 @DescCurta, @DescLonga, @TipoParceria, 
                                 NOW(), @Url_Site, @UrlLinkedin, @MaisInfo,
                                 @CodColaboradorUsuarioLogado, @OrgId);
                            ";

                            await connection.ExecuteAsync(qParceiro, new
                            {
                                Id = parceiroId,
                                CodigoInternoColaborador = codUsuario,
                                NomeParceiro = nomeEmpresa,
                                DescCurta = descCurta,
                                DescLonga = descLonga,
                                TipoParceria = tipoParceria,
                                Url_Site = urlSite,
                                UrlLinkedin = urlLinkedin,
                                MaisInfo = maisInfo,
                                CodColaboradorUsuarioLogado = codUsuario,
                                OrgId = 2
                            }, tx);

                            // ─── 2. INSERIR CONTATOS ───
                            if (!string.IsNullOrWhiteSpace(nomeContato))
                            {
                                var email = "";
                                if (!string.IsNullOrWhiteSpace(emailContato))
                                    email = emailContato;
                                else
                                    email = emailsNotificacao;

                                const string qContato = @"
                                    INSERT INTO tb_parcerios_contatos
                                    (id, tb_parceiros_id, nome, telefone,email)
                                    VALUES
                                    (@Id, @ParceiroId, @Nome, @Telefone, @Email);
                                ";

                                await connection.ExecuteAsync(qContato, new
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    ParceiroId = parceiroId,
                                    Nome = nomeContato,
                                    Telefone = telContato,
                                    Email = email
                                }, tx);

                            }

                            // Não deletar esse trecho
                            // ─── 3. INSERIR CATEGORIA (se houver tipo de parceria) ───
                            /*
                            if (!string.IsNullOrWhiteSpace(categoria))
                            {
                                const string qCategoria = @"
                                    INSERT INTO tb_parcerios_categorias
                                    (id, tb_parceiros_id, categoria)
                                    VALUES
                                    (@Id, @ParceiroId, @Categoria);
                                ";

                                await connection.ExecuteAsync(qCategoria, new
                                {
                                    Id = Guid.NewGuid(),
                                    ParceiroId = parceiroId,
                                    Categoria = categoria
                                }, tx);
                            }
                            */

                            // ─── 4. INSERIR GESTÃO DE CONTRATO ───
                            DateTime? inicioContrato = null;
                            DateTime? fimContrato = null;
                            int? numeroPaginas = null;

                            // Parse de datas
                            if (!string.IsNullOrWhiteSpace(inicioContratoStr))
                            {
                                if (DateTime.TryParse(inicioContratoStr, out DateTime dataInicio))
                                    inicioContrato = dataInicio;
                            }

                            if (!string.IsNullOrWhiteSpace(fimContratoStr))
                            {
                                if (DateTime.TryParse(fimContratoStr, out DateTime dataFim))
                                    fimContrato = dataFim;
                            }

                            // Parse de número de páginas
                            if (!string.IsNullOrWhiteSpace(numeroPaginasStr))
                            {
                                if (int.TryParse(numeroPaginasStr, out int numPaginas))
                                    numeroPaginas = numPaginas;
                            }
                            
                            var gestaoContratoId = Guid.NewGuid().ToString();

                            const string qGestao = @"
                                INSERT INTO tb_parceiros_gestao_contratos
                                (id, tb_parceiros_id, inicio_contrato, fim_contrato, nr_pagina, cd_contrato,
                                 cd_contrato_anterior, clausula_penalidade, ds_reajuste_anual, cd_status)
                                VALUES
                                (@Id, @ParceiroId, @InicioContrato, @FimContrato, @NumeroPagina, @Contrato,
                                 @ContratoAnt, @ClausulaPenalidade, @ReajusteAnual, @Status);
                            ";

                            await connection.ExecuteAsync(qGestao, new
                            {
                                Id = gestaoContratoId,
                                ParceiroId = parceiroId,
                                InicioContrato = inicioContrato,
                                FimContrato = fimContrato,
                                NumeroPagina = numeroPaginas,
                                Contrato = nomeContrato,
                                ContratoAnt = cdContratoAnt,
                                ClausulaPenalidade = clausulaPenal,
                                ReajusteAnual = reajusteAnual,
                                Status = EnumStatusContrato.Andamento.ToString()
                            }, tx);

                            // ─── 5. INSERIR EMAILS DE NOTIFICAÇÃO ───
                            if (!string.IsNullOrWhiteSpace(emailsNotificacao))
                            {
                                // Divide os emails por vírgula e remove espaços em branco
                                var listaEmails = emailsNotificacao
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(e => e.Trim())
                                    .Where(e => !string.IsNullOrWhiteSpace(e))
                                    .Distinct() // Remove emails duplicados
                                    .ToList();

                                const string qEmailNotificacao = @"
                                    INSERT INTO tb_notificacao_contratos_vencidos_destinatarios
                                    (tb_parceiros_gestao_contratos_id, email)
                                    VALUES
                                    (@GestaoContratoId, @Email);
                                ";

                                foreach (var email in listaEmails)
                                {
                                    // Validação básica de tamanho (máximo 255 caracteres)
                                    if (email.Length > 255)
                                    {
                                        mensagens.Add($"Linha {row}: Email '{email.Substring(0, 50)}...' excede 255 caracteres e foi ignorado.");
                                        continue;
                                    }

                                    await connection.ExecuteAsync(qEmailNotificacao, new
                                    {
                                        GestaoContratoId = gestaoContratoId,
                                        Email = email
                                    }, tx);
                                }
                            }

                            await tx.CommitAsync();
                            sucesso++;
                            mensagens.Add($"Linha {row}: Parceiro '{nomeEmpresa}' importado com sucesso.");
                        }
                        catch (Exception ex)
                        {
                            await tx.RollbackAsync();
                            erros++;
                            mensagens.Add($"Linha {row}: Erro ao importar - {ex.Message}");
                        }
                    }
                }

                return (sucesso, erros, mensagens);
            }
            catch (Exception ex)
            {
                mensagens.Add($"Erro geral na importação: {ex.Message}");
                return (sucesso, erros + 1, mensagens);
            }
        }
    }
}

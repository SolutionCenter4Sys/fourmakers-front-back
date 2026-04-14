using Colaboracao.Core.Impl;
using Core.Domain.Colaborador;
using Dapper;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Endereco;
using Colaborador.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class ColaboradorDapperRepository : IColaboradorDapperRepository
    {
        public async Task<EditarColaboradorDTO> ObterColaboradorPorCodigoAsync(string codigoInternoColaborador)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        tc.codigo_interno_colaborador as CodigoInternoColaborador,
                        tc.nome_completo as NomeCompleto,
                        tc.data_nascimento as DataNascimento,
                        tc.rg as Rg,
                        tc.matricula as Matricula,
                        tc.endereco_id as EnderecoId,
                        tc.ativo as Ativo,
                        tc.contato_principal_ddi as ContatoPrincipalDdi,
                        tc.contato_principal as ContatoPrincipal,
                        tc.contato_outro as ContatoOutro,
                        tc.imagem_id as ImagemId,
                        tc.candidato as Candidato,
                        tc.passaporte as Passaporte,
                        tc.colaborador_saude_id as ColaboradorSaudeId,
                        tc.estado_civil as EstadoCivil,
                        tc.genero as Genero,
                        tc.etnia as Etnia,
                        tc.orientacao_sexual as OrientacaoSexual,
                        tc.escolaridade as Escolaridade,
                        tc.refugiado as Refugiado,
                        tc.email_alternativo as EmailAlternativo,
                        (SELECT tu.email FROM tb_usuario tu WHERE tu.codigo_interno_colaborador = tc.codigo_interno_colaborador AND tu.ativo = 1 ORDER BY tu.id DESC LIMIT 1) as Email,
                        tc.nacionalidade as Nacionalidade,
                        tcs.descricao as Sobre,
                        tc.documento_colaborador as DocumentoColaborador,
                        tc.url_linkedin as UrlLinkedin,
                        tc.data_sync_linkedin as DataSyncLinkedin,
                        tc.visualizar_busca_aderencia as VisualizarBuscaAderencia,
                        tc.qualificado as Qualificado,
                        ultima_cand.pretensao_salarial as UltimaPretensaoSalarial,
                        ultima_cand.tb_modelo_trabalho_id as UltimoModeloTrabalhoId,
                        ultima_cand.modelo_trabalho_descricao as UltimoModeloTrabalhoDescricao
                    FROM tb_colaborador tc
                    LEFT JOIN tb_colaborador_sobre tcs ON tcs.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    LEFT JOIN (
                        SELECT cv.pretensao_salarial,
                               cv.tb_modelo_trabalho_id,
                               mt.descricao as modelo_trabalho_descricao
                        FROM tb_candidato_vaga cv
                        LEFT JOIN tb_modelo_trabalho mt ON mt.id = cv.tb_modelo_trabalho_id
                        WHERE cv.tb_colaborador_codigo_interno_colaborador = @CodigoInternoColaborador
                          AND cv.ativo = 1
                        ORDER BY cv.data_criacao DESC, cv.id DESC
                        LIMIT 1
                    ) ultima_cand ON 1=1
                    WHERE tc.codigo_interno_colaborador = @CodigoInternoColaborador";

                var colaborador = await connection.QueryFirstOrDefaultAsync<EditarColaboradorDTO>(query, new { CodigoInternoColaborador = codigoInternoColaborador });

                if (colaborador == null)
                {
                    return null;
                }

                if (colaborador.EnderecoId.HasValue)
                {
                    var queryEndereco = @"
                        SELECT
	                        te.id AS Id,
                            te.cep AS Cep,
                            te.endereco AS Endereco,
                            te.complemento AS Complemento,
                            te.numero AS Numero,
                            te.bairro AS Bairro,
                            te.cidade AS Cidade,
                            te.estado AS Estado,
                            te.com_quem_mora AS ComQuemMora,
                            te.internacional_linha_um AS InternacionalLinhaUm,
                            te.internacional_linha_dois AS InternacionalLinhaDois
                        FROM
                            tb_endereco te
                        WHERE
                            te.id = @EnderecoId;";

                    colaborador.Endereco = await connection.QueryFirstOrDefaultAsync<EnderecoDTO?>(queryEndereco, new { colaborador.EnderecoId });
                }

                if (colaborador.ColaboradorSaudeId.HasValue && colaborador.ColaboradorSaudeId.Value > 0)
                {
                    var querySaude = @"
                        SELECT
                            tcs.pcd AS Pcd,
                            tcs.grupo_risco_covid AS GrupoRiscoCovid,
                            tcs.condicao_saude_relevante AS CondicaoSaudeRelevante
                        FROM
                            tb_colaborador_saude tcs
                        WHERE
                            tcs.id = @ColaboradorSaudeId;";

                    var saudeData = await connection.QueryFirstOrDefaultAsync<dynamic>(querySaude, new { colaborador.ColaboradorSaudeId });

                    if (saudeData != null)
                    {
                        colaborador.Saude = new ColaboradorSaudeDTO
                        {
                            PCD = ConverterPCDDoBanco(saudeData.Pcd),
                            GrupoDeRiscoCovid = saudeData.GrupoRiscoCovid ?? (sbyte)0,
                            CondicaoDeSaudeRelevante = saudeData.CondicaoSaudeRelevante ?? string.Empty
                        };
                    }
                }
                else
                {
                    colaborador.Saude = new ColaboradorSaudeDTO
                    {
                        PCD = ConverterPCDDoBanco(""),
                        GrupoDeRiscoCovid = (sbyte)0
                    };
                }

                return colaborador;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<EditarColaboradorDTO> AtualizarColaboradorComLogAsync(EditarColaboradorDTO colaborador, EditarColaboradorDTO colaboradorAnterior, string codigoInternoColaboradorAlterador)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();
                using var transaction = await connection.BeginTransactionAsync();

                try
                {
                    // 1 Atualizar ou inserir o endereco na tabela tb_endereco
                    long? enderecoId = await connection.QuerySingleOrDefaultAsync<long?>(@" SELECT 
	                                                                                            tc.endereco_id 
                                                                                            FROM tb_colaborador tc
                                                                                            WHERE tc.codigo_interno_colaborador = @CodigoInternoColaborador",
                                                                                            new { colaborador.CodigoInternoColaborador },
                                                                                            transaction: transaction);

                    if (colaborador.Endereco != null)
                    {
                        if (enderecoId.HasValue)
                        {
                            colaborador.Endereco.Id = enderecoId;
                            colaborador.EnderecoId = enderecoId;

                            // Atualizar endereço existente
                            var enderecoUpdateQuery = @"
                                    UPDATE tb_endereco SET
                                        cep = @Cep,
                                        endereco = @Endereco,
                                        numero = @Numero,
                                        complemento = @Complemento,
                                        bairro = @Bairro,
                                        cidade = @Cidade,
                                        estado = @Estado,
                                        com_quem_mora = @ComQuemMora,
                                        internacional_linha_um = @InternacionalLinhaUm,
                                        internacional_linha_dois = @InternacionalLinhaDois,
                                        data_alteracao = NOW()
                                    WHERE id = @Id";

                            await connection.ExecuteAsync(enderecoUpdateQuery, colaborador.Endereco, transaction);
                        }
                        else
                        {
                            // Inserir novo endereço
                            var enderecoInsertQuery = @"
                                    INSERT INTO tb_endereco (
                                        cep,
                                        endereco,
                                        numero,
                                        complemento,
                                        bairro,
                                        cidade,
                                        estado,
                                        com_quem_mora,
                                        internacional_linha_um,
                                        internacional_linha_dois,
                                        ativo,
                                        data_criacao,
                                        data_alteracao
                                    ) VALUES (
                                        @Cep,
                                        @Endereco,
                                        @Numero,
                                        @Complemento,
                                        @Bairro,
                                        @Cidade,
                                        @Estado,
                                        @ComQuemMora,
                                        @InternacionalLinhaUm,
                                        @InternacionalLinhaDois,
                                        1,
                                        NOW(),
                                        NOW()
                                    )";

                            await connection.ExecuteAsync(enderecoInsertQuery, colaborador.Endereco, transaction);
                            enderecoId = await connection.QuerySingleAsync<long>("SELECT LAST_INSERT_ID()", transaction: transaction);
                            colaborador.Endereco.Id = enderecoId;
                            colaborador.EnderecoId = enderecoId;
                        }
                    }

                    // Processar dados de saúde se o objeto Saude foi enviado
                    if (colaborador.Saude != null)
                    {
                        // Obter o colaborador_saude_id atual do banco
                        var colaboradorSaudeIdAtual = await connection.QuerySingleOrDefaultAsync<int?>(
                            @"SELECT colaborador_saude_id FROM tb_colaborador WHERE codigo_interno_colaborador = @CodigoInternoColaborador",
                            new { colaborador.CodigoInternoColaborador },
                            transaction: transaction);

                        // Converter PCD para formato do banco
                        string pcdParaBanco = ConverterPCDParaBanco(colaborador.Saude.PCD);

                        if (colaboradorSaudeIdAtual.HasValue && colaboradorSaudeIdAtual.Value > 0)
                        {
                            // Atualizar registro existente na tb_colaborador_saude
                            var updateSaudeQuery = @"
                                UPDATE tb_colaborador_saude SET
                                    pcd = @Pcd,
                                    grupo_risco_covid = @GrupoRiscoCovid,
                                    condicao_saude_relevante = @CondicaoSaudeRelevante
                                WHERE id = @Id";

                            await connection.ExecuteAsync(updateSaudeQuery, new
                            {
                                Pcd = pcdParaBanco,
                                GrupoRiscoCovid = colaborador.Saude.GrupoDeRiscoCovid,
                                CondicaoSaudeRelevante = colaborador.Saude.CondicaoDeSaudeRelevante ?? string.Empty,
                                Id = colaboradorSaudeIdAtual.Value
                            }, transaction: transaction);

                            colaborador.ColaboradorSaudeId = colaboradorSaudeIdAtual;
                        }
                        else
                        {
                            // Criar novo registro na tb_colaborador_saude
                            var insertSaudeQuery = @"
                                INSERT INTO tb_colaborador_saude (pcd, grupo_risco_covid, condicao_saude_relevante)
                                VALUES (@Pcd, @GrupoRiscoCovid, @CondicaoSaudeRelevante)";

                            await connection.ExecuteAsync(insertSaudeQuery, new
                            {
                                Pcd = pcdParaBanco,
                                GrupoRiscoCovid = colaborador.Saude.GrupoDeRiscoCovid,
                                CondicaoSaudeRelevante = colaborador.Saude.CondicaoDeSaudeRelevante ?? string.Empty
                            }, transaction: transaction);

                            var novoId = await connection.QuerySingleAsync<int>("SELECT LAST_INSERT_ID()", transaction: transaction);
                            colaborador.ColaboradorSaudeId = novoId;
                        }
                    }
                    else
                    {
                        // Se Saude não foi enviado, manter o colaborador_saude_id atual do banco
                        var colaboradorSaudeIdAtual = await connection.QuerySingleOrDefaultAsync<int?>(
                            @"SELECT colaborador_saude_id FROM tb_colaborador WHERE codigo_interno_colaborador = @CodigoInternoColaborador",
                            new { colaborador.CodigoInternoColaborador },
                            transaction: transaction);

                        colaborador.ColaboradorSaudeId = colaboradorSaudeIdAtual;
                    }

                    // 1.1 Atualizar o colaborador
                    var updateQuery = @"
                        UPDATE tb_colaborador SET
                            nome_completo = @NomeCompleto,
                            data_nascimento = @DataNascimento,
                            rg = @Rg,
                            matricula = @Matricula,
                            endereco_id = @EnderecoId,
                            ativo = @Ativo,
                            contato_principal_ddi = @ContatoPrincipalDdi,
                            contato_principal = @ContatoPrincipal,
                            contato_outro = @ContatoOutro,
                            imagem_id = @ImagemId,
                            candidato = @Candidato,
                            passaporte = @Passaporte,
                            colaborador_saude_id = @ColaboradorSaudeId,
                            estado_civil = @EstadoCivil,
                            genero = @Genero,
                            etnia = @Etnia,
                            orientacao_sexual = @OrientacaoSexual,
                            escolaridade = @Escolaridade,
                            refugiado = @Refugiado,
                            email_alternativo = @EmailAlternativo,
                            nacionalidade = @Nacionalidade,
                            documento_colaborador = @DocumentoColaborador,
                            url_linkedin = @UrlLinkedin,
                            data_sync_linkedin = @DataSyncLinkedin,
                            visualizar_busca_aderencia = @VisualizarBuscaAderencia,
                            qualificado = @Qualificado,
                            data_alteracao = CURRENT_TIMESTAMP
                        WHERE codigo_interno_colaborador = @CodigoInternoColaborador";

                    await connection.ExecuteAsync(updateQuery, colaborador, transaction);

                    // 1.2 Atualizar ou inserir o campo "sobre" na tabela tb_colaborador_sobre
                    var verificarSobreQuery = @"
                        SELECT COUNT(*) 
                        FROM tb_colaborador_sobre 
                        WHERE codigo_interno_colaborador = @CodigoInternoColaborador";
                    
                    var existeSobre = await connection.ExecuteScalarAsync<int>(verificarSobreQuery, new { CodigoInternoColaborador = colaborador.CodigoInternoColaborador }, transaction) > 0;
                    
                    if (existeSobre)
                    {
                        var updateSobreQuery = @"
                            UPDATE tb_colaborador_sobre 
                            SET descricao = @Sobre,
                                data_criacao = CURRENT_TIMESTAMP
                            WHERE codigo_interno_colaborador = @CodigoInternoColaborador";
                        
                        await connection.ExecuteAsync(updateSobreQuery, new 
                        { 
                            Sobre = colaborador.Sobre,
                            CodigoInternoColaborador = colaborador.CodigoInternoColaborador 
                        }, transaction);
                    }
                    else if (!string.IsNullOrEmpty(colaborador.Sobre))
                    {
                        var insertSobreQuery = @"
                            INSERT INTO tb_colaborador_sobre (
                                codigo_interno_colaborador, 
                                descricao, 
                                data_criacao
                            ) VALUES (
                                @CodigoInternoColaborador, 
                                @Sobre, 
                                CURRENT_TIMESTAMP
                            )";
                        
                        await connection.ExecuteAsync(insertSobreQuery, new 
                        { 
                            CodigoInternoColaborador = colaborador.CodigoInternoColaborador,
                            Sobre = colaborador.Sobre
                        }, transaction);
                    }

                    // 2. Criar o log da alteração
                    var log = new ColaboradorLogDTO
                    {
                        Id = Guid.NewGuid(),
                        ColaboradorCodigoInternoColaborador = colaborador.CodigoInternoColaborador,
                        Acao = "UPDATE",
                        ColaboradorCodigoInternoColaboradorAlterador = codigoInternoColaboradorAlterador,
                        DataAlteracao = DateTime.Now,
                        Objeto = System.Text.Json.JsonSerializer.Serialize(colaboradorAnterior),
                        Alteracoes = System.Text.Json.JsonSerializer.Serialize(colaborador)
                    };

                    var logQuery = @"
                        INSERT INTO tb_colaborador_log 
                        (id, tb_colaborador_codigo_interno_colaborador, acao, tb_colaborador_codigo_interno_colaborador_alterador, 
                         data_alteracao, objeto, alteracoes)
                        VALUES 
                        (@Id, @ColaboradorCodigoInternoColaborador, @Acao, @ColaboradorCodigoInternoColaboradorAlterador, 
                         @DataAlteracao, @Objeto, @Alteracoes)";

                    await connection.ExecuteAsync(logQuery, log, transaction);

                    // 3. Commit da transação
                    await transaction.CommitAsync();

                    return colaborador;
                }
                catch (Exception)
                {
                    // Rollback em caso de erro
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<bool> ExisteColaboradorAsync(string codigoInternoColaborador)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT COUNT(1) 
                    FROM tb_colaborador 
                    WHERE codigo_interno_colaborador = @CodigoInternoColaborador";

                var count = await connection.QuerySingleAsync<int>(query, new { CodigoInternoColaborador = codigoInternoColaborador });
                return count > 0;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }


        public async Task<List<ColaboradorLogDTO>> ListarLogsPorColaboradorAsync(string codigoInternoColaborador, int limite, int cursor)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id as Id,
                        tb_colaborador_codigo_interno_colaborador as ColaboradorCodigoInternoColaborador,
                        acao as Acao,
                        tb_colaborador_codigo_interno_colaborador_alterador as ColaboradorCodigoInternoColaboradorAlterador,
                        data_alteracao as DataAlteracao,
                        objeto as Objeto,
                        alteracoes as Alteracoes
                    FROM tb_colaborador_log 
                    WHERE tb_colaborador_codigo_interno_colaborador = @CodigoInternoColaborador
                    ORDER BY data_alteracao DESC
                    LIMIT @Limite OFFSET @Cursor";

                var logs = await connection.QueryAsync<ColaboradorLogDTO>(query, new 
                { 
                    CodigoInternoColaborador = codigoInternoColaborador,
                    Limite = limite,
                    Cursor = cursor
                });

                return logs.ToList();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<List<ColaboradorLogDTO>> ListarLogsPorAlteradorAsync(string codigoInternoColaboradorAlterador, int limite, int cursor)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id as Id,
                        tb_colaborador_codigo_interno_colaborador as ColaboradorCodigoInternoColaborador,
                        acao as Acao,
                        tb_colaborador_codigo_interno_colaborador_alterador as ColaboradorCodigoInternoColaboradorAlterador,
                        data_alteracao as DataAlteracao,
                        objeto as Objeto,
                        alteracoes as Alteracoes
                    FROM tb_colaborador_log 
                    WHERE tb_colaborador_codigo_interno_colaborador_alterador = @CodigoInternoColaboradorAlterador
                    ORDER BY data_alteracao DESC
                    LIMIT @Limite OFFSET @Cursor";

                var logs = await connection.QueryAsync<ColaboradorLogDTO>(query, new 
                { 
                    CodigoInternoColaboradorAlterador = codigoInternoColaboradorAlterador,
                    Limite = limite,
                    Cursor = cursor
                });

                return logs.ToList();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<List<ColaboradorLogDTO>> ListarLogsPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, int limite, int cursor)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id as Id,
                        tb_colaborador_codigo_interno_colaborador as ColaboradorCodigoInternoColaborador,
                        acao as Acao,
                        tb_colaborador_codigo_interno_colaborador_alterador as ColaboradorCodigoInternoColaboradorAlterador,
                        data_alteracao as DataAlteracao,
                        objeto as Objeto,
                        alteracoes as Alteracoes
                    FROM tb_colaborador_log 
                    WHERE data_alteracao BETWEEN @DataInicio AND @DataFim
                    ORDER BY data_alteracao DESC
                    LIMIT @Limite OFFSET @Cursor";

                var logs = await connection.QueryAsync<ColaboradorLogDTO>(query, new 
                { 
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    Limite = limite,
                    Cursor = cursor
                });

                return logs.ToList();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        private EnumPCD ConverterPCDDoBanco(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return EnumPCD.Nenhuma;

            if (valor == "Psicossocial / Mental")
                return EnumPCD.PsicossocialMental;

            // Para todos os outros valores usa o TryParse normal
            if (Enum.TryParse(valor, out EnumPCD pcd))
                return pcd;

            // Se não conseguir converter, retorne um padrão
            return EnumPCD.Nenhuma;
        }

        private string ConverterPCDParaBanco(EnumPCD pcd)
        {
            if (pcd == EnumPCD.PsicossocialMental)
                return "Psicossocial / Mental";

            return pcd.ToString();
        }

        public async Task InserirDadosDemograficosAsync(DadosDemograficosColaboradorDTO dadosDemograficos, string codigoInternoColaborador)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO tb_colaborador_dados_demograficos (
                        id,
                        codigo_interno_colaborador,
                        quantidade_pessoas_residencia,
                        dependentes_irpf,
                        possui_conjuge,
                        data_nascimento_conjuge,
                        possui_filhos,
                        possui_seguro_saude,
                        valor_atual_seguro_saude,
                        operadora_seguro_saude,
                        acomodacao_seguro_saude,
                        seguro_saude_possui_coparticipacao,
                        observacoes_seguro_saude,
                        possui_interesse_plano_foursys,
                        faixa_etaria,
                        categoria_plano_saude,
                        incluir_dependentes_plano_foursys,
                        quantidade_dependentes_plano_foursys,
                        valor_plano_dependentes,
                        valor_cartao_refeicao,
                        valor_cartao_alimentacao,
                        estuda_atualmente,
                        custo_mensal_educacao,
                        filhos_estudam_ate_24_anos,
                        custo_mensal_educacao_filhos,
                        custo_total_educacao,
                        distancia_ida_volta,
                        data_criacao,
                        data_alteracao
                    ) VALUES (
                        @Id,
                        @CodigoInternoColaborador,
                        @QuantidadePessoasResidencia,
                        @DependentesIRPF,
                        @PossuiConjuge,
                        @DataNascimentoConjuge,
                        @PossuiFilhos,
                        @PossuiSeguroSaude,
                        @ValorAtualSeguroSaude,
                        @OperadoraSeguroSaude,
                        @AcomodacaoSeguroSaude,
                        @SeguroSaudePossuiCoparticipacao,
                        @ObservacoesSeguroSaude,
                        @PossuiInteressePlanoFoursys,
                        @FaixaEtaria,
                        @CategoriaPlanoSaude,
                        @IncluirDependentesPlanoFoursys,
                        @QuantidadeDependentesPlanoFoursys,
                        @ValorPlanoDependentes,
                        @ValorCartaoRefeicao,
                        @ValorCartaoAlimentacao,
                        @EstudaAtualmente,
                        @CustoMensalEducacao,
                        @FilhosEstudamAte24Anos,
                        @CustoMensalEducacaoFilhos,
                        @CustoTotalEducacao,
                        @DistanciaIdaVolta,
                        NOW(),
                        NOW()
                    )";

                var dadosDemograficosId = Guid.NewGuid().ToString();
                var parameters = new
                {
                    Id = dadosDemograficosId,
                    CodigoInternoColaborador = codigoInternoColaborador,
                    QuantidadePessoasResidencia = dadosDemograficos.QuantidadePessoasResidencia,
                    DependentesIRPF = dadosDemograficos.DependentesIRPF,
                    PossuiConjuge = dadosDemograficos.PossuiConjuge,
                    DataNascimentoConjuge = dadosDemograficos.DataNascimentoConjuge,
                    PossuiFilhos = dadosDemograficos.PossuiFilhos,
                    PossuiSeguroSaude = dadosDemograficos.PossuiSeguroSaude,
                    ValorAtualSeguroSaude = dadosDemograficos.ValorAtualSeguroSaude,
                    OperadoraSeguroSaude = dadosDemograficos.OperadoraSeguroSaude,
                    AcomodacaoSeguroSaude = dadosDemograficos.AcomodacaoSeguroSaude,
                    SeguroSaudePossuiCoparticipacao = dadosDemograficos.SeguroSaudePossuiCoparticipacao,
                    ObservacoesSeguroSaude = dadosDemograficos.ObservacoesSeguroSaude,
                    PossuiInteressePlanoFoursys = dadosDemograficos.PossuiInteressePlanoFoursys,
                    FaixaEtaria = dadosDemograficos.FaixaEtaria,
                    CategoriaPlanoSaude = dadosDemograficos.CategoriaPlanoSaude,
                    IncluirDependentesPlanoFoursys = dadosDemograficos.IncluirDependentesPlanoFoursys,
                    QuantidadeDependentesPlanoFoursys = dadosDemograficos.QuantidadeDependentesPlanoFoursys,
                    ValorPlanoDependentes = dadosDemograficos.ValorPlanoDependentes,
                    ValorCartaoRefeicao = dadosDemograficos.ValorCartaoRefeicao,
                    ValorCartaoAlimentacao = dadosDemograficos.ValorCartaoAlimentacao,
                    EstudaAtualmente = dadosDemograficos.EstudaAtualmente,
                    CustoMensalEducacao = dadosDemograficos.CustoMensalEducacao,
                    FilhosEstudamAte24Anos = dadosDemograficos.FilhosEstudamAte24Anos,
                    CustoMensalEducacaoFilhos = dadosDemograficos.CustoMensalEducacaoFilhos,
                    CustoTotalEducacao = dadosDemograficos.CustoTotalEducacao,
                    DistanciaIdaVolta = dadosDemograficos.DistanciaIdaVolta
                };

                await connection.ExecuteAsync(query, parameters);

                // Inserir filhos na tabela separada
                if (dadosDemograficos.PossuiFilhos && dadosDemograficos.Filhos != null && dadosDemograficos.Filhos.Any())
                {
                    var queryFilhos = @"
                        INSERT INTO tb_colaborador_filhos (
                            id,
                            codigo_interno_colaborador,
                            data_nascimento,
                            data_criacao,
                            data_alteracao
                        ) VALUES (
                            @Id,
                            @CodigoInternoColaborador,
                            @DataNascimento,
                            NOW(),
                            NOW()
                        )";

                    foreach (var filho in dadosDemograficos.Filhos)
                    {
                        if (filho.DataNascimento.HasValue)
                        {
                            // Gerar GUID se não foi informado
                            var filhoId = filho.Id ?? Guid.NewGuid();
                            
                            await connection.ExecuteAsync(queryFilhos, new
                            {
                                Id = filhoId.ToString(),
                                CodigoInternoColaborador = codigoInternoColaborador,
                                DataNascimento = filho.DataNascimento.Value
                            });
                        }
                    }
                }

                // Inserir outros custos na tabela separada
                if (dadosDemograficos.OutrosCustos != null && dadosDemograficos.OutrosCustos.Any())
                {
                    var queryOutrosCustos = @"
                        INSERT INTO tb_colaborador_dados_demograficos_outros_custos (
                            id,
                            codigo_interno_colaborador,
                            tb_colaborador_dados_demograficos_id,
                            descricao,
                            valor,
                            data_criacao,
                            data_alteracao
                        ) VALUES (
                            @Id,
                            @CodigoInternoColaborador,
                            @TbColaboradorDadosDemograficosId,
                            @Descricao,
                            @Valor,
                            NOW(),
                            NOW()
                        )";

                    foreach (var outroCusto in dadosDemograficos.OutrosCustos)
                    {
                        // Gerar GUID se não foi informado
                        var outroCustoId = string.IsNullOrEmpty(outroCusto.Id) ? Guid.NewGuid().ToString() : outroCusto.Id;
                        
                        await connection.ExecuteAsync(queryOutrosCustos, new
                        {
                            Id = outroCustoId,
                            CodigoInternoColaborador = codigoInternoColaborador,
                            TbColaboradorDadosDemograficosId = dadosDemograficosId,
                            Descricao = outroCusto.Descricao,
                            Valor = outroCusto.Valor
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inserir dados demográficos: {ex.Message}", ex);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task AlterarDadosDemograficosAsync(DadosDemograficosColaboradorDTO dadosDemograficos, string codigoInternoColaborador)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    UPDATE tb_colaborador_dados_demograficos SET
                        quantidade_pessoas_residencia = @QuantidadePessoasResidencia,
                        dependentes_irpf = @DependentesIRPF,
                        possui_conjuge = @PossuiConjuge,
                        data_nascimento_conjuge = @DataNascimentoConjuge,
                        possui_filhos = @PossuiFilhos,
                        possui_seguro_saude = @PossuiSeguroSaude,
                        valor_atual_seguro_saude = @ValorAtualSeguroSaude,
                        operadora_seguro_saude = @OperadoraSeguroSaude,
                        acomodacao_seguro_saude = @AcomodacaoSeguroSaude,
                        seguro_saude_possui_coparticipacao = @SeguroSaudePossuiCoparticipacao,
                        observacoes_seguro_saude = @ObservacoesSeguroSaude,
                        possui_interesse_plano_foursys = @PossuiInteressePlanoFoursys,
                        faixa_etaria = @FaixaEtaria,
                        categoria_plano_saude = @CategoriaPlanoSaude,
                        incluir_dependentes_plano_foursys = @IncluirDependentesPlanoFoursys,
                        quantidade_dependentes_plano_foursys = @QuantidadeDependentesPlanoFoursys,
                        valor_plano_dependentes = @ValorPlanoDependentes,
                        valor_cartao_refeicao = @ValorCartaoRefeicao,
                        valor_cartao_alimentacao = @ValorCartaoAlimentacao,
                        estuda_atualmente = @EstudaAtualmente,
                        custo_mensal_educacao = @CustoMensalEducacao,
                        filhos_estudam_ate_24_anos = @FilhosEstudamAte24Anos,
                        custo_mensal_educacao_filhos = @CustoMensalEducacaoFilhos,
                        custo_total_educacao = @CustoTotalEducacao,
                        distancia_ida_volta = @DistanciaIdaVolta,
                        data_alteracao = NOW()
                    WHERE codigo_interno_colaborador = @CodigoInternoColaborador";

                var parameters = new
                {
                    CodigoInternoColaborador = codigoInternoColaborador,
                    QuantidadePessoasResidencia = dadosDemograficos.QuantidadePessoasResidencia,
                    DependentesIRPF = dadosDemograficos.DependentesIRPF,
                    PossuiConjuge = dadosDemograficos.PossuiConjuge,
                    DataNascimentoConjuge = dadosDemograficos.DataNascimentoConjuge,
                    PossuiFilhos = dadosDemograficos.PossuiFilhos,
                    PossuiSeguroSaude = dadosDemograficos.PossuiSeguroSaude,
                    ValorAtualSeguroSaude = dadosDemograficos.ValorAtualSeguroSaude,
                    OperadoraSeguroSaude = dadosDemograficos.OperadoraSeguroSaude,
                    AcomodacaoSeguroSaude = dadosDemograficos.AcomodacaoSeguroSaude,
                    SeguroSaudePossuiCoparticipacao = dadosDemograficos.SeguroSaudePossuiCoparticipacao,
                    ObservacoesSeguroSaude = dadosDemograficos.ObservacoesSeguroSaude,
                    PossuiInteressePlanoFoursys = dadosDemograficos.PossuiInteressePlanoFoursys,
                    FaixaEtaria = dadosDemograficos.FaixaEtaria,
                    CategoriaPlanoSaude = dadosDemograficos.CategoriaPlanoSaude,
                    IncluirDependentesPlanoFoursys = dadosDemograficos.IncluirDependentesPlanoFoursys,
                    QuantidadeDependentesPlanoFoursys = dadosDemograficos.QuantidadeDependentesPlanoFoursys,
                    ValorPlanoDependentes = dadosDemograficos.ValorPlanoDependentes,
                    ValorCartaoRefeicao = dadosDemograficos.ValorCartaoRefeicao,
                    ValorCartaoAlimentacao = dadosDemograficos.ValorCartaoAlimentacao,
                    EstudaAtualmente = dadosDemograficos.EstudaAtualmente,
                    CustoMensalEducacao = dadosDemograficos.CustoMensalEducacao,
                    FilhosEstudamAte24Anos = dadosDemograficos.FilhosEstudamAte24Anos,
                    CustoMensalEducacaoFilhos = dadosDemograficos.CustoMensalEducacaoFilhos,
                    CustoTotalEducacao = dadosDemograficos.CustoTotalEducacao,
                    DistanciaIdaVolta = dadosDemograficos.DistanciaIdaVolta
                };

                var rowsAffected = await connection.ExecuteAsync(query, parameters);
                
                if (rowsAffected == 0)
                {
                    throw new Exception("Dados demográficos não encontrados para atualização. Use o método de inserção primeiro.");
                }

                // Atualizar filhos: deletar todos e inserir novamente
                await connection.ExecuteAsync(
                    "DELETE FROM tb_colaborador_filhos WHERE codigo_interno_colaborador = @CodigoInternoColaborador",
                    new { CodigoInternoColaborador = codigoInternoColaborador }
                );

                // Inserir filhos atualizados
                if (dadosDemograficos.PossuiFilhos && dadosDemograficos.Filhos != null && dadosDemograficos.Filhos.Any())
                {
                    var queryFilhos = @"
                        INSERT INTO tb_colaborador_filhos (
                            id,
                            codigo_interno_colaborador,
                            data_nascimento,
                            data_criacao,
                            data_alteracao
                        ) VALUES (
                            @Id,
                            @CodigoInternoColaborador,
                            @DataNascimento,
                            NOW(),
                            NOW()
                        )";

                    foreach (var filho in dadosDemograficos.Filhos)
                    {
                        if (filho.DataNascimento.HasValue)
                        {
                            // Gerar GUID se não foi informado
                            var filhoId = filho.Id ?? Guid.NewGuid();
                            
                            await connection.ExecuteAsync(queryFilhos, new
                            {
                                Id = filhoId.ToString(),
                                CodigoInternoColaborador = codigoInternoColaborador,
                                DataNascimento = filho.DataNascimento.Value
                            });
                        }
                    }
                }

                // Obter ID dos dados demográficos
                var dadosDemograficosId = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT id FROM tb_colaborador_dados_demograficos WHERE codigo_interno_colaborador = @CodigoInternoColaborador",
                    new { CodigoInternoColaborador = codigoInternoColaborador }
                );

                // Atualizar outros custos: deletar todos e inserir novamente
                await connection.ExecuteAsync(
                    "DELETE FROM tb_colaborador_dados_demograficos_outros_custos WHERE codigo_interno_colaborador = @CodigoInternoColaborador",
                    new { CodigoInternoColaborador = codigoInternoColaborador }
                );

                // Inserir outros custos atualizados
                if (dadosDemograficos.OutrosCustos != null && dadosDemograficos.OutrosCustos.Any() && !string.IsNullOrEmpty(dadosDemograficosId))
                {
                    var queryOutrosCustos = @"
                        INSERT INTO tb_colaborador_dados_demograficos_outros_custos (
                            id,
                            codigo_interno_colaborador,
                            tb_colaborador_dados_demograficos_id,
                            descricao,
                            valor,
                            data_criacao,
                            data_alteracao
                        ) VALUES (
                            @Id,
                            @CodigoInternoColaborador,
                            @TbColaboradorDadosDemograficosId,
                            @Descricao,
                            @Valor,
                            NOW(),
                            NOW()
                        )";

                    foreach (var outroCusto in dadosDemograficos.OutrosCustos)
                    {
                        // Gerar GUID se não foi informado
                        var outroCustoId = string.IsNullOrEmpty(outroCusto.Id) ? Guid.NewGuid().ToString() : outroCusto.Id;
                        
                        await connection.ExecuteAsync(queryOutrosCustos, new
                        {
                            Id = outroCustoId,
                            CodigoInternoColaborador = codigoInternoColaborador,
                            TbColaboradorDadosDemograficosId = dadosDemograficosId,
                            Descricao = outroCusto.Descricao,
                            Valor = outroCusto.Valor
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar dados demográficos: {ex.Message}", ex);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<DadosDemograficosColaboradorDTO> ObterDadosDemograficosAsync(string codigoInternoColaborador)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        codigo_interno_colaborador as CodigoInternoColaborador,
                        quantidade_pessoas_residencia as QuantidadePessoasResidencia,
                        dependentes_irpf as DependentesIRPF,
                        possui_conjuge as PossuiConjuge,
                        data_nascimento_conjuge as DataNascimentoConjuge,
                        possui_filhos as PossuiFilhos,
                        possui_seguro_saude as PossuiSeguroSaude,
                        valor_atual_seguro_saude as ValorAtualSeguroSaude,
                        operadora_seguro_saude as OperadoraSeguroSaude,
                        acomodacao_seguro_saude as AcomodacaoSeguroSaude,
                        seguro_saude_possui_coparticipacao as SeguroSaudePossuiCoparticipacao,
                        observacoes_seguro_saude as ObservacoesSeguroSaude,
                        possui_interesse_plano_foursys as PossuiInteressePlanoFoursys,
                        faixa_etaria as FaixaEtaria,
                        categoria_plano_saude as CategoriaPlanoSaude,
                        incluir_dependentes_plano_foursys as IncluirDependentesPlanoFoursys,
                        quantidade_dependentes_plano_foursys as QuantidadeDependentesPlanoFoursys,
                        valor_plano_dependentes as ValorPlanoDependentes,
                        valor_cartao_refeicao as ValorCartaoRefeicao,
                        valor_cartao_alimentacao as ValorCartaoAlimentacao,
                        estuda_atualmente as EstudaAtualmente,
                        custo_mensal_educacao as CustoMensalEducacao,
                        filhos_estudam_ate_24_anos as FilhosEstudamAte24Anos,
                        custo_mensal_educacao_filhos as CustoMensalEducacaoFilhos,
                        custo_total_educacao as CustoTotalEducacao,
                        distancia_ida_volta as DistanciaIdaVolta
                    FROM tb_colaborador_dados_demograficos
                    WHERE codigo_interno_colaborador = @CodigoInternoColaborador";

                var result = await connection.QueryFirstOrDefaultAsync<DadosDemograficosResult>(query, new { CodigoInternoColaborador = codigoInternoColaborador });

                if (result == null)
                {
                    return null;
                }

                // Buscar filhos da tabela separada
                var queryFilhos = @"
                    SELECT 
                        id as Id,
                        data_nascimento as DataNascimento
                    FROM tb_colaborador_filhos
                    WHERE codigo_interno_colaborador = @CodigoInternoColaborador
                    ORDER BY data_nascimento ASC";

                var filhosRaw = await connection.QueryAsync<(string Id, DateTime? DataNascimento)>(queryFilhos, new { CodigoInternoColaborador = codigoInternoColaborador });
                var filhos = filhosRaw.Select(f => new FilhoColaboradorDTO
                {
                    Id = Guid.TryParse(f.Id, out var guidId) ? guidId : (Guid?)null,
                    DataNascimento = f.DataNascimento
                }).ToList();

                // Buscar outros custos da tabela separada
                var queryOutrosCustos = @"
                    SELECT 
                        id as Id,
                        descricao as Descricao,
                        valor as Valor
                    FROM tb_colaborador_dados_demograficos_outros_custos
                    WHERE codigo_interno_colaborador = @CodigoInternoColaborador
                    ORDER BY data_criacao ASC";

                var outrosCustos = await connection.QueryAsync<OutroCustoColaboradorDTO>(queryOutrosCustos, new { CodigoInternoColaborador = codigoInternoColaborador });

                return new DadosDemograficosColaboradorDTO
                {
                    QuantidadePessoasResidencia = result.QuantidadePessoasResidencia ?? 1,
                    DependentesIRPF = result.DependentesIRPF ?? 0,
                    PossuiConjuge = result.PossuiConjuge ?? false,
                    DataNascimentoConjuge = result.DataNascimentoConjuge,
                    PossuiFilhos = result.PossuiFilhos ?? false,
                    Filhos = filhos.ToList(),
                    PossuiSeguroSaude = result.PossuiSeguroSaude ?? false,
                    ValorAtualSeguroSaude = result.ValorAtualSeguroSaude,
                    OperadoraSeguroSaude = result.OperadoraSeguroSaude,
                    AcomodacaoSeguroSaude = result.AcomodacaoSeguroSaude,
                    SeguroSaudePossuiCoparticipacao = result.SeguroSaudePossuiCoparticipacao ?? false,
                    ObservacoesSeguroSaude = result.ObservacoesSeguroSaude,
                    PossuiInteressePlanoFoursys = result.PossuiInteressePlanoFoursys ?? false,
                    FaixaEtaria = result.FaixaEtaria,
                    CategoriaPlanoSaude = result.CategoriaPlanoSaude,
                    IncluirDependentesPlanoFoursys = result.IncluirDependentesPlanoFoursys ?? false,
                    QuantidadeDependentesPlanoFoursys = result.QuantidadeDependentesPlanoFoursys ?? 0,
                    ValorPlanoDependentes = result.ValorPlanoDependentes,
                    ValorCartaoRefeicao = result.ValorCartaoRefeicao,
                    ValorCartaoAlimentacao = result.ValorCartaoAlimentacao,
                    EstudaAtualmente = result.EstudaAtualmente ?? false,
                    CustoMensalEducacao = result.CustoMensalEducacao,
                    FilhosEstudamAte24Anos = result.FilhosEstudamAte24Anos ?? false,
                    CustoMensalEducacaoFilhos = result.CustoMensalEducacaoFilhos,
                    CustoTotalEducacao = result.CustoTotalEducacao,
                    DistanciaIdaVolta = result.DistanciaIdaVolta,
                    OutrosCustos = outrosCustos.ToList()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter dados demográficos: {ex.Message}", ex);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }
}

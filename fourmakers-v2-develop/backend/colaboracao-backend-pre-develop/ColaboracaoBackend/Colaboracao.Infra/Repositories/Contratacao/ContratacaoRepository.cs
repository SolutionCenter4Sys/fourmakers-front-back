using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Contratacao;
using Dapper;
using DataTransferObject.Domain.Contratacao;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.SRS;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Contratacao
{
    public class ContratacaoRepository : IContratacaoRepository
    {
        private readonly IDBConnection _dapperConnection;
        private readonly ITemplateContratacaoLogRepository _logRepository;
        private readonly ISistemasLiberadosRepository _sistemasLiberadosRepository;
        private readonly IDiretoriosRepository _diretoriosRepository;
        private readonly IGruposEmailsRepository _gruposEmailsRepository;

        public ContratacaoRepository(IDBConnection dapperConnection, ITemplateContratacaoLogRepository logRepository, ISistemasLiberadosRepository sistemasLiberadosRepository, IDiretoriosRepository diretoriosRepository, IGruposEmailsRepository gruposEmailsRepository)
        {
            _dapperConnection = dapperConnection;
            _logRepository = logRepository;
            _sistemasLiberadosRepository = sistemasLiberadosRepository;
            _diretoriosRepository = diretoriosRepository;
            _gruposEmailsRepository = gruposEmailsRepository;
        }

        public async Task<TemplateDTO> ObterTemplatePorIdAsync(Guid id)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                        SELECT 
                            t.id as Id,
                            t.tb_colaborador_codigo_interno_colaborador_analista as ColaboradorCodigoInternoColaboradorAnalista,
                            analista.nome_completo as NomeColaboradorAnalista,
                            CAST(t.tb_colaborador_codigo_interno_colaborador AS CHAR) as ColaboradorCodigoInternoColaborador,
                            CAST(t.tb_candidato_vaga_id AS CHAR) as CandidatoVagaId,
                            t.cargo as Cargo,
                            CAST(t.tb_equipamento_padrao_cargo_funcao_id AS CHAR) as EquipamentoPadraoCargoFuncaoId,
                            grupo.nome_grupo as GrupoAreaEquipamentoPadraoCargoFuncao,
                            t.data_inicio as DataInicio,
                            t.horario_jornada as HorarioJornada,
                            t.tipo_horario_jornada as TipoHorarioJornada,
                            c.documento_colaborador as DocumentoColaborador,
                            c.rg as RgColaborador,
                            c.data_nascimento as DataNascimento,
                            c.contato_principal as ContatoPrincipal,
                            c.nome_completo as NomeCompleto,
                            t.tamanho_camiseta as TamanhoCamiseta,
                            v.maquina as DescricaoMaquina,
                            t.hardware as Hardware,
                            t.softwares_necessarios as SoftwaresNecessarios,
                            t.softwares_ec as SoftwaresEc,
                            CAST(t.tb_colaborador_codigo_interno_colaborador_superior_imediato AS CHAR) as ColaboradorCodigoInternoColaboradorSuperiorImediato,
                            superior.nome_completo as NomeColaboradorSuperiorImediato,
                            c.email_alternativo as EmailPessoal,
                            t.email_corporativo as EmailCorporativo,
                            t.login_rede as LoginRede,
                            t.tipo_login_rede as TipoLoginRede,
                            t.tipo_maquina as TipoMaquina,
                            t.observacoes_acesso_usuario as ObservacoesAcessoUsuario,
                            t.grupo_email_contrato as GrupoEmailContrato,
                            t.observacoes_aprovador_acessos as ObservacoesAprovadorAcessos,
                            t.salario as Salario,
                            t.custo_hora as CustoHora,
                            t.vr as VR,
                            t.va as VA,
                            t.assistencia_medica as AssistenciaMedica,
                            t.ajuda_de_custo as AjudaDeCusto,
                            t.mobilidade as Mobilidade,
                            t.educacao as Educacao,
                            t.remuneracao_total as RemuneracaoTotal,
                            t.celular as Celular,
                            t.plano_dados as PlanoDados,
                            t.quantidade_minutos_plano_dados as QuantidadeMinutosPlanoDados,
                            t.cartao_visitas as CartaoVisitas,
                            t.quantidade_cartao_visitas as QuantidadeCartaoVisitas,
                            t.outros_equipamentos as OutrosEquipamentos,
                            v.codigo as CodigoVaga,
                            v.titulo as TituloVaga,
                            tco.nome_cliente as NomeClienteVaga,
                            tv.descricao as DescricaoTipoVaga,
                            COALESCE(CAST(t.tb_modelo_trabalho_id AS CHAR), CAST(cv.tb_modelo_trabalho_id AS CHAR)) as ModeloTrabalhoId,
                            COALESCE(tmt_template.descricao, tmt_candidato.descricao) as ModeloTrabalhoDescricao,
                            COALESCE(t.quantidade_dias_presencial, cv.quantidade_dias_presencial) as QuantidadeDiasPresencial,
                            COALESCE(t.cargo_confianca, 0) as CargoConfianca,
                            t.valor_adicional_cargo_confianca as ValorAdicionalCargoConfianca,
                            CASE WHEN EXISTS (
                                SELECT 1 FROM tb_colaborador_org tco_colab 
                                WHERE tco_colab.codigo_interno_colaborador = c.codigo_interno_colaborador 
                                AND tco_colab.tb_org_id = v.tb_org_id 
                                AND tco_colab.ativo = 0
                            ) THEN 1 ELSE 0 END as ExColaborador
                        FROM tb_template_contratacao t
                        LEFT JOIN tb_colaborador c ON t.tb_colaborador_codigo_interno_colaborador = c.codigo_interno_colaborador
                        LEFT JOIN tb_colaborador analista ON t.tb_colaborador_codigo_interno_colaborador_analista = analista.codigo_interno_colaborador
                        LEFT JOIN tb_colaborador superior ON t.tb_colaborador_codigo_interno_colaborador_superior_imediato = superior.codigo_interno_colaborador
                        LEFT JOIN tb_endereco e ON c.endereco_id = e.id
                        LEFT JOIN tb_candidato_vaga cv ON t.tb_candidato_vaga_id = cv.id
                        LEFT JOIN tb_vaga v ON cv.tb_vaga_id = v.id
                        LEFT JOIN tb_gestor_externo tge ON v.tb_gestor_cod = tge.cod_gestor_externo AND v.tb_org_id = tge.tb_org_id
                        LEFT JOIN tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = v.tb_org_id
                        LEFT JOIN tb_tipo_vaga tv ON v.tb_tipo_vaga_id = tv.id
                        LEFT JOIN tb_modelo_trabalho tmt_template ON t.tb_modelo_trabalho_id = tmt_template.id
                        LEFT JOIN tb_modelo_trabalho tmt_candidato ON cv.tb_modelo_trabalho_id = tmt_candidato.id
                        LEFT JOIN tb_equipamento_padrao_cargo_funcao cargo ON t.tb_equipamento_padrao_cargo_funcao_id = cargo.id
                        LEFT JOIN tb_equipamento_padrao_grupo_area grupo ON cargo.tb_equipamento_padrao_grupo_area_id = grupo.id
                        WHERE t.id = @id";

                var result = await connection.QueryFirstOrDefaultAsync<TemplateDTO>(query, new { id });
                
                if (result != null)
                {
                    result.Saude = await ObterSaudeAsync(connection, result.ColaboradorCodigoInternoColaborador);
                    // Mapear endereço manualmente
                    var enderecoQuery = @"
                        SELECT 
                            e.id as Id,
                            e.cep as Cep,
                            e.endereco as Endereco,
                            e.numero as Numero,
                            e.complemento as Complemento,
                            e.bairro as Bairro,
                            e.cidade as Cidade,
                            e.estado as Estado,
                            e.com_quem_mora as ComQuemMora,
                            e.internacional_linha_um as InternacionalLinhaUm,
                            e.internacional_linha_dois as InternacionalLinhaDois
                        FROM tb_template_contratacao t
                        LEFT JOIN tb_colaborador c ON t.tb_colaborador_codigo_interno_colaborador = c.codigo_interno_colaborador
                        LEFT JOIN tb_endereco e ON c.endereco_id = e.id
                        WHERE t.id = @id";
                    
                    result.Endereco = await connection.QueryFirstOrDefaultAsync<EnderecoDTO>(enderecoQuery, new { id });
                }
                
                if (result != null)
                {
                    // Buscar outros grupos da tabela auxiliar
                    var outrosGruposQuery = @"
                        SELECT 
                            email_grupo as EmailGrupo
                        FROM tb_template_contratacao_outros_grupos_email 
                        WHERE tb_template_contratacao_id = @TemplateId AND ativo = 1
                        ORDER BY data_criacao";
                    
                    var outrosGrupos = await connection.QueryAsync<OutrosGruposDTO>(outrosGruposQuery, new { TemplateId = id });
                    result.OutrosGrupos = outrosGrupos.ToList();

                    // Buscar sistemas liberados da tabela de relacionamento
                    var sistemasLiberadosQuery = @"
                        SELECT 
                            sl.id,
                            sl.descricao
                        FROM tb_template_contratacao_sistemas_liberados tsl
                        INNER JOIN tb_sistemas_liberados_srs_template sl ON tsl.tb_sistemas_liberados_srs_template_id = sl.id
                        WHERE tsl.tb_template_contratacao_id = @TemplateId AND tsl.ativo = 1 AND sl.ativo = 1
                        ORDER BY sl.descricao";
                    
                    var sistemasLiberados = await connection.QueryAsync<SistemaLiberadoDTO>(sistemasLiberadosQuery, new { TemplateId = id });
                    result.SistemasLiberados = sistemasLiberados.ToList();

                    // Buscar diretórios da tabela de relacionamento
                    var diretoriosQuery = @"
                        SELECT 
                            d.id,
                            d.descricao,
                            td.leitura as Leitura,
                            td.escrita as Escrita
                        FROM tb_template_contratacao_diretorios td
                        INNER JOIN tb_diretorios_srs_template d ON td.tb_diretorios_srs_template_id = d.id
                        WHERE td.tb_template_contratacao_id = @TemplateId AND td.ativo = 1 AND d.ativo = 1
                        ORDER BY d.descricao";
                    
                    var diretorios = await connection.QueryAsync<DiretorioDTO>(diretoriosQuery, new { TemplateId = id });
                    result.Diretorios = diretorios.ToList();

                    // Buscar grupos de emails da tabela de relacionamento
                    var gruposEmailsQuery = @"
                        SELECT 
                            ge.id,
                            ge.descricao
                        FROM tb_template_contratacao_grupos_emails tge
                        INNER JOIN tb_grupos_emails_template ge ON tge.tb_grupos_emails_template_id = ge.id
                        WHERE tge.tb_template_contratacao_id = @TemplateId AND tge.ativo = 1 AND ge.ativo = 1
                        ORDER BY ge.descricao";
                    
                    var gruposEmails = await connection.QueryAsync<GrupoEmailDTO>(gruposEmailsQuery, new { TemplateId = id });
                    result.GruposEmails = gruposEmails.ToList();
                }
                
                return result;
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

        public async Task<TemplateDTO> ObterTemplatePorCandidatoVagaIdAsync(Guid tbCandidatoVagaId)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                        SELECT 
                            t.id as Id,
                            t.tb_colaborador_codigo_interno_colaborador_analista as ColaboradorCodigoInternoColaboradorAnalista,
                            analista.nome_completo as NomeColaboradorAnalista,
                            CAST(t.tb_colaborador_codigo_interno_colaborador AS CHAR) as ColaboradorCodigoInternoColaborador,
                            CAST(t.tb_candidato_vaga_id AS CHAR) as CandidatoVagaId,
                            t.cargo as Cargo,
                            CAST(t.tb_equipamento_padrao_cargo_funcao_id AS CHAR) as EquipamentoPadraoCargoFuncaoId,
                            grupo.nome_grupo as GrupoAreaEquipamentoPadraoCargoFuncao,
                            t.data_inicio as DataInicio,
                            t.horario_jornada as HorarioJornada,
                            t.tipo_horario_jornada as TipoHorarioJornada,
                            c.documento_colaborador as DocumentoColaborador,
                            c.rg as RgColaborador,
                            c.data_nascimento as DataNascimento,
                            c.contato_principal as ContatoPrincipal,
                            c.nome_completo as NomeCompleto,
                            t.tamanho_camiseta as TamanhoCamiseta,
                            v.maquina as DescricaoMaquina,
                            t.hardware as Hardware,
                            t.softwares_necessarios as SoftwaresNecessarios,
                            t.softwares_ec as SoftwaresEc,
                            CAST(t.tb_colaborador_codigo_interno_colaborador_superior_imediato AS CHAR) as ColaboradorCodigoInternoColaboradorSuperiorImediato,
                            superior.nome_completo as NomeColaboradorSuperiorImediato,
                            c.email_alternativo as EmailPessoal,
                            t.email_corporativo as EmailCorporativo,
                            t.login_rede as LoginRede,
                            t.tipo_login_rede as TipoLoginRede,
                            t.tipo_maquina as TipoMaquina,
                            t.observacoes_acesso_usuario as ObservacoesAcessoUsuario,
                            t.grupo_email_contrato as GrupoEmailContrato,
                            t.observacoes_aprovador_acessos as ObservacoesAprovadorAcessos,
                            t.salario as Salario,
                            t.custo_hora as CustoHora,
                            t.vr as VR,
                            t.va as VA,
                            t.assistencia_medica as AssistenciaMedica,
                            t.ajuda_de_custo as AjudaDeCusto,
                            t.mobilidade as Mobilidade,
                            t.educacao as Educacao,
                            t.remuneracao_total as RemuneracaoTotal,
                            t.celular as Celular,
                            t.plano_dados as PlanoDados,
                            t.quantidade_minutos_plano_dados as QuantidadeMinutosPlanoDados,
                            t.cartao_visitas as CartaoVisitas,
                            t.quantidade_cartao_visitas as QuantidadeCartaoVisitas,
                            t.outros_equipamentos as OutrosEquipamentos,
                            v.codigo as CodigoVaga,
                            v.titulo as TituloVaga,
                            tco.nome_cliente as NomeClienteVaga,
                            tv.descricao as DescricaoTipoVaga,
                            COALESCE(CAST(t.tb_modelo_trabalho_id AS CHAR), CAST(cv.tb_modelo_trabalho_id AS CHAR)) as ModeloTrabalhoId,
                            COALESCE(tmt_template.descricao, tmt_candidato.descricao) as ModeloTrabalhoDescricao,
                            COALESCE(t.quantidade_dias_presencial, cv.quantidade_dias_presencial) as QuantidadeDiasPresencial,
                            COALESCE(t.cargo_confianca, 0) as CargoConfianca,
                            t.valor_adicional_cargo_confianca as ValorAdicionalCargoConfianca,
                            CASE WHEN EXISTS (
                                SELECT 1 FROM tb_colaborador_org tco_colab 
                                WHERE tco_colab.codigo_interno_colaborador = c.codigo_interno_colaborador 
                                AND tco_colab.tb_org_id = v.tb_org_id 
                                AND tco_colab.ativo = 0
                            ) THEN 1 ELSE 0 END as ExColaborador
                        FROM tb_template_contratacao t
                        LEFT JOIN tb_colaborador c ON t.tb_colaborador_codigo_interno_colaborador = c.codigo_interno_colaborador
                        LEFT JOIN tb_colaborador analista ON t.tb_colaborador_codigo_interno_colaborador_analista = analista.codigo_interno_colaborador
                        LEFT JOIN tb_colaborador superior ON t.tb_colaborador_codigo_interno_colaborador_superior_imediato = superior.codigo_interno_colaborador
                        LEFT JOIN tb_endereco e ON c.endereco_id = e.id
                        LEFT JOIN tb_candidato_vaga cv ON t.tb_candidato_vaga_id = cv.id
                        LEFT JOIN tb_vaga v ON cv.tb_vaga_id = v.id
                        LEFT JOIN tb_gestor_externo tge ON v.tb_gestor_cod = tge.cod_gestor_externo AND v.tb_org_id = tge.tb_org_id
                        LEFT JOIN tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = v.tb_org_id
                        LEFT JOIN tb_tipo_vaga tv ON v.tb_tipo_vaga_id = tv.id
                        LEFT JOIN tb_modelo_trabalho tmt_template ON t.tb_modelo_trabalho_id = tmt_template.id
                        LEFT JOIN tb_modelo_trabalho tmt_candidato ON cv.tb_modelo_trabalho_id = tmt_candidato.id
                        LEFT JOIN tb_equipamento_padrao_cargo_funcao cargo ON t.tb_equipamento_padrao_cargo_funcao_id = cargo.id
                        LEFT JOIN tb_equipamento_padrao_grupo_area grupo ON cargo.tb_equipamento_padrao_grupo_area_id = grupo.id
                        WHERE t.tb_candidato_vaga_id = @tbCandidatoVagaId";

                var result = await connection.QueryFirstOrDefaultAsync<TemplateDTO>(query, new { tbCandidatoVagaId });
                
                if (result != null)
                {
                    result.Saude = await ObterSaudeAsync(connection, result.ColaboradorCodigoInternoColaborador);
                    // Mapear endereço manualmente
                    var enderecoQuery = @"
                        SELECT 
                            e.id as Id,
                            e.cep as Cep,
                            e.endereco as Endereco,
                            e.numero as Numero,
                            e.complemento as Complemento,
                            e.bairro as Bairro,
                            e.cidade as Cidade,
                            e.estado as Estado,
                            e.com_quem_mora as ComQuemMora,
                            e.internacional_linha_um as InternacionalLinhaUm,
                            e.internacional_linha_dois as InternacionalLinhaDois
                        FROM tb_template_contratacao t
                        LEFT JOIN tb_colaborador c ON t.tb_colaborador_codigo_interno_colaborador = c.codigo_interno_colaborador
                        LEFT JOIN tb_endereco e ON c.endereco_id = e.id
                        WHERE t.tb_candidato_vaga_id = @tbCandidatoVagaId";
                    
                    result.Endereco = await connection.QueryFirstOrDefaultAsync<EnderecoDTO>(enderecoQuery, new { tbCandidatoVagaId });
                }
                
                if (result != null)
                {
                    // Buscar outros grupos da tabela auxiliar
                    var outrosGruposQuery = @"
                        SELECT 
                            email_grupo as EmailGrupo
                        FROM tb_template_contratacao_outros_grupos_email 
                        WHERE tb_template_contratacao_id = @TemplateId AND ativo = 1
                        ORDER BY data_criacao";
                    
                    var outrosGrupos = await connection.QueryAsync<OutrosGruposDTO>(outrosGruposQuery, new { TemplateId = result.Id });
                    result.OutrosGrupos = outrosGrupos.ToList();

                    // Buscar sistemas liberados da tabela de relacionamento
                    var sistemasLiberadosQuery = @"
                        SELECT 
                            sl.id,
                            sl.descricao
                        FROM tb_template_contratacao_sistemas_liberados tsl
                        INNER JOIN tb_sistemas_liberados_srs_template sl ON tsl.tb_sistemas_liberados_srs_template_id = sl.id
                        WHERE tsl.tb_template_contratacao_id = @TemplateId AND tsl.ativo = 1 AND sl.ativo = 1
                        ORDER BY sl.descricao";
                    
                    var sistemasLiberados = await connection.QueryAsync<SistemaLiberadoDTO>(sistemasLiberadosQuery, new { TemplateId = result.Id });
                    result.SistemasLiberados = sistemasLiberados.ToList();

                    // Buscar diretórios da tabela de relacionamento
                    var diretoriosQuery = @"
                        SELECT 
                            d.id,
                            d.descricao,
                            td.leitura as Leitura,
                            td.escrita as Escrita
                        FROM tb_template_contratacao_diretorios td
                        INNER JOIN tb_diretorios_srs_template d ON td.tb_diretorios_srs_template_id = d.id
                        WHERE td.tb_template_contratacao_id = @TemplateId AND td.ativo = 1 AND d.ativo = 1
                        ORDER BY d.descricao";
                    
                    var diretorios = await connection.QueryAsync<DiretorioDTO>(diretoriosQuery, new { TemplateId = result.Id });
                    result.Diretorios = diretorios.ToList();

                    // Buscar grupos de emails da tabela de relacionamento
                    var gruposEmailsQuery = @"
                        SELECT 
                            ge.id,
                            ge.descricao
                        FROM tb_template_contratacao_grupos_emails tge
                        INNER JOIN tb_grupos_emails_template ge ON tge.tb_grupos_emails_template_id = ge.id
                        WHERE tge.tb_template_contratacao_id = @TemplateId AND tge.ativo = 1 AND ge.ativo = 1
                        ORDER BY ge.descricao";
                    
                    var gruposEmails = await connection.QueryAsync<GrupoEmailDTO>(gruposEmailsQuery, new { TemplateId = result.Id });
                    result.GruposEmails = gruposEmails.ToList();
                }
                
                return result;
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

        public async Task<TemplateDTO> ObterInformacoesColaboradorPorCandidatoAsync(Guid tbCandidatoVagaId)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                        SELECT 
                            cv.id as tb_candidato_vaga_id,
                            c.codigo_interno_colaborador as tb_colaborador_codigo_interno_colaborador,
                            c.documento_colaborador as DocumentoColaborador,
                            c.rg as RgColaborador,
                            c.data_nascimento as DataNascimento,
                            c.contato_principal as ContatoPrincipal,
                            c.nome_completo as NomeCompleto,
                            c.email_alternativo as EmailPessoal,
                            v.maquina as DescricaoMaquina,
                            v.codigo as CodigoVaga,
                            v.titulo as TituloVaga,
                            tco.nome_cliente as NomeClienteVaga,
                            tv.descricao as DescricaoTipoVaga,
                            CAST(cv.tb_modelo_trabalho_id AS CHAR) as ModeloTrabalhoId,
                            tmt.descricao as ModeloTrabalhoDescricao,
                            cv.quantidade_dias_presencial as QuantidadeDiasPresencial,
                            0 as CargoConfianca,
                            CASE WHEN EXISTS (
                                SELECT 1 FROM tb_colaborador_org tco_colab 
                                WHERE tco_colab.codigo_interno_colaborador = c.codigo_interno_colaborador 
                                AND tco_colab.tb_org_id = v.tb_org_id 
                                AND tco_colab.ativo = 0
                            ) THEN 1 ELSE 0 END as ExColaborador,
                            e.id as Endereco_Id,
                            e.cep as Endereco_Cep,
                            e.endereco as Endereco_Endereco,
                            e.numero as Endereco_Numero,
                            e.complemento as Endereco_Complemento,
                            e.bairro as Endereco_Bairro,
                            e.cidade as Endereco_Cidade,
                            e.estado as Endereco_Estado,
                            e.com_quem_mora as Endereco_ComQuemMora,
                            e.internacional_linha_um as Endereco_InternacionalLinhaUm,
                            e.internacional_linha_dois as Endereco_InternacionalLinhaDois
                        FROM tb_candidato_vaga cv
                        INNER JOIN tb_colaborador c ON cv.tb_colaborador_codigo_interno_colaborador = c.codigo_interno_colaborador
                        LEFT JOIN tb_endereco e ON c.endereco_id = e.id
                        LEFT JOIN tb_vaga v ON cv.tb_vaga_id = v.id
                        LEFT JOIN tb_gestor_externo tge ON v.tb_gestor_cod = tge.cod_gestor_externo AND v.tb_org_id = tge.tb_org_id
                        LEFT JOIN tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = v.tb_org_id
                        LEFT JOIN tb_tipo_vaga tv ON v.tb_tipo_vaga_id = tv.id
                        LEFT JOIN tb_modelo_trabalho tmt ON cv.tb_modelo_trabalho_id = tmt.id
                        WHERE cv.id = @tbCandidatoVagaId";

                var result = await connection.QueryFirstOrDefaultAsync<TemplateDTO>(query, new { tbCandidatoVagaId });
                
                if (result != null)
                {
                    result.Saude = await ObterSaudeAsync(connection, result.ColaboradorCodigoInternoColaborador);
                    // Mapear endereço manualmente
                    var enderecoQuery = @"
                        SELECT 
                            e.id as EnderecoId,
                            e.cep as Cep,
                            e.endereco as Endereco,
                            e.numero as Numero,
                            e.complemento as Complemento,
                            e.bairro as Bairro,
                            e.cidade as Cidade,
                            e.estado as Estado,
                            e.com_quem_mora as ComQuemMora,
                            e.internacional_linha_um as InternacionalLinhaUm,
                            e.internacional_linha_dois as InternacionalLinhaDois
                        FROM tb_candidato_vaga cv
                        INNER JOIN tb_colaborador c ON cv.tb_colaborador_codigo_interno_colaborador = c.codigo_interno_colaborador
                        LEFT JOIN tb_endereco e ON c.endereco_id = e.id
                        WHERE cv.id = @tbCandidatoVagaId";
                    
                    result.Endereco = await connection.QueryFirstOrDefaultAsync<EnderecoDTO>(enderecoQuery, new { tbCandidatoVagaId });
                }
                
                if (result != null)
                {
                    // Inicializar listas vazias para as relações
                    result.OutrosGrupos = new List<OutrosGruposDTO>();
                    result.SistemasLiberados = new List<SistemaLiberadoDTO>();
                    result.Diretorios = new List<DiretorioDTO>();
                    result.GruposEmails = new List<GrupoEmailDTO>();
                }
                
                return result;
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

        public async Task<TemplateDTO> ObterNomeECargoCandidatoAsync(Guid tbCandidatoVagaId)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                        SELECT 
                            c.nome_completo as NomeCompleto,
                            COALESCE(t.cargo, v.cargo) as Cargo
                        FROM tb_candidato_vaga cv
                        INNER JOIN tb_colaborador c ON cv.tb_colaborador_codigo_interno_colaborador = c.codigo_interno_colaborador
                        LEFT JOIN tb_vaga v ON cv.tb_vaga_id = v.id
                        LEFT JOIN tb_template_contratacao t ON t.tb_candidato_vaga_id = cv.id
                        WHERE cv.id = @tbCandidatoVagaId
                        LIMIT 1";

                var result = await connection.QueryFirstOrDefaultAsync<TemplateDTO>(query, new { tbCandidatoVagaId });
                
                return result;
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

        public async Task<bool> ExisteEmailUsuarioAsync(string email)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT COUNT(1) 
                    FROM tb_usuario 
                    WHERE email = @Email";

                var count = await connection.QuerySingleAsync<int>(query, new { Email = email });
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

        public async Task<List<TemplateDTO>> ListarTemplatesAsync(int limite, int cursor)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                        SELECT 
                            t.id as Id,
                            t.tb_colaborador_codigo_interno_colaborador_analista as ColaboradorCodigoInternoColaboradorAnalista,
                            analista.nome_completo as NomeColaboradorAnalista,
                            CAST(t.tb_colaborador_codigo_interno_colaborador AS CHAR) as ColaboradorCodigoInternoColaborador,
                            CAST(t.tb_candidato_vaga_id AS CHAR) as CandidatoVagaId,
                            t.cargo as Cargo,
                            t.data_inicio as DataInicio,
                            t.horario_jornada as HorarioJornada,
                            t.tipo_horario_jornada as TipoHorarioJornada,
                            c.documento_colaborador as DocumentoColaborador,
                            c.rg as RgColaborador,
                            c.data_nascimento as DataNascimento,
                            c.contato_principal as ContatoPrincipal,
                            c.nome_completo as NomeCompleto,
                            t.tamanho_camiseta as TamanhoCamiseta,
                            v.maquina as DescricaoMaquina,
                            t.hardware as Hardware,
                            t.softwares_necessarios as SoftwaresNecessarios,
                            t.softwares_ec as SoftwaresEc,
                            CAST(t.tb_colaborador_codigo_interno_colaborador_superior_imediato AS CHAR) as ColaboradorCodigoInternoColaboradorSuperiorImediato,
                            superior.nome_completo as NomeColaboradorSuperiorImediato,
                            c.email_alternativo as EmailPessoal,
                            t.email_corporativo as EmailCorporativo,
                            t.login_rede as LoginRede,
                            t.tipo_login_rede as TipoLoginRede,
                            t.tipo_maquina as TipoMaquina,
                            t.observacoes_acesso_usuario as ObservacoesAcessoUsuario,
                            t.grupo_email_contrato as GrupoEmailContrato,
                            t.outros_grupos_descricao,
                            t.observacoes_aprovador_acessos as ObservacoesAprovadorAcessos,
                            t.salario as Salario,
                            t.custo_hora as CustoHora,
                            t.vr as VR,
                            t.va as VA,
                            t.assistencia_medica as AssistenciaMedica,
                            t.ajuda_de_custo as AjudaDeCusto,
                            t.mobilidade as Mobilidade,
                            t.educacao as Educacao,
                            t.remuneracao_total as RemuneracaoTotal,
                            t.celular as Celular,
                            t.plano_dados as PlanoDados,
                            t.quantidade_minutos_plano_dados as QuantidadeMinutosPlanoDados,
                            t.cartao_visitas as CartaoVisitas,
                            t.quantidade_cartao_visitas as QuantidadeCartaoVisitas,
                            t.outros_equipamentos as OutrosEquipamentos,
                            v.codigo as CodigoVaga,
                            v.titulo as TituloVaga,
                            tco.nome_cliente as NomeClienteVaga,
                            tv.descricao as DescricaoTipoVaga,
                            COALESCE(CAST(t.tb_modelo_trabalho_id AS CHAR), CAST(cv.tb_modelo_trabalho_id AS CHAR)) as ModeloTrabalhoId,
                            COALESCE(tmt_template.descricao, tmt_candidato.descricao) as ModeloTrabalhoDescricao,
                            COALESCE(t.quantidade_dias_presencial, cv.quantidade_dias_presencial) as QuantidadeDiasPresencial,
                            COALESCE(t.cargo_confianca, 0) as CargoConfianca,
                            t.valor_adicional_cargo_confianca as ValorAdicionalCargoConfianca,
                            CASE WHEN EXISTS (
                                SELECT 1 FROM tb_colaborador_org tco_colab 
                                WHERE tco_colab.codigo_interno_colaborador = c.codigo_interno_colaborador 
                                AND tco_colab.tb_org_id = v.tb_org_id 
                                AND tco_colab.ativo = 0
                            ) THEN 1 ELSE 0 END as ExColaborador,
                            e.id as Endereco_Id,
                            e.cep as Endereco_Cep,
                            e.endereco as Endereco_Endereco,
                            e.numero as Endereco_Numero,
                            e.complemento as Endereco_Complemento,
                            e.bairro as Endereco_Bairro,
                            e.cidade as Endereco_Cidade,
                            e.estado as Endereco_Estado,
                            e.com_quem_mora as Endereco_ComQuemMora,
                            e.internacional_linha_um as Endereco_InternacionalLinhaUm,
                            e.internacional_linha_dois as Endereco_InternacionalLinhaDois
                        FROM tb_template_contratacao t
                        LEFT JOIN tb_colaborador c ON t.tb_colaborador_codigo_interno_colaborador = c.codigo_interno_colaborador
                        LEFT JOIN tb_colaborador analista ON t.tb_colaborador_codigo_interno_colaborador_analista = analista.codigo_interno_colaborador
                        LEFT JOIN tb_colaborador superior ON t.tb_colaborador_codigo_interno_colaborador_superior_imediato = superior.codigo_interno_colaborador
                        LEFT JOIN tb_endereco e ON c.endereco_id = e.id
                        LEFT JOIN tb_candidato_vaga cv ON t.tb_candidato_vaga_id = cv.id
                        LEFT JOIN tb_vaga v ON cv.tb_vaga_id = v.id
                        LEFT JOIN tb_gestor_externo tge ON v.tb_gestor_cod = tge.cod_gestor_externo AND v.tb_org_id = tge.tb_org_id
                        LEFT JOIN tb_cliente_org tco ON tco.codigo_cliente = tge.codigo_cliente AND tco.tb_org_id = v.tb_org_id
                        LEFT JOIN tb_tipo_vaga tv ON v.tb_tipo_vaga_id = tv.id
                        LEFT JOIN tb_modelo_trabalho tmt_template ON t.tb_modelo_trabalho_id = tmt_template.id
                        LEFT JOIN tb_modelo_trabalho tmt_candidato ON cv.tb_modelo_trabalho_id = tmt_candidato.id
                        ORDER BY t.data_inicio DESC
                        LIMIT @limite OFFSET @cursor";

                var result = await connection.QueryAsync<TemplateDTO>(query, new { limite, cursor });
                var templates = result.ToList();
                
                // Buscar outros grupos para cada template
                foreach (var template in templates)
                {
                    template.Saude = await ObterSaudeAsync(connection, template.ColaboradorCodigoInternoColaborador);
                    var outrosGruposQuery = @"
                        SELECT 
                            id,
                            email_grupo
                        FROM tb_template_contratacao_outros_grupos_email 
                        WHERE tb_template_contratacao_id = @TemplateId AND ativo = 1
                        ORDER BY data_criacao";
                    
                    var outrosGrupos = await connection.QueryAsync<OutrosGruposDTO>(outrosGruposQuery, new { TemplateId = template.Id });
                    template.OutrosGrupos = outrosGrupos.ToList();
                }
                
                return templates;
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

        public async Task<TemplateDTO> CriarTemplateAsync(TemplateDTO template, string codColaboradorLogado)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        long? enderecoId = null;

                        // Se há endereço, inserir na tb_endereco primeiro
                        if (template.Endereco != null)
                        {
                            var enderecoQuery = @"
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

                            await connection.ExecuteAsync(enderecoQuery, template.Endereco, transaction);
                            enderecoId = await connection.QuerySingleAsync<long>("SELECT LAST_INSERT_ID()", transaction: transaction);
                        }

                        // Atualizar dados do colaborador na tb_colaborador

                        var colaboradorQuery = @"
                            UPDATE tb_colaborador SET
                                documento_colaborador = @DocumentoColaborador,
                                data_nascimento = @DataNascimento,
                                contato_principal = @ContatoPrincipal,
                                nome_completo = @NomeCompleto,
                                rg = @Rg,
                                endereco_id = @EnderecoId,
                                email_alternativo = @EmailPessoal,
                                data_alteracao = NOW()
                            WHERE codigo_interno_colaborador = @ColaboradorCodigoInternoColaborador";

                        var colaboradorParams = new
                        {
                            template.DocumentoColaborador,
                            template.DataNascimento,
                            template.ContatoPrincipal,
                            template.NomeCompleto,
                            Rg = template.RgColaborador,
                            EnderecoId = enderecoId,
                            template.EmailPessoal,
                            template.ColaboradorCodigoInternoColaborador
                        };

                        await connection.ExecuteAsync(colaboradorQuery, colaboradorParams, transaction);
                        

                        // Inserir o template na tb_template_contratacao
                        var templateQuery = @"
                            INSERT INTO tb_template_contratacao (
                                id,
                                tb_colaborador_codigo_interno_colaborador_analista,
                                tb_colaborador_codigo_interno_colaborador,
                                tb_candidato_vaga_id,
                                cargo,
                                tb_equipamento_padrao_cargo_funcao_id,
                                data_inicio,
                                horario_jornada,
                                tipo_horario_jornada,
                                tamanho_camiseta,
                                hardware,
                                softwares_necessarios,
                                softwares_ec,
                                tb_colaborador_codigo_interno_colaborador_superior_imediato,
                                email_pessoal,
                                email_corporativo,
                                login_rede,
                                tipo_login_rede,
                                tipo_maquina,
                                observacoes_acesso_usuario,
                                grupo_email_contrato,
                                observacoes_aprovador_acessos,
                                salario,
                                custo_hora,
                                vr,
                                va,
                                assistencia_medica,
                                ajuda_de_custo,
                                mobilidade,
                                educacao,
                                remuneracao_total,
                                celular,
                                plano_dados,
                                quantidade_minutos_plano_dados,
                                cartao_visitas,
                                quantidade_cartao_visitas,
                                outros_equipamentos,
                                tb_modelo_trabalho_id,
                                quantidade_dias_presencial,
                                cargo_confianca,
                                valor_adicional_cargo_confianca,
                                data_criacao,
                                data_alteracao,
                                ativo
                            ) VALUES (
                                @Id,
                                @ColaboradorCodigoInternoColaboradorAnalista,
                                @ColaboradorCodigoInternoColaborador,
                                @CandidatoVagaId,
                                @Cargo,
                                @EquipamentoPadraoCargoFuncaoId,
                                @DataInicio,
                                @HorarioJornada,
                                @TipoHorarioJornada,
                                @TamanhoCamiseta,
                                @Hardware,
                                @SoftwaresNecessarios,
                                @SoftwaresEc,
                                @ColaboradorCodigoInternoColaboradorSuperiorImediato,
                                @EmailPessoal,
                                @EmailCorporativo,
                                @LoginRede,
                                @TipoLoginRede,
                                @TipoMaquina,
                                @ObservacoesAcessoUsuario,
                                @GrupoEmailContrato,
                                @ObservacoesAprovadorAcessos,
                                @Salario,
                                @CustoHora,
                                @VR,
                                @VA,
                                @AssistenciaMedica,
                                @AjudaDeCusto,
                                @Mobilidade,
                                @Educacao,
                                @RemuneracaoTotal,
                                @Celular,
                                @PlanoDados,
                                @QuantidadeMinutosPlanoDados,
                                @CartaoVisitas,
                                @QuantidadeCartaoVisitas,
                                @OutrosEquipamentos,
                                @ModeloTrabalhoId,
                                @QuantidadeDiasPresencial,
                                @CargoConfianca,
                                @ValorAdicionalCargoConfianca,
                                NOW(),
                                NOW(),
                                1
                            )";

                        var templateParams = new
                        {
                            template.Id,
                            template.ColaboradorCodigoInternoColaboradorAnalista,
                            template.ColaboradorCodigoInternoColaborador,
                            template.CandidatoVagaId,
                            template.Cargo,
                            EquipamentoPadraoCargoFuncaoId = template.EquipamentoPadraoCargoFuncaoId,
                            template.DataInicio,
                            template.HorarioJornada,
                            template.TipoHorarioJornada,
                            template.TamanhoCamiseta,
                            template.Hardware,
                            template.SoftwaresNecessarios,
                            template.SoftwaresEc,
                            template.ColaboradorCodigoInternoColaboradorSuperiorImediato,
                            template.EmailPessoal,
                            template.EmailCorporativo,
                            template.LoginRede,
                            template.TipoLoginRede,
                            template.TipoMaquina,
                            template.ObservacoesAcessoUsuario,
                            template.GrupoEmailContrato,
                            template.ObservacoesAprovadorAcessos,
                            template.Salario,
                            template.CustoHora,
                            template.VR,
                            template.VA,
                            template.AssistenciaMedica,
                            template.AjudaDeCusto,
                            template.Mobilidade,
                            template.Educacao,
                            template.RemuneracaoTotal,
                            Celular = template.Celular ? 1 : 0,
                            PlanoDados = template.PlanoDados ? 1 : 0,
                            template.QuantidadeMinutosPlanoDados,
                            CartaoVisitas = template.CartaoVisitas ? 1 : 0,
                            template.QuantidadeCartaoVisitas,
                            template.OutrosEquipamentos,
                            ModeloTrabalhoId = string.IsNullOrEmpty(template.ModeloTrabalhoId) ? null : template.ModeloTrabalhoId,
                            template.QuantidadeDiasPresencial,
                            CargoConfianca = template.CargoConfianca ? 1 : 0,
                            ValorAdicionalCargoConfianca = template.ValorAdicionalCargoConfianca ?? 0
                        };

                        await connection.ExecuteAsync(templateQuery, templateParams, transaction);

                        // Inserir outros grupos na tabela auxiliar
                        if (template.OutrosGrupos != null && template.OutrosGrupos.Any())
                        {
                            var outrosGruposQuery = @"
                                INSERT INTO tb_template_contratacao_outros_grupos_email (
                                    tb_template_contratacao_id,
                                    email_grupo,
                                    data_criacao,
                                    data_alteracao,
                                    ativo
                                ) VALUES (
                                    @TemplateContratacaoId,
                                    @EmailGrupo,
                                    NOW(),
                                    NOW(),
                                    1
                                )";

                            foreach (var grupo in template.OutrosGrupos)
                            {
                                await connection.ExecuteAsync(outrosGruposQuery, new
                                {
                                    TemplateContratacaoId = template.Id,
                                    EmailGrupo = grupo.EmailGrupo
                                }, transaction);
                            }
                        }

                        // Inserir sistemas liberados na tabela de relacionamento
                        if (template.SistemasLiberados != null && template.SistemasLiberados.Any())
                        {
                            var sistemasLiberadosQuery = @"
                                INSERT INTO tb_template_contratacao_sistemas_liberados (
                                    tb_template_contratacao_id,
                                    tb_sistemas_liberados_srs_template_id,
                                    data_criacao,
                                    data_alteracao,
                                    ativo
                                ) VALUES (
                                    @TemplateContratacaoId,
                                    @TbSistemasLiberadosSrsTemplateId,
                                    NOW(),
                                    NOW(),
                                    1
                                )";

                            foreach (var sistema in template.SistemasLiberados)
                            {
                                // Verificar se o sistema liberado existe
                                var sistemaExiste = await _sistemasLiberadosRepository.ExisteSistemaLiberadoAsync(sistema.Id);
                                if (!sistemaExiste)
                                {
                                    throw new ApplicationException($"Sistema liberado com ID {sistema.Id} não encontrado ou inativo.");
                                }
                                
                                await connection.ExecuteAsync(sistemasLiberadosQuery, new
                                {
                                    TemplateContratacaoId = template.Id,
                                    TbSistemasLiberadosSrsTemplateId = sistema.Id
                                }, transaction);
                            }
                        }

                        // Inserir diretórios na tabela de relacionamento
                        if (template.Diretorios != null && template.Diretorios.Any())
                        {
                            var diretoriosQuery = @"
                                INSERT INTO tb_template_contratacao_diretorios (
                                    tb_template_contratacao_id,
                                    tb_diretorios_srs_template_id,
                                    leitura,
                                    escrita,
                                    data_criacao,
                                    data_alteracao,
                                    ativo
                                ) VALUES (
                                    @TemplateContratacaoId,
                                    @TbDiretoriosSrsTemplateId,
                                    @Leitura,
                                    @Escrita,
                                    NOW(),
                                    NOW(),
                                    1
                                )";

                            foreach (var diretorio in template.Diretorios)
                            {
                                // Verificar se o diretório existe
                                var diretorioExiste = await _diretoriosRepository.ExisteDiretorioAsync(diretorio.Id);
                                if (!diretorioExiste)
                                {
                                    throw new ApplicationException($"Diretório com ID {diretorio.Id} não encontrado ou inativo.");
                                }
                                
                                await connection.ExecuteAsync(diretoriosQuery, new
                                {
                                    TemplateContratacaoId = template.Id,
                                    TbDiretoriosSrsTemplateId = diretorio.Id,
                                    Leitura = diretorio.Leitura,
                                    Escrita = diretorio.Escrita
                                }, transaction);
                            }
                        }

                        // Inserir grupos de emails na tabela de relacionamento
                        if (template.GruposEmails != null && template.GruposEmails.Any())
                        {
                            var gruposEmailsQuery = @"
                                INSERT INTO tb_template_contratacao_grupos_emails (
                                    tb_template_contratacao_id,
                                    tb_grupos_emails_template_id,
                                    data_criacao,
                                    data_alteracao,
                                    ativo
                                ) VALUES (
                                    @TemplateContratacaoId,
                                    @TbGruposEmailsTemplateId,
                                    NOW(),
                                    NOW(),
                                    1
                                )";

                            foreach (var grupoEmail in template.GruposEmails)
                            {
                                // Verificar se o grupo de email existe
                                var grupoEmailExiste = await _gruposEmailsRepository.ExisteGrupoEmailAsync(grupoEmail.Id);
                                if (!grupoEmailExiste)
                                {
                                    throw new ApplicationException($"Grupo de email com ID {grupoEmail.Id} não encontrado ou inativo.");
                                }
                                
                                await connection.ExecuteAsync(gruposEmailsQuery, new
                                {
                                    TemplateContratacaoId = template.Id,
                                    TbGruposEmailsTemplateId = grupoEmail.Id
                                }, transaction);
                            }
                        }

                        // Atualizar o campo maquina na tb_vaga se houver id_candidatura
                        if (!string.IsNullOrEmpty(template.TipoMaquina))
                        {
                            var vagaUpdateQuery = @"
                                UPDATE tb_vaga v
                                INNER JOIN tb_candidato_vaga cv ON v.id = cv.tb_vaga_id
                                SET v.maquina = @TipoMaquina
                                WHERE cv.id = @CandidatoVagaId";

                            await connection.ExecuteAsync(vagaUpdateQuery, new 
                            { 
                                template.TipoMaquina, 
                                template.CandidatoVagaId 
                            }, transaction);
                        }

                        // Atualizar o template com o ID do endereço
                        if (template.Endereco != null)
                        {
                            template.Endereco.Id = enderecoId;
                        }

                        transaction.Commit();

                        // Registrar log de criação
                        try
                        {
                            await _logRepository.InserirLogTemplateAsync(
                                template.Id,
                                codColaboradorLogado,
                                AcaoLogTemplateEnum.CREATE,
                                JsonConvert.SerializeObject(template, Formatting.Indented),
                                "Template criado com sucesso"
                            );
                        }
                        catch (Exception logEx)
                        {
                            // Log do erro, mas não falha a operação principal
                            // TODO: Implementar log de erro
                        }

                        // Buscar o template completo com dados da vaga após criação
                        return await ObterTemplatePorIdAsync(template.Id);
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
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

        public async Task<TemplateDTO> AtualizarTemplateAsync(TemplateDTO template, string codColaboradorLogado)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        long? enderecoId = await connection.QuerySingleOrDefaultAsync<long?>(@"SELECT 
	                                                                                    tc.endereco_id 
                                                                                    FROM tb_colaborador tc
                                                                                    WHERE tc.codigo_interno_colaborador = @ColaboradorCodigoInternoColaborador",
                                                                                    new { template.ColaboradorCodigoInternoColaborador }, 
                                                                                    transaction: transaction);

                        // Se há endereço, atualizar ou inserir na tb_endereco
                        if (template.Endereco != null)
                        {
                            if (enderecoId.HasValue)
                            {
                                template.Endereco.Id = enderecoId;

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

                                await connection.ExecuteAsync(enderecoUpdateQuery, template.Endereco, transaction);
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

                                await connection.ExecuteAsync(enderecoInsertQuery, template.Endereco, transaction);
                                enderecoId = await connection.QuerySingleAsync<long>("SELECT LAST_INSERT_ID()", transaction: transaction);
                                template.Endereco.Id = enderecoId;
                            }
                        }

                        // Atualizar dados do colaborador na tb_colaborador
                        if (!String.IsNullOrEmpty(template.ColaboradorCodigoInternoColaborador))
                        {
                            var colaboradorQuery = @"
                                UPDATE tb_colaborador SET
                                    documento_colaborador = @DocumentoColaborador,
                                    data_nascimento = @DataNascimento,
                                    contato_principal = @ContatoPrincipal,
                                    nome_completo = @NomeCompleto,
                                    rg = @Rg,
                                    endereco_id = @EnderecoId,
                                    email_alternativo = @EmailPessoal,
                                    data_alteracao = NOW()
                                WHERE codigo_interno_colaborador = @ColaboradorCodigoInternoColaborador";

                            var colaboradorParams = new
                            {
                                template.DocumentoColaborador,
                                template.DataNascimento,
                                template.ContatoPrincipal,
                                template.NomeCompleto,
                                Rg = template.RgColaborador,
                                EnderecoId = enderecoId,
                                template.EmailPessoal,
                                template.ColaboradorCodigoInternoColaborador
                            };

                            await connection.ExecuteAsync(colaboradorQuery, colaboradorParams, transaction);
                        }

                        // Atualizar o template na tb_template_contratacao
                        var templateQuery = @"
                            UPDATE tb_template_contratacao SET
                                tb_colaborador_codigo_interno_colaborador_analista = @ColaboradorCodigoInternoColaboradorAnalista,
                                tb_colaborador_codigo_interno_colaborador = @ColaboradorCodigoInternoColaborador,
                                tb_candidato_vaga_id = @CandidatoVagaId,
                                cargo = @Cargo,
                                data_inicio = @DataInicio,
                                horario_jornada = @HorarioJornada,
                                tipo_horario_jornada = @TipoHorarioJornada,
                                tamanho_camiseta = @TamanhoCamiseta,
                                hardware = @Hardware,
                                softwares_necessarios = @SoftwaresNecessarios,
                                softwares_ec = @SoftwaresEc,
                                tb_colaborador_codigo_interno_colaborador_superior_imediato = @ColaboradorCodigoInternoColaboradorSuperiorImediato,
                                email_pessoal = @EmailPessoal,
                                email_corporativo = @EmailCorporativo,
                                login_rede = @LoginRede,
                                tipo_login_rede = @TipoLoginRede,
                                tipo_maquina = @TipoMaquina,
                                observacoes_acesso_usuario = @ObservacoesAcessoUsuario,
                                grupo_email_contrato = @GrupoEmailContrato,
                                observacoes_aprovador_acessos = @ObservacoesAprovadorAcessos,
                                salario = @Salario,
                                custo_hora = @CustoHora,
                                vr = @VR,
                                va = @VA,
                                assistencia_medica = @AssistenciaMedica,
                                ajuda_de_custo = @AjudaDeCusto,
                                mobilidade = @Mobilidade,
                                educacao = @Educacao,
                                remuneracao_total = @RemuneracaoTotal,
                                celular = @Celular,
                                plano_dados = @PlanoDados,
                                quantidade_minutos_plano_dados = @QuantidadeMinutosPlanoDados,
                                cartao_visitas = @CartaoVisitas,
                                quantidade_cartao_visitas = @QuantidadeCartaoVisitas,
                                outros_equipamentos = @OutrosEquipamentos,
                                tb_modelo_trabalho_id = @ModeloTrabalhoId,
                                quantidade_dias_presencial = @QuantidadeDiasPresencial,
                                cargo_confianca = @CargoConfianca,
                                valor_adicional_cargo_confianca = @ValorAdicionalCargoConfianca,
                                data_alteracao = NOW()
                            WHERE id = @Id";

                        var templateParams = new
                        {
                            template.Id,
                            template.ColaboradorCodigoInternoColaboradorAnalista,
                            template.ColaboradorCodigoInternoColaborador,
                            template.CandidatoVagaId,
                            template.Cargo,
                            template.DataInicio,
                            template.HorarioJornada,
                            template.TipoHorarioJornada,
                            template.TamanhoCamiseta,
                            template.Hardware,
                            template.SoftwaresNecessarios,
                            template.SoftwaresEc,
                            template.ColaboradorCodigoInternoColaboradorSuperiorImediato,
                            template.EmailPessoal,
                            template.EmailCorporativo,
                            template.LoginRede,
                            template.TipoLoginRede,
                            template.TipoMaquina,
                            template.ObservacoesAcessoUsuario,
                            template.GrupoEmailContrato,
                            template.ObservacoesAprovadorAcessos,
                            template.Salario,
                            template.CustoHora,
                            template.VR,
                            template.VA,
                            template.AssistenciaMedica,
                            template.AjudaDeCusto,
                            template.Mobilidade,
                            template.Educacao,
                            template.RemuneracaoTotal,
                            Celular = template.Celular ? 1 : 0,
                            PlanoDados = template.PlanoDados ? 1 : 0,
                            template.QuantidadeMinutosPlanoDados,
                            CartaoVisitas = template.CartaoVisitas ? 1 : 0,
                            template.QuantidadeCartaoVisitas,
                            template.OutrosEquipamentos,
                            ModeloTrabalhoId = string.IsNullOrEmpty(template.ModeloTrabalhoId) ? null : template.ModeloTrabalhoId,
                            template.QuantidadeDiasPresencial,
                            template.CargoConfianca,
                            ValorAdicionalCargoConfianca = template.ValorAdicionalCargoConfianca ?? 0
                        };

                        await connection.ExecuteAsync(templateQuery, templateParams, transaction);

                        // Atualizar outros grupos na tabela auxiliar
                        if (template.OutrosGrupos != null)
                        {
                            // Deletar grupos existentes
                            var deleteGruposQuery = @"
                                DELETE FROM tb_template_contratacao_outros_grupos_email 
                                WHERE tb_template_contratacao_id = @TemplateId";
                            
                            await connection.ExecuteAsync(deleteGruposQuery, new { TemplateId = template.Id }, transaction);

                            // Inserir novos grupos
                            if (template.OutrosGrupos.Any())
                            {
                                var outrosGruposQuery = @"
                                    INSERT INTO tb_template_contratacao_outros_grupos_email (
                                        tb_template_contratacao_id,
                                        email_grupo,
                                        data_criacao,
                                        data_alteracao,
                                        ativo
                                    ) VALUES (
                                        @TemplateContratacaoId,
                                        @EmailGrupo,
                                        NOW(),
                                        NOW(),
                                        1
                                    )";

                                foreach (var grupo in template.OutrosGrupos)
                                {
                                    await connection.ExecuteAsync(outrosGruposQuery, new
                                    {
                                        TemplateContratacaoId = template.Id,
                                        EmailGrupo = grupo.EmailGrupo
                                    }, transaction);
                                }
                            }
                        }

                        // Atualizar sistemas liberados na tabela de relacionamento
                        if (template.SistemasLiberados != null)
                        {
                            // Deletar sistemas liberados existentes
                            var deleteSistemasQuery = @"
                                DELETE FROM tb_template_contratacao_sistemas_liberados 
                                WHERE tb_template_contratacao_id = @TemplateId";
                            
                            await connection.ExecuteAsync(deleteSistemasQuery, new { TemplateId = template.Id }, transaction);

                            // Inserir novos sistemas liberados
                            if (template.SistemasLiberados.Any())
                            {
                                var sistemasLiberadosQuery = @"
                                    INSERT INTO tb_template_contratacao_sistemas_liberados (
                                        tb_template_contratacao_id,
                                        tb_sistemas_liberados_srs_template_id,
                                        data_criacao,
                                        data_alteracao,
                                        ativo
                                    ) VALUES (
                                        @TemplateContratacaoId,
                                        @TbSistemasLiberadosSrsTemplateId,
                                        NOW(),
                                        NOW(),
                                        1
                                    )";

                                foreach (var sistema in template.SistemasLiberados)
                                {
                                    // Verificar se o sistema liberado existe
                                    var sistemaExiste = await _sistemasLiberadosRepository.ExisteSistemaLiberadoAsync(sistema.Id);
                                    if (!sistemaExiste)
                                    {
                                        throw new ApplicationException($"Sistema liberado com ID {sistema.Id} não encontrado ou inativo.");
                                    }
                                    
                                    await connection.ExecuteAsync(sistemasLiberadosQuery, new
                                    {
                                        TemplateContratacaoId = template.Id,
                                        TbSistemasLiberadosSrsTemplateId = sistema.Id
                                    }, transaction);
                                }
                            }
                        }

                        // Atualizar diretórios na tabela de relacionamento
                        if (template.Diretorios != null)
                        {
                            // Deletar diretórios existentes
                            var deleteDiretoriosQuery = @"
                                DELETE FROM tb_template_contratacao_diretorios 
                                WHERE tb_template_contratacao_id = @TemplateId";
                            
                            await connection.ExecuteAsync(deleteDiretoriosQuery, new { TemplateId = template.Id }, transaction);

                            // Inserir novos diretórios
                            if (template.Diretorios.Any())
                            {
                                var diretoriosQuery = @"
                                    INSERT INTO tb_template_contratacao_diretorios (
                                        tb_template_contratacao_id,
                                        tb_diretorios_srs_template_id,
                                        leitura,
                                        escrita,
                                        data_criacao,
                                        data_alteracao,
                                        ativo
                                    ) VALUES (
                                        @TemplateContratacaoId,
                                        @TbDiretoriosSrsTemplateId,
                                        @Leitura,
                                        @Escrita,
                                        NOW(),
                                        NOW(),
                                        1
                                    )";

                                foreach (var diretorio in template.Diretorios)
                                {
                                    // Verificar se o diretório existe
                                    var diretorioExiste = await _diretoriosRepository.ExisteDiretorioAsync(diretorio.Id);
                                    if (!diretorioExiste)
                                    {
                                        throw new ApplicationException($"Diretório com ID {diretorio.Id} não encontrado ou inativo.");
                                    }
                                    
                                    await connection.ExecuteAsync(diretoriosQuery, new
                                    {
                                        TemplateContratacaoId = template.Id,
                                        TbDiretoriosSrsTemplateId = diretorio.Id,
                                        Leitura = diretorio.Leitura,
                                        Escrita = diretorio.Escrita
                                    }, transaction);
                                }
                            }
                        }

                        // Atualizar grupos de emails na tabela de relacionamento
                        if (template.GruposEmails != null)
                        {
                            // Deletar grupos de emails existentes
                            var deleteGruposEmailsQuery = @"
                                DELETE FROM tb_template_contratacao_grupos_emails 
                                WHERE tb_template_contratacao_id = @TemplateId";
                            
                            await connection.ExecuteAsync(deleteGruposEmailsQuery, new { TemplateId = template.Id }, transaction);

                            // Inserir novos grupos de emails
                            if (template.GruposEmails.Any())
                            {
                                var gruposEmailsQuery = @"
                                    INSERT INTO tb_template_contratacao_grupos_emails (
                                        tb_template_contratacao_id,
                                        tb_grupos_emails_template_id,
                                        data_criacao,
                                        data_alteracao,
                                        ativo
                                    ) VALUES (
                                        @TemplateContratacaoId,
                                        @TbGruposEmailsTemplateId,
                                        NOW(),
                                        NOW(),
                                        1
                                    )";

                                foreach (var grupoEmail in template.GruposEmails)
                                {
                                    // Verificar se o grupo de email existe
                                    var grupoEmailExiste = await _gruposEmailsRepository.ExisteGrupoEmailAsync(grupoEmail.Id);
                                    if (!grupoEmailExiste)
                                    {
                                        throw new ApplicationException($"Grupo de email com ID {grupoEmail.Id} não encontrado ou inativo.");
                                    }
                                    
                                    await connection.ExecuteAsync(gruposEmailsQuery, new
                                    {
                                        TemplateContratacaoId = template.Id,
                                        TbGruposEmailsTemplateId = grupoEmail.Id
                                    }, transaction);
                                }
                            }
                        }

                        // Atualizar o campo maquina na tb_vaga se houver id_candidatura
                        if (String.IsNullOrEmpty(template.CandidatoVagaId) && !string.IsNullOrEmpty(template.DescricaoMaquina))
                        {
                            var vagaUpdateQuery = @"
                                UPDATE tb_vaga v
                                INNER JOIN tb_candidato_vaga cv ON v.id = cv.tb_vaga_id
                                SET v.maquina = @DescricaoMaquina
                                WHERE cv.id = @CandidatoVagaId";

                            await connection.ExecuteAsync(vagaUpdateQuery, new 
                            { 
                                template.DescricaoMaquina, 
                                template.CandidatoVagaId 
                            }, transaction);
                        }

                        transaction.Commit();

                        // Registrar log de atualização
                        try
                        {
                            await _logRepository.InserirLogTemplateAsync(
                                template.Id,
                                codColaboradorLogado,
                                AcaoLogTemplateEnum.UPDATE,
                                JsonConvert.SerializeObject(template, Formatting.Indented),
                                "Template atualizado com sucesso"
                            );
                        }
                        catch (Exception logEx)
                        {
                            // Log do erro, mas não falha a operação principal
                            // TODO: Implementar log de erro
                        }

                        // Buscar o template completo com dados da vaga após atualização
                        return await ObterTemplatePorIdAsync(template.Id);
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
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

        public async Task<bool> DeletarTemplateAsync(Guid id)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                // Buscar o template antes de deletar para registrar no log
                var templateAntes = await ObterTemplatePorIdAsync(id);

                var query = "DELETE FROM tb_template_contratacao WHERE id = @id";
                var rowsAffected = await connection.ExecuteAsync(query, new { id });

                if (rowsAffected > 0)
                {
                    // Registrar log de exclusão
                    try
                    {
                        await _logRepository.InserirLogTemplateAsync(
                            id,
                            templateAntes?.ColaboradorCodigoInternoColaboradorAnalista ?? "SISTEMA",
                            AcaoLogTemplateEnum.DELETE,
                            templateAntes != null ? JsonConvert.SerializeObject(templateAntes, Formatting.Indented) : null,
                            "Template deletado com sucesso"
                        );
                    }
                    catch (Exception logEx)
                    {
                        // Log do erro, mas não falha a operação principal
                        // TODO: Implementar log de erro
                    }
                }

                return rowsAffected > 0;
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

        public async Task<int> ContarTemplatesAsync()
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = "SELECT COUNT(*) FROM tb_template_contratacao";
                var count = await connection.QuerySingleAsync<int>(query);
                return count;
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

        public async Task<bool> ExisteTemplateParaCandidatoAsync(string tbCandidatoVagaId)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT COUNT(1) 
                    FROM tb_template_contratacao 
                    WHERE tb_candidato_vaga_id = @CandidatoVagaId";

                var count = await connection.QuerySingleAsync<int>(query, new { CandidatoVagaId = tbCandidatoVagaId });
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

        public async Task<List<EquipamentoPadraoDTO>> ListarEquipamentosPadroesAsync()
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        -- Mapeamento
                        mapeamento.id AS MapeamentoId,    
                        -- Dicionários Principais (COM O GRUPO_AREA)
                        grupo.nome_grupo AS GrupoArea,
                        cargo.id AS CargoId,
                        cargo.nome_cargo_funcao AS NomeCargoFuncao,
                        cat.categoria_nome AS CategoriaNome,    
                        -- Especificações
                        spec.tipo_equipamento AS TipoEquipamento,
                        spec.sistema_operacional AS SistemaOperacional,
                        spec.cpu_geracao AS CpuGeracao,
                        spec.gpu AS Gpu,
                        spec.memoria_ram AS MemoriaRam,
                        spec.armazenamento_disco AS ArmazenamentoDisco,    
                        -- Modelos Aprovados (com aliases mais claros)
                        mod1.nome_modelo AS ModeloAprovado1IntelLenovo,
                        mod2.nome_modelo AS ModeloAprovado2IntelDell,
                        mod3.nome_modelo AS ModeloAprovado3AmdLenovo,
                        mod4.nome_modelo AS ModeloAprovado4Apple,    
                        -- Upgrade
                        upg.descricao_upgrade AS DescricaoUpgrade
                    FROM 
                        tb_equipamento_padrao_mapeamento AS mapeamento    
                    -- JOINs CORRIGIDOS (com a adição do JOIN para GRUPO)
                    JOIN tb_equipamento_padrao_cargo_funcao AS cargo ON mapeamento.tb_equipamento_padrao_cargo_funcao_id = cargo.id
                    JOIN tb_equipamento_padrao_grupo_area AS grupo ON cargo.tb_equipamento_padrao_grupo_area_id = grupo.id
                    JOIN tb_equipamento_padrao_categoria_requisito AS cat ON mapeamento.tb_equipamento_padrao_categoria_requisito_id = cat.id
                    JOIN tb_equipamento_padrao_especificacao AS spec ON mapeamento.tb_equipamento_padrao_especificacao_id = spec.id
                    -- LEFT JOINs
                    LEFT JOIN tb_equipamento_padrao_catalogo_modelo AS mod1 ON mapeamento.tb_equipamento_padrao_catalogo_modelo_id_aprovado_1 = mod1.id
                    LEFT JOIN tb_equipamento_padrao_catalogo_modelo AS mod2 ON mapeamento.tb_equipamento_padrao_catalogo_modelo_id_aprovado_2 = mod2.id
                    LEFT JOIN tb_equipamento_padrao_catalogo_modelo AS mod3 ON mapeamento.tb_equipamento_padrao_catalogo_modelo_id_aprovado_3 = mod3.id
                    LEFT JOIN tb_equipamento_padrao_catalogo_modelo AS mod4 ON mapeamento.tb_equipamento_padrao_catalogo_modelo_id_aprovado_4 = mod4.id
                    LEFT JOIN tb_equipamento_padrao_upgrade_customizado AS upg ON mapeamento.tb_equipamento_padrao_upgrade_customizado_id = upg.id
                    ORDER BY
                        grupo.nome_grupo,
                        cargo.nome_cargo_funcao,
                        cat.id";

                var equipamentos = await connection.QueryAsync<EquipamentoPadraoDTO>(query);
                return equipamentos.ToList();
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

        public async Task<List<EquipamentoPadraoAninhadoDTO>> ListarEquipamentosPadroesAninhadosAsync()
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        -- Mapeamento
                        mapeamento.id AS MapeamentoId,    
                        -- Dicionários Principais (COM O GRUPO_AREA)
                        grupo.nome_grupo AS GrupoArea,
                        cargo.id AS CargoId,
                        cargo.nome_cargo_funcao AS NomeCargoFuncao,
                        cat.categoria_nome AS CategoriaNome,    
                        -- Especificações
                        spec.tipo_equipamento AS TipoEquipamento,
                        spec.sistema_operacional AS SistemaOperacional,
                        spec.cpu_geracao AS CpuGeracao,
                        spec.gpu AS Gpu,
                        spec.memoria_ram AS MemoriaRam,
                        spec.armazenamento_disco AS ArmazenamentoDisco,    
                        -- Modelos Aprovados (com aliases mais claros)
                        mod1.nome_modelo AS ModeloAprovado1IntelLenovo,
                        mod2.nome_modelo AS ModeloAprovado2IntelDell,
                        mod3.nome_modelo AS ModeloAprovado3AmdLenovo,
                        mod4.nome_modelo AS ModeloAprovado4Apple,    
                        -- Upgrade
                        upg.descricao_upgrade AS DescricaoUpgrade
                    FROM 
                        tb_equipamento_padrao_mapeamento AS mapeamento    
                    -- JOINs CORRIGIDOS (com a adição do JOIN para GRUPO)
                    JOIN tb_equipamento_padrao_cargo_funcao AS cargo ON mapeamento.tb_equipamento_padrao_cargo_funcao_id = cargo.id
                    JOIN tb_equipamento_padrao_grupo_area AS grupo ON cargo.tb_equipamento_padrao_grupo_area_id = grupo.id
                    JOIN tb_equipamento_padrao_categoria_requisito AS cat ON mapeamento.tb_equipamento_padrao_categoria_requisito_id = cat.id
                    JOIN tb_equipamento_padrao_especificacao AS spec ON mapeamento.tb_equipamento_padrao_especificacao_id = spec.id
                    -- LEFT JOINs
                    LEFT JOIN tb_equipamento_padrao_catalogo_modelo AS mod1 ON mapeamento.tb_equipamento_padrao_catalogo_modelo_id_aprovado_1 = mod1.id
                    LEFT JOIN tb_equipamento_padrao_catalogo_modelo AS mod2 ON mapeamento.tb_equipamento_padrao_catalogo_modelo_id_aprovado_2 = mod2.id
                    LEFT JOIN tb_equipamento_padrao_catalogo_modelo AS mod3 ON mapeamento.tb_equipamento_padrao_catalogo_modelo_id_aprovado_3 = mod3.id
                    LEFT JOIN tb_equipamento_padrao_catalogo_modelo AS mod4 ON mapeamento.tb_equipamento_padrao_catalogo_modelo_id_aprovado_4 = mod4.id
                    LEFT JOIN tb_equipamento_padrao_upgrade_customizado AS upg ON mapeamento.tb_equipamento_padrao_upgrade_customizado_id = upg.id
                    ORDER BY
                        grupo.nome_grupo,
                        cargo.nome_cargo_funcao,
                        cat.id";

                var equipamentos = await connection.QueryAsync<EquipamentoPadraoDTO>(query);
                
                // Organizar dados de forma aninhada
                var resultadoAninhado = new List<EquipamentoPadraoAninhadoDTO>();
                
                var gruposAgrupados = equipamentos.GroupBy(e => e.GrupoArea);
                
                foreach (var grupo in gruposAgrupados)
                {
                    var grupoAninhado = new EquipamentoPadraoAninhadoDTO
                    {
                        GrupoArea = grupo.Key,
                        CargosFuncoes = new List<CargoFuncaoEquipamentoDTO>()
                    };
                    
                        var cargosAgrupados = grupo.GroupBy(e => e.NomeCargoFuncao);
                    
                    foreach (var cargo in cargosAgrupados)
                    {
                        var cargoAninhado = new CargoFuncaoEquipamentoDTO
                        {
                            IdCargoFuncao = cargo.First().CargoId.ToString(),
                            GrupoArea = grupo.Key,
                            NomeCargoFuncao = cargo.Key,
                            OpcoesEquipamento = new List<OpcaoEquipamentoDTO>()
                        };
                        
                        foreach (var equipamento in cargo)
                        {
                            // Criar opção baseada na categoria do equipamento
                            var opcao = new OpcaoEquipamentoDTO
                            {
                                TipoOpcao = equipamento.CategoriaNome, // Usar a categoria como tipo de opção
                                CategoriaNome = equipamento.CategoriaNome,
                                TipoEquipamento = equipamento.TipoEquipamento,
                                CpuGeracao = equipamento.CpuGeracao,
                                MemoriaRam = equipamento.MemoriaRam,
                                ArmazenamentoDisco = equipamento.ArmazenamentoDisco,
                                So = equipamento.SistemaOperacional,
                                Gpu = equipamento.Gpu,
                                DescricaoUpgrade = equipamento.DescricaoUpgrade
                            };

                            // Listar todas as opções possíveis de modelos aprovados
                            var modelosPossiveis = new List<string>();
                            if (!string.IsNullOrWhiteSpace(equipamento.ModeloAprovado1IntelLenovo)) modelosPossiveis.Add(equipamento.ModeloAprovado1IntelLenovo);
                            if (!string.IsNullOrWhiteSpace(equipamento.ModeloAprovado2IntelDell)) modelosPossiveis.Add(equipamento.ModeloAprovado2IntelDell);
                            if (!string.IsNullOrWhiteSpace(equipamento.ModeloAprovado3AmdLenovo)) modelosPossiveis.Add(equipamento.ModeloAprovado3AmdLenovo);
                            if (!string.IsNullOrWhiteSpace(equipamento.ModeloAprovado4Apple)) modelosPossiveis.Add(equipamento.ModeloAprovado4Apple);

                            // Preencher com defaults amigáveis quando ausentes
                            if (modelosPossiveis.Count == 0)
                            {
                                // Defaults conforme linhas típicas
                                modelosPossiveis.Add("Lenovo ThinkPad T14");
                                modelosPossiveis.Add("Dell Latitude 3450");
                                modelosPossiveis.Add("Lenovo ThinkPad P14");
                                modelosPossiveis.Add("Apple MacBook Pro");
                            }

                            // Atribuir a lista de opções possíveis
                            opcao.ModelosPossiveis = modelosPossiveis.Distinct().ToList();
                            
                            cargoAninhado.OpcoesEquipamento.Add(opcao);
                        }
                        
                        grupoAninhado.CargosFuncoes.Add(cargoAninhado);
                    }
                    
                    resultadoAninhado.Add(grupoAninhado);
                }
                
                return resultadoAninhado;
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

        public async Task<bool> ExisteColaboradorPorCodigoInternoAsync(string codigoInternoColaborador)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT EXISTS (
                        SELECT 1 
                        FROM tb_colaborador 
                        WHERE codigo_interno_colaborador = @CodigoInternoColaborador
                    )";

                var result = await connection.ExecuteScalarAsync<bool>(query, new { CodigoInternoColaborador = codigoInternoColaborador });
                return result;
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

        public async Task<bool> ExisteEquipamentoPadraoCargoFuncaoAsync(string equipamentoPadraoCargoFuncaoId)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT EXISTS (
                        SELECT 1 
                        FROM tb_equipamento_padrao_cargo_funcao 
                        WHERE id = @EquipamentoPadraoCargoFuncaoId
                    )";

                var result = await connection.ExecuteScalarAsync<bool>(query, new { EquipamentoPadraoCargoFuncaoId = equipamentoPadraoCargoFuncaoId });
                return result;
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

        private async Task<TemplateSaudeDTO> ObterSaudeAsync(MySql.Data.MySqlClient.MySqlConnection connection, string codigoInternoColaborador)
        {
            var saude = CriarSaudeDefault();

            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
            {
                return saude;
            }

            var query = @"
                SELECT
                    cs.pcd AS TipoPcd,
                    CASE
                        WHEN cs.pcd IS NULL OR cs.pcd = 'Nenhuma' THEN 'Não'
                        ELSE 'Sim'
                    END AS Pcd,
                    cs.grupo_risco_covid AS GrupoDeRiscoCovid,
                    cs.condicao_saude_relevante AS CondicaoDeSaudeRelevante
                FROM tb_colaborador c
                LEFT JOIN tb_colaborador_saude cs ON c.colaborador_saude_id = cs.id
                WHERE c.codigo_interno_colaborador = @Codigo
                LIMIT 1";

            var data = await connection.QueryFirstOrDefaultAsync<SaudeRecord>(query, new { Codigo = codigoInternoColaborador });

            if (data == null)
            {
                return saude;
            }

            if (!string.IsNullOrWhiteSpace(data.TipoPcd))
            {
                if (Enum.TryParse<EnumPCD>(data.TipoPcd, true, out var enumPcd))
                {
                    saude.TipoPcd = enumPcd.ToString();
                    saude.EnumPCD = (int)enumPcd;
                }
                else
                {
                    saude.TipoPcd = data.TipoPcd.ToUpperInvariant();
                }
            }

            saude.Pcd = data.Pcd;
            saude.GrupoDeRiscoCovid = data.GrupoDeRiscoCovid ?? 0;
            saude.CondicaoDeSaudeRelevante = data.CondicaoDeSaudeRelevante ?? string.Empty;

            return saude;
        }

        private static TemplateSaudeDTO CriarSaudeDefault()
        {
            return new TemplateSaudeDTO
            {
                Pcd = "Não",
                TipoPcd = EnumPCD.Nenhuma.ToString(),
                EnumPCD = (int)EnumPCD.Nenhuma,
                GrupoDeRiscoCovid = 0,
                CondicaoDeSaudeRelevante = string.Empty
            };
        }

        private class SaudeRecord
        {
            public string Pcd { get; set; }
            public string TipoPcd { get; set; }
            public sbyte? GrupoDeRiscoCovid { get; set; }
            public string CondicaoDeSaudeRelevante { get; set; }
        }
    }
}
using ApiClient.Domain;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Extension;
using Colaboracao.Infra.Context;
using Core.Domain.Colaborador;
using Dapper;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.LG.Holerite;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.Org;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Util;
using DataTransferObject.Domain.Vaga;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataTransferObject.Domain.Cargo;
using DataTransferObject.Domain.Foursys;
using UglyToad.PdfPig.Logging;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class BuscaColaboradorRepository : IBuscaColaboradorRepository
    {
        private const int ATIVO = 1;
        private const int NAO_ATIVO = 0;
        private const int ORG_FOURSYS = 2;
        private ColaboradorContext _colaboradorContext;
        private string serviceMidia = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);
        private IConnectionStringCore _connectionString;
        private IDBConnection _dapperConnection;

        public BuscaColaboradorRepository(ColaboradorContext colaboradorContext, IConnectionStringCore connectionString, IDBConnection dapperConnection)
        {
            _colaboradorContext = colaboradorContext;
            _connectionString = connectionString;
            _dapperConnection = dapperConnection;
        }

        public ColaboradorDTO GetColaborador(string cpf, int orgId)
        {
            var registroDb = _colaboradorContext.tb_colaborador.Where(x => x.codigo_interno_colaborador == cpf).FirstOrDefault();

            if (registroDb is null)
            {
                throw new ArgumentException("Código colaborador informado não encontrado");
            }

            var colaboradorOrg = registroDb.tb_colaborador_org.Where(x => x.tb_org_id == orgId).FirstOrDefault();

            var sobre = registroDb.tb_colaborador_sobre.FirstOrDefault()?.descricao;

            var passaportes = _colaboradorContext.tb_colaborador_passaporte
                                .Join(_colaboradorContext.tb_nacionalidade, tcp => tcp.tb_nacionalidade_id, tn => tn.Id, (tcp, tn) => new { tcp, tn })
                                .Where(x => x.tcp.codigo_interno_colaborador == cpf)
                                .Select(x => new PassaporteColaboradorDTO()
                                {
                                    Id = x.tcp.id,
                                    IdNacionalidade = x.tcp.tb_nacionalidade_id,
                                    Validade = x.tcp.validade,
                                    DescricaoNacionalidade = x.tn.Descricao
                                }).ToList();

            var vistos = _colaboradorContext.tb_colaborador_visto
                                .Join(_colaboradorContext.tb_pais, tcv => tcv.tb_pais_id, tp => tp.Id, (tcv, tp) => new { tcv, tp })
                                .Where(x => x.tcv.codigo_interno_colaborador == cpf)
                                .Select(x => new VistoColaboradorDTO()
                                {
                                    Id = x.tcv.id,
                                    IdPais = x.tcv.tb_pais_id,
                                    Validade = x.tcv.validade,
                                    DescricaoPais = x.tp.Descricao
                                }).ToList();

            var ret = new ColaboradorDTO();
            if (registroDb == null)
            {
                return null;
            }

            ret.Cpf = registroDb.codigo_interno_colaborador;
            ret.Sobre = sobre;
            ret.Passaportes = passaportes;
            ret.Vistos = vistos;
            ret.NomeCompleto = registroDb.nome_completo;
            ret.ContatoPrincipalDDI = registroDb.contato_principal_ddi;
            ret.ContatoPrincipal = registroDb.contato_principal;
            ret.ContatoOutros = registroDb.contato_outro;
            ret.DataNascimento = registroDb.data_nascimento;
            ret.Rg = registroDb.rg;
            ret.Matricula = registroDb.matricula;
            ret.DataAdmissao = colaboradorOrg?.data_admissao ?? null;
            ret.FlagCandidato = registroDb.candidato == 1;
            ret.FlagAtivo = colaboradorOrg != null && colaboradorOrg.ativo == 1;
            ret.Passaporte = registroDb.passaporte;
            ret.EmailAlternativo = registroDb.email_alternativo;
            ret.EstadoCivil = registroDb.estado_civil;
            ret.Genero = registroDb.genero;
            ret.Etnia = registroDb.etnia;
            ret.OrientacaoSexual = registroDb.orientacao_sexual;
            ret.Escolaridade = registroDb.escolaridade;
            ret.PessoaRefugiada = registroDb.refugiado == 1 ? true : false;
            ret.Nacionalidade = registroDb.nacionalidade;
            ret.DocumentoColaborador = registroDb.documento_colaborador;
            ret.UrlLinkedin = registroDb.url_linkedin;
            ret.DataSincronizacaoLinkedin = registroDb.data_sync_linkedin;
            
            

            //Saude
            ret.Saude = null;
            if (registroDb.colaborador_saude != null && registroDb.colaborador_saude_id != 0)
            {
                string pcdBanco = registroDb.colaborador_saude.pcd;

                if (pcdBanco == "Psicossocial / Mental")
                {
                    pcdBanco = "PsicossocialMental";
                }

                if (Enum.TryParse(pcdBanco, out EnumPCD pcd))
                {
                    ret.Saude = new ColaboradorSaudeDTO
                    {
                        PCD = pcd,
                        CondicaoDeSaudeRelevante = registroDb.colaborador_saude.condicao_saude_relevante,
                        GrupoDeRiscoCovid = Convert.ToSByte(registroDb.colaborador_saude.grupo_risco_covid)
                    };

                    if (ret.Saude.PCD == EnumPCD.Auditiva &&
                        ret.Saude.GrupoDeRiscoCovid == 0 &&
                        ret.Saude.CondicaoDeSaudeRelevante == null)
                    {
                        ret.Saude = null;
                    }
                }
            }

            //Usuario
            var usuarioDb = registroDb.tb_usuario.Where(x => x.tb_org_id == orgId).FirstOrDefault();

            if (usuarioDb != null)
            {
                ret.Email = usuarioDb.email;
                ret.Slack_id = usuarioDb.slack_id;
                ret.FcmToken = usuarioDb.fcm_token;
                ret.EmpresasVinculadas = new List<VinculoEmpresaColaboradorDTO>();
                if (usuarioDb.tb_empresa_usuario.Count() > 0)
                {
                    foreach (var rowEmpresa in usuarioDb.tb_empresa_usuario.ToList())
                    {
                        ret.EmpresasVinculadas.Add(new VinculoEmpresaColaboradorDTO
                        {
                            Cnpj = rowEmpresa.tb_empresa_cnpj,
                            Confirmado = rowEmpresa.confirmado == (sbyte)1,
                            Pendente = rowEmpresa.pendente == (sbyte)1,
                            DataConvite = rowEmpresa.data_convite,
                            NomeFantasia = rowEmpresa.tb_empresa_cnpjNavigation.nome_fantasia
                        });
                    }
                }
            }

            //Diretoria
            if (colaboradorOrg != null)
            {
                ret.Diretoria.Id = colaboradorOrg.cod_diretoria;
                ret.Diretoria.Diretoria = colaboradorOrg.diretoria;
            }

            //Endereco
            var enderecoDb = registroDb.endereco;
            if (enderecoDb != null && enderecoDb.cep != null)
            {
                ret.Endereco = new EnderecoDTO
                {
                    Cep = enderecoDb.cep,
                    Endereco = enderecoDb.endereco,
                    Complemento = enderecoDb.complemento,
                    Numero = enderecoDb.numero,
                    Bairro = enderecoDb.bairro,
                    Cidade = enderecoDb.cidade,
                    Estado = enderecoDb.estado,
                    ComQuemMora = enderecoDb.com_quem_mora,
                    InternacionalLinhaUm = enderecoDb.internacional_linha_um,
                    InternacionalLinhaDois = enderecoDb.internacional_linha_dois
                };
            }
            else
            {
                ret.Endereco = null;
            }

            var imagemDb = registroDb.imagem;
            if (imagemDb != null)
            {
                try
                {
                    var imagemPath = imagemDb.path;
                    var imagemThumbPath = imagemPath.Replace(".png", "_thumb.png");
                    var imagemThumbPathMini = imagemPath.Replace(".png", "_thumb50.png");
                    var imagemThumbPathVeryMini = imagemPath.Replace(".png", "_thumb25.png");

                    ret.UrlFoto = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + imagemPath;
                    ret.UrlFotoThumb = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + imagemThumbPath;
                    ret.UrlFotoThumbMini = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + imagemThumbPathMini;
                    ret.UrlFotoThumbVeryMini = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + imagemThumbPathVeryMini;
                }
                catch (Exception e)
                {
                    ret.UrlFoto = null;
                }
            }
            else
            {
                ret.UrlFoto = null;
            }

            var curriculo = registroDb.tb_curriculo_colaborador.Where(x => x.codigo_interno_colaborador == cpf)?.LastOrDefault();
            if (curriculo != null)
            {
                ret.ColaboradorCurriculo = new()
                {
                    PathCurriculo = curriculo.path_curriculo,
                    Ativo = curriculo.ativo == 1,
                    ColaboradorCPF = curriculo.codigo_interno_colaborador
                };
            }

            return ret;
        }

        public async Task<List<ColaboradorCvDadosDTO>> ListarDadosDoColaboradorPorIds(List<string> cpfs)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"SELECT
                            tc.codigo_interno_colaborador AS Cpf,
                            tc.nome_completo AS NomeCompleto,
                            tu.email AS Email,
                            tu.slack_id AS Slack_Id,
                            tu.fcm_token AS FcmToken,
                            tc.contato_principal_ddi AS ContatoPrincipalDDI,
                            tc.contato_principal AS ContatoPrincipal,
                            tc.contato_outro AS ContatoOutros,
                            tc.data_nascimento AS DataNascimento,
                            tcs.descricao AS Sobre,
                            ti.`path` AS UrlFoto,
                            tcp.id AS Id,
                            tcp.tb_nacionalidade_id AS IdNacionalidade,
                            tcp.validade AS Validade,
                            tn.Descricao AS DescricaoNacionalidade,
                            tcv.id AS Id,
                            tcv.tb_pais_id AS IdPais,
                            tcv.validade AS Validade,
                            tp.Descricao AS DescricaoPais,
                            te.cep AS Cep,
                            te.endereco AS Endereco,
                            te.complemento AS Complemento,
                            te.numero AS Numero,
                            te.bairro AS Bairro,
                            te.cidade AS Cidade,
                            te.estado AS Estado,
                            te.com_quem_mora AS ComQuemMora,
                            te.internacional_linha_um AS InternacionalLinhaUm,
                            te.internacional_linha_dois AS InternacionalLinhaDois,
                            tco.cod_diretoria AS Id,
                            tco.diretoria AS Diretoria
                        FROM
                            tb_colaborador_org tco
                        LEFT JOIN tb_colaborador tc
                            ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                        LEFT JOIN tb_usuario tu
                            ON tu.codigo_interno_colaborador = tco.codigo_interno_colaborador
                        LEFT JOIN tb_colaborador_sobre tcs
                            ON tcs.codigo_interno_colaborador = tco.codigo_interno_colaborador
                        LEFT JOIN tb_colaborador_passaporte tcp
                            ON tcp.codigo_interno_colaborador = tco.codigo_interno_colaborador
                        LEFT JOIN tb_nacionalidade tn
                            ON tn.Id = tcp.tb_nacionalidade_id
                        LEFT JOIN tb_colaborador_visto tcv
                            ON tcv.codigo_interno_colaborador = tco.codigo_interno_colaborador
                        LEFT JOIN tb_pais tp
                            ON tp.Id = tcv.tb_pais_id
                        LEFT JOIN tb_endereco te
                            ON te.id = tc.endereco_id
                        LEFT JOIN tb_imagem ti
                            ON ti.id = tc.imagem_id
                        WHERE
                            tco.codigo_interno_colaborador IN @Cpfs;";

            var parametros = new
            {
                Cpfs = cpfs,
            };

            var result = await connection.QueryAsync<ColaboradorCvDadosDTO, PassaporteColaboradorDTO, VistoColaboradorDTO, EnderecoDTO, DiretoriaDTO, ColaboradorCvDadosDTO>(
                query,
                (colaboradorDTO, passaporteColaboradorDTO, vistoColaboradorDTO, enderecoDTO, diretoriaDTO) =>
                {
                    colaboradorDTO.Passaportes = colaboradorDTO.Passaportes ?? new List<PassaporteColaboradorDTO>();
                    colaboradorDTO.Vistos = colaboradorDTO.Vistos ?? new List<VistoColaboradorDTO>();

                    if (passaporteColaboradorDTO != null)
                        colaboradorDTO.Passaportes.Add(passaporteColaboradorDTO);
                    if (vistoColaboradorDTO != null)
                        colaboradorDTO.Vistos.Add(vistoColaboradorDTO);

                    colaboradorDTO.Endereco = enderecoDTO ?? new();
                    colaboradorDTO.Diretoria = diretoriaDTO;

                    return colaboradorDTO;
                },
                splitOn: "Id, Id, Cep, Id", // Ajuste os pontos de split conforme a consulta
                param: parametros
            );

            var groupedResult = result
                .GroupBy(x => x.Cpf)
                .Select(g =>
                {
                    var colaborador = g.First();
                    colaborador.Vistos = g.SelectMany(x => x.Vistos).ToList();
                    colaborador.Passaportes = g.SelectMany(x => x.Passaportes).ToList();
                    if (colaborador.UrlFoto != null)
                    {
                        var imagemPath = colaborador.UrlFoto;
                        var imagemThumbPath = imagemPath.Replace(".png", "_thumb.png");
                        var imagemThumbPathMini = imagemPath.Replace(".png", "_thumb50.png");
                        var imagemThumbPathVeryMini = imagemPath.Replace(".png", "_thumb25.png");

                        colaborador.UrlFoto = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + imagemPath;
                        colaborador.UrlFotoThumb = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + imagemThumbPath;
                        colaborador.UrlFotoThumbMini = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + imagemThumbPathMini;
                        colaborador.UrlFotoThumbVeryMini = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + imagemThumbPathVeryMini;
                    }
                    return colaborador;
                }).ToList();

            return groupedResult;
        }

        public ColaboradorOrgDTO GetColaboradorOrg(string cpf, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org.Where(x => x.codigo_interno_colaborador == cpf && x.tb_org_id == orgId).ToList()
                .Select(x => BuildColaboradorOrgDTO(x)).FirstOrDefault();
        }
        public int? GetEmailOrg(string email)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"
                        select
                            tco.tb_org_id
                        from
                            tb_colaborador_org tco
                            inner join tb_usuario tu on tu.cpf = tco.codigo_interno_colaborador
                        where
                            tco.tb_org_id <> 1 and tu.email = @Email and exists (select 1 from tb_ad_sso tas where tas.tb_org_id = tco.tb_org_id);";
                    return _connection.Query<int?>(query, new { Email = email }).ToList().FirstOrDefault();
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

        public string MontarUrlSSO(int orgId)
        {
            var url = "https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize?response_type=code&client_id={client_id}&scope=openid%20profile%20offline_access&redirect_uri={redirect_url}&code_challenge={code_verifier_plain}&code_challenge_method=plain";

            Regex regex = new Regex(@"{([^}]+)}");
            MatchCollection matches = regex.Matches(url);

            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = "SELECT * FROM tb_ad_sso WHERE tb_org_id = @OrgId";
                    var sso = _connection.QueryAsync<dynamic>(query, new { OrgId = orgId });
                    if (sso.Result.Any())
                    {
                        foreach (Match match in matches)
                        {
                            string coluna = match.Groups[1].Value;
                            var valorDaColuna = ((IDictionary<string, object>)sso.Result.First())[coluna];
                            url = url.Replace("{" + coluna + "}", valorDaColuna?.ToString() ?? "");
                        }
                    }
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
            return url;
        }

        private ColaboradorOrgDTO BuildColaboradorOrgDTO(tb_colaborador_org colaboradorOrg)
        {
            return new ColaboradorOrgDTO
            {
                Ativo = colaboradorOrg.ativo == 1,
                Cargo = colaboradorOrg.cargo,
                CodCargo = colaboradorOrg.codigo_cargo,
                CodColaborador = colaboradorOrg.cod_colaborador_externo,
                CodDepartamento = colaboradorOrg.cod_departamento,
                CodDiretoria = colaboradorOrg.cod_diretoria,
                DataAdmissao = colaboradorOrg.data_admissao,
                Departamento = colaboradorOrg.departamento,
                Diretoria = colaboradorOrg.diretoria,
                Idioma = colaboradorOrg.idioma,
                OrgId = colaboradorOrg.tb_org_id,
                ModeloContratacao = colaboradorOrg.modelo_contratacao,
                EmpresaRelacionada = colaboradorOrg.empresa_relacionada,
                ModeloTrabalho = colaboradorOrg.modelo_trabalho,
                DiasPorSemana = colaboradorOrg.dias_por_semana,
                ValorHora = colaboradorOrg.valor_hora,
                CustoHora = colaboradorOrg.custo_hora,
                BaseHoraMes = colaboradorOrg.base_hora_mes,
                Cpf = colaboradorOrg.codigo_interno_colaborador
            };
        }

        public List<ColaboradorOrgDTO> GetSubordinadosColaboradorOrg(string cpf, int orgId)
        {
            var ret = new List<ColaboradorOrgDTO>();
            var colaboradorOrg = _colaboradorContext.tb_colaborador_org.Where(x => x.codigo_interno_colaborador == cpf && x.tb_org_id == orgId).FirstOrDefault();
            if (colaboradorOrg == null)
                return null;
            var colaboradoresCodigo = _colaboradorContext.tb_colaborador_hierarquia
                .Where(x => x.cod_colaborador_superior == colaboradorOrg.cod_colaborador_externo && x.tb_org_id == orgId)
                .Select(x => x.cod_colaborador_externo).ToList();
            foreach (var codColaborador in colaboradoresCodigo)
            {
                var subordinadoRow = _colaboradorContext.tb_colaborador_org.Where(x => x.cod_colaborador_externo == codColaborador && x.tb_org_id == orgId).FirstOrDefault();
                if (subordinadoRow != null)
                    ret.Add(BuildColaboradorOrgDTO(subordinadoRow));
            }
            return ret;
        }

        public HashSet<string> ListarCodigosInternosHierarquiaTimeIncluindoGestor(string codigoInternoGestor, int orgId)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(codigoInternoGestor))
                return result;

            var gestorTrim = codigoInternoGestor.Trim();
            result.Add(gestorTrim);

            var conn = _dapperConnection.GetConnection();

            const string sqlCodExternoGestor = @"
                SELECT TRIM(cod_colaborador_externo) AS CodExterno
                FROM tb_colaborador_org
                WHERE tb_org_id = @OrgId
                  AND TRIM(codigo_interno_colaborador) = @CodigoInterno
                ORDER BY ativo DESC, data_alteracao DESC
                LIMIT 1";

            var codExternoRaiz = conn.QueryFirstOrDefault<string>(sqlCodExternoGestor,
                new { OrgId = orgId, CodigoInterno = gestorTrim });
            if (string.IsNullOrWhiteSpace(codExternoRaiz))
                return result;

            // Árvore completa (todos os níveis) via CTE recursiva: Denise→Geo→Samuel→Tiago etc.
            // Percorre só tb_colaborador_hierarquia; depois resolve codigo_interno em tb_colaborador_org.
            const string sqlDescendentesRecursivo = @"
                WITH RECURSIVE desc_ext AS (
                    SELECT TRIM(ch.cod_colaborador_externo) AS cod_ext
                    FROM tb_colaborador_hierarquia ch
                    WHERE ch.tb_org_id = @OrgId
                      AND TRIM(ch.cod_colaborador_superior) = @CodExternoRaiz
                    UNION
                    SELECT TRIM(ch.cod_colaborador_externo)
                    FROM tb_colaborador_hierarquia ch
                    INNER JOIN desc_ext d ON TRIM(ch.cod_colaborador_superior) = d.cod_ext
                    WHERE ch.tb_org_id = @OrgId
                )
                SELECT DISTINCT TRIM(co.codigo_interno_colaborador) AS CodigoInterno
                FROM desc_ext d
                INNER JOIN tb_colaborador_org co
                    ON TRIM(co.cod_colaborador_externo) = d.cod_ext
                   AND co.tb_org_id = @OrgId
                WHERE TRIM(co.codigo_interno_colaborador) <> ''";

            var parametrosRaiz = new { OrgId = orgId, CodExternoRaiz = codExternoRaiz.Trim() };
            foreach (var interno in conn.Query<string>(sqlDescendentesRecursivo, parametrosRaiz))
            {
                var i = (interno ?? string.Empty).Trim();
                if (i.Length > 0)
                    result.Add(i);
            }

            return result;
        }

        public bool ColaboradorEstaNaHierarquiaSubordinadaDoGestorNaOrg(string codigoInternoGestor, string codigoInternoColaborador, int orgId)
        {
            var g = (codigoInternoGestor ?? string.Empty).Trim();
            var c = (codigoInternoColaborador ?? string.Empty).Trim();
            if (g.Length == 0 || c.Length == 0)
                return false;
            if (string.Equals(g, c, StringComparison.OrdinalIgnoreCase))
                return true;

            var conn = _dapperConnection.GetConnection();

            const string sqlCodExterno = @"
                SELECT TRIM(cod_colaborador_externo) AS CodExterno
                FROM tb_colaborador_org
                WHERE tb_org_id = @OrgId
                  AND TRIM(codigo_interno_colaborador) = @CodigoInterno
                ORDER BY ativo DESC, data_alteracao DESC
                LIMIT 1";

            var gestorExt = conn.QueryFirstOrDefault<string>(sqlCodExterno, new { OrgId = orgId, CodigoInterno = g });
            var alvoExt = conn.QueryFirstOrDefault<string>(sqlCodExterno, new { OrgId = orgId, CodigoInterno = c });
            if (string.IsNullOrWhiteSpace(gestorExt) || string.IsNullOrWhiteSpace(alvoExt))
                return false;

            gestorExt = gestorExt.Trim();
            alvoExt = alvoExt.Trim();
            if (string.Equals(gestorExt, alvoExt, StringComparison.OrdinalIgnoreCase))
                return true;

            const string sqlSuperiores = @"
                SELECT TRIM(cod_colaborador_superior) AS Sup
                FROM tb_colaborador_hierarquia
                WHERE tb_org_id = @OrgId
                  AND TRIM(cod_colaborador_externo) = @CodExtSub";

            var visitados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var fila = new Queue<string>();
            fila.Enqueue(alvoExt);

            const int maxPassos = 500;
            for (var passo = 0; passo < maxPassos && fila.Count > 0; passo++)
            {
                var atual = fila.Dequeue();
                if (!visitados.Add(atual))
                    continue;

                var superiores = conn.Query<string>(sqlSuperiores, new { OrgId = orgId, CodExtSub = atual });
                foreach (var raw in superiores)
                {
                    var sup = (raw ?? string.Empty).Trim();
                    if (sup.Length == 0)
                        continue;
                    if (string.Equals(sup, gestorExt, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (!visitados.Contains(sup))
                        fila.Enqueue(sup);
                }
            }

            return false;
        }

        public List<SimpleColaboradorDTO> GetColaboradoresPorDiretoria(string cpf, string nome, string diretoria, int cursor, int limite, string codGestor, int orgId)
        {
            return _colaboradorContext.tb_colaborador_org
                .Where(x =>
                    (diretoria == null || diretoria == "0" || x.cod_diretoria == (diretoria ?? "")) &&
                    x.tb_org_id == orgId &&
                    x.ativo == 1 &&
                    x.codigo_interno_colaboradorNavigation.ativo == 1 &&
                    (
                        (!String.IsNullOrEmpty(cpf) && x.codigo_interno_colaborador.Contains(cpf)) ||
                        (!String.IsNullOrEmpty(nome) && x.codigo_interno_colaboradorNavigation.nome_completo.ToUpper().Contains(nome)) ||
                        (String.IsNullOrEmpty(cpf) && String.IsNullOrEmpty(nome))
                    ) &&
                    (String.IsNullOrEmpty(codGestor)
                        || codGestor == "null"
                        || codGestor == "0"
                        || _colaboradorContext.vw_colaboradores_gestor
                            .Where(y => y.cod_colaborador == x.cod_colaborador_externo && y.cod_gerente == codGestor && y.tb_org_id == x.tb_org_id).Any()
                    )
                )
                .OrderBy(x => x.codigo_interno_colaboradorNavigation.nome_completo)
                .Skip(cursor)
                .Take(limite)
                .Select(x => new SimpleColaboradorDTO { Cpf = x.codigo_interno_colaborador, NomeCompleto = x.codigo_interno_colaboradorNavigation.nome_completo })
                .ToList();
        }

        public ColaboradorOrgHierarquiaDTO BuscaColaboradorOrgHierarquia(string codColaboradorExterno, int orgId)
        {
            var superiorRow = _colaboradorContext.tb_colaborador_hierarquia.Where(x => x.tb_org_id == orgId && x.cod_colaborador_externo == codColaboradorExterno).ToList();
            if (!superiorRow.Any())
                return null;

            var superiorOrgRow = _colaboradorContext.tb_colaborador_org.Where(x => x.tb_org_id == orgId && x.cod_colaborador_externo == superiorRow.First().cod_colaborador_superior).FirstOrDefault();
            if (superiorOrgRow == null)
                return null;
            var usuarioOrgRow = _colaboradorContext.tb_usuario.Where(x => x.tb_org_id == orgId && x.codigo_interno_colaborador == superiorOrgRow.codigo_interno_colaborador).FirstOrDefault();
            return new ColaboradorOrgHierarquiaDTO
            {
                NomeProfissionalSuperior = superiorOrgRow.codigo_interno_colaboradorNavigation.nome_completo,
                CodProfissionalSuperior = superiorOrgRow.cod_colaborador_externo,
                CodigoInternoProfissionalSuperior = superiorOrgRow.codigo_interno_colaborador,
                EmailProfissionalSuperior = usuarioOrgRow?.email?? ""
            };
        }

        public bool ValidaAcesso(string cpf, int orgId, string token)
        {
            try
            {
                if (_colaboradorContext.tb_usuario_grupo_acesso
                    .Where(x => x.tb_usuario.codigo_interno_colaborador == cpf
                        && x.tb_grupo_acesso.tb_org_id == orgId
                        && x.ativo == ATIVO
                        && x.tb_grupo_acesso.nivel == 0
                        && x.tb_grupo_acesso.tb_grupo_acesso_funcionalidade_sistema
                        .Where(y => y.tb_funcionalidade_sistema.descricao == FuncionalidadeSistemaEnum.DADOS_COLABORADOR.ToString()).Any()
                    ).Any() || token == VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO))
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

        public List<HoleriteSimplesDTO> GetAllHolerite(string cpf)
        {
            var holerites = new List<HoleriteSimplesDTO>();

            var registroDb = _colaboradorContext.tb_colaborador_lg.Where(x => x.cpf == cpf).FirstOrDefault();

            if (registroDb != null)
            {
                var holeritesDB = _colaboradorContext.tb_holerite
                    .Where(x => x.id == registroDb.tb_holerite_id)
                    .OrderByDescending(x => x.competencia_ano)
                    .ThenByDescending(x => x.competencia_mes)
                    .ToList() ?? throw new Exception("Não possui holerites!");

                holerites.AddRange(
                    holeritesDB.Select(x => new HoleriteSimplesDTO
                    {
                        path = x.s3_url_arquivo,
                        ano = Convert.ToInt32(x.competencia_ano),
                        emissao = (DateTime)x.data_emissao,
                        mes = Convert.ToInt32(x.competencia_mes)
                    }).ToList()
                );
            }

            return holerites;
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

        public List<ColaboradoresOrgDTO> ListaColaboradoresOrg(int orgId, int cursor, int limite, bool fourtalents, string nomeOuEmail = "", string codExterno = "", List<string>? restricaoDiretorias = null)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();
                var inClauseDiretorias = restricaoDiretorias.BuildInClauseOrNull();

                var query = $@"
                    SELECT
                        tco.cod_colaborador_externo AS CodColaborador,
                        tco.codigo_interno_colaborador AS Cpf,
                        tc.documento_colaborador AS DocumentoColaborador,
                        tco.data_admissao AS DataAdmissao,
                        tco.ativo AS Ativo,
                        tco.modelo_contratacao AS ModeloContratacao,
                        tco.empresa_relacionada AS EmpresaRelacionada,
                        tco.modelo_trabalho AS ModeloTrabalho,
                        tco.dias_por_semana AS DiasPorSemana,
                        tco.valor_hora AS ValorHora,
                        tco.custo_hora AS CustoHora,
                        tco.base_hora_mes AS BaseHoraMes,
                        tc.contato_principal_ddi AS ContatoPrincipalDDI,
                        tc.contato_principal AS contatoPrincipal,
                        tu.primeiro_acesso_realizado AS PrimeiroAcessoRealizado,
                        tco.data_inativacao AS DataInativacao,
                        tu.email AS Email,
                        tu.id AS UsuarioId,
                        tc.nome_completo AS Nome,
                        tc.visualizar_busca_aderencia AS ConsiderarVisualizacaoAderencia,
                        IF(tbdt.codigo_interno_colaborador IS NOT NULL, 1, 0) AS ConsiderarBancoDeTalentos,
                        ti.path AS ImagemPath,
                        tco.tb_org_id AS Id,
                        org.descricao AS Descricao,
                        tco.cod_departamento AS Cod,
                        tco.departamento AS Departamento,
                        tco.cod_diretoria AS Cod,
                        tco.diretoria AS Diretoria,
                        vgco.cod_colaborador_externo_gestor AS CodigoProfissional,
                        vgco.nome_completo_gestor AS NomeGestor,
                        tco.cargo AS Cargo,
                        tco.codigo_cargo AS CodigoCargo
                    FROM
                        tb_colaborador_org tco
                    LEFT JOIN tb_colaborador tc
                        ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                    LEFT JOIN tb_imagem ti
                        ON ti.id = tc.imagem_id AND ti.ativo = 1
                    LEFT JOIN tb_usuario tu
                        ON tu.codigo_interno_colaborador = tco.codigo_interno_colaborador AND tu.tb_org_id = tco.tb_org_id
                    LEFT JOIN tb_org org
                        ON org.id = tco.tb_org_id
                    LEFT JOIN vw_gestores_colaboradores_org vgco
                        ON vgco.codigo_interno_colaborador_subordinado = tco.codigo_interno_colaborador
                        AND vgco.tb_org_id = tco.tb_org_id
                    LEFT JOIN tb_banco_talentos tbdt
                        ON tbdt.codigo_interno_colaborador = tco.cod_colaborador_externo
                    WHERE
                        tco.tb_org_id = @OrgId
                        AND (@Busca IS NULL OR
                            tu.email LIKE CONCAT('%', @Busca, '%') OR
                            tc.nome_completo LIKE CONCAT('%', @Busca, '%'))
                        AND (@Fourtalents = 0 OR (tco.cod_diretoria != 'BANCO TALENTOS' AND tco.ativo = 1))
                        AND (@CodExterno IS NULL OR tco.cod_colaborador_externo = @CodExterno)
                        {(inClauseDiretorias == null ? "" : $"AND tco.cod_diretoria IN {inClauseDiretorias}")}
                    ORDER BY tc.nome_completo
                    LIMIT @Limite
                    OFFSET @Cursor;
                ";
                var parametros = new
                {
                    OrgId = orgId,
                    Cursor = cursor,
                    Limite = limite,
                    Busca = nomeOuEmail,
                    CodExterno = codExterno.ToNullSeTextoNull(),
                    Fourtalents = fourtalents ? 1 : 0
                };

                var result = connection.Query<
                    ColaboradoresOrgDTO,
                    OrgDTO,
                    DepartamentoColaboradorDTO,
                    DiretoriaColaboradorDTO,
                    GestorDTO,
                    CargoColaboradorOrgDTO,
                    ColaboradoresOrgDTO
                >
                    (query, (colaborador, org, departamento, diretoria, gestor, cargoColaborador) =>
                    {
                        colaborador.Org = org;
                        colaborador.DepartamentoColaborador = departamento;
                        colaborador.DiretoriaColaborador = diretoria;
                        colaborador.Gestor = gestor;
                        colaborador.CargoColaborador = cargoColaborador;
                        return colaborador;
                    },
                    splitOn: "Id, Cod, Cod, CodigoProfissional, Cargo",
                    param: parametros);

                var list = result.AsList();
                foreach (var c in list)
                {
                    if (string.IsNullOrWhiteSpace(c.ImagemPath))
                        continue;
                    try
                    {
                        c.UrlFoto = serviceMidia + c.ImagemPath;
                    }
                    catch
                    {
                        // Igual GetColaborador: em falha, sem URL.
                    }
                    c.ImagemPath = null;
                }

                return list;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public string BuscaColaboradorPorCodigoExterno(string codigoColaborador, int orgId)
        {
            var colaborador = _colaboradorContext.tb_colaborador_org.Where(x => x.tb_org_id == orgId && x.cod_colaborador_externo == codigoColaborador).FirstOrDefault();
            if (colaborador == null)
                return null;
            return colaborador.codigo_interno_colaborador;
        }
        public async Task<List<dynamic>> RelatorioColaboradoresSkills(int orgId, bool ComHardskills)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@orgId", orgId, DbType.Int32);
                    parameters.Add("@hardskills", ComHardskills, DbType.Boolean);

                    var colabCompetenciaList = await _connection.QueryAsync<dynamic>(
                        "spr_rpt_relatorio_colaboradores_skills",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return colabCompetenciaList.ToList();
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

        public List<KeyValuePair<string, List<int?>>> GetAllColaboradoresCodigoInternoComOrgs()
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    var query = @"  SELECT
                                        tco.tb_org_id,
                                        tc.codigo_interno_colaborador
                                    FROM
                                        tb_colaborador_org tco
                                    INNER JOIN
                                        tb_colaborador tc on tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                                    WHERE
                                        tco.ativo = 1";

                    var result = _connection.Query<dynamic>(query);

                    return result.Select(x =>
                        new { codigo_interno_colaborador = x.codigo_interno_colaborador as string, tb_org_id = x.tb_org_id as int? })
                        .GroupBy(x => x.codigo_interno_colaborador)
                        .Select(x => new KeyValuePair<string, List<int?>>(x.Key, x.Select(y => y.tb_org_id).ToList()))
                        .ToList();
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

        public void AtualizaInfoLinkedin(string codigoInternoColaborador, string urlLinkedin)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var result = _connection.Execute("update tb_colaborador set url_linkedin = @UrlLinkedin, data_sync_linkedin = @DataAtual where codigo_interno_colaborador = @CodigoInternoColaborador",
                        new { UrlLinkedin = urlLinkedin, DataAtual = DateTime.UtcNow, CodigoInternoColaborador = codigoInternoColaborador });
                    if (result == 0)
                        throw new Exception("Falha de SQL ao atualizar as informaçoes do linkedin do colaborador");
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

        public List<string> BuscaColaborador(string coluna, string busca, int candidato, string cpf)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    string query = $@"
                                        SELECT
                                            codigo_interno_colaborador
                                        FROM
                                            buscacolaborador c
                                        WHERE
                                            {coluna} LIKE @Busca
                                            AND ativo = 1
                                            AND candidato = @Candidato
                                            AND cpf_colaborador <> @Cpf";

                    var parameters = new
                    {
                        Busca = "%" + busca.Replace(' ', '%') + "%",
                        Candidato = candidato,
                        Cpf = cpf
                    };

                    return _connection.Query<string>(query, parameters, commandType: CommandType.Text).ToList();
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

        public async Task<List<AniversariantesSemanaColaboradorDTO>> GetColaboradoresAniversariantesDaSemana(int orgId, string? codDiretoria, int? proximosDiasQuantidade, bool ocultarEmail)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    var query = @"
                        SELECT
                            tco.codigo_interno_colaborador AS CodigoColaborador,
                            tc.nome_completo AS Nome,
                            tc.data_nascimento AS DataNascimento,
                            tu.email AS Email,
                            tc.imagem_id ImagemId,
                            CASE DAYOFWEEK(STR_TO_DATE(CONCAT(YEAR(CURDATE()), '-', MONTH(tc.data_nascimento), '-', DAY(tc.data_nascimento)), '%Y-%m-%d'))
                                WHEN 1 THEN 'Domingo'
                                WHEN 2 THEN 'Segunda-feira'
                                WHEN 3 THEN 'Terça-feira'
                                WHEN 4 THEN 'Quarta-feira'
                                WHEN 5 THEN 'Quinta-feira'
                                WHEN 6 THEN 'Sexta-feira'
                                WHEN 7 THEN 'Sábado'
                            END AS DiaDaSemana,

                            -- Calcular dias até próximo aniversário
                            DATEDIFF(
                                STR_TO_DATE(CONCAT(
                                    IF(
                                        MONTH(tc.data_nascimento) < MONTH(CURDATE())
                                        OR (MONTH(tc.data_nascimento) = MONTH(CURDATE()) AND DAY(tc.data_nascimento) < DAY(CURDATE())),
                                        YEAR(CURDATE()) + 1, YEAR(CURDATE())
                                    ),
                                    '-', MONTH(tc.data_nascimento), '-', DAY(tc.data_nascimento)
                                ), '%Y-%m-%d'),
                                CURDATE()
                            ) AS DiasProximos
                        FROM
                            tb_colaborador tc
                        INNER JOIN
                            tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                        INNER JOIN
                            tb_usuario tu ON tu.codigo_interno_colaborador = tc.codigo_interno_colaborador AND tu.tb_org_id = @OrgId
                        WHERE
                            tco.tb_org_id = @OrgId
                            AND tc.oculta_aniversariante = 0
                            AND tco.ativo = 1
                            AND tc.oculta_aniversariante = 0
                            AND (@CodDiretoria IS NULL OR tco.cod_diretoria = @CodDiretoria)
                            AND (
                                -- Se proximosDiasQuantidade informado, busca até esse limite
                                (
                                    @ProximosDiasQuantidade IS NOT NULL
                                    AND DATEDIFF(
                                        STR_TO_DATE(CONCAT(
                                            IF(
                                                MONTH(tc.data_nascimento) < MONTH(CURDATE())
                                                OR (MONTH(tc.data_nascimento) = MONTH(CURDATE()) AND DAY(tc.data_nascimento) < DAY(CURDATE())),
                                                YEAR(CURDATE()) + 1, YEAR(CURDATE())
                                            ),
                                            '-', MONTH(tc.data_nascimento), '-', DAY(tc.data_nascimento)
                                        ), '%Y-%m-%d'),
                                        CURDATE()
                                    ) BETWEEN 0 AND @ProximosDiasQuantidade
                                )
                                OR
                                -- Caso contrário, aplica a lógica da semana
                                (
                                    @ProximosDiasQuantidade IS NULL
                                    AND (
                                        STR_TO_DATE(CONCAT(YEAR(CURDATE()), '-', MONTH(tc.data_nascimento), '-', DAY(tc.data_nascimento)), '%Y-%m-%d')
                                            BETWEEN CURDATE() - INTERVAL WEEKDAY(CURDATE()) DAY
                                            AND CURDATE() + INTERVAL (6 - WEEKDAY(CURDATE())) DAY
                                        OR
                                        STR_TO_DATE(CONCAT(YEAR(CURDATE()) + 1, '-', MONTH(tc.data_nascimento), '-', DAY(tc.data_nascimento)), '%Y-%m-%d')
                                            BETWEEN CURDATE() - INTERVAL WEEKDAY(CURDATE()) DAY
                                            AND CURDATE() + INTERVAL (6 - WEEKDAY(CURDATE())) DAY
                                    )
                                )
                            )
                        ORDER BY MONTH(tc.data_nascimento), DAY(tc.data_nascimento) ASC;";

                    var parametros = new
                    {
                        OrgId = orgId.ToString(),
                        CodDiretoria = string.IsNullOrEmpty(codDiretoria) ? null : codDiretoria,
                        ProximosDiasQuantidade = proximosDiasQuantidade,
                    };

                    var resultQuery = await _connection.QueryAsync<AniversariantesSemanaColaboradorDTO>(query,  parametros);

                    foreach (var result in resultQuery)
                    {
                        if (ocultarEmail)
                        {
                            result.Email = null;
                        }
                        if (result.ImagemId != null)
                        {
                            var colaboradorFull = GetColaborador(result.CodigoColaborador, orgId);

                            result.UrlFoto = colaboradorFull.UrlFoto;
                            result.UrlFotoThumb = colaboradorFull.UrlFotoThumb;
                            result.UrlFotoThumbMini = colaboradorFull.UrlFotoThumbMini;
                            result.UrlFotoThumbVeryMini = colaboradorFull.UrlFotoThumbVeryMini;
                        }
                    }

                    return resultQuery.ToList();
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

        public async Task<List<AnalistaResponsavelDTO>> BuscarAnalistasResponsaveis(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT
                    tco.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tc.nome_completo AS Nome
                FROM
                    tb_colaborador_org tco
                LEFT JOIN tb_colaborador tc
                    ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                    AND tc.ativo = 1
                WHERE
                    tco.tb_org_id = 2
                    AND tco.ativo = 1
                    AND tco.cargo LIKE '%Recrutamento e Seleção%'
                ORDER BY 
                    tc.nome_completo;
            ";

            var parametros = new
            {
                OrgId = orgId
            };

            var result = await connection.QueryAsync<AnalistaResponsavelDTO>(query, parametros);
            return result.ToList();
        }

        public async Task<bool> ExisteColaboradorComEsteEmail(string email)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        SELECT EXISTS (
                                       SELECT 
					                         1
					                     FROM 
					                         tb_colaborador tb
					                     WHERE 
					                         tb.email_alternativo = @email
                                   );";

            var parameters = new
            {
                email
            };

            return await connection.ExecuteScalarAsync<bool>(query, parameters);
        }

        public async Task<bool> ExisteColaboradorComEsteLinkedin(string perfilIN)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        SELECT EXISTS (
                                       SELECT 
					                         1
					                     FROM 
					                         tb_colaborador tb
					                     WHERE 
					                         tb.url_linkedin LIKE CONCAT('%', @perfilIN, '%')
                                   );";

            var parameters = new
            {
                perfilIN
            };

            return await connection.ExecuteScalarAsync<bool>(query, parameters);
        }

        public async Task<string> GetCodigoInternoColaboradorByLinkedin(string perfilIN)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        SELECT       
                            tb.codigo_interno_colaborador
                        FROM 
                            tb_colaborador tb
                        WHERE tb.url_linkedin LIKE CONCAT('%', @perfilIN, '%')";

            var parameters = new
            {
                perfilIN
            };

            return await connection.ExecuteScalarAsync<string>(query, parameters);
        }

        public async Task<bool> QualificarColaborador(string cpf, string cpfUsuarioLogado)
        {
            var connection = _dapperConnection.GetConnection();
            
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Atualiza o campo qualificado na tabela tb_colaborador
                    var queryUpdate = @"
                        UPDATE tb_colaborador 
                        SET qualificado = 1 
                        WHERE codigo_interno_colaborador = @cpf;";

                    var parametersUpdate = new { cpf };
                    var rowsAffected = await connection.ExecuteAsync(queryUpdate, parametersUpdate, transaction);

                    if (rowsAffected > 0)
                    {
                        // Registra o log de qualificação
                        var queryLog = @"
                            INSERT INTO tb_colaborador_qualificacao_log (
                                tb_colaborador_codigo_interno_colaborador_qualificado,
                                tb_colaborador_codigo_interno_colaborador_qualificador,
                                qualificado,
                                data_criacao
                            ) VALUES (
                                @cpf,
                                @cpfUsuarioLogado,
                                1,
                                NOW()
                            );";

                        var parametersLog = new { cpf, cpfUsuarioLogado };
                        await connection.ExecuteAsync(queryLog, parametersLog, transaction);

                        transaction.Commit();
                        return true;
                    }

                    transaction.Rollback();
                    return false;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<bool> DesqualificarColaborador(string cpf, string cpfUsuarioLogado)
        {
            var connection = _dapperConnection.GetConnection();
            
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Atualiza o campo qualificado na tabela tb_colaborador
                    var queryUpdate = @"
                        UPDATE tb_colaborador 
                        SET qualificado = 0 
                        WHERE codigo_interno_colaborador = @cpf;";

                    var parametersUpdate = new { cpf };
                    var rowsAffected = await connection.ExecuteAsync(queryUpdate, parametersUpdate, transaction);

                    if (rowsAffected > 0)
                    {
                        // Registra o log de desqualificação
                        var queryLog = @"
                            INSERT INTO tb_colaborador_qualificacao_log (
                                tb_colaborador_codigo_interno_colaborador_qualificado,
                                tb_colaborador_codigo_interno_colaborador_qualificador,
                                qualificado,
                                data_criacao
                            ) VALUES (
                                @cpf,
                                @cpfUsuarioLogado,
                                0,
                                NOW()
                            );";

                        var parametersLog = new { cpf, cpfUsuarioLogado };
                        await connection.ExecuteAsync(queryLog, parametersLog, transaction);

                        transaction.Commit();
                        return true;
                    }

                    transaction.Rollback();
                    return false;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<ColaboradorBasicoDTO> GetColaboradorBasicoPorCodigo(string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT 
                    codigo_interno_colaborador AS CodigoInternoColaborador,
                    nome_completo AS NomeCompleto,
                    data_nascimento AS DataNascimento,
                    rg AS Rg,
                    matricula AS Matricula,
                    endereco_id AS EnderecoId,
                    ativo AS Ativo,
                    data_criacao AS DataCriacao,
                    data_alteracao AS DataAlteracao,
                    contato_principal_ddi AS ContatoPrincipalDDI,
                    contato_principal AS ContatoPrincipal,
                    contato_outro AS ContatoOutro,
                    imagem_id AS ImagemId,
                    candidato AS Candidato,
                    passaporte AS Passaporte,
                    colaborador_saude_id AS ColaboradorSaudeId,
                    estado_civil AS EstadoCivil,
                    genero AS Genero,
                    etnia AS Etnia,
                    orientacao_sexual AS OrientacaoSexual,
                    escolaridade AS Escolaridade,
                    refugiado AS Refugiado,
                    email_alternativo AS EmailAlternativo,
                    nacionalidade AS Nacionalidade,
                    documento_colaborador AS DocumentoColaborador,
                    url_linkedin AS UrlLinkedin,
                    data_sync_linkedin AS DataSyncLinkedin,
                    visualizar_busca_aderencia AS VisualizarBuscaAderencia,
                    qualificado AS Qualificado
                FROM tb_colaborador 
                WHERE codigo_interno_colaborador = @codigoInternoColaborador;";

            var parameters = new { codigoInternoColaborador };
            var result = await connection.QueryFirstOrDefaultAsync<ColaboradorBasicoDTO>(query, parameters);
            
            return result;
        }

        public async Task<List<ColaboradorQualificadoDTO>> GetColaboradoresQualificadosPorColaboradorQualificador(string codigoInternoColaborador)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT DISTINCT
                    tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                    tc.nome_completo AS NomeCompleto,
                    tc.email_alternativo AS EmailAlternativo,
                    MAX(tcql.data_criacao) AS DataQualificacao
                FROM tb_colaborador_qualificacao_log tcql
                INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tcql.tb_colaborador_codigo_interno_colaborador_qualificado
                WHERE tcql.tb_colaborador_codigo_interno_colaborador_qualificador = @codigoInternoColaborador
                AND tcql.qualificado = 1
                GROUP BY tc.codigo_interno_colaborador, tc.nome_completo, tc.email_alternativo, tcql.tb_colaborador_codigo_interno_colaborador_qualificador, tc.ativo, tc.candidato
                ORDER BY DataQualificacao DESC;";

            var parameters = new { codigoInternoColaborador };
            var result = await connection.QueryAsync<ColaboradorQualificadoDTO>(query, parameters);
            
            return result.ToList();
        }

        public async Task<List<OrganizacaoCandidatoDTO>> GetOrganizacoesColaborador(string codigoInternoColaborador)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();

            try
            {
                var query = @"
                    SELECT 
                        COALESCE(tor_banco.id, tor_colaborg.id) as OrgId,
                        COALESCE(tor_banco.descricao, tor_colaborg.descricao) as OrgDescricao,
                        tco.ativo as AtivoNaOrg,
                        banco.tipo_cadastro as TipoCadastroBancoDeTalentos,
                        tor_banco.id as OrgIdTbBancoTalento,
                        tor_colaborg.id as OrgIdTbColaboradorOrg,
                        tco.cod_diretoria as CodDiretoria
                    FROM 
                        tb_colaborador tc
                    LEFT JOIN 
                        tb_colaborador_org tco ON tco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    LEFT JOIN 
                        tb_banco_talentos banco ON banco.codigo_interno_colaborador = tc.codigo_interno_colaborador
                    LEFT JOIN 
                        tb_org tor_banco ON banco.tb_org_id = tor_banco.id
                    LEFT JOIN 
                        tb_org tor_colaborg ON tco.tb_org_id = tor_colaborg.id
                    WHERE 
                        tc.codigo_interno_colaborador = @codigoInternoColaborador
                        AND (tor_banco.id IS NOT NULL OR tor_colaborg.id IS NOT NULL);";

                var parameters = new { codigoInternoColaborador };
                var result = await connection.QueryAsync<OrganizacaoCandidatoDTO>(query, parameters);
            
                return result.ToList();
            }
            catch (Exception)
            {
                return new List<OrganizacaoCandidatoDTO>();
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public void InserirDadosIA(ColaboradorIADTO colaboradorIA)
        {
            var sql = @"INSERT INTO tb_colaborador_ia 
                        (id, codigo_interno_colaborador, email, phone, first_name, last_name, headline, summary, 
                         location_name, industry_name, url_linkedin, hard_skills, soft_skills, dominio_negocios, 
                         metodologias, data_criacao, data_alteracao) 
                        VALUES 
                        (@Id, @CodigoInternoColaborador, @Email, @Phone, @FirstName, @LastName, @Headline, @Summary,
                         @LocationName, @IndustryName, @UrlLinkedin, @HardSkills, @SoftSkills, @DominioNegocios,
                         @Metodologias, @DataCriacao, @DataAlteracao)";

            var connection = _dapperConnection.GetConnection();

            connection.Execute(sql, new
            {
                colaboradorIA.Id,
                colaboradorIA.CodigoInternoColaborador,
                colaboradorIA.Email,
                colaboradorIA.Phone,
                colaboradorIA.FirstName,
                colaboradorIA.LastName,
                colaboradorIA.Headline,
                colaboradorIA.Summary,
                colaboradorIA.LocationName,
                colaboradorIA.IndustryName,
                colaboradorIA.UrlLinkedin,
                colaboradorIA.HardSkills,
                colaboradorIA.SoftSkills,
                colaboradorIA.DominioNegocios,
                colaboradorIA.Metodologias,
                DataCriacao = DateTime.Now,
                DataAlteracao = DateTime.Now
            });
        }
        
        public async Task<string> BuscaNomeColaboradorPorEmail(string email)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT tc.nome_completo
                FROM tb_usuario tu
                INNER JOIN tb_colaborador tc ON tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
                WHERE tu.email = @Email
                LIMIT 1;
            ";

            return await connection.QuerySingleOrDefaultAsync<string>(query,
                new { Email = email });
        }

        public async Task<IEnumerable<OrigemColaboradorDTO>> ListarOrigensColaborador()
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                            SELECT 
                                id AS Id,
                                descricao AS Descricao,
                                ativo AS Ativo
                            FROM 
                                tb_origem_colaborador
                            ORDER BY 
                                descricao";

            return await connection.QueryAsync<OrigemColaboradorDTO>(query);
        }
        
        public async Task AtualizaInfoLinkedinAsync(string codigoInternoColaborador, string urlLinkedin)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                UPDATE tb_colaborador 
                SET 
                    url_linkedin = @UrlLinkedin, 
                    data_sync_linkedin = @DataAtual 
                WHERE 
                    codigo_interno_colaborador = @CodigoInternoColaborador
            ";
            
            await connection.ExecuteAsync(query,
            new
            {
                UrlLinkedin = urlLinkedin, 
                DataAtual = DateTime.UtcNow, 
                CodigoInternoColaborador = codigoInternoColaborador
            });
        }

        public async Task<List<DiretoriaColaboradorDTO>> ListarDiretoriasDistintasPorOrgAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                SELECT DISTINCT tco.cod_diretoria AS Cod, tco.diretoria AS Diretoria
                FROM tb_colaborador_org tco
                WHERE tco.tb_org_id = @OrgId
                  AND tco.ativo = 1
                  AND tco.cod_diretoria IS NOT NULL
                  AND TRIM(tco.cod_diretoria) <> ''
                ORDER BY tco.diretoria";
            var rows = (await connection.QueryAsync<DiretoriaColaboradorDTO>(query, new { OrgId = orgId })).ToList();
            return rows;
        }
    }
}
using Colaboracao.Core.Interfaces;
using Core.Domain.BI;
using Dapper;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.BI;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Dominio;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.Metodologia;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Org;
using DataTransferObject.Domain.Softskill;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.BI
{
    public class ColaboradorBIRepository : IColaboradorBIRepository
    {
        private IConnectionStringCore _connectionString;

        public ColaboradorBIRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }
        public List<ColaboradorBIDTO> GetAllColaboradoresBI()
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"  select
                                        tc.codigo_interno_colaborador, tc.nome_completo, tc.data_nascimento, tc.endereco_id, tc.contato_principal_ddi,
                                        tc.contato_principal, tc.contato_outro, tc.colaborador_saude_id, tc.estado_civil, tc.genero, tc.etnia,
                                        tc.orientacao_sexual , tc.refugiado , tc.email_alternativo, tc.nacionalidade, tc.documento_colaborador, tu.email, tcs.pcd,
                                        tcs.grupo_risco_covid, tcs.condicao_saude_relevante, te.cep, te.endereco, te.numero, te.numero, te.complemento,
                                        te.bairro, te.cidade, te.estado, te.com_quem_mora, te.internacional_linha_um, te.internacional_linha_dois, tcso.descricao as sobre
                                    from
                                        tb_colaborador tc
                                        inner join tb_usuario tu on tu.codigo_interno_colaborador = tc.codigo_interno_colaborador
                                        left join tb_colaborador_saude tcs on tc.colaborador_saude_id = tcs.id
                                        left join tb_endereco te on tc.endereco_id = te.id and te.ativo = 1
                                        left join tb_colaborador_sobre tcso on tcso.codigo_interno_colaborador = tc.codigo_interno_colaborador";

                    return _connection.Query<dynamic>(query).Select(x =>
                        new ColaboradorBIDTO
                        {
                            ContatoOutros = x.contato_outro,
                            ContatoPrincipal = x.contato_principal,
                            ContatoPrincipalDDI = x.contato_principal_ddi,
                            Cpf = x.codigo_interno_colaborador,
                            DataNascimento = x.data_nascimento,
                            DocumentoColaborador = x.documento_colaborador,
                            Email = x.email,
                            Sobre = x.sobre,
                            EmailAlternativo = x.email_alternativo,
                            Endereco = x.endereco_id != null ? new EnderecoDTO
                            {
                                Bairro = x.bairro,
                                Cep = x.cep,
                                Cidade = x.cidade,
                                Complemento = x.complemento,
                                ComQuemMora = x.com_quem_mora,
                                Endereco = x.endereco,
                                Estado = x.estado,
                                Numero = x.numero,
                                InternacionalLinhaUm = x.internacional_linha_um,
                                InternacionalLinhaDois = x.internacional_linha_dois
                            } : null,
                            EstadoCivil = x.estado_civil,
                            Etnia = x.etnia,
                            Genero = x.genero,
                            Nacionalidade = x.nacionalidade,
                            NomeCompleto = x.nome_completo,
                            OrientacaoSexual = x.orientacao_sexual,
                            Saude = new ColaboradorSaudeDTO
                            {
                                CondicaoDeSaudeRelevante = x.condicao_saude_relevante,
                                GrupoDeRiscoCovid = x.grupo_risco_covid ?? (sbyte)0,
                                PCD = x.pcd != null ? (EnumPCD)Enum.Parse(typeof(EnumPCD), x.pcd) : EnumPCD.Nenhuma
                            }
                        }
                    ).ToList();
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

        public List<KeyValuePair<string, ListaOrgColaboradorDTO>> GetOrgInfoBI()
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"  select
                                        tco.codigo_interno_colaborador , tco.tb_org_id , tco.cod_diretoria , tco.diretoria , tco.cod_departamento ,
                                        tco.departamento , tco.codigo_cargo , tco.cargo , tco.cod_colaborador_externo , tco.ativo , tco.data_admissao,
                                        vgco.nome_completo_gestor , vgco.cod_colaborador_externo_gestor
                                    from
                                        tb_colaborador_org tco
                                        left join vw_gestores_colaboradores_org vgco
                                            on tco.tb_org_id = vgco.tb_org_id and vgco.codigo_interno_colaborador_subordinado = tco.codigo_interno_colaborador
                                    where tco.tb_org_id <> 1;";

                    return _connection.Query<dynamic>(query)
                        .Select(x => new KeyValuePair<string, ListaOrgColaboradorDTO>(x.codigo_interno_colaborador, new ListaOrgColaboradorDTO
                        {
                            ColaboradorOrg = new DataTransferObject.Domain.Org.ColaboradorOrgDTO
                            {
                                Ativo = x.ativo == (sbyte)1,
                                Cargo = x.cargo,
                                CodCargo = x.codigo_cargo,
                                CodColaborador = x.cod_colaborador_externo,
                                CodDepartamento = x.cod_departamento,
                                CodDiretoria = x.cod_diretoria,
                                Cpf = x.codigo_interno_colaborador,
                                DataAdmissao = x.data_admissao,
                                Departamento = x.departamento,
                                Diretoria = x.diretoria,
                                OrgId = x.tb_org_id
                            },
                            HierarquiaOrg = new ColaboradorOrgHierarquiaDTO
                            {
                                CodProfissionalSuperior = x.cod_colaborador_externo_gestor,
                                NomeProfissionalSuperior = x.nome_completo_gestor
                            }
                        })).ToList();
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

        public List<KeyValuePair<string, CompetenciaColaboradorDTO>> GetCompetenciaColaboradorBI()
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"  select
                                        tcc.id , tcc.codigo_interno_colaborador , tc.id as competencia_id, tcc.data_criacao ,
                                        tc.descricao as competencia_descricao, tc.confirmada , tc.ativo , tn.id as nivel_id,
                                        tn.descricao as nivel_descricacao
                                    from tb_colaborador_competencia tcc
                                        inner join tb_competencia tc on tc.id = tcc.competencia_id
                                        inner join tb_nivel tn on tn.id = tcc.tb_nivel_id ;";

                    return _connection.Query<dynamic>(query)
                        .Select(x => new KeyValuePair<string, CompetenciaColaboradorDTO>(x.codigo_interno_colaborador, new CompetenciaColaboradorDTO
                        {
                            ColaboradorCpf = x.codigo_interno_colaborador,
                            Competencia = new CompetenciaDTO
                            {
                                Ativo = x.ativo == (sbyte)1,
                                Descricao = x.competencia_descricao,
                                Pendente = x.confirmada == (sbyte)0,
                                Id = x.competencia_id
                            },
                            Data = x.data_criacao,
                            Id = x.id,
                            Nivel = new NivelDTO
                            {
                                Descricao = x.nivel_descricacao,
                                Id = x.nivel_id
                            }
                        })).ToList();
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

        public List<KeyValuePair<Tuple<string, long>, CertificadoDTO>> GetCertificadoCompetenciaColaboradorBI()
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"  select
                                        tcc.id , tcc.codigo_interno_colaborador,
                                        tc2.descricao as certiticado_descricao, tc2.ativo as certificado_ativo,
                                        tc2.instituicao  as certificado_instituicao, tc2.carga_horaria, tc2.data_conclusao, tc2.id as certificado_id
                                    from tb_colaborador_competencia tcc
                                        inner join tb_colaborador_competencia_certificado tccc on tccc.tb_colaborador_competencia_id = tcc.id
                                        inner join tb_certificado tc2 on tc2.id = tccc.tb_certificado_id;";

                    return _connection.Query<dynamic>(query)
                        .Select(x => new KeyValuePair<Tuple<string, long>, CertificadoDTO>(
                            new Tuple<string, long>(x.codigo_interno_colaborador, x.id),
                            new CertificadoDTO
                            {
                                ativo = x.certificado_ativo == (sbyte)1,
                                cargaHoraria = x.carga_horaria,
                                conclusao = x.data_conclusao,
                                descricao = x.certiticado_descricao,
                                IdCertificado = x.certificado_id,
                                instituicao = x.certificado_instituicao,
                                Principal = false
                            }
                        )).ToList();
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

        public List<KeyValuePair<string, SoftskillColaboradorDTO>> GetSoftskillColaboradorBI()
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"  select
                                        tcs.id , tcs.codigo_interno_colaborador , ts.id as competencia_id,
                                        ts.descricao, ts.confirmada , ts.ativo , tn.id as nivel_id,
                                        tn.descricao as nivel_descricacao
                                    from
                                        tb_colaborador_softskill tcs
                                        inner join tb_softskill ts on ts.id = tcs.softskill_id
                                        inner join tb_nivel tn on tn.id = tcs.tb_nivel_id;";

                    return _connection.Query<dynamic>(query)
                        .Select(x => new KeyValuePair<string, SoftskillColaboradorDTO>(x.codigo_interno_colaborador, new SoftskillColaboradorDTO
                        {
                            ColaboradorCpf = x.codigo_interno_colaborador,
                            SoftSkill = new ItemPerfilDTO
                            {
                                Descricao = x.descricao,
                                Pendente = x.confirmada == (sbyte)0,
                                Id = x.competencia_id
                            },
                            Data = x.data_criacao ?? DateTime.MinValue,
                            Id = x.id,
                            Nivel = new NivelDTO
                            {
                                Descricao = x.nivel_descricacao,
                                Id = x.nivel_id
                            }
                        })).ToList();
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

        public List<KeyValuePair<string, ListaMetodologiaColaboradorResult>> GetMetodologiaColaboradorBI()
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"  select
                                        tcm.id , tcm.codigo_interno_colaborador , tm.id as competencia_id,
                                        tm.descricao, tm.confirmada , tm.ativo , tn.id as nivel_id,
                                        tn.descricao as nivel_descricacao
                                    from tb_colaborador_metodologia tcm
                                        inner join tb_metodologia tm on tm.id = tcm.metodologia_id
                                        inner join tb_nivel tn on tn.id = tcm.tb_nivel_id;";

                    return _connection.Query<dynamic>(query)
                        .Select(x => new KeyValuePair<string, ListaMetodologiaColaboradorResult>(x.codigo_interno_colaborador, new ListaMetodologiaColaboradorResult
                        {
                            Metodologia = new MetodologiaDTO
                            {
                                Descricao = x.descricao,
                                Pendente = x.confirmada == (sbyte)0,
                                Id = x.competencia_id
                            },
                            Nivel = new NivelDTO
                            {
                                Descricao = x.nivel_descricacao,
                                Id = x.nivel_id
                            }
                        })).ToList();
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

        public List<KeyValuePair<string, DominioColaboradorDTO>> GetDominiColaboradorBI()
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"  select
                                        tcd.id , tcd.codigo_interno_colaborador , td.id as competencia_id,
                                        td.descricao, td.confirmada , td.ativo , tn.id as nivel_id,
                                        tn.descricao as nivel_descricacao
                                    from tb_colaborador_dominionegocio tcd
                                        inner join tb_dominionegocio td on td.id = tcd.dominionegocio_id
                                        inner join tb_nivel tn on tn.id = tcd.tb_nivel_id;";

                    return _connection.Query<dynamic>(query)
                        .Select(x => new KeyValuePair<string, DominioColaboradorDTO>(x.codigo_interno_colaborador, new DominioColaboradorDTO
                        {
                            Dominio = new ItemPerfilDTO
                            {
                                Descricao = x.descricao,
                                Pendente = x.confirmada == (sbyte)0,
                                Id = x.competencia_id
                            },
                            Nivel = new NivelDTO
                            {
                                Descricao = x.nivel_descricacao,
                                Id = x.nivel_id
                            }
                        })).ToList();
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

        public List<KeyValuePair<string, IdiomaColaboradorDTO>> GetIdiomaColaboradorBI()
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();
                    var query = @"  select
                                        tci.id , tci.codigo_interno_colaborador , ti.id as competencia_id,
                                        ti.descricao, ti.confirmada , ti.ativo , tn.id as nivel_id,
                                        tn.descricao as nivel_descricacao
                                    from tb_colaborador_idioma tci
                                        inner join tb_idioma ti on ti.id = tci.idioma_id
                                        inner join tb_nivel tn on tn.id = tci.tb_nivel_id;";

                    return _connection.Query<dynamic>(query)
                        .Select(x => new KeyValuePair<string, IdiomaColaboradorDTO>(x.codigo_interno_colaborador, new IdiomaColaboradorDTO
                        {
                            Idioma = new IdiomaDTO
                            {
                                Descricao = x.descricao,
                                Pendente = x.confirmada == (sbyte)0,
                                Id = (int)x.competencia_id
                            },
                            Nivel = new NivelDTO
                            {
                                Descricao = x.nivel_descricacao,
                                Id = x.nivel_id
                            }
                        })).ToList();
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
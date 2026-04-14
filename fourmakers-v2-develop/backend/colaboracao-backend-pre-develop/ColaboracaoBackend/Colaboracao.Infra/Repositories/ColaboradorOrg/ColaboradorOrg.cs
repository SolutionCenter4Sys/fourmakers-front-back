using Colaboracao.Core.Interfaces;
using Core.Domain;
using Dapper;
using DataTransferObject.Domain.Colaborador;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra
{
    public class BuscaColaboradorOrgRepository : IBuscaColaboradorOrgRepository
    {
        private readonly IDBConnection _dapperConnection;
        public BuscaColaboradorOrgRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<ColaboradorEColaboradorOrgDTO> GetColaboradorEOrgsAsync(string documentoColaborador, string codigoInternoColaborador, int? orgId)
        {
            if (string.IsNullOrEmpty(documentoColaborador) && string.IsNullOrEmpty(codigoInternoColaborador))
                throw new ArgumentException("É obrigatório informar documento_colaborador ou codigo_interno_colaborador");

            // Limpar caracteres especiais do documento para pesquisa
            string documentoLimpo = null;
            if (!string.IsNullOrEmpty(documentoColaborador))
            {
                documentoLimpo = documentoColaborador.Replace(".", "").Replace(",", "").Replace("-", "").Replace(" ", "");
            }

            var connection = _dapperConnection.GetConnection();

            var sql = @"
                    SELECT 
                        tc.codigo_interno_colaborador AS CodigoInternoColaborador,
                        tc.nome_completo AS NomeCompleto,
                        tc.data_nascimento AS DataNascimento,
                        tc.rg AS Rg,
                        tc.matricula AS Matricula,
                        tc.endereco_id AS EnderecoId,
                        tc.ativo AS Ativo,
                        tc.data_criacao AS DataCriacao,
                        tc.data_alteracao AS DataAlteracao,
                        tc.contato_principal_ddi AS ContatoPrincipalDdi,
                        tc.contato_principal AS ContatoPrincipal,
                        tc.contato_outro AS ContatoOutro,
                        tc.imagem_id AS ImagemId,
                        tc.candidato AS Candidato,
                        tc.passaporte AS Passaporte,
                        tc.colaborador_saude_id AS ColaboradorSaudeId,
                        tc.estado_civil AS EstadoCivil,
                        tc.genero AS Genero,
                        tc.etnia AS Etnia,
                        tc.orientacao_sexual AS OrientacaoSexual,
                        tc.escolaridade AS Escolaridade,
                        tc.refugiado AS Refugiado,
                        tc.email_alternativo AS EmailAlternativo,
                        tc.nacionalidade AS Nacionalidade,
                        tc.documento_colaborador AS DocumentoColaborador,
                        tc.url_linkedin AS UrlLinkedin,
                        tc.data_sync_linkedin AS DataSyncLinkedin,
                        tc.visualizar_busca_aderencia AS VisualizarBuscaAderencia,
                        tc.qualificado AS Qualificado,
                        tco.tb_org_id AS TbOrgId,
                        tco.cod_diretoria AS CodDiretoria,
                        tco.diretoria AS Diretoria,
                        tco.departamento AS Departamento,
                        tco.cod_departamento AS CodDepartamento,
                        tco.data_criacao AS DataCriacao,
                        tco.data_alteracao AS DataAlteracao,
                        tco.cargo AS Cargo,
                        tco.codigo_cargo AS CodigoCargo,
                        tco.cod_colaborador_externo AS CodColaboradorExterno,
                        tco.data_admissao AS DataAdmissao,
                        tco.ativo AS Ativo,
                        tco.data_inativacao AS DataInativacao,
                        tco.modelo_contratacao AS ModeloContratacao,
                        tco.empresa_relacionada AS EmpresaRelacionada,
                        tco.modelo_trabalho AS ModeloTrabalho,
                        tco.dias_por_semana AS DiasPorSemana,
                        tco.valor_hora AS ValorHora,
                        tco.custo_hora AS CustoHora,
                        tco.base_hora_mes AS BaseHoraMes,
                        tco.idioma AS Idioma
                    FROM 
                        tb_colaborador tc
                    JOIN
                        tb_colaborador_org tco ON tc.codigo_interno_colaborador = tco.codigo_interno_colaborador
                    WHERE
                        (@DocumentoLimpo IS NULL OR REPLACE(REPLACE(REPLACE(REPLACE(tc.documento_colaborador, '.', ''), ',', ''), '-', ''), ' ', '') = @DocumentoLimpo)
                        AND (@CodigoInternoColaborador IS NULL OR tc.codigo_interno_colaborador = @CodigoInternoColaborador)
                        AND (@OrgId IS NULL OR tco.tb_org_id = @OrgId)";

            ColaboradorEColaboradorOrgDTO colaborador = null;

            await connection.QueryAsync<ColaboradorEColaboradorOrgDTO, ColaboradorOrgsDTO, ColaboradorEColaboradorOrgDTO>(
                sql,
                (col, colOrg) =>
                {
                    if (colaborador == null)
                    {
                        colaborador = col;
                        colaborador.ColaboradorOrgs = new List<ColaboradorOrgsDTO>();
                    }

                    if (colOrg != null)
                    {
                        colaborador.ColaboradorOrgs.Add(colOrg);
                    }

                    return colaborador;
                },
                new {   
                        DocumentoLimpo = documentoLimpo,
                        CodigoInternoColaborador = codigoInternoColaborador,
                        OrgId = orgId
                },
                splitOn: "TbOrgId"
            );

            return colaborador;

        }
    }
}
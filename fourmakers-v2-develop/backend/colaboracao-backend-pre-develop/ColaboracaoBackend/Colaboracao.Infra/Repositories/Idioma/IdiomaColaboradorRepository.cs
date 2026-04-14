using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Context;
using Core.Domain.IIdioma;
using Dapper;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Idioma
{
    public class IdiomaColaboradorRepository : IIdiomaColaboradorRepository
    {
        private const int ATIVO = 1;
        private const int NAO_ATIVO = 0;
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IDBConnection _dapperConnection;

        public IdiomaColaboradorRepository(ColaboradorContext colaboradorContext, IDBConnection dapperConnection)
        {
            _colaboradorContext = colaboradorContext;
            _dapperConnection = dapperConnection;
        }

        public List<IdiomaColaboradorDTO> ListarIdiomaColaborador(IdiomaColaboradorDTO model)
        {
            try
            {
                var connection = _dapperConnection.GetConnection();
                var query = @"
                    SELECT
                        ci.id AS Id,
                        ci.data_criacao AS Data,
                        ci.codigo_interno_colaborador AS ColaboradorCpf,
                        ci.idioma_id AS IdIdioma,
                        ci.tb_nivel_id AS IdNivel,
                        i.id AS Id,
                        i.descricao AS Descricao,
                        i.confirmada AS Pendente,
                        n.id AS Id,
                        n.descricao AS Descricao
                    FROM
                        tb_colaborador_idioma ci
                    INNER JOIN
                        tb_idioma i ON ci.idioma_id = i.id
                    LEFT JOIN
                        tb_nivel n ON ci.tb_nivel_id = n.id
                    WHERE
                        ci.codigo_interno_colaborador = @ColaboradorCpf
                        AND ci.ativo = 1
                        AND i.ativo = 1
                    ORDER BY
                        i.descricao";

                var parametros = new { model.ColaboradorCpf };

                var resultado = connection.Query<IdiomaColaboradorDTO, IdiomaDTO, NivelDTO, IdiomaColaboradorDTO>(
                    query,
                    (dto, idioma, nivel) =>
                    {
                        dto.Idioma = idioma;
                        dto.Nivel = nivel ?? new NivelDTO { Id = 32, Descricao = "A definir" };
                        return dto;
                    },
                    parametros,
                    splitOn: "Id" //separar objetos pelo id para o dapper conseguir transformar em dtos (tb_colaborador_idioma/ tb_idioma/ tb_nivel)
                ).ToList();

                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IdiomaColaboradorDTO AdicionarIdiomaColaborador(IdiomaColaboradorDTO idioma)
        {
            try
            {
                var colabRow = _colaboradorContext.tb_colaborador.Find(idioma.ColaboradorCpf)
                    ?? throw new ValidationException("Colaborador não existe.");
                var itemRow = _colaboradorContext.tb_idioma.Find(idioma.IdIdioma)
                    ?? throw new ValidationException("Este Idioma não existe.");
                var itemColabRow = _colaboradorContext.tb_colaborador_idioma
                    .Where(x => x.codigo_interno_colaborador == idioma.ColaboradorCpf && x.idioma_id == idioma.IdIdioma).FirstOrDefault();

                var nivelRow = idioma.IdNivel != null ? _colaboradorContext.tb_nivel.Find(idioma.IdNivel) : null;

                if (itemColabRow != null)
                {
                    if (itemColabRow.ativo == ATIVO)
                    {
                        throw new ValidationException("Colaborador já possuí esse Idioma.");
                    }
                    else
                    {
                        itemColabRow.ativo = ATIVO;
                        itemColabRow.tb_nivel_id = idioma.IdNivel;
                        _colaboradorContext.SaveChanges();
                        idioma.Id = itemColabRow.id;
                        return idioma;
                    }
                }
                else
                {
                    var row = new tb_colaborador_idioma()
                    {
                        ativo = ATIVO,
                        codigo_interno_colaborador = colabRow.codigo_interno_colaborador,
                        idioma_id = itemRow.id,
                        tb_nivel_id = nivelRow?.id
                    };
                    _colaboradorContext.tb_colaborador_idioma.Add(row);
                    _colaboradorContext.SaveChanges();

                    idioma.Id = row.id;
                    return idioma;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoverIdiomaColaborador(IdiomaColaboradorDTO idiomaColaborador)
        {
            try
            {
                var itemColabRow = _colaboradorContext.tb_colaborador_idioma
                    .Where(x => x.codigo_interno_colaborador == idiomaColaborador.ColaboradorCpf && x.idioma_id == idiomaColaborador.IdIdioma && x.ativo == 1).FirstOrDefault()
                    ?? throw new Exception("Colaborador não possuí esse idioma!");

                itemColabRow.ativo = NAO_ATIVO;
                _colaboradorContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IdiomaColaboradorDTO AlterarIdiomaColaborador(IdiomaColaboradorDTO idiomaColaborador)
        {
            try
            {
                var colabRow = _colaboradorContext.tb_colaborador.Find(idiomaColaborador.ColaboradorCpf)
                        ?? throw new ValidationException("Colaborador não possui idiomas cadastrados.");
                var IdiomaRow = _colaboradorContext.tb_idioma.Find(idiomaColaborador.Idioma.Id)
                    ?? throw new ValidationException("Idioma não encontrado.");
                var IdiomaColaboradorRow = IdiomaRow.tb_colaborador_idioma
                    .Where(x => x.idioma_id == IdiomaRow.id && x.codigo_interno_colaborador == idiomaColaborador.ColaboradorCpf).FirstOrDefault()
                    ?? throw new Exception("Colaborador não possuí este Idioma");
                var nivelRow = _colaboradorContext.tb_nivel.Find(idiomaColaborador.Nivel.Id)
                    ?? throw new Exception("Nível do item não encontrado.");

                if (nivelRow.tb_item_perfil.descricao != "IDIOMA")
                    throw new Exception("Nível do item não pertence ao idioma.");

                IdiomaColaboradorRow.tb_nivel = nivelRow;
                IdiomaColaboradorRow.ativo = ATIVO;
                _colaboradorContext.SaveChanges();

                idiomaColaborador.Id = IdiomaColaboradorRow.id;

                return idiomaColaborador;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
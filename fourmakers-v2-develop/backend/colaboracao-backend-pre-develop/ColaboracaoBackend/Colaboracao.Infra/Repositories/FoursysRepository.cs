using Colaboracao.Infra.Context;
using Core.DomainModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Extension;
using Dapper;
using DataTransferObject.Domain.Cargo;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Foursys;

namespace Colaboracao.Infra.Repositories
{
    public class FoursysRepository : IFoursysDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IDBConnection _dapperConnection;
        private int ID_FOURSYS = 2;

        public FoursysRepository(ColaboradorContext colaboradorContext, IDBConnection dapperConnection)
        {
            this._colaboradorContext = colaboradorContext;
            _dapperConnection = dapperConnection;
        }

        public CargoDTO AlteraCargo(int id, string cargo)
        {
            var cargoRow = _colaboradorContext.tb_cargo.Find(id);
            if (cargoRow == null) throw new Exception("Cargo não encontrado.");
            cargoRow.descricao = cargo;
            _colaboradorContext.SaveChanges();
            return new CargoDTO { Id = cargoRow.id, Cargo = cargoRow.descricao };
        }

        public CargoDTO BuscarCargo(int id)
        {
            var row = _colaboradorContext.tb_cargo.Where(x => x.ativo == 1 && x.id == id).FirstOrDefault();
            if (row == null) throw new Exception("Cargo não encontrado");
            return new CargoDTO { Id = row.id, Cargo = row.descricao };
        }

        public BuscaCargoResult BuscarCargos(string busca, int cursor, int limite)
        {
            var result = new BuscaCargoResult();

            List<tb_cargo> items;
            if (string.IsNullOrEmpty(busca))
            {
                items = _colaboradorContext.tb_cargo
                    .Where(x => x.ativo == 1)
                    .OrderBy(x => x.descricao)
                    .Skip(cursor)
                    .Take(limite)
                    .ToList();

                result.TotalResultCount = _colaboradorContext.tb_cargo.Count(x => x.ativo == 1);
            }
            else
            {
                var filteredQuery = _colaboradorContext.tb_cargo
                    .Where(x => x.ativo == 1 && EF.Functions.Like(x.descricao, "%" + busca + "%"))
                    .OrderBy(x => x.descricao);

                items = filteredQuery
                    .Skip(cursor)
                    .Take(limite)
                    .ToList();

                result.TotalResultCount = filteredQuery
                    .Skip(cursor)
                    .Count();
            }

            result.FilteredResultCount = items.Count;
            result.Cargos = items.Select(row => new CargoDTO { Id = row.id, Cargo = row.descricao }).ToList();
            return result;
        }

        public BuscaDiretoriaResult BuscarDiretorias(string busca, int cursor, int limite)
        {
            var result = new BuscaDiretoriaResult();

            var baseQuery = _colaboradorContext.tb_colaborador_org
                .Where(x => x.tb_org_id == ID_FOURSYS)
                .GroupBy(x => new { x.cod_diretoria, x.diretoria })
                .Select(x => new { id = x.Key.cod_diretoria, descricao = x.Key.diretoria })
                .ToList();

            result.TotalResultCount = baseQuery.Count;

            IEnumerable<dynamic> filtered = baseQuery;
            if (!string.IsNullOrEmpty(busca))
                filtered = baseQuery.Where(x => x.descricao != null && x.descricao.Contains(busca, StringComparison.OrdinalIgnoreCase));

            var items = filtered.OrderBy(x => x.descricao).Skip(cursor).Take(limite).ToList();

            result.Diretorias = items.Select(row => new DiretoriaDTO { Id = row.id, Diretoria = row.descricao }).ToList();
            return result;
        }

        public void DeletaCargo(int id)
        {
            var cargoRow = _colaboradorContext.tb_cargo.Find(id);
            if (cargoRow == null) throw new Exception("Cargo não encontrado.");
            cargoRow.ativo = 0;
            _colaboradorContext.SaveChanges();
        }

        public CargoDTO InsereCargo(string cargo)
        {
            var cargoRow = new tb_cargo();
            cargoRow.descricao = cargo;
            cargoRow.ativo = 1;
            _colaboradorContext.tb_cargo.Add(cargoRow);
            _colaboradorContext.SaveChanges();
            return new CargoDTO { Id = cargoRow.id, Cargo = cargoRow.descricao };
        }

        public List<UnidadesDTO> ListarUnidades()
        {
            return _colaboradorContext.tb_colaborador_org
                .Where(x => x.tb_org_id == ID_FOURSYS)
                .GroupBy(x => new { x.cod_diretoria, x.diretoria })
                .Select(x => new UnidadesDTO { Id = x.Key.cod_diretoria, Descricao = x.Key.diretoria })
                .ToList();
        }

        public List<UnidadesDTO> ListarUnidadesPorOrg(int orgId)
        {
            return _colaboradorContext.tb_colaborador_org
                .Where(x => x.tb_org_id == orgId && x.ativo == (sbyte)1 && x.diretoria != null && x.diretoria != "")
                .GroupBy(x => new { id = x.cod_diretoria })
                .ToList()
                .Select(x => new UnidadesDTO { Id = x.Key.id, Descricao = x.FirstOrDefault().diretoria })
                .OrderBy(x => x.Descricao)
                .DistinctBy(x => x.Id)
                .ToList();
        }

        public async Task<List<UnidadesDTO>> ListarUnidadesPorOrgIdComRestricao(int orgId, List<string> diretorias)
        {
            var connection = _dapperConnection.GetConnection();

            var inClauseUnidades = diretorias.BuildInClauseOrNull();

            var query = $@"
                SELECT DISTINCT
                    cod_diretoria AS Id,
                    diretoria AS Descricao
                FROM tb_colaborador_org
                WHERE tb_org_id = @OrgId
                AND cod_diretoria <> ''
                {(inClauseUnidades == null ? "" : $"AND cod_diretoria IN {inClauseUnidades}")}
            ";

            var parametros = new { OrgId = orgId };
            var result = await connection.QueryAsync<UnidadesDTO>(query, parametros);
            return result.ToList();
        }
    }
}

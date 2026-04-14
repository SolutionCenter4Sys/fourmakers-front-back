using Colaboracao.Infra.Context;
using Core.DomainModel;
using DataTransferObject.Domain.Interesse;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class InteresseRepository : IInteresseDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public InteresseRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public InteresseDTO GetInteresseById(long id)
        {
            var registroDb = _colaboradorContext.tb_interesse
                .Where(x => x.id == id)
                .FirstOrDefault();

            if (registroDb != null)
            {
                return new InteresseDTO
                {
                    IdInteresse = registroDb.id,
                    Descricao = registroDb.descricao
                };
            }
            return null;
        }

        public long GetUsuarioCriacaoIdByCpf(string cpf)
        {
            var registroUser = _colaboradorContext.tb_usuario
                .Where(x => x.codigo_interno_colaborador == cpf)
                .FirstOrDefault();

            if (registroUser != null)
                return registroUser.id;

            throw new Exception("Usuário não encontrado.");
        }

        public InteresseDTO SaveInteresse(string descricao, long usuarioCriacaoId)
        {
            var userRow = _colaboradorContext.tb_usuario.Find(usuarioCriacaoId);
            if (_colaboradorContext.tb_interesse
                .Where(x => x.descricao.ToUpper() == descricao.ToUpper()).Count() > 0)
                throw new Exception("Este interesse já existe.");

            var row = new tb_interesse();
            row.ativo = 1;
            row.descricao = descricao;
            row.usuario_criacao = userRow ?? throw new Exception("Usuário não encontrado");

            _colaboradorContext.tb_interesse.Add(row);
            _colaboradorContext.SaveChanges();

            return new InteresseDTO
            {
                IdInteresse = row.id,
                Descricao = row.descricao,
                UsuarioCriacaoId = usuarioCriacaoId
            };
        }

        public List<InteresseDTO> ListInteresses(string busca, int cursor, int limite)
        {
            IQueryable<tb_interesse> query = _colaboradorContext.tb_interesse.Where(x => x.ativo == 1);

            if (!string.IsNullOrEmpty(busca))
                query = query.Where(x => EF.Functions.Like(x.descricao, "%" + busca + "%"));

            return query
                .OrderBy(x => x.descricao)
                .Skip(cursor)
                .Take(limite)
                .Select(row => new InteresseDTO
                {
                    IdInteresse = row.id,
                    Descricao = row.descricao,
                    UsuarioCriacaoId = row.usuario_criacao_id
                })
                .ToList();
        }
    }
}

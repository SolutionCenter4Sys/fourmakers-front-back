using Colaboracao.Infra.Context;
using Core.DomainModel;
using DataTransferObject.Domain.Dependentes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class DependenteColaboradorRepository : IDependenteDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public DependenteColaboradorRepository(ColaboradorContext colaboradorContext, IConfiguration configuration)
        {
            this._colaboradorContext = colaboradorContext;
        }

        public List<TipoDependenteDTO> ListarTipoDependente()
        {
            try
            {
                var lista = _colaboradorContext.tb_tipo_dependente.ToList();
                var ret = new List<TipoDependenteDTO>();
                foreach (tb_tipo_dependente tipo in lista)
                {
                    if (tipo.ativo == 1)
                    {
                        ret.Add(new TipoDependenteDTO()
                        {
                            Id = tipo.id,
                            Descricao = tipo.descricao,
                        });
                    }
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public DependentesDTO GetModelByKey(string key)
        {
            var registroDb = _colaboradorContext.tb_colaborador_dependente
                .Where(x => x.id == long.Parse(key) && x.ativo == 1)
                .FirstOrDefault();

            if (registroDb == null)
                return null;

            var tipoDependenteDb = _colaboradorContext.tb_tipo_dependente
                .Where(y => y.id == registroDb.tipo_dependente_id)
                .FirstOrDefault();

            return new DependentesDTO
            {
                Id = registroDb.id,
                NomeCompleto = registroDb.nome_completo,
                DataNascimento = registroDb.data_nascimento,
                Rg = registroDb.rg,
                Cpf = registroDb.cpf,
                PortadorDeficiencia = registroDb.portador_deficiencia,
                RequerAjudaQual = registroDb.requer_ajuda_qual,
                TipoDependente = tipoDependenteDb != null ? new TipoDependenteDTO
                {
                    Id = tipoDependenteDb.id,
                    Descricao = tipoDependenteDb.descricao
                } : null
            };
        }

        public List<DependentesDTO> GetModel(string cpf)
        {
            var dependenteDb = _colaboradorContext.tb_colaborador_dependente
                .Where(x => x.ativo == 1 && x.codigo_interno_colaborador == cpf)
                .ToList();

            var ret = new List<DependentesDTO>();

            foreach (var item in dependenteDb)
            {
                var tipoDependenteDb = _colaboradorContext.tb_tipo_dependente
                    .Where(y => y.id == item.tipo_dependente_id)
                    .FirstOrDefault();

                ret.Add(new DependentesDTO
                {
                    Id = item.id,
                    NomeCompleto = item.nome_completo,
                    DataNascimento = item.data_nascimento,
                    Rg = item.rg,
                    Cpf = item.cpf,
                    PortadorDeficiencia = item.portador_deficiencia,
                    RequerAjudaQual = item.requer_ajuda_qual,
                    TipoDependente = tipoDependenteDb != null ? new TipoDependenteDTO
                    {
                        Id = tipoDependenteDb.id,
                        Descricao = tipoDependenteDb.descricao
                    } : null
                });
            }

            return ret;
        }

        public DependentesDTO UpdateModel(DependentesDTO model, string cpf)
        {
            var registroDb = _colaboradorContext.tb_colaborador_dependente
                .Where(x => x.codigo_interno_colaborador == cpf && x.id == model.Id)
                .FirstOrDefault();

            if (registroDb == null)
                throw new Exception("Dependente não encontrado.");

            registroDb.nome_completo = model.NomeCompleto;
            registroDb.data_nascimento = model.DataNascimento;
            registroDb.rg = model.Rg;
            registroDb.cpf = model.Cpf;
            registroDb.portador_deficiencia = model.PortadorDeficiencia;
            registroDb.requer_ajuda_qual = model.RequerAjudaQual;
            registroDb.ativo = model.Ativo;
            registroDb.data_alteracao = DateTime.Now;
            registroDb.tipo_dependente_id = model.tipoDependenteId;

            _colaboradorContext.tb_colaborador_dependente.Update(registroDb);
            _colaboradorContext.SaveChanges();

            var registroTipoDb = _colaboradorContext.tb_tipo_dependente
                .Where(y => y.id == registroDb.tipo_dependente_id)
                .FirstOrDefault();

            model.TipoDependente = new TipoDependenteDTO
            {
                Id = registroTipoDb.id,
                Descricao = registroTipoDb.descricao
            };
            model.Id = registroDb.id;

            return model;
        }

        public DependentesDTO SaveModel(DependentesDTO model, string cpf)
        {
            var registroDb = new tb_colaborador_dependente();

            registroDb.nome_completo = model.NomeCompleto;
            registroDb.data_nascimento = model.DataNascimento;
            registroDb.rg = model.Rg;
            registroDb.cpf = model.Cpf;
            registroDb.portador_deficiencia = model.PortadorDeficiencia;
            registroDb.requer_ajuda_qual = model.RequerAjudaQual;
            registroDb.ativo = 1;
            registroDb.data_alteracao = DateTime.Now;
            registroDb.data_criacao = DateTime.Now;
            registroDb.tipo_dependente_id = model.tipoDependenteId;
            registroDb.codigo_interno_colaborador = cpf;

            _colaboradorContext.tb_colaborador_dependente.Add(registroDb);
            _colaboradorContext.SaveChanges();

            var registroTipoDb = _colaboradorContext.tb_tipo_dependente
                .Where(y => y.id == registroDb.tipo_dependente_id)
                .FirstOrDefault();

            model.Id = registroDb.id;
            model.TipoDependente = registroTipoDb != null ? new TipoDependenteDTO
            {
                Id = registroTipoDb.id,
                Descricao = registroTipoDb.descricao
            } : null;

            return model;
        }

        public DependentesDTO DeleteModel(DependentesDTO model, string cpf)
        {
            var registroDb = _colaboradorContext.tb_colaborador_dependente
                .Where(x => x.id == model.Id && x.codigo_interno_colaborador == cpf)
                .FirstOrDefault();

            if (registroDb == null)
                throw new Exception("Dependente não encontrado.");

            if (registroDb.ativo == 0)
                throw new Exception("Este dependente já está desativado");

            registroDb.ativo = 0;
            registroDb.data_alteracao = DateTime.Now;
            _colaboradorContext.tb_colaborador_dependente.Update(registroDb);
            _colaboradorContext.SaveChanges();

            return model;
        }
    }
}

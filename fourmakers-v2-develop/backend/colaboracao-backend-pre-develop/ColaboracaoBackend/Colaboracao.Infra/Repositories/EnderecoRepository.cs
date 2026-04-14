using Colaboracao.Infra.Context;
using Core.DomainModel;
using DataTransferObject.Domain.Endereco;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories
{
    public class EnderecoRepository : IEnderecoDtoRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public EnderecoRepository(ColaboradorContext EnderecoContext)
        {
            this._colaboradorContext = EnderecoContext;
        }

        public EnderecoDTO GetModelByKey(string key)
        {
            var registroDb = _colaboradorContext.tb_endereco.Where(x => x.tb_colaborador.Where(m => m.codigo_interno_colaborador == key).FirstOrDefault().endereco_id == x.id).FirstOrDefault();

            if (registroDb != null)
            {
                return new EnderecoDTO
                {
                    Id = registroDb.id,
                    Cep = registroDb.cep,
                    Cidade = registroDb.cidade,
                    Complemento = registroDb.complemento,
                    Estado = registroDb.estado,
                    Endereco = registroDb.endereco,
                    Numero = registroDb.numero,
                    Bairro = registroDb.bairro,
                    ComQuemMora = registroDb.com_quem_mora,
                    InternacionalLinhaUm = registroDb.internacional_linha_um,
                    InternacionalLinhaDois = registroDb.internacional_linha_dois
                };
            }
            else
            {
                return null;
            }
        }

        public EnderecoDTO SaveModel(EnderecoDTO model)
        {
            try
            {
                if (_colaboradorContext.tb_endereco
                    .Where(x => x.id == model.Id).Count() > 0)
                    throw new Exception("Este endereco já existe.");
                var row = new tb_endereco();

                row.cep = model.Cep;
                row.cidade = model.Cidade;
                row.complemento = model.Complemento;
                row.estado = model.Estado;
                row.endereco = model.Endereco;
                row.numero = model.Numero;
                row.bairro = model.Bairro;
                row.com_quem_mora = model.ComQuemMora;
                row.internacional_linha_um = model.InternacionalLinhaUm;
                row.internacional_linha_dois = model.InternacionalLinhaDois;

                _colaboradorContext.tb_endereco.Add(row);
                _colaboradorContext.SaveChanges();
                model.Id = row.id;

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public EnderecoDTO UpdateModel(EnderecoDTO model)
        {
            try
            {
                var registroDb = _colaboradorContext.tb_endereco
                    .Where(x => x.id == model.Id)
                    .FirstOrDefault();

                registroDb.cep = model.Cep;
                registroDb.cidade = model.Cidade;
                registroDb.complemento = model.Complemento;
                registroDb.estado = model.Estado;
                registroDb.endereco = model.Endereco;
                registroDb.numero = model.Numero;
                registroDb.bairro = model.Bairro;
                registroDb.com_quem_mora = model.ComQuemMora;
                registroDb.internacional_linha_um = model.InternacionalLinhaUm;
                registroDb.internacional_linha_dois = model.InternacionalLinhaDois;

                _colaboradorContext.tb_endereco.Update(registroDb);
                _colaboradorContext.SaveChanges();

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

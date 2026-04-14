using Colaboracao.Infra.Context;
using Core.Domain.Curriculo;
using Colaboracao.Core.Interfaces;
using Dapper;
using System;
using System.Data;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Curriculo
{
    public class CurriculoColaboradorRespository : ICurriculoColaboradorRespository
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly IDBConnection _dapperConnection;

        public CurriculoColaboradorRespository(ColaboradorContext colaboradorContext, IDBConnection dapperConnection)
        {
            this._colaboradorContext = colaboradorContext;
            this._dapperConnection = dapperConnection;
        }

        public string GetCurriculoColaborador(string cpf)
        {
            try
            {
                var rowCurriculo = _colaboradorContext.tb_curriculo_colaborador.Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1).OrderByDescending(x => x.data_alteracao).FirstOrDefault();
                if (rowCurriculo != null)
                {
                    return rowCurriculo.path_curriculo;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void InsereCurriculoColaborador(string path, string cpf)
        {
            try
            {
                var colab = _colaboradorContext.tb_curriculo_colaborador.Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1).FirstOrDefault();
                if (colab != null)
                {
                    AlterarCurriculoColaborador(path, cpf);
                }
                else
                {
                    var rowCurriculo = new tb_curriculo_colaborador();
                    rowCurriculo.path_curriculo = path;
                    rowCurriculo.ativo = 1;
                    rowCurriculo.codigo_interno_colaborador = cpf;
                    _colaboradorContext.Add(rowCurriculo);
                    _colaboradorContext.SaveChanges();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void InsereCurriculoColaboradorDapper(string path, string cpf)
        {
            var connection = _dapperConnection.GetConnection();

            var sqlInserir = @"
                    INSERT INTO tb_curriculo_colaborador 
                    (codigo_interno_colaborador, path_curriculo, ativo, data_criacao, data_alteracao) 
                    VALUES 
                    (@cpf, @path, 1, NOW(), NOW())";

            var result = connection.Execute(sqlInserir, new { cpf, path });
        }

        public void AlterarCurriculoColaborador(string path, string cpf)
        {
            try
            {
                var colab = _colaboradorContext.tb_curriculo_colaborador.Where(x => x.codigo_interno_colaborador == cpf && x.ativo == 1).FirstOrDefault();

                if (colab == null)
                    throw new Exception("Colaborador não possui currículo cadastrado!");

                colab.ativo = 0;

                _colaboradorContext.Update(colab);

                var rowCurriculo = new tb_curriculo_colaborador
                {
                    path_curriculo = path,
                    ativo = 1,
                    codigo_interno_colaborador = cpf
                };

                _colaboradorContext.Add(rowCurriculo);

                _colaboradorContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
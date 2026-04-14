using Colaboracao.Infra.Context;
using Core.Domain.DadosPerfil;
using DataTransferObject.Domain.DadosPerfil;
using DataTransferObject.Domain.DadosPerfil.GrauParentesco;
using DataTransferObject.Domain.DadosPerfil.TipoContratacao;
using System.Collections.Generic;
using System.Linq;

namespace Colaboracao.Infra.Repositories.DadosPerfil
{
    public class DadosPerfilRepository : IDadosPerfilRepository
    {
        private readonly ColaboradorContext _colaboradorContext;

        public DadosPerfilRepository(ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
        }
        public List<DisponibilidadeDTO> ListarDisponibilidade(string busca, int cursor, int limite)
        {
            try
            {
                var listaDisponibilidade = _colaboradorContext.tb_disponibilidade.Where(x => x.ativo == 1);
                var lista = listaDisponibilidade.OrderBy(x => x.descricao == busca).Skip(cursor).Take(limite).ToList();

                var disponibilidade = new List<DisponibilidadeDTO>();

                foreach (var row in lista)
                {
                    disponibilidade.Add(new DisponibilidadeDTO
                    {
                        Id = row.id,
                        Descricao = row.descricao
                    });
                }

                return disponibilidade;
            }
            catch
            {
                throw;
            }
        }

        public List<FonteOrigemDTO> ListarFonteOrigem(string busca, int cursor, int limite)
        {
            try
            {
                var listaFonteOrigem = _colaboradorContext.tb_fonteorigem_candidato.Where(x => x.ativo == 1);
                var lista = listaFonteOrigem.OrderBy(x => x.descricao == busca).Skip(cursor).Take(limite).ToList();

                var fonteOrigem = new List<FonteOrigemDTO>();

                foreach (var row in lista)
                {
                    fonteOrigem.Add(new FonteOrigemDTO
                    {
                        Id = row.id,
                        Descricao = row.descricao
                    });
                }

                return fonteOrigem;
            }
            catch
            {
                throw;
            }
        }

        public List<TipoDeCargoDTO> ListarTipoDeCargo(string busca, int cursor, int limite)
        {
            try
            {
                var listaTipoDeCargo = _colaboradorContext.tb_tipo_cargo.Where(x => x.ativo == 1);
                var lista = listaTipoDeCargo.OrderBy(x => x.descricao == busca).Skip(cursor).Take(limite).ToList();

                var tipoDeCargo = new List<TipoDeCargoDTO>();

                foreach (var row in lista)
                {
                    tipoDeCargo.Add(new TipoDeCargoDTO
                    {
                        Id = row.id,
                        Descricao = row.descricao
                    });
                }

                return tipoDeCargo;
            }
            catch
            {
                throw;
            }
        }

        public List<TipoCargaHorariaDTO> ListarTipoCargaHoraria(string busca, int cursor, int limite)
        {
            try
            {
                var listaTipoCargaHoraria = _colaboradorContext.tb_tipo_carga_horaria.Where(x => x.ativo == 1);
                var lista = listaTipoCargaHoraria.OrderBy(x => x.descricao == busca).Skip(cursor).Take(limite).ToList();

                var tipoCargaHoraria = new List<TipoCargaHorariaDTO>();

                foreach (var row in lista)
                {
                    tipoCargaHoraria.Add(new TipoCargaHorariaDTO
                    {
                        Id = row.id,
                        Descricao = row.descricao
                    });
                }

                return tipoCargaHoraria;
            }
            catch
            {
                throw;
            }
        }

        public List<ModalidadeContratacaoDTO> ListarModalidadeContratacao(string busca, int cursor, int limite)
        {
            try
            {
                var listaModalidadeContratacao = _colaboradorContext.tb_modalidade_contratacao.Where(x => x.ativo == 1);
                var lista = listaModalidadeContratacao.OrderBy(x => x.descricao == busca).Skip(cursor).Take(limite).ToList();

                var modalidadeContratacao = new List<ModalidadeContratacaoDTO>();

                foreach (var row in lista)
                {
                    modalidadeContratacao.Add(new ModalidadeContratacaoDTO
                    {
                        Id = row.id,
                        Descricao = row.descricao
                    });
                }

                return modalidadeContratacao;
            }
            catch
            {
                throw;
            }
        }

        public List<EstadoCivilDTO> ListarEstadoCivil(string busca, int cursor, int limite)
        {
            try
            {
                var listaEstadoCivil = _colaboradorContext.tb_estado_civil.Where(x => x.ativo == 1);
                var lista = listaEstadoCivil.OrderBy(x => x.descricao == busca).Skip(cursor).Take(limite).ToList();

                var estadoCivil = new List<EstadoCivilDTO>();

                foreach (var row in lista)
                {
                    estadoCivil.Add(new EstadoCivilDTO
                    {
                        Id = row.id,
                        Descricao = row.descricao
                    });
                }

                return estadoCivil;
            }
            catch
            {
                throw;
            }
        }

        public List<EscolaridadeDTO> ListarEscolaridade(string busca, int cursor, int limite)
        {
            try
            {
                var listaEscolaridade = _colaboradorContext.tb_nivel_escolaridade.Where(x => x.ativo == 1);
                var lista = listaEscolaridade.OrderBy(x => x.descricao == busca).Skip(cursor).Take(limite).ToList();

                var escolaridade = new List<EscolaridadeDTO>();

                foreach (var row in lista)
                {
                    escolaridade.Add(new EscolaridadeDTO
                    {
                        Id = row.id,
                        Descricao = row.descricao
                    });
                }

                return escolaridade;
            }
            catch
            {
                throw;
            }
        }

        public List<OrientacaoSexualDTO> ListarOrientacaoSexual(string busca, int cursor, int limite)
        {
            try
            {
                var listaOrientacaoSexual = _colaboradorContext.tb_orientacao_sexual.Where(x => x.ativo == 1);
                var lista = listaOrientacaoSexual.OrderBy(x => x.descricao == busca).Skip(cursor).Take(limite).ToList();

                var orientacaoSexual = new List<OrientacaoSexualDTO>();

                foreach (var row in lista)
                {
                    orientacaoSexual.Add(new OrientacaoSexualDTO
                    {
                        Id = row.id,
                        Descricao = row.descricao
                    });
                }

                return orientacaoSexual;
            }
            catch
            {
                throw;
            }
        }

        public List<IdentidadeDeGeneroDTO> ListarIdentidadeDeGenero(string busca, int cursor, int limite)
        {
            try
            {
                var listaIdentidadeDeGenero = _colaboradorContext.tb_identidade_genero.Where(x => x.ativo == 1);
                var lista = listaIdentidadeDeGenero.OrderBy(x => x.descricao == busca).Skip(cursor).Take(limite).ToList();

                var identidadeDeGenero = new List<IdentidadeDeGeneroDTO>();

                foreach (var row in lista)
                {
                    identidadeDeGenero.Add(new IdentidadeDeGeneroDTO
                    {
                        Id = row.id,
                        Descricao = row.descricao
                    });
                }

                return identidadeDeGenero;
            }
            catch
            {
                throw;
            }
        }

        public List<GrauParentescoDTO> ListarGrauParentesco(string busca, int cursor, int limite)
        {
            try
            {
                var listaGrauParentesco = _colaboradorContext.tb_grau_parentesco.Where(x => x.ativo == 1);
                var lista = listaGrauParentesco.OrderBy(x => x.descricao == busca).Skip(cursor).Take(limite).ToList();

                var grauParentesco = new List<GrauParentescoDTO>();

                foreach (var row in lista)
                {
                    grauParentesco.Add(new GrauParentescoDTO
                    {
                        Id = row.id,
                        Descricao = row.descricao
                    });
                }

                return grauParentesco;
            }
            catch
            {
                throw;
            }
        }

        public List<ListarTipoContratacaoDTO> ListarTipoContratacao(string busca, int cursor, int limite)
        {
            try
            {
                var TipoContratacao = _colaboradorContext.tb_tipo_contratacao.Where(x => x.ativo == 1);
                var lista = TipoContratacao.OrderBy(x => x.descricao == busca).Skip(cursor).Take(limite);

                var ret = new List<ListarTipoContratacaoDTO>();

                foreach (var row in lista)
                {
                    ret.Add(new ListarTipoContratacaoDTO
                    {
                        Id = row.id,
                        Descricao = row.descricao
                    });
                }

                return ret;
            }
            catch
            {
                throw;
            }
        }

        public List<EtniaDTO> ListarEtnia(string busca, int cursor, int limite)
        {
            try
            {
                var etnia = _colaboradorContext.tb_etnia.Where(x => x.ativo == 1);
                var lista = etnia.OrderBy(x => x.descricao == busca).Skip(cursor).Take(limite);

                var ret = new List<EtniaDTO>();

                foreach (var row in lista)
                {
                    ret.Add(new EtniaDTO
                    {
                        Id = row.id,
                        Descricao = row.descricao
                    });
                }

                return ret;
            }
            catch
            {
                throw;
            }
        }
    }
}
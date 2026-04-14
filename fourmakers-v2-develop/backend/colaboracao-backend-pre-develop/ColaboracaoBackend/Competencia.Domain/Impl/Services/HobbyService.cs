using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Competencia.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using System;
using System.Collections.Generic;

using Logs.Infra.Attributes;

namespace Competencia.Domain.Impl.Services
{
    [LogDomainClass]
    public class HobbyService : IHobbyService
    {
        private IHobbyDomainFactory _hobbyfactory;
        private readonly IFirebaseClient _firebaseClient;
        private readonly IColaboradorClient _colaboradorClient;
        private readonly IAspNetUser _aspNetUser;

        public HobbyService(IHobbyDomainFactory hobbyfactory, IFirebaseClient firebaseClient, IColaboradorClient colaboradorClient, IAspNetUser aspNetUser)
        {
            _hobbyfactory = hobbyfactory;
            _firebaseClient = firebaseClient;
            _colaboradorClient = colaboradorClient;
            _aspNetUser = aspNetUser;
        }

        public IHobbyModel InserirHobby(string descricao)
        {
            try
            {
                var hobby = _hobbyfactory.buildHobbyModel();
                var idUsuario = hobby.GetByCpf(_aspNetUser.GetUsuarioLogado().Cpf, _hobbyfactory);
                hobby.HobbyDTO.UsuarioCriacaoId = idUsuario.HobbyDTO.UsuarioCriacaoId;
                hobby.HobbyDTO.Descricao = descricao;
                hobby = hobby.SaveModel();
                return hobby;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<IHobbyModel> ListarHobbies(string busca, int cursor, int limite)
        {
            try
            {
                return _hobbyfactory.buildHobbyModel().Listar(busca, cursor, limite, _hobbyfactory);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IHobbyModel GetHobbiesById(long id)
        {
            try
            {
                return _hobbyfactory.buildHobbyModel().GetById(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IHobbyColaboradorModel InserirHobbieColaborador(long id, string cpf)
        {
            try
            {
                var hobbyColaborador = _hobbyfactory.buildHobbyColaboradorModel();
                hobbyColaborador.HobbyId = id;
                hobbyColaborador.ColaboradorCpf = cpf;
                hobbyColaborador.SaveModel();

                var hobby = _hobbyfactory.buildHobbyModel().GetById(hobbyColaborador.HobbyId);
                hobbyColaborador.HobbyColaboradorDTO.Hobbie = new ItemPerfilDTO
                {
                    Id = hobby.HobbyDTO.IdHobby,
                    Descricao = hobby.HobbyDTO.Descricao
                };

                _colaboradorClient.EnviaPushNotificationRedeColaborador(hobbyColaborador.ColaboradorCpf, "Novo Hobby", " adicionou o hobby " + hobbyColaborador.HobbyColaboradorDTO.Hobbie.Descricao, _aspNetUser.GetUsuarioLogado().Token);

                return hobbyColaborador;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<IHobbyColaboradorModel> ListarHobbiesColaborador(string cpfColaborador)
        {
            try
            {
                var hobbyColaborador = _hobbyfactory.buildHobbyColaboradorModel();
                hobbyColaborador.ColaboradorCpf = cpfColaborador;
                var listaHobbyColaborador = hobbyColaborador.ListarHobbiesDeColaboradores(_hobbyfactory);
                foreach (var itemHobbyColaborador in listaHobbyColaborador)
                {
                    var hobby = _hobbyfactory.buildHobbyModel().GetById(itemHobbyColaborador.HobbyId);
                    itemHobbyColaborador.HobbyColaboradorDTO.Hobbie = new ItemPerfilDTO
                    {
                        Id = hobby.HobbyDTO.IdHobby,
                        Descricao = hobby.HobbyDTO.Descricao
                    };
                }
                return listaHobbyColaborador;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IHobbyColaboradorModel RemoverHobbieColaborador(long id, string cpf)
        {
            try
            {
                var hobbyColaborador = _hobbyfactory.buildHobbyColaboradorModel();
                hobbyColaborador.ColaboradorCpf = cpf;
                hobbyColaborador.HobbyId = id;
                hobbyColaborador.RemoverHobbyDoColaborador();
                return hobbyColaborador;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
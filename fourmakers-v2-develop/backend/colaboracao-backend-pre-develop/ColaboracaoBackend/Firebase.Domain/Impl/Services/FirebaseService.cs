using Colaboracao.Core;
using DataTransferObject.Domain.Log;
using Core.DomainModel;
using Firebase.Domain.Interfaces;
using Firebase.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firebase.Domain.Impl.Services
{
    [LogDomainClass]
    public class FirebaseService : IFirebaseService
    {
        private readonly IFirebaseSDK _firebase;
        private readonly IFirebaseRepository _firebaseRepository;
        private readonly ILogCore _log;

        public FirebaseService(IFirebaseSDK firebase, IFirebaseRepository firebaseRepository, ILogCore log)
        {
            _firebase = firebase;
            _firebaseRepository = firebaseRepository;
            _log = log;
        }

        public async Task<bool> EnviaPush(string token, string titulo, string mensagem)
        {
            try
            {
                await _firebase.EnviaPush(token, titulo, mensagem);
                return true;
            }
            catch (Exception e)
            {
                _log.Log(e.Message, LevelsEnum.Trace);
                return false;
            }
        }

        public async Task<bool> EnviaPushEmLote(List<string> tokens, string titulo, string mensagem)
        {
            try
            {
                await _firebase.EnviaPushEmLote(tokens, titulo, mensagem);
                return true;
            }
            catch (Exception e)
            {
                _log.Log(e.Message, LevelsEnum.Trace);
                return false;
            }
        }

        public async Task<bool> EnviarNotificacaoPushApp(string codigoColaborador, string titulo, string mensagem)
        {
            try
            {
                var deviceToken = await _firebaseRepository.BuscarDeviceTokenApp(codigoColaborador);
                await _firebase.EnviaPush(deviceToken, titulo, mensagem);
                return true;
            }
            catch (Exception e)
            {
                _log.Log(e.Message, LevelsEnum.Trace);
                return false;
            }
        }

        public async Task<bool> EnviarNotificacaoPushAppEmLote(List<string> codigosColaboradores, string titulo, string mensagem)
        {
            try
            {
                var deviceTokens = await _firebaseRepository.BuscarDeviceTokensAppEmLote(codigosColaboradores);
                await _firebase.EnviaPushEmLote(deviceTokens, titulo, mensagem);
                return true;
            }
            catch (Exception e)
            {
                _log.Log(e.Message, LevelsEnum.Trace);
                return false;
            }
        }
    }
}
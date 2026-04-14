using Colaboracao.Core;
using DataTransferObject.Domain.Log;
using Firebase.Domain.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Colaboracao.Infra
{
    public class FirebaseSDK : IFirebaseSDK
    {
        private static FirebaseApp _admin;
        private static readonly object _initLock = new object();
        private readonly ILogCore _log;

        public FirebaseSDK(ILogCore log)
        {
            _log = log;
        }

        private void EnsureFirebaseInitialized()
        {
            if (_admin != null)
                return;
            lock (_initLock)
            {
                if (_admin != null)
                    return;
                GoogleCredential credential = CreateCredentialFromEnvironment();
                _admin = FirebaseApp.Create(new AppOptions()
                {
                    Credential = credential,
                });
            }
        }

        private static GoogleCredential CreateCredentialFromEnvironment()
        {
            var json = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON");
            if (!string.IsNullOrWhiteSpace(json))
                return GoogleCredential.FromJson(json);

            var path = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                throw new InvalidOperationException(
                    "Configure FIREBASE_SERVICE_ACCOUNT_JSON (JSON completo) ou GOOGLE_APPLICATION_CREDENTIALS (caminho do arquivo JSON da service account do Firebase).");
            }

            return GoogleCredential.FromFile(path);
        }

        public async Task EnviaPushEmLote(List<string> tokens, string titulo, string mensagem)
        {
            EnsureFirebaseInitialized();
            try
            {
                foreach (var token in tokens)
                {
                    await EnviaPush(token, titulo, mensagem);
                }
            }
            catch (Exception e)
            {
                _log.Log("Fail sent message: " + e.Message, LevelsEnum.Trace);
                throw;
            }
        }

        public async Task EnviaPush(string token, string titulo, string mensagem)
        {
            EnsureFirebaseInitialized();
            try
            {
                if (string.IsNullOrEmpty(token))
                    return;

                var message = new Message()
                {
                    Token = token,
                    Notification = new Notification
                    {
                        Body = mensagem,
                        Title = titulo,
                    },
                };

                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _log.Log("Successfully sent message: " + response, LevelsEnum.Information);
            }
            catch (Exception e)
            {
                _log.Log($"Falha ao enviar notificacao push. DeviceToken: {token} - ERRO: " + e.Message, LevelsEnum.Trace);
            }
        }

    }
}

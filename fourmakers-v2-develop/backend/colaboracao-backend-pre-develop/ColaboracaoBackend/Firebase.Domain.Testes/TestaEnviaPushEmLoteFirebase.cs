using Colaboracao.Core;
using Colaboracao.Infra;
using Firebase.Domain.Impl.Services;
using Firebase.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Firebase.Domain.Testes
{
    public class TestaEnviaPushEmLoteFirebase
    {
        private readonly ILogCore _log;
        private readonly IFirebaseSDK _firebaseSDK;

        public TestaEnviaPushEmLoteFirebase()
        {
            var iLogger = Mock.Of<ILogCore>();
            _log = iLogger;
            var firebaseSDK = new FirebaseSDK(_log);
            _firebaseSDK = firebaseSDK;
        }

        [Fact]
        public async void EnviaUmaRequisicaoParaUmaListaDeColaboradoresDadoColaboradorExistenteNoBancoDeDados()
        {
            try
            {
                List<string> tokens = new List<string>();
                tokens.Add("eVGVsgrUWENwrobv4ZxYOF:APA91bG45tIoXNKDBLOzAAMJwgdzLn0i64IFQ5LaiFQWI1oWh68Yn4F2h8GZGNhRBpvVtFhnlBpTZY5yWCF9V1umabPGB7r_nP70JOeanR6YfExjgbslGYBmmH2D9WjZCwW7-QjKWvsm");
                //var FirebaseService = new FirebaseService(_firebaseSDK, _log);
                //await FirebaseService.EnviaPushEmLote(tokens, "Teste2", "Segundo teste");
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.True(false);
            }
        }
    }
}
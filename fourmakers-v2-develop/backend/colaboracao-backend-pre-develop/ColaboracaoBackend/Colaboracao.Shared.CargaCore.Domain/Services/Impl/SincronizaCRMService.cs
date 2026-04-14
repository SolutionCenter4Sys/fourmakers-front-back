using ApiClient.Domain.Interfaces;
using CargaCore.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Core.Domain.Usuario;
using Core.DomainModel.Projeto;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.CRM;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SincronizarCRM;
using DataTransferObject.Domain.Projeto;
using DataTransferObject.Domain.Projeto.GestorExterno;
using DataTransferObject.Domain.Util.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargaFourmaker.API.Services
{
    public class SincronizaCRMService : ISincronizaCRMService
    {
        private readonly IClienteOrgRepository _clienteRepository;
        private readonly IColaboracaoBridgeClient _colaboracaoBridgeClient;
        private readonly IGestorExternoRepository _gestorExternoRepository;
        private readonly IProjetoOrgRepository _projetoOrgRepository;
        private readonly ILogDBCore _logDB;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;

        private readonly int _orgId = EnumORG.FOURSYS_2.ToInt();

        public SincronizaCRMService(IClienteOrgRepository clienteRepository,
                                      IColaboracaoBridgeClient colaboracaoBridgeClient,
                                      IGestorExternoRepository gestorExternoRepository,
                                      IProjetoOrgRepository projetoOrgRepository,
                                      ILogDBCore logDB,
                                      IDBConnectionUnitOfWork dbConnectionUnitOfWork,
                                      IUsuarioColaboradorRepository usuarioColaboradorRepository)
        {
            _clienteRepository = clienteRepository;
            _colaboracaoBridgeClient = colaboracaoBridgeClient;
            _gestorExternoRepository = gestorExternoRepository;
            _projetoOrgRepository = projetoOrgRepository;
            _logDB = logDB;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
        }

        public async Task SincronizarCRM()
        {
            var rotinaNome = nameof(SincronizarCRM);

            _logDB.SaveLogDefaultInDatabase($"Início {rotinaNome}", string.Empty, ProcessIdentifierEnum.SincronizaClienteCRMInProgress);
            Console.WriteLine($"Iniciou a rotina {rotinaNome} as " + DateTime.Now.ToLogFormat());

            await ExecutaRotinaAsync(CargaClienteCRM);
            ////await ExecutaRotinaAsync(AtualizarProjetosComCodigoClienteBaseadoNaTabelaProjetoProposta);
            await ExecutaRotinaAsync(AtualizarProjetosComCodigoClienteBaseadoPeloCodigoCCH);
            await ExecutaRotinaAsync(DesativarClientesQueForamSubstituidosNaClienteOrg);
            await ExecutaRotinaAsync(CargaGestoresCRM);

            _logDB.SaveLogDefaultInDatabase($"Finalizou {rotinaNome}", string.Empty, ProcessIdentifierEnum.SincronizaClienteCRMCompleted);
            Console.WriteLine($"Finalizou a rotina {rotinaNome} as " + DateTime.Now.ToLogFormat());
        }

        private async Task DesativarClientesQueForamSubstituidosNaClienteOrg()
        {
            await _clienteRepository.DesativarClientesQueForamSubstituidosPorCodigoClienteDoCRM(_orgId);
        }

        private async Task ExecutaRotinaAsync(Func<Task> acaoRotina)
        {
            var rotinaNome = acaoRotina.Method.Name;
            var processIdentifier = ProcessIdentifierEnum.SincronizaClienteCRMInProgress;

            _logDB.SaveLogDefaultInDatabase($"Iniciou a rotina {rotinaNome}", string.Empty, processIdentifier);
            Console.WriteLine($"Iniciou a rotina {rotinaNome} as " + DateTime.Now.ToLogFormat());

            try
            {
                await acaoRotina();
            }
            catch (Exception ex)
            {
                _logDB.SaveExceptionLogInDatabase($"Finalizou {rotinaNome}", ex, processIdentifier);
                Console.WriteLine($"Erro na rotina {rotinaNome} as " + DateTime.Now.ToLogFormat());
                throw;
            }

            _logDB.SaveLogDefaultInDatabase($"Finalizou {rotinaNome}", string.Empty, processIdentifier);
            Console.WriteLine($"Finalizou a rotina {rotinaNome} as " + DateTime.Now.ToLogFormat());
        }
        public async Task CargaClienteCRM()
        {

            var clientesExistentes = await _clienteRepository.ListarClientesOrg(_orgId, null, null);
            var codigosClientesExistentes = new HashSet<string>(clientesExistentes.Select(c => c.CodigoCliente));

            var clientesCRM = await _colaboracaoBridgeClient.GetClientesFourmakersCrm(); //aqui pegar da ColaboracaoBridge
            var novosClientesCRMPorCodigo = clientesCRM
                .Where(cliente => !codigosClientesExistentes.Contains(cliente.Account_no.Trim().ToUpper()));

            foreach (var cliente in novosClientesCRMPorCodigo)
            {
                try
                {
                    var clienteDTO = await CriarOuAtualizarClienteOrg(cliente.Account_no, cliente.Nome_Conta, _orgId);
                    clientesExistentes.Add(new ClienteOrgDTO() { CodigoCliente = cliente.Account_no, NomeCliente = cliente.Nome_Conta }); 
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Erro ao inserir Cliente: {cliente.Account_no.ToStringOuParametro("<sem códgigo informado>")}");
                }
            }

            var nomesClientesExistentes = new HashSet<string>(clientesExistentes.Select(c => c.NomeCliente));
            var novosClientesCRMPorNome = clientesCRM
            .Where(cliente => !nomesClientesExistentes.Contains(cliente.Nome_Conta));

            foreach (var cliente in novosClientesCRMPorNome)
            {
                try
                {
                    var clienteDTO = await CriarOuAtualizarClienteOrg(cliente.Account_no, cliente.Nome_Conta, _orgId);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Erro ao inserir Cliente: {cliente.Account_no.ToStringOuParametro("<sem códgigo informado>")}");
                }
            }



        }
        public async Task AtualizarProjetosComCodigoClienteBaseadoPeloCodigoCCH()
        {

            var clientesExistentes = await _clienteRepository.ListarClientesOrg(_orgId, null, null);

            var clientesCRM = await _colaboracaoBridgeClient.GetClientesFourmakersCrm(); //aqui pegar da ColaboracaoBridge

            var clientesCRMFiltrados = clientesCRM.Where(cliente => !string.IsNullOrWhiteSpace(cliente.Codigo_CCH) && cliente.Codigo_CCH != "0") 
                                               .GroupBy(cliente => cliente.Codigo_CCH)
                                               .Where(grupo => grupo.Count() == 1) 
                                               .Select(grupo => grupo.First());

            try
            {
                foreach (var clienteCRM in clientesCRMFiltrados)
                {

                    // verificar se o cliente já existe na nossa base
                    var clienteDB = clientesExistentes.FirstOrDefault(c => c.CodigoCliente == clienteCRM.Account_no);

                    if (clienteDB == null)
                    {
                        var clienteDTO = await CriarOuAtualizarClienteOrg(clienteCRM.Account_no, clienteCRM.Nome_Conta, _orgId);

                        await _clienteRepository.UpsertClienteOrg(clienteDTO, _orgId, false, TipoCadastroClienteEnum.CADASTRO_CARGA_CRM);

                    }

                    bool precisaAtualizar = await _projetoOrgRepository.VerificarSeCodigoClienteAntigoExiste(clienteCRM.Codigo_CCH, _orgId);
                    bool precisaAtualizarPoisCodigoEstaDesincronizado = await _projetoOrgRepository.VerificarSeCodClienteAtualEhDiferenteDoCodigoClienteCRM(clienteCRM.Account_no, clienteCRM.Codigo_CCH, _orgId);

                    if (precisaAtualizar
                        || precisaAtualizarPoisCodigoEstaDesincronizado)
                    {
                        var projetoAntesDeAtualizar = await _projetoOrgRepository.GetProjetoPorCodigoClienteRegistroCargaAsync(clienteCRM.Codigo_CCH, _orgId);

                        var linhasAtualizadas = await _projetoOrgRepository.AtualizarCodigoClienteNosProjetosPorCodClienteAntigo(clienteCRM.Account_no, clienteCRM.Codigo_CCH, _orgId);
                        if (linhasAtualizadas > 0)
                        {
                            _logDB.SaveLogDefaultInDatabase($"AtualizarProjetosComCodigoClienteBaseadoPeloCodigoCCH atualizou {linhasAtualizadas} linha(s) para CodCCH: {clienteCRM.Codigo_CCH} > CodCRM: {clienteCRM.Account_no}", string.Empty, ProcessIdentifierEnum.SincronizaClienteCRMInProgress);
                        }

                        // neste ponto vou atualizar os gestores, mas baseado no código que estava setado antes de ser alterado,
                        // e não pelo códigoCCH original essa mudança previne os casos onde o código CCH é alterado no CRM mais de uma vez
                        var linhasAtualizadasGestoresExternos = await _projetoOrgRepository.AtualizarCodigoClienteNosGestoresExternosPorCodClienteAntigo(clienteCRM.Account_no, projetoAntesDeAtualizar.ClienteProjeto.CodigoCliente, _orgId);
                        if (linhasAtualizadasGestoresExternos > 0)
                        {
                            _logDB.SaveLogDefaultInDatabase($"{nameof(_projetoOrgRepository.AtualizarCodigoClienteNosGestoresExternosPorCodClienteAntigo)} atualizou {linhasAtualizadasGestoresExternos} linha(s) para CodCCH: {clienteCRM.Codigo_CCH} > CodCRM: {clienteCRM.Account_no}", string.Empty, ProcessIdentifierEnum.SincronizaClienteCRMInProgress);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar projetos pelo codigo CCH", ex);
            }
        }
        public async Task CargaGestoresCRM()
        {
            var gestoresExistentesNaBase = await _gestorExternoRepository.ListarGestoresExternosTodosAsync(_orgId, false);

            var clientesDB = await _clienteRepository.ListarClientesOrg(_orgId, null, null);

            // pegando os gestores que estão associados aos clientes que já temos no nosso banco de dados
            var gestoresCRMFiltradosPorClientesDB = await _colaboracaoBridgeClient.GetGestoresPorAccountNos(clientesDB.Select(c => c.CodigoCliente).ToList()); // aqui pegar da ColaboracaoBridge

            var agrupadosPorEmail = gestoresCRMFiltradosPorClientesDB
                .Where(c => !string.IsNullOrWhiteSpace(c.Email)) // Filtra registros com Email não vazio
                .GroupBy(c => new { c.Email, c.AccountNo }) // Agrupa por Email e AccountNo
                .Select(group => group.OrderByDescending(c => c.ContactId).First()) // Seleciona o maior ContactId de cada grupo
                .ToList();

            // Adiciona os registros com Email vazio ou nulo
            agrupadosPorEmail.AddRange(gestoresCRMFiltradosPorClientesDB.Where(c => string.IsNullOrWhiteSpace(c.Email)));

            int contadorAtualizacao = 0,
                contadorInsercao = 0;
            
            foreach (var gestorCRM in agrupadosPorEmail)
            {
                try
                {
                    var telefoneCRMTruncado = GestorCRMHelper.ObterTelefone(gestorCRM).ToStringOuVazio();
                    telefoneCRMTruncado = telefoneCRMTruncado.Length > 20
                        ? telefoneCRMTruncado.Substring(0, 20)
                        : telefoneCRMTruncado;

                    GestorExternoInput novoGestorExternoAIncluir = new()
                    {
                        CodigoCliente = gestorCRM.AccountNo,
                        OrgId = _orgId,
                        Nome = gestorCRM.FirstName.ToNomeCompleto(gestorCRM.LastName),
                        Email = gestorCRM.Email,
                        PerfilLinkedin = GestorCRMHelper.ObterPerfilLinkedin(gestorCRM.Linkedin),
                        CodGestorExterno = gestorCRM.ContactNo,
                        Telefone = telefoneCRMTruncado,
                        Ativo = true
                    };
                   
                    GestorExternoResult gestorExistente = null;
                    

                    // verifica se o email é válido antes de buscar
                    if (!string.IsNullOrWhiteSpace(novoGestorExternoAIncluir.Email))
                    {
                        // pesquisa levando em consideração accountno além do e-mail, pois pode ter email duplicado para account_no diferentes
                        gestorExistente = gestoresExistentesNaBase.Where(g => g.Email == novoGestorExternoAIncluir.Email && g.CodigoCliente == novoGestorExternoAIncluir.CodigoCliente).FirstOrDefault();
                    }

                    if (gestorExistente is not null)
                    {
                        //vamos manter o ultimo codigo de gestor (pode ter mais de um pro mesmo email
                        if (PossuiDiferencaEntreGestorDBEGestorCRM(gestorExistente, novoGestorExternoAIncluir))
                        {
                            contadorAtualizacao++;
                            await AtualizarGestorExternoAsync(novoGestorExternoAIncluir, gestorExistente, gestorCRM, _orgId, contadorAtualizacao, "ENCONTRADO_POR_EMAIL");
                        }

                        if(NaoExisteColaboradorComEsteEmail(novoGestorExternoAIncluir.Email))
                            await ProcessarCriacaoColaborador(novoGestorExternoAIncluir);

                        continue;
                    }

                    // caso o gestor não seja achado por email anteriormente, vamos verificar se já existe o codigo de gestor na base
                    // se já existir, e possuir diferença, atualiza, se não existir, criar um gestor novo
                    gestorExistente = gestoresExistentesNaBase.Where(g => g.CodGestorExterno == gestorCRM.ContactNo).FirstOrDefault();

                    if (gestorExistente is null)
                    {
                        await _gestorExternoRepository.InserirGestorExternoAsync(novoGestorExternoAIncluir, TipoCadastrogGestorExternoEnum.CADASTRO_CRM);

                        contadorInsercao++;

                        _logDB.SaveLogDefaultInDatabase($"CargaGestoresCRM.GESTOR_INSERIDO:[{contadorInsercao}] CodGestorExterno: {novoGestorExternoAIncluir.CodGestorExterno} - CodigoCliente: {novoGestorExternoAIncluir.CodigoCliente} - Novo gestor inserido.",
                            string.Empty, ProcessIdentifierEnum.SincronizaClienteCRMInProgress);

                        await ProcessarCriacaoColaborador(novoGestorExternoAIncluir);
                    }
                    else if (PossuiDiferencaEntreGestorDBEGestorCRM(gestorExistente, novoGestorExternoAIncluir))
                    {
                       contadorAtualizacao++;
                        await AtualizarGestorExternoAsync(novoGestorExternoAIncluir, gestorExistente, gestorCRM, _orgId, contadorAtualizacao, "ENCONTRADO_POR_CODIGO");

                        if (NaoExisteColaboradorComEsteEmail(novoGestorExternoAIncluir.Email))
                            await ProcessarCriacaoColaborador(novoGestorExternoAIncluir);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Erro ao inserir Gestor: {gestorCRM.ContactNo.ToStringOuParametro("<sem códgigo informado>")}");
                }
            }
        
        }

        private bool NaoExisteColaboradorComEsteEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return !_usuarioColaboradorRepository.ExisteColaboradorComEmailAlternativo(email.Trim(), _orgId);
        }

        private async Task ProcessarCriacaoColaborador(GestorExternoInput gestorExternoInput)
        {
            var codColaborador = Guid.NewGuid().ToString();

            try
            {
                _logDB.SaveLogDefaultInDatabase($"CargaGestoresCRM.CRIAR_COD_COLABORADOR_PARA_GESTOR NomeGestorExterno: {gestorExternoInput.Nome} - CodGestorExterno: {gestorExternoInput.CodGestorExterno} - CodigoCliente: {gestorExternoInput.CodigoCliente} - codColaborador: {codColaborador}.",
                            string.Empty, ProcessIdentifierEnum.SincronizaClienteCRMInProgress);

                _usuarioColaboradorRepository.InsertColaboradorSemUsuario(new ColaboradorDTO
                {
                    Cpf = codColaborador,
                    NomeCompleto = gestorExternoInput.Nome,
                    EmailAlternativo = gestorExternoInput.Email,
                    ContatoPrincipal = gestorExternoInput.Telefone,
                    ContatoPrincipalDDI = "55",
                });

                await _gestorExternoRepository.AtualizarCodColaboradorGestorExternoAsync(codColaborador, gestorExternoInput.CodGestorExterno, _orgId);
            }
            catch
            {
                _logDB.SaveLogDefaultInDatabase($"CargaGestoresCRM.CRIAR_COD_COLABORADOR_PARA_GESTOR_ERRO NomeGestorExterno: {gestorExternoInput.Nome} - CodGestorExterno: {gestorExternoInput.CodGestorExterno} - CodigoCliente: {gestorExternoInput.CodigoCliente} - codColaborador: {codColaborador}.",
                            string.Empty, ProcessIdentifierEnum.SincronizaClienteCRMInProgress);
            }
        }

        private bool PossuiDiferencaEntreGestorDBEGestorCRM(GestorExternoResult gestorDB, GestorExternoInput novoGestorExternoAIncluir)
        {

            var linkedingDiferente = VerificarDiferencaLinkedin(gestorDB.PerfilLinkedin, novoGestorExternoAIncluir.PerfilLinkedin);
            var codigoGestorDiferente = gestorDB.CodGestorExterno.ToStringOuVazio().Trim() != novoGestorExternoAIncluir.CodGestorExterno.ToStringOuVazio().Trim();
            var codigoClienteDiferente = gestorDB.CodigoCliente.ToStringOuVazio().Trim() != novoGestorExternoAIncluir.CodigoCliente.ToStringOuVazio().Trim();
            var telefoneDiferente = gestorDB.Telefone.ToStringOuVazio() != novoGestorExternoAIncluir.Telefone;
            var nomeDiferente = gestorDB.Nome != novoGestorExternoAIncluir.Nome;
            var emailDiferente = gestorDB.Email != novoGestorExternoAIncluir.Email;

            var possuiDiferenca = linkedingDiferente || codigoGestorDiferente || codigoClienteDiferente || telefoneDiferente || nomeDiferente || emailDiferente;

            if (possuiDiferenca)
            {
                var diferencas = new List<string>();

                if (linkedingDiferente)
                    diferencas.Add($"PerfilLinkedin: '{gestorDB.PerfilLinkedin}' -> '{novoGestorExternoAIncluir.PerfilLinkedin}'");

                if (codigoGestorDiferente)
                    diferencas.Add($"CodGestorExterno: '{gestorDB.CodGestorExterno}' -> '{novoGestorExternoAIncluir.CodGestorExterno}'");

                if (codigoClienteDiferente)
                    diferencas.Add($"CodigoCliente: '{gestorDB.CodigoCliente}' -> '{novoGestorExternoAIncluir.CodigoCliente}'");

                if (telefoneDiferente)
                    diferencas.Add($"Telefone: '{gestorDB.Telefone}' -> '{novoGestorExternoAIncluir.Telefone}'");

                if (nomeDiferente)
                    diferencas.Add($"Nome: '{gestorDB.Nome}' -> '{novoGestorExternoAIncluir.Nome}'");

                if (emailDiferente)
                    diferencas.Add($"Email: '{gestorDB.Email}' -> '{novoGestorExternoAIncluir.Email}'");

                _logDB.SaveLogDefaultInDatabase(
                    $"CargaGestoresCRM.DIFERENCA_DETECTADA: CodGestorExterno: {novoGestorExternoAIncluir.CodGestorExterno} - Diferenças: {string.Join(", ", diferencas)}",
                    string.Empty,
                    ProcessIdentifierEnum.SincronizaClienteCRMInProgress
                );
            }

            return possuiDiferenca;
        }

        private static bool VerificarDiferencaLinkedin(string perfilLinkedinDB, string perfilLinkedinCRM)
        {
            if (string.IsNullOrWhiteSpace(perfilLinkedinCRM))
            {
                return false; // considera sem diferença se o CRM não tiver um perfil válido
            }

            return perfilLinkedinDB.ToStringOuVazio() != perfilLinkedinCRM.ToStringOuVazio();
        }

        private async Task AtualizarGestorExternoAsync(GestorExternoInput novoGestorExterno,GestorExternoResult gestorExistente,
                                                       ContactDetailsDTO gestorCRM, int orgId, int contador, string encontradoPor)
        {
            //novoGestorExterno.AtualizarPropriedadesDaClasseBase(gestorExistente);
            novoGestorExterno.ConfigurarParaPersistencia(orgId, gestorExistente.CodigoInternoColaborador, gestorExistente.CodigoInternoColaborador, gestorCRM.AccountNo);

            var linkedinCadastroCRM = GestorCRMHelper.ObterPerfilLinkedin(gestorCRM.Linkedin);

            novoGestorExterno.PreferenciasPessoais = gestorExistente.PreferenciasPessoais;

            if (VerificarDiferencaLinkedin(gestorExistente.PerfilLinkedin, linkedinCadastroCRM))
            {
                novoGestorExterno.PerfilLinkedin = linkedinCadastroCRM;
            }

            var forcarAtualizacaoCodClienteCarga = gestorCRM.AccountNo != gestorExistente.CodigoCliente;

            await _gestorExternoRepository.AtualizarGestorExternoAsync(
                novoGestorExterno,
                gestorExistente.CodGestorExterno,
                TipoCadastrogGestorExternoEnum.CADASTRO_CRM,
                forcarAtualizacaoCodClienteCarga
            );

            LogarAlteracaoGestorExterno(gestorExistente, novoGestorExterno, contador, encontradoPor);
        }


        private void LogarAlteracaoGestorExterno(GestorExternoResult gestorAtual, GestorExternoInput gestorAtualizado, int contador, string encontradoPor)
        {
            // Verifica alterações e gera logs
            var alteracoes = new List<string>();

            if (gestorAtual.PerfilLinkedin != gestorAtualizado.PerfilLinkedin)
            {
                alteracoes.Add($"LinkedIn de: {gestorAtual.PerfilLinkedin} para: {gestorAtualizado.PerfilLinkedin}");
            }

            if (gestorAtual.Telefone != gestorAtualizado.Telefone)
            {
                alteracoes.Add($"Telefone de: {gestorAtual.Telefone} para: {gestorAtualizado.Telefone}");
            }

            if (gestorAtual.CodigoCliente != gestorAtualizado.CodigoCliente)
            {
                alteracoes.Add($"CodigoCliente de: {gestorAtual.CodigoCliente} para: {gestorAtualizado.CodigoCliente}");
            }

            if (gestorAtual.CodGestorExterno != gestorAtualizado.CodGestorExterno)
            {
                alteracoes.Add($"para: {gestorAtualizado.CodGestorExterno}");
            }

            if (alteracoes.Any())
            {
                foreach (var alteracao in alteracoes)
                {
                    _logDB.SaveLogDefaultInDatabase(
                        $"CargaGestoresCRM.GESTOR_ATUALIZADO.{encontradoPor}:[{contador}] CodGestorExterno: {gestorAtualizado.CodGestorExterno} {alteracao}",
                        string.Empty,
                        ProcessIdentifierEnum.SincronizaClienteCRMInProgress
                    );
                }
            }
            else
            {
                _logDB.SaveLogDefaultInDatabase(
                    $"CargaGestoresCRM.GESTOR_ATUALIZADO.{encontradoPor}:[{contador}] CodGestorExterno: {gestorAtualizado.CodGestorExterno} - Nenhuma alteração detectada.",
                    string.Empty,
                    ProcessIdentifierEnum.SincronizaClienteCRMInProgress
                );
            }
        }

        public async Task AtualizarProjetosComCodigoClienteBaseadoNaTabelaProjetoProposta()
        {
            var listaProjetosComProposta = await _projetoOrgRepository.ListarProjetosComPropostasAsync(_orgId);

            foreach (var proj in listaProjetosComProposta)
            {
                for (int i = 0; i < proj.CodigosProposta.Count();i++)
                {
                    if (!proj.CodigosProposta[0].StartsWith("P") && proj.CodigosProposta[0].StartsWith("20"))
                    {
                        proj.CodigosProposta[i] = "P" + proj.CodigosProposta[i];
                    }
                }
            }

            listaProjetosComProposta = listaProjetosComProposta.Where(c => !c.CodCliente.StartsWith("ACC"));
            
            var codigosPropostaParaFiltro = listaProjetosComProposta.SelectMany(projeto => projeto.CodigosProposta).ToList();

            var listaPropostasCRM = await _colaboracaoBridgeClient.GetCotacoesIncluindoAccountNoByQuoteNos(codigosPropostaParaFiltro);

            var clientesExistentes = await _clienteRepository.ListarClientesOrg(_orgId, null, null);

            var projetosAtualizados = new List<(string AccountNo, string CodCliente)>();

            var clientesCRM = await _colaboracaoBridgeClient.GetClientesFourmakersCrm();

            var clientesCRMNaoAutorizados = clientesCRM.Where(cliente => !string.IsNullOrWhiteSpace(cliente.Codigo_CCH) && cliente.Codigo_CCH != "0")
                                                  .GroupBy(cliente => cliente.Codigo_CCH)
                                                  .Where(grupo => grupo.Count() > 1)
                                                  .Select(grupo => grupo.First())
                                                  .ToList();

            var codigosDuplicados = string.Join(", ", clientesCRMNaoAutorizados.Select(grupo => grupo.Codigo_CCH));

            if (!string.IsNullOrWhiteSpace(codigosDuplicados))
            {
                _logDB.SaveLogDefaultInDatabase($"Os seguintes códigos CCH não serão atualizados pois estão duplicados no CRM: {codigosDuplicados}",string.Empty, ProcessIdentifierEnum.SincronizaClienteCRMInProgress);
            }

            try
            {
                foreach (var projeto in listaProjetosComProposta)
                {

                    // só está autorizado quem não está com código cch duplicado no crm
                    if (clientesCRMNaoAutorizados.Exists(x => x.Codigo_CCH == projeto.CodCliente))
                    {
                        continue;
                    }
                    
                    // pegamos as propostas que existem no crm para poder atualizar o codigo_cliente
                    var propostaCRM = listaPropostasCRM.FirstOrDefault(p => projeto.CodigosProposta.Contains(p.Quote_No));

                    if (propostaCRM != null)
                    {

                        //Console.WriteLine(projeto.CodCliente + " | " + propostaCRM.Account_no);

                        // verificar se o cliente já existe na nossa base
                        var clienteDB = clientesExistentes.FirstOrDefault(c => c.CodigoCliente == propostaCRM.Account_no);

                        if (clienteDB == null)
                        {
                            var clienteDTO = await CriarOuAtualizarClienteOrg(propostaCRM.Account_no, propostaCRM.Nome_Conta, _orgId);

                            // atualiza a lista local com o novo cliente (para não incluir dois clientes iguais)
                            clientesExistentes.Add(clienteDTO);
                        }

                        var projetoJaFoiAtualizado = projetosAtualizados.Any(p => p.AccountNo == propostaCRM.Account_no && p.CodCliente == projeto.CodCliente);

                        if (!projetoJaFoiAtualizado
                            && propostaCRM.Account_no != projeto.CodCliente)
                        {
                            var linhasAtualizadas = await _projetoOrgRepository.AtualizarCodigoClienteNosProjetosPorCodClienteAntigo(propostaCRM.Account_no, projeto.CodCliente, _orgId);
                            if (linhasAtualizadas > 0)
                            {
                                _logDB.SaveLogDefaultInDatabase($"AtualizarCodigoClienteNosProjetosPorCodClienteAntigo atualizou {linhasAtualizadas} linha(s) para CodCliente: {projeto.CodCliente} > CodCRM: {propostaCRM.Account_no}", string.Empty, ProcessIdentifierEnum.SincronizaClienteCRMInProgress);
                            }

                            var linhasAtualizadasGestoresExterno = await _projetoOrgRepository.AtualizarCodigoClienteNosGestoresExternosPorCodClienteAntigo(propostaCRM.Account_no, projeto.CodCliente, _orgId);
                            if (linhasAtualizadasGestoresExterno > 0)
                            {
                                _logDB.SaveLogDefaultInDatabase($"AtualizarCodigoClienteNosGestoresExternosPorCodClienteAntigo atualizou {linhasAtualizadas} linha(s) para CodCliente: {projeto.CodCliente} > CodCRM: {propostaCRM.Account_no}", string.Empty, ProcessIdentifierEnum.SincronizaClienteCRMInProgress);
                            }

                            projetosAtualizados.Add((propostaCRM.Account_no, projeto.CodCliente));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar projetos pela tabela de propostas", ex);
            }
        }
        public async Task<SincronizarCRMResult> GetDataUltimaSincronizacaoClientesEGestoresCRM()
        {
            var log = await _logDB.ObterUltimoLogPorProcessIdentifierAsync(ProcessIdentifierEnum.SincronizaClienteCRMCompleted);

            return new SincronizarCRMResult
            {
                DataUltimaExecucao = log.Data
            };
        }
        private async Task<ClienteOrgDTO> CriarOuAtualizarClienteOrg(string codigoCliente, string nomeCliente, int orgId)
        {
            var novoCliente = new ClienteOrgDTO() { CodigoCliente = codigoCliente, NomeCliente = nomeCliente };

            await _clienteRepository.UpsertClienteOrg(novoCliente, orgId, false, TipoCadastroClienteEnum.CADASTRO_CARGA_CRM);

            return novoCliente;
        }
    }

    public static class GestorCRMHelper
    {
        public static string ObterTelefone(DataTransferObject.Domain.CRM.ContactDetailsDTO gestorCRM)
        {
            return string.IsNullOrWhiteSpace(gestorCRM.Mobile.ToStringOuVazio())
                ? gestorCRM.Phone.ToStringOuVazio()
                : gestorCRM.Mobile.ToStringOuVazio();
        }

        public static string ObterPerfilLinkedin(string linkedin)
        {
            return linkedin.ToStringOuVazio().Split("?")[0];
        }
    }
}
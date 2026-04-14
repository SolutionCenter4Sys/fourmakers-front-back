using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Contratacao;
using Core.DomainModel;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.Contratacao;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Util.Enum;
using SRS.Domain.Interfaces.Service;
using SRS.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace SRS.Domain.Impl.Service
{
    [LogDomainClass]
    public class ContratacaoService : IContratacaoService
    {
        private readonly IContratacaoRepository _contratacaoRepository;
        private readonly ITemplateContratacaoValidatorService _templateValidatorService;
        private readonly ILogCore _log;
        private readonly ICandidaturaRepository _candidaturaRepository;
        private readonly IEnvioEmail _envioEmail;
        private readonly IAspNetUser _aspNetUser;

        public ContratacaoService(IContratacaoRepository contratacaoRepository, ITemplateContratacaoValidatorService templateValidatorService, ILogCore log, ICandidaturaRepository candidaturaRepository, IEnvioEmail envioEmail, IAspNetUser aspNetUser)
        {
            _contratacaoRepository = contratacaoRepository;
            _templateValidatorService = templateValidatorService;
            _log = log;
            _candidaturaRepository = candidaturaRepository;
            _envioEmail = envioEmail;
            _aspNetUser = aspNetUser;
        }

        public async Task<ApiGenericResult<TemplateDTO>> CriarTemplateAsync(TemplateDTO template, string codColaboradorLogado)
        {
            var result = new ApiGenericResult<TemplateDTO>();
            try
            {
                // Validações básicas
                if (template == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Template não pode ser nulo";
                    return result;
                }

                template.Id = Guid.NewGuid();

                // Verificar se já existe um template para este candidato
                if (String.IsNullOrEmpty(template.CandidatoVagaId))
                {
                    var existeTemplate = await _contratacaoRepository.ExisteTemplateParaCandidatoAsync(template.CandidatoVagaId);
                    if (existeTemplate)
                    {
                        result.Sucesso = false;
                        result.Mensagem = "Já existe um template para este candidato. Utilize a operação de atualização ao invés de criação.";
                        return result;
                    }
                }

                // Validar template usando o validador
                await _templateValidatorService.ValidarCriacaoTemplate(template);

                var templateCriado = await _contratacaoRepository.CriarTemplateAsync(template, codColaboradorLogado);

                // Preencher primeira opção de equipamento padrão
                await PreencherPrimeiraOpcaoEquipamentoPadraoAsync(templateCriado);

                result.Retorno = templateCriado;
                result.Sucesso = true;
                result.Mensagem = "Template criado com sucesso";

                return result;
            }
            catch (ApplicationException ex)
            {
                // Exceções de validação - retornar mensagem específica
                result.Sucesso = false;
                result.Mensagem = ex.Message;
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao criar template: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);

                result.Sucesso = false;
                result.Mensagem = "Erro interno ao criar template";
                return result;
            }
        }

        public async Task<ApiGenericResult<TemplateDTO>> ObterTemplatePorIdAsync(Guid id)
        {
            var result = new ApiGenericResult<TemplateDTO>();
            try
            {
                if (id == Guid.Empty)
                {
                    result.Sucesso = false;
                    result.Mensagem = "ID do template é obrigatório";
                    return result;
                }

                // Validar se o template existe usando o validador
                await _templateValidatorService.ValidarTemplate(id, null, CRUDEnum.Read);

                var template = await _contratacaoRepository.ObterTemplatePorIdAsync(id);

                // Preencher primeira opção de equipamento padrão
                await PreencherPrimeiraOpcaoEquipamentoPadraoAsync(template);

                result.Retorno = template;
                result.Sucesso = true;
                result.Mensagem = "Template obtido com sucesso";

                return result;
            }
            catch (ApplicationException ex)
            {
                // Exceções de validação - retornar mensagem específica
                result.Sucesso = false;
                result.Mensagem = ex.Message;
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao obter template: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);

                result.Sucesso = false;
                result.Mensagem = "Erro interno ao obter template";
                return result;
            }
        }

        public async Task<ApiGenericResult<TemplateDTO>> ObterTemplatePorCandidatoVagaIdAsync(Guid tbCandidatoVagaId)
        {
            var result = new ApiGenericResult<TemplateDTO>();
            try
            {
                if (tbCandidatoVagaId == Guid.Empty)
                {
                    result.Sucesso = false;
                    result.Mensagem = "ID da candidatura é obrigatório";
                    return result;
                }

                var template = await _contratacaoRepository.ObterTemplatePorCandidatoVagaIdAsync(tbCandidatoVagaId);

                if (template == null)
                {
                    // Se não existe template, buscar informações do colaborador
                    var informacoesColaborador = await _contratacaoRepository.ObterInformacoesColaboradorPorCandidatoAsync(tbCandidatoVagaId);

                    if (informacoesColaborador == null)
                    {
                        result.Sucesso = false;
                        result.Mensagem = "Candidato não encontrado";
                        return result;
                    }

                    // Gerar email corporativo baseado no nome
                    informacoesColaborador.EmailCorporativo = await GerarEmailCorporativoAsync(informacoesColaborador.NomeCompleto);

                    // Gerar login de rede baseado no email corporativo (parte antes do @)
                    informacoesColaborador.LoginRede = informacoesColaborador.EmailCorporativo.Split('@')[0];

                    // Preencher primeira opção de equipamento padrão
                    await PreencherPrimeiraOpcaoEquipamentoPadraoAsync(informacoesColaborador);

                    result.Retorno = informacoesColaborador;
                    result.Sucesso = true;
                    result.Mensagem = "Informações do colaborador obtidas com sucesso (template não existe)";
                }
                else
                {
                    // Preencher primeira opção de equipamento padrão
                    await PreencherPrimeiraOpcaoEquipamentoPadraoAsync(template);

                    result.Retorno = template;
                    result.Sucesso = true;
                    result.Mensagem = "Template obtido com sucesso";
                }

                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao obter template por candidato: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);

                result.Sucesso = false;
                result.Mensagem = "Erro interno ao obter template";
                return result;
            }
        }

        public async Task<ApiGenericResult<List<TemplateDTO>>> ListarTemplatesAsync(int limite = 10, int cursor = 0)
        {
            var result = new ApiGenericResult<List<TemplateDTO>>();
            try
            {
                if (limite <= 0 || limite > 100)
                {
                    limite = 10;
                }

                if (cursor < 0)
                {
                    cursor = 0;
                }

                var templates = await _contratacaoRepository.ListarTemplatesAsync(limite, cursor);

                // Preencher primeira opção de equipamento padrão para cada template
                await PreencherPrimeiraOpcaoEquipamentoPadraoAsync(templates);

                result.Retorno = templates;
                result.Sucesso = true;
                result.Mensagem = "Templates listados com sucesso";

                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao listar templates: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);

                result.Sucesso = false;
                result.Mensagem = "Erro interno ao listar templates";
                return result;
            }
        }

        public async Task<ApiGenericResult<TemplateDTO>> AtualizarTemplateAsync(Guid id, TemplateDTO template, string codColaboradorLogado)
        {
            var result = new ApiGenericResult<TemplateDTO>();
            try
            {
                if (id == Guid.Empty)
                {
                    result.Sucesso = false;
                    result.Mensagem = "ID do template é obrigatório";
                    return result;
                }

                if (template == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Template não pode ser nulo";
                    return result;
                }

                // Atualizar o ID do template
                template.Id = id;

                // Validar template usando o validador
                await _templateValidatorService.ValidarAtualizacaoTemplate(template);

                var templateAtualizado = await _contratacaoRepository.AtualizarTemplateAsync(template, codColaboradorLogado);

                // Preencher primeira opção de equipamento padrão
                await PreencherPrimeiraOpcaoEquipamentoPadraoAsync(templateAtualizado);

                result.Retorno = templateAtualizado;
                result.Sucesso = true;
                result.Mensagem = "Template atualizado com sucesso";

                return result;
            }
            catch (ApplicationException ex)
            {
                // Exceções de validação - retornar mensagem específica
                result.Sucesso = false;
                result.Mensagem = ex.Message;
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao atualizar template: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);

                result.Sucesso = false;
                result.Mensagem = "Erro interno ao atualizar template";
                return result;
            }
        }

        public async Task<ApiGenericResult<bool>> DeletarTemplateAsync(Guid id)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                if (id == Guid.Empty)
                {
                    result.Sucesso = false;
                    result.Mensagem = "ID do template é obrigatório";
                    return result;
                }

                // Validar se o template existe usando o validador
                await _templateValidatorService.ValidarTemplate(id, null, CRUDEnum.Delete);

                var deletado = await _contratacaoRepository.DeletarTemplateAsync(id);

                if (!deletado)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Erro ao deletar template";
                    return result;
                }

                result.Retorno = true;
                result.Sucesso = true;
                result.Mensagem = "Template deletado com sucesso";

                return result;
            }
            catch (ApplicationException ex)
            {
                // Exceções de validação - retornar mensagem específica
                result.Sucesso = false;
                result.Mensagem = ex.Message;
                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao deletar template: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);

                result.Sucesso = false;
                result.Mensagem = "Erro interno ao deletar template";
                return result;
            }
        }

        public async Task<ApiGenericResult<bool>> ExisteTemplateParaCandidatoAsync(string tbCandidatoVagaId)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                var existe = await _contratacaoRepository.ExisteTemplateParaCandidatoAsync(tbCandidatoVagaId);

                result.Retorno = existe;
                result.Sucesso = true;
                result.Mensagem = existe ? "Template já existe para este candidato" : "Nenhum template encontrado para este candidato";

                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao verificar existência de template para candidato: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);

                result.Sucesso = false;
                result.Mensagem = "Erro interno ao verificar existência de template";
                return result;
            }
        }

        private async Task<string> GerarEmailCorporativoAsync(string nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto))
                return string.Empty;

            // Remover acentos do nome completo
            var nomeSemAcentos = StringUtil.RemoveDiacritics(nomeCompleto);

            // Dividir o nome em partes
            var partesNome = nomeSemAcentos.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (partesNome.Length == 0)
                return string.Empty;

            var primeiroNome = partesNome[0].ToLower();
            var ultimoNome = partesNome[partesNome.Length - 1].ToLower();
            var emailSugerido = String.Empty;
            var existe = false;

            // Se ainda existe, tentar do último sobrenome até o segundo nome
            for (int i = partesNome.Length - 1; i >= 1; i--)
            {
                var sobrenome = partesNome[i].ToLower();
                emailSugerido = $"{primeiroNome}.{sobrenome}@foursys.com.br";
                existe = await _contratacaoRepository.ExisteEmailUsuarioAsync(emailSugerido);

                if (!existe)
                    return emailSugerido;
            }

            // Se todos existem, tentar concatenar todos os sobrenomes
            if (partesNome.Length > 2)
            {
                var todosSobrenomes = string.Join("", partesNome.Skip(1).Select(n => n.ToLower()));
                emailSugerido = $"{primeiroNome}.{todosSobrenomes}@foursys.com.br";
                existe = await _contratacaoRepository.ExisteEmailUsuarioAsync(emailSugerido);

                if (!existe)
                    return emailSugerido;
            }

            // Se ainda existe, retornar o primeiro formato com um número
            var contador = 1;
            do
            {
                emailSugerido = $"{primeiroNome}.{ultimoNome}{contador}@foursys.com.br";
                existe = await _contratacaoRepository.ExisteEmailUsuarioAsync(emailSugerido);
                contador++;
            } while (existe && contador <= 99);

            return emailSugerido;
        }

        public async Task<ApiGenericResult<List<EquipamentoPadraoDTO>>> ListarEquipamentosPadroesAsync()
        {
            var result = new ApiGenericResult<List<EquipamentoPadraoDTO>>();
            try
            {
                var equipamentos = await _contratacaoRepository.ListarEquipamentosPadroesAsync();

                result.Retorno = equipamentos;
                result.Sucesso = true;
                result.Mensagem = "Equipamentos padrões listados com sucesso";

                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao listar equipamentos padrões: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);

                result.Sucesso = false;
                result.Mensagem = "Erro interno ao listar equipamentos padrões";
                return result;
            }
        }

        public async Task<ApiGenericResult<List<EquipamentoPadraoAninhadoDTO>>> ListarEquipamentosPadroesAninhadosAsync()
        {
            var result = new ApiGenericResult<List<EquipamentoPadraoAninhadoDTO>>();
            try
            {
                var equipamentosAninhados = await _contratacaoRepository.ListarEquipamentosPadroesAninhadosAsync();

                result.Retorno = equipamentosAninhados;
                result.Sucesso = true;
                result.Mensagem = "Equipamentos padrões aninhados listados com sucesso";

                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao listar equipamentos padrões aninhados: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);

                result.Sucesso = false;
                result.Mensagem = "Erro interno ao listar equipamentos padrões aninhados";
                return result;
            }
        }

        public async Task<ApiGenericResult<bool>> ExisteEmailUsuarioAsync(string email)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Email é obrigatório";
                    return result;
                }

                var existe = await _contratacaoRepository.ExisteEmailUsuarioAsync(email);

                result.Retorno = existe;
                result.Sucesso = true;
                result.Mensagem = existe ? "Email já existe no sistema" : "Email disponível";

                return result;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao verificar existência de email: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);

                result.Sucesso = false;
                result.Mensagem = "Erro interno ao verificar existência de email";
                return result;
            }
        }

        public async Task<bool> EnviarEmailTemplateCandidato(string idCandidatura, byte[] documento, int orgId, bool anexo, string nomePDF, List<string> emailsAdicionais = null, bool ocultarValores = false)
        {
            try
            {
                // Org 9 (ROYAL): não enviar anexo
                bool enviarAnexo = anexo && orgId != EnumORG.ROYAL_9.ToInt();

                if (enviarAnexo)
                {
                    var content = PdfUtil.ToString(documento);

                    if (string.IsNullOrWhiteSpace(content))
                        throw new InvalidOperationException("O CV não possui conteúdo válido para extração.");
                }

                List<TemplateDestinatarioEmail> destinatarios;

                // Org 9 (ROYAL): usar apenas emails adicionais, não buscar na base
                if (orgId == EnumORG.ROYAL_9.ToInt())
                {
                    // Validar se há emails adicionais
                    if (emailsAdicionais == null || !emailsAdicionais.Any(e => !string.IsNullOrWhiteSpace(e)))
                    {
                        throw new ApplicationException("Para a organização Royal, é obrigatório informar pelo menos um email adicional.");
                    }

                    destinatarios = new List<TemplateDestinatarioEmail>();
                    
                    foreach (var email in emailsAdicionais.Where(e => !string.IsNullOrWhiteSpace(e)))
                    {
                        destinatarios.Add(new TemplateDestinatarioEmail
                        {
                            Email = email.Trim(),
                            Assunto = "Template Vaga Candidato",
                            Area = "Email Adicional",
                            Tb_Org_id = orgId,
                            Anexo = 0, // Org 9 não envia anexo
                            Ativo = 1
                        });
                    }
                }
                else
                {
                    // Outras orgs: buscar na base + emails adicionais
                    destinatarios = (await _candidaturaRepository.BuscaDestinatariosTemplateCandidatos(orgId, !ocultarValores)).ToList();

                    if (!destinatarios.Any())
                        throw new ApplicationException("Nenhum destinatário encontrado na base de dados.");

                    destinatarios.Add(new TemplateDestinatarioEmail
                    {
                        Email = _aspNetUser.GetUsuarioLogado().Email,
                        Assunto = "Template Vaga Candidato",
                        Area = "Usuario Logado",
                        Tb_Org_id = _aspNetUser.GetUsuarioLogado().OrgId,
                        Anexo = enviarAnexo ? 1 : 0,
                        Ativo = 1
                    });

                    if (emailsAdicionais != null && emailsAdicionais.Any())
                    {
                        foreach (var email in emailsAdicionais.Where(e => !string.IsNullOrWhiteSpace(e)))
                        {
                            destinatarios.Add(new TemplateDestinatarioEmail
                            {
                                Email = email.Trim(),
                                Assunto = "Template Vaga Candidato",
                                Area = "Email Adicional",
                                Tb_Org_id = _aspNetUser.GetUsuarioLogado().OrgId,
                                Anexo = enviarAnexo ? 1 : 0,
                                Ativo = 1
                            });
                        }
                    }
                }

                Guid candidatura;
                if(!Guid.TryParse(idCandidatura, out candidatura))
                    throw new ApplicationException($"Nenhuma candidatura sob este id {idCandidatura}");

                TemplateDTO template;

                // Org 9 (ROYAL): não buscar template completo, apenas nome e cargo
                if (orgId == EnumORG.ROYAL_9.ToInt())
                {
                    template = await _contratacaoRepository.ObterNomeECargoCandidatoAsync(candidatura);
                    
                    if (template == null)
                        throw new ApplicationException("Nenhum dado encontrado para o candidato.");
                    
                    // Não preencher equipamentos para org 9
                }
                else
                {
                    // Outras orgs: buscar template completo e preencher equipamentos
                    template = await _contratacaoRepository.ObterTemplatePorCandidatoVagaIdAsync(candidatura);

                    if (template == null)
                        throw new ApplicationException("Nenhum dado encontrado para o candidato.");

                    // Preencher opções de equipamento padrão antes de enviar o email
                    await PreencherOpcoesEquipamentoPadraoAsync(template);
                }

                _envioEmail.EnviaEmailTemplateCandidatos(destinatarios, template, documento, enviarAnexo, nomePDF, orgId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

            return true;
        }

        private string ConcatenarCamposOpcaoEquipamento(OpcaoEquipamentoDTO opcao)
        {
            if (opcao == null)
                return string.Empty;

            var partes = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(opcao.TipoOpcao))
                partes.Add(opcao.TipoOpcao);
            
            if (!string.IsNullOrWhiteSpace(opcao.CategoriaNome))
                partes.Add(opcao.CategoriaNome);
            
            if (!string.IsNullOrWhiteSpace(opcao.TipoEquipamento))
                partes.Add(opcao.TipoEquipamento);
            
            if (!string.IsNullOrWhiteSpace(opcao.CpuGeracao))
                partes.Add(opcao.CpuGeracao);
            
            if (!string.IsNullOrWhiteSpace(opcao.MemoriaRam))
                partes.Add(opcao.MemoriaRam);
            
            if (!string.IsNullOrWhiteSpace(opcao.ArmazenamentoDisco))
                partes.Add(opcao.ArmazenamentoDisco);
            
            if (!string.IsNullOrWhiteSpace(opcao.So))
                partes.Add(opcao.So);
            
            if (!string.IsNullOrWhiteSpace(opcao.Gpu))
                partes.Add(opcao.Gpu);
            
            if (opcao.ModelosPossiveis != null && opcao.ModelosPossiveis.Any())
                partes.Add(string.Join(",", opcao.ModelosPossiveis));
            
            if (!string.IsNullOrWhiteSpace(opcao.DescricaoUpgrade))
                partes.Add(opcao.DescricaoUpgrade);

            return string.Join("|", partes);
        }

        private async Task PreencherOpcoesEquipamentoPadraoAsync(TemplateDTO template)
        {
            if (template == null || string.IsNullOrWhiteSpace(template.EquipamentoPadraoCargoFuncaoId))
            {
                template.PrimeiraOpcaoEquipamentoPadraoCargoFuncao = string.Empty;
                template.SegundaOpcaoEquipamentoPadraoCargoFuncao = string.Empty;
                return;
            }

            try
            {
                // Buscar os equipamentos uma única vez
                var equipamentosAninhados = await _contratacaoRepository.ListarEquipamentosPadroesAninhadosAsync();

                // Buscar o cargo/função correspondente
                foreach (var grupoArea in equipamentosAninhados)
                {
                    foreach (var cargoFuncao in grupoArea.CargosFuncoes)
                    {
                        if (cargoFuncao.IdCargoFuncao == template.EquipamentoPadraoCargoFuncaoId)
                        {
                            var opcoes = cargoFuncao.OpcoesEquipamento;
                            
                            // Preencher primeira opção
                            var primeiraOpcao = opcoes?.FirstOrDefault();
                            template.PrimeiraOpcaoEquipamentoPadraoCargoFuncao = primeiraOpcao != null 
                                ? ConcatenarCamposOpcaoEquipamento(primeiraOpcao) 
                                : string.Empty;
                            
                            // Preencher segunda opção
                            if (opcoes != null && opcoes.Count > 1)
                            {
                                template.SegundaOpcaoEquipamentoPadraoCargoFuncao = ConcatenarCamposOpcaoEquipamento(opcoes[1]);
                            }
                            else
                            {
                                template.SegundaOpcaoEquipamentoPadraoCargoFuncao = string.Empty;
                            }
                            
                            return;
                        }
                    }
                }

                // Se não encontrou o cargo/função, definir como vazio
                template.PrimeiraOpcaoEquipamentoPadraoCargoFuncao = string.Empty;
                template.SegundaOpcaoEquipamentoPadraoCargoFuncao = string.Empty;
            }
            catch (Exception)
            {
                // Em caso de erro, definir como vazio para não quebrar o envio do email
                template.PrimeiraOpcaoEquipamentoPadraoCargoFuncao = string.Empty;
                template.SegundaOpcaoEquipamentoPadraoCargoFuncao = string.Empty;
            }
        }

        private async Task PreencherPrimeiraOpcaoEquipamentoPadraoAsync(TemplateDTO template)
        {
            await PreencherOpcoesEquipamentoPadraoAsync(template);
        }

        private async Task PreencherPrimeiraOpcaoEquipamentoPadraoAsync(List<TemplateDTO> templates)
        {
            if (templates != null && templates.Any())
            {
                foreach (var template in templates)
                {
                    await PreencherOpcoesEquipamentoPadraoAsync(template);
                }
            }
        }
    }
}


using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Contratacao;
using DataTransferObject.Domain.Contratacao;
using DataTransferObject.Domain.Endereco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DomainModel;
using Logs.Infra.Attributes;

namespace SRS.Domain.Impl.Service.Validadores
{
    [LogDomainClass]
    public class TemplateContratacaoValidatorService : ITemplateContratacaoValidatorService
    {
        private readonly IContratacaoRepository _contratacaoRepository;
        private readonly ICandidaturaRepository _candidaturaRepository;

        public TemplateContratacaoValidatorService(IContratacaoRepository contratacaoRepository, ICandidaturaRepository candidaturaRepository)
        {
            _contratacaoRepository = contratacaoRepository;
            _candidaturaRepository = candidaturaRepository;
        }

        public async Task ValidarTemplate(Guid templateId, TemplateDTO template, CRUDEnum cRUDEnum)
        {
            if (cRUDEnum == CRUDEnum.Read || cRUDEnum == CRUDEnum.Update || cRUDEnum == CRUDEnum.Delete)
            {
                if (templateId == Guid.Empty)
                {
                    throw new ApplicationException("ID do template não informado.");
                }

                await ValidarSeExisteTemplate(templateId);
            }

            if (cRUDEnum == CRUDEnum.Create || cRUDEnum == CRUDEnum.Update)
            {
                await ValidarCamposDeEntrada(template);
            }
        }

        public async Task ValidarCamposDeEntrada(TemplateDTO template)
        {
            var campos = new List<CampoValidacao>();

            // Campos obrigatórios básicos
            campos.Add(new("ID", template.CandidatoVagaId, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Código do Colaborador Analista", template.ColaboradorCodigoInternoColaboradorAnalista, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Código do Colaborador", template.ColaboradorCodigoInternoColaborador, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("ID da Candidatura", template.CandidatoVagaId, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Cargo", template.Cargo, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Data de Início", template.DataInicio, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Horário da Jornada", template.HorarioJornada, TipoValidacaoEnum.Obrigatoriedade));

            // Campos do colaborador (obrigatórios)
            campos.Add(new("Documento do Colaborador", template.DocumentoColaborador, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("RG do Colaborador", template.RgColaborador, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Data de Nascimento", template.DataNascimento, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Contato Principal", template.ContatoPrincipal, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Nome Completo", template.NomeCompleto, TipoValidacaoEnum.Obrigatoriedade));

            // Campos de email e acesso
            campos.Add(new("Email Pessoal", template.EmailPessoal, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Email Corporativo", template.EmailCorporativo, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Login de Rede", template.LoginRede, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Tipo de Login de Rede", template.TipoLoginRede, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Grupo de Email do Contrato", template.GrupoEmailContrato, TipoValidacaoEnum.Obrigatoriedade));

            // Validações específicas de formato
            campos.Add(new("Email Pessoal", template.EmailPessoal, TipoValidacaoEnum.ValidarEmail));
            campos.Add(new("Email Corporativo", template.EmailCorporativo, TipoValidacaoEnum.ValidarEmail));
            campos.Add(new("Contato Principal", template.ContatoPrincipal, TipoValidacaoEnum.ValidarTelefoneFixoOuCelular));
            
            // Validar Data de Início apenas se não for null
            if (template.DataInicio.HasValue)
            {
                campos.Add(new("Data de Início", template.DataInicio.Value.ToString("dd/MM/yyyy"), TipoValidacaoEnum.ValidarData));
            }
            
            // Validar Data de Nascimento apenas se não for null
            if (template.DataNascimento.HasValue)
            {
                campos.Add(new("Data de Nascimento", template.DataNascimento.Value.ToString("dd/MM/yyyy"), TipoValidacaoEnum.ValidarData));
            }

            // Validações de tamanho
            campos.Add(new("Cargo", template.Cargo, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 255 });
            campos.Add(new("Horário da Jornada", template.HorarioJornada, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 100 });
            campos.Add(new("Tipo de Horário da Jornada", template.TipoHorarioJornada, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 50 });
            campos.Add(new("Tipo de Login de Rede", template.TipoLoginRede, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 50 });
            campos.Add(new("Tipo de Máquina", template.TipoMaquina, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 100 });
            campos.Add(new("Login de Rede", template.LoginRede, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 100 });
            campos.Add(new("Email Pessoal", template.EmailPessoal, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 255 });
            campos.Add(new("Email Corporativo", template.EmailCorporativo, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 255 });
            campos.Add(new("Grupo de Email do Contrato", template.GrupoEmailContrato, TipoValidacaoEnum.TamanhoMaximo) { TamanhoMaximo = 255 });

            // Validação dos outros grupos (lista de emails)
            if (template.OutrosGrupos != null && template.OutrosGrupos.Any())
            {
                foreach (var grupo in template.OutrosGrupos)
                {
                    if (string.IsNullOrWhiteSpace(grupo.EmailGrupo))
                    {
                        throw new ApplicationException("Email do grupo não pode estar vazio.");
                    }
                    
                    // Validar formato do email
                    var emailValidation = new CampoValidacao("Email do Grupo", grupo.EmailGrupo, TipoValidacaoEnum.ValidarEmail);
                    await ValidadorCamposUtil.ValidaCampos(new List<CampoValidacao> { emailValidation });
                }
            }

            // Validação do endereço (se fornecido)
            if (template.Endereco != null)
            {
                await ValidarEndereco(template.Endereco);
            }

            await ValidadorCamposUtil.ValidaCampos(campos);

            // Validações específicas de existência de colaboradores
            await ValidarColaboradoresExistem(template);
        }

        private async Task ValidarColaboradoresExistem(TemplateDTO template)
        {
            // Validar ColaboradorCodigoInternoColaboradorAnalista
            if (!string.IsNullOrWhiteSpace(template.ColaboradorCodigoInternoColaboradorAnalista))
            {
                var existeAnalista = await _contratacaoRepository.ExisteColaboradorPorCodigoInternoAsync(template.ColaboradorCodigoInternoColaboradorAnalista);
                if (!existeAnalista)
                {
                    throw new ApplicationException($"Campo analista de ReS e obrigatorio.");
                }
            }

            // Validar ColaboradorCodigoInternoColaborador
            if (!string.IsNullOrWhiteSpace(template.ColaboradorCodigoInternoColaborador))
            {
                var existeColaborador = await _contratacaoRepository.ExisteColaboradorPorCodigoInternoAsync(template.ColaboradorCodigoInternoColaborador);
                if (!existeColaborador)
                {
                    throw new ApplicationException($"Colaborador com código interno '{template.ColaboradorCodigoInternoColaborador}' não encontrado na base de dados.");
                }
            }

            // Validar ColaboradorCodigoInternoColaboradorSuperiorImediato
            // Nao vai mais ser validado esse campo
            //if (!string.IsNullOrWhiteSpace(template.ColaboradorCodigoInternoColaboradorSuperiorImediato))
            //{
            //    var existeSuperior = await _contratacaoRepository.ExisteColaboradorPorCodigoInternoAsync(template.ColaboradorCodigoInternoColaboradorSuperiorImediato);
            //    if (!existeSuperior)
            //    {
            //        throw new ApplicationException($"Campo superior imediato e obrigatorio.");
            //    }
            //}

            // Validar EquipamentoPadraoCargoFuncaoId
            if (!string.IsNullOrWhiteSpace(template.EquipamentoPadraoCargoFuncaoId))
            {
                var existeCargoFuncao = await _contratacaoRepository.ExisteEquipamentoPadraoCargoFuncaoAsync(template.EquipamentoPadraoCargoFuncaoId);
                if (!existeCargoFuncao)
                {
                    throw new ApplicationException($"Cargo/Função com ID '{template.EquipamentoPadraoCargoFuncaoId}' não encontrado na tabela de equipamentos padrão.");
                }
            }
        }

        public async Task ValidarCriacaoTemplate(TemplateDTO template)
        {
            await ValidarCamposDeEntrada(template);
            
            // Validações específicas para criação
            await ValidarSeTemplateJaExiste(template.Id, template.CandidatoVagaId);
            
            // Validar se a candidatura existe no banco de dados
            await ValidarSeCandidatoVagaExiste(template.CandidatoVagaId);
        }

        public async Task ValidarAtualizacaoTemplate(TemplateDTO template)
        {
            await ValidarCamposDeEntrada(template);
            
            // Validações específicas para atualização
            await ValidarSeExisteTemplate(template.Id);
            
            // Validar se a candidatura existe no banco de dados
            if (!string.IsNullOrEmpty(template.CandidatoVagaId))
            {
                await ValidarSeCandidatoVagaExiste(template.CandidatoVagaId);
                await ValidarSeJaExisteTemplateParaCandidato(template.CandidatoVagaId, template.Id);
            }
        }

        private async Task ValidarSeExisteTemplate(Guid templateId)
        {
            var templateExistente = await _contratacaoRepository.ObterTemplatePorIdAsync(templateId);
            if (templateExistente == null)
            {
                throw new ApplicationException($"Template com ID {templateId} não encontrado.");
            }
        }

        private async Task ValidarSeTemplateJaExiste(Guid templateId, string candidatoVagaId)
        {
            var templateExistente = await _contratacaoRepository.ObterTemplatePorIdAsync(templateId);
            if (templateExistente != null)
                throw new ApplicationException($"Template com ID {templateId} já existe.");

            Guid candidatura;
            if(!Guid.TryParse(candidatoVagaId, out candidatura))
                throw new ApplicationException($"Candidatura {templateId} nao existe.");

            templateExistente = await _contratacaoRepository.ObterTemplatePorCandidatoVagaIdAsync(candidatura);
            if (templateExistente != null)
                throw new ApplicationException($"Template com esta candidatura {templateId} já existe.");
        }

        private async Task ValidarSeJaExisteTemplateParaCandidato(string candidatoVagaId, Guid templateIdAtual)
        {
            var templateExistente = await _contratacaoRepository.ObterTemplatePorCandidatoVagaIdAsync(Guid.Parse(candidatoVagaId));
            if (templateExistente != null && templateExistente.Id != templateIdAtual)
            {
                throw new ApplicationException($"Já existe um template para o candidato {candidatoVagaId}. Utilize a operação de atualização no template existente.");
            }
        }

        private async Task ValidarSeCandidatoVagaExiste(string candidatoVagaId)
        {
            var candidatoVagaExiste = await _candidaturaRepository.EstaCandidaturaExiste(candidatoVagaId);
            if (!candidatoVagaExiste)
            {
                throw new ApplicationException($"Candidatura com ID {candidatoVagaId} não encontrada no banco de dados.");
            }
        }

        private async Task ValidarEndereco(EnderecoDTO endereco)
        {
            var campos = new List<CampoValidacao>();

            campos.Add(new("CEP", endereco.Cep, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Endereço", endereco.Endereco, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Número", endereco.Numero, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Bairro", endereco.Bairro, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Cidade", endereco.Cidade, TipoValidacaoEnum.Obrigatoriedade));
            campos.Add(new("Estado", endereco.Estado, TipoValidacaoEnum.Obrigatoriedade));

            // Validações de tamanho para endereço
            campos.Add(new("CEP", endereco.Cep, TipoValidacaoEnum.TamanhoExato) { TamanhoExato = 8 });
            campos.Add(new("Estado", endereco.Estado, TipoValidacaoEnum.TamanhoExato) { TamanhoExato = 2 });

            await ValidadorCamposUtil.ValidaCampos(campos);
        }
    }
}

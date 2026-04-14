import { inject, injectable } from 'tsyringe'

import type {
  ColetaPerfilColaboradorCampanhaRequest,
  ColetaPerfilColaboradorCampanhaResponse,
} from '@domain/entities/PerfilColaboradorCampanha'
import type {
  PerfilColaboradorCampanhaValidationData,
  ValidationError,
} from '@domain/entities/PerfilColaboradorCampanhaValidation'
import type { CampanhaRepository } from '@domain/repositories/CampanhaRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ColetaPerfilColaboradorCampanhaUseCase {
  constructor(
    @inject(DiTokens.campanhaRepository)
    private readonly repository: CampanhaRepository,
  ) {}

  /**
   * Valida os dados antes de enviar para a API
   * @param validationData Dados para validação
   * @returns Erro de validação ou null se válido
   */
  validate(validationData: PerfilColaboradorCampanhaValidationData): ValidationError | null {
    // Validação condicional do bairro (obrigatório exceto para Estados Unidos)
    if (validationData.paisSelecionado !== "eua" && !validationData.bairro) {
      return { message: "Bairro é obrigatório para este país." }
    }

    // Validações condicionais baseadas no modelo de trabalho
    if (validationData.modeloTrabalho === "presencial" || validationData.modeloTrabalho === "hibrido") {
      // Local de trabalho é obrigatório para presencial e híbrido
      if (!validationData.localTrabalho || validationData.localTrabalho.trim() === "") {
        return { message: "Local de trabalho é obrigatório para este modelo." }
      }

      // Se local de trabalho é cliente, deve ter cliente selecionado
      if (validationData.localTrabalho === "cliente" && !validationData.clienteSelecionado) {
        return { message: "Selecione um cliente." }
      }
    }

    // Validações específicas para híbrido
    if (validationData.modeloTrabalho === "hibrido") {
      if (!validationData.frequenciaId || validationData.frequenciaId === 0) {
        return { message: "Frequência é obrigatória para modelo híbrido." }
      }
      if (!validationData.diasSemana || validationData.diasSemana.length === 0) {
        return { message: "Selecione pelo menos um dia da semana." }
      }
    }

    if (validationData.possuiCertificadoAWS === "sim") {
      const certificadosPreenchidos = Object.values(validationData.certificadosAWS).some(
        (cert) => cert.status !== ""
      )
      if (!certificadosPreenchidos) {
        return { message: "Preencha pelo menos uma certificação AWS." }
      }
    }

    if (!validationData.comprovanteFile) {
      return { message: "É necessário anexar o comprovante de residência." }
    }

    return null
  }

  async execute(
    token: string,
    payload: ColetaPerfilColaboradorCampanhaRequest
  ): Promise<ColetaPerfilColaboradorCampanhaResponse> {
    return this.repository.coletaPerfilColaboradorCampanha(token, payload)
  }
}


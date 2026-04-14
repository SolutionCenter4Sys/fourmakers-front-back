import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import { CandidateApi } from '@data/api/CandidateApi';
import type { CandidateDetails } from '@domain/entities/CandidateDetails';
import type { CandidateRepository } from '@domain/repositories/CandidateRepository';

@injectable()
export class CandidateRepositoryImpl implements CandidateRepository {
  constructor(
    @inject(DiTokens.candidateApi)
    private readonly api: CandidateApi
  ) {}

  async getCandidateDetails(token: string, candidateId: string): Promise<CandidateDetails> {
    const response = await this.api.buscarDados(token, candidateId);
    const colaborador = response?.colaborador;

    if (!colaborador) {
      throw new Error('Detalhes do colaborador não encontrados.');
    }

    return {
      cpf: colaborador.documentoColaborador ?? colaborador.cpf ?? '',
      nomeCompleto: colaborador.nomeCompleto,
      email: colaborador.email ?? null,
      contatoPrincipal: colaborador.contatoPrincipal ?? null,
      rg: colaborador.rg ?? null,
      dataNascimento: colaborador.dataNascimento ?? null,
      cargo: colaborador.cargo?.cargo ?? null,
      diretoriaNome: colaborador.diretoria?.diretoria ?? null,
      endereco: {
        cep: colaborador.endereco?.cep ?? null,
        endereco: colaborador.endereco?.endereco ?? null,
        complemento: colaborador.endereco?.complemento ?? null,
        numero: colaborador.endereco?.numero ?? null,
        bairro: colaborador.endereco?.bairro ?? null,
        cidade: colaborador.endereco?.cidade ?? null,
        estado: colaborador.endereco?.estado ?? null,
      },
    };
  }
}

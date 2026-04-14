import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import { CandidatoListApi, type ListarCandidatosRequest } from '@data/api/CandidatoListApi';
import type { CandidatoListItem } from '@domain/entities/CandidatoListItem';
import type { CandidatoListRepository } from '@domain/repositories/CandidatoListRepository';

@injectable()
export class CandidatoListRepositoryImpl implements CandidatoListRepository {
  constructor(
    @inject(DiTokens.candidatoListApi)
    private readonly api: CandidatoListApi,
  ) {}

  async listCandidatos(token: string, vagaId: string): Promise<CandidatoListItem[]> {
    const payload: ListarCandidatosRequest = {
      busca: '',
      cursor: 0,
      limite: 100000,
      dataInicio: '2000-01-01',
      dataFim: '2100-01-01',
      vagaId,
      qualificados: false,
      diasUltimaAlteracao: 0,
      localizacaoCidade: '',
      localizacaoEstado: '',
    };
    const response = await this.api.listarCandidatos(token, payload);
    return (
      response.retorno?.map((item) => ({
        idCandidatura: item.idCandidatura,
        nome: item.nome,
        codigo: item.codigo,
        emailUsuario: item.emailUsuario ?? null,
        emailAlternativo: item.emailAlternativo ?? null,
        descricaoStatusCandidatura: item.descricaoStatusCandidatura ?? null,
      })) || []
    );
  }
}

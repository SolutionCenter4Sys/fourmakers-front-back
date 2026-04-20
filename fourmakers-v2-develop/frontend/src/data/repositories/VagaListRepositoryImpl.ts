import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import { VagaListApi, type ListarVagasRequest } from '@data/api/VagaListApi';
import type { VagaListItem } from '@domain/entities/VagaListItem';
import type { VagaListRepository } from '@domain/repositories/VagaListRepository';

@injectable()
export class VagaListRepositoryImpl implements VagaListRepository {
  constructor(
    @inject(DiTokens.vagaListApi)
    private readonly api: VagaListApi,
  ) {}

  async listVagas(token: string): Promise<VagaListItem[]> {
    const payload: ListarVagasRequest = {
      limite: 100000,
      cursor: 0,
      busca: '',
      status: [],
      dataInicio: '2025-01-01',
      dataFim: '2030-01-01',
    };
    const response = await this.api.listarVagas(token, payload);
    return (
      response.retorno?.map((item) => ({
        id: item.id,
        codigo: item.codigo ?? null,
        titulo: item.titulo ?? null,
        modeloTrabalhoDescricao: item.modeloTrabalhoDescricao ?? null,
        nomeGestor: item.nomeGestor ?? null,
        nomeCliente: item.nomeCliente ?? null,
        codigoCliente: item.codigoCliente ?? null,
        quantidadeCandidatosPorEstagio:
          Array.isArray(item.quantidadeCandidatosPorEstagio) && item.quantidadeCandidatosPorEstagio.length > 0
            ? item.quantidadeCandidatosPorEstagio.map((e) => ({
                idStatus: e.idStatus,
                descricaoStatus: e.descricaoStatus,
                quantidade: e.quantidade,
              }))
            : undefined,
      })) || []
    );
  }
}

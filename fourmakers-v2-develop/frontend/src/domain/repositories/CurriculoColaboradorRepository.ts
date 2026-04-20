import type {
  ImportarColaboradorResult,
  ImportarColaboradorLoteResult,
  ImportarColaboradorLotePlanilhaResult,
} from '@domain/entities/CurriculoColaborador';

export interface SincronizarCurriculoResult {
  sucesso: boolean;
  mensagem?: string | null;
}

export interface InsereCurriculoColaboradorResult {
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: unknown;
}

export interface CurriculoColaboradorRepository {
  importarColaborador(token: string, file: File): Promise<ImportarColaboradorResult>;
  importarColaboradorLinkedin(token: string, perfilId: string): Promise<ImportarColaboradorResult>;
  importarColaboradorLote(token: string, file: File): Promise<ImportarColaboradorLoteResult>;
  importarColaboradorLotePlanilha(token: string, file: File): Promise<ImportarColaboradorLotePlanilhaResult>;
  sincronizarCurriculoColaborador(token: string, file: File): Promise<SincronizarCurriculoResult>;
  /** Insere/atualiza currículo do colaborador logado (portal público de vagas). */
  insereCurriculoColaborador(token: string, file: File): Promise<InsereCurriculoColaboradorResult>;
}

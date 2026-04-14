import { injectable } from 'tsyringe';
import { httpClient } from './httpClient';

export interface ImportarColaboradorResponse {
  sucesso?: boolean;
  mensagem?: string | null;
  /** Grafia retornada pelo backend em respostas de erro. */
  mensagen?: string | null;
  erros?: unknown;
  /** Código do colaborador criado (UUID). */
  codigoColaborador?: string;
}

export interface ImportarColaboradorLinkedinResponse {
  sucesso?: boolean;
  mensagem?: string | null;
  /** Grafia retornada pelo backend em respostas de erro. */
  mensagen?: string | null;
  erros?: unknown;
  /** Código do colaborador criado (UUID). */
  codigoColaborador?: string;
}

/** Item retornado em ImportarColaboradorLote (ZIP). */
export interface PdfProcessadoItem {
  nomeArquivo: string;
  aProcessar: boolean;
  erro: string | null;
}

/** Retorno de sucesso da importação em lote (ZIP). */
export interface ImportarColaboradorLoteRetorno {
  pdfsProcessados: PdfProcessadoItem[];
  totalArquivos: number;
  arquivosAProcessar: number;
  arquivosComErro: number;
  idLote: string;
}

export interface ImportarColaboradorLoteResponse {
  retorno?: ImportarColaboradorLoteRetorno;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: unknown;
}

/** Retorno de sucesso da importação em lote via planilha (LinkedIn). */
export interface ImportarColaboradorLotePlanilhaRetorno {
  totalLinhas: number;
  linhasAProcessar: number;
  linhasComErro: number;
  idLote: string;
}

export interface ImportarColaboradorLotePlanilhaResponse {
  retorno?: ImportarColaboradorLotePlanilhaRetorno;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: unknown;
}

@injectable()
export class CurriculoColaboradorApi {
  /**
   * Importa colaborador a partir de currículo em PDF (multipart/form-data, campo File).
   */
  async importarColaborador(token: string, file: File): Promise<ImportarColaboradorResponse> {
    const formData = new FormData();
    formData.append('File', file);
    return httpClient.request<ImportarColaboradorResponse>({
      url: '/api/CurriculoColaborador/ImportarColaborador',
      method: 'POST',
      token,
      body: formData,
    });
  }

  /**
   * Insere/atualiza currículo do colaborador logado (portal público de vagas).
   */
  async insereCurriculoColaborador(
    token: string,
    file: File
  ): Promise<{ sucesso?: boolean; mensagem?: string | null; erros?: unknown }> {
    const formData = new FormData();
    formData.append('File', file, file.name);
    return httpClient.request<{ sucesso?: boolean; mensagem?: string | null; erros?: unknown }>({
      url: '/api/CurriculoColaborador/InserirCurriculoColaborador',
      method: 'POST',
      token,
      body: formData,
    });
  }

  /**
   * Importa colaborador a partir do perfil LinkedIn (JSON com PerfilIN = id do perfil).
   */
  async importarColaboradorLinkedin(
    token: string,
    perfilId: string
  ): Promise<ImportarColaboradorLinkedinResponse> {
    return httpClient.post<ImportarColaboradorLinkedinResponse>(
      '/api/CurriculoColaborador/ImportarColaboradorLinkedin',
      { PerfilIN: perfilId },
      { token }
    );
  }

  /**
   * Importa colaboradores em lote a partir de um arquivo ZIP contendo PDFs (multipart/form-data, campo File).
   */
  async importarColaboradorLote(
    token: string,
    file: File
  ): Promise<ImportarColaboradorLoteResponse> {
    const formData = new FormData();
    formData.append('File', file);
    return httpClient.request<ImportarColaboradorLoteResponse>({
      url: '/api/CurriculoColaborador/ImportarColaboradorLote',
      method: 'POST',
      token,
      body: formData,
    });
  }

  /**
   * Sincroniza currículo do colaborador (atualiza dados profissionais via CV). Multipart form-data, campo "file".
   */
  async sincronizarCurriculoColaborador(token: string, file: File): Promise<{ sucesso?: boolean; mensagem?: string | null; erros?: unknown }> {
    const formData = new FormData();
    formData.append('file', file);
    return httpClient.request<{ sucesso?: boolean; mensagem?: string | null; erros?: unknown }>({
      url: '/api/CurriculoColaborador/SincronizarCurriculoColaborador',
      method: 'POST',
      token,
      body: formData,
    });
  }

  /**
   * Importa colaboradores em lote a partir de planilha Excel com links/perfis (multipart/form-data, campo File).
   */
  async importarColaboradorLotePlanilha(
    token: string,
    file: File
  ): Promise<ImportarColaboradorLotePlanilhaResponse> {
    const formData = new FormData();
    formData.append('File', file);
    return httpClient.request<ImportarColaboradorLotePlanilhaResponse>({
      url: '/api/CurriculoColaborador/ImportarColaboradorLoteLinkedin',
      method: 'POST',
      token,
      body: formData,
    });
  }
}

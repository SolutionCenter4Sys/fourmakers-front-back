import { API_BASE_URL } from '@shared/constants';
import { createHttpClient } from './httpClient';

const normalizeApiBase = (base: string) => (base.endsWith('/api') ? base : `${base}/api`);
const BASE_URL = normalizeApiBase(API_BASE_URL);
const IA_BASE = normalizeApiBase(import.meta.env.VITE_API_FOURMAKERS_IA_URL || 'https://fourmakershub-api.dev.fourmakers.io/api');

interface ApiListResponse<T> {
  retorno: T[];
}

const httpClient = createHttpClient({ baseURL: BASE_URL });
const iaClient = createHttpClient({ baseURL: IA_BASE });

export const fetchPermanencias = async (token: string) => {
  const data = await httpClient.get<ApiListResponse<{ id: string; descricao: string }>>(
    '/GestaoDeAlocados/ListarPermanencias',
    { token }
  );
  return data?.retorno || [];
};

export const fetchModelosTrabalho = async (token: string) => {
  // Endpoint correto: ListarModelosTrabalho (sem "De")
  const data = await httpClient.get<ApiListResponse<{ id: string; descricao: string; codigo: number }>>(
    '/GestaoDeAlocados/ListarModelosTrabalho',
    { token }
  );
  return data?.retorno || [];
};

export const fetchLocalidades = async (token: string) => {
  // Endpoint válido para localidades de profissionais
  const data = await httpClient.get<ApiListResponse<{ id: string; descricao: string }>>(
    '/GestaoDeAlocados/ListarProfissionaisLocalidades',
    { token }
  );
  return data?.retorno || [];
};

export const fetchTiposEmprego = async (token: string) => {
  // Endpoint válido (LinkedIn)
  const data = await httpClient.get<ApiListResponse<{ id: string; descricao: string }>>(
    '/Vaga/ListarTiposEmpregosLinkedin',
    { token }
  );
  return data?.retorno || [];
};

export const fetchNiveisExperiencia = async (token: string) => {
  // Endpoint válido (LinkedIn)
  const data = await httpClient.get<ApiListResponse<{ id: string; descricao: string }>>(
    '/Vaga/ListarNiveisExperienciaLinkedin',
    { token }
  );
  return data?.retorno || [];
};

// Skills APIs
const buildSearchParams = (busca: string) => {
  const params = new URLSearchParams();
  if (busca) params.set('busca', busca);
  params.set('cursor', '0');
  params.set('limite', '500');
  return params.toString();
};

export const fetchCompetencias = async (token: string, busca: string) => {
  const data = await httpClient.get<{ competencias?: { id: number; descricao: string; nome?: string | null }[] }>(
    `/Competencia/ListarCompetencia?${buildSearchParams(busca)}`,
    { token }
  );
  return data?.competencias || [];
};

export const fetchSoftskills = async (token: string, busca: string) => {
  const data = await httpClient.get<{ retorno?: { id: number; descricao: string }[] }>(
    `/Competencia/Softskill/ListarSoftskill?${buildSearchParams(busca)}`,
    { token }
  );
  return data?.retorno || [];
};

export const fetchMetodologias = async (token: string, busca: string) => {
  const data = await httpClient.get<{ retorno?: { id: number; descricao: string }[] }>(
    `/Competencia/Metodologia/ListarMetodologia?${buildSearchParams(busca)}`,
    { token }
  );
  return data?.retorno || [];
};

export const fetchDominios = async (token: string, busca: string) => {
  const data = await httpClient.get<{ dominio?: { id: number; descricao: string; nome?: string }[] }>(
    `/Competencia/Dominio/ListarDominio?${buildSearchParams(busca)}`,
    { token }
  );
  return data?.dominio || [];
};

export const fetchIdiomas = async (token: string, busca: string) => {
  const data = await httpClient.get<{ idioma?: { id: number; descricao: string; nome?: string | null }[] }>(
    `/Competencia/Idioma/ListarIdioma?${buildSearchParams(busca)}`,
    { token }
  );
  return data?.idioma || [];
};

export const fetchNiveisCompetencia = async (token: string) => {
  const data = await httpClient.get<{ niveis?: { id: number; descricao: string }[] }>(
    '/Competencia/ListarNivelCompetencia',
    { token }
  );
  return (data?.niveis || []).filter(n => n.descricao.toLowerCase() !== 'a definir');
};

export const fetchNiveisSoftskill = async (token: string) => {
  const data = await httpClient.get<{ retorno?: { id: number; descricao: string }[] }>(
    '/Competencia/Softskill/ListarNivelSoftskill',
    { token }
  );
  return (data?.retorno || []).filter(n => n.descricao.toLowerCase() !== 'a definir');
};

export const fetchNiveisMetodologia = async (token: string) => {
  const data = await httpClient.get<{ retorno?: { id: number; descricao: string }[] }>(
    '/Competencia/Metodologia/ListarNivelMetodologia',
    { token }
  );
  return (data?.retorno || []).filter(n => n.descricao.toLowerCase() !== 'a definir');
};

export const fetchNiveisDominio = async (token: string) => {
  const data = await httpClient.get<{ niveis?: { id: number; descricao: string }[] }>(
    '/Competencia/Dominio/ListarNivelDominio',
    { token }
  );
  return (data?.niveis || []).filter(n => n.descricao.toLowerCase() !== 'a definir');
};

export const fetchNiveisIdioma = async (token: string) => {
  const data = await httpClient.get<{ niveis?: { id: number; descricao: string }[] }>(
    '/Competencia/Idioma/ListarNivelIdioma',
    { token }
  );
  return (data?.niveis || []).filter(n => n.descricao.toLowerCase() !== 'a definir');
};

export const extrairPerfilPorIA = async (token: string, prompt: string) => {
  return iaClient.post<{
    perfil_extraido?: {
      codGestorExterno?: string | null;
      nomePerfil?: string;
      custoPerfil?: number | null;
      ratecardPerfil?: number | null;
      informacoesRelevantes?: string | null;
      permanenciaId?: string | null;
      modeloTrabalhoId?: string | null;
      profissionalLocalidadeId?: string | null;
      cidade?: string | null;
      estado?: string | null;
      cep?: string | null;
      hibridoDias?: number | null;
      gestorExternoPerfilSkills?: {
        itemPerfil: { id: number; descricao: string };
        skill: { id: number; descricao: string };
        nivel: { id: number; descricao: string };
      }[];
      nivelExperienciaLinkedin?: string | null;
      tipoEmpregoLinkedin?: string | null;
      atribuicoes?: string | null;
      informacoesLinkedin?: string | null;
    };
    validacao_informacoes?: {
      resumo_informacoes?: Record<string, string | null>;
      completude_percentual?: number;
    };
  }>(
    '/GestaoDeAlocados/Perfil/ExtrairPerfilDeUmPrompt',
    { texto_vaga: prompt },
    { token }
  );
};

export const fetchClientes = async (token: string, busca: string) => {
  const params = new URLSearchParams();
  params.set('cursor', '0');
  params.set('limite', '5000');
  if (busca) {
    params.set('buscaCodigoOuNome', busca);
    params.set('busca', busca);
  }
  const data = await httpClient.get<ApiListResponse<{ id: string; codigoCliente: string; nomeCliente: string }>>(
    `/GestaoDeAlocados/ListarClienteOrgDaGestaoDeAlocados?${params.toString()}`,
    { token }
  );
  return data?.retorno || [];
};

export const fetchGestores = async (
  token: string,
  clientCode: string | null,
  busca: string,
  cursor = 0,
  limite = 5000,
) => {
  const params = new URLSearchParams();
  params.set('Cursor', String(cursor));
  params.set('Limite', String(limite));
  if (clientCode) params.set('codigoCliente', clientCode);
  if (busca) params.set('busca', busca);
  const data = await httpClient.get<ApiListResponse<{
    codGestorExterno: string;
    codigoInternoColaborador: string;
    nome: string;
    email: string;
  }>>(
    `/GestaoDeAlocados/GestorExterno/ListarGestoresExterno?${params.toString()}`,
    { token }
  );
  return data?.retorno || [];
};

export const obterGestorExternoPerfilPorId = async (token: string, id: string) => {
  const data = await httpClient.get<{
    retorno?: {
      id: string;
      codGestorExterno?: string | null;
      nomePerfil?: string | null;
      custoPerfil?: number | null;
      ratecardPerfil?: number | null;
      informacoesRelevantes?: string | null;
      permanenciaId?: string | null;
      modeloTrabalhoId?: string | null;
      modeloTrabalhoDescricao?: string | null;
      profissionalLocalidadeId?: string | null;
      cidade?: string | null;
      estado?: string | null;
      cep?: string | null;
      hibridoDias?: number | null;
      tipoEmpregoLinkedin?: string | null;
      nivelExperienciaLinkedin?: string | null;
      atribuicoes?: string | null;
      gestorExternoPerfilSkills?: Array<{
        itemPerfil: { id: number; descricao: string };
        skill: { id: number; descricao: string };
        nivel: { id: number; descricao: string };
        relevante?: boolean;
      }>;
    };
    sucesso?: boolean;
    mensagem?: string | null;
  }>(`/GestaoDeAlocados/GestorExternoPerfil/ObterGestorExternoPerfilPorId?id=${encodeURIComponent(id)}`, { token });
  return data?.retorno ?? null;
};

export const inserirGestorExternoPerfil = async (token: string, payload: unknown) => {
  return httpClient.post<{ sucesso: boolean; mensagem?: string }>(
    '/GestaoDeAlocados/GestorExternoPerfil/InserirGestorExternoPerfil',
    payload,
    { token }
  );
};

export const atualizarGestorExternoPerfil = async (token: string, id: string, payload: unknown) => {
  return httpClient.put<{ sucesso: boolean; mensagem?: string }>(
    `/GestaoDeAlocados/GestorExternoPerfil/AtualizarGestorExternoPerfil?id=${encodeURIComponent(id)}`,
    payload,
    { token }
  );
};

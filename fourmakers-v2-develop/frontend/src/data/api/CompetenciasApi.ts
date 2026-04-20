import type { MinhaJornadaInteresse } from '@domain/entities/MinhaJornadaInteresse'
import type { 
  ListarSkillsSumarioParams, 
  ListarSkillsSumarioResponse,
  ListarTodasSkillsColaboradorParams,
  ListarTodasSkillsColaboradorResponse,
  SumarioCompetenciasListarColaboradoresParams,
  SumarioCompetenciasListarColaboradoresResponse,
} from '@domain/entities/Competencia'
import { httpClient } from './httpClient'

// ESTES ENDPOINTS SÃO LEGADO E PODEM HAVER DIVERGENCIAS EM PARAMETROS E NA FORMA DE CHAMAD OS ENDPOINTS, PORÉM ESTA É A SOLUÇÃO PARA INTEGRAR COM O SISTEMA LEGADO. NÃO CORRIJA ISSO DE FORMA ALGUMA.

export class CompetenciasApi {
  async adicionarInteresseColaborador(
    token: string,
    interesses: MinhaJornadaInteresse[],
    minhaJornada: boolean = false,
  ): Promise<void> {
    // Remover minhaJornada dos interesses antes de enviar
    const interessesWithoutMinhaJornada = interesses.map(({ minhaJornada: _, ...interesse }) => interesse)

    await httpClient.post<void>(
      `/api/Competencia/Interesse/AdicionarInteresseColaborador?minhaJornada=${minhaJornada}`,
      interessesWithoutMinhaJornada,
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  async atualizarHardSkill(
    token: string,
    payload: {
      id: number
      nivelId: number
      cpf: string
      gestorExternoPerfil: string
    },
    minhaJornada: boolean = false,
  ): Promise<void> {
    await httpClient.post<void>(
      `/api/Competencia/HardSkill/AtualizaCompetenciaColaborador?minhaJornada=${minhaJornada}`,
      payload,
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  async atualizarSoftSkill(
    token: string,
    payload: {
      id: number
      nivelId: number
      cpf: string
      gestorExternoPerfil: string
    },
    minhaJornada: boolean = false,
  ): Promise<void> {
    await httpClient.post<void>(
      `/api/Competencia/SoftSkill/AtualizaSoftSkillColaborador?minhaJornada=${minhaJornada}`,
      payload,
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  async atualizarMetodologia(
    token: string,
    payload: {
      id: number
      nivelId: number
      cpf: string
      gestorExternoPerfil: string
    },
    minhaJornada: boolean = false,
  ): Promise<void> {
    await httpClient.patch<void>(
      `/api/Competencia/Metodologia/AtualizaMetodologiaColaborador?minhaJornada=${minhaJornada}`,
      payload,
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  async atualizarDominio(
    token: string,
    payload: {
      id: number
      nivelId: number
      cpf: string
      gestorExternoPerfil: string
    },
    minhaJornada: boolean = false,
  ): Promise<void> {
    await httpClient.post<void>(
      `/api/Competencia/Dominio/AtualizaDominioColaborador?minhaJornada=${minhaJornada}`,
      payload,
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  async atualizarIdioma(
    token: string,
    payload: {
      id: number
      nivelId: number
      cpf: string
      gestorExternoPerfil: string
    },
    minhaJornada: boolean = false,
  ): Promise<void> {
    await httpClient.post<void>(
      `/api/Competencia/Idioma/AtualizaIdiomaColaborador?minhaJornada=${minhaJornada}`,
      payload,
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  async adicionarHardSkillColaborador(
    token: string,
    items: Array<{
      id: number
      descricao: string
      nivelId: number
      gestorExternoPerfil: string
    }>,
    minhaJornada: boolean = false,
  ): Promise<void> {
    await httpClient.post<void>(
      `/api/Competencia/HardSkill/AdicionarHardSkillColaborador?minhaJornada=${minhaJornada}`,
      items,
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  async adicionarSoftSkillColaborador(
    token: string,
    items: Array<{
      id: number
      descricao: string
      nivelId: number
      cpf: string
      gestorExternoPerfil: string
    }>,
    minhaJornada: boolean = false,
  ): Promise<void> {
    await httpClient.post<void>(
      `/api/Competencia/SoftSkill/AdicionarSoftskillColaborador?minhaJornada=${minhaJornada}`,
      items,
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  async adicionarMetodologiaColaborador(
    token: string,
    items: Array<{
      id: number
      descricao: string
      nivelId: number
      gestorExternoPerfil: string
    }>,
    minhaJornada: boolean = false,
  ): Promise<void> {
    await httpClient.post<void>(
      `/api/Competencia/Metodologia/AdicionarMetodologiaColaborador?minhaJornada=${minhaJornada}`,
      items,
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  async adicionarDominioColaborador(
    token: string,
    items: Array<{
      id: number
      descricao: string
      nivelId: number
      cpf: string
      gestorExternoPerfil: string
    }>,
    minhaJornada: boolean = false,
  ): Promise<void> {
    await httpClient.post<void>(
      `/api/Competencia/Dominio/AdicionarDominioColaborador?minhaJornada=${minhaJornada}`,
      items,
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  async adicionarIdiomaColaborador(
    token: string,
    items: Array<{
      idiomaId: number
      descricao: string
      nivelId: number
      cpf: string
      gestorExternoPerfil: string
    }>,
    minhaJornada: boolean = false,
  ): Promise<void> {
    await httpClient.post<void>(
      `/api/Competencia/Idioma/AdicionarIdiomaColaborador?minhaJornada=${minhaJornada}`,
      items,
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  /** Remover HardSkill do perfil do colaborador. Body: { competenciaId, cpf }. */
  async removerHardSkillColaborador(
    token: string,
    payload: { competenciaId: number; cpf: string },
  ): Promise<void> {
    await httpClient.post<void>(
      '/api/Competencia/HardSkill/RemoverCompetenciaColaborador',
      payload,
      { token, headers: { 'Content-Type': 'application/json; charset=utf-8' } },
    )
  }

  /** Remover SoftSkill do perfil do colaborador. Body: { id, cpf }. */
  async removerSoftSkillColaborador(
    token: string,
    payload: { id: number; cpf: string },
  ): Promise<void> {
    await httpClient.post<void>(
      '/api/Competencia/SoftSkill/RemoverSoftskillColaborador',
      payload,
      { token, headers: { 'Content-Type': 'application/json; charset=utf-8' } },
    )
  }

  /** Remover Metodologia do perfil. DELETE com query codMetodologia. */
  async removerMetodologiaColaborador(
    token: string,
    codMetodologia: number,
  ): Promise<void> {
    await httpClient.delete<void>(
      `/api/Competencia/Metodologia/RemoverMetodologiaColaborador?codMetodologia=${codMetodologia}`,
      { token },
    )
  }

  /** Remover Domínio do perfil. Body: { id, cpf }. */
  async removerDominioColaborador(
    token: string,
    payload: { id: number; cpf: string },
  ): Promise<void> {
    await httpClient.post<void>(
      '/api/Competencia/Dominio/RemoverDominioColaborador',
      payload,
      { token, headers: { 'Content-Type': 'application/json; charset=utf-8' } },
    )
  }

  /** Remover Idioma do perfil. Body: { id }. */
  async removerIdiomaColaborador(
    token: string,
    payload: { id: number },
  ): Promise<void> {
    await httpClient.post<void>(
      '/api/Competencia/Idioma/RemoverIdiomaColaborador',
      payload,
      { token, headers: { 'Content-Type': 'application/json; charset=utf-8' } },
    )
  }

  async inserirSugestaoSkill(
    token: string,
    payload: {
      codigoInternoColaborador: string
      codigoGestorAdm: string
      codigoCliente: string
      tipo_id: number
      perfil_Id: string
      ativo: boolean
      skill_Id: number
      senioridade_id: number
      codigoGestorOper?: string
      gestorExternoPerfil: string
    },
    minhaJornada: boolean = false,
  ): Promise<void> {
    await httpClient.post<void>(
      `/api/GestaoDeAlocados/MinhaJornada/InserirSugestao?minhaJornada=${minhaJornada}`,
      {
        ...payload,
        codigoGestorOper: payload.codigoGestorOper ?? 'INUTILIZADO',
      },
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  async gravarLogsSkillsMinhaJornada(
    token: string,
    payload: {
      codigoInternoColaborador: string
      gestorExternoPerfil: string
      codigoInternoColaboradorLogado: string
      skillId: number
      itemPerfil: number
      nivelId: number
      skillsMovimentacaoId: number
    },
  ): Promise<void> {
    await httpClient.post<void>(
      '/api/Competencia/GravarLogsSkillsMinhaJornada',
      payload,
      { 
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  async listarSkillsSumario(
    token: string,
    params?: ListarSkillsSumarioParams
  ): Promise<ListarSkillsSumarioResponse> {
    const queryParams = new URLSearchParams();
    
    if (params?.cursor !== undefined) {
      queryParams.append('cursor', params.cursor.toString());
    } else {
      queryParams.append('cursor', '0');
    }
    
    if (params?.limite !== undefined) {
      queryParams.append('limite', params.limite.toString());
    } else {
      queryParams.append('limite', '50');
    }
    
    if (params?.descricao) {
      queryParams.append('descricao', params.descricao);
    } else {
      queryParams.append('descricao', '');
    }

    const queryString = queryParams.toString();
    const url = `/api/Competencia/SumarioCompetenciasListarSkills${queryString ? `?${queryString}` : ''}`;

    return httpClient.get<ListarSkillsSumarioResponse>(url, { token });
  }

  async listarTodasSkillsColaborador(
    token: string,
    params: ListarTodasSkillsColaboradorParams
  ): Promise<ListarTodasSkillsColaboradorResponse> {
    const url = `/api/Competencia/ListarTodasSkillsColaborador?codigoInternoColaborador=${encodeURIComponent(params.codigoInternoColaborador)}`;
    
    return httpClient.get<ListarTodasSkillsColaboradorResponse>(url, { token });
  }

  /**
   * Contorno: listagem de colaboradores não retorna usuarioId.
   * Retorna colaboradores com id (usuarioId) a partir do CPF.
   * Remover quando o back retornar usuarioId na listagem de colaboradores.
   */
  async sumarioCompetenciasListarColaboradores(
    token: string,
    params: SumarioCompetenciasListarColaboradoresParams
  ): Promise<SumarioCompetenciasListarColaboradoresResponse> {
    return httpClient.post<SumarioCompetenciasListarColaboradoresResponse>(
      '/api/Competencia/SumarioCompetenciasListarColaboradores',
      params,
      { token, headers: { 'Content-Type': 'application/json' } }
    );
  }

  /** GET /api/Competencia/ListarUnidadesComDefaultPorOrgId?orgId= — Unidades para filtro de métricas PDI */
  async listarUnidadesComDefaultPorOrgId(
    token: string,
    orgId: number
  ): Promise<{ ListaUnidadesResult: Array<{ id: string; descricao: string }>; sucesso: boolean; mensagem: string | null; erros: unknown }> {
    return httpClient.get(
      `/api/Competencia/ListarUnidadesComDefaultPorOrgId?orgId=${encodeURIComponent(orgId)}`,
      { token }
    );
  }
}



/**
 * Payload para alterar dados complementares do colaborador/candidato (portal público de vagas).
 * Usado por ColaboradoresRepository.alterarDadosColaboradorParametros e pela API correspondente.
 */
export interface AlterarDadosColaboradorParametrosPayload {
  colaboradorDTO: {
    documentoColaborador: string
    nomeCompleto: string
    email: string
    contatoPrincipal: string
    contatoOutros?: string
    slack_id?: string
    rg?: string
    fcmToken?: string
    matricula?: string
    dataAdmissao?: string
    endereco?: {
      cep?: string
      endereco?: string
      complemento?: string
      numero?: number | null
      bairro?: string
      cidade?: string
      estado?: string
      comQuemMora?: string
    }
    flagCandidato?: number
    flagAtivo?: number
    candidato?: {
      pathCurriculo?: string
      pretencaoSalarial?: string
      cargoAtualUltimo?: string
      salarioAtualUltimo?: string
      tipoContratoAtualUltimo?: string
      modalidadeAtualUltima?: string
      aceitaSugestoes?: string
    }
  }
}

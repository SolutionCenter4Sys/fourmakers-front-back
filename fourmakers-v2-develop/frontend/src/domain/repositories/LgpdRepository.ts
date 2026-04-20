export interface LgpdRegistroPayload {
  cpfPessoa: string
  nomePessoa: string
  isColab?: boolean
  unidade?: string
  contrato?: string
}

export interface LgpdRepository {
  registrar(payload: LgpdRegistroPayload): Promise<void>
}

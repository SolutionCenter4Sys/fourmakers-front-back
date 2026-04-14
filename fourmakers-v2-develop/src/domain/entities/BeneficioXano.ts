/** Miniatura ou anexo da API XANO (imagem/arquivo) */
export interface BeneficioXanoMedia {
  access: string
  path: string
  name: string
  type: string
  size: number
  mime: string
  meta?: { width?: number; height?: number; validated?: boolean }
  url: string
}

export interface BeneficioXanoUrlDestino {
  url1: string
  url2: string
  titulo: string
  descricao: string
  label_botao1: string
  label_botao2: string
}

export interface BeneficioXanoPergunta {
  pergunta: string
  resposta: string
  link: string
  anexo: BeneficioXanoMedia | null
}

/** Item da lista GET .../beneficio_fourmakers?orgId=&user_id= */
export interface BeneficioXanoListItem {
  id: number
  created_at: number
  nome: string
  descricao: Array<{ pergunta: string; resposta: string }>
  url_destino: BeneficioXanoUrlDestino
  tipo: string
  org_id: number
  segmento: string
  miniatura: BeneficioXanoMedia | null
  perguntas: BeneficioXanoPergunta[]
  qrcode1: BeneficioXanoMedia | null
  qrcode2: BeneficioXanoMedia | null
  media: number
  jaAvaliou?: boolean
}

/** Detalhe GET .../beneficio_fourmakers/:id (sem jaAvaliou) */
export interface BeneficioXanoDetail extends Omit<BeneficioXanoListItem, 'jaAvaliou'> {}

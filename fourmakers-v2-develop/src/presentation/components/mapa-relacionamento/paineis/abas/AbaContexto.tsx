import { Card } from '@/components/ui/card'

import type { DadosVcx360 } from '@domain/entities/Vcx360'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import { DoresList } from './DoresList'
import { IniciativasList } from './IniciativasList'

interface AbaContextoProps {
  contexto: DadosVcx360['contexto']
  no?: NoMapaRelacionamento
}

export function AbaContexto({ contexto, no }: AbaContextoProps) {
  return (
    <div className="space-y-10">
      {/* Componente de Dores com CRUD completo */}
      {no ? (
        <DoresList no={no} />
      ) : (
        <Card className="p-4">
          <p className="mb-2 text-sm font-semibold">Dores e Oportunidades</p>
          {contexto.doresOportunidades.length === 0 ? (
            <p className="text-xs text-muted-foreground">Nenhuma dor cadastrada</p>
          ) : (
            contexto.doresOportunidades.map((item) => (
              <div key={item.id} className="border-b border-dashed border-border py-2">
                <p className="text-sm font-semibold">{item.titulo}</p>
                <p className="text-xs text-muted-foreground">{item.descricao}</p>
                <span className="text-xxs">{item.prioridade}</span>
              </div>
            ))
          )}
        </Card>
      )}

      {/* Componente de Iniciativas com CRUD completo */}
      {no && <IniciativasList no={no} />}
    </div>
  )
}

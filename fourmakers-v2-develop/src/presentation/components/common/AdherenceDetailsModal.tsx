import type { MinhaJornadaAdherence } from '@domain/entities/MinhaJornadaAdherence'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Card, CardContent } from '@/components/ui/card'
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from '@/components/ui/accordion'
import { Target } from 'lucide-react'
import { cn } from '@/lib/utils'

interface AdherenceDetailsModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  adherence: MinhaJornadaAdherence | null
}

const categoryLabels: Record<string, { label: string; key: keyof MinhaJornadaAdherence['comparativoPorSkill'] }> = {
  hardSkills: { label: 'Score hardskills', key: 'hardSkills' },
  softSkills: { label: 'Score softskills', key: 'softSkills' },
  metodologias: { label: 'Score metodologias', key: 'metodologias' },
  dominiosNegocio: { label: 'Score domínios de negócio', key: 'dominiosNegocio' },
  idiomas: { label: 'Score idiomas', key: 'idiomas' },
  disponibilidades: { label: 'Score disponibilidade', key: 'disponibilidades' },
}

export const AdherenceDetailsModal = ({
  open,
  onOpenChange,
  adherence,
}: AdherenceDetailsModalProps) => {
  if (!adherence) return null

  const matchPercentage = adherence.match || 0
  const scoreVaga = adherence.scoreVaga || 0
  const scoreCandidato = adherence.scoreCandidato || 0

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Target className="w-5 h-5 text-primary" />
            <span>Cálculo de match - {adherence.nome}</span>
          </DialogTitle>
          <DialogDescription>
            Confira a baixo os detalhes do match de aderência.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6 py-4">
          {/* Summary Cards */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <Card className="bg-muted/50">
              <CardContent className="p-4">
                <div className="text-sm text-muted-foreground mb-1">
                  Total score vaga
                </div>
                <div className="text-2xl font-bold text-primaryText">
                  {scoreVaga.toFixed(2)}
                </div>
              </CardContent>
            </Card>
            <Card className="bg-muted/50">
              <CardContent className="p-4">
                <div className="text-sm text-muted-foreground mb-1">
                  Total score candidato
                </div>
                <div className="text-2xl font-bold text-primaryText">
                  {scoreCandidato.toFixed(2)}
                </div>
              </CardContent>
            </Card>
            <Card className="bg-yellow-100 dark:bg-yellow-900/20 border-yellow-300 dark:border-yellow-700">
              <CardContent className="p-4">
                <div className="text-sm text-muted-foreground mb-1">
                  Match candidato
                </div>
                <div className="text-2xl font-bold text-primaryText">
                  {matchPercentage.toFixed(2)}%
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Detailed Score Breakdown */}
          <div className="space-y-2">
            <Accordion type="multiple" className="w-full">
              {Object.entries(categoryLabels).map(([key, { label, key: comparativoKey }]) => {
                const detalhamento = adherence.detalhamentoCalculo?.[key as keyof typeof adherence.detalhamentoCalculo]
                const score = detalhamento?.scoreBrutoCategoria || 0
                const comparativo = adherence.comparativoPorSkill?.[comparativoKey] || []

                return (
                  <AccordionItem key={key} value={key} className="border rounded-lg px-4 mb-2">
                    <AccordionTrigger className="hover:no-underline">
                      <div className="flex items-center justify-between w-full pr-4">
                        <span className="font-medium text-primaryText">{label}</span>
                        <span className="text-primaryText font-semibold">{score.toFixed(2)}</span>
                      </div>
                    </AccordionTrigger>
                    <AccordionContent>
                      <div className="space-y-3 pt-2">
                        {comparativo.length > 0 ? (
                          <div className="space-y-2">
                            {comparativo.map((skill, index) => (
                              <Card
                                key={index}
                                className={cn(
                                  'border',
                                  skill.pontuacaoDaSkill > 0
                                    ? 'bg-green-50/50 dark:bg-green-950/20 border-green-200 dark:border-green-800'
                                    : 'bg-muted/30'
                                )}
                              >
                                <CardContent className="p-3">
                                  <div className="grid grid-cols-1 md:grid-cols-2 gap-3 text-sm">
                                    <div>
                                      <div className="text-xs text-muted-foreground mb-1">
                                        Skill Requisitada
                                      </div>
                                      <div className="font-medium text-primaryText">
                                        {skill.skillRequisitada}
                                      </div>
                                      <div className="text-xs text-muted-foreground mt-1">
                                        Nível: {skill.nivelRequerido} |{' '}
                                        {skill.obrigatoriedade === 'obrigatorio'
                                          ? 'Obrigatório'
                                          : 'Desejável'}
                                      </div>
                                    </div>
                                    <div>
                                      <div className="text-xs text-muted-foreground mb-1">
                                        Skill do Candidato
                                      </div>
                                      <div className="font-medium text-primaryText">
                                        {skill.skillDoCandidato}
                                      </div>
                                      <div className="text-xs text-muted-foreground mt-1">
                                        Nível: {skill.nivelDoCandidato}
                                      </div>
                                    </div>
                                  </div>
                                  <div className="mt-2 pt-2 border-t border-borderDefault">
                                    <div className="flex items-center justify-between">
                                      <span className="text-xs text-muted-foreground">
                                        Pontuação da skill:
                                      </span>
                                      <span
                                        className={cn(
                                          'font-semibold',
                                          skill.pontuacaoDaSkill > 0
                                            ? 'text-green-700 dark:text-green-400'
                                            : 'text-muted-foreground'
                                        )}
                                      >
                                        {skill.pontuacaoDaSkill.toFixed(2)}
                                      </span>
                                    </div>
                                  </div>
                                </CardContent>
                              </Card>
                            ))}
                          </div>
                        ) : (
                          <div className="text-sm text-muted-foreground py-2">
                            Nenhuma skill nesta categoria.
                          </div>
                        )}
                      </div>
                    </AccordionContent>
                  </AccordionItem>
                )
              })}
            </Accordion>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  )
}

import * as React from 'react'
import type { SkillGroup } from '@presentation/hooks/useMinhaJornadaViewModel'
import type { MinhaJornadaSkill } from '@domain/entities/MinhaJornadaSkill'
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from '@/components/ui/accordion'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Progress } from '@/components/ui/progress'
import { SkillCard } from './SkillCard'
import { Code, Heart, GitBranch, Briefcase, Globe, Plus, Minus } from 'lucide-react'
import { cn } from '@/lib/utils'


interface SkillGroupCardProps {
  group: SkillGroup
  profileId: string | null
  onMarkAlreadyHave: (skill: MinhaJornadaSkill, nivelId: number) => Promise<void>
  onMarkNoInterest: (skill: MinhaJornadaSkill) => Promise<void>
  onRevertInterest?: (skill: MinhaJornadaSkill) => Promise<void>
  onMarkWantToDevelop: (skill: MinhaJornadaSkill) => void
  onSuggestSkill?: () => void
  isSaving?: boolean
}

const TYPE_ICONS = {
  hard: Code,
  soft: Heart,
  methodology: GitBranch,
  domain: Briefcase,
  language: Globe,
}

const TYPE_LABELS = {
  hard: 'Hard Skills',
  soft: 'Soft Skills',
  methodology: 'Metodologias',
  domain: 'Domínios de Negócio',
  language: 'Idiomas',
}

export const SkillGroupCard = ({
  group,
  onMarkAlreadyHave,
  onMarkNoInterest,
  onRevertInterest,
  onMarkWantToDevelop,
  onSuggestSkill,
  isSaving = false,
}: SkillGroupCardProps) => {
  const Icon = TYPE_ICONS[group.type]
  const label = TYPE_LABELS[group.type]
  const progress = group.totalCount > 0 
    ? (group.metCount / group.totalCount) * 100 
    : 0
  const isEmpty = group.totalCount === 0
  const isCompleted = group.totalCount > 0 && group.metCount === group.totalCount

  // Drag to scroll functionality
  const scrollContainerRef = React.useRef<HTMLDivElement>(null)
  const [isDragging, setIsDragging] = React.useState(false)
  const [startX, setStartX] = React.useState(0)
  const [scrollLeft, setScrollLeft] = React.useState(0)

  const handleMouseDown = (e: React.MouseEvent<HTMLDivElement>) => {
    if (!scrollContainerRef.current) return
    setIsDragging(true)
    setStartX(e.pageX - scrollContainerRef.current.offsetLeft)
    setScrollLeft(scrollContainerRef.current.scrollLeft)
  }

  const handleMouseMove = (e: React.MouseEvent<HTMLDivElement>) => {
    if (!isDragging || !scrollContainerRef.current) return
    e.preventDefault()
    const x = e.pageX - scrollContainerRef.current.offsetLeft
    const walk = (x - startX) * 2 // Scroll speed multiplier
    scrollContainerRef.current.scrollLeft = scrollLeft - walk
  }

  const handleMouseUp = () => {
    setIsDragging(false)
  }

  const handleMouseLeave = () => {
    setIsDragging(false)
  }

  return (
    <Card className="border-borderSoft bg-surfaceElevated shadow-softToken rounded-lg">
      <Accordion type="single" collapsible className="w-full">
        <AccordionItem value={group.type} className="border-none">
          <CardHeader className="pb-3">
            <div className="flex items-center justify-between mb-3">
              <AccordionTrigger className="hover:no-underline py-0">
                <div className="flex items-center gap-3">
                  <Icon className="w-5 h-5 text-primary" />
                  <div className="text-left">
                    <CardTitle className="text-lg font-semibold text-primaryText">
                      {label}
                    </CardTitle>
                    {isEmpty && (
                      <span className="text-sm text-muted-foreground flex items-center gap-1 mt-1">
                        <Minus className="w-3 h-3" />
                        Sem requisitos no perfil
                      </span>
                    )}
                  </div>
                </div>
              </AccordionTrigger>
              {onSuggestSkill && (
                <Button
                  variant="outline"
                  size="sm"
                  className="rounded-pill"
                  onClick={onSuggestSkill}
                >
                  <Plus className="w-4 h-4 mr-1" />
                  Sugerir Nova
                </Button>
              )}
            </div>
            {!isEmpty && (
              <div className="space-y-2">
                <div className="flex items-center justify-between text-sm">
                  <span className="text-muted-foreground">
                    {group.metCount} de {group.totalCount} atingidas
                  </span>
                  <span className="font-medium text-primaryText">
                    {Math.round(progress)}%
                  </span>
                </div>
                <Progress value={progress} className="w-full h-3 [&>div]:bg-accent" />
              </div>
            )}
          </CardHeader>
          <AccordionContent>
            <CardContent className="pt-0">
              {isEmpty ? (
                <div className="text-sm text-muted-foreground text-center py-8">
                  Nenhuma skill desta categoria foi definida no perfil de atuação.
                  <br />
                  <span className="text-xs">Use o botão "Sugerir Nova" para adicionar.</span>
                </div>
              ) : (
                <>
                  {isCompleted && (
                    <div className="mb-4">
                      <Card className={cn(
                        'border-success/30 rounded-lg bg-green-50/50 dark:bg-green-950/20'
                      )}>
                        <CardContent className="p-4">
                          <div className="flex items-center gap-2 text-sm text-green-700 dark:text-green-400">
                            <svg className="w-5 h-5 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            <span className="font-medium">Todas as skills desta categoria foram atingidas! 🎉</span>
                          </div>
                        </CardContent>
                      </Card>
                    </div>
                  )}
                  <div
                    ref={scrollContainerRef}
                    className={`horizontal-scroll-container overflow-x-auto pb-2 -mx-1 px-1 scroll-smooth ${isDragging ? 'cursor-grabbing select-none' : 'cursor-grab'}`}
                    style={isDragging ? { userSelect: 'none' } : {}}
                    onMouseDown={handleMouseDown}
                    onMouseMove={handleMouseMove}
                    onMouseUp={handleMouseUp}
                    onMouseLeave={handleMouseLeave}
                  >
                    <div className="flex gap-4 min-w-max">
                      {group.skills.map((skill) => (
                        <div key={skill.skillId} className="flex-shrink-0 w-80">
                          <SkillCard
                            skill={skill}
                            onMarkAlreadyHave={onMarkAlreadyHave}
                            onMarkNoInterest={onMarkNoInterest}
                            onRevertInterest={onRevertInterest}
                            onMarkWantToDevelop={onMarkWantToDevelop}
                            isPending={isSaving}
                          />
                        </div>
                      ))}
                    </div>
                  </div>
                </>
              )}
            </CardContent>
          </AccordionContent>
        </AccordionItem>
      </Accordion>
    </Card>
  )
}


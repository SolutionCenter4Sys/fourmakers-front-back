import { useState } from 'react'
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Calendar, User, CheckSquare, AlertCircle, MessageCircle } from 'lucide-react'
import { Spinner } from '@/components/ui/spinner'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import { LABEL_DATA_NAO_INFORMADA } from '@shared/utils/agendaUtils'
import { parseDataAgendaParaExibicao } from '@shared/utils/timezoneAgendaUtils'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Textarea } from '@/components/ui/textarea'
import { Label } from '@/components/ui/label'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { atualizarStatusAcoes, inserirComentarioAcao } from '@app/store/slices/agendasComerciaisSlice'
import { toast } from 'sonner'
import { logUserAction } from '@shared/utils/firebaseAnalytics'

import type { ItemAgendaGestor } from '@domain/entities/AgendaGestor'
import { DetalhesAgendaModal } from './DetalhesAgendaModal'

interface AcaoCardProps {
  acao: ItemAgendaGestor
}

/**
 * Componente de card para exibir informações de uma ação comercial.
 * Possui borda colorida à esquerda baseada no status (verde=concluída,
 * laranja=em andamento, vermelho=atrasada, cinza=pendente).
 */

function getStatusStyles(status: string) {
  const isAtrasada = status === 'Atrasada'
  const isConcluida = status === 'Concluída'
  const isEmAndamento = status === 'Em andamento'

  const borderColor = isAtrasada
    ? 'border-l-destructive'
    : isConcluida
      ? 'border-l-success'
      : isEmAndamento
        ? 'border-l-warning'
        : 'border-l-border'

  const badgeClass = isAtrasada
    ? 'bg-destructive/10 text-destructive border-destructive/20'
    : isConcluida
      ? 'bg-success/10 text-success border-success/20'
      : isEmAndamento
        ? 'bg-warning/10 text-warning border-warning/20'
        : 'bg-muted text-muted-foreground border-border'

  const iconColor = isConcluida ? 'text-success' : 'text-warning'

  return { borderColor, badgeClass, iconColor, isConcluida }
}

const STATUS_OPTIONS = [
  { value: 'Pendente', label: 'Pendente' },
  { value: 'Em andamento', label: 'Em andamento' },
  { value: 'Concluída', label: 'Concluída' },
  { value: 'Atrasada', label: 'Atrasada' },
]

export function AcaoCard({ acao }: AcaoCardProps) {
  const dispatch = useAppDispatch()
  const { token, user } = useAppSelector((state) => state.auth)
  
  const [modalAberto, setModalAberto] = useState(false)
  const [mostrarComentario, setMostrarComentario] = useState(false)
  const [comentario, setComentario] = useState('')
  const [isUpdatingStatus, setIsUpdatingStatus] = useState(false)
  const [isAddingComment, setIsAddingComment] = useState(false)
  
  const { borderColor, badgeClass, iconColor, isConcluida } = getStatusStyles(acao.status)
  const dataParsed = parseDataAgendaParaExibicao(acao.data)
  const dataFormatada = dataParsed
    ? format(dataParsed, 'dd/MM/yyyy', { locale: ptBR })
    : LABEL_DATA_NAO_INFORMADA

  const handleStatusChange = async (novoStatus: string) => {
    if (!token || !acao.id) return

    // Mapear status string para número: 1=Em andamento, 2=Pendente, 3=Concluída
    const statusMap: Record<string, number> = {
      'Em andamento': 1,
      'Pendente': 2,
      'Concluída': 3,
      'Atrasada': 2, // Atrasada mapeia para Pendente
    }
    const statusAcoes = statusMap[novoStatus] || 2

    setIsUpdatingStatus(true)
    try {
      await dispatch(
        atualizarStatusAcoes({
          token,
          payload: {
            id: parseInt(acao.id),
            statusAcoes,
          },
        }),
      ).unwrap()
      
      // Rastrear ação do usuário
      if (user) {
        logUserAction(
          'AgendasComerciais',
          'AtualizarStatusAcao',
          {
            acaoId: acao.id,
            statusAnterior: acao.status,
            statusNovo: novoStatus,
            statusAcoes,
          },
          user
        )
      }
      
      toast.success('Status atualizado', { description: `Status alterado para: ${novoStatus}` })
    } catch (error) {
      toast.error('Erro', {
        description: error instanceof Error ? error.message : 'Erro ao atualizar status',
      })
    } finally {
      setIsUpdatingStatus(false)
    }
  }

  const handleAdicionarComentario = async () => {
    if (!token || !acao.id || !comentario.trim()) {
      toast.error('Validação', { description: 'Digite um comentário' })
      return
    }

    setIsAddingComment(true)
    try {
      await dispatch(
        inserirComentarioAcao({
          token,
          payload: {
            acaoId: parseInt(acao.id),
            comentarioAcao: comentario.trim(),
          },
        }),
      ).unwrap()
      
      // Rastrear ação do usuário
      if (user) {
        logUserAction(
          'AgendasComerciais',
          'AdicionarComentarioAcao',
          {
            acaoId: acao.id,
            comentarioTamanho: comentario.trim().length,
          },
          user
        )
      }
      
      toast.success('Comentário adicionado', { description: 'O comentário foi salvo com sucesso' })
      setComentario('')
      setMostrarComentario(false)
    } catch (error) {
      toast.error('Erro', {
        description: error instanceof Error ? error.message : 'Erro ao adicionar comentário',
      })
    } finally {
      setIsAddingComment(false)
    }
  }

  return (
    <>
      <Card className={`hover:shadow-lg transition-shadow border-l-4 ${borderColor} rounded-lg w-full flex flex-col`}>
        <CardHeader className="pb-3">
          <div className="flex items-start justify-between gap-2">
            <div className="flex-1 min-w-0">
              <div className="flex items-center gap-2 mb-1">
                {isConcluida ? (
                  <CheckSquare className={`h-4 w-4 ${iconColor} shrink-0`} />
                ) : (
                  <AlertCircle className={`h-4 w-4 ${iconColor} shrink-0`} />
                )}
                <Badge variant="secondary" className="text-xs">
                  Ação
                </Badge>
              </div>
              <CardTitle className="text-base font-semibold line-clamp-2">{acao.titulo}</CardTitle>
            </div>
            <Badge className={`${badgeClass} text-xs shrink-0`}>{acao.status}</Badge>
          </div>
        </CardHeader>
        <CardContent className="space-y-3 flex-1">
          <div className="flex items-center gap-2 text-sm text-muted-foreground">
            <Calendar className="h-4 w-4 shrink-0" />
            <span>Prazo: {dataFormatada}</span>
          </div>

          <div className="flex items-center gap-2 text-sm text-muted-foreground">
            <User className="h-4 w-4 shrink-0" />
            <span className="truncate">{acao.responsavel}</span>
          </div>

          {acao.descricao && (
            <p className="text-sm text-muted-foreground line-clamp-2">{acao.descricao}</p>
          )}

          {/* Status Update Dropdown */}
          <div className="space-y-2 pt-2">
            <Label htmlFor={`status-${acao.id}`} className="text-xs text-muted-foreground">
              Atualizar Status
            </Label>
            <Select
              value={acao.status}
              onValueChange={handleStatusChange}
              disabled={isUpdatingStatus}
            >
              <SelectTrigger id={`status-${acao.id}`} className="h-8 text-xs">
                <SelectValue placeholder="Selecione o status" />
              </SelectTrigger>
              <SelectContent>
                {STATUS_OPTIONS.map((option) => (
                  <SelectItem key={option.value} value={option.value} className="text-xs">
                    {option.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {/* Comment Section */}
          {mostrarComentario && (
            <div className="space-y-2 pt-2">
              <Label htmlFor={`comment-${acao.id}`} className="text-xs text-muted-foreground">
                Adicionar Comentário
              </Label>
              <Textarea
                id={`comment-${acao.id}`}
                placeholder="Digite seu comentário..."
                value={comentario}
                onChange={(e) => setComentario(e.target.value)}
                rows={3}
                className="text-sm"
              />
              <div className="flex gap-2">
                <Button
                  size="sm"
                  onClick={handleAdicionarComentario}
                  disabled={isAddingComment || !comentario.trim()}
                  className="text-xs"
                >
                  {isAddingComment && <Spinner size={12} className="mr-2 text-primary" />}
                  Salvar
                </Button>
                <Button
                  size="sm"
                  variant="outline"
                  onClick={() => {
                    setMostrarComentario(false)
                    setComentario('')
                  }}
                  className="text-xs"
                >
                  Cancelar
                </Button>
              </div>
            </div>
          )}
        </CardContent>
        <CardFooter className="pt-3 mt-auto flex gap-2">
          <Button 
            variant="ghost" 
            size="sm" 
            className="flex-1" 
            onClick={() => setModalAberto(true)}
          >
            Ver Mais
          </Button>
          {!mostrarComentario && (
            <Button
              variant="ghost"
              size="sm"
              onClick={() => setMostrarComentario(true)}
              className="h-9 w-9 p-0"
              title="Adicionar Comentário"
            >
              <MessageCircle className="h-4 w-4" />
            </Button>
          )}
        </CardFooter>
      </Card>
      <DetalhesAgendaModal
        open={modalAberto}
        onOpenChange={setModalAberto}
        item={acao}
        tipo="acao"
      />
    </>
  )
}

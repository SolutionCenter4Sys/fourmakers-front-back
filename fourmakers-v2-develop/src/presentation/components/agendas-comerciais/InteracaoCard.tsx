import { useState } from 'react'
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Calendar, User, MessageSquare, Edit, Trash2, Sparkles, Paperclip } from 'lucide-react'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import { LABEL_DATA_NAO_INFORMADA } from '@shared/utils/agendaUtils'
import { parseDataHoraAgendaParaExibicao } from '@shared/utils/timezoneAgendaUtils'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog'
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@/components/ui/tooltip'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { deletarInteracaoIa } from '@app/store/slices/agendasComerciaisSlice'
import { toast } from 'sonner'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import { podeEditarExcluirInteracao } from '@shared/utils/agendaPermissions'

import type { ItemAgendaGestor } from '@domain/entities/AgendaGestor'
import { DetalhesAgendaModal } from './DetalhesAgendaModal'
import { NovaInteracaoModal } from './NovaInteracaoModal'
import { EDITAR_EXCLUIR_INTERACAO_E_ARQUIVOS_HABILITADO } from './agendasComerciaisConstants'

interface InteracaoCardProps {
  interacao: ItemAgendaGestor
  /** Lista de agendas; se informada, criador da agenda também pode editar/excluir a interação */
  agendas?: ItemAgendaGestor[]
  /** Chamado após editar ou excluir com sucesso; use para recarregar lista/detalhe. */
  onEditSuccess?: () => void
  onDeleteSuccess?: () => void
  /** Quando true, card mais compacto (ex.: dentro do modal de detalhes da agenda). */
  compact?: boolean
  /** Abre modal de editar interação no nível da página (evita dialog aninhado). Quando informado, o botão Editar usa este callback em vez do modal local. */
  onAbrirEditarInteracao?: (interacao: ItemAgendaGestor) => void
}

/**
 * Componente de card para exibir informações de uma interação registrada.
 * Possui borda azul à esquerda e exibe título, status, data, responsável e descrição.
 */

export function InteracaoCard({
  interacao,
  agendas,
  onEditSuccess: onEditSuccessProp,
  onDeleteSuccess: onDeleteSuccessProp,
  compact = false,
  onAbrirEditarInteracao,
}: InteracaoCardProps) {
  const dispatch = useAppDispatch()
  const { token, user } = useAppSelector((state) => state.auth)
  const podeEditarExcluir = podeEditarExcluirInteracao(interacao, user, agendas)

  const [modalAberto, setModalAberto] = useState(false)
  const [modalEditarAberto, setModalEditarAberto] = useState(false)
  const [modalDeletarAberto, setModalDeletarAberto] = useState(false)
  const [isDeleting, setIsDeleting] = useState(false)
  
  // Data de Requisição: normalizada ao timezone do usuário (ex.: 25/03/2026 00:00)
  const dataRequisicaoRaw = interacao.dataRequisicao ?? interacao.data
  const dataRequisicaoParsed = parseDataHoraAgendaParaExibicao(dataRequisicaoRaw)
  const dataRequisicaoFormatada = dataRequisicaoParsed
    ? format(dataRequisicaoParsed, 'dd/MM/yyyy HH:mm', { locale: ptBR })
    : LABEL_DATA_NAO_INFORMADA
  
  // Verificar se há próximos passos
  const temProximosPassos = interacao.encontroAi?.passos && interacao.encontroAi.passos.length > 0
  const numeroPassos = interacao.encontroAi?.passos?.length || 0
  const numeroArquivos = interacao.arquivos?.length ?? 0
  const temAnexos = numeroArquivos > 0

  const handleDeletar = async () => {
    if (!token || !interacao.id) return

    setIsDeleting(true)
    try {
      await dispatch(deletarInteracaoIa({ token, interacaoId: parseInt(interacao.id) })).unwrap()
      
      // Rastrear ação do usuário
      if (user) {
        logUserAction(
          'AgendasComerciais',
          'DeletarInteracao',
          {
            interacaoId: interacao.id,
            titulo: interacao.titulo,
            agendaId: interacao.agendaId,
          },
          user
        )
      }
      
      toast.success('Interação deletada', { description: 'A interação foi deletada com sucesso' })
      setModalDeletarAberto(false)
      onDeleteSuccessProp?.()
    } catch (error) {
      toast.error('Erro', {
        description: error instanceof Error ? error.message : 'Erro ao deletar interação',
      })
    } finally {
      setIsDeleting(false)
    }
  }

  const handleEditSuccess = () => {
    setModalEditarAberto(false)
    onEditSuccessProp?.()
  }

  return (
    <>
      <Card
        className={
          compact
            ? 'hover:shadow-md transition-shadow border-l-4 border-l-info rounded-lg w-full flex flex-col py-2 px-3'
            : 'hover:shadow-lg transition-shadow border-l-4 border-l-info rounded-lg w-full flex flex-col'
        }
      >
        <CardHeader className={compact ? 'pb-1 pt-2 px-0' : 'pb-3'}>
          <div className="flex items-start justify-between gap-2">
            <div className="flex-1 min-w-0">
              <div className="flex items-center gap-2 mb-1 flex-wrap">
                <MessageSquare className={(compact ? 'h-3.5 w-3.5' : 'h-4 w-4') + ' text-info shrink-0'} />
                <Badge variant="secondary" className="text-xs">
                  Interação
                </Badge>
                {temProximosPassos && (
                  <Badge 
                    variant="outline" 
                    className="text-xs bg-gradient-to-r from-purple-50 to-blue-50 dark:from-purple-950/30 dark:to-blue-950/30 border-purple-300 dark:border-purple-700 text-purple-700 dark:text-purple-300"
                  >
                    <Sparkles className="h-3 w-3 mr-1" />
                    {numeroPassos} {numeroPassos === 1 ? 'Passo' : 'Passos'}
                  </Badge>
                )}
                {temAnexos && (
                  <Badge variant="outline" className="text-xs">
                    <Paperclip className="h-3 w-3 mr-1" />
                    {numeroArquivos} {numeroArquivos === 1 ? 'anexo' : 'anexos'}
                  </Badge>
                )}
              </div>
              <CardTitle className={compact ? 'text-sm font-semibold line-clamp-2' : 'text-base font-semibold line-clamp-2'}>{interacao.titulo}</CardTitle>
            </div>
          </div>
        </CardHeader>
        <CardContent className={compact ? 'space-y-1 flex-1 py-0 px-0' : 'space-y-2 flex-1'}>
          <div className="flex items-center gap-2 text-sm text-muted-foreground">
            <Calendar className="h-4 w-4 shrink-0" />
            <span title="Data de Requisição (normalizada ao timezone)">
              <span className="font-medium">Data de Requisição:</span> {dataRequisicaoFormatada}
            </span>
          </div>

          <div className="flex items-center gap-2 text-sm text-muted-foreground">
            <User className="h-4 w-4 shrink-0" />
            <span className="truncate">{interacao.responsavel}</span>
          </div>

          {interacao.descricao && (
            <p className="text-sm text-muted-foreground line-clamp-3 mt-2">{interacao.descricao}</p>
          )}
        </CardContent>
        <CardFooter className={compact ? 'pt-2 mt-auto flex gap-2 pb-0 px-0' : 'pt-3 mt-auto flex gap-2'}>
          <Button
            variant="ghost"
            size="sm"
            className="flex-1"
            onClick={() => setModalAberto(true)}
          >
            Ver Mais
          </Button>
          {podeEditarExcluir && (
            <TooltipProvider>
              <>
                {EDITAR_EXCLUIR_INTERACAO_E_ARQUIVOS_HABILITADO ? (
                  <>
                    <Button
                      variant="ghost"
                      size="sm"
                      className="h-9 w-9 p-0"
                      onClick={() => {
                        if (onAbrirEditarInteracao) {
                          onAbrirEditarInteracao(interacao)
                        } else {
                          setModalEditarAberto(true)
                        }
                      }}
                      aria-label="Editar interação"
                    >
                      <Edit className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      className="h-9 w-9 p-0 text-destructive hover:text-destructive"
                      onClick={() => setModalDeletarAberto(true)}
                      aria-label="Excluir interação"
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </>
                ) : (
                  <>
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <span className="inline-block">
                          <Button variant="ghost" size="sm" disabled className="h-9 w-9 p-0 opacity-50 cursor-not-allowed">
                            <Edit className="h-4 w-4" />
                          </Button>
                        </span>
                      </TooltipTrigger>
                      <TooltipContent>
                        <p>Em Breve</p>
                      </TooltipContent>
                    </Tooltip>
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <span className="inline-block">
                          <Button variant="ghost" size="sm" disabled className="h-9 w-9 p-0 text-destructive opacity-50 cursor-not-allowed">
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </span>
                      </TooltipTrigger>
                      <TooltipContent>
                        <p>Em Breve</p>
                      </TooltipContent>
                    </Tooltip>
                  </>
                )}
              </>
            </TooltipProvider>
          )}
        </CardFooter>
      </Card>
      
      <DetalhesAgendaModal
        open={modalAberto}
        onOpenChange={setModalAberto}
        item={interacao}
        tipo="interacao"
        onAbrirEditarInteracao={onAbrirEditarInteracao}
        onInteracaoEditOrDelete={onEditSuccessProp}
        agendasParaPermissao={agendas}
      />
      
      <NovaInteracaoModal
        open={modalEditarAberto}
        onOpenChange={setModalEditarAberto}
        interacaoParaEditar={interacao}
        agendaId={interacao.agendaId || 0}
        onSuccess={handleEditSuccess}
      />
      
      <AlertDialog open={modalDeletarAberto} onOpenChange={setModalDeletarAberto}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirmar exclusão</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza que deseja deletar a interação &quot;{interacao.titulo}&quot;? Esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isDeleting}>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDeletar}
              disabled={isDeleting}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {isDeleting ? 'Deletando...' : 'Deletar'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  )
}

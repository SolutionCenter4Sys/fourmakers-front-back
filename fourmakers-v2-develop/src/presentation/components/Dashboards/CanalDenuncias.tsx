import { useState, useMemo, useRef } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Textarea } from '@/components/ui/textarea'
import { Label } from '@/components/ui/label'
import { useToast } from '@/hooks/use-toast'
import { useParametros } from '@presentation/hooks/useParametros'
import { AlertCircle, CheckCircle } from '@/components/ui/system-icons'
import { container } from '@core/di/container'
import { EnviarDenunciaUseCase } from '@domain/usecases/EnviarDenunciaUseCase'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import type { PreviewDevice } from '@presentation/components/home-renderer/HomeRenderer'

interface CanalDenunciaConfig {
  title: string
  subTitle: string
  body: string
  emailDestinatario: string
  codigoUnidade: string
}

const gridClassSingle = 'grid grid-cols-1 gap-4 sm:gap-6 min-w-0'
const gridClassResponsive = 'grid grid-cols-1 xl:grid-cols-2 gap-4 sm:gap-6 xl:gap-8 min-w-0'

export const CanalDenuncias = ({ previewDevice }: { previewDevice?: PreviewDevice } = {}) => {
  const { toast } = useToast()
  const { user, token } = useAppSelector((state) => state.auth)
  const { getParametro } = useParametros()
  const [manifestacao, setManifestacao] = useState<string>('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [modalSucessoOpen, setModalSucessoOpen] = useState(false)

  // Use case - usando useRef para manter referência estável
  const useCaseRef = useRef<EnviarDenunciaUseCase | null>(null)
  if (!useCaseRef.current) {
    useCaseRef.current = container.resolve(EnviarDenunciaUseCase)
  }
  const enviarDenunciaUseCase = useCaseRef.current

  // Busca e processa a configuração do parâmetro
  const config = useMemo<CanalDenunciaConfig | null>(() => {
    const configParam = getParametro('CANAL_DENUNCIA_CONFIGURACOES')
    if (!configParam) return null

    try {
      // Função para limpar caracteres de controle do JSON de forma robusta
      // Remove todos os caracteres de controle problemáticos que podem causar erro no parse
      const cleanJsonString = (str: string): string => {
        // Primeiro, normaliza quebras de linha: \r\n -> \n, \r -> \n
        let cleaned = str.replace(/\r\n/g, '\n').replace(/\r/g, '\n')
        
        // Remove todos os caracteres de controle Unicode (0x00-0x1F) exceto \n (0x0A) e \t (0x09)
        // Isso inclui: \x00 (NUL), \x01-\x08, \x0B (VT), \x0C (FF), \x0E-\x1F
        cleaned = cleaned.replace(/[\x00-\x08\x0B-\x0C\x0E-\x1F]/g, '')
        
        // Remove também caracteres de controle Unicode adicionais (0x7F-0x9F)
        cleaned = cleaned.replace(/[\x7F-\x9F]/g, '')
        
        return cleaned
      }
      
      const cleanedParam = cleanJsonString(configParam)
      const configs: CanalDenunciaConfig[] = JSON.parse(cleanedParam)
      if (!Array.isArray(configs) || configs.length === 0) return null

      const codDiretoria = user?.colaboradorOrg?.codDiretoria

      // Busca configuração específica para a diretoria do usuário
      const configEspecifica = configs.find(
        (c) => c.codigoUnidade === codDiretoria
      )

      // Se encontrou configuração específica, usa ela; senão usa DEFAULT
      const configSelecionada =
        configEspecifica ||
        configs.find((c) => c.codigoUnidade === 'DEFAULT') ||
        configs[0]

      return configSelecionada
    } catch (error) {
      console.error('Erro ao parsear configuração de canal de denúncias:', error)
      return null
    }
  }, [getParametro, user?.colaboradorOrg?.codDiretoria])

  // Se não houver configuração, não renderiza o componente
  if (!config) {
    return null
  }

  const handleSubmit = async () => {
    if (!manifestacao.trim()) {
      toast({
        title: 'Campo obrigatório',
        description: 'Por favor, descreva sua manifestação.',
        variant: 'destructive',
      })
      return
    }

    if (!token) {
      toast({
        title: 'Erro de autenticação',
        description: 'Token de autenticação não encontrado. Por favor, faça login novamente.',
        variant: 'destructive',
      })
      return
    }

    if (!config) {
      toast({
        title: 'Erro de configuração',
        description: 'Configuração do canal de denúncias não encontrada.',
        variant: 'destructive',
      })
      return
    }

    setIsSubmitting(true)
    try {
      const resultado = await enviarDenunciaUseCase.execute(token, {
        mensagem: manifestacao.trim(),
        email: config.emailDestinatario,
      })

      if (resultado.sucesso) {
        setManifestacao('')
        setModalSucessoOpen(true)
      } else {
        toast({
          title: 'Erro ao enviar',
          description: resultado.mensagem || 'Não foi possível enviar sua manifestação. Tente novamente.',
          variant: 'destructive',
        })
      }
    } catch (error) {
      toast({
        title: 'Erro ao enviar',
        description: error instanceof Error ? error.message : 'Não foi possível enviar sua manifestação. Tente novamente.',
        variant: 'destructive',
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <Card className="shadow-softToken rounded-lgToken w-full min-w-0 overflow-hidden">
      <CardHeader className="bg-muted rounded-t-lgToken p-4 sm:p-6">
        <CardTitle className="flex flex-col sm:flex-row items-start gap-3 min-w-0">
          <div className="p-2 bg-primary rounded-lgToken flex-shrink-0">
            <AlertCircle className="h-6 w-6 text-inverseText" />
          </div>
          <div className="min-w-0">
            <h2 className="text-lg sm:text-xl font-bold text-primaryText break-words">
              {config.title}
            </h2>
            <p className="text-sm text-secondaryText mt-1 font-normal break-words">
              {config.subTitle}
            </p>
          </div>
        </CardTitle>
      </CardHeader>

      <CardContent className="p-4 sm:p-6">
        <div className={previewDevice === 'mobile' || previewDevice === 'tablet' ? gridClassSingle : gridClassResponsive}>
          {/* Coluna Esquerda - Textos (quebra no mobile) */}
          <div className="space-y-4 min-w-0">
            <p className="text-sm text-secondaryText whitespace-pre-line break-words">
              {config.body}
            </p>
          </div>

          {/* Coluna Direita - TextArea e Botão (input largura total no mobile) */}
          <div className="space-y-4 w-full min-w-0">
            <div className="space-y-2 w-full">
              <Label htmlFor="manifestacao" className="text-base font-semibold text-primaryText break-words block">
                Descreva abaixo sua manifestação:
              </Label>
              <Textarea
                id="manifestacao"
                value={manifestacao}
                onChange={(e) => setManifestacao(e.target.value)}
                placeholder="Digite sua sugestão, reclamação ou denúncia aqui..."
                className="w-full min-w-0 min-h-[160px] sm:min-h-[200px] rounded-lgToken resize-none"
                maxLength={2000}
              />
              <div className="flex flex-wrap justify-between items-center gap-2">
                <p className="text-xs text-secondaryText">
                  {manifestacao.length}/2000 caracteres
                </p>
                <Button
                  onClick={handleSubmit}
                  disabled={isSubmitting || !manifestacao.trim()}
                  className="rounded-pillToken shrink-0"
                >
                  {isSubmitting ? 'Enviando...' : 'Enviar'}
                </Button>
              </div>
            </div>
          </div>
        </div>
      </CardContent>

      {/* Modal de Confirmação de Envio */}
      <Dialog open={modalSucessoOpen} onOpenChange={setModalSucessoOpen}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <div className="flex items-center gap-3">
              <div className="p-3 bg-primary/10 rounded-lg">
                <CheckCircle className="h-8 w-8 text-primary" />
              </div>
              <div>
                <DialogTitle className="text-2xl font-bold">
                  Denúncia registrada com sucesso
                </DialogTitle>
                <DialogDescription className="mt-1">
                  Sua manifestação foi enviada de forma segura e anônima.<br/>Obrigado por utilizar o Canal de Denúncias.
                </DialogDescription>
              </div>
            </div>
          </DialogHeader>

          <div className="space-y-4 py-4">
            <Card className="border-border bg-muted/50">
              <CardContent className="pt-6">
                <div className="space-y-2">
                  <p className="text-sm font-semibold text-primaryText">
                    Importante:
                  </p>
                  <p className="text-sm text-muted-foreground">
                    O Fourmakers atua exclusivamente como plataforma tecnológica de registro e intermediação, não sendo responsável pela análise, apuração, tratativa ou definição de soluções relacionadas ao conteúdo informado. Essas responsabilidades cabem exclusivamente à empresa responsável pelo Canal de Denúncias.
                  </p>
                </div>
              </CardContent>
            </Card>
          </div>

          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setModalSucessoOpen(false)}
            >
              Ciente
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </Card>
  )
}


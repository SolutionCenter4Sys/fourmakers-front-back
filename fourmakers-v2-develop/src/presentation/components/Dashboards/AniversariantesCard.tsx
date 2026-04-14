import { useEffect, useState } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { GetAniversariantesSemanaUseCase } from '@domain/usecases/GetAniversariantesSemanaUseCase'
import type { Aniversariante } from '@domain/entities/Aniversariante'
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar'
import { Loader2, Search } from '@/components/ui/system-icons'

const MOCK_ANIVERSARIANTES = [
  { codigoColaborador: 1, nome: 'Ana Paula Souza', dataNascimento: `02/20/${new Date().getFullYear()} 00:00:00`, ehAniversarianteHoje: true, urlFotoThumb: null, urlFotoThumbMini: null, urlFoto: null },
  { codigoColaborador: 2, nome: 'Carlos Henrique Lima', dataNascimento: `02/22/${new Date().getFullYear()} 00:00:00`, ehAniversarianteHoje: false, urlFotoThumb: null, urlFotoThumbMini: null, urlFoto: null },
  { codigoColaborador: 3, nome: 'Mariana Costa', dataNascimento: `02/24/${new Date().getFullYear()} 00:00:00`, ehAniversarianteHoje: false, urlFotoThumb: null, urlFotoThumbMini: null, urlFoto: null },
]

interface AniversariantesCardProps {
  preview?: boolean
}

export const AniversariantesCard = ({ preview = false }: AniversariantesCardProps) => {
  const { token } = useAppSelector((state) => state.auth)
  const [aniversariantes, setAniversariantes] = useState<Aniversariante[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (preview) {
      setAniversariantes(MOCK_ANIVERSARIANTES as unknown as Aniversariante[])
      setLoading(false)
      return
    }

    const loadAniversariantes = async () => {
      const currentToken = token || localStorage.getItem('authToken')
      
      if (!currentToken) {
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        setError(null)
        
        const useCase = container.resolve(GetAniversariantesSemanaUseCase)
        const response = await useCase.execute(currentToken)
        
        if (response.sucesso && response.retorno) {
          setAniversariantes(response.retorno)
        } else {
          setError(response.mensagem || 'Erro ao carregar aniversariantes')
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar aniversariantes')
      } finally {
        setLoading(false)
      }
    }

    loadAniversariantes()
  }, [token, preview])

  const formatarDataAniversario = (dataNascimento: string): { data: string; diaSemana: string } => {
    // A API retorna no formato "MM/DD/YYYY HH:mm:ss"
    const [datePart] = dataNascimento.split(' ')
    const [mes, dia] = datePart.split('/')
    
    // Criar data para o ano atual
    const hoje = new Date()
    const anoAtual = hoje.getFullYear()
    const dataAniversario = new Date(anoAtual, parseInt(mes) - 1, parseInt(dia))
    
    // Formatar data como DD/MM
    const dataFormatada = `${dia.padStart(2, '0')}/${mes.padStart(2, '0')}`
    
    // Obter dia da semana em português
    const diasSemana = ['Domingo', 'Segunda-feira', 'Terça-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira', 'Sábado']
    const diaSemana = diasSemana[dataAniversario.getDay()]
    
    return { data: dataFormatada, diaSemana }
  }

  const getIniciais = (nome: string): string => {
    const partes = nome.trim().split(' ')
    if (partes.length >= 2) {
      return `${partes[0][0]}${partes[partes.length - 1][0]}`.toUpperCase()
    }
    return nome.substring(0, 2).toUpperCase()
  }

  const handleAtualizarCadastro = () => {
    window.location.href = '/page/dadospessoais'
  }

  if (loading) {
    return (
      <Card className="shadow-softToken rounded-lgToken h-full max-h-full flex flex-col overflow-hidden min-w-0">
        <CardHeader className="pb-3 rounded-t-lgToken flex-shrink-0 p-4 sm:p-6">
          <CardTitle className="flex items-center gap-2 text-sm font-semibold text-primaryText">
            <div className="w-6 h-6 rounded-smToken flex items-center justify-center flex-shrink-0">
              <span className="text-inverseText text-xs">🎁</span>
            </div>
            <span>Parabéns aos aniversariantes</span>
          </CardTitle>
        </CardHeader>
        <CardContent className="flex items-center justify-center py-12 flex-1 min-h-0">
          <Loader2 className="h-6 w-6 animate-spin text-primary" />
        </CardContent>
      </Card>
    )
  }

  if (error) {
    return (
      <Card className="shadow-softToken rounded-lgToken h-full max-h-full flex flex-col overflow-hidden min-w-0">
        <CardHeader className="pb-3 rounded-t-lgToken flex-shrink-0 p-4 sm:p-6">
          <CardTitle className="flex items-center gap-2 text-sm font-semibold text-primaryText">
            <div className="w-6 h-6 rounded-smToken flex items-center justify-center flex-shrink-0">
              <span className="text-inverseText text-xs">🎁</span>
            </div>
            <span>Parabéns aos aniversariantes</span>
          </CardTitle>
        </CardHeader>
        <CardContent className="flex-1 min-h-0">
          <p className="text-sm text-secondaryText text-center py-8">{error}</p>
        </CardContent>
      </Card>
    )
  }

  // Estado vazio ou com dados — flex col + min-h-0 para o conteúdo rolar e o footer ficar sempre visível
  return (
    <Card className="shadow-softToken rounded-lgToken h-full max-h-full flex flex-col min-h-0 overflow-hidden min-w-0">
      <CardHeader className="pb-2 sm:pb-3 rounded-t-lgToken flex-shrink-0 p-4 sm:p-6">
        <CardTitle className="flex items-center gap-2 text-sm font-semibold text-primaryText min-w-0">
          <div className="w-6 h-6 rounded-smToken flex items-center justify-center flex-shrink-0">
            <span className="text-inverseText text-xs">🎁</span>
          </div>
          <span className="truncate">Parabéns aos aniversariantes</span>
        </CardTitle>
      </CardHeader>
      
      <CardContent className="p-4 sm:p-6 pt-0 flex-1 min-h-0 overflow-hidden min-w-0 flex flex-col">
        {aniversariantes.length === 0 ? (
          // Estado vazio - ícone de pasta/busca centralizado
          <div className="flex flex-col items-center justify-center py-12">
            <div className="w-24 h-24 border-2 border-accentSoft border-dashed rounded-lg flex items-center justify-center">
              <Search className="h-12 w-12 text-accent" />
            </div>
            <p className="text-xs text-secondaryText mt-4">Nenhum aniversariante esta semana</p>
          </div>
        ) : (
          // Lista de aniversariantes — área rolável, não empurra o footer
          <div className="space-y-3 flex-1 min-h-0 overflow-y-auto overflow-x-hidden">
            {aniversariantes.map((aniversariante) => {
              const { data, diaSemana } = formatarDataAniversario(aniversariante.dataNascimento)
              const isHoje = aniversariante.ehAniversarianteHoje
              const avatarUrl = aniversariante.urlFotoThumb || aniversariante.urlFotoThumbMini || aniversariante.urlFoto

              return (
                <div
                  key={aniversariante.codigoColaborador}
                  className={`flex items-center gap-3 py-2 px-2 rounded-lg ${
                    isHoje ? 'bg-accentSoft/50' : 'bg-transparent'
                  }`}
                >
                  {/* Data e dia da semana */}
                  <div className="flex flex-col items-center min-w-[50px] text-center">
                    {isHoje ? (
                      <div className="flex flex-col items-center gap-1">
                        <span className="text-lg">🎉</span>
                        <Badge variant="destructive" className="text-xs px-1.5 py-0">
                          Hoje!
                        </Badge>
                      </div>
                    ) : (
                      <>
                        <span className="text-xs font-semibold text-primaryText">{data}</span>
                        <span className="text-xs text-secondaryText">{diaSemana}</span>
                      </>
                    )}
                  </div>

                  {/* Linha tracejada vertical */}
                  <div className="h-10 border-l border-dashed border-borderDefault" />

                  {/* Avatar */}
                  <Avatar className="h-10 w-10 flex-shrink-0">
                    {avatarUrl ? (
                      <AvatarImage src={avatarUrl} alt={aniversariante.nome} />
                    ) : null}
                    <AvatarFallback className="bg-primarySoft text-xs">
                      {getIniciais(aniversariante.nome)}
                    </AvatarFallback>
                  </Avatar>

                  {/* Nome */}
                  <div className="flex-1 min-w-0">
                    <p className="text-xs font-medium text-primaryText truncate">
                      {aniversariante.nome}
                    </p>
                  </div>
                </div>
              )
            })}
          </div>
        )}
      </CardContent>

      <CardFooter className="p-4 sm:p-6 pt-3 flex-shrink-0 border-t border-borderSoft mt-auto">
        <Button
          variant="primary"
          className="w-full h-auto min-h-10 py-2.5 px-4 text-xs sm:text-sm font-medium whitespace-normal text-center leading-snug"
          onClick={handleAtualizarCadastro}
        >
          Seu aniversário não está aqui? Atualize seu cadastro
        </Button>
      </CardFooter>
    </Card>
  )
}

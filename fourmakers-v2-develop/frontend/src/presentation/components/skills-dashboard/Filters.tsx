import { useEffect, useState } from 'react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { setFilter, clearFilters } from '@app/store/slices/skillsDashboardSlice'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Label } from '@/components/ui/label'
import { RotateCcw, Loader2 } from 'lucide-react'
import { container } from '@core/di/container'
import { DiTokens } from '@core/di/tokens'
import type { ProjetosApi, Cliente } from '@data/api/ProjetosApi'
import { SKILL_TYPE_LABELS } from '@shared/constants/skillTypes'
import { EVENTOS_DISPONIVEIS } from '@shared/utils/skillsDashboardUtils'

// Tipos de skill fixos (os 5 tipos)
const TIPOS_SKILL_FIXOS = Object.values(SKILL_TYPE_LABELS).sort()

export const Filters = () => {
  const dispatch = useAppDispatch()
  const filters = useAppSelector((state) => state.skillsDashboard.filters)
  const { token } = useAppSelector((state) => state.auth)
  const [clientes, setClientes] = useState<Cliente[]>([])
  const [clientesLoading, setClientesLoading] = useState(false)

  // Carregar clientes de forma assíncrona
  useEffect(() => {
    const loadClientes = async () => {
      if (!token) return

      setClientesLoading(true)
      try {
        const projetosApi = container.resolve<ProjetosApi>(DiTokens.projetosApi)
        const response = await projetosApi.listarClientesOrg(token)
        const clientesList = Array.isArray(response) ? response : []
        setClientes(clientesList)
      } catch (err) {
        console.error('Erro ao carregar clientes:', err)
        setClientes([])
      } finally {
        setClientesLoading(false)
      }
    }

    void loadClientes()
  }, [token])

  const handleChange = (name: keyof typeof filters, value: string) => {
    dispatch(setFilter({ name, value }))
  }

  const handleClear = () => {
    dispatch(clearFilters())
  }

  return (
    <Card className="mb-6 rounded-lgToken border-borderSoft shadow-softToken">
      <CardHeader className="mb-4 p-lg">
        <div className="flex items-center justify-between">
          <CardTitle className="text-lg font-semibold text-primaryText">Filtros</CardTitle>
          <button
            onClick={handleClear}
            className="flex items-center gap-1 text-sm text-secondaryText transition-colors hover:text-error"
          >
            <RotateCcw size={14} />
            Limpar Filtros
          </button>
        </div>
      </CardHeader>
      <CardContent>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
          {/* Nome */}
          <div className="space-y-2">
            <Label htmlFor="nome">Nome</Label>
            <Input
              id="nome"
              placeholder="Busca por Nome"
              value={filters.nome}
              onChange={(e) => handleChange('nome', e.target.value)}
            />
          </div>

          {/* Cliente */}
          <div className="space-y-2">
            <Label htmlFor="cliente">Cliente</Label>
            <Select
              value={filters.cliente || 'todos'}
              onValueChange={(value) => handleChange('cliente', value === 'todos' ? '' : value)}
              disabled={clientesLoading}
            >
              <SelectTrigger id="cliente">
                <SelectValue placeholder={clientesLoading ? 'Carregando...' : 'Todos Clientes'} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todos">Todos Clientes</SelectItem>
                {clientesLoading ? (
                  <div className="flex items-center justify-center p-2">
                    <Loader2 className="h-4 w-4 animate-spin" />
                  </div>
                ) : (
                  clientes
                    .filter((c) => c.nomeCliente && c.nomeCliente.trim() !== '')
                    .map((c) => {
                      // Ensure value is never empty string - use codigoCliente as fallback
                      const clienteValue = (c.nomeCliente?.trim() || `cliente-${c.codigoCliente}`)
                      // Double-check: if somehow still empty, skip this item
                      if (!clienteValue || clienteValue === '') {
                        return null
                      }
                      return (
                        <SelectItem key={c.codigoCliente} value={clienteValue}>
                          {c.nomeCliente}
                        </SelectItem>
                      )
                    })
                    .filter(Boolean)
                )}
              </SelectContent>
            </Select>
          </div>

          {/* Tipo Skill */}
          <div className="space-y-2">
            <Label htmlFor="tipoSkill">Tipo Skill</Label>
            <Select
              value={filters.tipoSkill || 'todos'}
              onValueChange={(value) => handleChange('tipoSkill', value === 'todos' ? '' : value)}
            >
              <SelectTrigger id="tipoSkill">
                <SelectValue placeholder="Todos Tipos" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todos">Todos Tipos</SelectItem>
                {TIPOS_SKILL_FIXOS
                  .filter((tipo) => tipo && tipo.trim() !== '')
                  .map((tipo) => {
                    // Ensure value is never empty string
                    const tipoValue = tipo?.trim() || 'unknown-tipo'
                    // Double-check: if somehow still empty, skip this item
                    if (!tipoValue || tipoValue === '') {
                      return null
                    }
                    return (
                      <SelectItem key={tipoValue} value={tipoValue}>
                        {tipo}
                      </SelectItem>
                    )
                  })
                  .filter(Boolean)}
              </SelectContent>
            </Select>
          </div>

          {/* Evento */}
          <div className="space-y-2">
            <Label htmlFor="evento">Evento</Label>
            <Select
              value={filters.evento || 'todos'}
              onValueChange={(value) => handleChange('evento', value === 'todos' ? '' : value)}
            >
              <SelectTrigger id="evento">
                <SelectValue placeholder="Todos Eventos" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todos">Todos Eventos</SelectItem>
                {EVENTOS_DISPONIVEIS
                  .filter((evento) => evento.value && evento.value.trim() !== '')
                  .map((evento) => {
                    // Ensure value is never empty string
                    const eventoValue = evento.value?.trim() || 'unknown-evento'
                    // Double-check: if somehow still empty, skip this item
                    if (!eventoValue || eventoValue === '') {
                      return null
                    }
                    return (
                      <SelectItem key={eventoValue} value={eventoValue}>
                        {evento.label}
                      </SelectItem>
                    )
                  })
                  .filter(Boolean)}
              </SelectContent>
            </Select>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}


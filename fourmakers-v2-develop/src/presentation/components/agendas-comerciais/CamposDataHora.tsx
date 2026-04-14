import { CalendarIcon } from 'lucide-react'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import { Calendar } from '@/components/ui/calendar'
import { Label } from '@/components/ui/label'
import { Input } from '@/components/ui/input'
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import { Button } from '@/components/ui/button'
import { getCamposObrigatoriosPorTipoInteracao } from '@shared/utils/validacaoAgenda'

interface CamposDataHoraProps {
  dataAgendada: Date | null
  horaInicio: string
  horaFim: string
  tipoInteracao: number
  errors: { [campo: string]: string }
  showInicioFim: boolean
  onDataChange: (date: Date | undefined) => void
  onHoraInicioChange: (hora: string) => void
  onHoraFimChange: (hora: string) => void
  onErrorClear: (campo: string) => void
}

export function CamposDataHora({
  dataAgendada,
  horaInicio,
  horaFim,
  tipoInteracao,
  errors,
  showInicioFim,
  onDataChange,
  onHoraInicioChange,
  onHoraFimChange,
  onErrorClear,
}: CamposDataHoraProps) {
  return (
    <>
      {/* Data Agendada */}
      <div className="space-y-2">
        <Label htmlFor="dataAgendada">
          Data Agendada <span className="text-destructive">*</span>
        </Label>
        <Popover>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              className="w-full justify-start text-left font-normal"
              id="dataAgendada"
            >
              <CalendarIcon className="mr-2 h-4 w-4" />
              {dataAgendada ? (
                format(dataAgendada, "PPP", { locale: ptBR })
              ) : (
                <span className="text-muted-foreground">Selecione a data</span>
              )}
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-auto p-0" align="start">
            <Calendar
              mode="single"
              selected={dataAgendada || undefined}
              onSelect={onDataChange}
              disabled={(date) => date < new Date(new Date().setHours(0, 0, 0, 0))}
              initialFocus
            />
          </PopoverContent>
        </Popover>
        {errors.dataAgendada && <p className="text-sm text-destructive">{errors.dataAgendada}</p>}
      </div>

      {/* Início/Fim - visível apenas para Reunião e Presencial */}
      {showInicioFim && (
        <div className="grid grid-cols-2 gap-4">
          <div className="space-y-2">
            <Label htmlFor="horaInicio">
              Hora Início
              {getCamposObrigatoriosPorTipoInteracao(tipoInteracao).includes('dataInicio') && (
                <span className="text-destructive"> *</span>
              )}
            </Label>
            <div 
              className="relative cursor-pointer"
              onClick={(e) => {
                const input = e.currentTarget.querySelector('input') as HTMLInputElement
                if (input && !input.disabled) {
                  input.focus()
                  // showPicker está disponível em navegadores modernos
                  if (typeof input.showPicker === 'function') {
                    input.showPicker()
                  }
                }
              }}
            >
              <Input
                id="horaInicio"
                type="time"
                value={horaInicio}
                onChange={(e) => {
                  onHoraInicioChange(e.target.value)
                  if (errors.dataInicio) {
                    onErrorClear('dataInicio')
                  }
                }}
                error={!!errors.dataInicio}
                disabled={!dataAgendada}
                className="pr-4 cursor-pointer"
              />
            </div>
            {!dataAgendada && (
              <p className="text-xs text-muted-foreground">Selecione a data primeiro</p>
            )}
            {errors.dataInicio && <p className="text-sm text-destructive">{errors.dataInicio}</p>}
          </div>

          <div className="space-y-2">
            <Label htmlFor="horaFim">
              Hora Fim
              {getCamposObrigatoriosPorTipoInteracao(tipoInteracao).includes('dataFim') && (
                <span className="text-destructive"> *</span>
              )}
            </Label>
            <div 
              className="relative cursor-pointer"
              onClick={(e) => {
                const input = e.currentTarget.querySelector('input') as HTMLInputElement
                if (input && !input.disabled) {
                  input.focus()
                  // showPicker está disponível em navegadores modernos
                  if (typeof input.showPicker === 'function') {
                    input.showPicker()
                  }
                }
              }}
            >
              <Input
                id="horaFim"
                type="time"
                value={horaFim}
                onChange={(e) => {
                  onHoraFimChange(e.target.value)
                  if (errors.dataFim) {
                    onErrorClear('dataFim')
                  }
                }}
                error={!!errors.dataFim}
                disabled={!dataAgendada}
                className="pr-4 cursor-pointer"
              />
            </div>
            {!dataAgendada && (
              <p className="text-xs text-muted-foreground">Selecione a data primeiro</p>
            )}
            {errors.dataFim && <p className="text-sm text-destructive">{errors.dataFim}</p>}
          </div>
        </div>
      )}
    </>
  )
}

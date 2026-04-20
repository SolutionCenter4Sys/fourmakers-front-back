import { Building2, ChevronsUpDown } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@/components/ui/command'
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import { Spinner } from '@/components/ui/spinner'
import { cn } from '@/lib/utils'

interface Cliente {
  id: string
  name: string
}

interface SelecaoClienteProps {
  clienteSelecionado: Cliente | null
  clientes: Cliente[]
  clienteSearch: string
  clientsLoading: boolean
  isPopoverOpen: boolean
  error?: string
  disabled?: boolean
  onClienteSelect: (cliente: Cliente) => void
  onSearchChange: (search: string) => void
  onPopoverChange: (open: boolean) => void
}

export function SelecaoCliente({
  clienteSelecionado,
  clientes,
  clienteSearch,
  clientsLoading,
  isPopoverOpen,
  error,
  disabled = false,
  onClienteSelect,
  onSearchChange,
  onPopoverChange,
}: SelecaoClienteProps) {
  return (
    <div className="space-y-2">
      <Label htmlFor="codigoCliente">
        Cliente <span className="text-destructive">*</span>
      </Label>
      <Popover
        open={isPopoverOpen}
        onOpenChange={(open) => {
          onPopoverChange(open)
          // Limpar busca apenas ao fechar, não ao abrir (para permitir busca imediata)
          if (!open) {
            onSearchChange('')
          }
        }}
      >
        <PopoverTrigger asChild>
          <Button
            variant="outline"
            role="combobox"
            className={cn('w-full justify-between', error && 'border-destructive')}
            disabled={disabled}
          >
            <span className="truncate">
              {clienteSelecionado ? (
                <span className="flex items-center gap-2">
                  <Building2 className="h-4 w-4 shrink-0" />
                  {clienteSelecionado.name}
                </span>
              ) : (
                'Selecione o cliente'
              )}
            </span>
            <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
          </Button>
        </PopoverTrigger>
        <PopoverContent className="w-[500px] p-0" align="start">
          <Command shouldFilter={false}>
            <CommandInput
              placeholder="Buscar cliente..."
              value={clienteSearch}
              onValueChange={onSearchChange}
            />
            <CommandList
              className="max-h-[300px]"
              onWheel={(e) => {
                e.stopPropagation()
              }}
            >
              {clientsLoading ? (
                <div className="flex items-center justify-center py-8">
                  <Spinner size={16} className="text-primary" />
                  <span className="ml-2 text-sm text-muted-foreground">Carregando...</span>
                </div>
              ) : clientes.length === 0 ? (
                <CommandEmpty>
                  {clienteSearch.trim() ? 'Nenhum cliente encontrado' : 'Digite para buscar clientes'}
                </CommandEmpty>
              ) : (
                <CommandGroup>
                  {clientes.map((cliente) => (
                    <CommandItem
                      key={cliente.id}
                      value={cliente.id}
                      onSelect={() => {
                        onClienteSelect(cliente)
                      }}
                    >
                      <Building2 className="mr-2 h-4 w-4" />
                      <div className="flex flex-col">
                        <span>{cliente.name}</span>
                        <span className="text-xs text-muted-foreground">{cliente.id}</span>
                      </div>
                    </CommandItem>
                  ))}
                </CommandGroup>
              )}
            </CommandList>
          </Command>
        </PopoverContent>
      </Popover>
      {clienteSelecionado && (
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <span>Código: {clienteSelecionado.id}</span>
        </div>
      )}
      {error && <p className="text-sm text-destructive">{error}</p>}
    </div>
  )
}

import type { MinhaJornadaProfile } from '@domain/entities/MinhaJornadaProfile'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'

interface ProfileSelectorProps {
  currentProfile: MinhaJornadaProfile | null
  availableProfiles: MinhaJornadaProfile[]
  onSelectProfile: (perfilId: string) => void
  isLoading?: boolean
}

export const ProfileSelector = ({
  currentProfile,
  availableProfiles,
  onSelectProfile,
  isLoading,
}: ProfileSelectorProps) => {
  if (isLoading) {
    return (
      <div className="h-10 w-full rounded-lg border border-input bg-input animate-pulse" />
    )
  }

  if (!currentProfile) {
    return (
      <div className="text-sm text-muted-foreground">
        Nenhum perfil de atuação encontrado
      </div>
    )
  }

  const isSelectorDisabled = availableProfiles.length <= 1

  return (
    <Select
      value={currentProfile.perfilId || ''}
      onValueChange={onSelectProfile}
      disabled={isSelectorDisabled}
    >
      <SelectTrigger className="h-10 rounded-lg text-primaryText opacity-100 disabled:opacity-100 font-medium">
        <SelectValue className="opacity-100">
          {currentProfile.perfilNome || 'Perfil não definido'}
          {currentProfile.nomeCliente && ` - ${currentProfile.nomeCliente}`}
        </SelectValue>
      </SelectTrigger>
      <SelectContent>
        {availableProfiles.map((p) => (
          <SelectItem 
            key={p.perfilId} 
            value={p.perfilId || ''}
            className="text-primaryText font-medium"
          >
            {p.perfilNome || 'Perfil não definido'}
            {p.nomeCliente && ` - ${p.nomeCliente}`}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  )
}


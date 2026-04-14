import { Spinner } from '@/components/ui/spinner'

export function MapaRelacionamentoLoadingState() {
  return (
    <div className="absolute inset-0 flex items-center justify-center bg-primaryBackground">
      <Spinner size={32} className="text-primary" />
    </div>
  )
}

import { useNavigate } from 'react-router-dom'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { User, FileText, ArrowRight } from 'lucide-react'

interface AtualizarPerfilModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

export const AtualizarPerfilModal = ({
  open,
  onOpenChange,
}: AtualizarPerfilModalProps) => {
  const navigate = useNavigate()

  const handleAtualizarPerfil = () => {
    onOpenChange(false)
    navigate('/atualizar-perfil')
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <div className="flex items-center gap-3">
            <div className="p-3 bg-primary/10 rounded-lg">
              <User className="h-8 w-8 text-primary" />
            </div>
            <div>
              <DialogTitle className="text-2xl font-bold">
                Atualize seu Perfil
              </DialogTitle>
              <DialogDescription className="mt-1">
              Manter seu perfil atualizado ajuda a empresa a organizar melhor alocações, certificados, férias, desempenho e novos projetos.
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <Card className="border-primary/20 bg-primary/5">
            <CardContent className="pt-6">
              <div className="space-y-4">
                <div className="flex items-start gap-3">
                  <div className="p-2 bg-primary/10 rounded-lg mt-1">
                    <FileText className="h-5 w-5 text-primary" />
                  </div>
                  <div className="flex-1 space-y-1">
                    <h3 className="font-semibold text-lg">
                      Questionário de Perfil Pendente
                    </h3>
                    <p className="text-sm text-muted-foreground">
                      Existe um questionário rápido aguardando preenchimento. Ele garante que seus dados estejam atualizados e evita ajustes manuais depois.
                    </p>
                  </div>
                </div>

                
              </div>
            </CardContent>
          </Card>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Depois
          </Button>
          <Button onClick={handleAtualizarPerfil} className="gap-2">
            Atualizar Perfil
            <ArrowRight className="h-4 w-4" />
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}


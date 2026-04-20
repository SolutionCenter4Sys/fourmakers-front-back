import { Plus, Award, Calendar, Clock, Download } from "@/components/ui/system-icons";
import { Button } from "@/components/ui/button";
import type { CertificadoCompletoProfile360 } from "@domain/entities/Profile360";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { useAppSelector } from "@app/store/hooks";
import { toast } from "sonner";
import { container } from "@core/di/container";
import { BaixarCertificadoColaboradorUseCase } from "@domain/usecases/BaixarCertificadoColaboradorUseCase";

interface CertificationSectionProps {
  certificados?: CertificadoCompletoProfile360[];
  readOnly?: boolean;
}

export const CertificationSection = ({ certificados = [], readOnly = false }: CertificationSectionProps) => {
  const { token } = useAppSelector((state) => state.auth);

  const formatDate = (dateString: string) => {
    try {
      const date = new Date(dateString);
      return format(date, "MM/yyyy", { locale: ptBR });
    } catch {
      return dateString;
    }
  };

  const handleDownloadCertificado = async (path: string) => {
    if (!token) {
      toast.error("Token de autenticação não disponível.");
      return;
    }

    try {
      const useCase = container.resolve(BaixarCertificadoColaboradorUseCase);
      const result = await useCase.execute(token, path);

      const blob = new Blob([result.arrayBuffer], { type: result.contentType });

      // Criar URL temporária e fazer download
      const blobUrl = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = blobUrl;
      link.download = result.fileName;

      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);

      // Limpar URL temporária
      window.URL.revokeObjectURL(blobUrl);

      toast.success("Download do certificado iniciado");
    } catch (err) {
      console.error("Erro ao baixar certificado:", err);
      toast.error(
        err instanceof Error ? err.message : "Erro ao baixar certificado"
      );
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Award className="h-5 w-5 text-muted-foreground" />
          <h2 className="text-xl font-semibold text-foreground">Certificações</h2>
        </div>
        {!readOnly && (
          <Button size="sm" variant="outline" className="h-8">
            <Plus className="h-3.5 w-3.5 mr-1.5" />
            Adicionar
          </Button>
        )}
      </div>

      <div className="space-y-3">
        {certificados.length === 0 ? (
          <div className="text-center py-8 text-muted-foreground">Nenhuma certificação cadastrada</div>
        ) : (
          certificados.map((certItem) => {
            const cert = certItem.certificado;
            return (
              <div
                key={cert.idCertificado}
                className="rounded-xl border bg-card p-4 transition-all hover:shadow-md"
              >
                <div className="flex items-start gap-4">
                  <div className="flex h-10 w-10 flex-shrink-0 items-center justify-center rounded-lg bg-violet-100 dark:bg-violet-950">
                    <Award className="h-5 w-5 text-violet-600 dark:text-violet-400" />
                  </div>
                  
                  <div className="flex-1 space-y-2">
                    <div>
                      <h4 className="font-semibold text-foreground text-base">{cert.descricao || "Certificação"}</h4>
                      {cert.instituicao && (
                        <p className="text-sm text-muted-foreground mt-0.5">{cert.instituicao}</p>
                      )}
                    </div>
                    
                    <div className="flex flex-wrap items-center gap-3 text-sm text-muted-foreground">
                      {cert.conclusao && (
                        <div className="flex items-center gap-1">
                          <Calendar className="h-3.5 w-3.5" />
                          <span>Conclusão: {formatDate(cert.conclusao)}</span>
                        </div>
                      )}
                      {cert.cargaHoraria > 0 && (
                        <div className="flex items-center gap-1">
                          <Clock className="h-3.5 w-3.5" />
                          <span>{cert.cargaHoraria}h</span>
                        </div>
                      )}
                    </div>
                    
                    {cert.path && (
                      <Button 
                        variant="outline" 
                        size="sm" 
                        className="h-8 text-sm mt-2"
                        onClick={() => handleDownloadCertificado(cert.path!)}
                      >
                        <Download className="h-3.5 w-3.5 mr-1" />
                        Baixar certificado
                      </Button>
                    )}
                  </div>
                </div>
              </div>
            );
          })
        )}
      </div>
    </div>
  );
};

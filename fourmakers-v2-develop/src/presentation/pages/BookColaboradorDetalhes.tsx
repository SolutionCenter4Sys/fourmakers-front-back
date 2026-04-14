import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAppSelector } from "@app/store/hooks";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "@/components/ui/accordion";
import { ArrowLeft, Send, Calendar, FileText, Maximize2 } from "@/components/ui/system-icons";
import { useColaboradorDetalhes, useFeriasInfo } from "@/presentation/hooks/useBookColaborador";
import { formatData, formatTelefone, formatSalario, formatSaldoHoras } from "@shared/utils/bookColaboradorFormatters";
import { DialogAgendar1on1 } from "@presentation/components/bookColaborador/DialogAgendar1on1";
import { DialogVerFerias } from "@presentation/components/bookColaborador/DialogVerFerias";
import { generateCVPDF } from "@shared/utils/generateCVPDF";

export default function BookColaboradorDetalhes() {
  const { codColaborador } = useParams<{ codColaborador: string }>();
  const navigate = useNavigate();
  const user = useAppSelector((state) => state.auth.user);
  const orgId = user?.colaboradorOrg?.orgId;
  const { detalhes, loading } = useColaboradorDetalhes(codColaborador || "");
  const { ferias } = useFeriasInfo(codColaborador || "");
  const [dialogAgendarOpen, setDialogAgendarOpen] = useState(false);
  const [dialogFeriasOpen, setDialogFeriasOpen] = useState(false);

  if (loading) {
    return (
      <div className="container mx-auto p-4">
        <div className="text-center py-8 text-muted-foreground">Carregando...</div>
      </div>
    );
  }

  if (!detalhes) {
    return (
      <div className="container mx-auto p-4">
        <div className="text-center py-8 text-destructive">Colaborador não encontrado</div>
      </div>
    );
  }

  const iniciais = detalhes.nome
    .split(" ")
    .map((n) => n[0])
    .join("")
    .toUpperCase()
    .slice(0, 2);

  const handleAbrirCV = () => {
    try {
      if (!detalhes) {
        alert("Dados do colaborador não disponíveis.");
        return;
      }
      generateCVPDF(detalhes);
    } catch (error) {
      console.error("Erro ao gerar CV:", error);
      alert("Erro ao gerar PDF do CV. Tente novamente.");
    }
  };

  const handleVerPDI = () => {
    // TODO: Implementar navegação para PDI
    navigate(`/pdijornada?colaborador=${detalhes.codColaborador}`);
  };

  const FieldBox = ({ label, value }: { label: string; value: string }) => (
    <div className="bg-white border border-gray-200 rounded-lg p-3">
      <p className="text-sm text-muted-foreground mb-1">{label}</p>
      <p className="font-medium text-foreground">{value}</p>
    </div>
  );

  return (
    <div className="container mx-auto p-4 space-y-4">
      <Button 
        variant="ghost" 
        onClick={() => navigate("/book-colaborador")}
        className="mb-2"
      >
        <ArrowLeft className="h-4 w-4 mr-2" />
        Voltar para equipe
      </Button>

      <Card className="rounded-xl">
        <CardContent className="p-6">
          <div className="flex items-center gap-2 mb-6">
            <div className="h-6 w-1 bg-gray-700 rounded"></div>
            <h1 className="text-2xl font-bold">Perfil do Colaborador</h1>
          </div>

          <div className="flex items-start gap-6">
            <Avatar className="h-24 w-24 rounded-lg">
              <AvatarImage src="" alt={detalhes.nome} />
              <AvatarFallback className="text-2xl rounded-lg bg-muted">
                {iniciais}
              </AvatarFallback>
            </Avatar>
            <div className="flex-1">
              <h2 className="text-2xl font-bold mb-1">{detalhes.nome}</h2>
              <p className="text-muted-foreground mb-4">{detalhes.cargo}</p>
              <div className="flex gap-2 mb-4 flex-wrap">
                <Badge variant="secondary" className="font-normal">
                  #{detalhes.codColaborador}
                </Badge>
                <Badge className="bg-green-100 text-green-800 hover:bg-green-100 border-0">
                  {detalhes.trabalho.status}
                </Badge>
                <Badge variant="secondary" className="font-normal">
                  {detalhes.trabalho.modelo}
                </Badge>
              </div>
              <div className="flex gap-2 flex-wrap">
                <Button
                  onClick={() => setDialogAgendarOpen(true)}
                  variant="outline"
                  size="sm"
                >
                  <Send className="h-4 w-4 mr-2" />
                  Agendar 1:1
                </Button>
                <Button
                  onClick={() => setDialogFeriasOpen(true)}
                  variant="outline"
                  size="sm"
                >
                  <Calendar className="h-4 w-4 mr-2" />
                  Ver Férias
                </Button>
                <Button
                  onClick={handleVerPDI}
                  variant="outline"
                  size="sm"
                >
                  <FileText className="h-4 w-4 mr-2" />
                  Ver PDI
                </Button>
                <Button
                  onClick={handleAbrirCV}
                  variant="outline"
                  size="sm"
                >
                  <Maximize2 className="h-4 w-4 mr-2" />
                  Abrir CV
                </Button>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      <Accordion type="single" collapsible defaultValue="informacoes-pessoais" className="w-full space-y-3">
        <AccordionItem value="informacoes-pessoais" className="border rounded-lg bg-white">
          <AccordionTrigger className="px-6 py-4">Informações Pessoais</AccordionTrigger>
          <AccordionContent className="px-6 pb-6">
            <div className="grid grid-cols-2 gap-4">
              <FieldBox label="Nome Completo" value={detalhes.nome} />
              <FieldBox label="Data de Nascimento" value={formatData(detalhes.nascimento)} />
              <FieldBox label="Telefone" value={formatTelefone(detalhes.telefone)} />
              <FieldBox label="E-mail" value={detalhes.email} />
              <FieldBox label="Cidadania" value={detalhes.cidadania} />
            </div>
          </AccordionContent>
        </AccordionItem>

        <AccordionItem value="informacoes-trabalho" className="border rounded-lg bg-white">
          <AccordionTrigger className="px-6 py-4">Informações de Trabalho</AccordionTrigger>
          <AccordionContent className="px-6 pb-6">
            <div className="grid grid-cols-2 gap-4">
              <FieldBox label="Modelo" value={detalhes.trabalho.modelo} />
              {orgId !== 2 && (
                <>
                  <FieldBox label="Salário" value={formatSalario(detalhes.trabalho.salario)} />
                  <FieldBox label="Saldo de Horas" value={formatSaldoHoras(detalhes.trabalho.saldoHoras)} />
                </>
              )}
              <FieldBox label="Tempo de Casa" value={detalhes.trabalho.tempoCasa} />
              <FieldBox label="Data de Admissão" value={formatData(detalhes.trabalho.dataAdmissao)} />
              <FieldBox label="Status" value={detalhes.trabalho.status} />
            </div>
          </AccordionContent>
        </AccordionItem>

        <AccordionItem value="informacoes-financeiras" className="border rounded-lg bg-white">
          <AccordionTrigger className="px-6 py-4">Informações Financeiras</AccordionTrigger>
          <AccordionContent className="px-6 pb-6">
            <div className="grid grid-cols-2 gap-4">
              <FieldBox label="Banco" value={detalhes.financeiro.banco} />
              <FieldBox label="Agência" value={detalhes.financeiro.agencia} />
              <FieldBox label="Conta" value={detalhes.financeiro.conta} />
              <FieldBox label="Tipo de Conta" value={detalhes.financeiro.tipoConta} />
            </div>
          </AccordionContent>
        </AccordionItem>

        <AccordionItem value="endereco" className="border rounded-lg bg-white">
          <AccordionTrigger className="px-6 py-4">Endereço</AccordionTrigger>
          <AccordionContent className="px-6 pb-6">
            <div className="grid grid-cols-2 gap-4">
              <div className="col-span-2">
                <FieldBox 
                  label="Rua" 
                  value={`${detalhes.endereco.rua}, ${detalhes.endereco.numero}${detalhes.endereco.complemento ? ` - ${detalhes.endereco.complemento}` : ''}`} 
                />
              </div>
              <FieldBox label="Bairro" value={detalhes.endereco.bairro} />
              <FieldBox label="Cidade" value={detalhes.endereco.cidade} />
              <FieldBox label="Estado" value={detalhes.endereco.estado} />
              <FieldBox label="CEP" value={detalhes.endereco.cep} />
            </div>
          </AccordionContent>
        </AccordionItem>
      </Accordion>

      <DialogAgendar1on1
        open={dialogAgendarOpen}
        onOpenChange={setDialogAgendarOpen}
        colaboradorNome={detalhes.nome}
        colaboradorCod={detalhes.codColaborador}
      />

      <DialogVerFerias
        open={dialogFeriasOpen}
        onOpenChange={setDialogFeriasOpen}
        ferias={ferias}
        colaboradorNome={detalhes.nome}
      />
    </div>
  );
}

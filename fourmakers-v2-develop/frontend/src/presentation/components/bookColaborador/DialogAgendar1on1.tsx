import { useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "@/components/ui/accordion";
import { CalendarIcon, Clock } from "@/components/ui/system-icons";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { cn } from "@/lib/utils";

interface DialogAgendar1on1Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  colaboradorNome: string;
  colaboradorCod: string;
}

export function DialogAgendar1on1({
  open,
  onOpenChange,
  colaboradorNome,
  colaboradorCod: _colaboradorCod,
}: DialogAgendar1on1Props) {
  const [data, setData] = useState<Date>();
  const [hora, setHora] = useState("");
  const [topicos, setTopicos] = useState("");
  const [salvando, setSalvando] = useState(false);

  const handleSalvar = async () => {
    if (!data || !hora) {
      alert("Por favor, preencha data e horário.");
      return;
    }

    setSalvando(true);
    // TODO: Implementar chamada de API
    setTimeout(() => {
      setSalvando(false);
      alert("1:1 agendado com sucesso!");
      onOpenChange(false);
      setData(undefined);
      setHora("");
      setTopicos("");
    }, 1000);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>Agendar 1:1</DialogTitle>
          <DialogDescription>
            Agende uma reunião 1:1 com {colaboradorNome}
          </DialogDescription>
        </DialogHeader>

        <Accordion type="single" collapsible defaultValue="dados" className="w-full">
          <AccordionItem value="dados" className="border-none">
            <AccordionTrigger className="hidden">Dados do Agendamento</AccordionTrigger>
            <AccordionContent className="space-y-4 pt-0">
              <div className="space-y-2">
                <Label htmlFor="data">Data</Label>
                <Popover>
                  <PopoverTrigger asChild>
                    <Button
                      variant="outline"
                      className={cn(
                        "w-full justify-start text-left font-normal",
                        !data && "text-muted-foreground"
                      )}
                    >
                      <CalendarIcon className="mr-2 h-4 w-4" />
                      {data ? format(data, "dd/MM/yyyy", { locale: ptBR }) : "dd/mm/aaaa"}
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent className="w-auto p-0">
                    <Calendar
                      mode="single"
                      selected={data}
                      onSelect={setData}
                      initialFocus
                      locale={ptBR}
                    />
                  </PopoverContent>
                </Popover>
              </div>

              <div className="space-y-2">
                <Label htmlFor="hora">Horário</Label>
                <div className="relative">
                  <Clock className="absolute right-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                  <Input
                    id="hora"
                    type="time"
                    value={hora}
                    onChange={(e) => setHora(e.target.value)}
                    className="pr-10"
                    placeholder="--:--"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="topicos">Tópicos a discutir</Label>
                <Textarea
                  id="topicos"
                  value={topicos}
                  onChange={(e) => setTopicos(e.target.value)}
                  placeholder="Ex: Progresso Q4, Feedback projeto X, Desenvolvimento..."
                  rows={4}
                />
              </div>
            </AccordionContent>
          </AccordionItem>
        </Accordion>

        <div className="flex justify-end gap-2 pt-4">
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancelar
          </Button>
          <Button onClick={handleSalvar} disabled={salvando}>
            {salvando ? "Salvando..." : "Agendar"}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}

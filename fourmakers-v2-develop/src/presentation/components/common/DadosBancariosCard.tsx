import { useState, useEffect } from "react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { DollarSign, Edit, Loader2 } from "@/components/ui/system-icons";
import { useToast } from "@/hooks/use-toast";
import { useAppSelector } from "@/app/store/hooks";
import { container } from "@/core/di/container";
import type { DadosBancarios, PayloadDadosBancarios } from "@domain/entities/DadosBancarios";
import { GetDadosBancariosUseCase } from "@domain/usecases/GetDadosBancariosUseCase";
import { CriarDadosBancariosUseCase } from "@domain/usecases/CriarDadosBancariosUseCase";
import { EditarDadosBancariosUseCase } from "@domain/usecases/EditarDadosBancariosUseCase";

interface DadosBancariosCardProps {
  /**
   * Se true, o componente será exibido. Se false, não será renderizado.
   */
  mostrar?: boolean;
  /**
   * Callback chamado quando os dados bancários são carregados ou atualizados.
   * Recebe true se há dados bancários cadastrados, false caso contrário.
   */
  onDadosBancariosChange?: (temDadosBancarios: boolean) => void;
}

const tipoChavePixOptions = [
  { value: "C", label: "CPF" },
  { value: "J", label: "CNPJ" },
  { value: "E", label: "Email" },
  { value: "T", label: "Telefone" },
  { value: "R", label: "Chave Aleatória" },
];

export function DadosBancariosCard({ mostrar = true, onDadosBancariosChange }: DadosBancariosCardProps) {
  const { token } = useAppSelector((state) => state.auth);
  const { toast } = useToast();
  
  const [dadosBancarios, setDadosBancarios] = useState<DadosBancarios | null>(null);
  const [loadingDadosBancarios, setLoadingDadosBancarios] = useState(true);
  const [modalDadosBancarios, setModalDadosBancarios] = useState(false);
  const [salvandoDadosBancarios, setSalvandoDadosBancarios] = useState(false);
  const [dadosBancariosErros, setDadosBancariosErros] = useState<string[]>([]);
  const [formDadosBancarios, setFormDadosBancarios] = useState<PayloadDadosBancarios>({
    codigoBancoTed: "",
    agenciaTed: "",
    agenciaDvTed: "",
    contaTed: "",
    contaDvTed: "",
    chavePix: "",
    tipoChavePix: "C",
    formaPagamento: 1,
  });

  // Use Cases
  const getDadosBancariosUseCase = container.resolve(GetDadosBancariosUseCase);
  const criarDadosBancariosUseCase = container.resolve(CriarDadosBancariosUseCase);
  const editarDadosBancariosUseCase = container.resolve(EditarDadosBancariosUseCase);

  // Função auxiliar para verificar se há dados bancários válidos
  const verificarDadosBancariosValidos = (dados: DadosBancarios | null): boolean => {
    if (!dados) return false;
    
    // Verificar se tem chave PIX
    const temChavePix = !!(dados.chavePix && dados.chavePix.trim() !== "");
    
    // Verificar se tem dados bancários completos
    const temDadosBancarios = !!(
      dados.codigoBancoTed && dados.codigoBancoTed.trim() !== "" &&
      dados.agenciaTed && dados.agenciaTed.trim() !== "" &&
      dados.contaTed && dados.contaTed.trim() !== ""
    );
    
    return temChavePix || temDadosBancarios;
  };

  // Carregar dados bancários
  useEffect(() => {
    const carregarDadosBancarios = async () => {
      if (!token || !mostrar) {
        if (!mostrar) {
          // Se não deve mostrar, não notificar (ou notificar false se necessário)
          return;
        }
        return;
      }

      setLoadingDadosBancarios(true);
      try {
        const response = await getDadosBancariosUseCase.buscarPorColaborador(token);
        if (response.sucesso && response.retorno) {
          setDadosBancarios(response.retorno);
          const temDados = verificarDadosBancariosValidos(response.retorno);
          onDadosBancariosChange?.(temDados);
        } else {
          setDadosBancarios(null);
          onDadosBancariosChange?.(false);
        }
      } catch (error) {
        console.error("Erro ao carregar dados bancários:", error);
        setDadosBancarios(null);
        onDadosBancariosChange?.(false);
      } finally {
        setLoadingDadosBancarios(false);
      }
    };

    carregarDadosBancarios();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token, mostrar]);

  // Notificar mudanças nos dados bancários sempre que eles mudarem (após carregamento)
  useEffect(() => {
    if (mostrar && !loadingDadosBancarios && onDadosBancariosChange) {
      const temDados = verificarDadosBancariosValidos(dadosBancarios);
      onDadosBancariosChange(temDados);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dadosBancarios, mostrar, loadingDadosBancarios]);

  // Abrir modal de dados bancários
  const handleAbrirModalDadosBancarios = () => {
    if (dadosBancarios) {
      setFormDadosBancarios({
        codigoBancoTed: dadosBancarios.codigoBancoTed || "",
        agenciaTed: dadosBancarios.agenciaTed || "",
        agenciaDvTed: dadosBancarios.agenciaDvTed || "",
        contaTed: dadosBancarios.contaTed || "",
        contaDvTed: dadosBancarios.contaDvTed || "",
        chavePix: dadosBancarios.chavePix || "",
        tipoChavePix: dadosBancarios.tipoChavePix || "C",
        formaPagamento: dadosBancarios.formaPagamento || 1,
      });
    } else {
      setFormDadosBancarios({
        codigoBancoTed: "",
        agenciaTed: "",
        agenciaDvTed: "",
        contaTed: "",
        contaDvTed: "",
        chavePix: "",
        tipoChavePix: "C",
        formaPagamento: 1,
      });
    }
    setDadosBancariosErros([]);
    setModalDadosBancarios(true);
  };

  // Salvar dados bancários
  const handleSalvarDadosBancarios = async () => {
    // Validação: precisa ter pelo menos chave PIX ou dados bancários
    const temChavePix = formDadosBancarios.chavePix.trim() !== "";
    const temDadosBancarios = 
      formDadosBancarios.codigoBancoTed.trim() !== "" &&
      formDadosBancarios.agenciaTed.trim() !== "" &&
      formDadosBancarios.contaTed.trim() !== "";

    if (!temChavePix && !temDadosBancarios) {
      toast({
        title: "Validação",
        description: "É necessário cadastrar ao menos uma informação bancária. Inclua uma chave Pix ou os dados de uma conta bancária.",
        variant: "destructive",
      });
      return;
    }

    if (!token) {
      toast({
        title: "Erro de autenticação",
        description: "Por favor, faça login novamente.",
        variant: "destructive",
      });
      return;
    }

    setSalvandoDadosBancarios(true);
    setDadosBancariosErros([]);

    try {
      let response;
      if (dadosBancarios) {
        // Editar
        response = await editarDadosBancariosUseCase.execute(token, formDadosBancarios);
      } else {
        // Criar
        response = await criarDadosBancariosUseCase.execute(token, formDadosBancarios);
      }

      if (response.sucesso) {
        // Recarregar dados
        const dadosAtualizados = await getDadosBancariosUseCase.buscarPorColaborador(token);
        if (dadosAtualizados.sucesso && dadosAtualizados.retorno) {
          setDadosBancarios(dadosAtualizados.retorno);
          const temDados = verificarDadosBancariosValidos(dadosAtualizados.retorno);
          onDadosBancariosChange?.(temDados);
        } else {
          setDadosBancarios(null);
          onDadosBancariosChange?.(false);
        }
        setModalDadosBancarios(false);
        toast({
          title: "Sucesso",
          description: dadosBancarios ? "Dados bancários atualizados com sucesso!" : "Dados bancários cadastrados com sucesso!",
        });
      } else {
        toast({
          title: "Erro",
          description: response.mensagem || "Erro ao salvar dados bancários",
          variant: "destructive",
        });
      }
    } catch (error) {
      const err = error as Error & { erros?: string[] }
      if (err?.erros && Array.isArray(err.erros) && err.erros.length > 0) {
        setDadosBancariosErros(err.erros)
        toast({
          title: "Erros de validação",
          description: err.message || "Verifique os dados e tente novamente.",
          variant: "destructive",
        })
      } else {
        toast({
          title: "Erro",
          description: err?.message || "Erro ao salvar dados bancários",
          variant: "destructive",
        })
      }
    } finally {
      setSalvandoDadosBancarios(false);
    }
  };

  if (!mostrar) {
    return null;
  }

  return (
    <>
      <Card className="rounded-lg shadow-none border border-border/50">
        <CardContent className="p-3 md:p-4">
          {loadingDadosBancarios ? (
            <div className="text-xs text-muted-foreground">Carregando dados bancários...</div>
          ) : dadosBancarios ? (
            <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-3">
              <div className="flex-1 space-y-1.5 min-w-0">
                <h3 className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
                  Dados Bancários para recebimento
                </h3>
                {dadosBancarios.codigoBancoTed && (
                  <div className="flex flex-wrap items-center gap-x-3 gap-y-1 text-xs">
                    <div className="flex items-center gap-1.5">
                      <span className="text-muted-foreground">Código Banco:</span>
                      <span className="font-medium text-foreground">{dadosBancarios.codigoBancoTed}</span>
                    </div>
                    <div className="flex items-center gap-1.5">
                      <span className="text-muted-foreground">Agência:</span>
                      <span className="font-medium text-foreground">
                        {dadosBancarios.agenciaTed}
                        {dadosBancarios.agenciaDvTed ? `-${dadosBancarios.agenciaDvTed}` : ""}
                      </span>
                    </div>
                    <div className="flex items-center gap-1.5">
                      <span className="text-muted-foreground">Conta Corrente:</span>
                      <span className="font-medium text-foreground">
                        {dadosBancarios.contaTed}
                        {dadosBancarios.contaDvTed ? `-${dadosBancarios.contaDvTed}` : ""}
                      </span>
                    </div>
                  </div>
                )}
                {dadosBancarios.chavePix && (
                  <div className="flex items-center gap-2 text-xs">
                    <span className="text-muted-foreground">Chave PIX:</span>
                    <span className="font-medium text-foreground break-all">{dadosBancarios.chavePix}</span>
                  </div>
                )}
              </div>
              <Button
                variant="primary"
                size="sm"
                onClick={handleAbrirModalDadosBancarios}
                className="w-full md:w-auto flex-shrink-0 gap-2"
              >
                <Edit className="h-4 w-4" />
                Editar dados bancários
              </Button>
            </div>
          ) : (
            <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-3">
              <h3 className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
                Dados Bancários para recebimento
              </h3>
              <Button
                variant="primary"
                size="sm"
                onClick={handleAbrirModalDadosBancarios}
                className="w-full md:w-auto flex-shrink-0"
              >
                Informar dados bancários
              </Button>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Modal de Dados Bancários */}
      <Dialog open={modalDadosBancarios} onOpenChange={setModalDadosBancarios}>
        <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <div className="flex items-center gap-2">
              <DollarSign className="h-5 w-5" />
              <DialogTitle>Editar dados bancários</DialogTitle>
            </div>
            <DialogDescription>
              É necessário cadastrar ao menos uma informação bancária. Inclua uma chave Pix ou os dados de uma conta bancária.
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-6 py-4">
            {/* Seção Chave PIX */}
            <div className="space-y-4">
              <h4 className="font-medium">Chave PIX</h4>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="tipoChavePix">Tipo chave PIX</Label>
                  <Select
                    value={formDadosBancarios.tipoChavePix}
                    onValueChange={(value) =>
                      setFormDadosBancarios((prev) => ({
                        ...prev,
                        tipoChavePix: value as 'C' | 'J' | 'E' | 'T' | 'R',
                      }))
                    }
                  >
                    <SelectTrigger>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {tipoChavePixOptions.map((option) => (
                        <SelectItem key={option.value} value={option.value}>
                          {option.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="chavePix">Chave PIX</Label>
                  <Input
                    id="chavePix"
                    placeholder="Chave PIX"
                    value={formDadosBancarios.chavePix}
                    onChange={(e) =>
                      setFormDadosBancarios((prev) => ({
                        ...prev,
                        chavePix: e.target.value,
                      }))
                    }
                  />
                </div>
              </div>
            </div>

            {/* Seção Dados Bancários */}
            <div className="space-y-4">
              <h4 className="font-medium">Dados Bancários</h4>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="codigoBanco">Cod. banco</Label>
                  <Input
                    id="codigoBanco"
                    placeholder="001"
                    value={formDadosBancarios.codigoBancoTed}
                    onChange={(e) =>
                      setFormDadosBancarios((prev) => ({
                        ...prev,
                        codigoBancoTed: e.target.value,
                      }))
                    }
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="agencia">Agência</Label>
                  <Input
                    id="agencia"
                    placeholder="0011"
                    value={formDadosBancarios.agenciaTed}
                    onChange={(e) =>
                      setFormDadosBancarios((prev) => ({
                        ...prev,
                        agenciaTed: e.target.value,
                      }))
                    }
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="agenciaDv">Dígito</Label>
                  <Input
                    id="agenciaDv"
                    placeholder="1"
                    maxLength={1}
                    value={formDadosBancarios.agenciaDvTed}
                    onChange={(e) =>
                      setFormDadosBancarios((prev) => ({
                        ...prev,
                        agenciaDvTed: e.target.value,
                      }))
                    }
                  />
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div className="space-y-2 md:col-span-2">
                  <Label htmlFor="conta">Conta Corrente</Label>
                  <Input
                    id="conta"
                    placeholder="000111"
                    value={formDadosBancarios.contaTed}
                    onChange={(e) =>
                      setFormDadosBancarios((prev) => ({
                        ...prev,
                        contaTed: e.target.value,
                      }))
                    }
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="contaDv">Dígito</Label>
                  <Input
                    id="contaDv"
                    placeholder="1"
                    maxLength={1}
                    value={formDadosBancarios.contaDvTed}
                    onChange={(e) =>
                      setFormDadosBancarios((prev) => ({
                        ...prev,
                        contaDvTed: e.target.value,
                      }))
                    }
                  />
                </div>
              </div>
            </div>

            {/* Forma de Recebimento */}
            <div className="space-y-2">
              <Label htmlFor="formaPagamento">Forma de recebimento</Label>
              <Select
                value={formDadosBancarios.formaPagamento.toString()}
                onValueChange={(value) =>
                  setFormDadosBancarios((prev) => ({
                    ...prev,
                    formaPagamento: Number(value) as 1 | 2,
                  }))
                }
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="1">PIX</SelectItem>
                  <SelectItem value="2">TED</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          {dadosBancariosErros.length > 0 && (
            <div className="rounded-lg border border-destructive/50 bg-destructive/10 p-4">
              <p className="text-sm font-medium text-destructive mb-2">Erros de validação:</p>
              <ul className="list-disc list-inside space-y-1 text-sm text-foreground">
                {dadosBancariosErros.map((erro, index) => (
                  <li key={index}>{erro}</li>
                ))}
              </ul>
            </div>
          )}

          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setModalDadosBancarios(false)}
              disabled={salvandoDadosBancarios}
            >
              Cancelar
            </Button>
            <Button
              variant="primary"
              onClick={handleSalvarDadosBancarios}
              disabled={salvandoDadosBancarios}
            >
              {salvandoDadosBancarios ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin" />
                  Salvando...
                </>
              ) : (
                "Salvar"
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
}


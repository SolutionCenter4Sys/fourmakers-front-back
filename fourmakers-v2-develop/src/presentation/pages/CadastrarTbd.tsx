import { useState, useEffect, useCallback } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { ArrowLeft, Loader2 } from "@/components/ui/system-icons";
import { useAppSelector } from "@/app/store/hooks";
import { container } from "@/core/di/container";
import { InserirTbdUseCase } from "@domain/usecases/InserirTbdUseCase";
import { AtualizarTbdUseCase } from "@domain/usecases/AtualizarTbdUseCase";
import { ListarTbdUseCase } from "@domain/usecases/ListarTbdUseCase";
import { ListarDiretoriasUseCase } from "@domain/usecases/ListarDiretoriasUseCase";
import { ListarDepartamentosUseCase } from "@domain/usecases/ListarDepartamentosUseCase";
import type { Tbd } from "@domain/entities/Tbd";
import { toast } from "sonner";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

const formSchema = z.object({
  descricao: z.string().min(1, "Campo obrigatório").max(100, "Máximo 100 caracteres"),
  codDiretoria: z.string().min(1, "Campo obrigatório"),
  codDepartamento: z.string().min(1, "Campo obrigatório"),
  codGestor: z.string().optional(),
});

type FormValues = z.infer<typeof formSchema>;

import type { DiretoriaColaborador } from "@domain/entities/Diretoria";
import type { Departamento } from "@domain/entities/Departamento";

export default function CadastrarTbd() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const { token, user } = useAppSelector((state) => state.auth);
  
  // Verificar se o usuário tem a funcionalidade CRIAR_RECURSO_TBD ativa
  const temCriarRecursoTbd = user?.funcionalidadeSistema?.some(
    (func) => func.descricao === "CRIAR_RECURSO_TBD" && func.ativo === true
  ) ?? false;

  // Redirecionar se não tiver permissão
  useEffect(() => {
    if (!temCriarRecursoTbd) {
      navigate("/mapa-alocacao");
    }
  }, [temCriarRecursoTbd, navigate]);

  const [loading, setLoading] = useState(false);
  const [carregandoDados, setCarregandoDados] = useState(false);
  const [diretorias, setDiretorias] = useState<DiretoriaColaborador[]>([]);
  const [departamentos, setDepartamentos] = useState<Departamento[]>([]);
  const [tbdEditando, setTbdEditando] = useState<Tbd | null>(null);
  const [loadingDiretorias, setLoadingDiretorias] = useState(false);
  const [loadingDepartamentos, setLoadingDepartamentos] = useState(false);

  const isEditando = id && id !== "novo";

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      descricao: "",
      codDiretoria: "",
      codDepartamento: "",
      codGestor: "",
    },
  });

  const descricaoValue = form.watch("descricao");

  // Carregar diretorias
  const carregarDiretorias = useCallback(async () => {
    if (!token) return;

    try {
      setLoadingDiretorias(true);
      const useCase = container.resolve(ListarDiretoriasUseCase);
      const response = await useCase.execute(token);
      const diretoriasData = response.diretoriaColaborador || [];
      // Remover duplicatas usando Set
      const seen = new Set<string>();
      const diretoriasUnicas = diretoriasData.filter((diretoria) => {
        if (seen.has(diretoria.cod)) return false;
        seen.add(diretoria.cod);
        return true;
      });
      setDiretorias(diretoriasUnicas);
    } catch (error) {
      console.error("Erro ao carregar diretorias:", error);
    } finally {
      setLoadingDiretorias(false);
    }
  }, [token]);

  // Carregar departamentos
  const carregarDepartamentos = useCallback(async () => {
    if (!token) return;

    try {
      setLoadingDepartamentos(true);
      const useCase = container.resolve(ListarDepartamentosUseCase);
      const response = await useCase.execute(token, { codigoDiretoria: "" });
      const departamentosData = response.retorno || [];
      // Remover duplicatas usando Set
      const seen = new Set<string>();
      const departamentosUnicos = departamentosData.filter((departamento) => {
        if (seen.has(departamento.cod)) return false;
        seen.add(departamento.cod);
        return true;
      });
      setDepartamentos(departamentosUnicos);
    } catch (error) {
      console.error("Erro ao carregar departamentos:", error);
    } finally {
      setLoadingDepartamentos(false);
    }
  }, [token]);

  // Carregar TBD para edição
  const carregarTbd = useCallback(async () => {
    if (!token || !isEditando) return;

    try {
      setCarregandoDados(true);
      const useCase = container.resolve(ListarTbdUseCase);
      const tbds = await useCase.execute(token);
      const tbd = tbds.find((t) => t.codTbdAlocado === Number(id));

      if (tbd) {
        setTbdEditando(tbd);
      } else {
        toast.error("TBD não encontrado");
        navigate("/mapa-alocacao?tab=tbd");
      }
    } catch (error) {
      console.error("Erro ao carregar TBD:", error);
      toast.error("Erro ao carregar TBD");
    } finally {
      setCarregandoDados(false);
    }
  }, [token, id, isEditando, navigate]);

  useEffect(() => {
    carregarDiretorias();
    carregarDepartamentos();
    if (isEditando) {
      carregarTbd();
    }
  }, [carregarDiretorias, carregarDepartamentos, isEditando, carregarTbd]);

  // Preencher formulário quando TBD e dados estiverem carregados
  useEffect(() => {
    if (tbdEditando && diretorias.length > 0 && departamentos.length > 0) {
      form.reset({
        descricao: tbdEditando.descricao,
        codDiretoria: tbdEditando.codDiretoria || "",
        codDepartamento: tbdEditando.codDepartamento || "",
        codGestor: tbdEditando.codGestor || "",
      });
    }
  }, [tbdEditando, diretorias, departamentos, form]);

  // Não renderizar se não tiver permissão
  if (!temCriarRecursoTbd) {
    return null;
  }

  const onSubmit = async (data: FormValues) => {
    if (!token) return;

    try {
      setLoading(true);

      const diretoriaSelecionada = diretorias.find(
        (d) => d.cod === data.codDiretoria
      );
      const departamentoSelecionado = departamentos.find(
        (d) => d.cod === data.codDepartamento
      );

      if (isEditando && tbdEditando) {
        const useCase = container.resolve(AtualizarTbdUseCase);
        const response = await useCase.execute(token, {
          codTbdAlocado: tbdEditando.codTbdAlocado,
          descricao: data.descricao.trim(),
          codDiretoria: data.codDiretoria || "",
          descricaoDiretoria: diretoriaSelecionada?.diretoria || "",
          codDepartamento: data.codDepartamento || "",
          descricaoDepartamento: departamentoSelecionado?.departamento || "",
          codGestor: data.codGestor || "",
        });

        // A API retorna o objeto TBD atualizado diretamente em caso de sucesso
        if (response && response.codTbdAlocado) {
          toast.success("TBD atualizado com sucesso!");
          navigate("/mapa-alocacao?tab=tbd");
        } else {
          toast.error("Erro ao atualizar TBD");
        }
      } else {
        const useCase = container.resolve(InserirTbdUseCase);
        const response = await useCase.execute(token, {
          descricao: data.descricao.trim(),
          codDiretoria: data.codDiretoria || "",
          descricaoDiretoria: diretoriaSelecionada?.diretoria || "",
          codDepartamento: data.codDepartamento || "",
          descricaoDepartamento: departamentoSelecionado?.departamento || "",
          codGestor: data.codGestor || "",
        });

        // A API retorna o objeto TBD diretamente em caso de sucesso
        if (response && response.codTbdAlocado) {
          toast.success("TBD criado com sucesso!");
          navigate("/mapa-alocacao?tab=tbd");
        } else {
          toast.error("Erro ao criar TBD");
        }
      }
    } catch (error: any) {
      console.error("Erro ao salvar TBD:", error);
      toast.error(error.message || "Erro ao salvar TBD");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto p-6 space-y-6">
      <div className="flex items-center gap-4">
        <Button
          variant="ghost"
          size="icon"
          onClick={() => navigate("/mapa-alocacao?tab=tbd")}
        >
          <ArrowLeft className="h-4 w-4" />
        </Button>
        <h1 className="page-title">
          {isEditando ? "Editar TBD" : "Cadastrar TBD"}
        </h1>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>
            {isEditando ? "Editar TBD" : "Cadastrar TBD"}
          </CardTitle>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          {isEditando && tbdEditando && (
            <FormItem>
              <FormLabel>Cód</FormLabel>
              <FormControl>
                <Input
                  value={tbdEditando.codTbdAlocado}
                  disabled
                  readOnly
                />
              </FormControl>
            </FormItem>
          )}

          <FormField
            control={form.control}
            name="descricao"
            render={({ field }) => (
              <FormItem>
                <FormLabel>
                  Nome{!isEditando && "*"}
                </FormLabel>
                <FormControl>
                  <Input
                    placeholder="Crie um nome para o TBD"
                    maxLength={100}
                    {...field}
                  />
                </FormControl>
                <div className="flex justify-between items-center">
                  <FormMessage />
                  <span className="text-xs text-muted-foreground">
                    {descricaoValue?.length || 0}/100
                  </span>
                </div>
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="codDiretoria"
            render={({ field }) => (
              <FormItem>
                <FormLabel>
                  Unidade{!isEditando && "*"}
                </FormLabel>
                <FormControl>
                  <Select
                    onValueChange={field.onChange}
                    value={field.value}
                    disabled={loadingDiretorias}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Selecione uma Unidade" />
                    </SelectTrigger>
                    <SelectContent>
                      {diretorias.map((diretoria) => (
                        <SelectItem
                          key={`diretoria-${diretoria.cod}`}
                          value={diretoria.cod}
                        >
                          {diretoria.diretoria}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="codDepartamento"
            render={({ field }) => (
              <FormItem>
                <FormLabel>
                  Departamento{!isEditando && "*"}
                </FormLabel>
                <FormControl>
                  <Select
                    onValueChange={field.onChange}
                    value={field.value}
                    disabled={loadingDepartamentos}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Selecione..." />
                    </SelectTrigger>
                    <SelectContent>
                      {departamentos.map((departamento) => (
                        <SelectItem
                          key={`departamento-${departamento.cod}`}
                          value={departamento.cod}
                        >
                          {departamento.departamento}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <div className="flex justify-end gap-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => navigate("/mapa-alocacao?tab=tbd")}
              disabled={loading}
            >
              Cancelar
            </Button>
            <Button type="submit" disabled={loading || carregandoDados}>
              {loading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              Salvar
            </Button>
            </div>
          </form>
        </Form>
        </CardContent>
      </Card>
    </div>
  );
}

import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent } from '@/components/ui/card';
import { Spinner } from '@/components/ui/spinner';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Table,
  TableBody,
  TableCell,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import { PageBreadcrumb, CountBadge } from '@presentation/components/common';
import { ArrowLeft, ChevronRight, Search, AssignmentInd, Calendar, Clock } from '@/components/ui/system-icons';
import { useAppSelector } from '@app/store/hooks';
import { container } from '@core/di/container';
import type { PessoaCadastradaPorColaborador } from '@domain/entities/GestaoVagasCandidatos';
import { BuscarPessoasCadastradasPorColaboradorUseCase } from '@domain/usecases/BuscarPessoasCadastradasPorColaboradorUseCase';
import { formatDatePtBr } from '@presentation/hooks/recrutamento'
import { toast } from 'sonner';

const NAO_INFORMADO = 'Não informado';
const LIMITE_OPCOES = [5, 10, 20, 50, 100] as const;

export default function TalentosInscritos() {
  const navigate = useNavigate();
  const token = useAppSelector((s) => s.auth.token);
  const codColaborador = useAppSelector((s) => s.auth.codColaborador);

  const [busca, setBusca] = useState('');
  const [limite, setLimite] = useState(10);
  const [cursor, setCursor] = useState(0);
  const [loading, setLoading] = useState(false);
  const [lista, setLista] = useState<PessoaCadastradaPorColaborador[]>([]);

  const buscarPessoasUseCase = container.resolve(BuscarPessoasCadastradasPorColaboradorUseCase);

  const buscar = useCallback(async () => {
    if (!token || !codColaborador) {
      toast.error('Sessão inválida. Faça login novamente.');
      return;
    }
    setLoading(true);
    try {
      const dados = await buscarPessoasUseCase.execute(token, codColaborador, {
        busca: busca.trim(),
        cursor,
        limite,
      });
      setLista(dados);
    } catch (e) {
      toast.error('Erro ao carregar talentos inscritos.');
      setLista([]);
    } finally {
      setLoading(false);
    }
  }, [buscarPessoasUseCase, token, codColaborador, busca, cursor, limite]);

  useEffect(() => {
    buscar();
  }, [buscar]);

  const limparFiltros = () => {
    setBusca('');
    setCursor(0);
  };

  const temFiltros = busca.trim() !== '';

  const total = lista.length;
  const inicio = total === 0 ? 0 : cursor + 1;
  const fim = cursor + total;
  const textoPaginacao = total === 0 ? '0 - 0 de 0' : `${inicio} - ${fim} de ${total}`;
  const podeAnterior = cursor > 0;
  const podeProximo = lista.length === limite;

  return (
    <div className="container mx-auto p-4 space-y-4">
      <PageBreadcrumb
        items={[
          { label: 'Recrutamento', href: '/recrutamento' },
          { label: 'Talentos cadastrados' },
        ]}
      />

      <div className="flex items-center gap-3">
        <Button
          variant="ghost"
          size="icon"
          aria-label="Voltar"
          onClick={() => navigate('/recrutamento')}
        >
          <ArrowLeft className="h-5 w-5" />
        </Button>
        <h1 className="text-xl font-semibold">Talentos cadastrados</h1>
      </div>

      <Card>
        <CardContent className="p-6 space-y-4">
          <div>
            <h2 className="text-lg font-medium">Histórico de talentos cadastrados</h2>
            <p className="text-sm text-muted-foreground">
              Lista de talentos que cadastrei no Fourmakers.
            </p>
          </div>

          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
            <div className="flex flex-col sm:flex-row gap-2 sm:items-center">
              <div className="flex gap-2">
                <Input
                  placeholder="Busca"
                  value={busca}
                  onChange={(e) => setBusca(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && buscar()}
                  className="max-w-[240px]"
                />
                <Button variant="primary" size="icon" onClick={() => buscar()} aria-label="Buscar">
                  <Search className="h-5 w-5" />
                </Button>
              </div>
              {temFiltros && (
                <button
                  type="button"
                  onClick={limparFiltros}
                  className="text-sm text-primary hover:underline text-left sm:text-center"
                >
                  Limpar Filtros
                </button>
              )}
            </div>
            {false && (
              <Button
                variant="primary"
                onClick={() => toast.info('Banco de talentos em breve.')}
                className="w-full sm:w-auto"
              >
                Banco de talentos
              </Button>
            )}
          </div>

          <div className="rounded-md border overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/50">
                  <TableCell className="font-medium">Nome</TableCell>
                  <TableCell className="font-medium">Data do cadastro</TableCell>
                  <TableCell className="font-medium">Candidatura em vagas</TableCell>
                  <TableCell className="font-medium">Criado por</TableCell>
                  <TableCell className="font-medium">Origem</TableCell>
                </TableRow>
              </TableHeader>
              <TableBody>
                {loading ? (
                  <TableRow>
                    <TableCell colSpan={5} className="text-center py-8">
                      <div className="flex flex-col items-center justify-center gap-2 text-muted-foreground">
                        <Spinner size={24} aria-hidden />
                        <span>Carregando...</span>
                      </div>
                    </TableCell>
                  </TableRow>
                ) : lista.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={5} className="text-center py-8 text-muted-foreground">
                      Nenhum talento encontrado.
                    </TableCell>
                  </TableRow>
                ) : (
                  lista.map((item) => (
                    <TableRow key={item.codigoInternoColaborador}>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Tooltip>
                            <TooltipTrigger asChild>
                              <Button
                                variant="ghost"
                                size="icon"
                                className="h-11 w-11 shrink-0 rounded-full"
                                aria-label="Ver Perfil"
                                onClick={() => {
                                  const codigoColaborador = item.codigoInternoColaborador?.trim();
                                  if (!codigoColaborador) {
                                    toast.info('Código do colaborador não disponível para abrir o perfil.');
                                    return;
                                  }
                                  navigate(`/curriculoProfissional?cpf=${encodeURIComponent(codigoColaborador)}`, {
                                    state: { from: '/recrutamento/talentosInscritos' },
                                  });
                                }}
                              >
                                <AssignmentInd className="h-5 w-5" />
                              </Button>
                            </TooltipTrigger>
                            <TooltipContent>Ver Perfil</TooltipContent>
                          </Tooltip>
                          <span>{item.nome || NAO_INFORMADO}</span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Calendar className="h-4 w-4 shrink-0 text-muted-foreground" />
                          <span>
                            {item.dataDoCadastro
                              ? formatDatePtBr(item.dataDoCadastro)
                              : NAO_INFORMADO}
                          </span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <span className="text-sm">
                            {item.possuiCandidatura ? 'Ver candidaturas' : 'Não possui candidaturas'}
                          </span>
                          {item.possuiCandidatura && (
                            <div className="relative inline-flex items-start">
                              <Tooltip>
                                <TooltipTrigger asChild>
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    className="h-9 w-9 shrink-0"
                                    aria-label={`Ver histórico do candidato${item.nome ? ` ${item.nome}` : ''}`}
                                    onClick={() =>
                                      navigate(
                                        `/recrutamento/historico-candidato/${encodeURIComponent(item.codigoInternoColaborador)}`
                                      )
                                    }
                                  >
                                    <Clock className="h-5 w-5" />
                                  </Button>
                                </TooltipTrigger>
                                <TooltipContent>Histórico do candidato</TooltipContent>
                              </Tooltip>
                              <span className="pointer-events-none absolute -top-0.5 -right-0.5 z-10">
                                <CountBadge
                                  count={item.candidaturas?.length ?? 0}
                                  className="!min-w-[1rem] !h-4 !p-0 !text-[10px]"
                                />
                              </span>
                            </div>
                          )}
                        </div>
                      </TableCell>
                      <TableCell>{item.nomeCadastrante || NAO_INFORMADO}</TableCell>
                      <TableCell>{NAO_INFORMADO}</TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </div>

          <div className="flex flex-wrap items-center justify-end gap-4 pt-2">
            <div className="flex items-center gap-2">
              <span className="text-sm text-muted-foreground">Linhas por página:</span>
              <Select
                value={String(limite)}
                onValueChange={(v) => {
                  setLimite(Number(v));
                  setCursor(0);
                }}
              >
                <SelectTrigger className="h-8 w-[70px]">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {LIMITE_OPCOES.map((n) => (
                    <SelectItem key={n} value={String(n)}>
                      {n}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <span className="text-sm text-muted-foreground">{textoPaginacao}</span>
            <div className="flex gap-1">
              <Button
                variant="outline"
                size="icon"
                className="h-8 w-8"
                disabled={!podeAnterior}
                onClick={() => setCursor((c) => Math.max(0, c - limite))}
                aria-label="Página anterior"
              >
                <ArrowLeft className="h-4 w-4" />
              </Button>
              <Button
                variant="outline"
                size="icon"
                className="h-8 w-8"
                disabled={!podeProximo}
                onClick={() => setCursor((c) => c + limite)}
                aria-label="Próxima página"
              >
                <ChevronRight className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

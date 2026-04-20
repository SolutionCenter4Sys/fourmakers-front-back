import { useEffect, useMemo, useState } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Spinner } from '@/components/ui/spinner';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import type { CandidatoListItem } from '@domain/entities/CandidatoListItem';
import type { VagaListItem } from '@domain/entities/VagaListItem';

interface SimulatorIdentifiersModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  vagas: VagaListItem[];
  candidatos: CandidatoListItem[];
  selectedVagaId: string;
  selectedCandidaturaId: string;
  loadingVagas: boolean;
  loadingCandidatos: boolean;
  loadingCandidaturaDetails?: boolean;
  /** Nome do candidato quando carregado por URL (lista de candidatos pode estar vazia) */
  candidateNameFromDetails?: string | null;
  /** Código do candidato quando carregado por URL (para opção sintética no dropdown) */
  selectedCandidateId?: string;
  onSelectVaga: (vagaId: string) => void;
  onSelectCandidato: (params: { candidaturaId: string; candidateId: string }) => void;
}

const renderVagaLabel = (vaga: VagaListItem) => {
  const codigo = vaga.codigo != null ? `${vaga.codigo}` : 'sem código';
  const titulo = vaga.titulo ?? 'Título não disponível';
  return `${codigo} - ${titulo}`;
};

/** Prioriza emailUsuario; se null/vazio/"null", usa emailAlternativo. */
function getEmailExibicao(item: CandidatoListItem): string {
  const u = item.emailUsuario;
  if (u != null && String(u).trim() !== '' && String(u).trim().toLowerCase() !== 'null')
    return String(u).trim();
  const alt = item.emailAlternativo;
  if (alt != null && String(alt).trim() !== '')
    return String(alt).trim();
  return 'Sem e-mail';
}

export const SimulatorIdentifiersModal = ({
  open,
  onOpenChange,
  vagas,
  candidatos,
  selectedVagaId,
  selectedCandidaturaId,
  loadingVagas,
  loadingCandidatos,
  loadingCandidaturaDetails,
  candidateNameFromDetails,
  selectedCandidateId,
  onSelectVaga,
  onSelectCandidato,
}: SimulatorIdentifiersModalProps) => {
  const [vagaSearch, setVagaSearch] = useState('');
  const [candidatoSearch, setCandidatoSearch] = useState('');

  useEffect(() => {
    setCandidatoSearch('');
  }, [selectedVagaId]);

  const normalizedSearch = (value: string) => value.trim().toLowerCase();

  const filteredVagas = useMemo(() => {
    const query = normalizedSearch(vagaSearch);
    if (!query) {
      return vagas;
    }
    return vagas.filter((vaga) => {
      const matches = [
        vaga.titulo,
        vaga.nomeGestor,
        vaga.nomeCliente,
        vaga.codigoCliente,
        vaga.codigo != null ? String(vaga.codigo) : null,
      ];
      return matches.some((entry) => entry?.toLowerCase().includes(query));
    });
  }, [vagaSearch, vagas]);

  const filteredCandidatos = useMemo(() => {
    const query = normalizedSearch(candidatoSearch);
    if (!query) {
      return candidatos;
    }
    return candidatos.filter((item) => {
      const matches = [
        item.nome,
        item.codigo,
        getEmailExibicao(item),
        item.descricaoStatusCandidatura,
      ];
      return matches.some((entry) => entry?.toLowerCase().includes(query));
    });
  }, [candidatoSearch, candidatos]);

  // Incluir candidato carregado por URL na lista de opções se ainda não estiver (para o Select mostrar selecionado)
  const candidateOptions = useMemo(() => {
    const hasSelectedInList = filteredCandidatos.some(
      (item) => item.idCandidatura === selectedCandidaturaId,
    );
    if (
      selectedCandidaturaId &&
      candidateNameFromDetails?.trim() &&
      selectedCandidateId &&
      !hasSelectedInList
    ) {
      return [
        {
          idCandidatura: selectedCandidaturaId,
          nome: candidateNameFromDetails.trim(),
          codigo: selectedCandidateId,
          emailUsuario: null,
          descricaoStatusCandidatura: null,
        } as CandidatoListItem,
        ...filteredCandidatos,
      ];
    }
    return filteredCandidatos;
  }, [
    filteredCandidatos,
    selectedCandidaturaId,
    candidateNameFromDetails,
    selectedCandidateId,
  ]);
  const handleVagaChange = (value: string) => {
    if (!value || value === 'loading' || value === 'empty') {
      return;
    }
    onSelectVaga(value);
  };

  const selectedVagaLabel = useMemo(() => {
    if (!selectedVagaId) {
      return '';
    }
    const vaga = vagas.find((item) => item.id === selectedVagaId);
    return vaga ? renderVagaLabel(vaga) : '';
  }, [selectedVagaId, vagas]);

  const selectedCandidateLabel = useMemo(() => {
    if (!selectedCandidaturaId) {
      return '';
    }
    const candidate = candidateOptions.find(
      (item) => item.idCandidatura === selectedCandidaturaId,
    );
    return candidate ? candidate.nome : candidateNameFromDetails?.trim() ?? '';
  }, [selectedCandidaturaId, candidateOptions, candidateNameFromDetails]);

  const handleCandidateChange = (value: string) => {
    if (!value || value === 'loading' || value === 'empty') {
      return;
    }
    const selected = candidateOptions.find((item) => item.idCandidatura === value);
    if (!selected) {
      return;
    }
    onSelectCandidato({ candidaturaId: selected.idCandidatura, candidateId: selected.codigo });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl pb-6">
        <DialogHeader>
          <DialogTitle>Carregar identificadores</DialogTitle>
          <DialogDescription>
            Escolha a vaga e o candidato/candidatura desejados para carregar automaticamente os dados no formulário.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6">
          <div className="space-y-3">
            <div className="flex items-center gap-2">
              <Label className="text-sm font-medium text-foreground">Vaga</Label>
              {loadingVagas && (
                <div className="flex items-center gap-1 text-[11px] text-muted-foreground">
                  <Spinner size={14} className="text-muted-foreground" />
                  Buscando vagas
                </div>
              )}
            </div>
            <Select value={selectedVagaId || ''} onValueChange={handleVagaChange}>
              <SelectTrigger className="w-full rounded-lg border border-input bg-input px-3 py-2 text-sm focus:ring-2 focus:ring-ring focus:ring-offset-2">
                <SelectValue placeholder={loadingVagas ? 'Carregando vagas...' : 'Selecione uma vaga'}>
                  {selectedVagaLabel}
                </SelectValue>
              </SelectTrigger>
              <SelectContent>
                {!loadingVagas && (
                  <div className="px-3 pb-2 pt-3">
                    <Input
                      value={vagaSearch}
                      onChange={(event) => setVagaSearch(event.target.value)}
                      placeholder="Buscar por título, gestor ou código"
                      className="h-8 text-sm"
                    />
                  </div>
                )}

                {loadingVagas ? (
                  <SelectItem value="loading" disabled>
                    Carregando vagas...
                  </SelectItem>
                ) : filteredVagas.length ? (
                  filteredVagas.map((vaga) => (
                    <SelectItem
                      key={vaga.id}
                      value={vaga.id}
                      className="data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground data-[state=checked]:[&_.text-muted-foreground]:text-primary-foreground/90 data-[highlighted=true]:bg-btnGhostHover data-[highlighted=true]:dark:bg-white/5"
                    >
                      <span className="sr-only">{renderVagaLabel(vaga)}</span>
                      <div className="flex flex-col text-left text-sm leading-tight">
                        <span className="font-semibold">{renderVagaLabel(vaga)}</span>
                        <span className="text-xs">
                          {vaga.nomeGestor
                            ? `Gestor: ${vaga.nomeGestor}`
                            : vaga.nomeCliente
                              ? `Cliente: ${vaga.nomeCliente}`
                              : vaga.codigoCliente
                                ? `Cliente: ${vaga.codigoCliente}`
                                : 'Sem gestor/cliente'}
                        </span>
                      </div>
                    </SelectItem>
                  ))
                ) : (
                  <SelectItem value="empty" disabled>
                    Nenhuma vaga encontrada
                  </SelectItem>
                )}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-3">
            <div className="flex items-center gap-2">
              <Label className="text-sm font-medium text-foreground">Candidato / Candidatura</Label>
              {loadingCandidatos && (
                <div className="flex items-center gap-1 text-[11px] text-muted-foreground">
                  <Spinner size={14} className="text-muted-foreground" />
                  Buscando candidatos
                </div>
              )}
            </div>
            <Select
              value={selectedCandidaturaId || ''}
              onValueChange={handleCandidateChange}
              disabled={!selectedVagaId || loadingCandidatos}
            >
              <SelectTrigger
                className="w-full rounded-lg border border-input bg-input px-3 py-2 text-sm focus:ring-2 focus:ring-ring focus:ring-offset-2"
                disabled={!selectedVagaId || loadingCandidatos}
              >
                <SelectValue
                  placeholder={
                    !selectedVagaId
                      ? 'Selecione uma vaga primeiro'
                      : loadingCandidatos
                        ? 'Carregando candidatos...'
                        : 'Selecione um candidato / candidatura'
                  }
                  className={(!selectedVagaId || loadingCandidatos) ? 'text-muted-foreground' : undefined}
                >
                  {selectedCandidateLabel}
                </SelectValue>
              </SelectTrigger>
              <SelectContent>
                {!loadingCandidatos && selectedVagaId && (
                  <div className="px-3 pb-2 pt-3">
                    <Input
                      value={candidatoSearch}
                      onChange={(event) => setCandidatoSearch(event.target.value)}
                      placeholder="Buscar por nome, código, e-mail ou status"
                      className="h-8 text-sm"
                    />
                  </div>
                )}

                {loadingCandidatos ? (
                  <SelectItem value="loading" disabled>
                    Carregando candidatos...
                  </SelectItem>
                ) : candidateOptions.length ? (
                  candidateOptions.map((item) => (
                    <SelectItem
                      key={item.idCandidatura}
                      value={item.idCandidatura}
                      className="data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground data-[state=checked]:[&_.text-muted-foreground]:text-primary-foreground/90 data-[highlighted=true]:bg-btnGhostHover data-[highlighted=true]:dark:bg-white/5"
                    >
                      <span className="sr-only">{item.nome}</span>
                      <div className="flex flex-col text-left text-sm leading-tight">
                        <span className="font-semibold">{item.nome}</span>
                        <span className="text-xs">
                          {getEmailExibicao(item)}
                        </span>
                        <span className="text-xs">
                          Status: {item.descricaoStatusCandidatura || 'Status indefinido'}
                        </span>
                      </div>
                    </SelectItem>
                  ))
                ) : (
                  <SelectItem value="empty" disabled>
                    Nenhum candidato encontrado para esta vaga
                  </SelectItem>
                )}
              </SelectContent>
            </Select>
            <p className="text-xs text-muted-foreground">
              Ao selecionar a vaga e a candidatura, os dados são carregados automaticamente para o formulário.
            </p>
          </div>

          {loadingCandidaturaDetails && (
            <p className="text-xs text-muted-foreground">Carregando detalhes da candidatura...</p>
          )}
        </div>

        <DialogFooter className="mt-4">
          <Button variant="ghost" className="w-full" onClick={() => onOpenChange(false)}>
            Fechar
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

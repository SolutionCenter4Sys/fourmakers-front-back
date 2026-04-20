import { useState, useEffect } from "react";
import type { Equipe, ColaboradorBook, ColaboradorDetalhes, FeriasInfo } from "@shared/types/bookColaborador";
import { mockEquipe, mockColaboradores, mockColaboradorDetalhes, mockFeriasInfo } from "@data/mocks/bookColaboradorMock";

export const useEquipe = (equipeId: number = 1) => {
  const [equipe, setEquipe] = useState<Equipe | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Simular chamada de API
    setTimeout(() => {
      setEquipe(mockEquipe);
      setLoading(false);
    }, 500);
  }, [equipeId]);

  return { equipe, loading };
};

export const useColaboradoresEquipe = (_equipeId: number = 1) => {
  const [colaboradores, setColaboradores] = useState<ColaboradorBook[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Simular chamada de API
    setTimeout(() => {
      setColaboradores(mockColaboradores);
      setLoading(false);
    }, 500);
  }, [_equipeId]);

  return { colaboradores, loading };
};

export const useColaboradorDetalhes = (codColaborador: string) => {
  const [detalhes, setDetalhes] = useState<ColaboradorDetalhes | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Simular chamada de API
    setTimeout(() => {
      const dados = mockColaboradorDetalhes(codColaborador);
      setDetalhes(dados);
      setLoading(false);
    }, 500);
  }, [codColaborador]);

  return { detalhes, loading };
};

export const useFeriasInfo = (codColaborador: string) => {
  const [ferias, setFerias] = useState<FeriasInfo | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Simular chamada de API
    setTimeout(() => {
      const dados = mockFeriasInfo(codColaborador);
      setFerias(dados);
      setLoading(false);
    }, 500);
  }, [codColaborador]);

  return { ferias, loading };
};


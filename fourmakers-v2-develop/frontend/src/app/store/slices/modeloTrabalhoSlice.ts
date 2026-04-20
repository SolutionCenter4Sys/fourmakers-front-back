import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { fetchModelosTrabalho as fetchModelosTrabalhoApi } from '@data/api/PerfilAtuacaoApi';

const STORAGE_KEY = 'fourmakers_modelos_trabalho';

export interface ModeloTrabalhoItem {
  id: string;
  descricao: string;
  codigo: number;
}

function getCached(): ModeloTrabalhoItem[] | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    const parsed = JSON.parse(raw) as unknown;
    if (!Array.isArray(parsed) || parsed.length === 0) return null;
    const valid = parsed.every(
      (x: unknown) =>
        typeof x === 'object' &&
        x !== null &&
        'id' in x &&
        'descricao' in x &&
        'codigo' in x &&
        typeof (x as ModeloTrabalhoItem).id === 'string' &&
        typeof (x as ModeloTrabalhoItem).descricao === 'string' &&
        typeof (x as ModeloTrabalhoItem).codigo === 'number'
    );
    return valid ? (parsed as ModeloTrabalhoItem[]) : null;
  } catch {
    return null;
  }
}

function setCached(list: ModeloTrabalhoItem[]) {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(list));
  } catch {
    // ignore
  }
}

export type FetchModelosTrabalhoArg = string | { token: string; force?: boolean };

export const fetchModelosTrabalho = createAsyncThunk(
  'modeloTrabalho/fetch',
  async (arg: FetchModelosTrabalhoArg): Promise<ModeloTrabalhoItem[]> => {
    const token = typeof arg === 'string' ? arg : arg.token;
    const force = typeof arg === 'string' ? false : (arg.force ?? false);
    if (!force) {
      const cached = getCached();
      if (cached && cached.length > 0) return cached;
    }
    const list = await fetchModelosTrabalhoApi(token);
    if (list?.length) setCached(list);
    return list ?? [];
  }
);

interface ModeloTrabalhoState {
  list: ModeloTrabalhoItem[];
  status: 'idle' | 'loading' | 'succeeded' | 'failed';
  error: string | null;
}

const initialState: ModeloTrabalhoState = {
  list: [],
  status: 'idle',
  error: null,
};

const modeloTrabalhoSlice = createSlice({
  name: 'modeloTrabalho',
  initialState,
  reducers: {
    clearModelosTrabalho: (state) => {
      state.list = [];
      state.status = 'idle';
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchModelosTrabalho.pending, (state) => {
        state.status = 'loading';
        state.error = null;
      })
      .addCase(fetchModelosTrabalho.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.list = action.payload;
      })
      .addCase(fetchModelosTrabalho.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message ?? 'Erro ao carregar modelos de trabalho';
      });
  },
});

export const { clearModelosTrabalho } = modeloTrabalhoSlice.actions;
export default modeloTrabalhoSlice.reducer;

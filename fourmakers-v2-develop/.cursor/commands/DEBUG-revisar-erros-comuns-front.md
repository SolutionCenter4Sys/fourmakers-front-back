**Versao:** 1.1.0  
**Ultima Atualizacao:** 06/03/2026  
**Dependencias:** `@ARCHITECTURE.md`, `@DESIGN_SYSTEM_AUDIT.md`, `@public/design-toolkit.md`

---

## Role

Voce e um **desenvolvedor Javascript senior** com extensa experiencia em arquitetura DDD em camadas, DRY, Clean Code e Boas praticas de desenvolvimento.

### Behaviors

**DO:**
- Systematically scan for common frontend errors
- Provide specific file and line references
- Offer auto-fix solutions where possible
- Categorize errors by severity

**DON'T:**
- Skip files without analysis
- Provide vague feedback
- Ignore architectural context

---

## Task Type

AUDIT — Verificacao de erros comuns em codigo frontend

---

## Inputs

- Codigo-fonte atual (foco em `src/presentation/`)
- `@ARCHITECTURE.md` — Guia de arquitetura e estrutura de arquivos
- `@DESIGN_SYSTEM_AUDIT.md` — Padroes de conformidade
- `@public/design-toolkit.md` — Componentes e hooks padronizados

---

## Execution Flow

### Phase 1: Discovery
Use `Grep` para detectar padroes problematicos:
- `fetch(` — Uso direto (deve usar httpClient)
- `Loader2` ou `RefreshCw` — Loading incorreto (deve usar Spinner)
- `bg-green-`, `text-red-`, `bg-blue-` — Cores hardcoded
- `: any` — Tipos any
- `?.` ausente em acessos aninhados
- `useEffect` sem array de dependencias ou incompleto

### Phase 2: Analysis
Para cada arquivo suspeito:
1. Read arquivo completo
2. Verificar contexto do erro
3. Classificar severidade
4. Propor solucao

### Phase 3: Report
Gerar relatorio estruturado com:
- Lista de erros por categoria
- Arquivos afetados
- Solucoes sugeridas
- Auto-fix possivel

---

## 1. Falha ao Validar Valores `null` ou `undefined`

**Descricao:**
Ocorre quando o codigo assume que determinados dados sempre estarao presentes, ignorando cenarios de carregamento assincrono, falha de API ou props nao inicializadas.

**Padroes de Deteccao:**
```bash
# Buscar acesso a propriedades sem optional chaining
grep -r "\.propriedade\." src/ --include="*.tsx" | grep -v "?\?\."

# Buscar desestruturacao sem default values
grep -r "const { " src/ --include="*.tsx" | grep -v "="
```

**Problemas Causados:**
- `Cannot read properties of undefined`
- Quebra de renderizacao
- UI inconsistente

**Exemplo de Erro:**
```typescript
// ❌ ERRADO: Assume que usuario sempre existe
const UsuarioCard = ({ usuario }) => {
  return <div>{usuario.nome}</div>; // Erro se usuario = null
};

// ❌ ERRADO: Acesso aninhado sem protecao
const NomeEmpresa = ({ usuario }) => {
  return <span>{usuario.empresa.nome}</span>; // Erro se empresa = undefined
};
```

**Boas Praticas:**
```typescript
// ✅ CORRETO: Default values
const UsuarioCard = ({ usuario = null }) => {
  if (!usuario) return <div>Carregando...</div>;
  return <div>{usuario.nome}</div>;
};

// ✅ CORRETO: Optional chaining
const NomeEmpresa = ({ usuario }) => {
  return <span>{usuario?.empresa?.nome ?? 'N/A'}</span>;
};

// ✅ CORRETO: Validacao explicita
const ListaItens = ({ itens }) => {
  if (!itens || itens.length === 0) return <EmptyState />;
  return itens.map(item => <Item key={item.id} {...item} />);
};
```

---

## 2. Declarar Estrutura de Dados com `any` (TypeScript)

**Descricao:**
Uso indiscriminado do tipo `any` elimina completamente os beneficios de tipagem estatica e pode mascarar erros graves de contrato de dados.

**Padroes de Deteccao:**
```bash
# Buscar uso de any
grep -r ": any" src/ --include="*.ts" --include="*.tsx"
grep -r "as any" src/ --include="*.ts" --include="*.tsx"
grep -r "Promise<any>" src/ --include="*.ts" --include="*.tsx"
```

**Problemas Causados:**
- Perda de autocomplete
- Erros so detectados em runtime
- Fragilidade na manutencao
- Falta de seguranca em refactors

**Exemplo de Erro:**
```typescript
// ❌ ERRADO: any em props
const ProcessarDados = (dados: any) => {
  return dados.map(d => d.valor); // Sem autocomplete, sem verificacao
};

// ❌ ERRADO: any em resposta de API
const buscarUsuario = async (id: string): Promise<any> => {
  const response = await httpClient.get(`/usuarios/${id}`);
  return response.data; // Tipo perdido
};

// ❌ ERRADO: as any para ignorar erro
const usuario = dados as any;
```

**Boas Praticas:**
```typescript
// ✅ CORRETO: Interface explicita
interface Usuario {
  id: string;
  nome: string;
  email: string;
}

const ProcessarDados = (dados: Usuario[]) => {
  return dados.map(d => d.nome); // Autocomplete funciona
};

// ✅ CORRETO: Generics para reuso
interface ApiResponse<T> {
  data: T;
  status: number;
}

const buscarUsuario = async (id: string): Promise<ApiResponse<Usuario>> => {
  return httpClient.get<Usuario>(`/usuarios/${id}`);
};

// ✅ CORRETO: unknown em vez de any
const processarDesconhecido = (valor: unknown) => {
  if (typeof valor === 'string') {
    return valor.toUpperCase(); // Type narrowing
  }
};
```

---

## 3. Chamar Endpoints de Forma Redundante

**Descricao:**
Chamadas repetidas e desnecessarias para APIs, geralmente causadas por ma configuracao de `useEffect`, ausencia de cache ou falta de controle de dependencias.

**Padroes de Deteccao:**
```bash
# Buscar useEffect sem array de dependencias
grep -A 5 "useEffect(" src/ --include="*.tsx" -r | grep -B 5 -A 5 "^}[^)]*$"

# Buscar dependencias vazias (roda uma vez, mas pode indicar problema)
grep -r "useEffect.*\[\s*\]" src/ --include="*.tsx"
```

**Problemas Causados:**
- Sobrecarga no backend
- Performance degradada
- Risco de rate limit
- UI instavel

**Exemplo de Erro:**
```typescript
// ❌ ERRADO: Sem array de dependencias (roda a cada render)
useEffect(() => {
  fetchDados();
}); // Array de dependencias ausente

// ❌ ERRADO: Objeto criado inline (nova referencia a cada render)
useEffect(() => {
  buscarUsuario({ id: userId });
}, [{ id: userId }]); // Objeto novo a cada render -> loop

// ❌ ERRADO: Funcao nao memoizada
useEffect(() => {
  processar();
}, [processar]); // processar recriada a cada render -> loop
```

**Boas Praticas:**
```typescript
// ✅ CORRETO: Dependencias explicitas
useEffect(() => {
  fetchDados();
}, [userId, filtro]); // Roda apenas quando dependencias mudam

// ✅ CORRETO: useCallback para funcoes
const processar = useCallback(() => {
  // logica
}, [dependencia1]);

useEffect(() => {
  processar();
}, [processar]); // Agora processar e estavel

// ✅ CORRETO: useMemo para objetos
const params = useMemo(() => ({
  id: userId,
  page: currentPage
}), [userId, currentPage]);

useEffect(() => {
  buscarUsuario(params);
}, [params]); // params so muda quando dependencias mudam

// ✅ CORRETO: AbortController para cancelar
useEffect(() => {
  const controller = new AbortController();
  
  fetchDados({ signal: controller.signal });
  
  return () => controller.abort();
}, [filtro]);
```

---

## 4. Risco de Loops Infinitos

**Descricao:**
Loops infinitos geralmente acontecem quando `setState` e chamado dentro de um `useEffect` que depende da propria variavel alterada, ou quando dependencias mudam continuamente.

**Padroes de Deteccao:**
```bash
# Buscar setState dentro de useEffect
grep -B 2 -A 10 "useEffect" src/ --include="*.tsx" -r | grep -B 5 "setState\|set[A-Z]"

# Buscar useEffect com dependencia de estado
# Requer analise manual do contexto
```

**Resultado:**
Render infinito -> travamento -> crash do navegador.

**Outros Cenarios Comuns:**
- Funcoes recriadas a cada render sem `useCallback`
- Objetos criados inline como dependencia do `useEffect`
- Atualizacao de estado derivado sem memoizacao

**Exemplo de Erro:**
```typescript
// ❌ ERRADO: Loop infinito - dependencia causa atualizacao
useEffect(() => {
  setContador(contador + 1);
}, [contador]); // Atualiza contador -> roda effect -> atualiza contador...

// ❌ ERRADO: Loop via funcao
const [lista, setLista] = useState([]);

useEffect(() => {
  const novaLista = processarLista(lista);
  setLista(novaLista);
}, [lista]); // lista muda -> effect roda -> lista muda...

// ❌ ERRADO: Dependencia instavel
useEffect(() => {
  fetch(url);
}, [{ url }]); // Objeto novo a cada render
```

**Boas Praticas:**
```typescript
// ✅ CORRETO: useCallback para funcoes
const incrementar = useCallback(() => {
  setContador(c => c + 1); // Funcional update
}, []); // Sem dependencias - usa valor anterior

// ✅ CORRETO: useMemo para valores derivados
const listaProcessada = useMemo(() => {
  return processarLista(listaOriginal);
}, [listaOriginal]); // So recalcula quando listaOriginal muda

// ✅ CORRETO: Estado derivado separado
const [valor, setValor] = useState(0);
const dobro = useMemo(() => valor * 2, [valor]); // Deriva do estado

// ✅ CORRETO: Dependencias primitivas
useEffect(() => {
  fetch(url);
}, [url]); // String e estavel

// ✅ CORRETO: eslint-plugin-react-hooks
// Configurar ESLint para detectar dependencias faltantes:
// "react-hooks/exhaustive-deps": "error"
```

---

## 5. Design System Non-Conformities

**Descricao:**
Violacoes dos padroes definidos em `@DESIGN_SYSTEM_AUDIT.md` e `@public/design-toolkit.md`.

### 5.1 Cores Hardcoded

**Padroes de Deteccao:**
```bash
grep -r "bg-green-" src/ --include="*.tsx"
grep -r "text-red-" src/ --include="*.tsx"
grep -r "bg-blue-" src/ --include="*.tsx"
grep -r "bg-white" src/ --include="*.tsx"
```

**Exemplo de Erro:**
```typescript
// ❌ ERRADO: Cores hardcoded
<div className="bg-green-100 text-green-800">
<span className="text-red-600">
<Card className="bg-white">
```

**Boas Praticas:**
```typescript
// ✅ CORRETO: Tokens do DS
<div className="bg-success/10 text-success">
<span className="text-destructive">
<Card className="bg-surfaceElevated">
```

### 5.2 Loading Incorreto

**Padroes de Deteccao:**
```bash
grep -r "Loader2" src/ --include="*.tsx"
grep -r "RefreshCw" src/ --include="*.tsx"
```

**Exemplo de Erro:**
```typescript
// ❌ ERRADO: Icone de loading direto
import { Loader2 } from "lucide-react";
<Loader2 className="animate-spin" />

// ❌ ERRADO: RefreshCw como loading
import { RefreshCw } from "lucide-react";
<RefreshCw className="animate-spin" />
```

**Boas Praticas:**
```typescript
// ✅ CORRETO: Componente Spinner
import { Spinner } from "@/components/ui/spinner";
<Spinner size="sm" />
<Spinner size="md" />
<Spinner size="lg" />
```

### 5.3 Modal Sem Acessibilidade

**Padroes de Deteccao:**
```bash
# Buscar Dialog sem DialogDescription
grep -l "Dialog" src/ --include="*.tsx" -r | xargs grep -L "DialogDescription"
```

**Exemplo de Erro:**
```typescript
// ❌ ERRADO: Modal sem descricao
<Dialog>
  <DialogHeader>
    <DialogTitle>Confirmar</DialogTitle>
  </DialogHeader>
  <DialogContent>
    {/* Falta DialogDescription */}
  </DialogContent>
</Dialog>
```

**Boas Praticas:**
```typescript
// ✅ CORRETO: Modal completo
<Dialog>
  <DialogHeader>
    <DialogTitle>Confirmar Exclusao</DialogTitle>
    <DialogDescription>
      Esta acao nao pode ser desfeita. Deseja continuar?
    </DialogDescription>
  </DialogHeader>
  <DialogContent>
    {/* Conteudo */}
  </DialogContent>
</Dialog>
```

---

## 6. Violações Arquiteturais

### 6.1 Uso Direto de `fetch()` ou `axios`

**Padroes de Deteccao:**
```bash
grep -r "fetch(" src/ --include="*.ts" --include="*.tsx"
grep -r "axios" src/ --include="*.ts" --include="*.tsx"
```

**Exemplo de Erro:**
```typescript
// ❌ ERRADO: Fetch direto
const resposta = await fetch('/api/usuarios');

// ❌ ERRADO: Axios direto
const resposta = await axios.get('/api/usuarios');
```

**Boas Praticas:**
```typescript
// ✅ CORRETO: httpClient factory
import { createHttpClient } from '@data/api/httpClient';

const httpClient = createHttpClient();
const resposta = await httpClient.get('/usuarios');
```

### 6.2 Presentation Acessando Data Layer Diretamente

**Padroes de Deteccao:**
```bash
grep -r "@data/api" src/presentation/ --include="*.tsx"
grep -r "from.*data/api" src/presentation/ --include="*.tsx"
```

**Exemplo de Erro:**
```typescript
// ❌ ERRADO: Componente acessando API direto
import { usuarioApi } from '@data/api/usuarioApi';

const UsuarioList = () => {
  useEffect(() => {
    usuarioApi.listar().then(...); // Violação!
  }, []);
};
```

**Boas Praticas:**
```typescript
// ✅ CORRETO: Via UseCase e DI
import { container } from 'tsyringe';
import { ListarUsuariosUseCase } from '@domain/usecases/usuario/ListarUsuariosUseCase';

const UsuarioList = () => {
  const useCase = container.resolve(ListarUsuariosUseCase);
  
  useEffect(() => {
    useCase.execute({}).then(...);
  }, [useCase]);
};
```

---

## Output Format

Generate a report file: `docs/code-reviews/ERROS_COMUNS_FRONT_[timestamp].md`

```markdown
# Relatorio de Erros Comuns — Frontend

**Data:** [timestamp]  
**Revisor:** AI Agent  
**Escopo:** [Feature/Projeto]

---

## Resumo Executivo

| Categoria | Issues Encontradas | Auto-Fix Possivel |
|-----------|-------------------|-------------------|
| null/undefined | [N] | [N] |
| any types | [N] | [N] |
| API redundante | [N] | [N] |
| Loops infinitos | [N] | [N] |
| DS Non-Conformity | [N] | [N] |
| Arquitetura | [N] | [N] |

---

## 1. Falha ao Validar null/undefined

### Issue 1.1: [Descricao]
**Arquivo:** `src/...`  
**Linha:** [X]  
**Severidade:** 🔴 Alta / 🟡 Media

**Codigo Problematico:**
```typescript
[Snippet]
```

**Solucao Sugerida:**
```typescript
[Snippet corrigido]
```

---

## 2. any Types

[Mesma estrutura]

---

## Recomendacoes Gerais

### Configuracoes Recomendadas

1. **ESLint:** Configurar regras para detectar:
   - `no-explicit-any`: error
   - `react-hooks/exhaustive-deps`: error
   - `no-undef`: error

2. **Code Review Checklist:**
   - [ ] Nenhum acesso a propriedade sem `?.`
   - [ ] Nenhum `any` sem justificativa
   - [ ] Todos useEffects revisados
   - [ ] Cores verificadas contra DS
   - [ ] Modais verificados
   - [ ] Nenhum `fetch()` direto

3. **Pre-Commit Hooks:**
   - Rodar `tsc --noEmit` para verificar tipos
   - Rodar `eslint` para verificar padroes
```

---

## Resumo Executivo (Nivel Staff/Arquitetura)

Esses problemas geralmente indicam:

- ❌ Falta de contrato de dados claro
- ❌ Falta de controle de efeitos colaterais
- ❌ Arquitetura reativa mal estruturada
- ❌ Ausencia de padroes de data fetching
- ❌ Falta de tipagem forte

**Recomendacao:** Criar checklists de code review e configurar ESLint para detectar automaticamente esses padroes.

The final output must be in Portuguese (PT-BR) with all file names, variables and code patterns following Brazilian Portuguese conventions.
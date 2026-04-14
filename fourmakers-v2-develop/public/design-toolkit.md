# Design Toolkit & Frontend Guidelines

Este documento serve como guia completo para design system, arquitetura e boas práticas de desenvolvimento frontend do projeto Fourmakers-v2. Use este toolkit como referência para prototipação, desenvolvimento de novas features, code review e onboarding de novos desenvolvedores.

---

## 1. Design System

### 1.1 Identidade e Paleta

#### Brand Core
- **Primary**: `#000000` (Soft: `#F1F5F9`, Strong: `#000000`)
- **Accent (Destaque)**: `#9A1BFF` (Soft: `#F4EBFF`)

#### Textos
- Primário: `#0F172A`
- Secundário: `#64748B`
- Inverso: `#FFFFFF`

#### Superfícies
- Primária: `#F0F5FA`
- Secundária: `#FFFFFF`
- Elevada: `#FFFFFF`
- Sutil: `#F9FAFB`

#### Bordas
- Default: `#E2E8F0`
- Soft: `#EEF2FF`

#### Status
- Sucesso: `#16A34A`
- Alerta: `#F59E0B`
- Erro: `#DC2626`
- Info: `#2563EB`

#### Gradiente de marca
`linear-gradient(135deg, #9A1BFF 0%, #7B1CE5 40%, #4F46E5 100%)`

#### Dark Mode (equivalentes)
- Primary: `#F9FAFB` (Soft: `#111827`, Strong: `#FFFFFF`)
- Accent: `#C084FF` (Soft: `#312E81`)
- Textos: Primário `#F9FAFB`, Secundário `#CBD5F5`, Inverso `#020617`
- Superfícies: Elevada `#0B1120`, Base/Subtle `#020617`
- Bordas: Default/Soft `#1E293B`
- Gradiente: `linear-gradient(135deg, #4C1D95 0%, #7C3AED 40%, #22C55E 100%)`

### 1.2 Tokens Estruturais

#### Spacing
- 2XS: `4px` (`--space-2xs`)
- XS: `8px` (`--space-xs`)
- SM: `12px` (`--space-sm`)
- MD: `16px` (`--space-md`)
- LG: `24px` (`--space-lg`)
- XL: `32px` (`--space-xl`)

#### Radius
- XS: `6px` (`--radius-xs`)
- SM: `8px` (`--radius-sm`)
- MD: `12px` (`--radius-md`)
- LG: `20px` (`--radius-lg`) - **Padrão para inputs, cards, dialogs**
- Pill: `999px` (`--radius-pill`) - **Padrão para botões**

#### Sombras
- Soft: `0 10px 25px rgba(15,23,42,0.06)` (`--elevation-soft`)
- Hover: `0 18px 45px rgba(15,23,42,0.10)` (`--elevation-card-hover`)

### 1.3 Componentes Padrão

#### Button
- **Primary (default/ação principal)**: usa `--color-btn-primary`. No light: fundo preto + texto branco; no dark: fundo claro + texto escuro. Hover neutro. Use em CTAs de confirmação/salvar. **Sempre raio Pill**.
- **Secondary**: fundo branco/superfície, texto escuro; borda default; hover com `primarySoft`.
- **Outline**: borda default, fundo transparente, texto `primary`; hover com `primarySoft`.
- **Ghost (texto)**: sem borda, fundo transparente, texto `primary`; hover sutil.
- **Brand Gradient (CTA hero)**: gradiente roxo→azul do DS, texto branco; uso restrito a heros/landing.

#### Input/Select/Textarea
- Altura: `40px` (`h-10`)
- Radius: **LG (20px)** (`rounded-lg`)
- Borda: `border-default`
- Foco: borda `primary`, sem outline extra (`focus-visible:ring-0 focus-visible:ring-offset-0`)
- Label sempre presente (placeholders não substituem label)

#### Card
- Fundo: `surfaceElevated`
- Borda: `border-borderSoft`
- Radius: `lg` (20px)
- Sombra: soft
- Padding content: `12-16px`

#### Badge/Chip
- Radius: **Pill**
- Altura: `28-32px`
- Uso de `primary` ou `accent`
- Texto uppercase pequeno opcional

#### Dialog/Modal
- Radius: **LG** em superfícies
- **Sempre incluir** `DialogTitle` e `DialogDescription` para acessibilidade
- Header com título e descrição
- Footer com ações (botões)

#### Table/DataTable
- Cabeçalho: `border-borderDefault`
- Linhas: `hover:bg-surfaceSubtle`
- Radius leve no contêiner (LG)
- Ações alinhadas à direita

### 1.4 Tipografia
- Fonte: "Inter", fallback system
- Pesos: 400/500/600/700
- Títulos: peso 600/700
- Textos: peso 400/500
- Hierarquia clara (títulos, subtítulos, body, legendas)

### 1.5 Acessibilidade
- **Contraste**: usar tokens de texto e superfícies; evitar cores custom sem checar contraste
- **Labels**: sempre associados a inputs; placeholders não substituem label
- **Foco**: sempre visível (não remover outline); usar ring/borda
- **Ícones**: em botões/ações devem ter `aria-label` ou texto visível
- **Tooltips**: não devem conter informação crítica única
- **Modais**: sempre incluir `DialogTitle` e `DialogDescription`

### 1.6 Dark Mode
- Toda a paleta reage à classe `dark` no html
- Fundo base: `#020617`, superfícies elevadas `#0B1120`
- Texto: `#F9FAFB`, bordas `#1E293B`
- Gradiente mantém roxo→verde
- **Nunca force cores** com `dark:bg-white`. Use tokens.

### 1.7 Comportamentos e Microinterações
- Hover em cards/botões: sombra/leve aumento, mantendo cores de tokens
- Estados de carregamento: skeleton ou spinner; disable ações enquanto processa
- Feedback de erro/sucesso: badges/alertas alinhados às cores de status
- Transições: 200-300ms (fade/translate curtas)

---

## 2. Arquitetura

### 2.1 Clean Architecture

O projeto segue os princípios da Clean Architecture, com separação clara de responsabilidades em camadas:

```
Presentation → App → Domain ← Data
     ↓           ↓      ↑        ↑
  Shared ←──────┴──────┴────────┘
     ↑
   Core
```

#### Camadas

1. **Presentation Layer**: Interface do usuário (Pages, Components, Layouts, Styles)
2. **App Layer**: Configuração da aplicação (AppProvider, Routes, Redux Store)
3. **Domain Layer**: Regras de negócio (Entities, Repository Interfaces, Use Cases)
4. **Data Layer**: Acesso a dados (APIs, Repository Implementations)
5. **Core Layer**: Infraestrutura (Dependency Injection)
6. **Shared Layer**: Código compartilhado (Constants, Utils, Types)

### 2.2 Fluxo de Dados

```
Page → Redux Store → UseCase → Repository (Interface) → Repository (Implementation) → API → httpClient → Backend
```

**Regra fundamental**: Páginas **nunca** fazem `fetch()` direto. Sempre usar UseCases que chamam Repositories.

### 2.3 HTTP Client Factory

**⚠️ Regra obrigatória**: Todas as chamadas HTTP devem ser feitas através da factory `httpClient` localizada em `@data/api/httpClient.ts`.

**Nunca use `fetch()` diretamente!**

A factory garante:
- Inclusão automática do header `FRONTEND_TRACE_ID`
- Configuração consistente de headers (Content-Type, Authorization)
- Tratamento padronizado de erros
- Suporte para diferentes métodos HTTP (GET, POST, PUT, PATCH, DELETE)
- Suporte para downloads de arquivos (postBlob)

**Exemplo de uso**:

```typescript
import { httpClient } from '@data/api/httpClient'

export class NovaApi {
  async buscarDados(token: string): Promise<DadosResponse> {
    return httpClient.get<DadosResponse>(
      '/api/endpoint/dados',
      { token }
    )
  }

  async criarDados(token: string, payload: DadosPayload): Promise<DadosResponse> {
    return httpClient.post<DadosResponse>(
      '/api/endpoint/dados',
      payload,
      { token }
    )
  }
}
```

**Para APIs com base URL diferente**:

```typescript
import { createHttpClient } from '@data/api/httpClient'

const customClient = createHttpClient({ 
  baseURL: 'https://api.externa.com' 
})
```

### 2.4 Dependency Inversion

#### Domain/repositories (Interfaces)
```typescript
// domain/repositories/AuthRepository.ts
export interface AuthRepository {
  fetchShowmeProfile(token: string): Promise<ShowmeUserProfile>
  logout(token: string, cpf: string): Promise<void>
}
```
- Define **o que** um repositório deve fazer (contrato)
- Não sabe **como** será implementado
- Pertence à camada de domínio (mais interna)

#### Data/repositories (Implementações)
```typescript
// data/repositories/AuthRepositoryImpl.ts
export class AuthRepositoryImpl implements AuthRepository {
  constructor(private readonly api: AuthApi) {}
  
  async fetchShowmeProfile(token: string): Promise<ShowmeUserProfile> {
    return this.api.getShowmeProfile(token) // Implementação concreta
  }
}
```
- Implementa **como** o repositório funciona
- Usa APIs HTTP, banco de dados, etc.
- Pertence à camada de dados (mais externa)
- Depende da interface definida em Domain

**Benefícios**:
- Inversão de Dependência: Domain não depende de Data
- Testabilidade: Pode criar mocks implementando a interface
- Flexibilidade: Pode trocar a implementação sem afetar o domínio

---

## 3. Organização de Código

### 3.1 Estrutura de Pastas

```
src/
├── app/              # Configuração da aplicação
│   ├── providers/   # Providers (Redux, Router, Theme)
│   ├── routes/      # Configuração de rotas
│   └── store/       # Redux store e slices
├── core/            # Infraestrutura
│   └── di/          # Dependency Injection (TSyringe)
├── data/            # Acesso a dados
│   ├── api/         # Clientes HTTP (usando httpClient)
│   └── repositories/ # Implementações de repositórios
├── domain/          # Regras de negócio
│   ├── entities/    # Entidades de domínio
│   ├── repositories/ # Interfaces de repositórios
│   └── usecases/    # Casos de uso
├── presentation/    # Interface do usuário
│   ├── components/  # Componentes reutilizáveis
│   ├── hooks/       # Hooks de apresentação
│   ├── layouts/     # Layouts principais
│   ├── pages/       # Páginas da aplicação
│   └── styles/       # Estilos globais
└── shared/          # Código compartilhado
    ├── constants/    # Constantes
    ├── utils/        # Funções utilitárias
    └── types/        # Tipos TypeScript
```

### 3.2 Convenções de Nomenclatura

- **Componentes**: `PascalCase` (ex: `UserCard.tsx`)
- **Hooks**: `useX` ou `useXViewModel` (ex: `useGestaoVagas.ts`)
- **Páginas**: `PascalCase` (ex: `GestaoVagas.tsx`)
- **APIs**: `XApi` (ex: `AuthApi.ts`)
- **Repositories**: `XRepository` (interface) / `XRepositoryImpl` (implementação)
- **UseCases**: `XUseCase` (ex: `GetShowmeProfileUseCase.ts`)
- **Utils**: `xUtils.ts` (ex: `dateUtils.ts`)
- **Types**: `XType` ou `XInterface` (ex: `UserType.ts`)

### 3.3 Padrões por Tipo de Arquivo

#### Pages (`src/presentation/pages/`)
- **Responsabilidade**: Apenas orquestrar UI (chamar componentes e hooks)
- **Não deve conter**: Lógica de negócio, chamadas diretas a APIs, cálculos complexos
- **Deve conter**: Estrutura de layout, composição de componentes, handlers simples

```typescript
// ✅ Bom exemplo
export default function GestaoVagas() {
  const { vagas, loading, handleEdit } = useGestaoVagas()
  
  return (
    <div>
      <PageHeader title="Gestão de Vagas" />
      <VagasList vagas={vagas} onEdit={handleEdit} />
    </div>
  )
}
```

```typescript
// ❌ Mau exemplo
export default function GestaoVagas() {
  const [vagas, setVagas] = useState([])
  
  useEffect(() => {
    fetch('/api/vagas') // ❌ fetch direto
      .then(res => res.json())
      .then(setVagas)
  }, [])
  
  return <div>{/* ... */}</div>
}
```

#### Hooks (`src/presentation/hooks/`)
- **Responsabilidade**: Lógica de apresentação, estado local, efeitos, cálculos
- **Nomenclatura**: `use[Feature].ts` ou `use[Feature]ViewModel.ts`
- **Pode conter**: useState, useEffect, useMemo, useCallback, chamadas a UseCases via Redux

```typescript
// ✅ Bom exemplo
export function useGestaoVagas() {
  const [vagas, setVagas] = useState([])
  const dispatch = useAppDispatch()
  
  useEffect(() => {
    dispatch(fetchVagas()) // ✅ Usa UseCase via Redux
  }, [dispatch])
  
  const handleEdit = useCallback((id: string) => {
    // Lógica de apresentação
  }, [])
  
  return { vagas, handleEdit }
}
```

#### Components (`src/presentation/components/`)
- **Responsabilidade**: Componentes reutilizáveis e desacoplados
- **Deve usar**: Componentes base do DS (Button, Card, Input, etc.)
- **Deve usar**: Tokens em vez de valores hardcoded
- **Estrutura**: Componentes comuns em `common/`, componentes de feature em `[feature]/`

```typescript
// ✅ Bom exemplo
export function UserCard({ user }: { user: User }) {
  return (
    <Card className="border-borderSoft bg-surfaceElevated">
      <CardContent className="p-4">
        <h3 className="text-primaryText">{user.nome}</h3>
        <p className="text-secondaryText">{user.email}</p>
      </CardContent>
    </Card>
  )
}
```

```typescript
// ❌ Mau exemplo
export function UserCard({ user }: { user: User }) {
  return (
    <div className="bg-white border border-gray-300 rounded-lg p-4">
      {/* ❌ Cores hardcoded */}
    </div>
  )
}
```

#### Modals (`src/presentation/components/[feature]/[Feature]Modal.tsx`)
- **Sempre incluir**: `DialogTitle` e `DialogDescription` para acessibilidade
- **Estrutura**: Header (title + description), Content (formulário/conteúdo), Footer (ações)

```typescript
// ✅ Bom exemplo
export function CriarPerfilModal({ open, onOpenChange }: Props) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Criar Novo Perfil</DialogTitle>
          <DialogDescription>
            Preencha os campos para criar um novo perfil de permissionamento.
          </DialogDescription>
        </DialogHeader>
        {/* Conteúdo */}
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancelar
          </Button>
          <Button onClick={handleSave}>Salvar</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
```

#### APIs (`src/data/api/`)
- **Sempre usar**: `httpClient` de `@data/api/httpClient`
- **Nunca usar**: `fetch()` diretamente
- **Estrutura**: Classes com métodos async que retornam Promises tipadas

```typescript
// ✅ Bom exemplo
import { httpClient } from '@data/api/httpClient'

export class VagasApi {
  async listarVagas(token: string): Promise<Vaga[]> {
    return httpClient.get<Vaga[]>('/api/vagas', { token })
  }
  
  async criarVaga(token: string, payload: CriarVagaPayload): Promise<Vaga> {
    return httpClient.post<Vaga>('/api/vagas', payload, { token })
  }
}
```

```typescript
// ❌ Mau exemplo
export class VagasApi {
  async listarVagas(token: string): Promise<Vaga[]> {
    const response = await fetch('/api/vagas', { // ❌ fetch direto
      headers: { Authorization: `Bearer ${token}` }
    })
    return response.json()
  }
}
```

#### UseCases (`src/domain/usecases/`)
- **Responsabilidade**: Lógica de negócio
- **Depende de**: Repository interfaces (não implementações)
- **Estrutura**: Classes com método `execute()` que recebe parâmetros e retorna resultado tipado

```typescript
// ✅ Bom exemplo
export class GetVagasUseCase {
  constructor(
    private readonly repository: VagasRepository // Interface, não implementação
  ) {}
  
  async execute(token: string): Promise<Vaga[]> {
    return this.repository.fetchVagas(token)
  }
}
```

#### Utils (`src/shared/utils/`)
- **Responsabilidade**: Funções puras reutilizáveis
- **Estrutura**: Funções exportadas individualmente ou como objeto
- **Não deve conter**: Lógica de negócio, estado, efeitos colaterais

```typescript
// ✅ Bom exemplo
export function formatCurrency(value: number): string {
  return value.toLocaleString('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  })
}

export function formatDate(date: Date): string {
  return date.toLocaleDateString('pt-BR')
}
```

### 3.4 Imports

**Sempre usar aliases** em vez de imports relativos:

```typescript
// ✅ Bom
import { Button } from '@/components/ui/button'
import { useGestaoVagas } from '@presentation/hooks/recrutamento'
import { httpClient } from '@data/api/httpClient'
import { formatCurrency } from '@shared/utils/dateUtils'

// ❌ Mau
import { Button } from '../../../components/ui/button'
import { useGestaoVagas } from '../../hooks/recrutamento'
```

**Aliases disponíveis**:
- `@/` → `src/`
- `@presentation/` → `src/presentation/`
- `@domain/` → `src/domain/`
- `@data/` → `src/data/`
- `@shared/` → `src/shared/`
- `@app/` → `src/app/`

---

## 4. Boas Práticas

### 4.1 Design System

1. **Sempre use tokens** em vez de valores hardcoded
   - ❌ `bg-white`, `text-gray-700`, `border-gray-300`
   - ✅ `bg-surfaceElevated`, `text-primaryText`, `border-borderDefault`

2. **Use componentes base** do design system
   - ❌ Criar botão custom
   - ✅ Usar `<Button>` do DS

3. **Respeite radius padrão**
   - Botões: **Pill** (999px)
   - Inputs/Cards/Dialogs: **LG** (20px)

4. **Sempre inclua acessibilidade**
   - Labels em inputs
   - `aria-label` em ícones
   - `DialogTitle` e `DialogDescription` em modais

### 4.2 Arquitetura

1. **Páginas não fazem fetch direto**
   - ❌ `fetch('/api/endpoint')`
   - ✅ Use UseCase via Redux ou hook que chama UseCase

2. **Sempre use httpClient**
   - ❌ `fetch()`, `axios.get()`
   - ✅ `httpClient.get()`, `httpClient.post()`

3. **Separe responsabilidades**
   - Pages: UI apenas
   - Hooks: Lógica de apresentação
   - UseCases: Lógica de negócio
   - APIs: Chamadas HTTP

4. **Use interfaces em Domain**
   - Repository interfaces em `domain/repositories/`
   - Implementações em `data/repositories/`

### 4.3 Código

1. **TypeScript estrito**
   - Sempre tipar props, retornos, parâmetros
   - Evitar `any`

2. **Componentes pequenos e focados**
   - Um componente, uma responsabilidade
   - Extrair lógica complexa para hooks

3. **Performance**
   - Use `useMemo` para cálculos pesados
   - Use `useCallback` para funções passadas como props
   - Evite re-renders desnecessários

4. **Nomenclatura clara**
   - Nomes descritivos
   - Evitar abreviações
   - Seguir convenções do projeto

---

## 5. Checklist para Novas Features

### Antes de começar
- [ ] Entender requisitos e regras de negócio
- [ ] Verificar se há componentes similares para reutilizar
- [ ] Planejar estrutura de pastas e arquivos

### Durante o desenvolvimento
- [ ] Criar estrutura de pastas seguindo padrão do projeto
- [ ] Usar componentes base do DS (Button, Card, Input, etc.)
- [ ] Usar tokens em vez de valores hardcoded
- [ ] Implementar acessibilidade (labels, aria-labels, DialogDescription)
- [ ] Seguir Clean Architecture (Pages → Hooks → UseCases → Repositories → APIs)
- [ ] Usar `httpClient` para todas as chamadas HTTP
- [ ] Tipar tudo com TypeScript
- [ ] Testar dark mode
- [ ] Testar responsividade

### Antes de commitar
- [ ] Verificar se não há cores hardcoded (`bg-white`, `text-gray-700`, etc.)
- [ ] Verificar se não há `fetch()` direto
- [ ] Verificar se modais têm `DialogTitle` e `DialogDescription`
- [ ] Verificar se está usando aliases de imports
- [ ] Verificar se código segue convenções de nomenclatura
- [ ] Executar linter e corrigir erros

---

## 6. Checklist para Code Review

### Design System
- [ ] Está usando tokens em vez de cores hardcoded?
- [ ] Está usando componentes base do DS?
- [ ] Radius está correto (Pill para botões, LG para inputs/cards)?
- [ ] Modais têm `DialogTitle` e `DialogDescription`?
- [ ] Inputs têm labels associados?
- [ ] Ícones têm `aria-label`?

### Arquitetura
- [ ] Páginas não fazem fetch direto?
- [ ] Está usando `httpClient` para chamadas HTTP?
- [ ] Separação de responsabilidades está correta?
- [ ] UseCases dependem de interfaces, não implementações?

### Código
- [ ] TypeScript está tipado corretamente?
- [ ] Componentes são pequenos e focados?
- [ ] Performance está otimizada (useMemo, useCallback)?
- [ ] Nomenclatura segue convenções?
- [ ] Imports usam aliases?

---

## 7. Referências

- **Arquitetura completa**: `ARCHITECTURE.md`
- **Documentação de componentes**: `/documentacao` (página da aplicação)
- **Design Toolkit**: Este arquivo (`public/design-toolkit.md`)

---

**Última atualização**: Janeiro 2026  
**Versão**: 2.0 (Guia completo com arquitetura e boas práticas)

# 📊 Padrão DataTable - Documentação Oficial

## 🎯 Objetivo

**TODAS as tabelas (datatables) criadas neste projeto DEVEM usar o componente `DataTable`**, que já implementa:
- ✅ Reordenação de colunas (drag & drop nos headers)
- ✅ Ordenação de colunas (sort ascendente/descendente)
- ✅ Interface consistente e responsiva
- ✅ Customização de células

## 🚀 Início Rápido

### Exemplo Mínimo

```typescript
import { DataTable } from "@/components/common";
import { Column } from "@/hooks/useColumnReorder";

const MinhaTabela = () => {
  const columns: Column[] = [
    { id: "nome", label: "Nome", sortable: true },
    { id: "email", label: "Email", sortable: true },
  ];

  const data = [
    { id: 1, nome: "João", email: "joao@email.com" },
    { id: 2, nome: "Maria", email: "maria@email.com" },
  ];

  const renderCell = (item: typeof data[0], columnId: string) => {
    switch (columnId) {
      case "nome": return item.nome;
      case "email": return item.email;
      default: return null;
    }
  };

  return (
    <DataTable
      columns={columns}
      data={data}
      keyExtractor={(item) => item.id}
      renderCell={renderCell}
    />
  );
};
```

## 📖 Estrutura Completa

### 1. Definir as Colunas

```typescript
const columns: Column[] = [
  { 
    id: "nome",              // ID único (usado no renderCell)
    label: "Nome",           // Texto do header
    sortable: true,          // Permite ordenação (padrão: true)
    width: "w-[200px]"       // Largura opcional (classes Tailwind)
  },
  { 
    id: "status", 
    label: "Status", 
    sortable: true 
  },
  { 
    id: "acoes", 
    label: "Ações", 
    sortable: false,         // Desabilita ordenação
    width: "w-[100px]" 
  },
];
```

### 2. Criar a Função renderCell

```typescript
const renderCell = (item: DataType, columnId: string) => {
  switch (columnId) {
    case "nome":
      return <span className="font-medium">{item.nome}</span>;
    
    case "status":
      return <StatusBadge status={item.status} />;
    
    case "acoes":
      return (
        <div className="flex gap-2">
          <Button variant="ghost" size="icon">
            <Edit className="h-4 w-4" />
          </Button>
          <Button variant="ghost" size="icon">
            <Trash className="h-4 w-4" />
          </Button>
        </div>
      );
    
    default:
      return null;
  }
};
```

### 3. Implementar o DataTable

```typescript
<Card>
  <CardContent className="p-6">
    <DataTable
      columns={columns}
      data={filteredData}
      keyExtractor={(item) => item.id}
      renderCell={renderCell}
      emptyMessage="Nenhum registro encontrado"
    />
  </CardContent>
</Card>
```

## 🎨 Exemplos de Células Customizadas

### Avatar com Ícone
```typescript
case "usuario":
  return (
    <div className="flex items-center gap-3">
      <Avatar className="h-10 w-10">
        <AvatarFallback className="bg-secondary text-secondary-foreground">
          <User className="h-5 w-5" />
        </AvatarFallback>
      </Avatar>
      <span className="font-medium">{item.nome}</span>
    </div>
  );
```

### Badges de Status
```typescript
case "status":
  return <StatusBadge status={item.status} variant="projeto" />;
```

### Múltiplos Badges
```typescript
case "tags":
  return (
    <div className="flex gap-2">
      {item.tags.map((tag, idx) => (
        <Badge key={idx} variant="outline">
          {tag}
        </Badge>
      ))}
    </div>
  );
```

### Checkbox
```typescript
case "checkbox":
  return <Checkbox />;
```

### Botões de Ação
```typescript
case "acoes":
  return (
    <div className="flex gap-2 justify-end">
      <Button variant="ghost" size="icon" onClick={() => handleEdit(item.id)}>
        <Edit className="h-4 w-4" />
      </Button>
      <Button variant="ghost" size="icon" onClick={() => handleDelete(item.id)}>
        <Trash className="h-4 w-4" />
      </Button>
    </div>
  );
```

## 📋 Props do DataTable

| Prop | Tipo | Obrigatório | Descrição |
|------|------|-------------|-----------|
| `columns` | `Column[]` | ✅ | Array de definições de colunas |
| `data` | `T[]` | ✅ | Array de dados a serem exibidos |
| `keyExtractor` | `(item: T) => string \| number` | ✅ | Função que retorna chave única |
| `renderCell` | `(item: T, columnId: string) => ReactNode` | ✅ | Função que renderiza cada célula |
| `emptyMessage` | `string` | ❌ | Mensagem quando não há dados (padrão: "Nenhum registro encontrado") |

## 🔧 Interface Column

```typescript
interface Column {
  id: string;           // Identificador único da coluna
  label: string;        // Texto exibido no header
  sortable?: boolean;   // Permite ordenação (padrão: true)
  width?: string;       // Classes Tailwind para largura (ex: "w-[100px]")
}
```

## ✨ Funcionalidades Automáticas

### Reordenação de Colunas (Drag & Drop)
1. Passe o mouse sobre o header de qualquer coluna
2. Aparecerá um ícone de grip (≡) à esquerda
3. Clique e arraste para reordenar
4. A ordem será mantida durante a sessão

### Ordenação (Sort)
1. Clique no ícone de setas (↕) no header
2. **Primeiro clique**: Ordenação ascendente (↑)
3. **Segundo clique**: Ordenação descendente (↓)
4. **Terceiro clique**: Remove ordenação
5. Apenas uma coluna pode ser ordenada por vez

### Células Vazias
- Use `emptyMessage` para customizar a mensagem quando não há dados
- Por padrão exibe: "Nenhum registro encontrado"

## 🎯 Exemplos Reais do Projeto

### Página de Colaboradores
```typescript
// src/pages/Colaboradores.tsx
const columns: Column[] = [
  { id: "colaborador", label: "Colaborador", sortable: true },
  { id: "email", label: "Email", sortable: true },
  { id: "telefone", label: "Telefone", sortable: false },
  { id: "departamento", label: "Departamento", sortable: true },
  { id: "cargo", label: "Cargo", sortable: true },
  { id: "situacao", label: "Situação", sortable: true },
];
```

### Página de Projetos
```typescript
// src/pages/Projetos.tsx
const columns: Column[] = [
  { id: "projeto", label: "Projeto", sortable: true },
  { id: "cliente", label: "Cliente", sortable: true },
  { id: "aprovadores", label: "Aprovadores", sortable: false },
  { id: "dataInicio", label: "Data Início", sortable: true },
  { id: "dataFim", label: "Data Fim", sortable: true },
  { id: "status", label: "Status do Projeto", sortable: true },
  { id: "acoes", label: "Ações", sortable: false, width: "text-right" },
];
```

### Mapa de Alocação
```typescript
// src/components/mapa-alocacao/AlocacoesTab.tsx
const columns: Column[] = [
  { id: "checkbox", label: "", sortable: false, width: "w-12" },
  { id: "cv", label: "CV", sortable: false },
  { id: "colaborador", label: "Colaborador/TBD", sortable: true },
  { id: "departamento", label: "Departamento", sortable: true },
  { id: "gestorAdm", label: "Gestor Adm", sortable: true },
  { id: "clienteProjeto", label: "Cliente/Projeto", sortable: true },
];
```

## ⚠️ Regras Obrigatórias

1. **SEMPRE use DataTable** para criar novas tabelas
2. **NUNCA use Table diretamente** dos components/ui
3. **Defina IDs únicos** em todos os items de data
4. **Use keyExtractor** para identificar cada linha
5. **Mantenha renderCell organizado** com switch/case
6. **Configure sortable=false** apenas para colunas que não devem ser ordenadas (ações, checkboxes, etc)

## 🐛 Troubleshooting

### A ordenação não funciona
- ✅ Verifique se `sortable: true` na definição da coluna
- ✅ Certifique-se que o campo existe nos dados
- ✅ Dados complexos (objetos, arrays) não são ordenáveis automaticamente

### Drag & drop não funciona
- ✅ Certifique-se de usar `DataTable` e não `Table` diretamente
- ✅ Verifique se não há CSS bloqueando eventos de drag

### Performance com muitos dados
- ✅ Use paginação (TablePagination component)
- ✅ Filtre dados antes de passar para DataTable
- ✅ Considere virtualização para +1000 registros

### Células não renderizam
- ✅ Verifique se todos os IDs das colunas têm casos no renderCell
- ✅ Retorne `null` no caso `default` do switch
- ✅ Certifique-se que keyExtractor retorna valores únicos

## 📚 Arquivos Relacionados

- `src/components/common/DataTable.tsx` - Componente principal
- `src/components/common/DraggableTableHead.tsx` - Header arrastável
- `src/hooks/useColumnReorder.ts` - Hook de gerenciamento
- `src/components/common/README.md` - Documentação detalhada

## 🎓 Boas Práticas

1. **Separe lógica de apresentação**: Use renderCell para UI, mantenha lógica de negócio fora
2. **Reutilize componentes**: Crie componentes de célula se o padrão se repete
3. **Responsividade**: Considere como a tabela aparece em mobile
4. **Acessibilidade**: Use labels descritivos e semântica correta
5. **Performance**: Memoize renderCell se necessário para grandes datasets

---

**⚡ Lembre-se: Este é o padrão OFICIAL do projeto. Todas as tabelas devem seguir esta estrutura!**

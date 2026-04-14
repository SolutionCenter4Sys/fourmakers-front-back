# DataTable Component - Padrão de Uso

## Visão Geral

O componente `DataTable` é o padrão para todas as tabelas no sistema. Ele já inclui:
- ✅ Reordenação de colunas (drag & drop)
- ✅ Ordenação de colunas (sort asc/desc)
- ✅ Interface responsiva
- ✅ Suporte a customização de células

## Como Usar

### 1. Importar Dependências

```typescript
import { DataTable } from "@presentation/components/common";
import { Column } from "@/hooks/useColumnReorder";
```

### 2. Definir Colunas

```typescript
const columns: Column[] = [
  { id: "nome", label: "Nome", sortable: true },
  { id: "email", label: "Email", sortable: true },
  { id: "telefone", label: "Telefone", sortable: false },
  { id: "acoes", label: "Ações", sortable: false, width: "w-[100px]" },
];
```

**Propriedades da Coluna:**
- `id`: Identificador único da coluna (usado para renderCell)
- `label`: Texto exibido no header
- `sortable`: Se a coluna permite ordenação (padrão: true)
- `width`: Classes Tailwind para largura (opcional)

### 3. Criar Função renderCell

```typescript
const renderCell = (item: DataType, columnId: string) => {
  switch (columnId) {
    case "nome":
      return <span className="font-medium">{item.nome}</span>;
    case "email":
      return item.email;
    case "telefone":
      return item.telefone;
    case "acoes":
      return (
        <Button variant="ghost" size="icon">
          <Edit className="h-4 w-4" />
        </Button>
      );
    default:
      return null;
  }
};
```

### 4. Usar o DataTable

```typescript
<DataTable
  columns={columns}
  data={filteredData}
  keyExtractor={(item) => item.id}
  renderCell={renderCell}
  emptyMessage="Nenhum registro encontrado"
/>
```

## Exemplo Completo

```typescript
import { useState } from "react";
import { DataTable } from "@presentation/components/common";
import { Column } from "@/hooks/useColumnReorder";
import { Button } from "@/components/ui/button";
import { Edit } from "@/components/ui/system-icons";

interface Usuario {
  id: number;
  nome: string;
  email: string;
  telefone: string;
}

const MinhaTabela = () => {
  const [data, setData] = useState<Usuario[]>([
    { id: 1, nome: "João Silva", email: "joao@email.com", telefone: "(11) 99999-9999" },
    { id: 2, nome: "Maria Santos", email: "maria@email.com", telefone: "(11) 88888-8888" },
  ]);

  const columns: Column[] = [
    { id: "nome", label: "Nome", sortable: true },
    { id: "email", label: "E-mail", sortable: true },
    { id: "telefone", label: "Telefone", sortable: false },
    { id: "acoes", label: "Ações", sortable: false, width: "w-[100px]" },
  ];

  const renderCell = (item: Usuario, columnId: string) => {
    switch (columnId) {
      case "nome":
        return <span className="font-medium">{item.nome}</span>;
      case "email":
        return item.email;
      case "telefone":
        return item.telefone;
      case "acoes":
        return (
          <Button variant="ghost" size="icon">
            <Edit className="h-4 w-4" />
          </Button>
        );
      default:
        return null;
    }
  };

  return (
    <DataTable
      columns={columns}
      data={data}
      keyExtractor={(item) => item.id}
      renderCell={renderCell}
      emptyMessage="Nenhum usuário encontrado"
    />
  );
};

export default MinhaTabela;
```

## Funcionalidades

### Reordenação de Colunas
- Passe o mouse sobre o header da coluna
- Clique e arraste o ícone de grip (≡) que aparece
- Solte na posição desejada

### Ordenação
- Clique no ícone de setas (↕) no header da coluna
- Primeiro clique: ordenação ascendente (↑)
- Segundo clique: ordenação descendente (↓)
- Terceiro clique: remove ordenação

### Células Customizadas
Use o `renderCell` para criar células com:
- Formatação de texto
- Ícones e badges
- Botões de ação
- Links
- Componentes complexos

## Boas Práticas

1. **Sempre defina keyExtractor** para identificação única de linhas
2. **Use sortable=false** apenas para colunas de ação ou que não façam sentido ordenar
3. **Mantenha renderCell organizado** com switch/case
4. **Use componentes do design system** dentro das células
5. **Considere responsividade** ao definir larguras fixas

## Troubleshooting

### A ordenação não funciona
- Verifique se `sortable: true` está definido na coluna
- Certifique-se que o campo existe nos dados
- Dados complexos podem precisar de lógica customizada no `sortData`

### Drag & Drop não funciona
- Certifique-se que está usando o `DataTable` e não o `Table` diretamente
- Verifique se não há sobrescrição de eventos de drag

### Performance com muitos dados
- Use paginação com `TablePagination`
- Considere virtualização para milhares de registros
- Filtre os dados antes de passar para o DataTable

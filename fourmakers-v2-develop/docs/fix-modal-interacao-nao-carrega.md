# Fix: Modal de Nova Interação Não Carregava

## Problema
O modal para criar novas interações/encontros na página de Agendas Comerciais não estava carregando.

## Causa Raiz
O componente `NovaInteracaoModal.tsx` tinha um problema de z-index que causava conflito de empilhamento (stacking context):

```tsx
// ANTES (PROBLEMA)
<DialogContent
  className="max-w-2xl max-h-[90vh] overflow-y-auto relative z-[120]"
  overlayClassName="z-[120]"
>
```

O problema era:
1. O `DialogContent` tinha `position: relative` com `z-[120]`
2. O `overlayClassName` também tinha `z-[120]`
3. Quando você tem `position: relative` com z-index, cria um novo contexto de empilhamento
4. Isso fazia com que o overlay (fundo escuro) ficasse na mesma camada ou acima do conteúdo do modal
5. O resultado era que o modal não aparecia ou ficava inacessível

## Solução
Remover o z-index customizado do `DialogContent` e deixar o componente usar o z-index padrão do sistema de design:

```tsx
// DEPOIS (CORRIGIDO)
<DialogContent
  className="max-w-2xl max-h-[90vh] overflow-y-auto"
>
```

O componente `Dialog` base já tem z-index apropriado configurado (`z-[110]` por padrão), e não precisa de override customizado.

## Arquivos Alterados
- `src/presentation/components/agendas-comerciais/NovaInteracaoModal.tsx`

## Teste
1. Navegar para a página de Agendas Comerciais
2. Clicar no botão de "Nova Interação" em qualquer card de agenda
3. O modal deve aparecer corretamente com o formulário visível e interativo

## Observações Técnicas
- O componente `DialogContent` do shadcn/ui já gerencia corretamente o z-index
- Overrides de z-index devem ser evitados a menos que absolutamente necessário
- Quando usar `position: relative` com z-index, sempre considerar o impacto no stacking context

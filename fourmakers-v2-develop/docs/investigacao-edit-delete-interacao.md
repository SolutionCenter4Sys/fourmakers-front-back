# Investigação: Acesso a Editar/Excluir Interação (Encontro)

## Onde as interações aparecem

1. **Timeline da página Agendas Comerciais**  
   - Fonte: lista plana `historico.interacoes` (vinda de `buscarEncontroPorColaboradorId(codInternoColaborador)`).  
   - Renderização: `InteracaoCard` em `TimelineAgendas.tsx`.  
   - **Editar/Excluir:** `mostrarEditarExcluirSempre={true}` é passado → os botões **devem** aparecer.

2. **Modal "Ver Mais" de uma agenda (seção Interações)**  
   - Fonte: `item.encontros` (agenda carregada ou vinda do detalhe).  
   - Renderização: apenas blocos `<div>` em `DetalhesAgendaModal.tsx` (lista de encontros).  
   - **Editar/Excluir:** **não existem** nessa lista; não há `InteracaoCard` nem botões.

## Conclusão

- Se o usuário **só** vê interações dentro do modal "Ver Mais" (ao abrir uma agenda), ele **nunca** vê os botões Editar/Excluir, porque essa lista não usa `InteracaoCard`.  
- Na timeline, os botões são exibidos por causa de `mostrarEditarExcluirSempre`; se o usuário não vê interações na timeline (filtros, datas, ou lista vazia), o problema é apenas no modal.

## Correção aplicada

- Na seção "Interações" do `DetalhesAgendaModal`, cada encontro passou a ser exibido como `InteracaoCard` (convertendo o encontro para `ItemAgendaGestor`), com `mostrarEditarExcluirSempre` e `agendas={[item]}`, para que Editar/Excluir apareçam também no modal.

## Regras de acesso (LEGACY_REFERENCE.md)

Conforme `LEGACY_REFERENCE.md` (AgendaPermissions.podeEditarExcluirInteracao):

- **Apenas** quem pode editar/excluir interação (encontro):
  1. **Criador da agenda** – pode editar qualquer interação da agenda.
  2. **Criador da interação** – pode editar/excluir a própria interação (match por `codigoInternoColaborador` ou por nome `nomeCompletoColaboradorCriador`).

- Não é permitido exibir Editar/Excluir para outros participantes.

Implementação: `podeEditarExcluirInteracao(interacao, user, agendas)` em `agendaPermissions.ts`; sem override “sempre visível”.

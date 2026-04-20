# Gestão de Vagas — Atribuição de Recrutador (Design)

## Comportamento visual e UX
- Modal com título "Recrutador/a" ou "Recrutador(a)", descrição com contexto (vaga ou candidatura).
- Campo de seleção: botão tipo combobox que abre popover com campo de busca e lista de nomes.
- Lista com altura limitada e scroll vertical; placeholder "Insira o nome do recrutador" / "Buscar recrutador...".
- Botões: Cancelar (outline), Salvar / Atribuir responsável (primary); Salvar desabilitado quando não há seleção.

## Estados
- Carregando: mensagem "Carregando colaboradores…".
- Lista vazia: combobox desabilitado.
- Nenhum resultado na busca: "Nenhum colaborador encontrado."
- Item selecionado: exibido no trigger do combobox; popover fecha ao selecionar.

## Interações
- Scroll do mouse deve rolar a lista de opções dentro do popover.
- Digitação no campo de busca deve filtrar a lista em tempo real (autocomplete), com busca case-insensitive e tolerante a acentos.

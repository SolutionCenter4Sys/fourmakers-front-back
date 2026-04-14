# Feedback (MVP) – Documentação de Design e UX

**Feature**: 15916 – MVP Feedback  
**Rota**: `/feedback360`  
**Última atualização**: Fevereiro 2026

---

## 1. Visão Geral de Design

A tela "Compartilhe seu Feedback" oferece fluxo de **Inserir Feedback** e **Consultar Feedbacks**, com layout limpo, roxo e branco (design system), foco em usabilidade e acessibilidade.

### 1.1 Princípios

- Clareza de labels e campos obrigatórios.
- Busca/autocomplete de colaborador no início do fluxo.
- Escala visual de emojis para avaliação.
- Consistência com design-toolkit (tokens, radius LG, pill para botões, Input/Select/Textarea padrão).

---

## 2. Layout e Estrutura

### 2.1 Cabeçalho da Página

- **Breadcrumb**: Início → Feedback 360.
- **Título**: "Compartilhe seu Feedback".
- **Descrição**: "Inserir, solicitar e consultar feedbacks para o seu perfil."

### 2.2 Abas Principais

- **Inserir Feedback**: formulário de oferta de feedback (ativo por padrão ou conforme estado).
- **Consultar Feedbacks**: listagem com abas internas "Feedbacks Recebidos" e "Feedbacks Enviados" (conforme referência visual; na implementação atual pode existir apenas uma lista "enviados" até backend suportar recebidos).

Estilo: `TabsList variant="primary"`, `TabsTrigger variant="primary"`; aba ativa em destaque (fundo claro / texto primary).

### 2.3 Formulário – Inserir Feedback

Ordem e comportamento:

1. **Colaborador**  
   - Label: "Colaborador".  
   - Controle: combobox/autocomplete (Popover + Command); placeholder "Ou digite o nome de um colega".  
   - Botão trigger com ícone de busca (Search); exibe nome do selecionado ou placeholder.  
   - Erro: "Selecione um colaborador." (texto destrutivo).  
   - Design: `Button variant="outline"`, `rounded-lg`, `border-border`, `focus-visible:ring-2 focus-visible:ring-primary`.

2. **Contexto**  
   - Label: "Contexto".  
   - Input texto; placeholder "Digite o contexto do feedback" (ou "Informe o contexto." conforme referência).  
   - Opcional: ícone de microfone para entrada por voz.  
   - Borda destrutiva e mensagem "Preencha o contexto." em erro.

3. **Data da Interação**  
   - Label: "Data da Interação".  
   - Input type="date"; ícone de calendário à esquerda.  
   - Erro: "Informe a data."

4. **Avaliação Geral**  
   - Label: "Avaliação Geral"; subtítulo "Todos os campos obrigatórios."  
   - Cinco opções em botões/chips: emoji + texto (Precisa evoluir bastante, Abaixo do esperado, Dentro do esperado, Acima do esperado, Referência para os demais).  
   - Estilo: botões `rounded-full`, `size="sm"`; selecionado `variant="primary"`, não selecionado `variant="outline"`.  
   - Erro: "Selecione o nível de avaliação."

5. **Relacionamento** (conforme PBIs)  
   - Label: "Relacionamento".  
   - Opções: Colega, Liderança, Pares, Outro (botões ou select com ícones).  
   - Na referência visual: ícones 🧑‍🤝‍🧑 Colega, 💻 Liderança, 👥 Pares, "... Outro".

6. **Comentário Livre**  
   - Label: "Comentário Livre" (e * quando obrigatório).  
   - Textarea; placeholder "Como foi a interação?" ou "Informe o contexto."; altura mínima ~100px.  
   - Opcional: ícone de microfone.  
   - Erro: mensagem de comentário obrigatório (para níveis 1–2 ou sempre, conforme regra de negócio).  
   - Borda e ring destrutivos em erro.

7. **Ação principal**  
   - Botão "Enviar Feedback" (ícone Send à esquerda); full width no mobile ou alinhado à direita no desktop; `rounded-full`, `bg-primary`, `text-primary-foreground`.  
   - Desabilitado durante submitting.

### 2.4 Consultar Feedbacks

- **Filtros**: Data (input date), Sentimento (Select com opção "Todos" + valores únicos dos níveis).  
  Referência também: filtros por Data, Sentimento, Relacionamento, Colaborador (pills com ícones).

- **Lista**: Cards em lista vertical; cada card:
  - Avatar do interagente (inicial ou foto).
  - Título/Assunto (ex.: "Teste" ou "Feedback para {nome}").
  - Nome do interagente e data (ex.: "Thiago Moreira Faria", "24/02/2026").
  - Badges/pills: Reação (ex.: "Dentro do esperado"), Relacionamento (ex.: "Outro").
  - Label "Comentário" + texto completo (legibilidade de textos longos, sem corte abrupto).
  - Ação "Ver Detalhes" (ícone Eye); opcional: ícone de edição (lápis) quando dentro da janela de 60 min.

- **Modal de detalhes**: Dialog com título "Detalhes do feedback"; exibe Para, Data, Avaliação (emoji + label), Contexto, Comentário. Apenas leitura.

### 2.5 Edição (janela de 60 min)

- Barra ou banner: "Editando feedback" + countdown "59 min 43s restantes" (ícone de relógio); botão "Cancelar" (X).
- Texto de instrução: "Altere os campos abaixo e toque em 'Salvar alterações'."
- Formulário com mesmos campos (Colaborador, Contexto, Data, Avaliação Geral); botão "Salvar alterações" (ícone check) no rodapé.

---

## 3. Componentes e Tokens

- **Cards**: `border-borderSoft`, `bg-surfaceElevated`, `rounded-lg`.
- **Inputs/Select/Textarea**: `rounded-lg`, `border-border`, `focus-visible:ring-2 focus-visible:ring-primary`, altura padrão para input (h-10).
- **Botões**: radius pill para CTAs; primary para ação principal.
- **Badges**: `rounded-full`, variantes secondary/outline para tags de reação e relacionamento.
- **Dialog**: `DialogTitle`, `DialogDescription` obrigatórios (acessibilidade); `rounded-lg`, footer com ações.

---

## 4. Estados e Interações

- **Loading**: "Carregando colaboradores…" / "Carregando feedbacks…" durante fetch.
- **Lista vazia**: Card com mensagem "Nenhum feedback encontrado." (texto centralizado, muted).
- **Validação**: Erros inline abaixo dos campos; toast para resumo ("Preencha todos os campos obrigatórios.").
- **Sucesso**: Toast de sucesso; formulário limpo; troca para aba "Consultar Feedbacks" e recarregar lista.
- **Erro de rede/API**: Toast de erro; lista pode ficar vazia sem quebrar a tela.

---

## 5. Referência Visual (Layout Web)

- Paleta: roxo e branco (primary/accent e superfícies do design system).
- Abas: "Inserir Feedback" e "Consultar Feedbacks" como botões/tabs horizontais.
- Consultar: sub-abas "Feedbacks Recebidos" e "Feedbacks Enviados" com underline na ativa.
- Filtros em linha (pills ou inputs compactos).
- Card: avatar à esquerda; título, nome, data e tags; comentário completo abaixo; ações à direita ou no rodapé do card.

---

## 6. Estado Atual da Implementação (alinhado ao código)

- **Abas**: Existem apenas "Inserir feedback" e "Consultar Feedbacks"; **não** há sub-abas "Feedbacks Recebidos" e "Feedbacks Enviados" na aba Consultar.
- **Formulário**: Não há campo "Relacionamento" (Colega, Liderança, Pares, Outro); não há "Tipo de Interação" categórico (apenas "Contexto" texto). Colaborador não exibe e-mail no combobox.
- **Filtros (Consultar)**: Apenas "Filtrar por data" e "Filtrar por sentimento"; **não** há filtros por relacionamento nem por colaborador.
- **Card**: Título é "Feedback para {nomeColaborador}" (não há campo "Assunto"); **não** há avatar do interagente; comentário na lista é truncado em 120 caracteres (comentário completo só no modal "Ver Detalhes"); **não** há badge de Relacionamento; badges fixos "Enviado" e "Espontâneo". Ação "Ver Detalhes" presente; **não** há ícone de edição nem UI de edição com countdown.
- **Edição em 60 min**: Banner "Editando feedback", countdown e botão "Salvar alterações" **não** implementados.
- **Sinalizador de novo feedback em tempo real**: **não** implementado.

---

## 7. Acessibilidade

- Labels associados a todos os inputs (htmlFor / id).
- Botões de avaliação com `aria-pressed` e `aria-label` com o texto do nível.
- Modal com `DialogTitle` e `DialogDescription`.
- Contraste e foco visível conforme design-toolkit.

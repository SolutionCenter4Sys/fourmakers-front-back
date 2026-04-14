# Feedback (MVP) – Divergências entre Documentação e Código

**Objetivo**: Registrar fatos verificáveis entre o que está documentado em `negocio.md`, `desenvolvimento.md` e `design.md` e o que está implementado no código. Não propõe solução; apenas relata.

**Última atualização**: Fevereiro 2026

---

## [Categoria: Negócio]

### Divergência 1 – Comentário obrigatório sempre vs condicional

**De:**  
PBIs 16005 e negocio.md: "Justificativa Obrigatória: Bloquear o botão 'Enviar' se campo de texto estiver vazio" e "Feedback Construtivo Forçado: O campo de texto é obrigatório" (sempre obrigatório).

**Para:**  
No código (`useFeedback360.ts`, `Feedback360.tsx`): o comentário é obrigatório apenas quando `nivel === 1 || nivel === 2` (`comentarioObrigatorio`). Para níveis 3, 4 e 5 o usuário pode enviar sem preencher comentário. A entidade `Feedback360.ts` comenta: "Comentário livre é obrigatório quando nivel é 1 ou 2."

**Explicação:**  
Regra de negócio documentada (sempre obrigatório) diverge da implementação (obrigatório só para níveis 1 e 2). Impacto: alinhamento produto/desenvolvimento. É divergência de implementação em relação ao que as PBIs pedem.

---

### Divergência 2 – Campo Relacionamento

**De:**  
negocio.md e design.md: campo obrigatório "Relacionamento" com opções Colega, Liderança, Pares, Outro (PBI 15916 / 16005).

**Para:**  
Não existe na entidade `Feedback360.ts`, nem no payload `Feedback360CreatePayload`, nem no formulário em `Feedback360.tsx`. Nenhum estado ou UI para "Relacionamento".

**Explicação:**  
Campo exigido pela documentação de negócio e design está ausente no código. Impacto: critério de aceite não atendido. É ausência de implementação.

---

### Divergência 3 – Abas Feedbacks Recebidos / Feedbacks Enviados

**De:**  
negocio.md e PBI 16010: sistema de abas para separar "Feedbacks Recebidos" e "Feedbacks Enviados"; lista em ordem cronológica decrescente.

**Para:**  
Na tela há apenas duas abas de primeiro nível: "Inserir feedback" e "Consultar Feedbacks". Dentro de "Consultar Feedbacks" não há sub-abas Recebidos/Enviados; a lista exibe uma única lista (na prática, a implementação atual lista os feedbacks que o usuário enviou, via API Listar). Não há separação visual Recebidos vs Enviados.
Na tela há apenas duas abas de primeiro nível: "Inserir feedback" e "Consultar Feedbacks". Dentro de "Consultar Feedbacks" não há sub-abas Recebidos/Enviados; a lista exibe uma única lista (na prática, a implementação atual lista os feedbacks que o usuário enviou, via API Listar). Não há separação visual Recebidos vs Enviados.

**Explicação:**  
Documentação e PBI exigem abas Recebidos e Enviados. Código tem só a lista única na aba Consultar, sem sub-abas. Impacto: critério de aceite 16010 não atendido. É ausência de implementação (e possivelmente de endpoint para "recebidos").

---

### Divergência 4 – Edição em até 60 minutos

**De:**  
negocio.md e PBIs: permitir edição em até 1 hora (60 min) do envio; apresentar possibilidade de edição nessa janela; card com ícone de edição.

**Para:**  
Não há fluxo de edição na UI (sem tela/modal de edição, sem countdown, sem botão "Salvar alterações"). Não há endpoint de edição referenciado no código. Apenas "Ver Detalhes" no card (somente leitura).

**Explicação:**  
Requisito documentado não implementado. Impacto: critério de aceite não atendido. É ausência de implementação.

---

### Divergência 5 – Tipo de Interação

**De:**  
negocio.md: "Categorização Obrigatória: O formulário deve exigir a seleção de um 'Tipo de Interação' e uma 'Data' para ser enviado" (PBI 15916).

**Para:**  
Na entidade e no formulário existe o campo "Contexto" (texto livre), não um campo categórico "Tipo de Interação". Não há select/opções para "Tipo de Interação".

**Explicação:**  
Documentação fala em "Tipo de Interação" como categorização; código tem apenas "Contexto" (texto). Pode ser divergência de nomenclatura/escopo ou ausência de campo categórico. Impacto: alinhar se "Tipo de Interação" é distinto de "Contexto".

---

## [Categoria: Desenvolvimento]

### Divergência 6 – Entidade e payload sem Relacionamento e Tipo de Interação

**De:**  
desenvolvimento.md (após consolidação): evolução prevê campos "relacionamento" e "tipo de interação" no contrato e na API.

**Para:**  
`Feedback360Item` e `Feedback360CreatePayload` em `Feedback360.ts` não possuem propriedades `relacionamento` nem `tipoInteracao` (ou equivalente). A API de criação não envia esses campos.

**Explicação:**  
Contrato de domínio e payload não refletem os campos exigidos nas PBIs. Impacto: quando backend e UI forem implementados, será necessário incluir esses campos na entidade e no payload.

---

### Divergência 7 – Busca preditiva (autocomplete) e exibição de e-mail

**De:**  
PBI 16005 e negocio.md: "Implementar um campo de busca preditiva (autocomplete)", "Exibir e-mail para evitar seleção de homônimos", busca também por e-mail.

**Para:**  
- O hook carrega a lista completa de colaboradores uma vez (`ListarColaboradoresOrgUseCase` com `busca: '', limite: 50000`) ao abrir a aba "inserir"; o filtro é feito no cliente pelo componente `Command` (CommandInput). Não há chamada à API com `busca` dinâmica durante a digitação (busca preditiva server-side).
- No item do colaborador só é exibido `nm_Profissional ?? codigoColaboradorInterno`; não há exibição de e-mail. A entidade `ColaboradorCch` não possui campo `email` (apenas `cd_Profissional`, `nm_Profissional`, `codigoColaboradorInterno`).

**Explicação:**  
Busca é client-side sobre lista já carregada; documentação fala em busca preditiva (habitualmente associada a busca no servidor conforme digitação). E-mail não está no tipo nem na UI. Impacto: critérios de aceite de busca e de exibição de e-mail não atendidos ou parcialmente atendidos. É implementação divergente (busca) e ausência (e-mail no tipo e na UI).

---

## [Categoria: Design]

### Divergência 8 – Filtros na aba Consultar

**De:**  
design.md e negocio.md (PBI 16010): Filtros por data, sentimento, relacionamento e colaborador.

**Para:**  
Em `Feedback360.tsx` na aba "consultar" existem apenas "Filtrar por data" (input date) e "Filtrar por sentimento" (Select). Não há filtros por "relacionamento" nem por "colaborador".

**Explicação:**  
Dois dos quatro filtros documentados estão ausentes. Impacto: UX e critério de aceite 16010. É ausência de implementação (e depende dos campos relacionamento e listagem por colaborador).

---

### Divergência 9 – Conteúdo do card (Assunto, Avatar, Comentário completo)

**De:**  
PBI 16010 e design.md: Cada card deve exibir obrigatoriamente Assunto, Nome do Interagente, Data, Reação, Relacionamento e Comentário qualitativo completo; layout deve garantir legibilidade de textos longos sem cortes abruptos; exibição do avatar do interagente.

**Para:**  
- Não há campo "Assunto" na entidade; o card usa como título "Feedback para {item.nomeColaborador}" (não um campo Assunto).
- Não há avatar no card; apenas texto (nome, data, badges de sentimento, trecho de contexto/comentário).
- O comentário na listagem é truncado com `truncate(item.contexto || item.comentario || '', 120)`; não é exibido o comentário completo no card (apenas no modal "Ver Detalhes").
- Não há badge/tag de "Relacionamento" no card (campo não existe no modelo).

**Explicação:**  
Documentação exige Assunto, avatar, Relacionamento e comentário completo no card. Código não tem Assunto como campo, não exibe avatar, não exibe Relacionamento e trunca o texto no card. Impacto: critérios de aceite e legibilidade. É implementação divergente e ausência de campos/UI.

---

### Divergência 10 – Sinalizador visual para novo feedback em tempo real

**De:**  
negocio.md / PBI 16010: "Implementar um sinalizador visual (ex: destaque temporário ou badge) quando um novo feedback entrar na lista em tempo real."

**Para:**  
Não há sinalizador, badge ou destaque para feedback novo; não há mecanismo de atualização em tempo real (WebSocket ou polling) na listagem.

**Explicação:**  
Requisito documentado não implementado. Impacto: critério de aceite 16010. É ausência de implementação.

---

### Divergência 11 – UI de edição (countdown, Salvar alterações)

**De:**  
design.md: Barra/banner "Editando feedback" com countdown (ex.: "59 min 43s restantes"), botão Cancelar, instrução e botão "Salvar alterações".

**Para:**  
Não existe essa UI na aplicação; não há estado nem componente para modo de edição com countdown.

**Explicação:**  
Design documentado para edição em 60 min não está implementado. Impacto: consistência com negócio e design. É ausência de implementação.

---

## Resumo

| # | Categoria    | Resumo da divergência |
|---|-------------|------------------------|
| 1 | Negócio     | Comentário sempre obrigatório (doc) vs obrigatório só nível 1–2 (código) |
| 2 | Negócio     | Campo Relacionamento ausente no código |
| 3 | Negócio     | Abas Recebidos/Enviados ausentes; só uma lista em Consultar |
| 4 | Negócio     | Edição em 60 min não implementada |
| 5 | Negócio     | "Tipo de Interação" não existe como campo; existe "Contexto" (texto) |
| 6 | Desenvolvimento | Entidade/payload sem relacionamento e tipo de interação |
| 7 | Desenvolvimento | Busca client-side (não preditiva server-side); e-mail não exibido nem no tipo ColaboradorCch |
| 8 | Design      | Filtros por relacionamento e colaborador ausentes |
| 9 | Design      | Card sem Assunto, avatar, Relacionamento; comentário truncado (120) no card |
| 10 | Design     | Sinalizador de novo feedback em tempo real ausente |
| 11 | Design     | UI de edição com countdown e "Salvar alterações" ausente |

Documentação temporária: este arquivo deve ser usado como fonte para ajustes em `negocio.md`, `desenvolvimento.md` e `design.md`. Deletar ou arquivar `divergencias.md` somente quando não houver mais divergências a tratar ou quando forem convertidas em "Pontos que Precisam de Definição".

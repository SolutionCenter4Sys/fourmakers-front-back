# Feedback (MVP) – Documentação de Negócio

**Feature**: 15916 – MVP Feedback - Oferecer Feedback (Interação Pontual)  
**Rota**: `/feedback360`  
**Última atualização**: Fevereiro 2026

---

## 1. Visão Geral e Objetivo

Como um colaborador que deseja reconhecer ou apoiar o crescimento de um colega, eu quero **selecionar uma pessoa e registrar um feedback rápido e contextual sobre uma interação específica**, para contribuir para uma cultura de melhoria contínua sem que o processo seja burocrático ou demorado.

### 1.1 Dores que Resolvemos

- **Burocracia e Esquecimento**: Evitar que o usuário desista de dar feedback por menus complexos ou listas infinitas.
- **Falta de Contexto**: Evitar feedbacks genéricos que o receptor não associa a um evento real.
- **Ansiedade por Falta de Dados**: Notas baixas sem justificativa geram ansiedade; o fluxo força feedback construtivo.

### 1.2 Valor Entregue

- **Redução de Fricção**: Foco no indivíduo no início do fluxo para direcionar o reconhecimento.
- **Memória Organizacional**: Registrar "onde" e "quando" de cada interação.
- **Segurança Psicológica**: Agilidade (emojis) com profundidade (campo livre obrigatório).

---

## 2. Regras de Negócio

### 2.1 Seleção de Colaborador (PBI 16005)

- Busca e seleção do colega que participou da interação no **início do fluxo**.
- **Busca preditiva (autocomplete)** obrigatória.
- Exibir **apenas colaboradores ativos** na organização.
- Exibir **e-mail** para evitar seleção de homônimos (quando aplicável); busca também por e-mail.
- **Foto ou e-mail**: identificação visual ou por e-mail.
- **Bloquear avanço** para o formulário se nenhuma pessoa for selecionada.
- **Métrica**: Tempo médio de seleção inferior a 5 segundos; seleção possível em menos de 5 segundos.

### 2.2 Campos Obrigatórios do Formulário

| Campo | Obrigatório | Observação |
|-------|-------------|------------|
| Colaborador (destinatário) | Sim | Código interno; deve existir e estar ativo na org. |
| Tipo de Interação | Sim | Categorização obrigatória (PBI 15916). |
| Data (da interação) | Sim | Formato YYYY-MM-DD. |
| Relacionamento | Sim | Opções: Colega, Liderança, Pares, Outro (PBI 16005). |
| Avaliação geral (nota/reação) | Sim | Escala 1 a 5 com emojis. |
| Comentário / Justificativa (texto livre) | Sempre obrigatório | Bloquear botão "Enviar" se campo de texto estiver vazio (PBI 16005). |

**Regra adicional (doc técnica existente):** Quando o nível de avaliação for **1 ou 2**, o comentário é obrigatório com mensagem específica; nas PBIs atuais o comentário é **sempre** obrigatório.

### 2.3 Avaliação Geral (Nota – Reação)

- **Escala visual de emojis (1 a 5)** para avaliação geral.
- Valores fixos:
  - 1: Precisa evoluir bastante
  - 2: Abaixo do esperado
  - 3: Dentro do esperado
  - 4: Acima do esperado
  - 5: Referência para os demais

### 2.4 Relacionamento

- **Campo obrigatório** com opções: **Colega**, **Liderança**, **Pares**, **Outro** (PBI 15916 / 16005).

### 2.5 Persistência e Reatividade

- Dado gravado deve **refletir em até 5 segundos** no dashboard de quem recebeu, sem recarregar a página (PBI 16010: latência &lt; 5 s).
- **Persistência reativa**: atualização em tempo real na lista.

### 2.6 Edição

- **Permitir edição em até 1 hora** (60 minutos) contados do envio do feedback (PBI 16005 / 16010).
- Apresentar possibilidade de edição em até 60 minutos do envio.

### 2.7 Quem Dá / Sobre Quem / O Que Listar

- **Quem dá**: Usuário autenticado (token).
- **Sobre quem**: Colaboradores retornados pela API de colaboradores da org (ativos).
- **Listagem**: Abas **Feedbacks Recebidos** e **Feedbacks Enviados** (PBI 16010); lista em ordem cronológica decrescente (mais recente no topo).

---

## 3. Critérios de Aceite (Definition of Done)

- Seleção imediata: buscar e selecionar colaborador em &lt; 5 s no início do fluxo (foto ou e-mail; só colaboradores ativos).
- Definir nível de relacionamento com a pessoa a quem se oferece feedback.
- Categorização obrigatória: Tipo de Interação e Data para envio.
- Nota (reação) em escala 1–5 com emojis.
- Feedback construtivo forçado: campo de texto obrigatório.
- Persistência reativa: dado gravado reflete em até 5 s no dashboard do receptor.
- Abas Recebidos / Enviados; filtros: data, sentimento, relacionamento, colaborador.
- Card com: Assunto, Nome do Interagente, Data, Reação, Relacionamento, Comentário completo; legibilidade de textos longos; avatar do interagente.
- Edição em até 60 minutos.
- Logs de ações significativas (conforme projeto).

---

## 4. Métricas de Sucesso

- **Taxa de conversão**: Menos de 5% de abandono do fluxo após seleção inicial.
- **Qualidade qualitativa**: Tamanho médio dos comentários &gt; 30 caracteres.
- **Volume**: Aumento no número de feedbacks pontuais registrados por mês.
- **Contexto definido**: 100% dos feedbacks registrados com contexto definido (PBI 16005).
- **Tempo de seleção**: Tempo médio de seleção inferior a 5 segundos (PBI 16005).

---

## 5. Abas e Filtros (PBI 16010)

- **Abas**: "Feedbacks Recebidos" e "Feedbacks Enviados".
- **Ordenação**: Automática em ordem cronológica decrescente (mais recente no topo).
- **Navegação**: Localizar qualquer feedback com no máximo 3 cliques.
- **Filtros**: Data, Sentimento, Relacionamento, Colaborador.
- **Sinalizador visual**: Destaque temporário ou badge quando novo feedback entrar em tempo real.
- **Latência**: Entre envio e aparição no histórico &lt; 5 segundos.

---

## 6. Estado Atual da Implementação (alinhado ao código)

- **Comentário**: Na implementação atual o comentário é obrigatório **apenas quando o nível é 1 ou 2**; para níveis 3–5 o envio é permitido sem comentário (divergente das PBIs que pedem sempre obrigatório).
- **Relacionamento**: Campo não existente na entidade nem no formulário.
- **Abas Recebidos/Enviados**: Não implementadas; na aba "Consultar Feedbacks" há apenas uma lista (feedbacks enviados pelo usuário).
- **Edição em 60 min**: Não implementada (sem UI de edição, countdown ou endpoint).
- **Tipo de Interação**: Não existe como campo categórico; o formulário possui apenas "Contexto" (texto livre).
- **Filtros**: Apenas data e sentimento; filtros por relacionamento e colaborador não existem.
- **Card**: Sem campo "Assunto" (título é "Feedback para {nomeColaborador}"); sem avatar; comentário truncado (120 caracteres) na listagem; sem tag de Relacionamento. Comentário completo só no modal "Ver Detalhes".
- **Busca colaborador**: Lista carregada uma vez no cliente; filtro no componente (não busca preditiva no servidor por digitação). E-mail não exibido (entidade ColaboradorCch não possui email).
- **Sinalizador de novo feedback em tempo real**: Não implementado.

---

## 7. Pontos que Precisam de Definição

- **Comentário sempre obrigatório vs condicional**: PBIs indicam "Justificativa Obrigatória" (sempre). Código exige apenas para níveis 1 e 2. Definir regra final com produto.
- **Tipo de Interação**: PBIs citam "Tipo de Interação" obrigatório; código tem apenas "Contexto" (texto). Definir se "Tipo de Interação" é campo categórico distinto ou equivalente ao contexto.
- **Assunto no card**: PBI 16010 exige "Assunto" no card. Código não tem campo Assunto; título do card é "Feedback para {nomeColaborador}". Definir se Assunto é campo novo ou título derivado.

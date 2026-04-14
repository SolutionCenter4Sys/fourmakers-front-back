# Nova Agenda com Cliente — Regras de Acesso (LEGACY vs Implementação)

**Data:** 25/02/2026  
**Referência:** `IMPLEMENTATION/Agendas Comerciais Correcao UX/LEGACY_REFERENCE.md`

---

## O que o LEGACY define

### 1. Criar agenda (geral)

- **Quem pode:** qualquer colaborador autenticado.
- **Autorização:** nenhuma checagem obrigatória no frontend.
- **Fonte:** seção "Creating an Agenda (CriarAgenda)" — *"Who Can Create: ANY authenticated collaborator"*, *"Authorization Check: None required"*.

### 2. Criar agenda filha (Nova Agenda com Cliente)

Validações descritas no backend ao criar com `agendaPaiId`:

1. `agendaPaiId` referencia uma agenda existente.
2. A agenda pai não é ela mesma uma filha (evitar referência circular).
3. **"User has permission to create agendas for this client"** (usuário tem permissão para criar agendas para este cliente).

**Regras de negócio (tabela):**

| Regra                 | Validação                                      | Onde aplicar   |
|------------------------|------------------------------------------------|----------------|
| Creator permissions    | User must have create permission for client    | Backend + Frontend |

Ou seja: a regra de **permissão para criar agendas para o cliente** deve ser aplicada em **backend e frontend**.

---

## O que a implementação atual faz

- **Quem vê "Nova Agenda com Cliente":**
  - **DetalhesAgendaModal:** qualquer usuário que abre o detalhe de uma agenda (quando `tipo === 'agenda'` e existe `item.agendaId`).
  - **AgendaCard:** qualquer usuário que vê o card da agenda (botão com ícone PlusCircle).
- **Quem pode submeter:** qualquer usuário autenticado que consiga abrir o modal e preencher o formulário (cliente vem pré-preenchido e travado quando há `agendaPai`).
- **Frontend:** não há checagem de “create permission for client” nem de “participante da agenda pai”. Não usamos `verificarSeUsuarioECriador` nem `souParticipante` para exibir ou bloquear “Nova Agenda com Cliente”.

---

## Conclusão: está alinhado?

- **Parcialmente.**
  - **Criar agenda (geral):** alinhado — LEGACY não exige checagem no frontend; qualquer autenticado pode criar.
  - **Criar agenda filha:**
    - **Backend:** o LEGACY deixa claro que o backend deve validar “user has permission to create agendas for this client”. Se o backend fizer essa validação, chamadas de usuários sem permissão serão rejeitadas.
    - **Frontend:** o LEGACY diz que a regra de “creator permissions” (permissão para criar para o cliente) deve ser aplicada também no frontend. Hoje **não** há essa checagem: não restringimos quem vê ou quem pode acionar “Nova Agenda com Cliente”.

---

## Recomendações

1. **Confirmar com o backend** que a validação *“user has permission to create agendas for this client”* existe ao criar agenda com `agendaPaiId`. Se existir, a segurança não depende só do frontend.
2. **Se quiser alinhar ao “Frontend” do LEGACY:**
   - Ter um critério claro do que é “create permission for client” (ex.: endpoint ou regra de negócio).
   - No frontend, usar esse critério para:
     - Mostrar o botão “Nova Agenda com Cliente” apenas para quem tem permissão, **ou**
     - Exibir mensagem clara quando o usuário não tiver permissão (e o backend rejeitar).
3. **Alternativa conservadora (sem novo endpoint):** restringir “Nova Agenda com Cliente” a **participantes da agenda pai** (criador ou na lista de colaboradores/gestores/participantes), como proxy de “tem relação com esta agenda/cliente”. Isso reduz a superfície de uso sem depender de um endpoint específico de permissão por cliente.

---

## Referências no LEGACY_REFERENCE.md

- Linhas 1780–1782: validações ao criar child agenda (incl. “User has permission to create agendas for this client”).
- Linhas 2016–2017: tabela de regras (Creator permissions — Backend + Frontend).
- Linhas 2036–2042: criação de agenda em geral (any authenticated collaborator; no authorization check).

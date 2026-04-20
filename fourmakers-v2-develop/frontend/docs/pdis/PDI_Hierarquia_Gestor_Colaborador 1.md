# PDI — Como é identificado que um colaborador pertence à gestão de um gestor

## Contexto

Na tabela **`pdi`** **não é armazenado o gestor** do colaborador. O vínculo “este PDI é do time de qual gestor?” é resolvido **sempre pela hierarquia organizacional**, não por campo na própria tabela PDI.

---

## Tabela de hierarquia: `tb_colaborador_hierarquia`

A relação **gestor ↔ subordinado** é definida na tabela:

**`tb_colaborador_hierarquia`**

| Coluna                    | Descrição                                                                 |
|---------------------------|---------------------------------------------------------------------------|
| `cod_colaborador_externo` | Código do **subordinado** (colaborador que reporta ao gestor)            |
| `cod_colaborador_superior` | Código do **gestor** (superior direto)                                    |
| `tb_org_id`               | Organização (org)                                                         |

- Chave primária: `(cod_colaborador_externo, cod_colaborador_superior, tb_org_id)`.
- Os códigos usados aqui são os **códigos externos** do colaborador na org (`cod_colaborador_externo` de `tb_colaborador_org`), não o CPF diretamente.
- O **CPF** (usado como `colaborador_id` no PDI) é obtido de `tb_colaborador_org.codigo_interno_colaborador` para cada `cod_colaborador_externo`.

Ou seja: **“colaborador X faz parte da gestão do gestor Y”** é determinado por linhas em `tb_colaborador_hierarquia` onde o gestor é `cod_colaborador_superior` e o colaborador é `cod_colaborador_externo`.

---

## Fluxo no backend (PDI e métricas)

**Contexto das APIs:** Os endpoints de **Meus PDIs** e **PDIs do time** (criar, listar, action plans, evidências) ficam na API **GestaoPessoa** (`api/GestaoPessoa/Pdi`). As **métricas PDI** podem estar na API **Colaborador** (`api/Colaborador/Pdi/Metricas`) dependendo do projeto. Em ambos os casos, a regra de "quem é do time" usa a mesma hierarquia abaixo.

### 1. Quem usa a hierarquia

- **Métricas PDI** (`PdiMetricasService`): “meus PDIs”, “métricas do time” e “métricas por colaborador do time”.
- **PDIs do time** (`PdisDoTimeService`): listar PDIs do time, listar por colaborador, obter PDI completo, criar/atualizar PDI em nome do colaborador, action plans, evidências (incluindo download de evidência).

Todos usam o mesmo repositório para hierarquia:

- **`IBuscaColaboradorRepository.GetSubordinadosColaboradorOrg(string cpf, int orgId)`**

### 2. Implementação de `GetSubordinadosColaboradorOrg`

**Arquivo:** `Colaboracao.Infra/Repositories/Colaborador/BuscaColaboradorRepository.cs`

Resumo do fluxo:

1. **Gestor (CPF do JWT)**  
   Busca em `tb_colaborador_org` o registro do usuário logado por `codigo_interno_colaborador == cpf` e `tb_org_id == orgId` para obter o **`cod_colaborador_externo`** do gestor.

2. **Subordinados diretos**  
   Consulta **`tb_colaborador_hierarquia`** com:
   - `cod_colaborador_superior` = `cod_colaborador_externo` do gestor  
   - `tb_org_id` = orgId  

   Obtém a lista de **`cod_colaborador_externo`** dos subordinados.

3. **Conversão para “código interno” (CPF)**  
   Para cada `cod_colaborador_externo` de subordinado, busca em `tb_colaborador_org` o registro com esse `cod_colaborador_externo` e `tb_org_id`, e lê o **`codigo_interno_colaborador`** (CPF). Esse CPF é o mesmo usado em **`pdi.colaborador_id`**.

4. **Retorno**  
   Devolve uma lista de `ColaboradorOrgDTO` (com propriedade **`Cpf`** = `codigo_interno_colaborador`).

Ou seja: a identificação de que um **colaborador_id** (CPF) “faz parte da gestão de tal gestor” vem **inteiramente** da tabela **`tb_colaborador_hierarquia`** + **`tb_colaborador_org`**, e não de nenhum campo na tabela `pdi`.

---

## Uso nos serviços PDI

### PdiMetricasService

- **Métricas do colaborador:** usa apenas o CPF do usuário logado (sem hierarquia).
- **Métricas do gestor:** monta o conjunto de CPFs = { CPF do gestor } ∪ { CPF de cada subordinado retornado por `GetSubordinadosColaboradorOrg(cpfGestor, orgId)` }. Consulta PDIs onde `pdi.colaborador_id` está nesse conjunto.
- **Métricas por colaborador do time:** verifica se o `colaboradorId` informado está nesse mesmo conjunto (gestor + subordinados). Só então busca métricas daquele colaborador.

### PdisDoTimeService

- **Listar PDIs do time / por colaborador / obter PDI completo:** usa o mesmo conjunto de CPFs permitidos (gestor + subordinados) via `ObterCodigosPermitidos(cpfGestor, orgId)`, que internamente chama `GetSubordinadosColaboradorOrg`.
- **Criar/atualizar PDI, action plans, evidências “para um colaborador”:** antes de qualquer operação, verifica se o `colaboradorId` (CPF) está em `ObterCodigosPermitidos`. Se não estiver, retorna erro do tipo “Colaborador não pertence ao seu time.”.

Em todos os casos, **“pertence ao seu time”** = existe relação na **`tb_colaborador_hierarquia`** (gestor como superior, colaborador como subordinado) na mesma org, ou é o próprio gestor.

---

## Importante: apenas um nível (subordinados diretos)

`GetSubordinadosColaboradorOrg` retorna **somente subordinados diretos** (um nível abaixo do gestor).

- **Exemplo:** Gestor A → Subordinado B → Subordinado C.  
  Para o gestor A, entram no “time” apenas A e B. **C não entra** nas métricas PDI nem nos “PDIs do time” de A neste backend.

Outros módulos (ex.: Gestão de Desempenho em `GestaoDesempenhoGestorService`) usam lógica **recursiva** (subordinados de subordinados). No PDI atual, a regra é **apenas direta**.

---

## Origem dos dados da hierarquia

- **Carga:** A tabela `tb_colaborador_hierarquia` é populada por rotinas de carga (ex.: `RotinaGeraCargaColaboradorHierarquia`) que chamam `ICargaFourmakerService.InsereCargaHierarquiaColaborador`.
- **DTO de carga:** `ColaboradorHierarquiaCargaDTO` com `IdentificadorColaborador` (subordinado) e `IdentificadorSuperior` (gestor), mapeados para `cod_colaborador_externo` e `cod_colaborador_superior`.
- **View auxiliar:** Existe a view **`vw_gestores_colaboradores_org`** que junta `tb_colaborador_hierarquia` com `tb_colaborador_org` e `tb_colaborador` para expor gestor/subordinado com nomes e CPFs (em outros relatórios/consultas). O PDI não depende dessa view; usa apenas `GetSubordinadosColaboradorOrg` e `tb_colaborador_hierarquia` + `tb_colaborador_org`.

---

## Resumo

| Pergunta | Resposta |
|----------|----------|
| O gestor é salvo na tabela `pdi`? | **Não.** |
| Como se sabe que um `colaborador_id` é do time de um gestor? | Pela tabela **`tb_colaborador_hierarquia`**: o gestor é o `cod_colaborador_superior` e o colaborador é o `cod_colaborador_externo` (convertido a CPF via `tb_colaborador_org`). |
| Onde isso é usado no código? | Em **`BuscaColaboradorRepository.GetSubordinadosColaboradorOrg`**, consumido por **`PdiMetricasService`** e **`PdisDoTimeService`**. |
| É hierarquia recursiva? | **Não** no PDI: apenas **subordinados diretos** do gestor. |

Assim, a orientação de “olhar para a tabela que contém hierarquia” está correta: essa tabela é a **`tb_colaborador_hierarquia`**, e é ela que determina se um dado `colaborador_id` faz parte da gestão de um determinado gestor no contexto do PDI.

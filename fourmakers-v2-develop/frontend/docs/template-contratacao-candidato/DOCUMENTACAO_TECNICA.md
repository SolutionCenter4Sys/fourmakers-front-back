# Template de Contratação Refatorado – Documentação Técnica

Documentação técnica da feature **Template de Contratação do Candidato** para portabilidade e paridade com o código React atual. Regras de negócio, contratos, endpoints e validações descritos de forma agnóstica, com notas sobre o que está implementado no React.

---

## 1. Visão geral e objetivo

A feature gerencia o ciclo do template de contratação de um candidato:

1. Carrega template existente por candidatura (ou modelo vazio quando não existe).
2. Permite edição de dados pessoais, admissão, acessos, equipamentos, grupos e remuneração.
3. Salva o template (create/update) pelos endpoints de persistência.
4. PDF é fluxo separado (endpoint legado), não acoplado ao salvar.
4. (Futuro) Envio por e-mail para grupos/departamentos.

### Escopo funcional no React atual

- Rota: **`/gestaodevagas/template-contratacao/:idCandidatura`** (não `vagas/templateRefatorado`).
- Contexto de entrada: **`idCandidatura`** vem da URL; **`vagaId`** opcional no `location.state` para link “Voltar”.
- Consulta por candidatura; edição; persistência via **CriarTemplate/AtualizarTemplate**.
- Auto preenchimento de endereço por CEP (ViaCEP).
- **Não implementado no React:** modal de envio de e-mail (`EnviarEmailTemplateCandidato`) e fluxo de PDF legado após salvar.

---

## 2. Autenticação e contexto

- Todas as chamadas: **`Authorization: Bearer {token}`**.
- Token e `idCandidatura` (da URL) são suficientes para carregar e salvar no React.

### Create vs Update (doc original vs React)

- **Doc original:** create quando `idTemplateParaEdicao === "00000000-0000-0000-0000-000000000000"`; senão update com `AtualizarTemplate/{idTemplate}`.
- **React atual:** segue essa regra. Se `template.id` for GUID vazio (ou vazio), chama **`POST Vaga/Contratacao/CriarTemplate`**; caso contrário chama **`PUT Vaga/Contratacao/AtualizarTemplate/{idTemplate}`**.

---

## 3. Endpoints – paridade com o React

Base: mesma API (ex.: `fourmakershub-api.dev.fourmakers.io`). Prefixo comum: `/api/`.

| # | Endpoint | Uso no React | Observação |
|---|----------|--------------|------------|
| 1 | `GET Candidatura/ObterCandidaturaPorId?idCandidatura={id}` | Sim | Candidatura + detalhes vaga; mapeado para `CandidaturaDetalhesVaga` no repositório. |
| 2 | `GET Vaga/Contratacao/ObterTemplatePorCandidatura/{idCandidatura}` | Sim | Template principal; modo criação quando `retorno.id` é GUID vazio. |
| 3 | `GET Vaga/Contratacao/ListarDiretorios` | Sim | Catálogo diretórios; fallback para constantes se falhar. |
| 4 | `GET Vaga/Contratacao/ListarSistemasLiberados` | Sim | Catálogo sistemas; fallback para constantes. |
| 5 | `GET Vaga/Contratacao/ListarGruposEmails` | Sim | Catálogo grupos de e-mail; fallback para constantes. |
| 6 | `GET Vaga/Contratacao/ListarEquipamentosPadroesAninhados` | Sim | Equipamentos/cargos por área. |
| 7 | `GET GestaoDeAlocados/ListarModelosTrabalho` | Sim | Modelos de trabalho (Remoto/Híbrido/Presencial). |
| 8 | `GET MapaDeAlocacao/ListarColaboradoresOrg?busca=&cursor=0&limite=...` | Sim | Autocomplete Analista de R&S e Superior Imediato. **React usa `limite=50`** (não 999999) para evitar travar a tela. |
| 9 | `POST Vaga/Contratacao/CriarTemplate` | Sim | Persistência quando `template.id` é GUID vazio. |
| 10 | `PUT Vaga/Contratacao/AtualizarTemplate/{idTemplate}` | Sim | Persistência quando template já existe. |
| 11 | `GET Candidatura/GetPdfTemplateDeAdmissaoPdf?...` | Não | Endpoint legado de PDF; ainda não acionado no fluxo atual React após salvar. |
| 12 | `POST Vaga/Contratacao/EnviarEmailTemplateCandidato` (multipart) | Não | Modal de envio não implementado no React. |
| 13 | `GET https://viacep.com.br/ws/{cep}/json/` | Sim | ViaCEP para auto preenchimento de endereço. |

---

## 4. Fluxo no React (alto nível)

1. Montagem: com `idCandidatura` e `token`, chama em paralelo **ObterTemplatePorCandidatura** e **ObterCandidaturaPorId**; faz merge no repositório (candidatura mapeada para detalhes da vaga).
2. Em paralelo (com token): **ListarEquipamentosPadroesAninhados**; **ListarDiretorios**, **ListarSistemasLiberados**, **ListarGruposEmails**; **ListarModelosTrabalho** (via slice Redux). Colaboradores: sob demanda no **ColaboradorSearchField** (debounce 350 ms, limite 50).
3. Edição: estado no hook `useTemplateContratacaoCandidato`; validação no “Salvar e baixar”.
4. Salvar: validação de campos obrigatórios; montagem do payload (incl. `contatoPrincipal` = DDD + telefone); chamada de **CriarTemplate** ou **AtualizarTemplate** conforme `template.id`.

---

## 5. Regras de negócio (aplicadas no React)

- **Diretório:** marcar `escrita` implica `leitura` (tratado na UI ao setar permissões).
- **Modelo de trabalho híbrido:** habilita “Dias no presencial” (1 a 4).
- **Datas:** formulário em `yyyy-MM-dd` (input date); payload envia ISO/string conforme entidade.
- **CEP:** enviado sem máscara (apenas dígitos onde aplicável).
- **Telefone:** `contatoPrincipal` = concatenação DDD + número (sem hífen no payload).
- **Create vs Update:** decidido pelo backend no SalvarEBaixarTemplate; front envia payload completo.
- **Listas (sistemas/diretórios/grupos):** itens com `id` e `descricao`; seleção enviada com `id` quando disponível da API.

---

## 6. Contratos de dados (payload principal)

O payload enviado no React segue a entidade **TemplateContratacaoData** e inclui (entre outros):

- `id`, `candidatoVagaId`, `colaboradorCodigoInternoColaboradorAnalista`, `colaboradorCodigoInternoColaboradorSuperiorImediato`
- `nomeCompleto`, `cargo`, `dataInicio`, `horarioJornada`, `tipoHorarioJornada`
- `documentoColaborador`, `rgColaborador`, `dataNascimento`, `dddColaborador`, `contatoPrincipal` (DDD+telefone), `tamanhoCamiseta`, `emailPessoal`, `emailCorporativo`, `loginRede`, `tipoLoginRede`
- `equipamentoPadraoCargoFuncaoId`, `grupoAreaEquipamentoPadraoCargoFuncao`, `hardware`, `softwaresNecessarios`, `observacoesAcessoUsuario`, `observacoesAprovadorAcessos`
- `sistemasLiberados[]` (`id`, `descricao`), `diretorios[]` (`id`, `descricao`, `leitura`, `escrita`), `gruposEmails[]` (`id`, `descricao`)
- `endereco`: `cep`, `endereco`, `complemento`, `numero`, `bairro`, `cidade`, `estado`
- Benefícios/remuneração: `salario`, `vr`, `va`, etc.; booleanos para celular, planoDados, cartaoVisitas; `modeloTrabalhoId`, `quantidadeDiasPresencial`
- `saude`: `pcd`, `tipoPcd`, `enumPCD`, etc.

**Grupos de e-mail:** no React envio é array de objetos com `id`/`descricao` (e opcionalmente flags); padronizar com backend se houver outro contrato (ex.: só `id` ou `leitura`).

---

## 7. Validações de formulário (React)

Validação centralizada no hook ao clicar em “Salvar e baixar Template”:

- **Obrigatórios:** Data Início, Analista de R&S, Tipo equipamento (grupo/cargo), CPF (11 dígitos), RG, Data Nascimento, DDD (2 dígitos), Telefone (8–9 dígitos), Nome Completo, Tamanho Camiseta, E-mail Pessoal, CEP (8 dígitos), Número, Bairro, Cidade, Estado, Superior Imediato, Observações de acesso do Usuário.
- Mensagem geral: **“Preencha os campos obrigatórios.”**; erros por campo em `formErrors`.

---

## 8. Resposta padrão das APIs

```json
{
  "retorno": {},
  "sucesso": true,
  "mensagem": "string|null",
  "erros": []
}
```

- Sucesso: uso de `retorno`.
- Erro: uso de `mensagem` (e/ou `erros`) para toast/diálogo.

**ListarColaboradoresOrg:** resposta usa **`ColaboradoresCchResult`** (array); itens com `codigoColaboradorInterno`, `nm_Profissional`, `cd_Profissional`.

---

## 9. Checklist de paridade (doc original ↔ React)

| Item | React |
|------|--------|
| Ler template por candidatura + catálogos em paralelo | Sim (template+candidatura; listas em paralelo) |
| Merge por id para seleções (diretórios/sistemas/grupos) | Sim (opções da API com id; fallback constantes) |
| Regra diretório escrita => leitura | Sim (UI) |
| Validação mínima no salvar | Sim (hook) |
| Salvar create/update | Sim (CriarTemplate/AtualizarTemplate) |
| PDF | Não no fluxo atual (endpoint legado separado) |
| Modal envio e-mail (multipart) | Não implementado |
| Mensagens de erro (mensagem) | Sim (toast) |
| ListarColaboradoresOrg com limite | **50** (não 999999) |
| Hooks sempre na mesma ordem | Sim (useCallback antes de qualquer return condicional) |

---

## 10. Arquivos de referência (React)

- `src/presentation/pages/TemplateContratacaoCandidato.tsx`
- `src/presentation/pages/recrutamento/TemplateContratacaoCandidato.constants.ts`
- `src/presentation/hooks/recrutamento/useTemplateContratacaoCandidato.ts` (import: `@presentation/hooks/recrutamento`)
- `src/presentation/components/template-contratacao/ColaboradorSearchField.tsx`
- `src/domain/entities/TemplateContratacao.ts`
- `src/domain/usecases/ObterTemplateContratacaoPorCandidaturaUseCase.ts`, `SalvarEBaixarTemplateUseCase.ts`, `ListarColaboradoresOrgUseCase.ts`
- `src/data/repositories/TemplateContratacaoRepositoryImpl.ts`
- `src/data/api/VagaApi.ts`, `CandidaturaApi.ts`

Documentação FlutterFlow original: `lib/fourmakers/recrutamento/template_contratacao_refatorado/`, `lib/backend/schema/structs/`, custom actions e `api_calls.dart`.

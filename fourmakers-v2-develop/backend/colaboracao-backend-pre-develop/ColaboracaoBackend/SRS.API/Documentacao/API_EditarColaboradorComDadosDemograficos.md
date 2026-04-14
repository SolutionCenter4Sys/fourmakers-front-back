# Documentação API - Editar Candidatura Colaborador com Dados Demográficos

## Endpoint

**POST** `/api/Candidatura/EditarCandidaturaColaboradorComDadosDemograficos`

## Descrição

Este endpoint permite atualizar simultaneamente os dados do colaborador e seus dados demográficos em uma única requisição. Ele combina as funcionalidades de `EditarDadosColaborador` e `InserirDadosDemograficos`/`AlterarDadosDemograficos`.

## Autenticação

O endpoint requer autenticação via token JWT no header:

```
Authorization: Bearer {token}
```

O CPF do colaborador é obtido automaticamente do token de autenticação do usuário logado.

## Request Body

### Estrutura

```typescript
{
  "Colaborador": {
    // Campos do EditarColaboradorDTO
  },
  "DadosDemograficos": {
    // Campos do DadosDemograficosColaboradorDTO
  }
}
```

### DTOs Completos

#### EditarColaboradorDTO

Consulte a documentação existente do endpoint `EditarDadosColaborador` para os campos completos do `EditarColaboradorDTO`.

#### DadosDemograficosColaboradorDTO

```typescript
{
  // Família & Dependentes
  QuantidadePessoasResidencia: number;        // Mínimo: 1
  DependentesIRPF: number;                    // Mínimo: 0
  PossuiConjuge: boolean;
  DataNascimentoConjuge?: string;              // Formato: "YYYY-MM-DD" (obrigatório se PossuiConjuge = true)
  PossuiFilhos: boolean;
  Filhos?: Array<{                            // Obrigatório se PossuiFilhos = true
    Id?: string;                              // GUID (null para novo registro)
    DataNascimento: string;                   // Formato: "YYYY-MM-DD"
  }>;

  // Seguro Saúde (e Plano Foursys)
  PossuiSeguroSaude: boolean;
  ValorAtualSeguroSaude?: number;             // Valor em R$
  OperadoraSeguroSaude?: string;
  AcomodacaoSeguroSaude?: string;             // "Apartamento" | "Enfermaria" | null
  SeguroSaudePossuiCoparticipacao: boolean;
  ObservacoesSeguroSaude?: string;
  PossuiInteressePlanoFoursys: boolean;
  FaixaEtaria?: string;                       // Ex: "30-40", "20-30"
  CategoriaPlanoSaude?: string;               // "general" | "supervisor" | "executive" | null
  IncluirDependentesPlanoFoursys: boolean;
  QuantidadeDependentesPlanoFoursys: number;  // Mínimo: 1 se IncluirDependentesPlanoFoursys = true
  ValorPlanoDependentes?: number;             // Valor em R$

  // Alimentação
  ValorCartaoRefeicao?: number;               // Valor em R$ (constante: 660)
  ValorCartaoAlimentacao?: number;             // Valor em R$

  // Educação
  EstudaAtualmente: boolean;
  CustoMensalEducacao?: number;               // Valor em R$
  FilhosEstudamAte24Anos: boolean;
  CustoMensalEducacaoFilhos?: number;         // Valor em R$
  CustoTotalEducacao?: number;                // Valor em R$ (derivado)

  // Preferências do Candidato
  ModeloDeTrabalhoPretendido?: string;        // "Presencial" | "Hibrido" | "100% Remoto"
  DiasPresenciaisDesejados?: string;          // "1 dia presencial" | "2 dias presenciais" | "3 dias presenciais" | "4 dias presenciais" (condicional se ModeloDeTrabalhoPretendido = "Hibrido")
  PretencaoLiquidaRef?: number;               // Valor em R$

  // Mobilidade
  DistanciaIdaVolta?: number;                 // Distância em km (Ida/Volta)
}
```

## Exemplo de Request

```json
{
  "Colaborador": {
    "NomeCompleto": "João Silva",
    "DataNascimento": "1990-05-15",
    "Rg": "123456789",
    "EstadoCivil": "Casado",
    "Genero": "Masculino",
    "Etnia": "Branco",
    "Escolaridade": "Superior Completo",
    "Email": "joao.silva@email.com",
    "Celular": "11999999999",
    "Endereco": {
      "Cep": "01310100",
      "Logradouro": "Avenida Paulista",
      "Numero": "1000",
      "Complemento": "Apto 101",
      "Bairro": "Bela Vista",
      "Cidade": "São Paulo",
      "Estado": "SP"
    }
  },
  "DadosDemograficos": {
    "QuantidadePessoasResidencia": 3,
    "DependentesIRPF": 2,
    "PossuiConjuge": true,
    "DataNascimentoConjuge": "1992-08-20",
    "PossuiFilhos": true,
    "Filhos": [
      {
        "Id": null,
        "DataNascimento": "2015-03-20"
      },
      {
        "Id": null,
        "DataNascimento": "2018-07-10"
      }
    ],
    "PossuiSeguroSaude": true,
    "ValorAtualSeguroSaude": 450.50,
    "OperadoraSeguroSaude": "Unimed",
    "AcomodacaoSeguroSaude": "Apartamento",
    "SeguroSaudePossuiCoparticipacao": true,
    "ObservacoesSeguroSaude": "Plano familiar com cobertura nacional",
    "PossuiInteressePlanoFoursys": true,
    "FaixaEtaria": "30-40",
    "CategoriaPlanoSaude": "supervisor",
    "IncluirDependentesPlanoFoursys": true,
    "QuantidadeDependentesPlanoFoursys": 2,
    "ValorPlanoDependentes": 300.00,
    "ValorCartaoRefeicao": 660.00,
    "ValorCartaoAlimentacao": 500.00,
    "EstudaAtualmente": true,
    "CustoMensalEducacao": 800.00,
    "FilhosEstudamAte24Anos": true,
    "CustoMensalEducacaoFilhos": 1200.00,
    "CustoTotalEducacao": 2000.00,
    "ModeloDeTrabalhoPretendido": "Hibrido",
    "DiasPresenciaisDesejados": "2 dias presenciais",
    "PretencaoLiquidaRef": 8000.00,
    "DistanciaIdaVolta": 25.5
  }
}
```

## Response

### Sucesso (200 OK)

```json
{
  "Retorno": {
    // EditarColaboradorDTO atualizado
  },
  "Sucesso": true,
  "Mensagem": "Dados do colaborador e dados demográficos atualizados com sucesso!"
}
```

### Erro de Validação (400 Bad Request)

```json
{
  "Retorno": null,
  "Sucesso": false,
  "Mensagem": "Mensagem de erro específica da validação"
}
```

**Exemplos de mensagens:**
- "Dados do colaborador são obrigatórios."
- "Dados demográficos são obrigatórios."
- "O número de pessoas com quem mora deve ser no mínimo 1."
- "Data de nascimento do cônjuge é obrigatória quando possui cônjuge."
- "Deve informar pelo menos um filho quando possui filhos."
- "A quantidade de dependentes do plano Foursys deve ser no mínimo 1 quando incluir dependentes."

### Erro de Autenticação (401 Unauthorized)

```json
{
  "Retorno": null,
  "Sucesso": false,
  "Mensagem": "Mensagem de erro de autenticação"
}
```

### Erro Interno (500 Internal Server Error)

```json
{
  "Retorno": null,
  "Sucesso": false,
  "Mensagem": "Erro interno ao atualizar dados do colaborador e dados demográficos"
}
```

## Validações Importantes

### Campos Obrigatórios

- `Colaborador`: Obrigatório
- `DadosDemograficos`: Obrigatório
- `QuantidadePessoasResidencia`: >= 1
- `DependentesIRPF`: >= 0

### Validações Condicionais

1. **Cônjuge:**
   - Se `PossuiConjuge = true`: `DataNascimentoConjuge` é obrigatório

2. **Filhos:**
   - Se `PossuiFilhos = true`: deve ter pelo menos 1 filho na lista `Filhos`
   - Cada filho deve ter `DataNascimento` preenchido
   - O campo `Id` pode ser `null` para novos registros (será gerado como GUID)

3. **Plano Foursys:**
   - Se `IncluirDependentesPlanoFoursys = true`: `QuantidadeDependentesPlanoFoursys` deve ser >= 1

4. **Modelo de Trabalho:**
   - Se `ModeloDeTrabalhoPretendido = "Hibrido"`: `DiasPresenciaisDesejados` pode ser informado
   - Valores aceitos para `ModeloDeTrabalhoPretendido`: `"Presencial"`, `"Hibrido"`, `"100% Remoto"`

### Valores Aceitos

- `AcomodacaoSeguroSaude`: `"Apartamento"` | `"Enfermaria"` | `null`
- `CategoriaPlanoSaude`: `"general"` | `"supervisor"` | `"executive"` | `null`
- `ModeloDeTrabalhoPretendido`: `"Presencial"` | `"Hibrido"` | `"100% Remoto"`
- `DiasPresenciaisDesejados`: `"1 dia presencial"` | `"2 dias presenciais"` | `"3 dias presenciais"` | `"4 dias presenciais"`

## Formato de Datas

Todas as datas devem estar no formato ISO 8601: `"YYYY-MM-DD"`

**Exemplos:**
- `"1990-05-15"`
- `"2015-03-20"`

## Comportamento do Endpoint

1. **Atualização do Colaborador:**
   - Atualiza os dados do colaborador usando `AtualizarColaboradorAsync`
   - Usa o CPF do usuário logado (obtido do token)

2. **Dados Demográficos:**
   - Verifica se já existem dados demográficos para o colaborador
   - Se **existir**: chama `AlterarDadosDemograficos`
   - Se **não existir**: chama `InserirDadosDemograficos`
   - Usa o CPF do usuário logado (obtido do token)

3. **Filhos:**
   - Ao alterar dados demográficos, todos os filhos existentes são deletados
   - Os novos filhos informados são inseridos
   - Se um filho tiver `Id` informado, será usado; caso contrário, será gerado um novo GUID

## Notas Importantes

- ⚠️ O CPF do colaborador é obtido automaticamente do token de autenticação
- ⚠️ Não é necessário informar o `CodigoInternoColaborador` no body (diferente dos endpoints individuais)
- ⚠️ Este endpoint realiza duas operações em sequência. Se uma falhar, a outra não será executada
- ⚠️ Para filhos: ao alterar, todos os filhos existentes são removidos e substituídos pelos novos
- ⚠️ O campo `Id` dos filhos pode ser `null` para novos registros (será gerado automaticamente)

## Exemplo de Uso (JavaScript/TypeScript)

```typescript
async function editarColaboradorComDadosDemograficos(
  colaborador: EditarColaboradorDTO,
  dadosDemograficos: DadosDemograficosColaboradorDTO
) {
  try {
    const response = await fetch('/api/Candidatura/EditarCandidaturaColaboradorComDadosDemograficos', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({
        Colaborador: colaborador,
        DadosDemograficos: dadosDemograficos
      })
    });

    const result = await response.json();

    if (result.Sucesso) {
      console.log('Dados atualizados com sucesso!', result.Retorno);
      return result.Retorno;
    } else {
      console.error('Erro:', result.Mensagem);
      throw new Error(result.Mensagem);
    }
  } catch (error) {
    console.error('Erro na requisição:', error);
    throw error;
  }
}
```

## Exemplo de Uso (cURL)

```bash
curl -X POST "https://api.exemplo.com/api/Candidatura/EditarCandidaturaColaboradorComDadosDemograficos" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -d '{
    "Colaborador": {
      "NomeCompleto": "João Silva",
      "DataNascimento": "1990-05-15"
    },
    "DadosDemograficos": {
      "QuantidadePessoasResidencia": 3,
      "DependentesIRPF": 2,
      "PossuiConjuge": true,
      "DataNascimentoConjuge": "1992-08-20",
      "PossuiFilhos": true,
      "Filhos": [
        {
          "Id": null,
          "DataNascimento": "2015-03-20"
        }
      ],
      "ModeloDeTrabalhoPretendido": "Hibrido",
      "DiasPresenciaisDesejados": "2 dias presenciais",
      "PretencaoLiquidaRef": 8000.00,
      "DistanciaIdaVolta": 25.5
    }
  }'
```

## Tratamento de Erros

### Códigos de Status HTTP

- **200 OK**: Operação realizada com sucesso
- **400 Bad Request**: Erro de validação (campos obrigatórios, valores inválidos)
- **401 Unauthorized**: Token inválido ou ausente
- **500 Internal Server Error**: Erro interno do servidor

### Tratamento Recomendado

```typescript
try {
  const result = await editarColaboradorComDadosDemograficos(colaborador, dadosDemograficos);
  // Sucesso
} catch (error) {
  if (error.response?.status === 400) {
    // Erro de validação - mostrar mensagem específica ao usuário
    showError(error.response.data.Mensagem);
  } else if (error.response?.status === 401) {
    // Não autenticado - redirecionar para login
    redirectToLogin();
  } else {
    // Erro genérico
    showError('Erro ao atualizar dados. Tente novamente.');
  }
}
```

## Changelog

- **2025-01-XX**: Endpoint criado
  - Combina atualização de dados do colaborador e dados demográficos
  - Suporte a preferências de trabalho e mobilidade

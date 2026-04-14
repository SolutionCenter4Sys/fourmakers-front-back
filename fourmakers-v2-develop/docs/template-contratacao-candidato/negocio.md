# Template de Contratação do Candidato – Negócio

## Objetivo

Formulário de template de contratação do candidato, refatorado do FlutterFlow para React, alinhado às regras de negócio e validadores do Dart.

## Regras e validadores

- **Dados da Vaga:** Tipo vaga, Analista R&S* (obrigatório), Cargo, Data Início* (obrigatório), Horário/Jornada (50 caracteres), Tipo Jornada*, Modelo trabalho*, Dias presencial*.
- **Informações Pessoais:** CPF* (11 dígitos), RG* (15), Data Nascimento* (10), DDD* (2), Telefone* (8–9 dígitos), CEP* (8), Rua (100), Número* (5), Complemento (50), Bairro* (50), Cidade* (50), E-mail Pessoal* (200).
- **Saúde do Candidato:** PCD (validator), Tipo deficiência, Necessita de adaptação (validator).
- **Acessórios:** Outros equipamentos (500 caracteres).
- **Checklist de Instalação:** Tipo equipamento* (obrigatório), Cargo x Máquina, Proprietário; Hardware (50), Softwares (500).
- **Acessos do Usuário:** Superior Imediato* (obrigatório), E-mail corporativo (200), Login Rede (50), Observações acesso* (1000), Grupo e-mail (100), Outros grupos, Observações aprovador (500).

## Critérios de aceite (mínimos)

- Seção “Informações sobre a vaga” como primeiro bloco (somente leitura).
- Ordem e títulos das seções conforme especificação; botão “Salvar e baixar Template” com ícone de download.
- Validação no submit para todos os campos obrigatórios (*); mensagens de erro nos campos.
- Limites de caracteres e contadores (0/N) respeitados.
- Seletor de profissionais (Analista de R&S, Superior Imediato) estável, sem flicker (aparecer e sumir); busca com debounce e sem refetch desnecessário a cada re-render.

## Restrições

- Campos obrigatórios devem exibir asterisco (*) e mensagem de erro quando vazios ou inválidos.
- Validação centralizada no hook; máscaras e maxLength alinhados às regras acima.

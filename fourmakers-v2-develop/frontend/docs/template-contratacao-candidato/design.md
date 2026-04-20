# Template de Contratação do Candidato – Design

## Estrutura da tela (ordem das seções)

1. **Informações sobre a vaga** – somente leitura (Código, Cliente – Unidade, Gestor, Abertura, Contratação, Observações internas, Proposta, Tipo de contratação, Custo, Modelo de trabalho, Localização, Frequência).
2. **Dados da Vaga** – formulário (Tipo vaga, Analista de R&S*, Cargo, Data de Início*, Horário/Jornada 50, Tipo de Jornada*, Modelo de trabalho*, Dias no presencial*).
3. **Informações Pessoais** – formulário (CPF*, RG*, Data de Nascimento*, DDD*, Telefone*, Tamanho da Camiseta*, Cep*, Rua, Número*, Complemento, Bairro*, Cidade*, Estado*, E-mail Pessoal*).
4. **Saúde do Candidato** – ícone (i) ao lado do título com tooltip.
5. **Acessórios Foursys** – checkboxes (Celular, Plano de Dados, Cartão de Visitas) + “Outros acessórios” (textarea 0/500).
6. **Checklist de Instalação** – Tipo de equipamento necessário*, Cargo x Máquina, Proprietário; Softwares Necessários (textarea 0/500).
7. **Acessos do Usuário** – Superior Imediato*, E-mail Corporativo, Login de Rede, Tipo de acesso, Observações de acesso do Usuário* (0/1000); Sistemas Liberados; Diretórios de Rede; Grupos de E-mail; Observações do Aprovador (0/500).
8. Botão principal: **“Salvar e baixar Template”** com ícone de download.

## Componentes de interação

- **Analista de R&S * / Superior Imediato *:** seletor/autocomplete de profissionais (`ColaboradorSearchField`). Placeholder “Pesquise o nome do analista” / “Buscar por nome ou código…”. Dropdown com busca interna; lista por nome ou código; seleção estável (sem aparecer e sumir).
- Selects e inputs com limites e contadores (0/N) onde aplicável.
- Acordeões/colapsáveis por seção; labels com asterisco para obrigatórios.

## Estados e feedback

- Validação no submit: mensagens de erro abaixo dos campos obrigatórios; toast de erro/sucesso no salvar e baixar.
- Carregando template/equipamentos/listas: estados de loading sem travar a UI; autocomplete com debounce (300 ms) e estado “Carregando…” na lista.

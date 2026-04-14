# Build CloudFormation / Cluster ECS

> A configuração do ambiente python, instalação dos pacotes necessários e configuração do ambiente client da aws devem ter sido executados anteriormente.

Um script make (defino no arquivo Makefile) é o responsável por executar todos os processos na AWS.
Um script python (build-template.py) é utilizado como ferramenta para gerar todos os arquivos de código IaC, configurações e escritps para os ambientes a serem criados.

Se a infraestrura não foi criada, utilize o projeto DevOps e certifique-se de que todos os stacks tenham sido executados.

Com a infraestrura básica em execução, para criar a infraestrutura específica do projeto, execute os processos abaixo.

---
## Para criar as configurações para cada ambiente

Para gerar os scripts e configurações para cada ambiente, execute o seguinte comando identificando a branch que vai originar o ambiente desejado - ambiente de Desenvolvimento, QA, Homologação e Produção (develop ,qa, staging, main).
```shell
./build-template.py develop
```
## ambientes destinos:

| Ambiente            | Branch Git | Sigla | Profile AWS       | URL                     |
|---------------------|------------|-------|-------------------|-------------------------|
| Desenvolvimento     | develop    | dev   | fsys2-development | *.dev.fourmakers.io     |
| Quality Assurance   | qa         | qa    | fsys2-development | *-qa.dev.fourmakers.io  |
| Homologação/Staging | staging    | stg   | fsys2-development | *-stg.dev.fourmakers.io |
| Produção            | main / tag | prd   | fsys2-production  | *.fourmakers.io         |

---
## Arquivo .env
---

Pode-se utilizar um arquivo `.env` para a customização de duas variáveis, `HEALTH_CHECK_PATH` e `HOST_NAME_BASE`. Na ausência destas variáveis no arquivo `.env`, os valores padrões (`HEALTH_CHECK_PATH`=`/` e `HOST_NAME_BASE`=`<Projeto>`) serão adotados. Estes valores serão utilizados pelo script `build-template.py` na produção dos arquivos yaml do CloudFormation.
```python
HEALTH_CHECK_PATH="/"
HOST_NAME_BASE="api-design"
```
Apenas para testes locais `sem repositório GIT`, outras duas variáveis podem ser definidas. Ao colocar o projeto em um repositório GIT, estas variáveis devem ser removidas do arquivo `.env`.
```python
PROJECT="api-design"
GIT_URL="https://gitlab.fourcamp.com/foursys-2/blueprints/api-design.git"
```

---
## Deploy
---

Antes de efetuar o deploy, é necessário garantir a unicidade da prioridade das regras do Load Balancer. Ao gerar um novo conjunto de arquivos de configuração para um novo ambiente (passo anterior), os scripts são gerados com a configuração de prioridade sequencial à última encontrada no Load Balancer em execução. Se forem gerados scripts para mais de um ambiente antes do deploy de cada um, todos terão o mesmo número de prioridade, sendo necessário o ajuste deste valor manualmente do arquivo YAML (Priority: Default: n).
Para verificar a prioridade e cada serviço e o próxima valor de prioridade, executando-se o script abaixo.
```shell
./priority.sh
```
Após estes ajustes, pode-se efetuar o deploy normalmente utilizando-se o comando `make`. Caso este deploy ocorra em momento posterior, deve-se revisar estes valores sempre no momento em que o deploy da nova infraestrutura tiver de ser executada.

---
## Deploy Automático
---

O deploy do ambiente ocorre de forma automática através da esteira DevOps ao se efetuar o push/merge para a branch referente aos arquivos de configuração criados neste processo.

---
## Deploy Manual
---

Exemplo do deploy para o ambiente de Desenvolvimento, após os ajustes necessários.

1. Criar na AWS o Taget Group do Load Balancer, o Listenner Rule do Load Balancer, o RecordSet do DNS Público, o Container Registry para a imagem Docker e o Task Definition para o criar os serviços no Cluster ECS.
```shell
make <projeto>-task-dev.yaml
make status
```
Caso o comando `make` reporte algum erro (como no caso da configuração do parâmetro priority), os problemas devem ser corrigidos e o deploy que falhou deve ser eliminado antes da execução de uma nova tentativa de deploy. Para elimitar o deploy com falha, deve-se utilizar o seguinte comando.
```shell
make clean <projeto>-task-dev.yaml
make status
```

2. Subir para o GitLab os novos arquivos gerados (scripts e configuracões de ambientes), verificar o status da Pipeline, solicitar o Merge Request para a branch Develop e/ou para a branch de destino (qa, staging, main).
#### obs: o comando `git push-merge` está documentado na Wiki, pode-se também efetuar o processo da forma convencional, efetuando o push da branch, criando um novo Merge Request na plataforma WEB e executando o deploy deste MR também pela plataforma WEB.
```shell
git add -A && git commit -m "[FSYS2-0000] CloudFormation Environment"
git push-merge develop
```
3. Após o merge dos novos códigos na branch de destino, é necessário, na primeira vez, aguardar o deploy da imagem docker no Registry (ECR) antes de criar os serviços.
```shell
make <projeto>-service-dev.yaml
make status
```
Aqui a mesma regra se aplica no deploy anterior. Caso erros sejam encontrados, deve-se executar o processo de limpeza antes de uma nova tentativa de deploy com o comando a seguir.
```shell
make clean <projeto>-service-dev.yaml
make status
```

4. Após este processo, é necessário aguardar alguns minutos para que toda a infra esteja ativa. Quando o ambiente já estiver respondendo (através do acesso via URL no browser), pode-se ampliar ou reduzir o número de instâncias ativas para o serviço.
```shell
./list-tasks.sh
./<projeto>-scale-dev.sh 1
```

---

Para todos os demais ambientes, a sequência é exatamente a mesma, trocando apenas em todos os passos acima o indicador do ambiente de Desenvolvimento (dev) para o ambiente que se deseja criar.

---
## Make
---

O comando make possui um help de linha de comando. Executando o mesmo sem passar nenhum parâmetro, é possível verificar todas as opções disponíveis.
```shell
make
```

---

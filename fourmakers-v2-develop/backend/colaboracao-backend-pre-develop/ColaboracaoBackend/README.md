# colaboracao-backend

`Para o projeto executar corretamente em sua máquina com docker, seguir as seguintes instruções:`

- Ajustar o arquivo .env_container para o ip da sua máquina, exemplo:
```sh
COLABORACAO_BASE=http://172.28.54.11
```
- Caso o docker-compose não funcione devido a falta da network "docker-network", poderá ser adicionado através da seguinte linha de comando:
```sh
docker network create -d bridge docker-network
```
- Para fazer o deploy no docker, entrar na pasta ColaboracaoBackend e executar o seguinte comando:
```sh
docker compose up -d --build
```

`IMPORTANTE`
- Favor NÃO commitar o arquivo .env_container. Caso seja necessário, ao commitar, não alterar a linha:
```sh
COLABORACAO_BASE=http://10.10.10.201
```

- Para rodar o projeto no windows, necessario configurar algumas variaveis de ambiente (vide EnvWindows.bat)
```sh
TOKEN_SISTEMA_COLABORACAO = ValorDoToken
COLABORACAO_BASE = http://localhost
COLABORADOR_API_PATH = %COLABORACAO_BASE%:30101/api/
COMENTARIO_API_PATH = %COLABORACAO_BASE%:30102/api/
FILTRO_API_PATH = %COLABORACAO_BASE%:30103/api/
FIREBASE_API_PATH = %COLABORACAO_BASE%:30104/api/
HOBBY_API_PATH = %COLABORACAO_BASE%:30105/api/
INTERESSE_API_PATH = %COLABORACAO_BASE%:30106/api/
METODOLOGIA_API_PATH = %COLABORACAO_BASE%:30107/api/
USUARIO_API_PATH = %COLABORACAO_BASE%:30108/api/
CANDIDATO_API_PATH = %COLABORACAO_BASE%:30109/api/
COMPETENCIA_API_PATH = %COLABORACAO_BASE%:30110/api/
DOMINIO_API_PATH = %COLABORACAO_BASE%:30111/api/
ENDOSSO_API_PATH = %COLABORACAO_BASE%:30112/api/
FEED_API_PATH = %COLABORACAO_BASE%:30113/api/
MODELOREFERENCIA_API_PATH = %COLABORACAO_BASE%:30114/api/
NOTICIA_API_PATH = %COLABORACAO_BASE%:30115/api/
UPLOADFILES_API_PATH = %COLABORACAO_BASE%:30116/api/
FORMACAO_API_PATH = %COLABORACAO_BASE%:30117/api/
```

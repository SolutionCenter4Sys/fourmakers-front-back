# Inscrever CV de candidato — Design

## Comportamento visual e UX
- Modal com título "Inscrever CV de candidato" e descrição acessível (`DialogDescription`).
- Upload de PDF: botão "Clique para selecionar um PDF"; quando há arquivo, card de confirmação com ícone de sucesso e botão remover.
- Campo LinkedIn: prefixo fixo `https://www.linkedin.com/in/` e input para o id ou URL do perfil.
- Botão "Importar" desabilitado quando não há PDF nem LinkedIn válido ou quando está importando.

## Estado de erro
- Quando a importação falha (API retorna erro ou exceção): exibir **Alert** com variante `destructive` (tokens do DS), título "Erro na importação" e descrição com a mensagem retornada (ou fallback amigável).
- O Alert é exibido **dentro do modal**, acima dos campos, para o usuário ler e poder tentar novamente sem perder o contexto.
- O erro é limpo ao reabrir o modal, ao iniciar nova importação ou ao fechar o modal.

## Falta de conexão e mensagens amigáveis
- Erros de rede (sem conexão, offline, falha ao enviar) não devem exibir mensagens técnicas (ex.: "Failed to fetch"). O projeto utiliza o utilitário `getMensagemAmigavelErro` (`@shared/utils/errorMessageUtils`) para normalizar: em caso de falha de rede, exibir **"Sem conexão com a internet. Verifique sua rede e tente novamente."**
- Demais exceções: exibir a mensagem do erro quando for válida e legível; caso contrário, fallback genérico amigável (ex.: "Não foi possível importar o candidato. Tente novamente.").

## Acessibilidade
- `DialogTitle` e `DialogDescription` presentes.
- Alert com `role="alert"` (componente do DS já fornece).

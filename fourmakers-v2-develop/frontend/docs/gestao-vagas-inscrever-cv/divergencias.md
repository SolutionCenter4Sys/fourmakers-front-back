# Divergências — Inscrever CV de candidato

Documentação consolidada e comparada ao código em: Fev 2026.

## [Categoria: Desenvolvimento]

### Divergência 1 (resolvida)
**De:** Contrato da API citava apenas `mensagem`.  
**Para:** Backend retorna `mensagen` em respostas de erro.  
**Explicação:** Implementação atual aceita `mensagen` na camada de dados e o repositório mapeia para `mensagem` no domínio. Nenhuma alteração pendente.

## [Categoria: Design]

Nenhuma divergência encontrada entre documentação e código para estados de erro e uso de Alert/DialogDescription.

---

**Resumo:** Nenhuma divergência em aberto. A documentação foi atualizada para refletir o mapeamento `mensagen` → `mensagem`, o feedback de erro em tela e o tratamento de falta de conexão com mensagens amigáveis (util `getMensagemAmigavelErro`).

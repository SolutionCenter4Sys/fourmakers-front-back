# Módulos da Demo E2E

Cada módulo é um **manifesto JSON** que define:

- Telas e **caminhos do código-fonte** (pré-selecionados — o agente não perde tempo procurando)
- APIs para o DataForge alinhar massa aos contratos reais
- Pastas de output versionado (BDD, massa, spec Playwright, relatório HTML)
- Comandos de ativação no chat

## Módulo padrão

| ID | Nome | Comando |
|----|------|---------|
| `reembolso` | Módulo de Reembolso | `demo reembolso` ou apenas `demo` |

Se o usuário pedir **demo**, **esteira e2e** ou **esteira de testes** sem citar outro módulo, usar **`reembolso`**.

## Adicionar novo módulo

1. Copie `reembolso.manifest.json` → `{modulo}.manifest.json`
2. Ajuste `telas[].arquivos`, `apis`, `outputs` e `comandosAtivacao`
3. Defina `"padrao": false` (apenas um módulo pode ser padrão)
4. Valide: `node DEMO/scripts/demo-modulo.cjs summary {modulo}`

## CLI útil

```bash
node DEMO/scripts/demo-modulo.cjs list
node DEMO/scripts/demo-modulo.cjs summary reembolso
node DEMO/scripts/demo-modulo.cjs sources reembolso
node DEMO/scripts/demo-modulo.cjs next-version reembolso
```

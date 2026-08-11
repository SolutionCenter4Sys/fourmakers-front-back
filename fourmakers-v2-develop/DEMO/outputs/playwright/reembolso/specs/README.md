# Versoes Playwright — Reembolso

Esta pasta armazena as specs versionadas da demo:

- `reembolso-demo-v5.spec.ts` — catálogo completo BDD v5 (129 cenários; 11 executáveis + 118 skip)
- `reembolso-demo-v1.spec.ts` … `reembolso-demo-v10.spec.ts` (legado)

Regras:

- Cada spec deve apontar para o BDD/DataForge da mesma versao `vN`.
- Nunca sobrescrever versao existente (regenerar via `npm run generate:spec:v5`).
- Manter no maximo 10 versoes.
- Ao atingir `v10`, limpar versoes anteriores e reiniciar em `v1`.

Executar v5:

```bash
cd frontend/playwright-automation-template
npm run test:bdd:v5
npm run demo          # usa BDD + DataForge + spec da mesma versao
```

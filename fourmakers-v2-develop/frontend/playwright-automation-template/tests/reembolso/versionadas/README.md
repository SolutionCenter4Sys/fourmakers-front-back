# Versoes Playwright — Reembolso

Esta pasta armazena as specs versionadas da demo:

- `reembolso-demo-v1.spec.ts`
- `reembolso-demo-v2.spec.ts`
- ...
- `reembolso-demo-v10.spec.ts`

Regras:

- Cada spec deve apontar para o BDD/DataForge da mesma versao `vN`.
- Nunca sobrescrever versao existente.
- Manter no maximo 10 versoes.
- Ao atingir `v10`, limpar versoes anteriores e reiniciar em `v1`.

# Bug da demo (self-healing)

Arquivo pronto para colar **manualmente** antes de `demo reembolso`.

## O que muda

No botão **Solicitar Reembolso**, a rota correta `/inserir-reembolso` vira `/inserir-reembolso-invalido`.

Cenário que falha: **R-04** (navegação para nova solicitação).

## Como injetar (Ctrl+A / Ctrl+V)

1. Abra `DEMO/bug-self-healing/Reembolso.tsx`
2. `Ctrl+A` → `Ctrl+C`
3. Abra `frontend/src/presentation/pages/Reembolso.tsx`
4. `Ctrl+A` → `Ctrl+V` → salve
5. Confirme o marcador:

```bash
node DEMO/scripts/demo-reembolso-bug.cjs status
```

Deve mostrar `"ativo": true` e rota `"/inserir-reembolso-invalido"`.

## Como reverter depois da demo

No chat, o Dev FourBlox corrige sozinho no self-healing.

Ou manualmente:

```bash
node DEMO/scripts/demo-reembolso-bug.cjs fix
```

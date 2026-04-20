# Rotas — módulo Recrutamento

- **`RecrutamentoRoutes.tsx`** — exporta `RecrutamentoChildRoutes` (`Fragment` + filhos `<Route>`). **Não** usar `<Routes>` aqui (React Router 7).
- **`RecrutamentoLegacyPrivateRoutes.tsx`** — `RedirectGestaodevagasToRecrutamento` (usado como `element` de um `<Route>` em `AppRoutes`).
- **`index.ts`** — reexports.

Montagem em **`AppRoutes.tsx`:** `<Route path="/recrutamento" element={<Outlet />}>` + `{RecrutamentoChildRoutes}`.

Docs: `docs/PROPOSTA_MODULARIZACAO_DI_ROTAS.md` §12, `docs/MODULARIZACAO_PRESENTATION_RECRUTAMENTO.md` §7.

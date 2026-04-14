# Gestão de Vagas — Atribuição de Recrutador (Desenvolvimento)

## Arquivos envolvidos
- **Apresentação:** `src/presentation/components/gestao-vagas/AtribuirRecrutadorModal.tsx`, `AtribuirRecrutadorCandidatoModal.tsx`
- **Domínio:** `ListarColaboradoresOrgUseCase`, `AdicionarRecrutadorVagaUseCase`, `AtualizarRecrutadorResponsavelUseCase`; entidade `ColaboradorCch`
- **UI:** `Command`, `CommandInput`, `CommandList`, `CommandItem`, `Popover` (`@/components/ui`)

## Contratos e integrações
- Listagem: `ListarColaboradoresOrgUseCase.execute(token, { busca, cursor, limite })` → `ColaboradoresCchResult`
- Vaga: `AdicionarRecrutadorVagaUseCase.execute(token, vagaId, codigoColaboradorInterno)`
- Candidato: `AtualizarRecrutadorResponsavelUseCase.execute(token, idCandidatura, codigoColaboradorInterno)`

## Regras técnicas
- Modais seguem padrão do projeto: `open`, `onOpenChange`, uso de Use Cases via container (tsyringe).
- Nenhum import de `@data/api` na camada de apresentação.
- Componente de seleção: Popover + Command (cmdk) com filtro e lista rolável.

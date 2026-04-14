INSERT INTO tb_vaga_motivos_perda (id, descricao, explicacao, ordem)
VALUES
(UUID(), 'Cancelada', 'Cliente suspendeu a contratação', 1),
(UUID(), 'Pausada', 'Processo congelado temporariamente.', 2),
(UUID(), 'Preenchida por outra consultoria', 'Outra empresa apresentou candidato mais aderente, rápido ou com custo menor.', 3),
(UUID(), 'Orquestração', 'Profissional interno (realocado, promovido) assumiu a posição.', 4),
(UUID(), 'Mudança de escopo', 'O perfil técnico ou senioridade mudou.', 5),
(UUID(), 'Cliente perdeu o budget', 'Redução de custos pelo cliente.', 6),
(UUID(), 'Vaga obsoleta / projeto descontinuado', 'Projeto pivotou, e a posição não faz mais sentido.', 7),
(UUID(), 'Mudança de prioridade estratégica', 'O foco da squad/projeto mudou', 8),
(UUID(), 'Sem retorno do cliente', '', 9);


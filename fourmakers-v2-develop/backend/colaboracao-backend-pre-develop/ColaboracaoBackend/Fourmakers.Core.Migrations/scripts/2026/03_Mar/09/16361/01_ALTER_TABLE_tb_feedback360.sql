ALTER TABLE `tb_feedback360`
  CHANGE COLUMN `titulo`    `situacao`  text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Situação em que ocorreu a interação (STAR - S)',
  ADD    COLUMN `tarefa`    text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Tarefa/objetivo da interação (STAR - T)' AFTER `situacao`,
  CHANGE COLUMN `descricao` `acao`      text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Ação realizada pelo colaborador (STAR - A)',
  ADD    COLUMN `resultado` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Resultado gerado pela ação (STAR - R)' AFTER `acao`;
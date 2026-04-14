CREATE TABLE `tb_log_recomendacao_profissionais_aderencia` (
  `id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `tb_org_id` int NOT NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `codigo_interno_colaborador_criacao` varchar(36) NOT NULL,
  `codigo_vaga` int NOT NULL,
  `objeto` text,
  `acao` int NOT NULL
)
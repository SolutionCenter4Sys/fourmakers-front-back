-- Log de alterações em pretensão salarial e modelo de trabalho (candidatura).
-- Vaga / candidato na vaga: JOIN tb_candidato_vaga via tb_candidato_vaga_id (FK).
-- Descrição do modelo: JOIN tb_modelo_trabalho nos IDs (FK quando não nulos).
-- Executar no banco gcolb (ou ambiente equivalente).

CREATE TABLE `tb_candidato_vaga_log_pretensao_modelo` (
  `id` varchar(36) NOT NULL,
  `tb_candidato_vaga_id` varchar(36) NOT NULL,
  `tb_colaborador_codigo_interno_colaborador_candidato` varchar(36) NOT NULL,
  `tipo_operacao` varchar(20) NOT NULL COMMENT 'INSERT | UPDATE | DELETE',
  `pretensao_salarial_valor_anterior` decimal(10,2) DEFAULT NULL,
  `pretensao_salarial_valor_novo` decimal(10,2) DEFAULT NULL,
  `tb_modelo_trabalho_id_anterior` varchar(36) DEFAULT NULL,
  `tb_modelo_trabalho_id_novo` varchar(36) DEFAULT NULL,
  `tb_colaborador_codigo_interno_colaborador_executor` varchar(36) NOT NULL,
  `data_alteracao` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `origem_alteracao` varchar(80) DEFAULT NULL COMMENT 'Candidato | Simulador | Movimentacao Kanban (OrigemAlteracaoPretensaoModeloLogEnum)',
  PRIMARY KEY (`id`),
  KEY `idx_log_pret_modelo_candidatura_data` (`tb_candidato_vaga_id`,`data_alteracao`),
  KEY `idx_log_pret_modelo_candidato_data` (`tb_colaborador_codigo_interno_colaborador_candidato`,`data_alteracao`),
  KEY `fk_log_pret_modelo_executor` (`tb_colaborador_codigo_interno_colaborador_executor`),
  KEY `fk_log_pret_modelo_mt_anterior` (`tb_modelo_trabalho_id_anterior`),
  KEY `fk_log_pret_modelo_mt_novo` (`tb_modelo_trabalho_id_novo`),
  CONSTRAINT `fk_log_pret_modelo_candidatura` FOREIGN KEY (`tb_candidato_vaga_id`) REFERENCES `tb_candidato_vaga` (`id`),
  CONSTRAINT `fk_log_pret_modelo_candidato` FOREIGN KEY (`tb_colaborador_codigo_interno_colaborador_candidato`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`),
  CONSTRAINT `fk_log_pret_modelo_executor` FOREIGN KEY (`tb_colaborador_codigo_interno_colaborador_executor`) REFERENCES `tb_colaborador` (`codigo_interno_colaborador`),
  CONSTRAINT `fk_log_pret_modelo_mt_anterior` FOREIGN KEY (`tb_modelo_trabalho_id_anterior`) REFERENCES `tb_modelo_trabalho` (`id`),
  CONSTRAINT `fk_log_pret_modelo_mt_novo` FOREIGN KEY (`tb_modelo_trabalho_id_novo`) REFERENCES `tb_modelo_trabalho` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

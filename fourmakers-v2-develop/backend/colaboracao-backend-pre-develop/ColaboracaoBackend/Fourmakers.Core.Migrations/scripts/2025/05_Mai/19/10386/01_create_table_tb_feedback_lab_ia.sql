DROP TABLE IF EXISTS `tb_feedback_lab_ia`;

CREATE TABLE `tb_feedback_lab_ia` (
                                      `id` VARCHAR(36) NOT NULL,
                                      `tb_colaborador_codigo_interno_colaborador` VARCHAR(36) NOT NULL,
                                      `contexto` TEXT,
                                      `pergunta` TEXT NOT NULL,
                                      `resposta` TEXT NOT NULL,
                                      `aprovado` TINYINT(1) DEFAULT NULL,
                                      `data_requisicao` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                      PRIMARY KEY (`id`),
                                      CONSTRAINT `fk_feedback_colaborador`
                                          FOREIGN KEY (`tb_colaborador_codigo_interno_colaborador`)
                                              REFERENCES `tb_colaborador` (`codigo_interno_colaborador`)
                                              ON DELETE CASCADE
                                              ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
CREATE TABLE `tb_motivo_descandidatar` (
  `id` CHAR(36) NOT NULL DEFAULT (UUID()),
  `descricao` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Não está avaliando no momento');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Só aceita PJ');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Só aceita CLT full');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Não aceita os modelos de contratação Cooperado nem CLT + Benefícios');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Não aceita trabalhar presencial');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Não aceita trabalhar de forma híbrida (presencial algumas vezes)');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Não aceita o local de trabalho');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Não gostou do perfil da vaga');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Fechou com outra empresa');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Recebeu uma contra proposta da empresa atual');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Demora do processo seletivo');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Valor');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Forma de contratação');
INSERT INTO tb_motivo_descandidatar (descricao) VALUES ('Outros');

CREATE TABLE `tb_descandidaturas` (
  `tb_candidato_vaga_id` VARCHAR(36) NOT NULL,
  `tb_motivo_descandidatar_id` CHAR(36) NOT NULL,
  PRIMARY KEY (`tb_candidato_vaga_id`, `tb_motivo_descandidatar_id`),
  CONSTRAINT `fk_tb_candidato_vaga_id` 
    FOREIGN KEY (`tb_candidato_vaga_id`) 
    REFERENCES `tb_candidato_vaga`(`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_tb_motivo_descandidatar_id` 
    FOREIGN KEY (`tb_motivo_descandidatar_id`) 
    REFERENCES `tb_motivo_descandidatar`(`id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

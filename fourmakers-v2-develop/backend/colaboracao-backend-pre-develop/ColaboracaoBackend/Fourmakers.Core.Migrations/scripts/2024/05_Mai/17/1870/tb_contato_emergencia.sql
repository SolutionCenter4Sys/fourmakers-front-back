CREATE TABLE tb_contato_emergencia (
	id char(36) NOT NULL,
	nome varchar(255) NOT NULL,
	telefone varchar(100) NOT NULL,
	grau_parentesco varchar(255) NOT NULL,
	tb_colaborador_cpf varchar(11) NOT NULL,
	CONSTRAINT tb_contato_emergencia_pk PRIMARY KEY (id),
	CONSTRAINT tb_contato_emergencia_tb_colaborador_FK FOREIGN KEY (tb_colaborador_cpf) REFERENCES tb_colaborador(cpf) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb3
COLLATE=utf8mb3_general_ci;
CREATE INDEX tb_contato_emergencia_id_IDX USING BTREE ON tb_contato_emergencia (id);
CREATE INDEX tb_contato_emergencia_tb_colaborador_cpf_IDX USING BTREE ON tb_contato_emergencia (tb_colaborador_cpf);

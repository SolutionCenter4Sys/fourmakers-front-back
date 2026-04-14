DROP TABLE IF EXISTS tb_candidato_vaga_opcao_contato;
DROP TABLE IF EXISTS tb_opcao_contato;
DROP TABLE IF EXISTS tb_candidato_vaga;

CREATE TABLE tb_opcao_contato (
    id VARCHAR(36) PRIMARY KEY,
    descricao VARCHAR(100) NOT NULL
);

CREATE TABLE `tb_candidato_vaga` (
  `id` varchar(36) NOT NULL,
  `tb_colaborador_codigo_interno_colaborador` varchar(36) NOT NULL,
  `tb_vaga_id` varchar(36) NOT NULL,
  `tb_org_id` int NOT NULL,
  `tb_status_id` int NOT NULL DEFAULT '1',
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

CREATE TABLE tb_candidato_vaga_opcao_contato (
    tb_candidato_vaga_id VARCHAR(36) NOT NULL,
    tb_opcao_contato_id VARCHAR(36) NOT NULL,
    PRIMARY KEY (tb_candidato_vaga_id, tb_opcao_contato_id),
    FOREIGN KEY (tb_candidato_vaga_id) REFERENCES tb_candidato_vaga(id) ON DELETE CASCADE,
    FOREIGN KEY (tb_opcao_contato_id) REFERENCES tb_opcao_contato(id) ON DELETE CASCADE
);

INSERT INTO tb_opcao_contato (id, descricao) VALUES 
(UUID(), 'Email'),
(UUID(), 'Telefone'),
(UUID(), 'Ligação WhatsApp'),
(UUID(), 'Mensagem WhatsApp');

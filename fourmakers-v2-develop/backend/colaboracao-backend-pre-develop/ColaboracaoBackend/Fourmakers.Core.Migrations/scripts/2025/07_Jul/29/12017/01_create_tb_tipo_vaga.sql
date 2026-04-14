CREATE TABLE `tb_tipo_vaga` (
  `id` varchar(36) NOT NULL,
  `descricao` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

INSERT INTO tb_tipo_vaga (id, descricao) VALUES
(UUID(), 'Alocação'),
(UUID(), 'Administrativo'),
(UUID(), 'Projeto');
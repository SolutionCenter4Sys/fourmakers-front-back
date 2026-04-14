CREATE TABLE `tb_unidade` (
  `id` varchar(36) NOT NULL,
  `descricao` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

INSERT INTO tb_unidade (id, descricao) VALUES
(UUID(), 'UN I - RONALDO'),
(UUID(), 'UN II - RAFAEL'),
(UUID(), 'UN III - MANOELLITO'),
(UUID(), 'UN IV - GENIVALDO'),
(UUID(), 'UN VI - FERNANDO GOULART'),
(UUID(), 'UNIDADE VIII'),
(UUID(), 'CORPORATIVO'),
(UUID(), 'Filial Padrão'),
(UUID(), 'CSC - Centro de Serviços Compartilhados'),
(UUID(), 'UN IX - Cibersegurança');
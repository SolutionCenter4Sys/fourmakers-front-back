CREATE TABLE `tb_experiencia_empresa_sugestao` (
  `nome` varchar(255) NOT NULL,
  `data_criacao` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`nome`)
);
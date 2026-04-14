CREATE TABLE `tb_vaga` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `id_vaga` int DEFAULT NULL,
  `titulo` varchar(255) DEFAULT NULL,
  `status` varchar(50) DEFAULT NULL,
  `tipo` varchar(50) DEFAULT NULL,
  `numero_de_vagas` int DEFAULT NULL,
  `taxa_maxima_hora` varchar(255) DEFAULT NULL,
  `nivel_de_urgencia` varchar(50) DEFAULT NULL,
  `descricao` text,
  `funcao` text,
  `data_criacao` datetime DEFAULT NULL,
  `data_atualizacao` datetime DEFAULT NULL,
  `data_publicacao` datetime DEFAULT NULL,
  `data_aceitacao` datetime DEFAULT NULL,
  `tipo_localizacao` varchar(50) DEFAULT NULL,
  `observacao_localizacao` text,
  `estado` varchar(50) DEFAULT NULL,
  `cidade` varchar(100) DEFAULT NULL,
  `visibilidade` tinyint(1) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`)
);

CREATE TABLE `tb_candidatos_vaga` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `id_vaga` bigint DEFAULT NULL,
  `nome` varchar(255) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `status` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`Id`)
);

CREATE TABLE `tb_skill_vaga_srs` (
  `vaga_id` bigint DEFAULT NULL,
  `categoria_id` int DEFAULT NULL,
  `descricao_id` int DEFAULT NULL,
  `nivel_id` int DEFAULT NULL,
  `data_criacao` datetime DEFAULT NULL,
  `data_alteracao` datetime DEFAULT NULL,
  `id` int NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`)
);
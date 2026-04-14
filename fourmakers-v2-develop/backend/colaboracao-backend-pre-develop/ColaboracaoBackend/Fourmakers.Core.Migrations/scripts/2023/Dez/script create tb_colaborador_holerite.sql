CREATE TABLE `tb_colaborador_holerite` (
  `id` bigint NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `path` varchar(350) not null,
  `cpf` varchar(11) not null,
  `emissao` datetime,
  `mes` int not null,
  `ano` int not null,
  `orgId` bigint not null,
  `data_criacao` datetime DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
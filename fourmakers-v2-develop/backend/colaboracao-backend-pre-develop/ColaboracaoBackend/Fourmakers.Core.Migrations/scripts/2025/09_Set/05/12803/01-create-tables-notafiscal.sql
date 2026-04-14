CREATE TABLE `tb_nota_fiscal_status` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `descricao` varchar(50)
);

CREATE TABLE `tb_nota_fiscal` (
  `id` char(36) PRIMARY KEY,
  `vigencia_mes` int,
  `vigencia_ano` int,
  `numero_nf` varchar(50),
  `data_emissao_nota_fiscal` timestamp,
  `valor` decimal(15,2),
  `url_nota_fiscal_download` varchar(500),
  `tb_nota_fiscal_status_id` int,
  `motivo_reprovacao` varchar(500),
  `data_criacao` timestamp,
  `data_alteracao` timestamp,
  `codigo_interno_colaborador_criacao` char(36),
  `codigo_interno_colaborador_alteracao` char(36)
);

CREATE TABLE `tb_nota_fiscal_rubrica` (
  `id` char(36) PRIMARY KEY,
  `tb_nota_fiscal_id` char(36),
  `tb_rubrica_colaborador_id` char(36),
  `valor` decimal(15,2)
);

CREATE TABLE `tb_nota_fiscal_log` (
  `id` char(36) PRIMARY KEY,
  `tb_nota_fiscal_id` char(36),
  `tb_nota_fiscal_status_anterior_id` int,
  `tb_nota_fiscal_status_novo_id` int,
  `numero_nf` varchar(50),
  `data_emissao_nota_fiscal` timestamp,
  `observacao` varchar(500),
  `data_criacao` timestamp,
  `data_alteracao` timestamp,
  `codigo_interno_colaborador_criacao` char(36),
  `codigo_interno_colaborador_alteracao` char(36)
);


ALTER TABLE `tb_nota_fiscal` ADD FOREIGN KEY (`codigo_interno_colaborador_criacao`) REFERENCES `tb_colaborador_org` (`codigo_interno_colaborador`);
ALTER TABLE `tb_nota_fiscal` ADD FOREIGN KEY (`codigo_interno_colaborador_alteracao`) REFERENCES `tb_colaborador_org` (`codigo_interno_colaborador`);
ALTER TABLE `tb_nota_fiscal` ADD FOREIGN KEY (`tb_nota_fiscal_status_id`) REFERENCES `tb_nota_fiscal_status` (`id`);

ALTER TABLE `tb_nota_fiscal_rubrica` ADD FOREIGN KEY (`tb_nota_fiscal_id`) REFERENCES `tb_nota_fiscal` (`id`);
ALTER TABLE `tb_nota_fiscal_rubrica` ADD FOREIGN KEY (`tb_rubrica_colaborador_id`) REFERENCES `tb_rubrica_colaborador` (`id`);

ALTER TABLE `tb_nota_fiscal_log` ADD FOREIGN KEY (`tb_nota_fiscal_id`) REFERENCES `tb_nota_fiscal` (`id`);
ALTER TABLE `tb_nota_fiscal_log` ADD FOREIGN KEY (`tb_nota_fiscal_status_anterior_id`) REFERENCES `tb_nota_fiscal_status` (`id`);
ALTER TABLE `tb_nota_fiscal_log` ADD FOREIGN KEY (`tb_nota_fiscal_status_novo_id`) REFERENCES `tb_nota_fiscal_status` (`id`);


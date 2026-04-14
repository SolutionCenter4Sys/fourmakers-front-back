CREATE TABLE `tb_grupo_acesso_cliente` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `tb_grupo_acesso_id` INT NOT NULL,
  `codigo_cliente` VARCHAR(45) NOT NULL,
  `tb_org_id` INT NOT NULL,
  `data_criacao` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_alteracao` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `ativo` TINYINT(1) NOT NULL DEFAULT 1,
  KEY `fk_tb_grupo_acesso_cliente_tb_grupo_acesso_idx` (`tb_grupo_acesso_id`),
  KEY `fk_tb_grupo_acesso_cliente_tb_cliente_org_idx` (`codigo_cliente`, `tb_org_id`),
  CONSTRAINT `fk_tb_grupo_acesso_cliente_tb_grupo_acesso` 
    FOREIGN KEY (`tb_grupo_acesso_id`) 
    REFERENCES `tb_grupo_acesso` (`id`) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE,
  CONSTRAINT `fk_tb_grupo_acesso_cliente_tb_cliente_org` 
    FOREIGN KEY (`codigo_cliente`, `tb_org_id`) 
    REFERENCES `tb_cliente_org` (`codigo_cliente`, `tb_org_id`) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE,
  UNIQUE KEY `uk_grupo_acesso_cliente` (`tb_grupo_acesso_id`, `codigo_cliente`, `tb_org_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='Relacionamento entre grupos de acesso e clientes';


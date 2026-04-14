START TRANSACTION;
SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

ALTER TABLE `tb_empresa` 
DROP FOREIGN KEY `fk_tb_empresa_tb_org1`;

ALTER TABLE `tb_grupo_acesso` 
DROP FOREIGN KEY `fk_tb_grupo_acesso_tb_org1`;

ALTER TABLE `tb_colaborador` 
DROP FOREIGN KEY `fk_tb_colaborador_tb_orgs1`;

ALTER TABLE `tb_diretoria` 
DROP FOREIGN KEY `fk_tb_diretoria_tb_orgs1`;

ALTER TABLE `tb_org` 
CHANGE COLUMN `id` `id` INT(11) NOT NULL ;

ALTER TABLE `tb_colaborador` 
DROP FOREIGN KEY `fk_colaborador_tb_diretoria1`;

ALTER TABLE `tb_colaborador` 
DROP COLUMN `org_id`,
DROP COLUMN `diretoria_id`,
DROP COLUMN `admissao`,
DROP INDEX `fk_tb_colaborador_tb_orgs1_idx` ,
DROP INDEX `fk_colaborador_tb_diretoria1_idx` ;

ALTER TABLE `tb_colaborador_alocado` 
ADD COLUMN `tb_org_id` INT(11) NOT NULL,
DROP COLUMN `nome_projeto`,
DROP COLUMN `nome_gestor`,
DROP COLUMN `codigo_gestor`,
CHANGE COLUMN `codigo_projeto` `codigo_projeto` VARCHAR(255) NOT NULL,
ADD INDEX `fk_tb_colaborador_alocado_tb_org1_idx` (`tb_org_id` ASC) VISIBLE;
;

ALTER TABLE `tb_empresa` 
CHANGE COLUMN `tb_org_id` `tb_org_id` INT(11) NOT NULL DEFAULT '3' ;

ALTER TABLE `tb_grupo_acesso` 
CHANGE COLUMN `tb_org_id` `tb_org_id` INT(11) NOT NULL;

CREATE TABLE IF NOT EXISTS `tb_colaborador_org` (
  `tb_org_id` INT(11) NOT NULL,
  `tb_colaborador_cpf` VARCHAR(11) NOT NULL,
  `cod_diretoria` VARCHAR(255) NOT NULL,
  `diretoria` VARCHAR(255) NOT NULL,
  `departamento` VARCHAR(255) NOT NULL,
  `cod_departamento` VARCHAR(255) NOT NULL,
  `data_criacao` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP(),
  `data_alteracao` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP(),
  `cargo` VARCHAR(255) NOT NULL,
  `codigo_cargo` VARCHAR(255) NOT NULL,
  `cod_colaborador_externo` VARCHAR(255) NOT NULL,
  `data_admissao` TIMESTAMP NULL DEFAULT NULL,
  `ativo` TINYINT NOT NULL DEFAULT 0,
  INDEX `fk_tb_colaborador_org_tb_org1_idx` (`tb_org_id` ASC) VISIBLE,
  INDEX `fk_tb_colaborador_org_tb_colaborador1_idx` (`tb_colaborador_cpf` ASC) VISIBLE,
  PRIMARY KEY (`tb_org_id`, `tb_colaborador_cpf`),
  CONSTRAINT `fk_tb_colaborador_org_tb_org1`
    FOREIGN KEY (`tb_org_id`)
    REFERENCES `tb_org` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_tb_colaborador_org_tb_colaborador1`
    FOREIGN KEY (`tb_colaborador_cpf`)
    REFERENCES `tb_colaborador` (`cpf`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;

CREATE TABLE IF NOT EXISTS `tb_colaborador_hierarquia` (
  `cod_colaborador_externo` VARCHAR(255) NOT NULL,
  `cod_colaborador_superior` VARCHAR(45) NOT NULL,
  `tb_org_id` INT(11) NOT NULL,
  PRIMARY KEY (`cod_colaborador_externo`, `cod_colaborador_superior`, `tb_org_id`),
  INDEX `fk_tb_colaborador_hierarquia_tb_org1_idx` (`tb_org_id` ASC) VISIBLE,
  CONSTRAINT `fk_tb_colaborador_hierarquia_tb_org1`
    FOREIGN KEY (`tb_org_id`)
    REFERENCES `tb_org` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;

CREATE TABLE IF NOT EXISTS `tb_projeto_org` (
  `cod_projeto` VARCHAR(255) NOT NULL,
  `projeto` VARCHAR(45) NOT NULL,
  `cod_cliente` VARCHAR(45) NOT NULL,
  `cliente` VARCHAR(255) NOT NULL,
  `cod_diretoria` VARCHAR(255) NOT NULL,
  `diretoria` VARCHAR(255) NOT NULL,
  `cod_status` INT(11) NOT NULL,
  `status` VARCHAR(255) NOT NULL,
  `data_inicio` DATETIME NULL DEFAULT NULL,
  `data_fim` DATETIME NULL DEFAULT NULL,
  `cod_proposta` VARCHAR(255) NULL DEFAULT NULL,
  `qtd_horas_planejadas` DECIMAL NOT NULL,
  `qtd_horas_executadas` DECIMAL NOT NULL,
  `tb_org_id` INT(11) NOT NULL,
  PRIMARY KEY (`cod_projeto`, `tb_org_id`),
  INDEX `fk_tb_projeto_org_tb_org1_idx` (`tb_org_id` ASC) VISIBLE,
  CONSTRAINT `fk_tb_projeto_org_tb_org1`
    FOREIGN KEY (`tb_org_id`)
    REFERENCES `tb_org` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;

CREATE TABLE IF NOT EXISTS `tb_projeto_gerente` (
  `cod_projeto` VARCHAR(255) NOT NULL,
  `cod_colaborador_gerente` VARCHAR(255) NOT NULL,
  `tipo_gerente` VARCHAR(255) NOT NULL,
  `tb_org_id` INT(11) NOT NULL,
  PRIMARY KEY (`cod_projeto`, `cod_colaborador_gerente`, `tipo_gerente`, `tb_org_id`),
  INDEX `fk_tb_projeto_gerente_tb_org1_idx` (`tb_org_id` ASC) VISIBLE,
  CONSTRAINT `fk_tb_projeto_gerente_tb_org1`
    FOREIGN KEY (`tb_org_id`)
    REFERENCES `tb_org` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;

CREATE TABLE IF NOT EXISTS `tb_ad_sso` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `base_url` VARCHAR(255) NOT NULL,
  `token_path` VARCHAR(255) NOT NULL,
  `graph_path` VARCHAR(255) NOT NULL,
  `tenant` VARCHAR(255) NOT NULL,
  `client_id` VARCHAR(255) NOT NULL,
  `client_secret_value` VARCHAR(255) NOT NULL,
  `code_verifier_plain` VARCHAR(255) NOT NULL,
  `redirect_url` VARCHAR(255) NOT NULL,
  `tb_org_id` INT(11) NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_tb_ad_sso_tb_org1_idx` (`tb_org_id` ASC) VISIBLE,
  CONSTRAINT `fk_tb_ad_sso_tb_org1`
    FOREIGN KEY (`tb_org_id`)
    REFERENCES `tb_org` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;

CREATE TABLE IF NOT EXISTS `tb_colaborador_projeto_org` (
  `cod_colaborador` VARCHAR(255) NOT NULL,
  `cod_projeto` VARCHAR(255) NOT NULL,
  `nome_projeto` VARCHAR(255) NOT NULL,
  `tb_org_id` INT(11) NOT NULL,
  PRIMARY KEY (`cod_colaborador`, `cod_projeto`),
  INDEX `fk_tb_colaborador_projeto_org_tb_org1_idx` (`tb_org_id` ASC) VISIBLE,
  CONSTRAINT `fk_tb_colaborador_projeto_org_tb_org1`
    FOREIGN KEY (`tb_org_id`)
    REFERENCES `tb_org` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;

UPDATE `tb_colaborador_alocado` SET `tb_org_id` = 2;

ALTER TABLE `tb_colaborador_alocado` 
ADD CONSTRAINT `fk_tb_colaborador_alocado_tb_org1`
  FOREIGN KEY (`tb_org_id`)
  REFERENCES `tb_org` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `tb_empresa` 
ADD CONSTRAINT `fk_tb_empresa_tb_org1`
  FOREIGN KEY (`tb_org_id`)
  REFERENCES `tb_org` (`id`);

ALTER TABLE `tb_grupo_acesso` 
ADD CONSTRAINT `fk_tb_grupo_acesso_tb_org1`
  FOREIGN KEY (`tb_org_id`)
  REFERENCES `tb_org` (`id`);

DROP TABLE IF EXISTS `tb_diretoria` ;

INSERT INTO `tb_colaborador_org`
(`tb_org_id`,
`tb_colaborador_cpf`,
`cod_diretoria`,
`diretoria`,
`departamento`,
`cod_departamento`,
`cargo`,
`codigo_cargo`,
`cod_colaborador_externo`,
`data_admissao`,
`ativo`)
select 1, a.cpf, '', '', '', '', '', '', '', current_timestamp(), a.ativo from tb_colaborador a
;

ALTER TABLE `tb_interesse` 
DROP FOREIGN KEY `fk_tb_interesse_tb_usuario1`;
ALTER TABLE `tb_interesse` 
ADD CONSTRAINT `fk_tb_interesse_tb_usuario1`
  FOREIGN KEY (`usuario_criacao_id`)
  REFERENCES `tb_usuario` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_colaborador_interesse` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_interesse_tb_interesse1`;
ALTER TABLE `tb_colaborador_interesse` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_interesse_tb_interesse1`
  FOREIGN KEY (`interesse_id`)
  REFERENCES `tb_interesse` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_formacao` 
DROP FOREIGN KEY `fk_tb_formacao_tb_usuario1`;
ALTER TABLE `tb_formacao` 
ADD CONSTRAINT `fk_tb_formacao_tb_usuario1`
  FOREIGN KEY (`usuario_criacao_id`)
  REFERENCES `tb_usuario` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_colaborador_formacao` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_formacao_tb_formacao1`;
ALTER TABLE `tb_colaborador_formacao` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_formacao_tb_formacao1`
  FOREIGN KEY (`formacao_id`)
  REFERENCES `tb_formacao` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_competencia` 
DROP FOREIGN KEY `fk_tb_competencia_tb_usuario1`;
ALTER TABLE `tb_competencia` 
ADD CONSTRAINT `fk_tb_competencia_tb_usuario1`
  FOREIGN KEY (`usuario_criacao_id`)
  REFERENCES `tb_usuario` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_colaborador_competencia` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_competencia_tb_competencia1`;
ALTER TABLE `tb_colaborador_competencia` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_competencia_tb_competencia1`
  FOREIGN KEY (`competencia_id`)
  REFERENCES `tb_competencia` (`id`)
  ON DELETE CASCADE;
  
ALTER TABLE `tb_hobbies` 
DROP FOREIGN KEY `fk_tb_hobbies_tb_usuario1`;
ALTER TABLE `tb_hobbies` 
ADD CONSTRAINT `fk_tb_hobbies_tb_usuario1`
  FOREIGN KEY (`usuario_criacao_id`)
  REFERENCES `tb_usuario` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_colaborador_hobbies` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_hobbies_tb_hobbies1`;
ALTER TABLE `tb_colaborador_hobbies` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_hobbies_tb_hobbies1`
  FOREIGN KEY (`hobbies_id`)
  REFERENCES `tb_hobbies` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_modeloreferencia` 
DROP FOREIGN KEY `fk_tb_modeloreferencia_tb_usuario1`;
ALTER TABLE `tb_modeloreferencia` 
ADD CONSTRAINT `fk_tb_modeloreferencia_tb_usuario1`
  FOREIGN KEY (`usuario_criacao_id`)
  REFERENCES `tb_usuario` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_colaborador_modeloreferencia` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_modeloreferencia_tb_modeloreferencia1`;
ALTER TABLE `tb_colaborador_modeloreferencia` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_modeloreferencia_tb_modeloreferencia1`
  FOREIGN KEY (`modeloreferencia_id`)
  REFERENCES `tb_modeloreferencia` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_metodologia` 
DROP FOREIGN KEY `fk_tb_metodologia_tb_usuario1`;
ALTER TABLE `tb_metodologia` 
ADD CONSTRAINT `fk_tb_metodologia_tb_usuario1`
  FOREIGN KEY (`usuario_criacao_id`)
  REFERENCES `tb_usuario` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_colaborador_metodologia` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_metodologia_tb_metodologia1`;
ALTER TABLE `tb_colaborador_metodologia` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_metodologia_tb_metodologia1`
  FOREIGN KEY (`metodologia_id`)
  REFERENCES `tb_metodologia` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_dominionegocio` 
DROP FOREIGN KEY `fk_tb_dominionegocio_tb_usuario1`;
ALTER TABLE `tb_dominionegocio` 
ADD CONSTRAINT `fk_tb_dominionegocio_tb_usuario1`
  FOREIGN KEY (`usuario_criacao_id`)
  REFERENCES `tb_usuario` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_colaborador_dominionegocio` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_dominionegocio_tb_dominionegocio1`;
ALTER TABLE `tb_colaborador_dominionegocio` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_dominionegocio_tb_dominionegocio1`
  FOREIGN KEY (`dominionegocio_id`)
  REFERENCES `tb_dominionegocio` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_like_competencia` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_colaborador_competencia_tb_colaborad2`;
ALTER TABLE `tb_like_competencia` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_colaborador_competencia_tb_colaborad2`
  FOREIGN KEY (`colaborador_competencia_id`)
  REFERENCES `tb_colaborador_competencia` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_like_dominionegocio` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_colaborador_dominionegocio_tb_colabo4`;
ALTER TABLE `tb_like_dominionegocio` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_colaborador_dominionegocio_tb_colabo4`
  FOREIGN KEY (`colaborador_dominionegocio_id`)
  REFERENCES `tb_colaborador_dominionegocio` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_like_formacao` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_colaborador_formacao_tb_colaborador_2`;
ALTER TABLE `tb_like_formacao` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_colaborador_formacao_tb_colaborador_2`
  FOREIGN KEY (`colaborador_formacao_id`)
  REFERENCES `tb_colaborador_formacao` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_like_hobbies` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_colaborador_hobbies_tb_colaborador_h1`;
ALTER TABLE `tb_like_hobbies` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_colaborador_hobbies_tb_colaborador_h1`
  FOREIGN KEY (`colaborador_hobbies_id`)
  REFERENCES `tb_colaborador_hobbies` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_like_interesse` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_colaborador_interesse_tb_colaborador2`;
ALTER TABLE `tb_like_interesse` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_colaborador_interesse_tb_colaborador2`
  FOREIGN KEY (`colaborador_interesse_id`)
  REFERENCES `tb_colaborador_interesse` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_like_metodologia` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_colaborador_metodologia_tb_colaborad4`;
ALTER TABLE `tb_like_metodologia` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_colaborador_metodologia_tb_colaborad4`
  FOREIGN KEY (`colaborador_metodologia_id`)
  REFERENCES `tb_colaborador_metodologia` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_like_modeloreferencia` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_colaborador_modeloreferencia_tb_cola2`;
ALTER TABLE `tb_like_modeloreferencia` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_colaborador_modeloreferencia_tb_cola2`
  FOREIGN KEY (`colaborador_modeloreferencia_id`)
  REFERENCES `tb_colaborador_modeloreferencia` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_endosso_competencia` 
DROP FOREIGN KEY `fk_tb_colaborador_competencia_has_tb_colaborador_tb_colaborad1`;
ALTER TABLE `tb_endosso_competencia` 
ADD CONSTRAINT `fk_tb_colaborador_competencia_has_tb_colaborador_tb_colaborad1`
  FOREIGN KEY (`colaborador_competencia_id`)
  REFERENCES `tb_colaborador_competencia` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_endosso_dominionegocio` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_colaborador_dominionegocio_tb_colabo2`;
ALTER TABLE `tb_endosso_dominionegocio` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_colaborador_dominionegocio_tb_colabo2`
  FOREIGN KEY (`colaborador_dominionegocio_id`)
  REFERENCES `tb_colaborador_dominionegocio` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_endosso_formacao` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_colaborador_formacao_tb_colaborador_1`;
ALTER TABLE `tb_endosso_formacao` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_colaborador_formacao_tb_colaborador_1`
  FOREIGN KEY (`colaborador_formacao_id`)
  REFERENCES `tb_colaborador_formacao` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_endosso_metodologia` 
DROP FOREIGN KEY `fk_tb_colaborador_has_tb_colaborador_metodologia_tb_colaborad2`;
ALTER TABLE `tb_endosso_metodologia` 
ADD CONSTRAINT `fk_tb_colaborador_has_tb_colaborador_metodologia_tb_colaborad2`
  FOREIGN KEY (`colaborador_metodologia_id`)
  REFERENCES `tb_colaborador_metodologia` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_endosso_modeloreferencia` 
DROP FOREIGN KEY `fk_tb_colaborador_modeloreferencia_has_tb_colaborador_tb_cola1`;
ALTER TABLE `tb_endosso_modeloreferencia` 
ADD CONSTRAINT `fk_tb_colaborador_modeloreferencia_has_tb_colaborador_tb_cola1`
  FOREIGN KEY (`colaborador_modeloreferencia_id`)
  REFERENCES `tb_colaborador_modeloreferencia` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_token_sso` 
DROP FOREIGN KEY `fk_tb_token_sso_tb_usuario1`;
ALTER TABLE `tb_token_sso` 
ADD CONSTRAINT `fk_tb_token_sso_tb_usuario1`
  FOREIGN KEY (`tb_usuario_id`)
  REFERENCES `tb_usuario` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_usuario_grupo_acesso` 
DROP FOREIGN KEY `fk_tb_usuario_has_tb_grupo_acesso_tb_usuario1`;
ALTER TABLE `tb_usuario_grupo_acesso` 
ADD CONSTRAINT `fk_tb_usuario_has_tb_grupo_acesso_tb_usuario1`
  FOREIGN KEY (`tb_usuario_id`)
  REFERENCES `tb_usuario` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_empresa_usuario` 
DROP FOREIGN KEY `fk_tb_empresa_usuario_tb_usuario1`;
ALTER TABLE `tb_empresa_usuario` 
ADD CONSTRAINT `fk_tb_empresa_usuario_tb_usuario1`
  FOREIGN KEY (`tb_usuario_id`)
  REFERENCES `tb_usuario` (`id`)
  ON DELETE CASCADE;
ALTER TABLE `tb_colaborador_competencia_certificado` 
DROP FOREIGN KEY `fk_tb_colaborador_competencia_has_tb_certificado_tb_colaborad1`;
ALTER TABLE `tb_colaborador_competencia_certificado` 
ADD CONSTRAINT `fk_tb_colaborador_competencia_has_tb_certificado_tb_colaborad1`
  FOREIGN KEY (`tb_colaborador_competencia_id`)
  REFERENCES `tb_colaborador_competencia` (`id`)
  ON DELETE CASCADE
  ON UPDATE CASCADE;
ALTER TABLE `tb_escolaridade` 
DROP FOREIGN KEY `fk_tb_escolaridade_tb_formacao1`;
ALTER TABLE `tb_escolaridade` 
ADD CONSTRAINT `fk_tb_escolaridade_tb_formacao1`
  FOREIGN KEY (`tb_formacao_id`)
  REFERENCES `tb_formacao` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_usuario_tokenacesso` 
DROP FOREIGN KEY `fk_usuario_tokenAcesso_usuario2`;
ALTER TABLE `tb_usuario_tokenacesso` 
ADD CONSTRAINT `fk_usuario_tokenAcesso_usuario2`
  FOREIGN KEY (`usuario_id`)
  REFERENCES `tb_usuario` (`id`)
  ON DELETE CASCADE;

ALTER TABLE `tb_token_resete_senha` 
DROP FOREIGN KEY `fk_tb_token_resete_senha_tb_usuario1`;
ALTER TABLE `tb_token_resete_senha` 
ADD CONSTRAINT `fk_tb_token_resete_senha_tb_usuario1`
  FOREIGN KEY (`tb_usuario_id`)
  REFERENCES `tb_usuario` (`id`)
  ON DELETE CASCADE;

delete from tb_usuario a where not exists (select 1 from tb_colaborador b where b.cpf = a.cpf);

drop view vw_filtro_mapaalocacao;
CREATE 
VIEW `vw_filtro_mapaalocacao` AS
    SELECT 
        `c`.`cpf` AS `cpf`,
        `c`.`nome_completo` AS `nome_completo`,
        `c`.`ativo` AS `ativo`,
        `co`.`tb_org_id` AS `tb_org_id`,
        (SELECT 
                CONCAT('||',
                            GROUP_CONCAT(DISTINCT `co`.`descricao`
                                SEPARATOR '||'),
                            '||')
            FROM
                (`tb_colaborador_competencia` `cc`
                JOIN `tb_competencia` `co` ON ((`co`.`id` = `cc`.`competencia_id`)))
            WHERE
                (`cc`.`colaborador_cpf` = `c`.`cpf`)
            GROUP BY `cc`.`colaborador_cpf`) AS `hardskills`,
        (SELECT 
                CONCAT('||',
                            GROUP_CONCAT(DISTINCT `so`.`descricao`
                                SEPARATOR '||'),
                            '||')
            FROM
                (`tb_colaborador_idioma` `cs`
                JOIN `tb_idioma` `so` ON ((`so`.`id` = `cs`.`idioma_id`)))
            WHERE
                (`cs`.`colaborador_cpf` = `c`.`cpf`)
            GROUP BY `cs`.`colaborador_cpf`) AS `idiomas`
    FROM
        (`tb_colaborador` `c`
        JOIN `tb_colaborador_org` `co` ON ((`co`.`tb_colaborador_cpf` = `c`.`cpf`)))
    GROUP BY `c`.`cpf` , `co`.`tb_org_id`;

CREATE 
VIEW `vw_gestores_org` AS
    SELECT 
        `a`.`cod_colaborador_superior` AS `cod_colaborador_superior`,
        `c`.`nome_completo` AS `nome_completo`,
        `a`.`tb_org_id` AS `tb_org_id`
    FROM
        ((`tb_colaborador_hierarquia` `a`
        JOIN `tb_colaborador_org` `b` ON (((`a`.`cod_colaborador_superior` = `b`.`cod_colaborador_externo`)
            AND (`a`.`tb_org_id` = `b`.`tb_org_id`))))
        JOIN `tb_colaborador` `c` ON ((`c`.`cpf` = `b`.`tb_colaborador_cpf`)))
    GROUP BY `a`.`cod_colaborador_superior` , `c`.`nome_completo` , `a`.`tb_org_id`;

CREATE 
VIEW `vw_colaboradores_gestor` AS
    SELECT 
        `a`.`cpf` AS `cpf`,
        `a`.`nome_completo` AS `nome_completo`,
        `b`.`tb_org_id` AS `tb_org_id`,
        `c`.`cod_colaborador_superior` AS `cod_gerente`,
        `b`.`cod_colaborador_externo` AS `cod_colaborador`
    FROM
        ((`tb_colaborador` `a`
        JOIN `tb_colaborador_org` `b` ON ((`a`.`cpf` = `b`.`tb_colaborador_cpf`)))
        JOIN `tb_colaborador_hierarquia` `c` ON ((`c`.`cod_colaborador_externo` = `b`.`cod_colaborador_externo`)))
    GROUP BY `a`.`cpf` , `a`.`nome_completo` , `b`.`tb_org_id` , `c`.`cod_colaborador_superior` , `b`.`cod_colaborador_externo`;

INSERT INTO `tb_ad_sso` VALUES (1,'https://login.microsoftonline.com/','oauth2/v2.0/token','https://graph.microsoft.com/v1.0/','8f0133fa-8efb-40b1-8ac6-37c78469f445','fe473897-36a6-4d42-b992-6fe50af4b310','kFx8Q~pTCK.a9z7NHCrFBXvUtTjck9D5OhcO9boO','47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU','http://localhost:3000',2);

DROP view `colaboradorpordiretoria`;
CREATE 
VIEW `colaboradorpordiretoria` AS
    SELECT 
        `cs`.`diretoria` AS `descricao`,
        `cs`.`cod_diretoria` AS `diretoria_id`,
        COUNT(0) AS `quantidade`
    FROM
        (`tb_colaborador` `c`
        JOIN `tb_colaborador_org` `cs` ON ((`c`.`cpf` = `cs`.`tb_colaborador_cpf`)))
    WHERE
        ((`c`.`ativo` = 1) AND (`cs`.`ativo` = 1))
    GROUP BY `cs`.`cod_diretoria` , `cs`.`diretoria`
    ORDER BY `cs`.`cod_diretoria`;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

COMMIT;
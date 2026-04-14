CREATE TABLE `tb_gestor_externo_perfil_log` (
  `id` char(36) NOT NULL, 
  `tb_gestor_externo_perfil_id` char(36) NOT NULL, 
  `tb_org_id` int NOT NULL,                        
  `cod_gestor_externo` varchar(36) NOT NULL,       
  `nome_perfil` varchar(255) NOT NULL,             
  `custo_perfil` decimal(10,2) NOT NULL,           
  `ratecard_perfil` decimal(10,2) NOT NULL,        
  `informacoes_relevantes` text,                   
  `tb_permanencia_id` char(36) DEFAULT NULL,       
  `tb_modelo_trabalho_id` char(36) DEFAULT NULL,   
  `tb_profissional_localidade_id` char(36) DEFAULT NULL, 
  `ativo` tinyint(1) DEFAULT NULL,                 
  `data_criacao` timestamp NULL, 
  `data_alteracao` timestamp NULL, 
  `codigo_interno_colaborador_criacao` varchar(36) DEFAULT NULL, 
  `codigo_interno_colaborador_alteracao` varchar(36) DEFAULT NULL, 
  PRIMARY KEY (`id`),
  KEY `idx_id` (`tb_gestor_externo_perfil_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

ALTER TABLE `tb_gestor_externo_perfil_log`
ADD CONSTRAINT `fk_gestor_externo_perfil_log`
FOREIGN KEY (`tb_gestor_externo_perfil_id`) 
REFERENCES `tb_gestor_externo_perfil` (`id`)
ON DELETE CASCADE
ON UPDATE CASCADE;

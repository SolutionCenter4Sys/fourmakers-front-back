ALTER TABLE tb_vagas_srs
MODIFY COLUMN textForLinkedin LONGTEXT
CHARACTER SET utf8mb4
COLLATE utf8mb4_general_ci;

ALTER TABLE `tb_vagas_srs`
CHANGE COLUMN `descricao` `descricao` 
TEXT NULL COLLATE 'utf8mb4_general_ci' 
AFTER `status_vaga`,
CHANGE COLUMN `cargo` `cargo` 
TEXT NULL COLLATE 'utf8mb4_general_ci' 
AFTER `descricao`;

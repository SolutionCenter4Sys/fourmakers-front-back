ALTER TABLE `tb_log` 
ADD COLUMN `log_type` ENUM('Exception', 'LogDefault') NOT NULL DEFAULT 'LogDefault',
ADD COLUMN `process_identifier` VARCHAR(100) NULL;

ALTER TABLE `tb_log` 
ADD INDEX `idx_log_type_proc_identifier` (`log_type` ASC, `process_identifier` ASC);

update tb_log set log_type = 'Exception' where 1 = 1;
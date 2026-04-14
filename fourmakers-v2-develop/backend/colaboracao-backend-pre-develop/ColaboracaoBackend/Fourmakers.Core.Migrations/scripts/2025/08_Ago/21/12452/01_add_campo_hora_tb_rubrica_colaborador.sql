-- Script para adicionar campo 'hora' na tabela tb_rubrica_colaborador
-- feat/12452-add-campo-hora

ALTER TABLE `tb_rubrica_colaborador` 
ADD COLUMN `hora` VARCHAR(10) DEFAULT NULL 
COMMENT 'Campo para armazenar hora relacionada à rubrica do colaborador no formato HHH:MM';
-- Script: 01_11666_ALTER_TABLE_tb_rubrica_ADD_calculo_tipo.sql
-- Task: 11666 - Adicionar campo calculo_tipo na tabela tb_rubrica
-- Data: 29/07/2025
-- Descrição: Adiciona campo calculo_tipo como ENUM com valores 'Valor' e 'Porcentagem'

ALTER TABLE tb_rubrica 
ADD COLUMN calculo_tipo ENUM('Valor', 'Porcentagem') NOT NULL DEFAULT 'Valor';
ALTER TABLE tb_tbd_alocado
DROP FOREIGN KEY tb_tbd_alocado_ibfk_2;

ALTER TABLE tb_tbd_alocado
CHANGE COLUMN codigo_interno_colaborador codigo_interno_colaborador_gestor VARCHAR(36);

ALTER TABLE tb_tbd_alocado
ADD CONSTRAINT tb_tbd_alocado_ibfk_2
FOREIGN KEY (codigo_interno_colaborador_gestor) 
REFERENCES tb_colaborador(codigo_interno_colaborador)
ON DELETE CASCADE
ON UPDATE CASCADE;
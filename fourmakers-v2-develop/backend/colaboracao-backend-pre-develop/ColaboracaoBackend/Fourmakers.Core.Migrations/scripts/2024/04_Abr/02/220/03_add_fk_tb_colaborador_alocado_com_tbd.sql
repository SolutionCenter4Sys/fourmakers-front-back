ALTER TABLE tb_colaborador_alocado
ADD COLUMN cod_tbd_alocado INT;

ALTER TABLE tb_colaborador_alocado
ADD CONSTRAINT fk_tb_tbd_alocado
FOREIGN KEY (`cod_tbd_alocado`)
REFERENCES `tb_tbd_alocado`(`cod_tbd_alocado`);
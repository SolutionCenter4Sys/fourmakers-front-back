CREATE TABLE tb_usuario_backup LIKE tb_usuario;
INSERT INTO tb_usuario_backup SELECT * FROM tb_usuario;
 
CREATE TABLE tb_colaborador_org_backup LIKE tb_colaborador_org;
INSERT INTO tb_colaborador_org_backup SELECT * FROM tb_colaborador_org;
 
CREATE TABLE tb_colaborador_periodo_alocacao_backup LIKE tb_colaborador_periodo_alocacao;
INSERT INTO tb_colaborador_periodo_alocacao_backup SELECT * FROM tb_colaborador_periodo_alocacao;
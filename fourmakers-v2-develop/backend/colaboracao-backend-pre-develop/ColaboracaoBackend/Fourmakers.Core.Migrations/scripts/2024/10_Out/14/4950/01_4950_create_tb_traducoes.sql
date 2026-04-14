ALTER TABLE tb_colaborador_org ADD idioma varchar(5);

CREATE TABLE tb_traducoes (
  tabela_chave VARCHAR(150) NOT NULL,
  valor_chave VARCHAR(50) NOT NULL,
  idioma CHAR(5) NOT NULL,
  traducao VARCHAR(100) NOT NULL,
  CONSTRAINT pk_traducao PRIMARY KEY (tabela_chave, valor_chave, idioma)
);

INSERT INTO tb_traducoes VALUES
    ('tb_status_apontamento_grupo.cod_status_grupo', '4', 'es-ES', 'Eliminado'),
    ('tb_status_apontamento_grupo.cod_status_grupo', '5', 'es-ES', 'No Señalado'),
    ('tb_status_apontamento_grupo.cod_status_grupo', '1', 'es-ES', 'Pendiente'),
    ('tb_status_apontamento_grupo.cod_status_grupo', '2', 'es-ES', 'Aprobado'),
    ('tb_status_apontamento_grupo.cod_status_grupo', '3', 'es-ES', 'Rechazado');

INSERT INTO tb_traducoes VALUES
    ('tb_status_apontamento_grupo.cod_status_grupo', '4', 'en-US', 'Deleted'),
    ('tb_status_apontamento_grupo.cod_status_grupo', '5', 'en-US', 'Not pointed out'),
    ('tb_status_apontamento_grupo.cod_status_grupo', '1', 'en-US', 'Pending'),
    ('tb_status_apontamento_grupo.cod_status_grupo', '2', 'en-US', 'Approved'),
    ('tb_status_apontamento_grupo.cod_status_grupo', '3', 'en-US', 'Rejected');

INSERT INTO tb_traducoes VALUES
    ('tb_status_apontamento_grupo.cod_status_grupo', '4', 'pt-BR', 'Deletado'),
    ('tb_status_apontamento_grupo.cod_status_grupo', '5', 'pt-BR', 'Não Apontado'),
    ('tb_status_apontamento_grupo.cod_status_grupo', '1', 'pt-BR', 'Pendente'),
    ('tb_status_apontamento_grupo.cod_status_grupo', '2', 'pt-BR', 'Aprovado'),
    ('tb_status_apontamento_grupo.cod_status_grupo', '3', 'pt-BR', 'Reprovado');

INSERT INTO tb_traducoes (tabela_chave, valor_chave, idioma, traducao) VALUES
    ('tb_status_apontamento.cod_status_apontamento', '1', 'en-US', 'Pending from project manager'),
    ('tb_status_apontamento.cod_status_apontamento', '1', 'es-ES', 'Pendiente del gestor del proyecto'),
    
    ('tb_status_apontamento.cod_status_apontamento', '2', 'en-US', 'Pending from administrative manager'),
    ('tb_status_apontamento.cod_status_apontamento', '2', 'es-ES', 'Pendiente del gestor administrativo'),
    
    ('tb_status_apontamento.cod_status_apontamento', '3', 'en-US', 'Rejected by administrative manager'),
    ('tb_status_apontamento.cod_status_apontamento', '3', 'es-ES', 'Rechazado por el gestor administrativo'),
    
    ('tb_status_apontamento.cod_status_apontamento', '4', 'en-US', 'Rejected by project manager'),
    ('tb_status_apontamento.cod_status_apontamento', '4', 'es-ES', 'Rechazado por el gestor del proyecto'),
    
    ('tb_status_apontamento.cod_status_apontamento', '5', 'en-US', 'Approved'),
    ('tb_status_apontamento.cod_status_apontamento', '5', 'es-ES', 'Aprobado'),
    
    ('tb_status_apontamento.cod_status_apontamento', '6', 'en-US', 'Deleted'),
    ('tb_status_apontamento.cod_status_apontamento', '6', 'es-ES', 'Eliminado'),
    
    ('tb_status_apontamento.cod_status_apontamento', '7', 'en-US', 'Not pointed out'),
    ('tb_status_apontamento.cod_status_apontamento', '7', 'es-ES', 'No señalado');

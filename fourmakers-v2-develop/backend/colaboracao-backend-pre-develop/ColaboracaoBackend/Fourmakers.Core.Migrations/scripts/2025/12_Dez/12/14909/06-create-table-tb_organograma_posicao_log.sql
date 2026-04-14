CREATE TABLE tb_organograma_posicao_log (
  id varchar(36) NOT NULL,
  tb_org_id int,
  tb_organograma_departamento_id varchar(36),
  tb_gestor_externo_perfil_id char(36),
  tb_organograma_posicao_id_superior varchar(36),
  ativo tinyint,
  data_criacao timestamp DEFAULT CURRENT_TIMESTAMP,
  codigo_interno_colaborador_logado varchar(36),
  tipo varchar(50),
  KEY idx_organograma_posicao (id, data_criacao)
  )

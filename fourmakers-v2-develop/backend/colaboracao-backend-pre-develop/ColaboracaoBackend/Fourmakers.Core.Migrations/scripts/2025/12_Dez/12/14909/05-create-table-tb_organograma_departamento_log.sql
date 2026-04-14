CREATE TABLE tb_organograma_departamento_log (
  id varchar(36) NOT NULL,
  tb_org_id int,
  nome varchar(150),
  codigo_cliente varchar(45),
  tb_organograma_posicao_id_lider varchar(36), 
  ativo tinyint,
  data_criacao timestamp DEFAULT CURRENT_TIMESTAMP,
  codigo_interno_colaborador_logado varchar(36),
  tipo varchar(50),
  KEY idx_organograma_departamento (id, data_criacao)
  )


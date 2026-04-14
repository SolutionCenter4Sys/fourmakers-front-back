  CREATE TABLE tb_organograma_posicao_alocacao_log (
  id varchar(36) NOT NULL,
  tb_org_id int,
  codigo_cliente varchar(100), 
  tb_organograma_posicao_id  varchar(36),
  codigo_interno_colaborador varchar(36),
  data_inicio datetime DEFAULT NULL,
  data_fim    datetime DEFAULT NULL,
  ativo tinyint,
  data_criacao timestamp DEFAULT CURRENT_TIMESTAMP,
  codigo_interno_colaborador_logado varchar(36),
  tipo varchar(50),
  KEY idx_organograma_alocacao (id, data_criacao)
  )
  

CREATE TABLE tb_organograma_posicao_alocacao (
  id varchar(36) NOT NULL,
  /*-- Contexto Multi-tenant e Cliente */
  tb_org_id int  NOT NULL,
  codigo_cliente varchar(45) NOT NULL, 
  /*-- Vínculo  */
  tb_organograma_posicao_id  varchar(36) NOT NULL,
  codigo_interno_colaborador varchar(36) CHARACTER SET utf8mb3 NOT NULL,
  /*-- Vigência   */
  data_inicio datetime NOT NULL,
  data_fim datetime DEFAULT NULL,
  ativo tinyint DEFAULT 1,
  data_criacao timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  
  /*-- Índices   */
  KEY idx_aloc_org    (tb_org_id),
  KEY idx_aloc_cli    (codigo_cliente), /*-- Índice para filtrar alocações por cliente  */
  KEY idx_aloc_perfil (tb_organograma_posicao_id),
  KEY idx_aloc_colab  (codigo_interno_colaborador),
  KEY idx_aloc_vigencia (data_fim),
  
  /*-- Constraints   */
  CONSTRAINT fk_aloc_org    FOREIGN KEY (tb_org_id) REFERENCES tb_org (id),
  CONSTRAINT fk_aloc_perfil FOREIGN KEY (tb_organograma_posicao_id) REFERENCES tb_organograma_posicao (id),
  CONSTRAINT fk_aloc_colab  FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador (codigo_interno_colaborador)
  /*-- OBS: Se você tiver uma tabela 'tb_cliente', adicione a FK aqui:
  -- CONSTRAINT fk_aloc_cliente FOREIGN KEY (codigo_cliente) REFERENCES tb_cliente (id)   */
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
 

CREATE TABLE tb_gest_desemp_feedback (
    id VARCHAR(36) PRIMARY KEY,
    codigo_interno_colaborador_superior VARCHAR(36) NOT NULL,
    codigo_interno_colaborador_avaliado VARCHAR(36) NOT NULL,
    data_reuniao DATETIME,
    data_criacao DATETIME,
    data_atualizacao DATETIME,
    descricao_continuar TEXT,
    descricao_comecar TEXT,
    descricao_parar TEXT,
    descricao_observacoes_gerais TEXT,
    visualizado_pelo_colaborador BOOLEAN,
    data_visualizado_colaborador DATETIME
);

CREATE TABLE tb_gest_desemp_one_on_one (
    id VARCHAR(36) PRIMARY KEY,
    codigo_interno_colaborador_superior VARCHAR(36) NOT NULL,
    codigo_interno_colaborador_avaliado VARCHAR(36) NOT NULL,
    data_reuniao DATETIME,
    data_criacao DATETIME,
    descricao_anotacoes TEXT,
    registro_critico BOOLEAN,
    visualizado_pelo_colaborador BOOLEAN,
    data_visualizado_colaborador DATETIME
);

CREATE TABLE tb_gest_desemp_pauta_sugerida (
      id VARCHAR(36) PRIMARY KEY,
      codigo_interno_colaborador_criacao VARCHAR(36) NOT NULL,
      codigo_interno_colaborador_superior VARCHAR(36) NOT NULL,
      codigo_interno_colaborador_avaliado VARCHAR(36) NOT NULL,
      data_criacao DATETIME,
      data_atualizacao DATETIME,
      descricao_pauta_sugerida TEXT,
      tipo_origem ENUM('GESTOR', 'SUBORDINADO') NOT NULL
);

 ALTER TABLE tb_gest_desemp_pauta_sugerida
  ADD UNIQUE KEY uk_superior_avaliado_origem (
      codigo_interno_colaborador_superior,
      codigo_interno_colaborador_avaliado,
      tipo_origem
  );

CREATE TABLE tb_gest_desemp_parametrizacao_org (
    tb_org_id INT PRIMARY KEY,
    frequencia_esperada_one_on_one_dias INT,
    frequencia_esperada_feedback_dias INT,
    periodo_padrao_analise_dias INT
);


ALTER TABLE tb_gest_desemp_feedback
ADD CONSTRAINT fk_gdf_colab_superior
FOREIGN KEY (codigo_interno_colaborador_superior)
REFERENCES tb_colaborador (codigo_interno_colaborador)
ON UPDATE CASCADE
ON DELETE RESTRICT;

ALTER TABLE tb_gest_desemp_feedback
ADD CONSTRAINT fk_gdf_colab_avaliado
FOREIGN KEY (codigo_interno_colaborador_avaliado)
REFERENCES tb_colaborador (codigo_interno_colaborador)
ON UPDATE CASCADE
ON DELETE RESTRICT;


ALTER TABLE tb_gest_desemp_one_on_one
ADD CONSTRAINT fk_gooo_colab_superior
FOREIGN KEY (codigo_interno_colaborador_superior)
REFERENCES tb_colaborador (codigo_interno_colaborador)
ON UPDATE CASCADE
ON DELETE RESTRICT;

ALTER TABLE tb_gest_desemp_one_on_one
ADD CONSTRAINT fk_gooo_colab_avaliado
FOREIGN KEY (codigo_interno_colaborador_avaliado)
REFERENCES tb_colaborador (codigo_interno_colaborador)
ON UPDATE CASCADE
ON DELETE RESTRICT;


ALTER TABLE tb_gest_desemp_pauta_sugerida
ADD CONSTRAINT fk_gdps_colab_criacao
FOREIGN KEY (codigo_interno_colaborador_criacao)
REFERENCES tb_colaborador (codigo_interno_colaborador)
ON UPDATE CASCADE
ON DELETE RESTRICT;

ALTER TABLE tb_gest_desemp_pauta_sugerida
ADD CONSTRAINT fk_gdps_colab_superior
FOREIGN KEY (codigo_interno_colaborador_superior)
REFERENCES tb_colaborador (codigo_interno_colaborador)
ON UPDATE CASCADE
ON DELETE RESTRICT;

ALTER TABLE tb_gest_desemp_pauta_sugerida
ADD CONSTRAINT fk_gdps_colab_avaliado
FOREIGN KEY (codigo_interno_colaborador_avaliado)
REFERENCES tb_colaborador (codigo_interno_colaborador)
ON UPDATE CASCADE
ON DELETE RESTRICT;

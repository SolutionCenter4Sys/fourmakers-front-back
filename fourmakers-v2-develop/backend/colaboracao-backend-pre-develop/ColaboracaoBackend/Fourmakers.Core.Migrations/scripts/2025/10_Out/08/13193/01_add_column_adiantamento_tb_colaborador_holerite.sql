ALTER TABLE tb_colaborador_holerite
ADD COLUMN adiantamento TINYINT NOT NULL DEFAULT 0;

ALTER TABLE tb_colaborador_holerite
ADD COLUMN tb_rubrica_colaborador_id char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE tb_colaborador_holerite ADD CONSTRAINT tb_colaborador_holerite_tb_rubrica_colaborador_FK 
FOREIGN KEY (tb_rubrica_colaborador_id) REFERENCES tb_rubrica_colaborador(id) ON DELETE RESTRICT ON UPDATE RESTRICT;


INSERT INTO tb_rubrica (
    id,
    ativo,
    codigo_interno_colaborador_alteracao,
    codigo_interno_colaborador_criacao,
    descricao,
    rubrica_tipo,
    codigo_rubrica,
    tb_org_id,
    refletir_contabil,
    calculo_tipo
)
Values(
    UUID(),
    1,
    '00000000000',
    '00000000000',
    'Adiantamento Salarial',
    1,
    '5501',
    9,
    1,
    1  
);

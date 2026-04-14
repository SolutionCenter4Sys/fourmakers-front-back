ALTER TABLE tb_verba_tipo_custo
    ADD COLUMN descricao_acao varchar(300) default "";

UPDATE tb_verba_tipo_custo
SET descricao_acao = "Variável: esse tipo de verba permite definir um limite de lançamento, que é alertado ao usuário no momento da solicitação."
WHERE id = 1;

UPDATE tb_verba_tipo_custo
SET descricao_acao = "Fixo: esse tipo de verba permite ao usuário informar quantas unidades foram consumidas, que serão multiplicadas pelo valor estabelecido por unidade, o que definirá o valor final da solicitação."
WHERE id = 2;
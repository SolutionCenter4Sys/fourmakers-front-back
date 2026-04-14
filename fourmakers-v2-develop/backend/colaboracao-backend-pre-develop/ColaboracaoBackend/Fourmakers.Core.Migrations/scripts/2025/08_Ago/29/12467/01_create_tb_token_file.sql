CREATE TABLE tb_token_file (
    token CHAR(36) PRIMARY KEY,
    nome_arquivo VARCHAR(500) NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IX_tb_token_file_nome_arquivo ON tb_token_file (nome_arquivo);


INSERT INTO tb_token_file
select UUID(),relative_path,current_date from tb_solicitacao_documento tsd
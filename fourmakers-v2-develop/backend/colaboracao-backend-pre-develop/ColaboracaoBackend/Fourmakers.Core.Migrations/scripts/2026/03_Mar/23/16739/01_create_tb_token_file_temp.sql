CREATE TABLE tb_token_files_temp (
     token char(36) NOT NULL,
     nome_arquivo varchar(500) NOT NULL,
     data_criacao datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
     PRIMARY KEY (token),
     KEY IX_tb_token_files_temp_nome_arquivo (nome_arquivo)
);
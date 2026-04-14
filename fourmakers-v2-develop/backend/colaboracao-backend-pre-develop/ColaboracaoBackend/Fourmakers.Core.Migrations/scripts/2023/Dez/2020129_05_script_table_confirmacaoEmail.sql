/*
	Nome: Gabrielle Prado
    Objetivo do script: criar uma tabela para manipulação do código para confirmação do email de usuário.
    Data: 29/01/2024
*/



CREATE TABLE tb_confirmacao_email (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome_colaborador VARCHAR(255),
    usuario_id BIGINT,
    cpf VARCHAR(15),
    email VARCHAR(255),
    codigo_enviado INT,
    codigo_confirmado BOOLEAN
);
ALTER TABLE tb_questao_chat_bot 
DROP COLUMN resposta;

ALTER TABLE tb_questao_chat_bot 
DROP COLUMN questao;

ALTER TABLE tb_questao_chat_bot 
ADD COLUMN query TEXT DEFAULT NULL;

ALTER TABLE tb_questao_chat_bot 
ADD COLUMN mensagem TEXT NOT NULL;

ALTER TABLE tb_questao_chat_bot 
ADD COLUMN tipo int NOT NULL DEFAULT 1;

ALTER TABLE tb_questao_chat_bot 
ADD COLUMN referencia_resposta_id int DEFAULT NULL;
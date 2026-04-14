ALTER TABLE tb_vaga_candidato
DROP FOREIGN KEY fk_status;

ALTER TABLE tb_vaga_candidato_status_log
DROP FOREIGN KEY tb_vaga_candidato_status_log_ibfk_1;

ALTER TABLE tb_candidato_status
DROP PRIMARY KEY,
ADD PRIMARY KEY (id, origem);

INSERT INTO tb_candidato_status (id, descricao, origem) VALUES
(1, 'Inscrição registrada', 'Fourmakers'),
(2, 'Entrevista Inicial', 'Fourmakers'),
(3, 'Testes Técnicos', 'Fourmakers'),
(4, 'Entrevista com Cliente', 'Fourmakers'),
(5, 'Carta-Oferta', 'Fourmakers');

ALTER TABLE tb_vaga_candidato
ADD CONSTRAINT fk_status
FOREIGN KEY (status_id) REFERENCES tb_candidato_status(id);

ALTER TABLE tb_vaga_candidato_status_log
ADD CONSTRAINT tb_vaga_candidato_status_log_ibfk_1
FOREIGN KEY (status_id) REFERENCES tb_candidato_status(id);

select * from tb_vaga_candidato;
show create table tb_vaga_candidato;
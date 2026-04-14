ALTER TABLE tb_agendas_comerciais
ADD CONSTRAINT fk_tb_agenda_objetivo_id FOREIGN KEY (tb_agenda_objetivo_id)
REFERENCES tb_agenda_objetivo (id) ;

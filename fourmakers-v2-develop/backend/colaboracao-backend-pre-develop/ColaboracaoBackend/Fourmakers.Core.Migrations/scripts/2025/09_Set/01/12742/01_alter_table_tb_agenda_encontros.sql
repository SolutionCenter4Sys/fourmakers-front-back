ALTER TABLE tb_agenda_encontros
ADD COLUMN agenda_pai_id INT NULL,
ADD CONSTRAINT fk_agenda_pai
    FOREIGN KEY (agenda_pai_id) REFERENCES tb_agenda_encontros(ID);
ALTER TABLE tb_encontros
  ADD COLUMN tb_agenda_encontros_id INT NULL,
  ADD CONSTRAINT fk_encontros_agenda
        FOREIGN KEY (tb_agenda_encontros_id)
        REFERENCES tb_agenda_encontros(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE;

UPDATE tb_encontros e
JOIN   tb_agenda_encontros a  ON a.tb_encontros_id = e.id
SET    e.tb_agenda_encontros_id = a.id;

ALTER TABLE tb_encontros
  ADD UNIQUE KEY uq_encontro_agenda (id, tb_agenda_encontros_id);

ALTER TABLE tb_agenda_encontros
  DROP FOREIGN KEY fk_tb_agenda_encontros_encontros,
  DROP COLUMN tb_encontros_id;

CREATE INDEX idx_encontros_agenda ON tb_encontros(tb_agenda_encontros_id);
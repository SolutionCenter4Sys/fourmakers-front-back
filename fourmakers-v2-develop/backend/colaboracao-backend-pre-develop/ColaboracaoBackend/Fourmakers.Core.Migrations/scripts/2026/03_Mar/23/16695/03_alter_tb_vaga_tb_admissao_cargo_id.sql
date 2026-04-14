-- =============================================================================
-- 16695 — tb_vaga.tb_admissao_cargo_id → tb_admissao_cargo.id (nullable legado)
-- =============================================================================
-- tb_vaga costuma estar em utf8mb3; tb_admissao_cargo em utf8mb4 (p.ex. COLLATE=utf8mb4_0900_ai_ci).
-- Sem CHARACTER SET explícito, CHAR(36) herda utf8mb3 e o MySQL 8 recusa a FK (erro 3780).
-- O COLLATE deve ser o mesmo de tb_admissao_cargo.id (em geral utf8mb4_0900_ai_ci no MySQL 8).
-- Confirme com: SHOW FULL COLUMNS FROM tb_admissao_cargo WHERE Field = 'id';
-- =============================================================================
ALTER TABLE tb_vaga
  ADD COLUMN tb_admissao_cargo_id CHAR(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL
    COMMENT 'Cargo de admissão (tb_admissao_cargo)' AFTER cargo;

ALTER TABLE tb_vaga
  ADD CONSTRAINT fk_tb_vaga_tb_admissao_cargo
    FOREIGN KEY (tb_admissao_cargo_id) REFERENCES tb_admissao_cargo (id)
    ON DELETE RESTRICT ON UPDATE CASCADE;

CREATE INDEX idx_tb_vaga_tb_admissao_cargo_id ON tb_vaga (tb_admissao_cargo_id);

begin;
ALTER TABLE tb_candidato
ADD cargo_atual_ultimo VARCHAR(50),
ADD salario_atual_ultimo DECIMAL(10, 2),
ADD tipo_contrato_atual_ultimo VARCHAR(20),
ADD modalidade_atual_ultima int,
ADD aceita_sugestoes_vagas BOOLEAN;
CHANGE COLUMN pretencao_salarial pretensao_salarial DOUBLE;
commit;







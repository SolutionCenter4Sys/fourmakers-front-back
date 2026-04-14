UPDATE tb_metodologia SET confirmada = 1
WHERE YEAR(data_criacao) < 2024;
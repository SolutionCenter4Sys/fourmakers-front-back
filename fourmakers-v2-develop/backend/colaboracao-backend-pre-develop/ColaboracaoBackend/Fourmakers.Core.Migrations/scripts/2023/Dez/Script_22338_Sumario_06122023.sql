begin;

CREATE VIEW estatisticas_etnia AS
SELECT
    COUNT(CASE WHEN etnia = 'preta' THEN 1 END) as preta,
    COUNT(CASE WHEN etnia = 'branca' THEN 1 END) as branca,
    COUNT(CASE WHEN etnia = 'negra' THEN 1 END) as negra,
    COUNT(CASE WHEN etnia = 'amarela' THEN 1 END) as amarela,
    COUNT(CASE WHEN etnia = 'parda' THEN 1 END) as parda,
    COUNT(CASE WHEN etnia = 'outras' THEN 1 END) as outras,
    COUNT(CASE WHEN etnia = 'prefiro não responder' THEN 1 END) as prefiro_nao_responder,
    COUNT(CASE WHEN etnia = 'sem resposta' THEN 1 END) as sem_resposta
FROM tb_colaborador;


CREATE VIEW estatisticas_idade AS
SELECT
    COUNT(CASE WHEN YEAR(CURDATE()) - YEAR(data_nascimento) <= 25 THEN 1 END) as idade_25_ou_menos,
    COUNT(CASE WHEN YEAR(CURDATE()) - YEAR(data_nascimento) BETWEEN 26 AND 34 THEN 1 END) as idade_26_34,
    COUNT(CASE WHEN YEAR(CURDATE()) - YEAR(data_nascimento) BETWEEN 35 AND 44 THEN 1 END) as idade_35_44,
    COUNT(CASE WHEN YEAR(CURDATE()) - YEAR(data_nascimento) BETWEEN 45 AND 54 THEN 1 END) as idade_45_54,
    COUNT(CASE WHEN YEAR(CURDATE()) - YEAR(data_nascimento) >= 55 THEN 1 END) as idade_55_ou_mais
FROM tb_colaborador;


CREATE VIEW estatisticas_tempo_servico AS
SELECT
    COUNT(CASE WHEN DATEDIFF(CURDATE(), admissao) <= 365 THEN 1 END) as ate_1_ano,
    COUNT(CASE WHEN DATEDIFF(CURDATE(), admissao) BETWEEN 366 AND 730 THEN 1 END) as entre_1_e_2_anos,
    COUNT(CASE WHEN DATEDIFF(CURDATE(), admissao) BETWEEN 731 AND 1825 THEN 1 END) as entre_3_e_5_anos,
    COUNT(CASE WHEN DATEDIFF(CURDATE(), admissao) BETWEEN 1826 AND 3650 THEN 1 END) as entre_6_e_10_anos,
    COUNT(CASE WHEN DATEDIFF(CURDATE(), admissao) BETWEEN 3651 AND 5475 THEN 1 END) as entre_11_e_15_anos,
    COUNT(CASE WHEN DATEDIFF(CURDATE(), admissao) BETWEEN 5476 AND 7300 THEN 1 END) as entre_16_e_20_anos,
    COUNT(CASE WHEN DATEDIFF(CURDATE(), admissao) > 7300 THEN 1 END) as mais_de_20_anos
FROM tb_colaborador;


CREATE VIEW estatisticas_genero AS
SELECT
    COUNT(CASE WHEN genero = 'Feminino' THEN 1 END) as feminino,
    COUNT(CASE WHEN genero = 'Masculino' THEN 1 END) as masculino,
    COUNT(CASE WHEN genero = 'Não binário' THEN 1 END) as nao_binario,
    COUNT(CASE WHEN genero = 'Homem cisgênero' THEN 1 END) as homem_cisgenero,
    COUNT(CASE WHEN genero = 'Agênero' THEN 1 END) as Agenero,
    COUNT(CASE WHEN genero = 'Transgênero' THEN 1 END) as transgenero,
    COUNT(CASE WHEN genero = 'Mulher cisgênero' THEN 1 END) as mulher_cisgenero,
    COUNT(CASE WHEN genero = 'Prefiro não responder' THEN 1 END) as prefiro_nao_responder,
    COUNT(CASE WHEN genero = 'Outro' THEN 1 END) as outro
FROM tb_colaborador;



CREATE VIEW estatisticas_orientacao_sexual AS
SELECT
    COUNT(CASE WHEN orientacao_sexual = 'Assexual' THEN 1 END) as assexual,
    COUNT(CASE WHEN orientacao_sexual = 'Bissexual' THEN 1 END) as bissexual,
    COUNT(CASE WHEN orientacao_sexual = 'Heterossexual' THEN 1 END) as heterossexual,
    COUNT(CASE WHEN orientacao_sexual = 'Homossexual' THEN 1 END) as homossexual,
    COUNT(CASE WHEN orientacao_sexual = 'Outras' THEN 1 END) as outras,
    COUNT(CASE WHEN orientacao_sexual = 'Prefiro não responder' THEN 1 END) as prefiro_nao_responder,
    COUNT(CASE WHEN orientacao_sexual = 'Pansexual' THEN 1 END) as pansexual
FROM tb_colaborador;


CREATE OR REPLACE VIEW estatisticas_escolaridade AS
SELECT
    COUNT(CASE WHEN escolaridade = 'Ensino fundamental completo ou menos' THEN 1 END) as ensino_fundamental_completo_menos,
    COUNT(CASE WHEN escolaridade = 'Ensino médio completo ou menos' THEN 1 END) as ensino_medio_completo_menos,
    COUNT(CASE WHEN escolaridade = 'Ensino superior incompleto' THEN 1 END) as ensino_superior_incompleto,
    COUNT(CASE WHEN escolaridade = 'Ensino superior completo' THEN 1 END) as ensino_superior_completo,
    COUNT(CASE WHEN escolaridade = 'Ensino superior cursando' THEN 1 END) as ensino_superior_cursando,
    COUNT(CASE WHEN escolaridade = 'Pós-graduação incompleto' THEN 1 END) as pos_graduacao_incompleto,
    COUNT(CASE WHEN escolaridade = 'Pós-graduação cursando' THEN 1 END) as pos_graduacao_cursando,
    COUNT(CASE WHEN escolaridade = 'Mestrado ou Doutorado cursando' THEN 1 END) as mestrado_doutorado_cursando,
    COUNT(CASE WHEN escolaridade = 'Pós-graduação completo' THEN 1 END) as pos_graduacao_completo,
    COUNT(CASE WHEN escolaridade = 'Mestrado ou Doutorado completo' THEN 1 END) as mestrado_doutorado_completo
FROM tb_colaborador;


commit;
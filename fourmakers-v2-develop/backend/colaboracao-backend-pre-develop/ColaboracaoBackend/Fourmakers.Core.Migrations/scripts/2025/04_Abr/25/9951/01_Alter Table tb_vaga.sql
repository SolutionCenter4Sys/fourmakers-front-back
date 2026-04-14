CREATE TEMPORARY TABLE temporaria_tb_vaga AS
SELECT * FROM tb_vaga;

DROP TABLE IF EXISTS tb_vaga;

CREATE TABLE `tb_vaga` (
  `id` varchar(36) NOT NULL,
  `titulo` varchar(255) NOT NULL,
  `numero_de_vagas` int DEFAULT NULL,
  `taxa_maxima_hora` varchar(255) DEFAULT NULL,
  `descricao` text,
  `funcao` text,
  `data_criacao` datetime DEFAULT NULL,
  `localizacao` varchar(50) DEFAULT NULL,
  `observacao_localizacao` text,
  `estado` varchar(50) DEFAULT NULL,
  `cidade` varchar(100) DEFAULT NULL,
  `tb_usuario_criador_id` int NOT NULL,
  `tb_usuario_aprovador_id` int NOT NULL,
  `tb_gestor_id` int NOT NULL,
  `tb_status_vaga_id` varchar(36) NOT NULL,
  `tb_origem_vaga_id` varchar(36) NOT NULL,
  PRIMARY KEY (`id`)
);

INSERT INTO tb_vaga
(
id,
titulo,
numero_de_vagas,
taxa_maxima_hora,
descricao,
funcao,
data_criacao,
localizacao,
observacao_localizacao,
estado,
cidade,
tb_usuario_criador_id,
tb_usuario_aprovador_id,
tb_gestor_id,
tb_status_vaga_id,
tb_origem_vaga_id
)
SELECT
uuid(),
titulo,
numero_de_vagas,
taxa_maxima_hora,
descricao,
funcao,
data_criacao,
tipo_localizacao,
observacao_localizacao,
estado,
cidade,
1691,
1691,
1691,
'bd8563c0-21de-11f0-92d8-029c6b897a8d',
'bd8563c0-21de-11f0-92d8-029c6b897a8d'
FROM temporaria_tb_vaga;

select count(*) from tb_vaga;

DROP TABLE temporaria_tb_vaga;

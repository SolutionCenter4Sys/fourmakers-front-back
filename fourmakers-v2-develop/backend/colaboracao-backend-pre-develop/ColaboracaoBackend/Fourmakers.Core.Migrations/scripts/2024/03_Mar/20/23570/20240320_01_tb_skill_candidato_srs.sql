begin;
create table tb_skill_candidato_srs (
    id int auto_increment primary key,
    candidato_id bigint,
    categoria_id int,
    descricao_id int,
    nivel_id int,
    data_criacao datetime,
    data_alteracao datetime
);
commit
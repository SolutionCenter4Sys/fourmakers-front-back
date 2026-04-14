CREATE TABLE `tb_status_apontamento_grupo` (
    `id` CHAR(36) PRIMARY KEY NOT NULL,
    `descricao` varchar(50),
    `cod_status_grupo` integer
);

CREATE UNIQUE INDEX `tb_status_apontamento_grupo_index` ON `tb_status_apontamento_grupo`(`cod_status_grupo`);

INSERT INTO tb_status_apontamento_grupo (`id`, `cod_status_grupo`, `descricao`) VALUES (UUID(), 1, 'Pendente');
INSERT INTO tb_status_apontamento_grupo (`id`, `cod_status_grupo`, `descricao`) VALUES (UUID(), 2, 'Aprovado');
INSERT INTO tb_status_apontamento_grupo (`id`, `cod_status_grupo`, `descricao`) VALUES (UUID(), 3, 'Reprovado');
INSERT INTO tb_status_apontamento_grupo (`id`, `cod_status_grupo`, `descricao`) VALUES (UUID(), 4, 'Deletado');
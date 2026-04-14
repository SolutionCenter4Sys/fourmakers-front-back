CREATE TABLE `tb_status_apontamento` (
    `id` CHAR(36) PRIMARY KEY NOT NULL,
    `descricao` varchar(50),
    `cod_status_apontamento` integer,
    `tb_cod_status_grupo` integer,
    FOREIGN KEY (`tb_cod_status_grupo`) REFERENCES `tb_status_apontamento_grupo`(`cod_status_grupo`)
);

CREATE UNIQUE INDEX `tb_status_apontamento_index` ON `tb_status_apontamento`(`cod_status_apontamento`);

INSERT INTO tb_status_apontamento (`id`, `cod_status_apontamento`, `descricao`, `tb_cod_status_grupo`) VALUES (UUID(), 1, 'Pendente do gestor do projeto', 1);
INSERT INTO tb_status_apontamento (`id`, `cod_status_apontamento`, `descricao`, `tb_cod_status_grupo`) VALUES (UUID(), 2, 'Pendente do gestor administrativo', 1);
INSERT INTO tb_status_apontamento (`id`, `cod_status_apontamento`, `descricao`, `tb_cod_status_grupo`) VALUES (UUID(), 3, 'Reprovado pelo gestor administrativo', 3);
INSERT INTO tb_status_apontamento (`id`, `cod_status_apontamento`, `descricao`, `tb_cod_status_grupo`) VALUES (UUID(), 4, 'Reprovado pelo gestor do projeto', 3);
INSERT INTO tb_status_apontamento (`id`, `cod_status_apontamento`, `descricao`, `tb_cod_status_grupo`) VALUES (UUID(), 5, 'Aprovado', 2);
INSERT INTO tb_status_apontamento (`id`, `cod_status_apontamento`, `descricao`, `tb_cod_status_grupo`) VALUES (UUID(), 6, 'Deletado', 4);
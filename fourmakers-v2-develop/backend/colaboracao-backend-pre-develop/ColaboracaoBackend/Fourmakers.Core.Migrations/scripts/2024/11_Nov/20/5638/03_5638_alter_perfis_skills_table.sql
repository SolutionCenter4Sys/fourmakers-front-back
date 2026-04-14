ALTER TABLE `tb_gestor_externo_perfil_skill`
DROP PRIMARY KEY,
ADD PRIMARY KEY (
    `tb_gestor_externo_perfil_id`,
    `tb_item_perfil_id`,
    `skill_id`,
    `tb_nivel_id`
);

INSERT INTO tb_parametro (id,nome_parametro,descricao_parametro,codigo_parametro,codigo_modulo_sistema,ativo) 
VALUES (uuid(), 'Associação Automática do Colaborador ao Projeto',
	'Esta parametrização permite que, ao realizar uma nova alocação, a associação ocorra automaticamente, ou seja, não é necessário ter um relacionamento prévio na tabela Colaborador x Projeto. Quando essa configuração está ativa, como consequência, na tela de Nova Alocação, todos os colaboradores serão apresentados, sem filtrar por projeto.',
    'ASSOCIACAO_AUTOMATICA_COLABORADOR_PROJETO', 'MAPA_DE_ALOCACAO', 1);
    
 INSERT INTO tb_org_parametro_configuracao (id,tb_org_id,codigo_parametro,valor_parametro) 
 VALUES (uuid(),4,'ASSOCIACAO_AUTOMATICA_COLABORADOR_PROJETO','true');
 
insert into tb_destinatarios_email_candidatos (email, assunto, area, tb_org_id, anexo, ativo, data_criacao, data_alteracao)
		values('gestaodepessoal@foursys.com.br','Template Candidatos - Gestão de pessoal','DP', 2, 1, 1, curdate(),curdate()),
		      ('onboarding@foursys.com.br','Template Candidatos - Onboarding','ONBG', 2, 1, 1, curdate(),curdate()),
		      ('hrbp@foursys.com.br','Template Candidatos - HRBP','HRBP', 2, 1, 1, curdate(),curdate());

insert into tb_destinatarios_email_candidatos (email, assunto, area, tb_org_id, anexo, ativo, data_criacao, data_alteracao)
		values('onboarding@foursys.com.br','Template Candidatos - Onboarding','ONBG', 2, 0, 1, now(),now()),
		      ('suporte.ti@foursys.com.br','Template Candidatos - Suporte TI','STI', 2, 0, 1, now(),now()),
		      ('nayara.alves@foursys.com.br','Template Candidatos - Produtos Foursys','FOURSYS', 2, 0, 1, now(),now());


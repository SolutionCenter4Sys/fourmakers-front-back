insert into tb_parceiros_gestao_contratos (id, tb_parceiros_id, url_anexo)
select 	uuid(),
		id,
		url_aditivos
from tb_parceiros
WHERE url_aditivos IS NOT NULL AND url_aditivos != '';

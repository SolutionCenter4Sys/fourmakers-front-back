insert into tb_parceiros_gestao_contratos (id, tb_parceiros_id, url_anexo)
select 	uuid(),
		id,
		url_contrato
from tb_parceiros
WHERE url_contrato IS NOT NULL AND url_contrato != '';

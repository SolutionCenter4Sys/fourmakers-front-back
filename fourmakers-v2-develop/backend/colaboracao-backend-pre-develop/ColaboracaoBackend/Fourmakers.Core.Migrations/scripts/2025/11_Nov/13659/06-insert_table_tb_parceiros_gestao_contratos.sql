insert into tb_parceiros_gestao_contratos (id, tb_parceiros_id, url_anexo)
select 	uuid(),
		id,
		url_Nda
from tb_parceiros
WHERE url_Nda IS NOT NULL  AND url_Nda != '';

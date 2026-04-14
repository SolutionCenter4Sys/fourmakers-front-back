INSERT INTO tb_parametro (id,nome_parametro,descricao_parametro,codigo_parametro,codigo_modulo_sistema,ativo) 
VALUES (uuid(), 'Configuração da Integração com a API SOAP da LG',
	'Esta parametrização permite configurar a integração com a API SOAP da LG. Ao ativar essa configuração, as solicitações de dados serão encaminhadas para a API SOAP da LG, permitindo a comunicação e integração com sistemas externos. É importante garantir que as credenciais e configurações da API estejam corretamente definidas para garantir o funcionamento adequado da integração.',
    'CONFIGURACAO_API_SOAP_LG', 'LG', 1);
    
    INSERT INTO tb_org_parametro_configuracao (id, tb_org_id, codigo_parametro, valor_parametro) 
VALUES (uuid(), 2, 'CONFIGURACAO_API_SOAP_LG', 
  '"Usuario":"eng.fourmakers@foursys.com.br","Senha":"nFmbRxd*fFZc","GuidTenant":"CC118D00-6826-420E-89D3-CCFEE7F8E7C2","Ambiente":449');



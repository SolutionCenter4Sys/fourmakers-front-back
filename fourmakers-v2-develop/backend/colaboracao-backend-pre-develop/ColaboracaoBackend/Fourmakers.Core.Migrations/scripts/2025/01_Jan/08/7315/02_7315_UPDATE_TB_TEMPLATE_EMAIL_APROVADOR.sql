UPDATE tb_template_email 
SET template = 
'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html dir="ltr" lang="es">
  <head>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
  <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500&display=swap" rel="stylesheet">
</head>
<body style="width: 550px; font-family: Arial, sans-serif; -webkit-font-smoothing: antialiased; -moz-osx-font-smoothing: grayscale; text-rendering: optimizeLegibility; margin: 0; padding: 0; box-sizing: border-box;">
    <div style="display: flex;">
        <div style="height: 134px; width: 221px; background-color: #8C1EE3; display: flex; align-items: center; justify-content: center;">
            <img src="https://fsys2-public.s3.us-east-1.amazonaws.com/logo_name_email_branca.png" style="width: 108px;">
        </div>
        <img src="https://fsys2-public.s3.us-east-1.amazonaws.com/banner_email.png" style="height: 134px;">
    </div>
    <div style="background-color: #FFF; max-width: 550px; min-height: 200px; padding: 40px;">
        <p style="color: #8C1EE3; font-weight: 700; font-size: 20px; line-height: 22px;">Ola, ${NOME},</p>

        <p style="font-weight: 400; font-size: 14px; color: #000; line-height: 22px;">Existem lançamentos pendentes de aprovação no timesheet.<br/> 
            Por favor, revise e aprove os lançamentos nos seguintes projetos:</p>
        <table style=" margin-top: 40px; width: 100%; border-spacing: 0; border-collapse: collapse; font-family: "Inter", sans-serif; box-shadow: 0px 2px 4px -2px #1018280F, 0px 4px 8px -2px #1018281A;">
          <thead>
            <tr style="text-align: left; background-color: #F9FAFB; border-bottom: 1px solid #E1E1E1;">
              <th style="padding: 12px 24px; font-size: 12px; font-weight: 500; color: #47564F;">Projeto</th>
              <th style="padding: 12px 24px; font-size: 12px; font-weight: 500; color: #47564F;">Horas pendentes</th>
            </tr>
          </thead>
          <tbody style="font-size: 12px; font-weight: 500; color: #1C1C1F;">
			${ITENS_DA_TABELA}
          </tbody>
        </table>
        <a href="${COLABORACAO_URL_APROVACAO}" target="_blank" style="display: inline-block; text-decoration: none; padding: 5px 12px 5px 12px; line-height: 28px; background-color: #222239; border-radius: 4px; color: #FFF; font-size: 16px; font-weight: 500; text-align: center; cursor: pointer; margin-top: 20px">Acessar o painel de aprovação</a>
        <p style="margin-top: 20px; font-weight: 400; font-size: 14px; color: #000; line-height: 22px;">
            Se precisar de mais informações ou suporte,<br/> 
            nossa equipe está à disposição. 
          
        </p>
        <p style="margin-top: 20px; font-weight: 400; font-size: 14px; color: #000; line-height: 22px;">
        	Obrigado!
        </p>
        <div style="display: flex; justify-content: space-between; align-items: center;">
            <p style="font-weight: 400; font-size: 14px; color: #000; line-height: 22px;">
                Unindo forças para criar o futuro
            </p>
            <img src="https://fsys2-public.s3.us-east-1.amazonaws.com/logo_name_email_preta.png" style="width: 74px;"/>
        </div>
    </div>
    <div style="display: grid; grid-template-columns: 34px 155px 76px 65px 49px 1fr; height: 10px; width: 100%;">
        <div style="height: 10px; background-color: #5B1096;"></div>
        <div style="height: 10px; background-color: #741BBB;"></div>
        <div style="height: 10px; background-color: #319DD9;"></div>
        <div style="height: 10px; background-color: #4CBFFF;"></div>
        <div style="height: 10px; background-color: #3BFE95;"></div>
        <div style="height: 10px; background-color: #8C1EE3;"></div>
    </div>
</body>
</html>'
WHERE codigo = "NOTIFICACAO_APONTAMENTOS_STATUS_PENDENTE";
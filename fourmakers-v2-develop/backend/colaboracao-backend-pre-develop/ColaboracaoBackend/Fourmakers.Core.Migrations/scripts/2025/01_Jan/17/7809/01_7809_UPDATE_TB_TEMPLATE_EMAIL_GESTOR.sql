UPDATE tb_template_email 
SET template = 
'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html dir="ltr" lang="es">
  <head>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
  <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500&display=swap" rel="stylesheet">
</head>
<body style="width: 550px; font-family: Arial, sans-serif; -webkit-font-smoothing: antialiased; -moz-osx-font-smoothing: grayscale; text-rendering: optimizeLegibility; margin: 0; padding: 0; box-sizing: border-box;">
    <table style="width: 100%; height: 134px; background-color: #8C1EE3; table-layout: fixed; border-collapse: collapse;">
        <tr>
            <td style="width: 221px; text-align: center; vertical-align: middle; border: none;">
                <img src="https://fsys2-public.s3.us-east-1.amazonaws.com/logo_name_email_branca.png" style="width: 108px;">
            </td>
            <td style="border: none;">
                <img src="https://fsys2-public.s3.us-east-1.amazonaws.com/banner_email.png" style="height: 133px; width: 100%;">
            </td>
        </tr>
    </table>
    <div style="background-color: #FFF; max-width: 550px; min-height: 200px; padding: 40px;">
        <p style="color: #8C1EE3; font-weight: 700; font-size: 20px; line-height: 22px;">Olá, ${NOME},</p>

        <p style="font-weight: 400; font-size: 14px; color: #000; line-height: 22px;">Você enviou uma notificação para lista abaixo solicitando aos<br/> aprovadores que revisem e aprovem os lançamentos pendentes.</p>
        <table style=" margin-top: 40px; width: 100%; border-spacing: 0; border-collapse: collapse; font-family: "Inter", sans-serif; box-shadow: 0px 2px 4px -2px #1018280F, 0px 4px 8px -2px #1018281A;">
          <thead>
            <tr style="text-align: left; background-color: #F9FAFB; border-bottom: 1px solid #E1E1E1;">
              <th style="padding: 12px 24px; font-size: 12px; font-weight: 500; color: #47564F;">Enviado para</th>
              <th style="padding: 12px 24px; font-size: 12px; font-weight: 500; color: #47564F;">Projeto</th>
              <th style="padding: 12px 24px; font-size: 12px; font-weight: 500; color: #47564F;">Horas pendentes</th>
            </tr>
          </thead>
          <tbody style="font-size: 12px; font-weight: 500; color: #1C1C1F;">
			${ITENS_DA_TABELA}
          </tbody>
        </table>
        <p style="margin-top: 20px; font-weight: 400; font-size: 14px; color: #000; line-height: 22px;">
            Se precisar de mais informações ou suporte,<br/> 
            nossa equipe está à disposição.<br/>
          <a href="mailto:ajuda@fourmakers.io" target="_blank">ajuda@fourmakers.io</a>
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
    <table style="width: 100%; height: 10px; border-collapse: collapse;">
      <tr>
          <td style="width: 34px; background-color: #5B1096; border: none;"></td>
          <td style="width: 155px; background-color: #741BBB; border: none;"></td>
          <td style="width: 76px; background-color: #319DD9; border: none;"></td>
          <td style="width: 65px; background-color: #4CBFFF; border: none;"></td>
          <td style="width: 49px; background-color: #3BFE95; border: none;"></td>
          <td style="width: 1fr; background-color: #8C1EE3; border: none;"></td>
      </tr>
  </table>
</body>
</html>'
WHERE codigo = "NOTIFICA_GESTOR_APONTAMENTOS_STATUS_PENDENTE";
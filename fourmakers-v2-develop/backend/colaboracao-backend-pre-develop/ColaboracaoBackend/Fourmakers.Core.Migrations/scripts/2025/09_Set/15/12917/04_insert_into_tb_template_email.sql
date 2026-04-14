INSERT INTO tb_template_email(id, codigo, descricao, template, ativo) VALUES
(
UUID(),
'NOTIFICACAO_PJ_EMISSAO',
'Email para notificar colaboradores Prestadores que contem nota fical aguardando emissão',
'
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
<p style="color: #8C1EE3; font-weight: 700; font-size: 20px; line-height: 22px;">Prezado(a) {COLABORADOR},</p>
<p style="font-weight: 400; font-size: 14px; color: #000; line-height: 22px;">
  Está disponível no sistema Fourmakers
<a href="{PATH_TO_LINK}" target="_blank" rel="noopener noreferrer">
    {PATH_TO_LINK}
</a>
  para registro da sua Nota Fiscal com vigência em {VIGENCIA}.  
  Favor realizar o upload da sua NF.
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
</html>
',
1
);
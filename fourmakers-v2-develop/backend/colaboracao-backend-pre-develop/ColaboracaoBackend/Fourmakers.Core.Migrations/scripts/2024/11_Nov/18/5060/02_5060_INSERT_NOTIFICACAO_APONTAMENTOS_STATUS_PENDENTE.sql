INSERT INTO tb_template_email
(id, codigo, descricao, template, ativo, data_alteracao, data_criacao, idioma)
VALUES('16b342dd-a1df-11ef-92d8-029c6b897a8d', 'NOTIFICACAO_APONTAMENTOS_STATUS_PENDENTE', 'Notificação para Aprovadores sobre Status Pendente', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
  <html dir="ltr" lang="en">
    <head>
      <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
      <link rel="preconnect" href="https://fonts.googleapis.com">
      <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
      <link
        href="https://fonts.googleapis.com/css2?family=Nunito:ital,wght@0,200..1000;1,200..1000&display=swap"
        rel="stylesheet">
    </head>
    <body
      style="background-color:#f3f3f5;font-family:Nunito,sans-serif">
      <table align="center" width="400" border="0" cellPadding="0"
        cellSpacing="0" role="presentation"
        style="max-width:100%;width:400px;margin:0 auto;background-color:#ffffff">
        <tbody>
          <tr style="width:400">
            <td>
              <table align="center" width="400" border="0" cellPadding="0"
                cellSpacing="0" role="presentation"
                style="border-radius:5px 5px 0 0;display:flex;flex-direction:column; width:400px">
                <tbody style="width:400">
                  <tr
                    style="width: 100%; display: flex; align-items: center; justify-content: space-between;">
                    <td><img
                        src="https://fsys2-public.s3.amazonaws.com/HeaderEmail.png"
                        style="display:block;outline:none;border:none;text-decoration:none;"
                        width="400" /></td>
                  </tr>
                </tbody>
              </table>
 
 <table align="center" width="100%" border="0" cellPadding="0"
                 cellSpacing="0" role="presentation"
                 style="padding:30px 30px 0px 30px">
                 <tbody>
                   <tr>
                     <td>
                       <h2
                         style="margin:0 0 30px;font-weight:bold;font-size:21px;line-height:22px;color:#FF5315">Prezado, 
                         ${NOME}</h2>
                       <p
                         style="font-size:13px;line-height:21px;margin:16px 0;color:#3c3f44">
                         Existem lançamentos pendentes de aprovação no timesheet. Por favor, revise e aprove os lançamentos nos seguintes projetos:</p>
  					   
  					<table style="width:100%;border-collapse:collapse;margin:16px 0;font-size:11px">
  					<thead>
  						<tr>
  						<th style="border:1px solid #ccc;padding:6px;text-align:left;background-color:#f5f5f5;font-weight:bold">
  							Projeto
  						</th>
  						<th style="border:1px solid #ccc;padding:6px;text-align:left;background-color:#f5f5f5;font-weight:bold">
  							Horas Pendentes
  						</th>
  						</tr>
  					</thead>
              <tbody>
    <tr>
      <td style="border:1px solid #ccc;padding:6px">${descricaoProjeto}</td>
      <td style="border:1px solid #ccc;padding:6px">${quantidadeHorasPendenteAprovacao}</td>
    </tr>
  </tbody>
  					</table>
                       
  <p style="font-size:13px;line-height:21px;margin:16px 0;color:#3c3f44">
  <b>Para corrigir o apontamento, siga as instruções abaixo:</b></p>
  
 
  					
  <p style="font-size:13px;line-height:21px;margin:16px 0;color:#3c3f44">
    Acesse o painel de aprovação aqui: <a href="${COLABORACAO_URL_APROVACAO}">Link</a>

         <p style="font-size:13px;line-height:21px;margin:10px 0;color:#3c3f44">Qualquer dúvida, estamos aqui para ajudar: <br>ajuda@fourmakers.io <br>
             Até breve, Equipe Fourmakers.io</p>
   
                     </td>
                   </tr>
                 </tbody>
               </table>
 
              <table align="center" width="400" border="0" cellPadding="0"
                cellSpacing="0" role="presentation"
                style="border-radius:5px 5px 0 0;display:flex;flex-direction:column; width:400px">
                <tbody style="width:100%">
                  <tr
                    style="width: 100%; display: flex; align-items: center; justify-content: space-between;">
                    <td><img
                        src="https://fsys2-public.s3.amazonaws.com/FooterEmail.png"
                        style="display:block;outline:none;border:none;text-decoration:none;"
                        width="400" /></td>
                  </tr>
                </tbody>
              </table>
            </td>
          </tr>
        </tbody>
      </table>
    </body>
  </html>', 1, '2024-11-13 13:48:20', '2024-11-13 13:48:20', 'pt-BR');
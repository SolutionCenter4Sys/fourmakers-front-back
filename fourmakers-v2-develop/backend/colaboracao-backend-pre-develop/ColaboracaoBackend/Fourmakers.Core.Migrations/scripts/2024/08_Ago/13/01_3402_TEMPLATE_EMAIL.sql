INSERT INTO `tb_template_email` (`id`,`codigo`, `descricao`, `template`, `ativo`)
VALUES ( UUID(),'REPROVACAO_APONTAMENTO_HORAS', 'Template de e-mail que será enviado no momento que ocorre uma reprovação no apontamento de horas.',
 '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                         style="margin:0 0 30px;font-weight:bold;font-size:21px;line-height:22px;color:#FF5315">Prezado
                         ${NOME},</h2>
                       <p
                         style="font-size:13px;line-height:21px;margin:16px 0;color:#3c3f44">
                         Informamos que você possui lançamentos reprovados.</p>
  					   
  					<table style="width:100%;border-collapse:collapse;margin:16px 0;font-size:11px">
  					<thead>
  						<tr>
  						<th style="border:1px solid #ccc;padding:6px;text-align:left;background-color:#f5f5f5;font-weight:bold">
  							Data Lançamento
  						</th>
  						<th style="border:1px solid #ccc;padding:6px;text-align:left;background-color:#f5f5f5;font-weight:bold">
  							Projeto
  						</th>
  						<th style="border:1px solid #ccc;padding:6px;text-align:left;background-color:#f5f5f5;font-weight:bold">
  							Atividade
  						</th>
  						</tr>
  					</thead>
  					<tbody>
  						${LINHA_REPROVACAO}
  					</tbody>
  					</table>
  				   
                       
  					<p style="font-size:13px;line-height:21px;margin:13px 0;color:#3c3f44">
  					<b>Motivo da reprovação:</b> ${JUSTIFICATIVA_REPROVACAO}
  					</p>
  					
  
  					 <p
                         style="font-size:13px;line-height:21px;margin:16px 0;color:#3c3f44">
                         <b>Para corrigir o apontamento, siga as instruções abaixo:</b></p>
                      
  					 
  					<ol style="font-size:13px;line-height:21px;margin-left:20px 0;color:#3c3f44">
  						<li>Acessar o Fourmakers</li>
  						<li>Clicar em Timesheet</li>
  						<li>Clicar no dia marcado com status "Reprovado", em vermelho, com ícone "x"</li>
  						<li>Ajustar as horas reprovadas de acordo com as orientações do aprovador</li>
  					</ol>
  					
  
                      
  					 
  					 <p
                         style="font-size:13px;line-height:21px;margin:16px 0;color:#3c3f44">
                         Caso tenha alguma dúvida referente a reprovação, entre em contato com o aprovador ou administrador.</p>

                       <p
                         style="font-size:13px;line-height:21px;margin:10px 0;color:#3c3f44">Qualquer
                         dúvida, estamos aqui para ajudar: <br>
                         ajuda@fourmakers.io <br>
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
  </html>'
 , '1' );

ALTER TABLE tb_template_email_rotina ADD COLUMN assunto VARCHAR(255) AFTER tb_org_id;

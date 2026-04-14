ALTER TABLE tb_template_email ADD idioma varchar(5);

UPDATE tb_template_email SET idioma = 'pt-BR' where codigo = 'REPROVACAO_APONTAMENTO_HORAS' and idioma is null;

INSERT INTO `tb_template_email` (`id`,`codigo`, `descricao`, `template`, `ativo`, `idioma`) VALUES (UUID(),'REPROVACAO_APONTAMENTO_HORAS', 'Template de e-mail que será enviado no momento que ocorre uma reprovação no apontamento de horas.', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html dir="ltr" lang="es">
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
                      style="margin:0 0 30px;font-weight:bold;font-size:21px;line-height:22px;color:#FF5315">Estimado
                      ${NOME},</h2>
                    <p
                      style="font-size:13px;line-height:21px;margin:16px 0;color:#3c3f44">
                      Le informamos que tiene lanzamientos rechazados.</p>

                <table style="width:100%;border-collapse:collapse;margin:16px 0;font-size:11px">
                <thead>
                  <tr>
                  <th style="border:1px solid #ccc;padding:6px;text-align:left;background-color:#f5f5f5;font-weight:bold">
                    Fecha de Lanzamiento
                  </th>
                  <th style="border:1px solid #ccc;padding:6px;text-align:left;background-color:#f5f5f5;font-weight:bold">
                    Proyecto
                  </th>
                  <th style="border:1px solid #ccc;padding:6px;text-align:left;background-color:#f5f5f5;font-weight:bold">
                    Actividad
                  </th>
                  </tr>
                </thead>
                <tbody>
                  ${LINHA_REPROVACAO}
                </tbody>
                </table>

                <p style="font-size:13px;line-height:21px;margin:13px 0;color:#3c3f44">
                <b>Motivo del rechazo:</b> ${JUSTIFICATIVA_REPROVACAO}
                </p>

                 <p
                      style="font-size:13px;line-height:21px;margin:16px 0;color:#3c3f44">
                      <b>Para corregir el registro, siga las instrucciones a continuación:</b></p>

                <ol style="font-size:13px;line-height:21px;margin-left:20px 0;color:#3c3f44">
                  <li>Acceder a Fourmakers</li>
                  <li>Hacer clic en Timesheet</li>
                  <li>Hacer clic en el día marcado con el estado "Rechazado", en rojo, con el icono "x"</li>
                  <li>Ajustar las horas rechazadas según las orientaciones del aprobador</li>
                </ol>

                 <p
                      style="font-size:13px;line-height:21px;margin:16px 0;color:#3c3f44">
                      Si tiene alguna duda respecto al rechazo, comuníquese con el aprobador o administrador.</p>

                <p
                      style="font-size:13px;line-height:21px;margin:10px 0;color:#3c3f44">Cualquier
                      duda, estamos aquí para ayudar: <br>
                      ajuda@fourmakers.io <br>
                      Hasta pronto, Equipo Fourmakers.io</p>

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
</html>
', '1', 'es-ES');

INSERT INTO `tb_template_email` (`id`,`codigo`, `descricao`, `template`, `ativo`, `idioma`) VALUES (UUID(),'REPROVACAO_APONTAMENTO_HORAS', 'Template de e-mail que será enviado no momento que ocorre uma reprovação no apontamento de horas.', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                      style="margin:0 0 30px;font-weight:bold;font-size:21px;line-height:22px;color:#FF5315">Dear
                      ${NOME},</h2>
                    <p
                      style="font-size:13px;line-height:21px;margin:16px 0;color:#3c3f44">
                      We inform you that you have rejected entries.</p>

                <table style="width:100%;border-collapse:collapse;margin:16px 0;font-size:11px">
                <thead>
                  <tr>
                  <th style="border:1px solid #ccc;padding:6px;text-align:left;background-color:#f5f5f5;font-weight:bold">
                    Entry Date
                  </th>
                  <th style="border:1px solid #ccc;padding:6px;text-align:left;background-color:#f5f5f5;font-weight:bold">
                    Project
                  </th>
                  <th style="border:1px solid #ccc;padding:6px;text-align:left;background-color:#f5f5f5;font-weight:bold">
                    Activity
                  </th>
                  </tr>
                </thead>
                <tbody>
                  ${LINHA_REPROVACAO}
                </tbody>
                </table>

                <p style="font-size:13px;line-height:21px;margin:13px 0;color:#3c3f44">
                <b>Reason for rejection:</b> ${JUSTIFICATIVA_REPROVACAO}
                </p>

                 <p
                      style="font-size:13px;line-height:21px;margin:16px 0;color:#3c3f44">
                      <b>To correct the entry, please follow the instructions below:</b></p>

                <ol style="font-size:13px;line-height:21px;margin-left:20px 0;color:#3c3f44">
                  <li>Access Fourmakers</li>
                  <li>Click on Timesheet</li>
                  <li>Click on the day marked with status "Rejected", in red, with an "x" icon</li>
                  <li>Adjust the rejected hours according to the approver''s instructions</li>
                </ol>

                 <p
                      style="font-size:13px;line-height:21px;margin:16px 0;color:#3c3f44">
                      If you have any questions regarding the rejection, please contact the approver or administrator.</p>

                <p
                      style="font-size:13px;line-height:21px;margin:10px 0;color:#3c3f44">If you have any questions, we are here to help: <br>
                      ajuda@fourmakers.io <br>
                      See you soon, Fourmakers.io Team</p>

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
</html>', '1', 'en-US');
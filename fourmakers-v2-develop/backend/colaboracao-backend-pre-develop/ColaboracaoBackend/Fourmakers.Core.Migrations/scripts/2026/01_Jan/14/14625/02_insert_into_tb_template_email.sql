INSERT INTO tb_template_email(id, codigo , descricao , template )
VALUES(
  UUID(),
  'CANAL_DENUNCIA',
  'Template de email para canal de denuncias',
  '
  <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                          style="margin:0 0 30px;font-weight:bold;font-size:21px;line-height:22px;color:#FF5315;text-align: center;">CANAL DE DENUNCIA</h2>
                        <p
                          style="font-size:15px;line-height:21px;margin:16px 0;color:#3c3f44;text-align: center;">
                            {MESSAGE}
                     
    
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
  '
);
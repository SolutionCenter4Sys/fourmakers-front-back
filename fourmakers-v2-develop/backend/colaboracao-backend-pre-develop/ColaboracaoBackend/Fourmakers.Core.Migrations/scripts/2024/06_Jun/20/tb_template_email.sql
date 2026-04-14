truncate table tb_template_email;

insert into tb_template_email (id, codigo, descricao, template, ativo) values 
(uuid(), 'CADASTRO_NOVO_USUARIO_SENHA', 'Template para novos colaboradores fourmakers com cpf e senha', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <table align="center" width="100%" border="0" cellPadding="0"
      cellSpacing="0" role="presentation"
      style="max-width:100%;width:680px;margin:0 auto;background-color:#ffffff">
      <tbody>
        <tr style="width:100%">
          <td>
            <table align="center" width="100%" border="0" cellPadding="0"
              cellSpacing="0" role="presentation"
              style="border-radius:5px 5px 0 0;display:flex;flex-direction:column; width:100%">
              <tbody style="width:100%">
                <svg style="width: 100%; " width="100%"
                  height="100%" id="svg" viewBox="0 0 1440 490"
                  xmlns="http://www.w3.org/2000/svg"
                  class="transition duration-300 ease-in-out delay-150"><path
                    d="M 0,500 L 0,187 C 122.80000000000001,200.46666666666667 245.60000000000002,213.93333333333334 398,199 C 550.4,184.06666666666666 732.4000000000001,140.73333333333335 911,134 C 1089.6,127.26666666666665 1264.8,157.13333333333333 1440,187 L 1440,500 L 0,500 Z"
                    stroke="none" stroke-width="0" fill="#1c1c38"
                    fill-opacity="1"
                    class="transition-all duration-300 ease-in-out delay-150 path-0"
                    transform="rotate(-180 720 250)"></path></svg>

                <tr
                  style="width: 100%; display: flex; align-items: center; justify-content: space-between; margin-top: -233px; padding: 30px;">
                  <td><img
                      src="https://storage.googleapis.com/flutterflow-io-6f20.appspot.com/projects/fourmakers-2-vr1q98/assets/ecfp9qwak914/logo-darkmode.png"
                      style="display:block;outline:none;border:none;text-decoration:none;"
                      width="156" /></td>
                  <td><img
                      src="https://storage.googleapis.com/flutterflow-io-6f20.appspot.com/projects/fourmakers-2-vr1q98/assets/wntmgxmbqzt5/avatar-inclinado.png"
                      style="display:block;outline:none;border:none;text-decoration:none; margin-right: 30px;"
                      width="146" /></td>
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
                      style="margin:0 0 30px;font-weight:bold;font-size:21px;line-height:22px;color:#FF5315">Olá
                      ${NOME},</h2>
                    <h2
                      style="margin:0 0 15px;font-weight:bold;font-size:22px;line-height:21px;color:#FF5315">Bem-vindo(a)
                      ao Fourmakers!!!</h2>
                    <p
                      style="font-size:15px;line-height:21px;margin:16px 0;color:#3c3f44">
                      Faça seu login com CPF e senha cadastrados para entrar
                      facilmente, descobrir conexões e oportunidades que esperam
                      por você.</p>
					
                    <p
                      style="font-size:15px;line-height:21px;margin:16px 0;color:#3c3f44">
                      Sua nova senha de acesso: <b>${SENHA}</b></p>

                
                    <br>

                    <table align="center" width="100%" border="0"
                      cellPadding="0" cellSpacing="0" role="presentation"
                      style="margin-top:24px;display:block">
                      <tbody>
                        <tr>
                          <td><a href="${URL}"
                              style="color:#fff;text-decoration:none;background-color:#ff5315;border:1px solid #ff5315;font-size:17px;line-height:17px;padding:13px 17px;border-radius:8px;max-width:120px"
                              target="_blank">Ir ao Fourmakers</a></td>
                        </tr>
                      </tbody>
                    </table>
                    <br>
                    <br>
                    <p
                      style="font-size:15px;line-height:21px;margin:16px 0;color:#3c3f44">Qualquer
                      dúvida, estamos aqui para ajudar: <br>
                      suporte@fourmakers.io. <br>
                      Até breve, Equipe Fourmakers.io</p>

                  </td>
                </tr>
              </tbody>
            </table>
            <table align="center" width="100%" border="0" cellPadding="0"
              cellSpacing="0" role="presentation"
              style="border-radius:5px 5px 0 0;display:flex;flex-direction:column; width:100%">
              <tbody style="width:100%">
                <svg width="100%" height="100%" id="svg" viewBox="0 0 1440 590"
                  xmlns="http://www.w3.org/2000/svg"
                  class="transition duration-300 ease-in-out delay-150"><path
                    d="M 0,600 L 0,225 C 110.34449760765548,178.21531100478467 220.68899521531097,131.43062200956936 307,133 C 393.31100478468903,134.56937799043064 455.5885167464115,184.4928229665072 545,200 C 634.4114832535885,215.5071770334928 750.956937799043,196.59808612440193 846,184 C 941.043062200957,171.40191387559807 1014.5837320574162,165.11483253588517 1110,173 C 1205.4162679425838,180.88516746411483 1322.7081339712918,202.94258373205741 1440,225 L 1440,600 L 0,600 Z"
                    stroke="none" stroke-width="0" fill="#1c1c38"
                    fill-opacity="1"
                    class="transition-all duration-300 ease-in-out delay-150 path-0"></path></svg>

                <tr
                  style="width: 100%; display: flex; align-items: center; justify-content: center; margin-top: -100px;">
                  <td><img
                      src="https://storage.googleapis.com/flutterflow-io-6f20.appspot.com/projects/fourmakers-2-vr1q98/assets/ecfp9qwak914/logo-darkmode.png"
                      style="display:block;outline:none;border:none;text-decoration:none;"
                      width="156" /></td>

                </tr>
              </tbody>
            </table>
          </td>
        </tr>
      </tbody>
    </table>

  </body>

</html>', 1),
(uuid(), 'RESET_SENHA_ORG_SSO', 'Template para quando usuario que possui acesso via SSO mas acessa botão esqueci a senha', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
     <table align="center" width="100%" border="0" cellPadding="0"
       cellSpacing="0" role="presentation"
       style="max-width:100%;width:680px;margin:0 auto;background-color:#ffffff">
       <tbody>
         <tr style="width:100%">
           <td>
             <table align="center" width="100%" border="0" cellPadding="0"
               cellSpacing="0" role="presentation"
               style="border-radius:5px 5px 0 0;display:flex;flex-direction:column; width:100%">
               <tbody style="width:100%">
                 <svg style="width: 100%; " width="100%"
                   height="100%" id="svg" viewBox="0 0 1440 490"
                   xmlns="http://www.w3.org/2000/svg"
                   class="transition duration-300 ease-in-out delay-150"><path
                     d="M 0,500 L 0,187 C 122.80000000000001,200.46666666666667 245.60000000000002,213.93333333333334 398,199 C 550.4,184.06666666666666 732.4000000000001,140.73333333333335 911,134 C 1089.6,127.26666666666665 1264.8,157.13333333333333 1440,187 L 1440,500 L 0,500 Z"
                     stroke="none" stroke-width="0" fill="#1c1c38"
                     fill-opacity="1"
                     class="transition-all duration-300 ease-in-out delay-150 path-0"
                     transform="rotate(-180 720 250)"></path></svg>
 
                 <tr
                   style="width: 100%; display: flex; align-items: center; justify-content: space-between; margin-top: -233px; padding: 30px;">
                   <td><img
                       src="https://storage.googleapis.com/flutterflow-io-6f20.appspot.com/projects/fourmakers-2-vr1q98/assets/ecfp9qwak914/logo-darkmode.png"
                       style="display:block;outline:none;border:none;text-decoration:none;"
                       width="156" /></td>
                   <td><img
                       src="https://storage.googleapis.com/flutterflow-io-6f20.appspot.com/projects/fourmakers-2-vr1q98/assets/wntmgxmbqzt5/avatar-inclinado.png"
                       style="display:block;outline:none;border:none;text-decoration:none; margin-right: 30px;"
                       width="146" /></td>
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
                       style="margin:0 0 30px;font-weight:bold;font-size:21px;line-height:22px;color:#FF5315">Olá
                       ${NOME},</h2>
                     <h2
                       style="margin:0 0 15px;font-weight:bold;font-size:22px;line-height:21px;color:#FF5315">Bem-vindo(a)
                       ao Fourmakers!!!</h2>
                     <p
                       style="font-size:15px;line-height:21px;margin:16px 0;color:#3c3f44">Use
                       sua conta Microsoft para entrar facilmente e descobrir
                       conexões e <br> oportunidades que esperam por você.</p>
 
                     <p
                       style="font-size:15px;line-height:21px;margin:16px 0;color:#3c3f44">Clique
                       no botão abaixo e
                       acesse agora</p>
 
                     <table align="center" width="100%" border="0"
                       cellPadding="0" cellSpacing="0" role="presentation"
                       style="margin-top:44px;display:block">
                       <tbody>
                         <tr>
                           <td><a href="${URL_SSO}"                              style="color:#fff;text-decoration:none;background-color:#ff5315;border:1px solid #ff5315;font-size:17px;line-height:17px;padding:13px 17px;border-radius:8px;max-width:120px"
                               target="_blank">Ir ao Fourmakers</a></td>
                         </tr>
                       </tbody>
                     </table>
                     <br>
                     <br>
                     <p
                       style="font-size:15px;line-height:21px;margin:16px 0;color:#3c3f44">Qualquer
                       dúvida, estamos aqui para ajudar: <br>
                       suporte@fourmakers.io. <br>
                       Até breve, Equipe Fourmakers.io</p>
 
                   </td>
                 </tr>
               </tbody>
             </table>
             <table align="center" width="100%" border="0" cellPadding="0"
               cellSpacing="0" role="presentation"
               style="border-radius:5px 5px 0 0;display:flex;flex-direction:column; width:100%">
               <tbody style="width:100%">
                 <svg width="100%" height="100%" id="svg" viewBox="0 0 1440 590"
                   xmlns="http://www.w3.org/2000/svg"
                   class="transition duration-300 ease-in-out delay-150"><path
                     d="M 0,600 L 0,225 C 110.34449760765548,178.21531100478467 220.68899521531097,131.43062200956936 307,133 C 393.31100478468903,134.56937799043064 455.5885167464115,184.4928229665072 545,200 C 634.4114832535885,215.5071770334928 750.956937799043,196.59808612440193 846,184 C 941.043062200957,171.40191387559807 1014.5837320574162,165.11483253588517 1110,173 C 1205.4162679425838,180.88516746411483 1322.7081339712918,202.94258373205741 1440,225 L 1440,600 L 0,600 Z"
                     stroke="none" stroke-width="0" fill="#1c1c38"
                     fill-opacity="1"
                     class="transition-all duration-300 ease-in-out delay-150 path-0"></path></svg>
 
                 <tr
                   style="width: 100%; display: flex; align-items: center; justify-content: center; margin-top: -100px;">
                   <td><img
                       src="https://storage.googleapis.com/flutterflow-io-6f20.appspot.com/projects/fourmakers-2-vr1q98/assets/ecfp9qwak914/logo-darkmode.png"
                       style="display:block;outline:none;border:none;text-decoration:none;"
                       width="156" /></td>
 
                 </tr>
               </tbody>
             </table>
           </td>
         </tr>
       </tbody>
     </table>
 
   </body>
 
 </html>', 1);

update tb_org set subdominio = 'https://app.fourmakers.io/' where id = 1;
update tb_org set subdominio = 'https://app.fourmakers.io/foursys' where id = 2;
update tb_org set subdominio = 'https://app.fourmakers.io/bwg' where id = 3;
update tb_org set subdominio = 'https://app.fourmakers.io/numen' where id = 4;
update tb_org set subdominio = 'https://app.fourmakers.io/showcase' where id = 5;
update tb_org set subdominio = 'https://app.fourmakers.io/atos' where id = 6;

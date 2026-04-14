INSERT
	INTO
	tb_template_email (id,
	codigo,
	descricao,
	template,
	idioma)
VALUES (
UUID()
,
"CV_EM_MASSA", 
'Template para envio de cv em massa', 
'
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html dir="ltr" lang="es">
<head>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500&display=swap" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500&display=swap" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;600;700&display=swap" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Lato:wght@400;500&display=swap" rel="stylesheet" />
</head>
<body>
    <div style="width: 550px; position: relative;">
        <table style="width: 550px; border-collapse: collapse; position: relative; z-index: 2;">
            <tr>
                <th />
                <th style="background-color: #5b1096; width: 34px; height: 10px; border: none; padding: 0;" />
                <th style="background-color: #741bbb; width: 155px; height: 10px; border: none; padding: 0;" />
                <th style="background-color: #319dd9; width: 76px; height: 10px; border: none; padding: 0;" />
                <th style="background-color: #4cbfff; width: 69px; height: 10px; border: none; padding: 0;" />
                <th style="background-color: #3bfe95; width: 49px; height: 10px; border: none; padding: 0;" />
            </tr>
        </table>
        <div style="display: flex; width: 550px; margin-top: -10px; position: relative; z-index: 1;">
            <div style="width: 356px; height: 94px; background: linear-gradient(93.77deg, #8c1ee3 0%, #381b81 51.78%);">
                <img src="https://fsys2-public.s3.us-east-1.amazonaws.com/logo_name_email_branca.png" style="width: 92px; height: 41px; margin-top: 26px; margin-left: 28px;" />
            </div>
            <img
                style="width: 194px; height: 94px;"
                src="https://fsys2-public.s3.us-east-1.amazonaws.com/cv_header_image.png"
            />
        </div>
        <div style="padding: 30px; width: 550px;">
            <p style="font-family: Poppins; font-size: 24px; font-weight: 700; line-height: 24px; text-align: left; text-underline-position: from-font; text-decoration-skip-ink: none; color: #5b1096;">
                Seleção Personalizada de <br />
                Profissionais para Sua Avaliação
            </p>
            <p style="font-family: Poppins; font-size: 14px; font-weight: 400; line-height: 22px; text-align: left; text-underline-position: from-font; text-decoration-skip-ink: none; color: #000000;">
                Que tal conhecer alguns talentos que podem transformar seus<br />
                projetos? Separamos alguns perfis que vão te surpreender!
            </p>
            <span style="font-family: Poppins; font-size: 14px; font-weight: 400; line-height: 22px; text-align: left; text-underline-position: from-font; text-decoration-skip-ink: none; color: #000000;">
                Gostou de algum perfil, quer dar sequencia a este e-mail <br />
                ou esclarecer dúvidas? <a href="mailto:${Email_Usuario_Logado}" target="_blank" style="color: #ff571d; cursor: pointer; text-decoration: none;">Clique Aqui</a> e fale direto com <br />
                a, ${NOME_E_ORG}
            </span>
            <p style="margin-top: 40px; font-family: Inter; font-size: 18px; font-weight: 500; line-height: 28px; text-align: left; text-underline-position: from-font; text-decoration-skip-ink: none; color: #1c1c1f;">
                Lista de colaboradores
            </p>
            <table style="width: 490px; border-collapse: collapse; box-shadow: 0px 4px 8px -2px #1018281a; border-radius: 8px; overflow: hidden;">
                <thead>
                    <tr>
                        <th
                            style="
                                padding: 12px 16px;
                                text-align: left;
                                background-color: #f9fafb;
                                border-bottom: 1px solid #eaecf0;
                                color: #667085;
                                font-family: Inter;
                                font-size: 12px;
                                font-weight: 500;
                                line-height: 18px;
                                text-align: left;
                                text-underline-position: from-font;
                                text-decoration-skip-ink: none;
                            "
                        >
                            Nome
                        </th>
                        <th
                            style="
                                padding: 12px 16px;
                                text-align: left;
                                background-color: #f9fafb;
                                border-bottom: 1px solid #eaecf0;
                                color: #667085;
                                font-family: Inter;
                                font-size: 12px;
                                font-weight: 500;
                                line-height: 18px;
                                text-align: left;
                                text-underline-position: from-font;
                                text-decoration-skip-ink: none;
                            "
                        >
                            Habilidades
                        </th>
                        <th
                            style="
                                padding: 12px 16px;
                                text-align: left;
                                background-color: #f9fafb;
                                border-bottom: 1px solid #eaecf0;
                                color: #667085;
                                font-family: Inter;
                                font-size: 12px;
                                font-weight: 500;
                                line-height: 18px;
                                text-align: left;
                                text-underline-position: from-font;
                                text-decoration-skip-ink: none;
                            "
                        >
                            Ação
                        </th>
                    </tr>
                </thead>
                <tbody>
                    ${ITENS_TABELA}
                </tbody>
            </table>
            <div style="display: flex; width: 490px; height: 32px; margin-top: 20px; margin-bottom: 30px; gap: 10px; border-radius: 8px; align-items: center; opacity: 0px; background: #dde7ed;">
                <svg style="margin-left: 10px;" width="17" height="18" viewBox="0 0 17 18" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <g clip-path="url(#clip0_2511_11455)">
                        <path
                            d="M8.5013 11.8346V9.0013M8.5013 6.16797H8.50839M15.5846 9.0013C15.5846 12.9133 12.4133 16.0846 8.5013 16.0846C4.58928 16.0846 1.41797 12.9133 1.41797 9.0013C1.41797 5.08928 4.58928 1.91797 8.5013 1.91797C12.4133 1.91797 15.5846 5.08928 15.5846 9.0013Z"
                            stroke="#8C9199"
                            stroke-width="2"
                            stroke-linecap="round"
                            stroke-linejoin="round"
                        />
                    </g>
                    <defs>
                        <clipPath id="clip0_2511_11455">
                            <rect width="17" height="17" fill="white" transform="translate(0 0.5)" />
                        </clipPath>
                    </defs>
                </svg>
                <p
                    style="
                        width: 443px;
                        font-family: Lato;
                        font-size: 12px;
                        font-weight: 500;
                        line-height: 16px;
                        letter-spacing: 0.30000001192092896px;
                        text-align: left;
                        text-underline-position: from-font;
                        text-decoration-skip-ink: none;
                        color: #626468;
                    "
                >
                    Os links gerados estarão disponíveis por 30 dias a partir da data de sua criação.
                </p>
            </div>
            <span style="font-family: Poppins; font-size: 14px; font-weight: 400; line-height: 22px; text-align: left; text-underline-position: from-font; text-decoration-skip-ink: none; color: #000000; width: 490px;">
                Problemas com acesso ou visualização? <br />
                Entre em contato com nosso time de suporte por <a href="mailto:ajuda@fourmakers.io" target="_blank" style="color: #ff571d; cursor: pointer; text-decoration: none;">aqui.</a>
            </span>
            <p style="font-family: Inter; font-size: 12px; font-weight: 400; line-height: 16px; text-align: left; text-underline-position: from-font; text-decoration-skip-ink: none; color: #babdc2; width: 490px;">
                Esta mensagem é destinada exclusivamente à(s) pessoa(s) a quem é dirigida, podendo conter informação confidencial e/ou legalmente privilegiada, cujo uso, distribuição ou cópia não autorizados são estritamente proibidos,
                estando legalmente sujeitos às penalidades cabíveis. Se você recebeu esta mensagem indevidamente, por favor, informe-nos imediatamente e proceda à sua exclusão, promovendo a eliminação do seu conteúdo em suas bases de
                dados, registros ou sistemas de controle.
            </p>
        </div>
        <div style="display: flex; justify-content: space-between; height: 77px; width: 550px; margin-bottom: -10px; position: relative; z-index: 1; background: linear-gradient(90.02deg, #4cbfff 0.02%, #8c1ee3 65.18%);">
            <p
                style="
                    font-family: Poppins;
                    font-size: 16px;
                    font-weight: 600;
                    line-height: 24px;
                    text-align: left;
                    text-underline-position: from-font;
                    text-decoration-skip-ink: none;
                    color: #ffffff;
                    margin-left: 60px;
                    margin-top: 24px;
                "
            >
                Unindo forças para criar o futuro
            </p>
            <img src="https://fsys2-public.s3.us-east-1.amazonaws.com/cv_footer_image_new.png" />
        </div>
        <table style="width: 550px; border-collapse: collapse; position: relative; z-index: 2;">
            <tr>
                <th style="background-color: #5b1096; width: 34px; height: 10px; border: none; padding: 0;" />
                <th style="background-color: #741bbb; width: 155px; height: 10px; border: none; padding: 0;" />
                <th style="background-color: #319dd9; width: 76px; height: 10px; border: none; padding: 0;" />
                <th style="background-color: #4cbfff; width: 69px; height: 10px; border: none; padding: 0;" />
                <th style="background-color: #3bfe95; width: 49px; height: 10px; border: none; padding: 0;" />
                <th />
            </tr>
        </table>
    </div>
</body>
</html>
',
'pt-BR');
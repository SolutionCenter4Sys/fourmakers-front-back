UPDATE tb_template_email
SET template = '
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html dir="ltr" lang="es">
<head>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
</head>
<body>
    <div style="width: 550px; position: relative;">
        <img style="display: flex; width: 550px; margin-top: -10px; position: relative; z-index: 1;" src="https://media-hosting.imagekit.io//a7b88c5b15cf4573/screenshot_1738332662196.png?Expires=1832940664&Key-Pair-Id=K2ZIVPTIP2VGHC&Signature=0tfXp0z5SKsovxnLx8K4zS-xlKG7ujV418xK3Vy32QGa0~YQKE57OMLSCuDQW5B8MT5uxojPZ0kqswN0pr3Tb-04xbhWkutZ9rBICznQgoDU8txkjHlo64Ux8389lMhSpB0R17pL7ZVVGqMKFq~~JuV019nypFmm171Gm7IVydZlXoOz4O6oQqTKT0~89lBjzP87gC0M0YW-cnRJTpf7GmlYCN6fjtabtsQexgfjIWAToOANLzo327wWOG1cBDDKM2QiL4KVernnprSpw0iE45pxJQ-9arRjBG7SfMmwwRTcS3tLflnIlsHlNFEDm~Sbo8wpXKNsevpAATNHX~xqNQ__"/>
          
        <div style="padding: 30px; width: 550px;">
            <p style="font-family: Arial, sans-serif; font-size: 24px; font-weight: 700; text-align: left;color: #5b1096;">
                Seleção Personalizada de <br />
                Profissionais para Sua Avaliação
            </p>
            <p style="font-family: Arial, sans-serif; font-size: 14px; font-weight: 400; text-align: left; color: #000000;">
                Que tal conhecer alguns talentos que podem transformar seus<br />
                projetos? Separamos alguns perfis que vão te surpreender!
            </p>
            <span style="font-family: Arial, sans-serif; font-size: 14px; font-weight: 400; text-align: left; color: #000000;">
                Gostou de algum perfil, quer dar sequencia a este e-mail <br />
                ou esclarecer dúvidas? <a href="mailto:${Email_Usuario_Logado}" target="_blank" style="color: #ff571d; cursor: pointer; text-decoration: none;">Clique Aqui</a> e fale direto com <br />
                a, ${NOME_E_ORG}
            </span>
            <p style="margin-top: 40px; font-family: Arial, sans-serif; font-size: 18px; font-weight: 500; text-align: left; color: #1c1c1f;">
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
                                font-family: Arial, sans-serif;
                                font-size: 12px;
                                font-weight: 500;
                                text-align: left;
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
                                font-family: Arial, sans-serif;
                                font-size: 12px;
                                font-weight: 500;
                                text-align: left;
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
                                font-family: Arial, sans-serif;
                                font-size: 12px;
                                font-weight: 500;
                                text-align: left;
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
                <img style="width: 17px; height: 17px; margin-left: 10px;" src="https://media-hosting.imagekit.io//9201e6707ee24b56/image%20(2).png?Expires=1832940854&Key-Pair-Id=K2ZIVPTIP2VGHC&Signature=OsdDxNCa2XYlx4uwZr3m4MLbwboFhKuEY5frAA3NU6IVIo8E4~G37pfkvWM02WKGupe2abpgH~TuVVraWqsbdjgQfSUgyeoV9SHWL~EVX78tHfMexUd7Snft2NCsRozfmGcfVradsLFXqojEPY1zGLnOh~YtgWvnGUyp~FwpeEDqH5kYuzFr7jfk7Juh6ZolmJ~xEs~XV1dVy3CIHRcIUdbYkk6fBLB3vx~yjU4fPsD3XY4MYYxF~txyZ6S40Hr6uhKY~mYknHPfeKVM7uNvYK~tOnA6W~8VWhGYx0ZzAlDIKLzXPhmOC4POYDPhmDhxgr8Gk5Qv6L52Ab83dhQgGA__">
                <p
                    style="
                        width: 443px;
                        font-family: Arial, sans-serif;
                        font-size: 12px;
                        font-weight: 500;
                        letter-spacing: 0.3px;
                        text-align: left;
                        color: #626468;
                    "
                >
                    Os links gerados estarão disponíveis por 30 dias a partir da data de sua criação.
                </p>
            </div>
            <span style="font-family: Arial, sans-serif; font-size: 14px; font-weight: 400; text-align: left; color: #000000; width: 490px;">
                Problemas com acesso ou visualização? <br />
                Entre em contato com nosso time de suporte por <a href="mailto:ajuda@fourmakers.io" target="_blank" style="color: #ff571d; cursor: pointer; text-decoration: none;">aqui.</a>
            </span>
            <p style="font-family: Arial, sans-serif; font-size: 12px; font-weight: 400; text-align: left; color: #babdc2; width: 490px;">
                Esta mensagem é destinada exclusivamente à(s) pessoa(s) a quem é dirigida, podendo conter informação confidencial e/ou legalmente privilegiada, cujo uso, distribuição ou cópia não autorizados são estritamente proibidos,
                estando legalmente sujeitos às penalidades cabíveis. Se você recebeu esta mensagem indevidamente, por favor, informe-nos imediatamente e proceda à sua exclusão, promovendo a eliminação do seu conteúdo em suas bases de
                dados, registros ou sistemas de controle.
            </p>
        </div>
        <img style="display: flex; justify-content: space-between; height: 77px; width: 550px; margin-bottom: -10px; position: relative; z-index: 1;" src="https://media-hosting.imagekit.io//1aa55eb26d9d4df5/image%20(1).png?Expires=1832940742&Key-Pair-Id=K2ZIVPTIP2VGHC&Signature=TSolQr7vBkSlRwFStRju-a6cJ0INmocROACgkoNKUo16rSPHlqxfQ2~5ywwjVtbBA3uNJER~Ftun9dnCf9WtCZAaV4aavIa98NSnkqexEelAFj1WfsXYRT3HkghBRckGhG78-7Ztv05Q~5js2NFZ5Aw7FW25xYFE7wqVdqT4SwNl7XXabGNFeJAeafgCik6PLfmpn2~rK97jY3NrhciVbGLosrRD8YUja4PSpLtLE7Fs8bSkLgpk6~W00C~IPvcGtCaQjwStCRW9WUKL1AZKvpgI92tRuD~hoKllmKZnQBzO36O07JOLH2EDq47L1io8MKsS~pqyrRuHGt~WDL4KrQ__"/>
    </div>
</body>
</html>
'
WHERE codigo = "CV_EM_MASSA";
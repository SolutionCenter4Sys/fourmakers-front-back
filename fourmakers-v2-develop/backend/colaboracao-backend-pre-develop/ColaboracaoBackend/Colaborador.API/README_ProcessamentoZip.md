# Endpoint para Processamento de Arquivos ZIP com PDFs

## Descrição
O endpoint `ProcessarArquivoZipComPdfs` permite enviar um arquivo ZIP contendo múltiplos arquivos PDF e extrair o texto de cada um deles.

## Endpoint
```
POST /api/CurriculoColaborador/ProcessarArquivoZipComPdfs
```

## Autenticação
O endpoint requer autenticação (token JWT).

## Parâmetros
- **File** (IFormFile): Arquivo ZIP contendo os PDFs

## Validações
- O arquivo deve ser um arquivo ZIP válido
- Apenas arquivos com extensão `.pdf` serão processados
- Arquivos que não são PDFs serão ignorados e reportados como erro

## Resposta de Sucesso (200)
```json
{
  "sucesso": true,
  "mensagem": "Processamento concluído. 3 arquivos processados com sucesso, 1 com erro.",
  "retorno": {
    "pdfsProcessados": [
      {
        "nomeArquivo": "curriculo1.pdf",
        "conteudoExtraido": "Texto extraído do PDF...",
        "processadoComSucesso": true,
        "erro": null
      },
      {
        "nomeArquivo": "documento.pdf",
        "conteudoExtraido": null,
        "processadoComSucesso": false,
        "erro": "Arquivo não é um PDF válido."
      }
    ],
    "totalArquivos": 4,
    "arquivosProcessadosComSucesso": 3,
    "arquivosComErro": 1
  }
}
```

## Resposta de Erro (400)
```json
{
  "sucesso": false,
  "mensagem": "O arquivo enviado não é um arquivo ZIP válido."
}
```

## Resposta de Erro (500)
```json
{
  "sucesso": false,
  "mensagem": "Erro interno no servidor: Detalhes do erro..."
}
```

## Exemplo de Uso com cURL
```bash
curl -X POST \
  -H "Authorization: Bearer SEU_TOKEN_JWT" \
  -F "File=@arquivo.zip" \
  http://localhost:5000/api/CurriculoColaborador/ProcessarArquivoZipComPdfs
```

## Exemplo de Uso com JavaScript/Fetch
```javascript
const formData = new FormData();
formData.append('File', arquivoZip);

const response = await fetch('/api/CurriculoColaborador/ProcessarArquivoZipComPdfs', {
  method: 'POST',
  headers: {
    'Authorization': 'Bearer ' + token
  },
  body: formData
});

const result = await response.json();
console.log(result);
```

## Limitações
- O endpoint processa apenas arquivos PDF dentro do ZIP
- Arquivos muito grandes podem causar timeout
- O conteúdo extraído é retornado como texto simples
- Não há limite específico de arquivos, mas arquivos muito grandes podem causar problemas de memória

## Tecnologias Utilizadas
- `System.IO.Compression.ZipArchive` para extração do ZIP
- `PdfUtil.ToString()` para extração de texto dos PDFs
- `UglyToad.PdfPig` para processamento dos PDFs 
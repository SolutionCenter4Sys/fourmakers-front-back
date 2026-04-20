#!/bin/bash

# Script para limpar completamente o S3 e fazer novo deploy com Content-Types corretos
# Uso: ./redeploy-s3-clean.sh <nome-do-bucket> <cloudfront-distribution-id>

set -e

if [ -z "$1" ] || [ -z "$2" ]; then
  echo "❌ Erro: Parâmetros obrigatórios ausentes"
  echo "Uso: ./redeploy-s3-clean.sh <nome-do-bucket> <cloudfront-distribution-id>"
  echo ""
  echo "Exemplo:"
  echo "  ./redeploy-s3-clean.sh meu-bucket E123ABCD456XYZ"
  exit 1
fi

S3_BUCKET=$1
CLOUDFRONT_ID=$2

echo "╔════════════════════════════════════════════════════════════╗"
echo "║  🧹 LIMPEZA E REDEPLOY COMPLETO - AWS S3 + CloudFront     ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo ""
echo "📦 Bucket S3: $S3_BUCKET"
echo "☁️  CloudFront: $CLOUDFRONT_ID"
echo ""

# Verificar se o diretório dist existe
if [ ! -d "dist" ]; then
  echo "❌ Erro: Diretório 'dist' não encontrado!"
  echo "Execute 'npm run build' antes de executar este script."
  exit 1
fi

# Confirmação
echo "⚠️  ATENÇÃO: Este script irá:"
echo "   1. Deletar TODOS os arquivos do bucket S3"
echo "   2. Fazer upload de todos os arquivos da pasta dist/"
echo "   3. Criar invalidação no CloudFront"
echo ""
read -p "🤔 Deseja continuar? (sim/não): " confirm

if [ "$confirm" != "sim" ]; then
  echo "❌ Operação cancelada pelo usuário."
  exit 0
fi

echo ""
echo "════════════════════════════════════════════════════════════"
echo "🗑️  ETAPA 1/4: Deletando todos os arquivos do S3..."
echo "════════════════════════════════════════════════════════════"
echo ""

aws s3 rm s3://${S3_BUCKET}/ --recursive

echo "✅ Bucket limpo com sucesso!"
echo ""

echo "════════════════════════════════════════════════════════════"
echo "📤 ETAPA 2/4: Fazendo upload dos arquivos com Content-Types corretos..."
echo "════════════════════════════════════════════════════════════"
echo ""

# HTML
echo "📄 Enviando arquivos HTML..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.html" \
  --content-type "text/html; charset=utf-8" \
  --cache-control "public, max-age=0, must-revalidate" \
  --no-progress

# JavaScript
echo "📜 Enviando arquivos JavaScript..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.js" \
  --include "*.mjs" \
  --content-type "application/javascript; charset=utf-8" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# CSS
echo "🎨 Enviando arquivos CSS..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.css" \
  --content-type "text/css; charset=utf-8" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# SVG
echo "🖼️  Enviando arquivos SVG..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.svg" \
  --content-type "image/svg+xml" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# PNG
echo "🖼️  Enviando arquivos PNG..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.png" \
  --content-type "image/png" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# JPEG
echo "🖼️  Enviando arquivos JPEG..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.jpg" \
  --include "*.jpeg" \
  --content-type "image/jpeg" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# GIF
echo "🖼️  Enviando arquivos GIF..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.gif" \
  --content-type "image/gif" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# WEBP
echo "🖼️  Enviando arquivos WEBP..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.webp" \
  --content-type "image/webp" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# ICO
echo "🖼️  Enviando arquivos ICO..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.ico" \
  --content-type "image/x-icon" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# WOFF
echo "🔤 Enviando fontes WOFF..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.woff" \
  --content-type "font/woff" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# WOFF2
echo "🔤 Enviando fontes WOFF2..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.woff2" \
  --content-type "font/woff2" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# TTF
echo "🔤 Enviando fontes TTF..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.ttf" \
  --content-type "font/ttf" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# EOT
echo "🔤 Enviando fontes EOT..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.eot" \
  --content-type "application/vnd.ms-fontobject" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# JSON
echo "📋 Enviando arquivos JSON..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.json" \
  --content-type "application/json; charset=utf-8" \
  --cache-control "public, max-age=0, must-revalidate" \
  --no-progress

# MAP (sourcemaps)
echo "🗺️  Enviando arquivos MAP..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*" \
  --include "*.map" \
  --content-type "application/json; charset=utf-8" \
  --cache-control "public, max-age=31536000, immutable" \
  --no-progress

# Outros arquivos (se houver)
echo "📦 Enviando outros arquivos..."
aws s3 sync dist/ s3://${S3_BUCKET}/ \
  --exclude "*.html" \
  --exclude "*.js" \
  --exclude "*.mjs" \
  --exclude "*.css" \
  --exclude "*.svg" \
  --exclude "*.png" \
  --exclude "*.jpg" \
  --exclude "*.jpeg" \
  --exclude "*.gif" \
  --exclude "*.webp" \
  --exclude "*.ico" \
  --exclude "*.woff" \
  --exclude "*.woff2" \
  --exclude "*.ttf" \
  --exclude "*.eot" \
  --exclude "*.json" \
  --exclude "*.map" \
  --no-progress

echo ""
echo "✅ Upload concluído com sucesso!"
echo ""

echo "════════════════════════════════════════════════════════════"
echo "🔍 ETAPA 3/4: Verificando Content-Types dos arquivos JS..."
echo "════════════════════════════════════════════════════════════"
echo ""

# Listar e verificar alguns arquivos JS
JS_FILES=$(aws s3 ls s3://${S3_BUCKET}/assets/ --recursive | grep -E '\.js$' | head -3 | awk '{print $4}')

if [ -z "$JS_FILES" ]; then
  echo "⚠️  Nenhum arquivo JS encontrado na pasta assets/"
else
  for file in $JS_FILES; do
    echo "📄 Verificando: $file"
    CONTENT_TYPE=$(aws s3api head-object --bucket ${S3_BUCKET} --key "$file" --query 'ContentType' --output text)
    echo "   Content-Type: $CONTENT_TYPE"
    
    if [[ "$CONTENT_TYPE" == "application/javascript"* ]]; then
      echo "   ✅ Content-Type correto!"
    else
      echo "   ❌ Content-Type incorreto!"
    fi
    echo ""
  done
fi

echo "════════════════════════════════════════════════════════════"
echo "🔄 ETAPA 4/4: Invalidando cache do CloudFront..."
echo "════════════════════════════════════════════════════════════"
echo ""

INVALIDATION_ID=$(aws cloudfront create-invalidation \
  --distribution-id ${CLOUDFRONT_ID} \
  --paths "/*" \
  --query 'Invalidation.Id' \
  --output text)

echo "✅ Invalidação criada com sucesso!"
echo "   ID: $INVALIDATION_ID"
echo ""
echo "⏳ Aguardando conclusão da invalidação..."
echo "   (isso pode levar 3-5 minutos)"
echo ""

aws cloudfront wait invalidation-completed \
  --distribution-id ${CLOUDFRONT_ID} \
  --id $INVALIDATION_ID && echo "✅ Invalidação concluída!" || echo "⚠️ Invalidação em andamento..."

echo ""
echo "╔════════════════════════════════════════════════════════════╗"
echo "║  🎉 PROCESSO CONCLUÍDO COM SUCESSO!                       ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo ""
echo "📝 Próximos passos:"
echo "   1. Aguarde 1-2 minutos para propagação do CloudFront"
echo "   2. Limpe o cache do navegador (Ctrl+Shift+Delete)"
echo "   3. Acesse o site em modo anônimo/privado"
echo "   4. Verifique se o erro desapareceu"
echo ""
echo "🔗 URL CloudFront: https://${CLOUDFRONT_ID}.cloudfront.net"
echo ""

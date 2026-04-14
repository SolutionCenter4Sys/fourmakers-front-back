#!/bin/bash

set -e

echo "================ DEV ================="

echo "Conteúdo do setenv antes (DEV):"
grep '^export AWS_ENVIRONMENT=' setenv || true

echo "Carregando . setenv dev..."
set +e
. ./setenv dev
set -e

echo "Conteúdo do setenv depois (DEV):"
echo "AWS_PROFILE=$AWS_PROFILE"

echo "Rodando develop..."
python3 ./build-template.py develop

echo "================ PRD ================="

echo "Conteúdo do setenv antes (PRD):"
grep '^export AWS_ENVIRONMENT=' setenv || true

echo "Carregando . setenv prd..."
set +e
. ./setenv prd
set -e

echo "Conteúdo do setenv depois (PRD):"
echo "AWS_PROFILE=$AWS_PROFILE"

echo "Rodando main..."
python3 ./build-template.py main

echo "Fim."
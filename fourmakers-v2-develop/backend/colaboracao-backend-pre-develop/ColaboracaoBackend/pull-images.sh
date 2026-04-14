#!/bin/bash
REGISTRY_BASE=sistemasinternos.foursys.com.br:5000/
TAG=homolog

green=`tput setaf 2`
reset=`tput sgr0`

pull_image()
{
  IMAGE=$1

  echo "${green}Realizando o pull da imagem $IMAGE ${reset}"
  docker pull "$REGISTRY_BASE$IMAGE":"$TAG"
  echo "\n"
}

echo "${green}Iniciando pull de images no registry: $REGISTRY_BASE ${reset}"

echo "${green}Realizando o pull da imagens do colaboracao-backend \n\n\n ${reset}"

pull_image apontamento-api
pull_image competencia-api
pull_image bi-api
pull_image uploadfiles-api
pull_image colaborador-api
pull_image usuario-api
pull_image foursys-api
pull_image srs-api
pull_image vagassrs-api
pull_image crm-api
pull_image projeto-api
pull_image mapaalocacao-api
pull_image mockserver-api
pull_image rotinasbackoffice-api
pull_image colaboracaobridge-api


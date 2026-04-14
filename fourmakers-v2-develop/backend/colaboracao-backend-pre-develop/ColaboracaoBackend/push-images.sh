#!/bin/bash
REGISTRY_BASE=sistemasinternos.foursys.com.br:5000/
TAG=homolog

green=`tput setaf 2`
reset=`tput sgr0`

push_image()
{
  IMAGE=$1

  echo "${green}Realizando o push da imagem $IMAGE ${reset}"
  docker push "$REGISTRY_BASE$IMAGE":"$TAG"
  echo "\n"
}

echo "${green}Iniciando push de images no registry: $REGISTRY_BASE ${reset}"

echo "${green}Realizando o push das imagens do colaboracao-backend \n\n\n ${reset}"
push_image apontamento-api
push_image competencia-api
push_image bi-api
push_image uploadfiles-api
push_image colaborador-api
push_image usuario-api
push_image foursys-api
push_image srs-api
push_image vagassrs-api
push_image crm-api
push_image projeto-api
push_image mapaalocacao-api
push_image mockserver-api
push_image rotinasbackoffice-api
push_image colaboracaobridge-api


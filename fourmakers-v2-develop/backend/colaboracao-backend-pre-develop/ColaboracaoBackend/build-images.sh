#!/bin/bash

docker build -f Apontamento.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/apontamento-api:homolog .
docker build -f BI.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/bi-api:homolog .
docker build -f Competencia.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/competencia-api:homolog .
docker build -f UploadFiles.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/uploadfiles-api:homolog .
docker build -f Formacao.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/formacao-api:homolog .
docker build -f Colaborador.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/colaborador-api:homolog .
docker build -f Usuario.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/usuario-api:homolog .
docker build -f Foursys.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/foursys-api:homolog .
docker build -f SRS.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/srs-api:homolog .
docker build -f Vaga.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/vagassrs-api:homolog .
docker build -f CRM.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/crm-api:homolog .
docker build -f Projeto.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/projeto-api:homolog .
docker build -f MapaDeAlocacao.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/mapaalocacao-api:homolog .
docker build -f MockServer.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/mockserver-api:homolog .
docker build -f RotinasBackoffice.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/rotinasbackoffice-api:homolog .
docker build -f ColaboracaoBridge.API/Dockerfile -t sistemasinternos.foursys.com.br:5000/colaboracaobridge-api:homolog .
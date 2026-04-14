using DataTransferObject.Domain;
using DataTransferObject.Domain.BI;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Dominio;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.Metodologia;
using DataTransferObject.Domain.Softskill;
using System;
using System.Collections.Generic;

namespace Core.Domain.BI
{
    public interface IColaboradorBIRepository
    {
        List<ColaboradorBIDTO> GetAllColaboradoresBI();
        List<KeyValuePair<string, ListaOrgColaboradorDTO>> GetOrgInfoBI();
        List<KeyValuePair<string, CompetenciaColaboradorDTO>> GetCompetenciaColaboradorBI();
        List<KeyValuePair<Tuple<string, long>, CertificadoDTO>> GetCertificadoCompetenciaColaboradorBI();
        List<KeyValuePair<string, SoftskillColaboradorDTO>> GetSoftskillColaboradorBI();
        List<KeyValuePair<string, ListaMetodologiaColaboradorResult>> GetMetodologiaColaboradorBI();
        List<KeyValuePair<string, DominioColaboradorDTO>> GetDominiColaboradorBI();
        List<KeyValuePair<string, IdiomaColaboradorDTO>> GetIdiomaColaboradorBI();
    }
}
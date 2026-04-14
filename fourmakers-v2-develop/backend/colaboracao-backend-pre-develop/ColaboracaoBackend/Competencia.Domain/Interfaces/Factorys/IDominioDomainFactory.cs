using Competencia.Domain.Interfaces.Models;
using System;

namespace Competencia.Domain.Interfaces.Factorys
{
    public interface IDominioDomainFactory
    {
        IDominioModel buildDominioModel();
        IDominioModel buildDominioModel(long id, string descricao, long usuarioCriacaoId);
        IDominioModel buildDominioModel(long id, string descricao, long usuarioCriacaoId, bool pendente);
        IDominioColaboradorModel buildDominioColaboradorModel();
        IDominioColaboradorModel buildDominioColaboradorModel(long idDominioColaborador, long idDominio, string colaboradorCpf, long? idNivel, DateTime dataAlteracao, bool pendente);
        IDominioColaboradorModel buildDominioColaboradorModel(long idDominioColaborador, long idDominio, string colaboradorCpf, long? idNivel, DateTime dataAlteracao);
        IDominioColaboradorModel buildDominioColaboradorModel(long idDominioColaborador, long idDominio, string colaboradorCpf, DateTime dataAlteracao, bool pendente);
        IDominioColaboradorModel buildDominioColaboradorModel(long idDominioColaborador, long idDominio, string colaboradorCpf, DateTime dataAlteracao);
        IDominioNivelModel buildDominioNivelModel();
        IDominioNivelModel buildDominioNivelModel(long? nivelId);
        IDominioNivelModel buildDominioNivelModel(long nivelId, string nivelDescricao);
        IDominioEndossoModel buildDominioEndossoModel(long idDominioColaborador);
        IDominioEndossoColaboradorModel buildDominioEndossoColaboradorModel(long idDominioColaborador);
        IDominioEndossoColaboradorModel buildDominioEndossoColaboradorModel(string cpfColaborador, DateTime dataEndosso, int? idTipoEndosso);
        IDominioTipoEndossoModel buildDominioTipoEndossoModel(int? tipoEndossoId);
    }
}
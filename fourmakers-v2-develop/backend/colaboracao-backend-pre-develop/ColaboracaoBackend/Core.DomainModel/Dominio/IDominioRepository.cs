using DataTransferObject.Domain.Dominio;
using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;

namespace Core.Domain.Dominio
{
    public interface IDominioRepository
    {
        List<long> ListarDominiosAtribuidos(string cpfColaborador);
        List<DominioDTO> ListarDominio(string busca, int cursor, int limite);
        List<DominioDTO> ListarDominioPorDescricao(string descricao);
        List<DominioColaboradorDTO> ListarDominioColaborador(string cpf);
        DominioColaboradorDTO ObterDominioColaborador(string cpf, long dominioId);
        DominioDTO ObterDominioPorId(long id);
        DominioDTO InserirDominio(string descricao, long usuarioId);
        long? GetUsuarioCriacaoId(string cpf);
        DominioColaboradorDTO AssociarDominioColaborador(long dominioId, long? nivelId, string cpf);
        DominioColaboradorDTO AlterarDominioColaborador(long dominioId, long? nivelId, string cpf);
        bool RemoverDominioColaborador(long dominioId, string cpf);
        List<NivelDTO> ListaNivelDominio();
        StatusEndossoDTO GetStatusEndosso(long idColaboradorDominio);
        List<EndossoRawDTO> GetEndossoConcedido(long idColaboradorDominio);
        TipoEndossoDTO GetTipoEndosso(long tipoEndossoId);
        NivelDTO GetNivelById(long nivelId);
    }

    public class EndossoRawDTO
    {
        public string cpfColaborador { get; set; }
        public DateTime dataEndosso { get; set; }
        public long TipoEndossoId { get; set; }
    }
}
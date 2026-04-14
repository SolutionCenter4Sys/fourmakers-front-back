using DataTransferObject.Domain.DadosPerfil;
using DataTransferObject.Domain.DadosPerfil.GrauParentesco;
using DataTransferObject.Domain.DadosPerfil.TipoContratacao;
using System.Collections.Generic;

namespace Core.Domain.DadosPerfil
{
    public interface IDadosPerfilRepository
    {
        List<DisponibilidadeDTO> ListarDisponibilidade(string busca, int cursor, int limite);

        List<FonteOrigemDTO> ListarFonteOrigem(string busca, int cursor, int limite);

        List<TipoDeCargoDTO> ListarTipoDeCargo(string busca, int cursor, int limite);

        List<TipoCargaHorariaDTO> ListarTipoCargaHoraria(string busca, int cursor, int limite);

        List<ModalidadeContratacaoDTO> ListarModalidadeContratacao(string busca, int cursor, int limite);

        List<EstadoCivilDTO> ListarEstadoCivil(string busca, int cursor, int limite);

        List<EscolaridadeDTO> ListarEscolaridade(string busca, int cursor, int limite);

        List<OrientacaoSexualDTO> ListarOrientacaoSexual(string busca, int cursor, int limite);

        List<IdentidadeDeGeneroDTO> ListarIdentidadeDeGenero(string busca, int cursor, int limite);

        List<GrauParentescoDTO> ListarGrauParentesco(string busca, int cursor, int limite);

        List<ListarTipoContratacaoDTO> ListarTipoContratacao(string busca, int cursor, int limite);

        List<EtniaDTO> ListarEtnia(string busca, int cursor, int limite);
    }
}
using DataTransferObject.Domain.Cep;
using DataTransferObject.Domain.DadosPerfil;
using DataTransferObject.Domain.DadosPerfil.GrauParentesco;
using DataTransferObject.Domain.DadosPerfil.TipoContratacao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foursys.Domain.Interfaces.Services
{
    public interface IDadosPerfilService
    {
        List<DisponibilidadeDTO> ListarDisponibilidade(string busca, int curso, int limite);

        List<FonteOrigemDTO> ListarFonteOrigem(string busca, int curso, int limite);

        List<TipoDeCargoDTO> ListarTipoDeCargo(string busca, int cursor, int limite);

        List<TipoCargaHorariaDTO> ListarTipoCargaHoraria(string busca, int cursor, int limite);

        List<ModalidadeContratacaoDTO> ListarModalidadeContratacao(string busca, int cursor, int limite);

        List<EstadoCivilDTO> ListarEstadoCivil(string busca, int cursor, int limite);

        List<EscolaridadeDTO> ListarEscolaridade(string busca, int cursor, int limite);

        List<OrientacaoSexualDTO> ListarOrientacaoSexual(string busca, int curso, int limite);

        List<IdentidadeDeGeneroDTO> ListarIdentidadeDeGenero(string busca, int cursor, int limite);

        Task<ConsultaCepDTO> ConsultarCep(string cep);

        Task<TipoContratacaoDTO> BuscarTipoContratacao(string emailColaborador);

        List<GrauParentescoDTO> ListarGrauParentesco(string busca, int cursor, int limite);

        List<ListarTipoContratacaoDTO> ListarTipoContratacao(string busca, int cursor, int limite);

        List<EtniaDTO> ListarEtnia(string busca, int cursor, int limite);
    }
}
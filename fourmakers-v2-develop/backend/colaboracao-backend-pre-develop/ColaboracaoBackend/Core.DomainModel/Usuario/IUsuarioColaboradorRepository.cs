using DataTransferObject.Domain.Carga;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.Org;
using DataTransferObject.Domain.Usuario;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Foursys;

namespace Core.Domain.Usuario
{
    public interface IUsuarioColaboradorRepository
    {
        void UpsertUsuarioColaborador(UsuarioColaboradorDTO usuario, ColaboradorDTO colaborador, EnderecoDTO endereco);
        void InsertUsuarioColaborador(UsuarioColaboradorDTO usuario, ColaboradorDTO colaborador, EnderecoDTO endereco);
        void InsertColaboradorSemUsuario(ColaboradorDTO colaborador);
        void UpsertColaboradorOrg(ColaboradorDTO colaborador, ColaboradorOrgDTO colaboradorOrg);
        void InserePrestadorServicoPessoaJuridica(string cnpj, PrestadorServicoDTO prestadorServico, RegimeTributarioDTO regimeTributario, string cpf);
        void AlterarPrestadorServicoPessoaJuridica(string cnpj, PrestadorServicoDTO prestadorServico, RegimeTributarioDTO regimeTributario, string cpf);
        bool ExisteColaborador(string cpf);
        bool ExisteColaboradorComCPF(string cpf, int orgId);
        bool ExisteColaboradorComEmail(string email, int orgId);
        bool ExisteColaboradorComEmailAlternativo(string email, int orgId);
        bool ExisteColaboradorComCodColaboradorExterno(string codColaboradorExt, int orgId);
        bool ExisteColaboradorComCodColaboradorExternoECpf(string codColaboradorExt, string cpf, int orgId);
        bool SeTokenSistema(string token);
        UsuarioColaboradorDTO GetUserByCPFEOrgId(string cpf, int orgId, int? userId = null);
        void DeletaHierarquiaColaborador(int orgId);
        void DeletaHierarquiaColaboradorPorCodColaborador(string codColaborador, int orgId);
        void InsereHierarquiaColaborador(ColaboradorHierarquiaCargaDTO cargaColaboradorHierarquia, int orgId);
        void InsereHierarquiaColaborador(List<ColaboradorHierarquiaCargaDTO> cargaColaboradorHierarquia, int orgId);
        void DeletaColaboradorProjetoOrg(int orgId, string codColaborador);
        void InsereColaboradorProjetoOrg(List<ColaboradorProjetoCargaDTO> colaboradoresProjetoCarga, int orgId);
        bool CheckColaboradorOrg(string cpf, int orgId);
        bool CheckColaboradorOrgAtivo(string cpf, int orgId);
        void AlteraSenhaUsuario(string cpf, string senha, int orgId);
        string GetColaboradorCpf(string codColaborador, int orgId);
        string GetCodigoInternoByCodExterno(string codExterno, int orgId);
        string GetCodigoInternoByDocumentoEOrg(string documentoColaborador, int orgId);
        void EditarColaborador(CadastroColaboradorInput colaboradorInput, int orgId);
        void AtualizaEmailUsuario(UsuarioColaboradorDTO usuario);
        void AtualizaNomelUsuario(String codInternoColaborador, String nomeCompleto);
        void AtualizaDocumentoUsuario(String codInternoColaborador, String documentoColaborador);
        void AtualizaDadosBasicosUsuario(String codInternoColaborador, DateTime dataNascimento);
        void AtualizaListaColaboradorProjetoOrg(List<ColaboradorProjetoCargaDTO> colaboradoresProjetoCarga, string codColaborador, int orgId);
        Task<bool> AlterarIdiomaPadrao(string idioma, string cpf, int orgId);
        UsuarioColaboradorDTO BuscaColaboradorPorCodigo(string codColaborador, int orgId);
        Task<string> GetCodColaboradorByEmailEOrgId(string email, int orgId);
        Task<List<UsuarioDTO>> BuscarUsuariosOrgPorEmail(string email);
        void InserirUsuarioEColaboradorOrg(UsuarioColaboradorDTO usuario, ColaboradorOrgDTO colaboradorOrg);
        Task<bool> VerificaSeEhGestorHierarquicoDeUmAprovador(int orgId, string codigoInternoGerente);
        bool SalvarDispositivoColaboradorApp(string codigoInternoColaborador, string deviceToken, int orgId);
        Task<int> BuscarQuantidadeDeDependentesPorCodigoInternoColaborador(string codigoInternoColaborador);
        Task<List<ModeloContratacaoDTO>> ListarModelosDeContratacoesPorOrg(int orgId);
        void AtualizarEnderecoColaborador(string codigoInternoColaborador, string cidade, string estado);
        void AtualizarContatoEEmailAlternativo(string codigoInternoColaborador, string contatoPrincipal, string emailAlternativo);
        Task<string> GetNomeColaboradorPorCodigoInterno(string codigoInternoColaborador);
        Task<string> GetEmailColaboradorPorCodigoInterno(string codigoInternoColaborador, int orgId);
        Task<List<CargoColaboradorOrgDTO>> ListarCargosPorOrgIdAsync(int orgId, List<string>? restricaoDiretorias = null);
        Task EditarTelefoneColaborador(string telefone, string ddi, string codigoInternoColaborador);
        Task EditarColaboradorDapperAsync(CadastroColaboradorInput colaboradorInput, int orgId);
    }
}
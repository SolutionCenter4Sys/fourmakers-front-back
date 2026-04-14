using DataTransferObject.Domain.Empresa;
using System.Collections.Generic;

namespace Core.Domain
{
    public interface IEmpresaRepository
    {
        EmpresaDTO Save(EmpresaDTO empresa, long usuarioId);
        void AddUser(string cnpj, long usuarioId, bool pendente, bool responsavelAcesso);
        void RemoveUser(string cnpj, long usuarioId);
        void ConfirmaConvite(string cnpj, long usuarioId, bool confirmado);
        void Update(EmpresaDTO empresa);
        EmpresaDTO GetById(string cnpj);
        long GetUsuarioAcesso(string cnpj);
        void UpdateUsuarioAcesso(string cnpj, long usuarioId);
        List<UsuarioEmpresaDTO> ListaUsuariosEmpresa(string cnpj);
        List<UsuarioCpfIdDTO> ListarUsuarioEmpresaPorId(List<long> idUsuarios);
        public bool VerificaAcessoDados(long idUsuario, string cnpj);
    }
}
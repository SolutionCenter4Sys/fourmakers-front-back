using System;

namespace DataTransferObject.Domain.Fourmakers.ParametroConfiguracao
{
    public class ParametroConfiguracaoRepositoryInput : ParametroConfiguracaoBase
    {
        public ParametroConfiguracaoRepositoryInput()
        { }
        public ParametroConfiguracaoRepositoryInput(ParametroConfiguracaoBase input, Guid id, long usuarioIdAlteracao, int orgId)
        {
            ColaboradorOrgCpf = input.ColaboradorOrgCpf;
            GrupoAcessoId = input.GrupoAcessoId;
            CodigoParametro = input.CodigoParametro.ToUpperInvariant();
            ValorParametro = input.ValorParametro;
            ParametroNivelId = input.ParametroNivelId;
            Id = id;
            OrgId = orgId;
            UsuarioIdAlteracao = usuarioIdAlteracao;

            if (ParametroNivelId == (int)ParametroNivelEnum.org)
            {
                ColaboradorOrgCpf = null;
                GrupoAcessoId = null;
            }
            if (ParametroNivelId == (int)ParametroNivelEnum.grupo_acesso)
            {
                ColaboradorOrgCpf = null;
            }
            if (ParametroNivelId == (int)ParametroNivelEnum.colaborador_org)
            {
                GrupoAcessoId = null;
            }
        }

        public Guid Id { get; set; }
        public int OrgId { get; set; }
        public long UsuarioIdAlteracao { get; }
    }
}
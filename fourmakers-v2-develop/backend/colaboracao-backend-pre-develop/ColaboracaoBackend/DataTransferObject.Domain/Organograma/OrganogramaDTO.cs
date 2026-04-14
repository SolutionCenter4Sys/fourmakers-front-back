using System;
using System.Collections.Generic;
namespace DataTransferObject.Domain
{
    //-------------------- Departamento ---------------------//

    public class OrganogramaDeptoInserirParamDTO
    {
        //public int OrgId { get; set; }
        public string Nome { get; set; }
        public string CodigoCliente { get; set; }
        public string OrganogramaPosicaoIdLider { get; set; }
        public bool Ativo { get; set; }
        //public DateTime? DataCriacao { get; set; }
    }
    public class OrganogramaDeptoAtualizarParamDTO
    {
        public string Id { get; set; }
        //public int OrgId { get; set; }
        public string Nome { get; set; }
        public string CodigoCliente { get; set; }
        public string OrganogramaPosicaoIdLider { get; set; }
        public bool Ativo { get; set; }

    }

    //-------------------- Response ---------------------//

    public class OrganogramaDeptoListarPorIdResponseDTO
    {
        public string Id { get; set; }
        public int OrgId { get; set; }
        public string Nome { get; set; }
        public string CodigoCliente { get; set; }
        public string OrganogramaPosicaoIdLider { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
    public class OrganogramaDeptoListarPorIDCompletoResponseDTO
    {
        public string Id { get; set; }
        public int OrgId { get; set; }
        public string Nome { get; set; }
        public string DescricaoOrg { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string OrganogramaPosicaoIdLider { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataCriacao { get; set; }

    }

    //-------------------- Posição ---------------------//
    public class OrganogramaPosicaoInserirParamDTO
    {
        //public int OrgId { get; set; }
        public string CodigoCliente { get; set; }
        public string OrganogramaDepartamentoId { get; set; }
        public string PerfilCorporativoId { get; set; }
        public string OrganogramaPosicaoIdSuperior { get; set; }
        public bool Ativo { get; set; }
        public bool CLevel { get; set; }
        public bool ProfissionalExterno { get; set; }
        public int MapaRelacionamentoInfluenciaId { get; set; }
        //public DateTime? DataCriacao { get; set; }
    }

    public class OrganogramaPosicaoAtualizarParamDTO
    {
        public string Id { get; set; }
        //public int OrgId { get; set; }
        public string CodigoCliente { get; set; }
        public string OrganogramaDepartamentoId { get; set; }
        public string PerfilCorporativoId { get; set; }
        public string OrganogramaPosicaoIdSuperior { get; set; }
        public bool Ativo { get; set; }
        public bool CLevel { get; set; }
        public bool ProfissionalExterno { get; set; }
        public int MapaRelacionamentoInfluenciaId { get; set; }

    }

    //-------------------- Response ---------------------//

    public class OrganogramaPosicaoListarPorIdResponseDTO
    {
        public string Id { get; set; }
        public int OrgId { get; set; }
        public string CodigoCliente { get; set; }
        public string DepartamentoId { get; set; }
        public string PerfilCorporativoId { get; set; }
        public string PosicaoIdSuperior { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool CLevel { get; set; }
        public bool ProfissionalExterno { get; set; }
        public int MapaRelacionamentoInfluenciaId { get; set; }
        public string MapaRelacionamentoInfluenciaDescricao { get; set; }
        /// <summary>Último orçamento registrado (histórico), por data de criação.</summary>
        public decimal? Orcamento { get; set; }
        public DateTime? OrcamentoDataInicio { get; set; }
        public DateTime? OrcamentoDataFim { get; set; }
    }

    //-------------------- Alocação ---------------------//
    public class OrganogramaAlocacaoInserirParamDTO
    {
        public string OrganogramaPosicaoId { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool Ativo { get; set; }
        //public DateTime? DataCriacao { get; set; }
    }

    public class OrganogramaAlocacaoAtualizarParamDTO
    {
        public string Id { get; set; }
        public string OrganogramaPosicaoId { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool Ativo { get; set; }

    }


    //-------------------- Response ---------------------//

    public class OrganogramaAlocacaoListarPorIdResponseDTO
    {
        public string Id { get; set; }
        public int OrgId { get; set; }
        public string OrganogramaPosicaoId { get; set; }
        public string TbPerfilCorporativoAlocacaoId { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }

    //-------------------- Perfil Corporativo Alocação ---------------------//
    public class OrganogramaPerfilCorporativoAlocacaoInserirParamDTO
    {
        public string PerfilCorporativoId { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool Ativo { get; set; }
    }

    public class OrganogramaPerfilCorporativoAlocacaoAtualizarParamDTO
    {
        public string Id { get; set; }
        public string PerfilCorporativoId { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool Ativo { get; set; }
    }

    public class OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO
    {
        public string Id { get; set; }
        public int OrgId { get; set; }
        public Guid PerfilCorporativoId { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }

    public class OrganogramaColaboradorAlocadoDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public string NomeColaborador { get; set; }
    }

    //------------------------------ Logs ----------------------------------

    public class OrganogramaLogDTO
    {
        public string Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string Acao { get; set; }
        public string CodigoInternoColaboradorAlterador { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Objeto { get; set; }
        public string Alteracoes { get; set; }
    }

    //------------------------------ Perfil Corporativo ----------------------------------

    public class OrganogramaPerfilCorporativoInserirParamDTO
    {
        //public int OrgId { get; set; }
        public string Descricao { get; set; }
        public string PermanenciaId { get; set; }
        public string ModeloTrabalhoId { get; set; }
        public string ProfissionalLocalidadeId { get; set; }
        public string EmpregoLinkdinId { get; set; }
        public string ExperienciaLinkedinId { get; set; }
        public string Atribuicoes { get; set; }
    }

    public class OrganogramaPerfilCorporativoAtualizarParamDTO
    {
        public string Id { get; set; }
        //public int OrgId { get; set; }
        public string Descricao { get; set; }
        public string PermanenciaId { get; set; }
        public string ModeloTrabalhoId { get; set; }
        public string ProfissionalLocalidadeId { get; set; }
        public string EmpregoLinkdinId { get; set; }
        public string ExperienciaLinkedinId { get; set; }
        public string Atribuicoes { get; set; }
        //public string CodigoInternoColaboradorCriacao { get; set; }
        //public string CodigoInternoColaboradorAlteracao { get; set; }
        public bool Ativo { get; set; }
    }

    //-------------------- Response ---------------------//

    public class OrganogramaPerfilCorporativoListarPorIdResponseDTO
    {
        public string Id { get; set; }
        public int OrgId { get; set; }
        public string Descricao { get; set; }
        public string PermanenciaId { get; set; }
        public string ModeloTrabalhoId { get; set; }
        public string ProfissionalLocalidadeId { get; set; }
        public string EmpregoLinkdinId { get; set; }
        public string ExperienciaLinkedinId { get; set; }
        public string Atribuicoes { get; set; }
        public string CodigoInternoColaboradorCriacao { get; set; }
        public string CodigoInternoColaboradorAlteracao { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool Ativo { get; set; }

    }


    //--------------------------- Perfil Corporativo Skill----------------------------------

    public class OrganogramaPerfilCorporativoSkillInserirParamDTO
    {
        //public int OrgId { get; set; }
        public string PerfilCorporativoId { get; set; }
        public int ItemPerfilId { get; set; }
        public int SkillId { get; set; }
        public int NivelId { get; set; }
    }

    public class OrganogramaPerfilCorporativoSkillAtualizarParamDTO
    {
        public string Id { get; set; }
        //public int OrgId { get; set; }
        public string PerfilCorporativoId { get; set; }
        public int ItemPerfilId { get; set; }
        public int SkillId { get; set; }
        public int NivelId { get; set; }
        //public string CodigoInternoColaboradorCriacao { get; set; }
        //public string CodigoInternoColaboradorAlteracao { get; set; }
        public bool Ativo { get; set; }
    }
    //-------------------- Response ---------------------//

    public class OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO
    {
        public string Id { get; set; }
        public int OrgId { get; set; }
        public string PerfilCorporativoId { get; set; }
        public int ItemPerfilId { get; set; }
        public int SkillId { get; set; }
        public int NivelId { get; set; }
        public string CodigoInternoColaboradorCriacao { get; set; }
        public string CodigoInternoColaboradorAlteracao { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool Ativo { get; set; }

    }

    //-------------------- Cliente Org ---------------------//
    public class OrganogramaClienteOrgDTO
    {
        public Guid Id { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
    }

    //-------------------- Organograma Completo ---------------------//
    public class OrganogramaCompletoResponseDTO
    {
        public int OrgId { get; set; }
        public string CodigoCliente { get; set; }
        public List<OrganogramaPosicaoCompletaDTO> Posicoes { get; set; } = new();
    }

    public class OrganogramaPosicaoCompletaDTO
    {
        public Guid Id { get; set; }
        public int OrgId { get; set; }
        public Guid? DepartamentoId { get; set; }
        public string DepartamentoNome { get; set; }
        public string CodigoCliente { get; set; }
        public Guid? PerfilCorporativoId { get; set; }
        public string PerfilCorporativoNome { get; set; }
        public Guid? PosicaoIdSuperior { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool CLevel { get; set; }
        public bool ProfissionalExterno { get; set; }
        public int MapaRelacionamentoInfluenciaId { get; set; }
        public string MapaRelacionamentoInfluenciaDescricao { get; set; }
        public decimal? Orcamento { get; set; }
        public DateTime? OrcamentoDataInicio { get; set; }
        public DateTime? OrcamentoDataFim { get; set; }
        public List<OrganogramaPosicaoAlocacaoCompletaDTO> Alocacoes { get; set; } = new();
    }

    public class OrganogramaPosicaoAlocacaoCompletaDTO
    {
        public Guid Id { get; set; }
        public string TbPerfilCorporativoAlocacaoId { get; set; }
        public Guid CodigoInternoColaborador { get; set; }
        public string NomeColaborador { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }

    public class OrganogramaPosicaoCompletaRow
    {
        public string PosicaoId { get; set; }
        public int OrgId { get; set; }

        public string? DepartamentoId { get; set; }
        public string? DepartamentoNome { get; set; }

        public string? CodigoCliente { get; set; }

        public Guid? PerfilCorporativoId { get; set; }
        public string? PerfilCorporativoNome { get; set; }

        public string? PosicaoIdSuperior { get; set; }
        public bool PosicaoAtivo { get; set; }
        public DateTime? PosicaoDataCriacao { get; set; }
        public DateTime? PosicaoDataAlteracao { get; set; }

        public bool CLevel { get; set; }
        public bool ProfissionalExterno { get; set; }

        public int MapaRelacionamentoInfluenciaId { get; set; }
        public string MapaRelacionamentoInfluenciaDescricao { get; set; }

        public decimal? PosicaoOrcamento { get; set; }
        public DateTime? PosicaoOrcamentoDataInicio { get; set; }
        public DateTime? PosicaoOrcamentoDataFim { get; set; }

        public string CodigoInternoColaborador { get; set; }
        public string? NomeColaborador { get; set; }

        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public string? AlocacaoId { get; set; }
        public string? TbPerfilCorporativoAlocacaoId { get; set; }
        public bool AlocacaoAtivo { get; set; }
        public DateTime? AlocacaoDataCriacao { get; set; }
        public DateTime? AlocacaoDataAlteracao { get; set; }
    }

}

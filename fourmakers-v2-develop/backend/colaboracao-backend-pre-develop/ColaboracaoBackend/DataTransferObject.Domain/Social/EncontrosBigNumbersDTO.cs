using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Social
{
    public class EncontrosBigNumbers
    {
        public int ReunioesRealizados { get; set; }
        public int ReunioesSemInteracao { get; set; }
        public int AcoesEmAtraso { get; set; }
        public int ClientesImpactados { get; set; }
        public int GestoresImpactados { get; set; }
        public int CategoriasComInteracao { get; set; }
    }

    public class EncontrosBigNumbersParam
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string CodigoCliente { get; set; } // tb_cliente_org_codigo_cliente
        public string CodigoColaboradorAgendou { get; set; } // tb_colaborador_codigo_interno_colaborador
        public string CodigoGestorExterno { get; set; } // cod_gestor_externo
    }

    public class EncontrosBigNumbersCategoria
    {
        public int CategoriaId { get; set; }
        public string CategoriaDescricao { get; set; }

        /// <summary>
        /// Quantidade de vínculos em tb_interacoes_categoria_sub (por subcategoria). Bate com a soma de itens em interacoes no CategoriaEmFocoDetalhe (a mesma interação pode contar mais de uma vez em subcategorias diferentes).
        /// </summary>
        public int TotalInteracoes { get; set; }
    }

    public class EncontrosBigNumbersObjetivo
    {
        public int ObjetivoId { get; set; }
        public string ObjetivoTitulo { get; set; }
        public string ObjetivoDescricao { get; set; }
        public int TotalAgendas { get; set; }
    }

    public class AgendaRealizadaDetalhe
    {
        public int AgendaId { get; set; }
        public string Organizador { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string Titulo { get; set; }
        public DateTime DataAgendada { get; set; }
        public List<GestorClienteDetalhe> Gestores { get; set; }
    }

    public class AgendaSemInteracaoDetalhe
    {
        public int AgendaId { get; set; }
        public string Titulo { get; set; }
        public DateTime? DataAgendada { get; set; }
        public string NomeCliente { get; set; }
        public int? TipoAgendaId { get; set; }
        public string TipoAgendaDescricao { get; set; }
        public string CriadorDaAgenda { get; set; }
    }

    public class ClientesImpactadosDetalhe
    {
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public List<AgendaDoCliente> Agendas { get; set; }
    }

    public class AgendaDoCliente
    {
        public int AgendaId { get; set; }
        public string Titulo { get; set; }
        public DateTime? DataAgendada { get; set; }
        public int? TipoAgendaId { get; set; }
        public string TipoAgendaDescricao { get; set; }
    }

    public class GestoresImpactadosDetalhe
    {
        public string CodigoGestorExterno { get; set; }
        public string NomeGestor { get; set; }
        public List<AgendaDoGestor> Agendas { get; set; }
    }

    public class AgendaDoGestor
    {
        public int AgendaId { get; set; }
        public string Titulo { get; set; }
        public DateTime? DataAgendada { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public int? TipoAgendaId { get; set; }
        public string TipoAgendaDescricao { get; set; }
    }

    public class CategoriaComInteracaoDetalhe
    {
        public int CategoriaId { get; set; }
        public string CategoriaDescricao { get; set; }
        public List<AgendaDaCategoria> Agendas { get; set; }
    }

    public class AgendaDaCategoria
    {
        public int AgendaId { get; set; }
        public string Titulo { get; set; }
        public DateTime? DataAgendada { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public int? TipoAgendaId { get; set; }
        public string TipoAgendaDescricao { get; set; }
        public int? SubcategoriaId { get; set; }
        public string SubcategoriaDescricao { get; set; }
        public List<InteracaoDaCategoria> Interacoes { get; set; }
    }

    public class InteracaoDaCategoria
    {
        public int? InteracaoId { get; set; }
        public string TituloInteracao { get; set; }
        public string DescricaoInteracao { get; set; }

        /// <summary>Subcategoria do vínculo em tb_interacoes_categoria_sub (a mesma interação pode aparecer mais de uma vez com subcategorias diferentes).</summary>
        public int? SubcategoriaId { get; set; }
        public string SubcategoriaDescricao { get; set; }
    }

    public class ObjetivosAgendaDetalhe
    {
        public int ObjetivoId { get; set; }
        public string ObjetivoTitulo { get; set; }
        public string ObjetivoDescricao { get; set; }
        public List<AgendaDoObjetivo> Agendas { get; set; }
    }

    public class AgendaDoObjetivo
    {
        public int AgendaId { get; set; }
        public string Titulo { get; set; }
        public DateTime? DataAgendada { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public int? TipoAgendaId { get; set; }
        public string TipoAgendaDescricao { get; set; }
        public string Organizador { get; set; }
    }

    public class AcoesEmAtrasoDetalhe
    {
        public int StatusAcaoId { get; set; }
        public string StatusDescricao { get; set; }
        public List<AgendaComAcao> Agendas { get; set; }
    }

    public class AgendaComAcao
    {
        public int AgendaId { get; set; }
        public string Titulo { get; set; }
        public DateTime? DataAgendada { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public int? TipoAgendaId { get; set; }
        public string TipoAgendaDescricao { get; set; }
        public int? AcaoId { get; set; }
        public DateTime? DataLimiteAcao { get; set; }
        public string ResponsavelAcao { get; set; }        
        public string DescricaoAcao { get; set; }

    }

    public class CategoriaEmFocoDetalhe
    {
        public int CategoriaId { get; set; }
        public string CategoriaDescricao { get; set; }
        public List<SubcategoriaComAgendas> Subcategorias { get; set; }
    }

    public class SubcategoriaComAgendas
    {
        public int SubcategoriaId { get; set; }
        public string SubcategoriaDescricao { get; set; }
        public List<AgendaDaSubcategoria> Agendas { get; set; }
    }

    public class AgendaDaSubcategoria
    {
        public int AgendaId { get; set; }
        public string Titulo { get; set; }
        public DateTime? DataAgendada { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string Organizador { get; set; }
        public List<InteracaoDaAgenda> Interacoes { get; set; }
        public List<GestorDaAgenda> Gestores { get; set; }
    }

    public class InteracaoDaAgenda
    {
        public int? InteracaoId { get; set; }
        public string TituloInteracao { get; set; }
        public string DescricaoInteracao { get; set; }
    }

    public class GestorDaAgenda
    {
        public string CodigoGestorExterno { get; set; }
        public string Nome { get; set; }
    }

    public class GestorClienteDetalhe
    {
        public string CodigoGestorExterno { get; set; }
        public string Nome { get; set; }
    }

    public class AgendaVersaoDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Versao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

    public class AtualizaVersaoAppParam
    {
        public string Descricao { get; set; }
        public string Versao { get; set; }
    }
    public sealed class CategoriaEmFocoSqlRow
    {
        public int CategoriaId { get; set; }
        public string CategoriaDescricao { get; set; }
        public int SubcategoriaId { get; set; }
        public string SubcategoriaDescricao { get; set; }
        public int AgendaId { get; set; }
        public string Titulo { get; set; }
        public DateTime? DataAgendada { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string Organizador { get; set; }
        public int InteracaoId { get; set; }
        public string TituloInteracao { get; set; }
        public string DescricaoInteracao { get; set; }
    }

    public sealed class CategoriaComInteracaoSqlRow
    {
        public int CategoriaId { get; set; }
        public string CategoriaDescricao { get; set; }
        public int AgendaId { get; set; }
        public string Titulo { get; set; }
        public DateTime? DataAgendada { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public int? TipoAgendaId { get; set; }
        public string TipoAgendaDescricao { get; set; }
        public int SubcategoriaId { get; set; }
        public string SubcategoriaDescricao { get; set; }
        public int InteracaoId { get; set; }
        public string TituloInteracao { get; set; }
        public string DescricaoInteracao { get; set; }
        public int TcsId { get; set; }
    }

}

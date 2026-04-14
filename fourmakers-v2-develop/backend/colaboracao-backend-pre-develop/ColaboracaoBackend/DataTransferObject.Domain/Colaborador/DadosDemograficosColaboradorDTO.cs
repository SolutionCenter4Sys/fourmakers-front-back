using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class DadosDemograficosColaboradorDTO
    {
        // Família & Dependentes
        public int QuantidadePessoasResidencia { get; set; } // Mora com quantas pessoas? (mínimo 1)
        public int DependentesIRPF { get; set; } // Dependentes IRRF (mínimo 0)
        public bool PossuiConjuge { get; set; } // Possui cônjuge/companheiro(a)?
        public DateTime? DataNascimentoConjuge { get; set; } // Data de Nascimento do Cônjuge (condicional)
        public bool PossuiFilhos { get; set; } // Possui filhos?
        public List<FilhoColaboradorDTO> Filhos { get; set; } // Lista de filhos

        // Seguro Saúde (e Plano Foursys)
        public bool PossuiSeguroSaude { get; set; } // Possui seguro saúde?
        public decimal? ValorAtualSeguroSaude { get; set; } // Valor Atual (R$)
        public string OperadoraSeguroSaude { get; set; } // Operadora
        public string AcomodacaoSeguroSaude { get; set; } // Acomodação: 'Apartamento' | 'Enfermaria' | ''
        public bool SeguroSaudePossuiCoparticipacao { get; set; } // Possui coparticipação?
        public string ObservacoesSeguroSaude { get; set; } // Observações
        public bool PossuiInteressePlanoFoursys { get; set; } // Possui interesse no plano Foursys?
        public string FaixaEtaria { get; set; } // Faixa Etária (auto-derivado de birthDate)
        public string CategoriaPlanoSaude { get; set; } // Categoria do Plano: 'general' | 'supervisor' | 'executive'
        public bool IncluirDependentesPlanoFoursys { get; set; } // Incluir dependentes
        public int QuantidadeDependentesPlanoFoursys { get; set; } // Quantidade de Dependentes (mínimo 1 se IncluirDependentesPlanoFoursys = true)
        public decimal? ValorPlanoDependentes { get; set; } // Valor do Plano Dependentes (R$)

        // Alimentação
        public decimal? ValorCartaoRefeicao { get; set; } // Cartão Refeição (R$) - constante: 660
        public decimal? ValorCartaoAlimentacao { get; set; } // Cartão Alimentação (R$)

        // Educação
        public bool EstudaAtualmente { get; set; } // Estuda atualmente?
        public decimal? CustoMensalEducacao { get; set; } // Custo Mensal com Educação (R$)
        public bool FilhosEstudamAte24Anos { get; set; } // Filhos (até 24 anos) estudam?
        public decimal? CustoMensalEducacaoFilhos { get; set; } // Custo Mensal com Educação dos Filhos (R$)
        public decimal? CustoTotalEducacao { get; set; } // Custo Total Educação (R$) - derivado: EstudaAtualmente + FilhosEstudamAte24Anos

        // Preferências do Candidato
        // Nota: ModeloDeTrabalhoPretendido foi removido - deve ser atualizado em tb_candidato_vaga.tb_modelo_trabalho_id
        // Nota: DiasPresenciaisDesejados foi removido - deve ser atualizado em tb_candidato_vaga.quantidade_dias_presencial
        // Nota: PretencaoLiquidaRef foi removido - deve ser atualizado em tb_candidato_vaga.pretensao_salarial

        // Mobilidade
        public decimal? DistanciaIdaVolta { get; set; } // Distância (Ida/Volta) em km

        // Outros Custos (variáveis)
        public List<OutroCustoColaboradorDTO> OutrosCustos { get; set; } = new List<OutroCustoColaboradorDTO>(); // Lista de outros custos variáveis
    }
}

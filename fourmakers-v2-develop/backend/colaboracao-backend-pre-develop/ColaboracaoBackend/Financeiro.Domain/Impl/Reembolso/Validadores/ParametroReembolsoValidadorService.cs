using System.ComponentModel.DataAnnotations;
using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Financeiro.Reembolso.Parametro;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;
using Financeiro.Domain.Interfaces.Reembolso.Validadores;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Services.Reembolso.Validadores;

[LogDomainClass]
public class ParametroReembolsoValidadorService : IParametroReembolsoValidadorService
{

    public void ValidaParametroReembolso(ParametroReembolsoDTO input, CRUDEnum operation)
    {
        if (operation == CRUDEnum.Update || operation == CRUDEnum.Create)
        {
            ValidaPersistenciaDia(input);
        }
    }

    private void ValidaPersistenciaDia(ParametroReembolsoDTO input)
    {
        if (input.LimiteEnvio > 31)
        {
            throw new ArgumentException("Não é possível ter um limite maior que o dia 31");
        }
        
        if (input.DiaPagamento > 31)
        {
            throw new ArgumentException("Não é possível ter um dia de pagamento maior que o dia 31");
        }
        
        if (input.DiaPagamentoAlternativo != null && input.DiaPagamentoAlternativo > 31)
        {
            throw new ArgumentException("Não é possível ter um dia de pagamento alternativo maior que o dia 31");
        }
        
        if (input.LimiteEnvioAlternativo != null && input.LimiteEnvioAlternativo > 31)
        {
            throw new ArgumentException("Não é possível ter um limite alternativo maior que o dia 31");
        }
    }
    
    public List<VerbaLogDTO> GerenciarLogs(ParametroReembolsoDTO prev, ParametroReembolsoDTO next, CRUDEnum tipo)
    {
        var logs = new List<VerbaLogDTO>();
    
        void AdicionarLog(string regra, string valorAnterior, string novoValor)
        {
            logs.Add(new VerbaLogDTO
            {
                Regra = regra,
                Acao = tipo.ObterAcao(),
                ValorAnterior = valorAnterior,
                NovoValor = novoValor
            });
        }
    
        if (prev.LimiteEnvio != next.LimiteEnvio)
            AdicionarLog("Limite de Envio", prev.LimiteEnvio.ToString(), next.LimiteEnvio.ToString());
    
        if (prev.DiaPagamento != next.DiaPagamento)
            AdicionarLog("Dia de Pagamento", prev.DiaPagamento.ToString(), next.DiaPagamento.ToString());
    
        if (prev.ValidadeComprovanteDias != next.ValidadeComprovanteDias)
            AdicionarLog("Validade do Comprovante (Dias)", prev.ValidadeComprovanteDias.ToString(), next.ValidadeComprovanteDias.ToString());
    
        if (prev.LimiteEnvioAlternativo != next.LimiteEnvioAlternativo)
            AdicionarLog("Limite de Envio Alternativo",
                prev.LimiteEnvioAlternativo?.ToString() ?? "Vazio",
                next.LimiteEnvioAlternativo?.ToString() ?? "Vazio");
    
        if (prev.DiaPagamentoAlternativo != next.DiaPagamentoAlternativo)
            AdicionarLog("Dia de Pagamento Alternativo",
                prev.DiaPagamentoAlternativo?.ToString() ?? "Vazio",
                next.DiaPagamentoAlternativo?.ToString() ?? "Vazio");
    
        return logs;
    }
}
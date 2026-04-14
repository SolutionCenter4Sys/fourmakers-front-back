using System.ComponentModel.DataAnnotations;
using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;
using Financeiro.Domain.Interfaces.Reembolso.Validadores;
using Financeiro.Domain.Interfaces.Reembolso.Verba;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Services.Reembolso.Validadores;

[LogDomainClass]
public class VerbaValidadorService(IVerbaTipoService verbaTipoService) : IVerbaValidadorService
{

    public async Task ValidaVerba(VerbaDTO verba, CRUDEnum crud)
    {
        if (crud == CRUDEnum.Create || crud == CRUDEnum.Update)
        {
            await ValidaTipoCustoPersistencia(verba.TipoCusto, verba.Unidade);
            ValidaCamposObrigatorios(verba);
        }

        if (crud == CRUDEnum.Update)
        {
            ValidaIdVerba(verba);
        }
    }

    private async Task ValidaTipoCustoPersistencia(int tipo, string? unidade)
    {
        var existeTipo = await verbaTipoService.BuscarPorIdAsync(tipo);
        if (existeTipo.Retorno == null)
        {
            throw new ArgumentException("Tipo de custo não existe.");
        }
        if (existeTipo.Retorno.TipoCodigo == VerbaTipoCustoEnum.Fixo && string.IsNullOrWhiteSpace(unidade))
        {
            throw new ArgumentException("Tipo de custo 'Fixo' requer uma unidade especificada.");
        }
    }
    

    private void ValidaIdVerba(VerbaDTO verba)
    {
        if (verba.Id == 0 || verba.Id == null)
        {
            throw new ArgumentException("Não é possível editar verba com id 0 ou nulo");
        }
    }

    private void ValidaCamposObrigatorios(VerbaDTO verba)
    {
        if (verba.Valor <= 0)
        {
            throw new ArgumentException("Valor tem que ser maior que 0");
        }

        if (string.IsNullOrEmpty(verba.Categoria))
        {
            throw new ArgumentException("Categoria é Obrigatório");
        }
    }
    
    public async Task<List<VerbaLogDTO>> GerenciarLogs(VerbaDTO prev, VerbaDTO next, CRUDEnum tipo)
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

        if (prev.Unidade != next.Unidade)
            AdicionarLog("Unidade", prev.Unidade.ToString(), next.Unidade.ToString());

        if (prev.Categoria != next.Categoria)
            AdicionarLog("Categoria", prev.Categoria.ToString(), next.Categoria.ToString());

        if (prev.Valor != next.Valor)
            AdicionarLog("Valor", prev.Valor.ToString(), next.Valor.ToString());

        if (prev.TipoCusto != next.TipoCusto)
        {
            var tipoAntigo = await verbaTipoService.BuscarPorIdAsync(prev.TipoCusto);
            var tipoNovo = await verbaTipoService.BuscarPorIdAsync(next.TipoCusto);
            AdicionarLog("Tipo custo",
                tipoAntigo.Retorno.Descricao,
                tipoNovo.Retorno.Descricao);
        }
           

        if (prev.CustoCliente != next.CustoCliente)
            AdicionarLog("Custo cliente",
                prev.CustoCliente ? "Ativo" : "Inativo",
                next.CustoCliente ? "Ativo" : "Inativo");
        
        if (prev.Ativo != next.Ativo)
            AdicionarLog("Ativo",
                prev.Ativo ? "Ativo" : "Inativo",
                next.Ativo ? "Ativo" : "Inativo");

        return logs;
    }
}
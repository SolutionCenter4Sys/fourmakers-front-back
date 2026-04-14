using Colaborador.Domain.Interfaces.Models;
using System;

namespace Colaborador.Domain.Interfaces.Factorys
{
    public interface IColaboradorDomainFactory
    {
        IColaboradorModel buildColaboradorModel();
        IColaboradorModel buildColaboradorModel(DateTime? admissao, sbyte ativo, sbyte candidato, string contato_outro, string contato_principal, string cpf, DateTime data_alteracao, DateTime data_criacao, DateTime data_nascimento, string diretoria_id, long? endereco_id, long? imagem_id, string matricula, string nome_completo, string rg, string email, string slack_id, string fcmToken, int idCargo, string nomeCargo, string descricaoDiretoria, int idStatus, string nomeStatus, string cep, string endereco, string complemento, int? numero, string bairro, string cidade, string estado);
        IColaboradorModel buildColaboradorModel(string cpf, string idDiretoria, string descricaoDiretoria, string nome_completo, string email, string slack_id, int idStatus, string descricaoStatus);
        IColaboradorModel buildStatusColaboradorModel();
        IColaboradorModel buildStatusColaboradorModel(int id, string descricao);
    }
}
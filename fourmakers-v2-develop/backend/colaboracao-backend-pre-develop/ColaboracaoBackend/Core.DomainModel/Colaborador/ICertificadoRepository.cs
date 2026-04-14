using System;
using System.Threading.Tasks;

namespace Core.Domain.Colaborador;

public interface ICertificadoRepository
{
    Task SalvarCertificadoAsync(string codigoInternoColaborador, string path, string descricao, string instituicao, DateTime dataConclusao, int cargaHoraria);
}
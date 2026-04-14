using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Colaborador.ModeloTrabalho;

namespace Core.Domain.Colaborador;

public interface IColaboradorOrgRepository
{
    Task EditarModeloTrabalhoAsync(int orgId, string codigoInternoColaborador, ModeloTrabalhoColaboradorDTO input);
}
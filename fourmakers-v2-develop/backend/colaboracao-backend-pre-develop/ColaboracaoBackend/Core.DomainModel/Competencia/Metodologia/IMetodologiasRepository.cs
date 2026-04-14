using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Metodologia;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;

namespace Core.Domain.Competencia.Metodologia
{
    public interface IMetodologiasRepository
    {
        long ObterIdUsuarioPorCpf(string cpf);
        List<long> ListarMetodologiasAtribuidas(string cpf);
        List<MetodologiaDTO> ListarMetodologias(string busca, int cursor, int limite);
        MetodologiaDTO ListarMetodologiasById(int codMetodologia);
        public List<NivelDTO> ListarNivelMetodologia();
        List<ListaMetodologiaColaboradorResult> ListarMetodologiasColaborador(string cpf);

        long InserirMetodologiaColaborador(MetodologiaDTO param, string cpf);
        long AdicionarMetodologia(MetodologiaDTO param, string cpf);
        void AtualizarNivelMetodologiaColaborador(MetodologiaDTO metodologia, string cpf);
        void RemoverMetodologiaColaborador(int codMetodologia, string cpf);

        void AssociaMetodologiaComColaborador(MetodologiaDTO metodologia, string cpf);
        bool MetodologiaExiste(string descricao);
        bool IsAssociadaComColaborador(long idMetodologia, string cpf);
    }
}
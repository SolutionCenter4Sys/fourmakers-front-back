using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.VagasSRS;

namespace DataTransferObject.Domain.Vaga
{
    public class ListarVagasEmBancoDeTalentosResult
    {
        public string Id { get; set; }
        public string Cliente { get; set; }
        public string Titulo { get; set; }
        public string Gestor { get; set; }
        public string Criador { get; set; }
        public DateTime Criacao { get; set; }
        public int Posicoes { get; set; }
        public string Descricao { get; set; }
        public string Localizacao { get; set; }
        public string Estado { get; set; }
        public int Codigo { get; set; }
        public string ModalidadeDescricao { get; set; }
        public string Cargo { get; set; }
        public string Cidade { get; set; }

        public List<VagaSkillRecrutamentoDTO> Skills { get; set; }
        public string Tracking { get; set; }
        public string NumeroVagaCliente { get; set; }
        public int? CodigoClienteFourmakers { get; set; }
        public int? OrgId { get; set; }
    }
}

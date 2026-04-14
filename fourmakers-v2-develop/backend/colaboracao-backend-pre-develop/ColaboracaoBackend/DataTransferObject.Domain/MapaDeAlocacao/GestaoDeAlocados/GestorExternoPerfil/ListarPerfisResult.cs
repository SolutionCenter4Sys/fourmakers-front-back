using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil
{
    public class ListarPerfisResult
    {
        public string GestorExternoPerfilNome { get; set; }
        public Guid GestorExternoPerfilId { get; set; }
        public DateTime DataCriacao { get; set; }
        public string NomeCliente { get; set; }
        public string CodCliente { get; set; }
        public string CodGestorExterno { get; set; }
        public string NomeGestorExterno { get; set; }
        public DateTime UltimaAlteracao { get; set; }
        public string UsuarioAlterador { get; set; }
        public string CodColaboradorCriador { get; set; }
        public string CodColaboradorAlteracao { get; set; }
        public string NomeColaboradorCriador { get; set; }
        public string NomeColaboradorAlteracao { get; set; }
    }
}
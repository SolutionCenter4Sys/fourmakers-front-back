namespace DataTransferObject.Domain.Marketing.Comunicacao.Profissionais
{
    public class ProfissionalDTO
    {
        public string CodigoColaboradorInterno { get; set; }
        public string Email { get; set; }
        public string NomeCompleto { get; set; }
        public string CodCargo { get; set; }
        public string Cargo { get; set; }
        public string CodDepartamento { get; set; }
        public string Departamento { get; set; }
        public string CodDiretoria { get; set; }
        public string Diretoria { get; set; }
        public string Telefone { get; set; }
        public string UrlFoto { get; set; }
        public string Sobre { get; set; }
        public string DataAniversario { get; set; }
        public string Supervisor { get; set; }
        /// <summary>URL para acessar o perfil/detalhe do profissional (ex: app/colaborador/{codigo}).</summary>
        public string UrlPerfil { get; set; }
        /// <summary>Indica se o profissional está favoritado pelo usuário logado.</summary>
        public bool Favoritado { get; set; }
    }
}

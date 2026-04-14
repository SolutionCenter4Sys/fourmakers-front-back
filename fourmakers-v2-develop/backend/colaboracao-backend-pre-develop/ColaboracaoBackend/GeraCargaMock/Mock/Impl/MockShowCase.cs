using Bogus;
using Bogus.Extensions.Brazil;
using DataTransferObject.Domain.Carga;
using RotinasBackoffice.API.Mock.Interface;

namespace RotinasBackoffice.API.Mock.Impl
{
    public class MockShowCase : IMockShowCase
    {
        private KeyValuePair<int, string>[] _status = new KeyValuePair<int, string>[] {
            new KeyValuePair<int, string>(1, "Proposta"),
            new KeyValuePair<int, string>(2, "Desenvolvimento"),
            new KeyValuePair<int, string>(4, "Finalizado / Faturado")
         };
        private KeyValuePair<string, string>[] _diretorias = new KeyValuePair<string, string>[] {
            new KeyValuePair<string, string>("1", "Filial Padrão")
         };
        private List<ProjetoCargaDTO> _cargaProjeto;
        private List<ProjetoGerenteCargaDTO> _cargaProjetoGerente;
        private List<ColaboradorCargaDTO> _cargaColaborador;
        private List<ColaboradorHierarquiaCargaDTO> _cargaHierarquiaColaborador;
        public MockShowCase()
        {
            Randomizer.Seed = new Random(7266633);
            var lorem = new Bogus.DataSets.Lorem(locale: "pt_BR");
            long idCliente = 0;
            long idProjeto = 0;
            long idCargo = 0;
            long idDepartamento = 0;
            long idColaboradorExterno = 0;
            int maxGerentes = 10;
            int numberOfProjects = 100;
            int numberOfColaboradores = 100;
            int indexSelectProjeto = 0;
            int indexSelectColaborador = 0;
            var cargaProjetoFaker = new Faker<ProjetoCargaDTO>("pt_BR")
                .StrictMode(true)
                .RuleFor(x => x.Cliente, (f, x) => f.Company.CompanyName())
                .RuleFor(x => x.DataInicio, (f, x) => f.Date.Past(1, DateTime.Now))
                .RuleFor(x => x.DataFim, (f, x) => f.Date.Future(2, x.DataInicio))
                .RuleFor(x => x.IdentificadorFilial, (f, x) => f.PickRandom<KeyValuePair<string, string>>(_diretorias).Key)
                .RuleFor(x => x.Filial, (f, x) => _diretorias.Where(y => y.Key == x.IdentificadorFilial).First().Value)
                .RuleFor(x => x.IdentificadorCliente, (f, x) => (idCliente++).ToString())
                .RuleFor(x => x.IdentificadorClienteRefatorado, (f, x) => String.Empty)
                .RuleFor(x => x.Propostas, (f, x) => new List<string>())
                .RuleFor(x => x.IdentificadorProjeto, (f, x) => (idProjeto++).ToString())
                .RuleFor(x => x.IdentificadorProposta, (f, x) => f.Random.AlphaNumeric(12))
                .RuleFor(x => x.IndentificadorStatus, (f, x) => f.PickRandom<KeyValuePair<int, string>>(_status).Key)
                .RuleFor(x => x.Status, (f, x) => _status.Where(y => y.Key == x.IndentificadorStatus).First().Value)
                .RuleFor(x => x.Projeto, (f, x) => f.Company.CatchPhrase())
                .RuleFor(x => x.QtdHorasPlanejada, (f, x) => f.Random.Number(500, 5000))
                .RuleFor(x => x.QtdHorasExecutadas, (f, x) => f.Random.Number(0, (int)x.QtdHorasPlanejada))
            ;
            var cargaGestorFaker = new Faker<ProjetoGerenteCargaDTO>("pt_BR")
                .StrictMode(true)
                .RuleFor(x => x.IdentificadorColaboradorGerente, (f, x) => (f.Random.Number(1, maxGerentes + 1)).ToString())
                .RuleFor(x => x.IdentificadorTipoGerente, (f, x) => "GerenteProjeto")
                .RuleFor(x => x.IdentificadorProjeto, (f, x) => _cargaProjeto.ElementAt(indexSelectProjeto++).IdentificadorProjeto)
            ;
            var cargaCargosFaker = new Faker<CargoFaker>("pt_BR")
                .StrictMode(true)
                .RuleFor(x => x.CodCargo, (f, x) => (idCargo++).ToString())
                .RuleFor(x => x.Cargo, (f, x) => f.Name.JobTitle()).Generate(20)
            ;
            var cargaDepartamentosFaker = new Faker<DepartamentoFaker>("pt_BR")
                .StrictMode(true)
                .RuleFor(x => x.CodDepartamento, (f, x) => (idDepartamento++).ToString())
                .RuleFor(x => x.Departamento, (f, x) => f.Commerce.Department()).Generate(20)
            ;
            var cargaColaboradorFaker = new Faker<ColaboradorCargaDTO>("pt_BR")
                .StrictMode(false)
                .RuleFor(x => x.Ativo, (f, x) => true)
                .RuleFor(x => x.IdentificadorColaborador, (f, x) => idColaboradorExterno++.ToString())
                .RuleFor(x => x.ColaboradorAtivo, (f, x) => true)
                .RuleFor(x => x.Cpf, (f, x) => f.Person.Cpf(false))
                .RuleFor(x => x.Email, (f, x) => f.Internet.Email(null, null, "showcase.com"))
                .RuleFor(x => x.DataAdmissao, (f, x) => f.Date.Past(4, DateTime.Now))
                .RuleFor(x => x.NomeCompleto, (f, x) => f.Name.FullName())
                .RuleFor(x => x.IdentificadorFilial, (f, x) => f.PickRandom<KeyValuePair<string, string>>(_diretorias).Key)
                .RuleFor(x => x.Filial, (f, x) => _diretorias.Where(y => y.Key == x.IdentificadorFilial).First().Value)
                .RuleFor(x => x.IdentificadorCargo, (f, x) => f.PickRandom<CargoFaker>(cargaCargosFaker).CodCargo)
                .RuleFor(x => x.Cargo, (f, x) => cargaCargosFaker.Where(y => y.CodCargo == x.IdentificadorCargo).First().Cargo)
                .RuleFor(x => x.IdentificadorDepartamento, (f, x) => f.PickRandom<DepartamentoFaker>(cargaDepartamentosFaker).CodDepartamento)
                .RuleFor(x => x.Departamento, (f, x) => cargaDepartamentosFaker.Where(y => y.CodDepartamento == x.IdentificadorDepartamento).First().Departamento)
                .RuleFor(x => x.Projetos, (f, x) => new List<ColaboradorProjetoCargaDTO>()
                )
            ;
            var cargaHierarquiaFaker = new Faker<ColaboradorHierarquiaCargaDTO>("pt_BR")
                .StrictMode(true)
                .RuleFor(x => x.IdentificadorSuperior, (f, x) => (f.Random.Number(0, maxGerentes)).ToString())
                .RuleFor(x => x.IdentificadorColaborador, (f, x) => _cargaColaborador.ElementAt(indexSelectColaborador++).IdentificadorColaborador)
            ;
            _cargaProjeto = cargaProjetoFaker.Generate(numberOfProjects);
            _cargaProjetoGerente = cargaGestorFaker.Generate(numberOfProjects);
            _cargaColaborador = cargaColaboradorFaker.Generate(numberOfColaboradores);
            _cargaHierarquiaColaborador = cargaHierarquiaFaker.Generate(numberOfColaboradores);

            foreach (var hierarquiaColab in _cargaHierarquiaColaborador)
            {
                var colabItem = _cargaColaborador.Where(x => x.IdentificadorColaborador == hierarquiaColab.IdentificadorColaborador).First();
                var gerenteProjetos = _cargaProjetoGerente.Where(x => x.IdentificadorColaboradorGerente == hierarquiaColab.IdentificadorSuperior).ToList();
                if (gerenteProjetos.Count > 0)
                    colabItem.Projetos = gerenteProjetos.Select(x => new ColaboradorProjetoCargaDTO
                    {
                        CodigoColaborador = hierarquiaColab.IdentificadorColaborador,
                        CodigoProjeto = x.IdentificadorProjeto,
                        NomeProjeto = _cargaProjeto.Where(y => y.IdentificadorProjeto == x.IdentificadorProjeto).First().Projeto
                    }).Take(new Faker().Random.Number(1, gerenteProjetos.Count)).ToList();
            }
        }
        public List<ColaboradorCargaDTO> GeraCargaColaborador()
        {
            return _cargaColaborador;
        }

        public List<ColaboradorHierarquiaCargaDTO> GeraCargaHierarquia()
        {
            return _cargaHierarquiaColaborador;
        }

        public List<ProjetoCargaDTO> GeraCargaProjeto()
        {
            return _cargaProjeto;
        }

        public List<ProjetoGerenteCargaDTO> GeraCargaProjetoGerente()
        {
            return _cargaProjetoGerente;
        }
    }
}
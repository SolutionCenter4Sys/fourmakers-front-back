using System;

namespace DataTransferObject.Domain.Fourmakers.Parametro
{
    public class ParametroRepositoryInput : ParametroBase
    {
        public ParametroRepositoryInput()
        { }
        public ParametroRepositoryInput(ParametroBase input, Guid id, long usuarioIdAlteracao)
        {
            NomeParametro = input.NomeParametro;
            DescricaoParametro = input.DescricaoParametro;
            CodigoParametro = input.CodigoParametro.ToUpperInvariant();
            CodigoModuloSistema = input.CodigoModuloSistema.ToUpperInvariant();
            Id = id;
            UsuarioIdAlteracao = usuarioIdAlteracao;
            Ativo = true;
        }

        public Guid Id { get; set; }
        public bool Ativo { get; set; }
        public string TipoParametro { get; set; }
        public long UsuarioIdAlteracao { get; }
    }
}
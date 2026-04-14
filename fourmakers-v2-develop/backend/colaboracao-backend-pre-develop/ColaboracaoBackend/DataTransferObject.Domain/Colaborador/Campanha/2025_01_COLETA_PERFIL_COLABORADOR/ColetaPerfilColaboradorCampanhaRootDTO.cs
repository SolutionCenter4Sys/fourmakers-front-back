using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Arquivo;

namespace DataTransferObject.Domain.Colaborador.Campanha._2025_01_COLETA_PERFIL_COLABORADOR;

public class ColetaPerfilColaboradorCampanhaRootDTO
{
    public Base64InputParam<ColetaPerfilEnderecoResidencia> Endereco { get; set; }
    public ColetaPerfilFormaAtuacao FormaAtuacao { get; set; }
    public Base64InputParam<ColetaPerfilCertificacao> AwsTechnical { get; set; }
    public Base64InputParam<ColetaPerfilCertificacao> AwsTechnicalFoundational { get; set; }
    public Base64InputParam<ColetaPerfilCertificacao> AwsTechnicalAccredited { get; set; }
    public ColetaPerfilDadosPessoais DadosPessoais { get; set; }
    
}

public class ColetaPerfilColaboradorJsonDTO
{
    public ColetaPerfilEnderecoResidencia Endereco { get; set; }
    public ColetaPerfilFormaAtuacao FormaAtuacao { get; set; }
    public ColetaPerfilCertificacao AwsTechnical { get; set; }
    public ColetaPerfilCertificacao AwsTechnicalFoundational { get; set; }
    public ColetaPerfilCertificacao AwsTechnicalAccredited { get; set; }
    public ColetaPerfilDadosPessoais DadosPessoais { get; set; }
}

public class Base64InputParam <T>
{
    public Base64DTO ArquivoBase64 { get; set; }
    public T Root { get; set; }
}

public class ColetaPerfilEnderecoResidencia
{
    public string CodigoPostal { get; set; }
    public string Endereco { get; set; }
    public int Numero { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string Complemento { get; set; }
    public string ComprovanteResidenciaPath { get; set; }
}

public class ColetaPerfilFormaAtuacao
{
    public string ModeloTrabalho { get; set; }
    public string Frequencia { get; set; }
    public int FrequenciaId { get; set; }
    public List<string> DiasSemana { get; set; }
    public string LocalTrabalho { get; set; }
    public string ClienteNome { get; set; }
    public string ClienteEndereco { get; set; }
}

public class ColetaPerfilCertificacao
{
    public string Status { get; set; }
    public string FilePath { get; set; }
    public string Nome { get; set; }
    public DateTime? DataEmissao { get; set; }
    public string Emissor { get; set; }
    public int CargaHoraria { get; set; }
    public DateTime? PrevisaoConclusao { get; set; }
}

public class ColetaPerfilDadosPessoais
{
    public string Linkedin { get; set; }
    public string Pdc { get; set; }
    public string Telefone { get; set; }
    public string Ddi { get; set; }
}
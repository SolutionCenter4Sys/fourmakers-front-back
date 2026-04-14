using DataTransferObject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class ProcessamentoCurriculoLoteDTO
    {
        public string Id { get; set; }

        public int OrgId { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataInicioProcessamento { get; set; }

        public DateTime? DataFimProcessamento { get; set; }

        public bool Processado { get; set; }

        public int TotalItens { get; set; }

        public int QuantidadeAProcessar { get; set; }

        public int QuantidadeProcessada { get; set; }

        public string ColaboradorCadastrante { get; set; }

        public string Identificadorfila { get; set; }
    }

}
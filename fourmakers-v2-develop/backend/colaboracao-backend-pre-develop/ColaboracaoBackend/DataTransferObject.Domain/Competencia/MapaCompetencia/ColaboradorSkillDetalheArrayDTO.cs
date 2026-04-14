using System;
using System.Linq;
using System.Reflection;

namespace DataTransferObject.Domain.Competencia.MapaCompetencia
{
    public class ColaboradorSkillDetalheArrayDTO : ColaboradorSkillDetalheDTO
    {
        public string[] HardskillsArray { get; set; }
        public string[] SoftSkillsArray { get; set; }
        public string[] MetodologiasArray { get; set; }
        public string[] DominiosNegocioArray { get; set; }
        public string[] IdiomasArray { get; set; }
        public string[] FormacoesArray { get; set; }
        public string[] ClientesArray { get; set; }
        public string[] CidadaniasArray { get; set; }
        public string[] VistosArray { get; set; }
        public string[] PassaportesArray { get; set; }

        //ajuste rápido para trazer a lista de skills em aray, precisa ser melhorado depois
        public ColaboradorSkillDetalheArrayDTO(ColaboradorSkillDetalheDTO dto, string separador)
        {
            foreach (PropertyInfo property in typeof(ColaboradorSkillDetalheDTO).GetProperties())
            {
                property.SetValue(this, property.GetValue(dto));
            }

            if (dto.Hardskills?.Length > 0)
            {
                int i = 0;
            }

            HardskillsArray = dto.Hardskills?.Split(separador).Select(c => c.Trim()).ToArray() ?? Array.Empty<string>();
            Hardskills = dto.Hardskills?.Replace(separador, ",");

            SoftSkillsArray = dto.SoftSkills?.Split(separador).Select(c => c.Trim()).ToArray() ?? Array.Empty<string>();
            SoftSkills = dto.SoftSkills?.Replace(separador, ",");

            MetodologiasArray = dto.Metodologias?.Split(separador).Select(c => c.Trim()).ToArray() ?? Array.Empty<string>();
            Metodologias = dto.Metodologias?.Replace(separador, ",");

            DominiosNegocioArray = dto.DominiosNegocio?.Split(separador).Select(c => c.Trim()).ToArray() ?? Array.Empty<string>();
            DominiosNegocio = dto.DominiosNegocio?.Replace(separador, ",");

            IdiomasArray = dto.Idiomas?.Split(separador).Select(c => c.Trim()).ToArray() ?? Array.Empty<string>();
            Idiomas = dto.Idiomas?.Replace(separador, ",");

            FormacoesArray = dto.Formacoes?.Split(separador).Select(c => c.Trim()).ToArray() ?? Array.Empty<string>();
            Formacoes = dto.Formacoes?.Replace(separador, ",");

            ClientesArray = dto.Clientes?.Split(separador).Select(c => c.Trim()).ToArray() ?? Array.Empty<string>();
            Clientes = dto.Clientes?.Replace(separador, ",");

            CidadaniasArray = dto.Cidadanias?.Split(separador).Select(c => c.Trim()).ToArray() ?? Array.Empty<string>();
            Cidadanias = dto.Cidadanias?.Replace(separador, ",");

            VistosArray = dto.Vistos?.Split(separador).Select(c => c.Trim()).ToArray() ?? Array.Empty<string>();
            Vistos = dto.Vistos?.Replace(separador, ",");

            PassaportesArray = dto.Passaportes?.Split(separador).Select(c => c.Trim()).ToArray() ?? Array.Empty<string>();
            Passaportes = dto.Passaportes?.Replace(separador, ",");
        }
    }
}
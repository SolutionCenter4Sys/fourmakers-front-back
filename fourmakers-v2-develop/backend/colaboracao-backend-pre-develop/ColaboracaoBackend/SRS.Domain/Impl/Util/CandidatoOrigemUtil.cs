using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Vaga;
using System;
using System.Linq;
using System.Collections.Generic;
using DataTransferObject.Domain.Util.Enum;
using Colaboracao.Helper;

namespace SRS.Domain.Impl.Util
{
    public static class CandidatoOrigemUtil
    {
        public static string AtribuirOrigem(List<OrganizacaoCandidatoDTO> organizacoes, string origem)
        {
            origem = ColaboradorBancoTalentos(organizacoes);
            if (String.IsNullOrEmpty(origem))
            {
                var orgAtiva = organizacoes.FirstOrDefault(o => o.AtivoNaOrg == true);
                if (orgAtiva != null)
                    origem = "Colaborador";
                else if (organizacoes.Any(o => o.AtivoNaOrg == false))
                    origem = "Inativo";
            }

            return origem;
        }

        public static string ColaboradorBancoTalentos(List<OrganizacaoCandidatoDTO> organizacoes)
        {
            if (organizacoes.FirstOrDefault(o => o.OrgId == EnumORG.FMU_7.ToInt()) is not null || organizacoes.FirstOrDefault(o => o.OrgId == EnumORG.FOURMAKERS_1.ToInt()) is not null)
            {
                var orgAQualEleEhBancoTalento = organizacoes.FirstOrDefault(o => o.OrgId == EnumORG.FMU_7.ToInt());
                if (orgAQualEleEhBancoTalento is null)
                    orgAQualEleEhBancoTalento = organizacoes.FirstOrDefault(o => o.OrgId == EnumORG.FOURMAKERS_1.ToInt());

                if (orgAQualEleEhBancoTalento.OrgIdTbBancoTalento > 0)
                    return "Banco De Talentos";
            }

            if (organizacoes.FirstOrDefault(o => o.OrgId == EnumORG.FOURSYS_2.ToInt()) is not null)
            {
                var orgAQualEleEhBancoTalento = organizacoes.FirstOrDefault(o => o.OrgId == EnumORG.FOURSYS_2.ToInt());

                if (orgAQualEleEhBancoTalento.CodDiretoria == "BANCO TALENTOS")
                {
                    if (organizacoes.FirstOrDefault(m => m.TipoCadastroBancoDeTalentos == "SRS_LINKEDIN") is not null)
                        return "Linkedin";
                    else
                        return "Banco De Talentos";
                }
            }

            return null;
        }
    }
}


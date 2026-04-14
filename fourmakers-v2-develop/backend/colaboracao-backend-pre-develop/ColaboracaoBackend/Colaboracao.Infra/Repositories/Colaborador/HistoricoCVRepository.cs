using Colaboracao.Core.Interfaces;
using Core.Domain.Colaborador;
using Dapper;
using DataTransferObject.Domain.Colaborador;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class HistoricoCVRepository : IHistoricoCVRepository
    {
        private readonly IDBConnection _dbConnection;
        private readonly int COMPETENCIA_ID = 1;
        private readonly int METODOLOGIA_ID = 3;
        private readonly int DOMINIO_NEGOCIO_ID = 4;
        private readonly int SOFTSKILL_ID = 8;
        private readonly int IDIOMA_ID = 9;
        private readonly int SOBRE_ID = 10;
        private readonly int EXPERIENCIA_ID = 11;
        private readonly int CERTIFICACAO_ID = 12;
        private readonly int ESCOLARIDADE_ID = 13;
        public HistoricoCVRepository(IDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public AtualizacaoCVDTO GetUltimaAtualizacao(string codInternoColaborador)
        {
            var connection = _dbConnection.GetConnection();

            var sql = @"select tohc.descricao, thc.data_criacao from tb_historico_cv thc
                inner join tb_origem_historico_cv tohc on tohc.id = thc.tb_origem_historico_cv_id
                where thc.codigo_interno_colaborador = @CodInternoColaborador order by thc.data_criacao DESC limit 1;";

            return connection.Query<dynamic>(sql, new { CodInternoColaborador = codInternoColaborador }).Select(x => new AtualizacaoCVDTO
            {
                Data = x.data_criacao,
                Origem = x.descricao
            }).FirstOrDefault();
        }

        public void InserirHistoricoCV(string codInternoColaborador, OrigemAlteracaoCVEnum origem, TipoItemCVEnum tipo, long? skillId, long? nivelId, ItemCVEnum itemPerfil)
        {
            var connection = _dbConnection.GetConnection();

            var sql = @"insert into tb_historico_cv (id, codigo_interno_colaborador, tb_item_perfil_id, tb_origem_historico_cv_id, tb_tipo_historico_cv_id, tb_skill_id, tb_nivel_id)
                values (uuid(), @CodInternoColaborador, @ItemPerfil, (select tohc.id from tb_origem_historico_cv tohc where tohc.descricao = @Origem), (select tthc.id from tb_tipo_historico_cv tthc where tthc.descricao = @Tipo), @SkillId, @NivelId);";

            connection.Execute(sql, new { CodInternoColaborador = codInternoColaborador, ItemPerfil = GetItemCVId(itemPerfil), Origem = origem.ToString(), Tipo = tipo.ToString(), SkillId = skillId, NivelId = nivelId });
        }

        public int GetItemCVId(ItemCVEnum itemCv)
        {
            return itemCv switch
            {
                ItemCVEnum.HARDSKILL => COMPETENCIA_ID,
                ItemCVEnum.SOFTSKILL => SOFTSKILL_ID,
                ItemCVEnum.METODOLOGIA => METODOLOGIA_ID,
                ItemCVEnum.DOMINIO_NEGOCIO => DOMINIO_NEGOCIO_ID,
                ItemCVEnum.IDIOMA => IDIOMA_ID,
                ItemCVEnum.SOBRE => SOBRE_ID,
                ItemCVEnum.EXPERIENCIA => EXPERIENCIA_ID,
                ItemCVEnum.CERTIFICACAO => CERTIFICACAO_ID,
                ItemCVEnum.ESCOLARIDADE => ESCOLARIDADE_ID,
                _ => COMPETENCIA_ID,
            };
        }
    }
}
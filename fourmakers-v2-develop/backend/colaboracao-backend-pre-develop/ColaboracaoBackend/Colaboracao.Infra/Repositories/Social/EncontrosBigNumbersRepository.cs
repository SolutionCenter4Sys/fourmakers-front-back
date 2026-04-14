using Colaboracao.Core.Interfaces;
using Core.Domain.Social;
using Dapper;
using DataTransferObject.Domain.Social;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Social
{
    public class EncontrosBigNumbersRepository : IEncontrosBigNumbersRepository
    {
        private readonly IDBConnection _dapperConnection;

        public EncontrosBigNumbersRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<EncontrosBigNumbers> ObterBigNumbers(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var conn = _dapperConnection.GetConnection();

            // Se as datas não forem informadas, assumir valores padrão para trazer tudo
            if (!param.DataInicio.HasValue)
                param.DataInicio = new System.DateTime(2000, 1, 1);
            
            if (!param.DataFim.HasValue)
                param.DataFim = System.DateTime.Now.AddDays(1000);

            var filtros = ObterFiltrosWhere(param);
            var filtrosGestores = ObterFiltrosWhereParaQueryGestores(param);

            var sql = $@"
                -- 1. Agendas(Reunioes) Realizados (agendas com interação)
                SELECT COUNT(tae.id) AS ReunioesRealizados
                FROM tb_agendas_comerciais tae
                INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                WHERE 1 = 1
                {filtros}
                AND DATE(tae.data_agendada) < CURDATE()
                ;

                -- 2. Encontros sem Interação (agendas sem interação)
                SELECT COUNT(*) AS ReunioesSemInteracao
                FROM tb_agendas_comerciais tae
                INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                WHERE NOT EXISTS (SELECT 1 FROM tb_interacoes ti WHERE ti.tb_agendas_comerciais_id = tae.id)
                {filtros}
                ;

                -- 3. Ações em Atraso (tb_interacao_acoes com data_limite < CURDATE e status pendente)
                SELECT COUNT(*) AS TotalAndamento
                FROM tb_interacao_acoes taee
                INNER JOIN tb_interacao_ai tii ON tii.id = taee.tb_interacao_ai_id
                INNER JOIN tb_interacoes ti ON ti.id = tii.tb_interacoes_id
                INNER JOIN tb_agendas_comerciais tae ON tae.id = ti.tb_agendas_comerciais_id
                INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                WHERE taee.tb_status_acoes_id in (1, 2)
                AND taee.data_limite IS NOT NULL
                AND DATE(taee.data_limite) < CURDATE() 
                {filtros}
                ;

                -- 4. Clientes Impactados (distinct clientes com agendas)
                SELECT COUNT(DISTINCT tae.tb_cliente_org_codigo_cliente) AS ClientesImpactados
                FROM tb_agendas_comerciais tae
                INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                WHERE 1 = 1
                {filtros}
                ;

                -- 5. Gestores Impactados (distinct gestores em tb_agenda_convidados)
                SELECT COUNT(DISTINCT p.codigo_colaborador_interno_externo) AS GestoresImpactados
                FROM tb_agenda_convidados p
                INNER JOIN tb_agendas_comerciais tae ON tae.id = p.tb_agendas_comerciais_id
                INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                WHERE p.tipo_codigo = 2
                {filtrosGestores}
                ;

                -- 6. Categorias com Interação (quantidade de categorias distintas em tb_interacoes_categoria_sub com interações na org)
                SELECT COUNT(*) AS CategoriasComInteracao
                FROM (
                    SELECT DISTINCT tca.id
                    FROM tb_interacoes_categoria_sub tcs
                    INNER JOIN tb_categoria_assunto tca ON tca.id = tcs.tb_categoria_assunto_id
                    INNER JOIN tb_subcategoria_assunto tsa ON tsa.tb_categoria_assunto_id = tcs.tb_categoria_assunto_id AND tsa.id = tcs.tb_subcategoria_assunto_id
                    INNER JOIN tb_interacoes ti ON ti.id = tcs.tb_interacoes_id
                    INNER JOIN tb_agendas_comerciais tae ON tae.id = ti.tb_agendas_comerciais_id
                    INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                    WHERE tcs.ativo = 1
                    {filtros}
                    GROUP BY tca.id
                    HAVING COUNT(DISTINCT ti.tb_agendas_comerciais_id) > 0
                ) AS categorias
                ; ";

            using (var multi = await conn.QueryMultipleAsync(sql, new
            {
                orgIdUsuarioLogado,
                DataInicio = param?.DataInicio,
                DataFim = param?.DataFim,
                CodigoCliente = param?.CodigoCliente,
                CodigoColaboradorAgendou = param?.CodigoColaboradorAgendou,
                CodigoGestorExterno = param?.CodigoGestorExterno
            }))
            {
                var encontrosRealizados = await multi.ReadFirstOrDefaultAsync<int?>();
                var encontrosSemInteracao = await multi.ReadFirstOrDefaultAsync<int?>();
                var acoesEmAtraso = await multi.ReadFirstOrDefaultAsync<int?>();
                var clientesImpactados = await multi.ReadFirstOrDefaultAsync<int?>();
                var gestoresImpactados = await multi.ReadFirstOrDefaultAsync<int?>();
                var categoriasComInteracao = await multi.ReadFirstOrDefaultAsync<int?>();

                return new EncontrosBigNumbers
                {
                    ReunioesRealizados = encontrosRealizados ?? 0,
                    ReunioesSemInteracao = encontrosSemInteracao ?? 0,
                    AcoesEmAtraso = acoesEmAtraso ?? 0,
                    ClientesImpactados = clientesImpactados ?? 0,
                    GestoresImpactados = gestoresImpactados ?? 0,
                    CategoriasComInteracao = categoriasComInteracao ?? 0
                };
            }
        }

        public async Task<List<EncontrosBigNumbersCategoria>> ObterBigNumbersCategoria(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var conn = _dapperConnection.GetConnection();

            // Se as datas não forem informadas, assumir valores padrão para trazer tudo
            if (!param.DataInicio.HasValue)
                param.DataInicio = new System.DateTime(2000, 1, 1);
            
            if (!param.DataFim.HasValue)
                param.DataFim = System.DateTime.Now.AddDays(1000);

            var filtros = ObterFiltrosWhere(param);

            var sql = $@"
                -- Vínculos categoria/subcategoria/interação (uma linha em tb_interacoes_categoria_sub = uma ocorrência no CategoriaEmFocoDetalhe)
               SELECT COUNT(DISTINCT tcs.id) AS TotalInteracoes, tcs.tb_categoria_assunto_id AS CategoriaId, tca.descricao AS CategoriaDescricao
                FROM tb_interacoes_categoria_sub tcs
                INNER JOIN tb_categoria_assunto tca ON tca.id = tcs.tb_categoria_assunto_id
                INNER JOIN tb_subcategoria_assunto tsa ON tsa.tb_categoria_assunto_id = tcs.tb_categoria_assunto_id AND tsa.id = tcs.tb_subcategoria_assunto_id
                INNER JOIN tb_interacoes ti ON ti.id = tcs.tb_interacoes_id
                INNER JOIN tb_agendas_comerciais tae ON tae.id = ti.tb_agendas_comerciais_id
                INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                WHERE tcs.ativo = 1
                {filtros}
                GROUP BY tcs.tb_categoria_assunto_id, tca.descricao
                HAVING COUNT(DISTINCT tcs.id) > 0
                ;
            ";

            var resultado = await conn.QueryAsync<EncontrosBigNumbersCategoria>(sql, new
            {
                orgIdUsuarioLogado,
                DataInicio = param?.DataInicio,
                DataFim = param?.DataFim,
                CodigoCliente = param?.CodigoCliente,
                CodigoColaboradorAgendou = param?.CodigoColaboradorAgendou,
                CodigoGestorExterno = param?.CodigoGestorExterno
            });
            return resultado.AsList();
        }

        public async Task<List<EncontrosBigNumbersObjetivo>> ObterBigNumbersObjetivo(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var conn = _dapperConnection.GetConnection();

            // Se as datas não forem informadas, assumir valores padrão para trazer tudo
            if (!param.DataInicio.HasValue)
                param.DataInicio = new System.DateTime(2000, 1, 1);
            
            if (!param.DataFim.HasValue)
                param.DataFim = System.DateTime.Now.AddDays(1000);

            var filtros = ObterFiltrosWhere(param);

            var sql = $@"
                -- Total de agendas por objetivo
                SELECT COUNT(DISTINCT tae.id) AS TotalAgendas, 
                       tao.id AS ObjetivoId, 
                       COALESCE(tao.titulo, 'Objetivo não Definido') AS ObjetivoTitulo,
                       COALESCE(tao.descricao, 'Objetivo não Definido') AS ObjetivoDescricao
                FROM tb_agendas_comerciais tae
                INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                LEFT JOIN tb_agenda_objetivo tao ON tao.id = tae.tb_agenda_objetivo_id AND tao.ativo = 1
                WHERE 1 = 1
                {filtros}
                GROUP BY tao.id, tao.titulo, tao.descricao
                HAVING COUNT(DISTINCT tae.id) > 0
                ORDER BY tao.titulo
                ;
            ";

            var resultado = await conn.QueryAsync<EncontrosBigNumbersObjetivo>(sql, new
            {
                orgIdUsuarioLogado,
                DataInicio = param?.DataInicio,
                DataFim = param?.DataFim,
                CodigoCliente = param?.CodigoCliente,
                CodigoColaboradorAgendou = param?.CodigoColaboradorAgendou,
                CodigoGestorExterno = param?.CodigoGestorExterno
            });
            return resultado.AsList();
        }

        public async Task<List<AgendaRealizadaDetalhe>> AgendaRealizadaDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var conn = _dapperConnection.GetConnection();

            // Se as datas não forem informadas, assumir valores padrão para trazer tudo
            if (!param.DataInicio.HasValue)
                param.DataInicio = new System.DateTime(2000, 1, 1);
            
            if (!param.DataFim.HasValue)
                param.DataFim = System.DateTime.Now.AddDays(1000);

            var filtros = ObterFiltrosWhere(param);

            var sql = $@"
                SELECT tae.id AS AgendaId,
                       tc.nome_completo AS Organizador, 
                       tco.codigo_cliente AS CodigoCliente, 
                       tco.nome_cliente AS NomeCliente, 
                       tae.titulo AS Titulo, 
                       tae.data_agendada AS DataAgendada
                FROM tb_agendas_comerciais tae
                INNER JOIN tb_cliente_org tco ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente AND tco.tb_org_id = @orgIdUsuarioLogado
                LEFT JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = tae.tb_colaborador_codigo_interno_colaborador 
                WHERE DATE(tae.data_agendada) < CURDATE()
                {filtros}
                ORDER BY tae.data_agendada DESC;
            ";

            var agendas = (await conn.QueryAsync<AgendaRealizadaDetalhe>(sql, new
            {
                orgIdUsuarioLogado,
                DataInicio = param?.DataInicio,
                DataFim = param?.DataFim,
                CodigoCliente = param?.CodigoCliente,
                CodigoColaboradorAgendou = param?.CodigoColaboradorAgendou,
                CodigoGestorExterno = param?.CodigoGestorExterno
            })).ToList();

            // Busca os gestores para cada agenda
            foreach (var agenda in agendas)
            {
                agenda.Gestores = await BuscarGestoresCliente(agenda.CodigoCliente, agenda.AgendaId);
            }

            return agendas;
        }
          
        public async Task<List<AgendaSemInteracaoDetalhe>> AgendaSemInteracaoDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var conn = _dapperConnection.GetConnection();

            // Se as datas não forem informadas, assumir valores padrão para trazer tudo
            if (!param.DataInicio.HasValue)
                param.DataInicio = new System.DateTime(2000, 1, 1);
            
            if (!param.DataFim.HasValue)
                param.DataFim = System.DateTime.Now.AddDays(1000);

            var filtros = ObterFiltrosWhere(param);

            var sql = $@"
                SELECT 
                    tae.id AS AgendaId,
                    tae.titulo AS Titulo,
                    tae.data_agendada AS DataAgendada,
                    tco.nome_cliente AS NomeCliente,
                    tae.tb_tipo_agenda_id AS TipoAgendaId,
                    tta.descricao AS TipoAgendaDescricao,
                    tc.nome_completo AS CriadorDaAgenda
                FROM tb_agendas_comerciais tae
                INNER JOIN tb_cliente_org tco 
                    ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente 
                    AND tco.tb_org_id = @orgIdUsuarioLogado
                LEFT JOIN tb_tipo_agenda tta 
                    ON tta.id = tae.tb_tipo_agenda_id
                LEFT JOIN tb_colaborador tc
                    ON tc.codigo_interno_colaborador = tae.tb_colaborador_codigo_interno_colaborador
                WHERE NOT EXISTS (
                    SELECT 1 
                    FROM tb_interacoes ti 
                    WHERE ti.tb_agendas_comerciais_id = tae.id
                )
                {filtros}
                ORDER BY tae.data_agendada DESC;
            ";

            var resultado = await conn.QueryAsync<AgendaSemInteracaoDetalhe>(sql, new
            {
                orgIdUsuarioLogado,
                DataInicio = param?.DataInicio,
                DataFim = param?.DataFim,
                CodigoCliente = param?.CodigoCliente,
                CodigoColaboradorAgendou = param?.CodigoColaboradorAgendou,
                CodigoGestorExterno = param?.CodigoGestorExterno
            });

            return resultado.AsList();
        }

        public async Task<List<ClientesImpactadosDetalhe>> ClientesImpactadosDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var conn = _dapperConnection.GetConnection();

            // Se as datas não forem informadas, assumir valores padrão para trazer tudo
            if (!param.DataInicio.HasValue)
                param.DataInicio = new System.DateTime(2000, 1, 1);
            
            if (!param.DataFim.HasValue)
                param.DataFim = System.DateTime.Now.AddDays(1000);

            var filtros = ObterFiltrosWhere(param);

            var sql = $@"
                SELECT 
                    tco.codigo_cliente AS CodigoCliente,
                    tco.nome_cliente AS NomeCliente,
                    tae.id AS AgendaId,
                    tae.titulo AS Titulo,
                    tae.data_agendada AS DataAgendada,
                    tae.tb_tipo_agenda_id AS TipoAgendaId,
                    tta.descricao AS TipoAgendaDescricao
                FROM tb_cliente_org tco
                INNER JOIN tb_agendas_comerciais tae 
                    ON tae.tb_cliente_org_codigo_cliente = tco.codigo_cliente
                LEFT JOIN tb_tipo_agenda tta 
                    ON tta.id = tae.tb_tipo_agenda_id
                WHERE tco.tb_org_id = @orgIdUsuarioLogado
                {filtros}
                ORDER BY tco.nome_cliente, tae.data_agendada DESC;
            ";

            // Query e agrupamento usando Dapper
            var lookup = new Dictionary<string, ClientesImpactadosDetalhe>();

            await conn.QueryAsync<ClientesImpactadosDetalhe, AgendaDoCliente, ClientesImpactadosDetalhe>(
                sql,
                (cliente, agenda) =>
                {
                    if (!lookup.TryGetValue(cliente.CodigoCliente, out var clienteEntry))
                    {
                        clienteEntry = cliente;
                        clienteEntry.Agendas = new List<AgendaDoCliente>();
                        lookup.Add(cliente.CodigoCliente, clienteEntry);
                    }

                    if (agenda != null)
                    {
                        clienteEntry.Agendas.Add(agenda);
                    }

                    return clienteEntry;
                },
                new
                {
                    orgIdUsuarioLogado,
                    DataInicio = param?.DataInicio,
                    DataFim = param?.DataFim,
                    CodigoCliente = param?.CodigoCliente,
                    CodigoColaboradorAgendou = param?.CodigoColaboradorAgendou,
                    CodigoGestorExterno = param?.CodigoGestorExterno
                },
                splitOn: "AgendaId"
            );

            return lookup.Values.ToList();
        }

        public async Task<List<GestoresImpactadosDetalhe>> GestoresImpactadosDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var conn = _dapperConnection.GetConnection();

            // Se as datas não forem informadas, assumir valores padrão para trazer tudo
            if (!param.DataInicio.HasValue)
                param.DataInicio = new System.DateTime(2000, 1, 1);
            
            if (!param.DataFim.HasValue)
                param.DataFim = System.DateTime.Now.AddDays(1000);

            var filtros = ObterFiltrosWhereParaQueryGestores(param);

            var sql = $@"
                SELECT 
                    tge.cod_gestor_externo AS CodigoGestorExterno,
                    tge.nome AS NomeGestor,
                    tae.id AS AgendaId,
                    tae.titulo AS Titulo,
                    tae.data_agendada AS DataAgendada,
                    tco.codigo_cliente AS CodigoCliente,
                    tco.nome_cliente AS NomeCliente,
                    tae.tb_tipo_agenda_id AS TipoAgendaId,
                    tta.descricao AS TipoAgendaDescricao
                FROM tb_gestor_externo tge
                INNER JOIN tb_agenda_convidados p 
                    ON p.codigo_colaborador_interno_externo = tge.cod_gestor_externo
                INNER JOIN tb_agendas_comerciais tae 
                    ON tae.id = p.tb_agendas_comerciais_id
                INNER JOIN tb_cliente_org tco 
                    ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente 
                    AND tco.tb_org_id = @orgIdUsuarioLogado
                LEFT JOIN tb_tipo_agenda tta 
                    ON tta.id = tae.tb_tipo_agenda_id
                WHERE p.tipo_codigo = 2 
                {filtros}
                ORDER BY tge.nome, tae.data_agendada DESC;
            ";

            // Query e agrupamento usando Dapper
            var lookup = new Dictionary<string, GestoresImpactadosDetalhe>();

            await conn.QueryAsync<GestoresImpactadosDetalhe, AgendaDoGestor, GestoresImpactadosDetalhe>(
                sql,
                (gestor, agenda) =>
                {
                    if (!lookup.TryGetValue(gestor.CodigoGestorExterno, out var gestorEntry))
                    {
                        gestorEntry = gestor;
                        gestorEntry.Agendas = new List<AgendaDoGestor>();
                        lookup.Add(gestor.CodigoGestorExterno, gestorEntry);
                    }

                    if (agenda != null)
                    {
                        gestorEntry.Agendas.Add(agenda);
                    }

                    return gestorEntry;
                },
                new
                {
                    orgIdUsuarioLogado,
                    DataInicio = param?.DataInicio,
                    DataFim = param?.DataFim,
                    CodigoCliente = param?.CodigoCliente,
                    CodigoColaboradorAgendou = param?.CodigoColaboradorAgendou,
                    CodigoGestorExterno = param?.CodigoGestorExterno
                },
                splitOn: "AgendaId"
            );

            return lookup.Values.ToList();
        }

        public async Task<List<CategoriaComInteracaoDetalhe>> CategoriaComInteracaoDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var conn = _dapperConnection.GetConnection();

            // Se as datas não forem informadas, assumir valores padrão para trazer tudo
            if (!param.DataInicio.HasValue)
                param.DataInicio = new System.DateTime(2000, 1, 1);
            
            if (!param.DataFim.HasValue)
                param.DataFim = System.DateTime.Now.AddDays(1000);

            var filtros = ObterFiltrosWhere(param);

            var sql = $@"
                SELECT 
                    tca.id AS CategoriaId,
                    tca.descricao AS CategoriaDescricao,
                    tae.id AS AgendaId,
                    tae.titulo AS Titulo,
                    tae.data_agendada AS DataAgendada,
                    tco.codigo_cliente AS CodigoCliente,
                    tco.nome_cliente AS NomeCliente,
                    tae.tb_tipo_agenda_id AS TipoAgendaId,
                    tta.descricao AS TipoAgendaDescricao,
                    tsa.id AS SubcategoriaId,
                    tsa.descricao AS SubcategoriaDescricao,
                    ti.id AS InteracaoId,
                    ti.titulo_interacao AS TituloInteracao,
                    ti.descricao_interacao AS DescricaoInteracao,
                    tcs.id AS TcsId
                FROM tb_interacoes_categoria_sub tcs
                INNER JOIN tb_categoria_assunto tca 
                    ON tca.id = tcs.tb_categoria_assunto_id
                INNER JOIN tb_subcategoria_assunto tsa 
                    ON tsa.tb_categoria_assunto_id = tcs.tb_categoria_assunto_id 
                    AND tsa.id = tcs.tb_subcategoria_assunto_id
                INNER JOIN tb_interacoes ti 
                    ON ti.id = tcs.tb_interacoes_id
                INNER JOIN tb_agendas_comerciais tae 
                    ON tae.id = ti.tb_agendas_comerciais_id
                INNER JOIN tb_cliente_org tco 
                    ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente 
                    AND tco.tb_org_id = @orgIdUsuarioLogado
                LEFT JOIN tb_tipo_agenda tta 
                    ON tta.id = tae.tb_tipo_agenda_id
                WHERE tcs.ativo = 1
                {filtros}
                ORDER BY tca.descricao, tae.data_agendada DESC, tsa.descricao, ti.id, tcs.id;
            ";

            var linhas = (await conn.QueryAsync<CategoriaComInteracaoSqlRow>(sql, new
            {
                orgIdUsuarioLogado,
                DataInicio = param?.DataInicio,
                DataFim = param?.DataFim,
                CodigoCliente = param?.CodigoCliente,
                CodigoColaboradorAgendou = param?.CodigoColaboradorAgendou,
                CodigoGestorExterno = param?.CodigoGestorExterno
            })).ToList();

            var resultado = new List<CategoriaComInteracaoDetalhe>();

            foreach (var grpCat in linhas.GroupBy(r => r.CategoriaId).OrderBy(g => g.First().CategoriaDescricao))
            {
                var headCat = grpCat.First();
                var categoria = new CategoriaComInteracaoDetalhe
                {
                    CategoriaId = headCat.CategoriaId,
                    CategoriaDescricao = headCat.CategoriaDescricao,
                    Agendas = new List<AgendaDaCategoria>()
                };

                foreach (var grpAg in grpCat.GroupBy(r => r.AgendaId).OrderByDescending(g => g.First().DataAgendada))
                {
                    var headAg = grpAg.First();
                    var subIds = grpAg.Select(r => r.SubcategoriaId).Distinct().ToList();

                    var agenda = new AgendaDaCategoria
                    {
                        AgendaId = headAg.AgendaId,
                        Titulo = headAg.Titulo,
                        DataAgendada = headAg.DataAgendada,
                        CodigoCliente = headAg.CodigoCliente,
                        NomeCliente = headAg.NomeCliente,
                        TipoAgendaId = headAg.TipoAgendaId,
                        TipoAgendaDescricao = headAg.TipoAgendaDescricao,
                        SubcategoriaId = subIds.Count == 1 ? subIds[0] : (int?)null,
                        SubcategoriaDescricao = subIds.Count == 1 ? headAg.SubcategoriaDescricao : null,
                        Interacoes = grpAg
                            .GroupBy(r => r.TcsId)
                            .Select(g =>
                            {
                                var r = g.First();
                                return new InteracaoDaCategoria
                                {
                                    InteracaoId = r.InteracaoId,
                                    TituloInteracao = r.TituloInteracao,
                                    DescricaoInteracao = r.DescricaoInteracao,
                                    SubcategoriaId = r.SubcategoriaId,
                                    SubcategoriaDescricao = r.SubcategoriaDescricao
                                };
                            })
                            .OrderBy(i => i.SubcategoriaDescricao)
                            .ThenBy(i => i.InteracaoId)
                            .ToList()
                    };

                    categoria.Agendas.Add(agenda);
                }

                resultado.Add(categoria);
            }

            return resultado.Where(c => c.Agendas != null && c.Agendas.Any()).ToList();
        }

        public async Task<List<ObjetivosAgendaDetalhe>> ObjetivosAgendaDetalhe(string objetivoIds, EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var conn = _dapperConnection.GetConnection();

            // Garantir que param não seja null
            if (param == null)
                param = new EncontrosBigNumbersParam();

            // Se as datas não forem informadas, assumir valores padrão para trazer tudo
            if (!param.DataInicio.HasValue)
                param.DataInicio = new System.DateTime(2000, 1, 1);
            
            if (!param.DataFim.HasValue)
                param.DataFim = System.DateTime.Now.AddDays(1000);

            var filtros = ObterFiltrosWhere(param);
            
            // Adicionar filtro específico de objetivo(s) se fornecido
            if (!string.IsNullOrWhiteSpace(objetivoIds))
            {
                // Converter string separada por vírgulas em lista de IDs
                var ids = objetivoIds.Split(',')
                    .Select(id => id.Trim())
                    .Where(id => !string.IsNullOrWhiteSpace(id) && int.TryParse(id, out _))
                    .Select(id => int.Parse(id))
                    .ToList();

                if (ids.Any())
                {
                    // Verificar se contém o ID 0 (significa trazer também os NULL)
                    if (ids.Contains(0))
                    {
                        // Remover o 0 da lista de IDs válidos
                        var idsValidos = ids.Where(id => id > 0).ToList();
                        
                        if (idsValidos.Any())
                        {
                            // Trazer os IDs especificados E os NULL
                            var idsString = string.Join(",", idsValidos);
                            filtros += $" AND (tao.id IN ({idsString}) OR tao.id IS NULL) ";
                        }
                        else
                        {
                            // Apenas 0 foi informado, trazer apenas NULL
                            filtros += " AND tao.id IS NULL ";
                        }
                    }
                    else
                    {
                        // Trazer apenas os IDs especificados (sem NULL)
                        var idsString = string.Join(",", ids);
                        filtros += $" AND tao.id IN ({idsString}) ";
                    }
                }
                else
                {
                    // Se a string foi fornecida mas não tem IDs válidos, buscar objetivos nulos
                    filtros += " AND tao.id IS NULL ";
                }
            }

            var sql = $@"
                SELECT 
                    tao.id AS ObjetivoId,
                    COALESCE(tao.titulo, 'Objetivo não Definido') AS ObjetivoTitulo,
                    COALESCE(tao.descricao, 'Objetivo não Definido') AS ObjetivoDescricao,
                    tae.id AS AgendaId,
                    tae.titulo AS Titulo,
                    tae.data_agendada AS DataAgendada,
                    tco.codigo_cliente AS CodigoCliente,
                    tco.nome_cliente AS NomeCliente,
                    tae.tb_tipo_agenda_id AS TipoAgendaId,
                    tta.descricao AS TipoAgendaDescricao,
                    tc.nome_completo AS Organizador
                FROM tb_agendas_comerciais tae  
                LEFT JOIN tb_agenda_objetivo tao
                    ON tae.tb_agenda_objetivo_id = tao.id AND tao.ativo = 1
                INNER JOIN tb_cliente_org tco 
                    ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente 
                    AND tco.tb_org_id = @orgIdUsuarioLogado
                LEFT JOIN tb_tipo_agenda tta 
                    ON tta.id = tae.tb_tipo_agenda_id
                LEFT JOIN tb_colaborador tc 
                    ON tc.codigo_interno_colaborador = tae.tb_colaborador_codigo_interno_colaborador
                WHERE 1 = 1
                {filtros}
                ORDER BY tao.titulo, tae.data_agendada DESC;
            ";

            // Query e agrupamento usando Dapper
            var lookup = new Dictionary<int, ObjetivosAgendaDetalhe>();

            await conn.QueryAsync<ObjetivosAgendaDetalhe, AgendaDoObjetivo, ObjetivosAgendaDetalhe>(
                sql,
                (objetivo, agenda) =>
                {
                    // Gerenciar objetivo
                    if (!lookup.TryGetValue(objetivo.ObjetivoId, out var objetivoEntry))
                    {
                        objetivoEntry = objetivo;
                        objetivoEntry.Agendas = new List<AgendaDoObjetivo>();
                        lookup.Add(objetivo.ObjetivoId, objetivoEntry);
                    }

                    // Gerenciar agenda (evitar duplicação DENTRO DO OBJETIVO)
                    if (agenda != null)
                    {
                        var agendaExistente = objetivoEntry.Agendas.FirstOrDefault(a => a.AgendaId == agenda.AgendaId);
                        
                        if (agendaExistente == null)
                        {
                            objetivoEntry.Agendas.Add(agenda);
                        }
                    }

                    return objetivoEntry;
                },
                new
                {
                    orgIdUsuarioLogado,
                    DataInicio = param?.DataInicio,
                    DataFim = param?.DataFim,
                    CodigoCliente = param?.CodigoCliente,
                    CodigoColaboradorAgendou = param?.CodigoColaboradorAgendou,
                    CodigoGestorExterno = param?.CodigoGestorExterno
                },
                splitOn: "AgendaId"
            );

            // Retornar todos os objetivos que têm pelo menos uma agenda
            return lookup.Values.Where(o => o.Agendas != null && o.Agendas.Any()).ToList();
        }

        public async Task<List<AcoesEmAtrasoDetalhe>> AcoesEmAtrasoDetalhe(EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var conn = _dapperConnection.GetConnection();

            // Se as datas não forem informadas, assumir valores padrão para trazer tudo
            if (!param.DataInicio.HasValue)
                param.DataInicio = new System.DateTime(2000, 1, 1);
            
            if (!param.DataFim.HasValue)
                param.DataFim = System.DateTime.Now.AddDays(1000);

            var filtros = ObterFiltrosWhere(param);

            var sql = $@"
                SELECT 
                    tst.id AS StatusAcaoId,
                    tst.descricao AS StatusDescricao,
                    tae.id AS AgendaId,
                    tae.titulo AS Titulo,
                    tae.data_agendada AS DataAgendada,
                    tco.codigo_cliente AS CodigoCliente,
                    tco.nome_cliente AS NomeCliente,
                    tae.tb_tipo_agenda_id AS TipoAgendaId,
                    tta.descricao AS TipoAgendaDescricao,
                    tbi.id AS AcaoId,
                    tbi.data_limite AS DataLimiteAcao,
                    tbi.texto AS DescricaoAcao,
                    tc.nome_completo AS ResponsavelAcao
                FROM tb_status_acoes tst
                INNER JOIN tb_interacao_acoes tbi 
                    ON tbi.tb_status_acoes_id = tst.id
                INNER JOIN tb_interacao_ai tii 
                    ON tii.id = tbi.tb_interacao_ai_id
                INNER JOIN tb_interacoes ti 
                    ON ti.id = tii.tb_interacoes_id
                INNER JOIN tb_agendas_comerciais tae 
                    ON tae.id = ti.tb_agendas_comerciais_id
                INNER JOIN tb_cliente_org tco 
                    ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente 
                    AND tco.tb_org_id = @orgIdUsuarioLogado
                LEFT JOIN tb_tipo_agenda tta 
                    ON tta.id = tae.tb_tipo_agenda_id
                LEFT JOIN tb_colaborador tc 
                    ON tc.codigo_interno_colaborador = tbi.tb_colaborador_codigo_interno_colaborador
                WHERE tbi.tb_status_acoes_id IN (1, 2)
                  AND tbi.data_limite IS NOT NULL
                  AND DATE(tbi.data_limite) < CURDATE()
                {filtros}
                ORDER BY tst.descricao, tae.data_agendada DESC;
            ";

            // Query e agrupamento usando Dapper
            var lookup = new Dictionary<int, AcoesEmAtrasoDetalhe>();

            await conn.QueryAsync<AcoesEmAtrasoDetalhe, AgendaComAcao, AcoesEmAtrasoDetalhe>(
                sql,
                (status, agenda) =>
                {
                    if (!lookup.TryGetValue(status.StatusAcaoId, out var statusEntry))
                    {
                        statusEntry = status;
                        statusEntry.Agendas = new List<AgendaComAcao>();
                        lookup.Add(status.StatusAcaoId, statusEntry);
                    }

                    if (agenda != null)
                    {
                        statusEntry.Agendas.Add(agenda);
                    }

                    return statusEntry;
                },
                new
                {
                    orgIdUsuarioLogado,
                    DataInicio = param?.DataInicio,
                    DataFim = param?.DataFim,
                    CodigoCliente = param?.CodigoCliente,
                    CodigoColaboradorAgendou = param?.CodigoColaboradorAgendou,
                    CodigoGestorExterno = param?.CodigoGestorExterno
                },
                splitOn: "AgendaId"
            );

            return lookup.Values.ToList();
        }

        public async Task<CategoriaEmFocoDetalhe> CategoriaEmFocoDetalhe(int categoriaId, EncontrosBigNumbersParam param, int orgIdUsuarioLogado)
        {
            var conn = _dapperConnection.GetConnection();

            // Se as datas não forem informadas, assumir valores padrão para trazer tudo
            if (!param.DataInicio.HasValue)
                param.DataInicio = new System.DateTime(2000, 1, 1);
            
            if (!param.DataFim.HasValue)
                param.DataFim = System.DateTime.Now.AddDays(1000);

            var filtros = ObterFiltrosWhere(param);

            var sql = $@"
                SELECT 
                    tca.id AS CategoriaId,
                    tca.descricao AS CategoriaDescricao,
                    tsa.id AS SubcategoriaId,
                    tsa.descricao AS SubcategoriaDescricao,
                    tae.id AS AgendaId,
                    tae.titulo AS Titulo,
                    tae.data_agendada AS DataAgendada,
                    tco.codigo_cliente AS CodigoCliente,
                    tco.nome_cliente AS NomeCliente,
                    tc.nome_completo AS Organizador,
                    ti.id AS InteracaoId,
                    ti.titulo_interacao AS TituloInteracao,
                    ti.descricao_interacao AS DescricaoInteracao
                FROM tb_categoria_assunto tca
                INNER JOIN tb_interacoes_categoria_sub tcs 
                    ON tcs.tb_categoria_assunto_id = tca.id
                INNER JOIN tb_subcategoria_assunto tsa 
                    ON tsa.tb_categoria_assunto_id = tcs.tb_categoria_assunto_id 
                    AND tsa.id = tcs.tb_subcategoria_assunto_id
                INNER JOIN tb_interacoes ti 
                    ON ti.id = tcs.tb_interacoes_id
                INNER JOIN tb_agendas_comerciais tae 
                    ON tae.id = ti.tb_agendas_comerciais_id
                INNER JOIN tb_cliente_org tco 
                    ON tco.codigo_cliente = tae.tb_cliente_org_codigo_cliente 
                    AND tco.tb_org_id = @orgIdUsuarioLogado
                LEFT JOIN tb_colaborador tc 
                    ON tc.codigo_interno_colaborador = tae.tb_colaborador_codigo_interno_colaborador
                WHERE tcs.ativo = 1
                  AND tca.id = @categoriaId
                {filtros}
                ORDER BY tsa.descricao, tae.data_agendada DESC, ti.id;
            ";

            var linhas = (await conn.QueryAsync<CategoriaEmFocoSqlRow>(sql, new
            {
                categoriaId,
                orgIdUsuarioLogado,
                DataInicio = param?.DataInicio,
                DataFim = param?.DataFim,
                CodigoCliente = param?.CodigoCliente,
                CodigoColaboradorAgendou = param?.CodigoColaboradorAgendou,
                CodigoGestorExterno = param?.CodigoGestorExterno
            })).ToList();

            if (linhas.Count == 0)
                return null;

            var cab = linhas[0];
            var resultado = new CategoriaEmFocoDetalhe
            {
                CategoriaId = cab.CategoriaId,
                CategoriaDescricao = cab.CategoriaDescricao,
                Subcategorias = new List<SubcategoriaComAgendas>()
            };

            foreach (var grpSub in linhas.GroupBy(r => r.SubcategoriaId).OrderBy(g => g.First().SubcategoriaDescricao))
            {
                var headSub = grpSub.First();
                var sub = new SubcategoriaComAgendas
                {
                    SubcategoriaId = headSub.SubcategoriaId,
                    SubcategoriaDescricao = headSub.SubcategoriaDescricao,
                    Agendas = new List<AgendaDaSubcategoria>()
                };

                foreach (var grpAg in grpSub.GroupBy(r => r.AgendaId).OrderByDescending(g => g.First().DataAgendada))
                {
                    var headAg = grpAg.First();
                    var agenda = new AgendaDaSubcategoria
                    {
                        AgendaId = headAg.AgendaId,
                        Titulo = headAg.Titulo,
                        DataAgendada = headAg.DataAgendada,
                        CodigoCliente = headAg.CodigoCliente,
                        NomeCliente = headAg.NomeCliente,
                        Organizador = headAg.Organizador,
                        Interacoes = grpAg
                            .GroupBy(x => x.InteracaoId)
                            .Select(g => new InteracaoDaAgenda
                            {
                                InteracaoId = g.Key,
                                TituloInteracao = g.First().TituloInteracao,
                                DescricaoInteracao = g.First().DescricaoInteracao
                            })
                            .OrderBy(i => i.InteracaoId)
                            .ToList()
                    };
                    sub.Agendas.Add(agenda);
                }

                resultado.Subcategorias.Add(sub);
            }

            foreach (var subcategoria in resultado.Subcategorias)
            {
                foreach (var agenda in subcategoria.Agendas)
                {
                    agenda.Gestores = await BuscarGestoresDaAgenda(agenda.AgendaId);
                }
            }

            return resultado;
        }

        private async Task<List<GestorDaAgenda>> BuscarGestoresDaAgenda(int agendaId)
        {
            var conn = _dapperConnection.GetConnection();

            var sql = @"
                SELECT 
                    tge.cod_gestor_externo AS CodigoGestorExterno,
                    tge.nome AS Nome
                FROM tb_gestor_externo tge 
                INNER JOIN tb_agenda_convidados tep 
                    ON tep.codigo_colaborador_interno_externo = tge.cod_gestor_externo 
                    AND tep.tb_agendas_comerciais_id = @agendaId
                WHERE tge.cod_gestor_externo IS NOT NULL
                AND tep.tipo_codigo = 2;
            ";

            return (await conn.QueryAsync<GestorDaAgenda>(sql, new { agendaId })).AsList();
        }

        private async Task<List<GestorClienteDetalhe>> BuscarGestoresCliente(string clienteId, int agendaId)
        {
            var conn = _dapperConnection.GetConnection();

            return (await conn.QueryAsync<GestorClienteDetalhe>(@"
                SELECT tge.cod_gestor_externo AS CodigoGestorExterno,
                       tge.nome AS Nome
                FROM tb_gestor_externo tge 
                INNER JOIN tb_agenda_convidados tep 
                    ON tep.codigo_colaborador_interno_externo = tge.cod_gestor_externo 
                    AND tep.tb_agendas_comerciais_id = @agendaId
                WHERE tge.codigo_cliente = @clienteId AND tep.tipo_codigo = 2 ", 
                new { clienteId, agendaId })).ToList();
        }

        private static string ObterFiltrosWhere(EncontrosBigNumbersParam param)
        {
            var filtros = new List<string>();

            // Filtro por período (data início e data fim)
            if (param?.DataInicio.HasValue == true)
                filtros.Add(" AND DATE(tae.data_agendada) >= @DataInicio ");

            if (param?.DataFim.HasValue == true)
                filtros.Add(" AND DATE(tae.data_agendada) <= @DataFim ");

            // Filtro por cliente
            if (!string.IsNullOrWhiteSpace(param?.CodigoCliente))
                filtros.Add(" AND tae.tb_cliente_org_codigo_cliente = @CodigoCliente ");

            // Filtro por quem agendou
            if (!string.IsNullOrWhiteSpace(param?.CodigoColaboradorAgendou))
                filtros.Add(" AND tae.tb_colaborador_codigo_interno_colaborador = @CodigoColaboradorAgendou ");

            // Filtro por gestor externo
            if (!string.IsNullOrWhiteSpace(param?.CodigoGestorExterno))
                filtros.Add(" AND EXISTS (SELECT 1 FROM tb_agenda_convidados tap WHERE tap.tb_agendas_comerciais_id = tae.id AND tap.tipo_codigo = 2 AND tap.codigo_colaborador_interno_externo = @CodigoGestorExterno) ");

            return filtros.Count > 0 ? string.Join(" ", filtros) : "";
        }

        private static string ObterFiltrosWhereParaQueryGestores(EncontrosBigNumbersParam param)
        {
            var filtros = new List<string>();

            // Filtro por período (data início e data fim)
            if (param?.DataInicio.HasValue == true)
                filtros.Add(" AND DATE(tae.data_agendada) >= @DataInicio ");

            if (param?.DataFim.HasValue == true)
                filtros.Add(" AND DATE(tae.data_agendada) <= @DataFim ");

            // Filtro por cliente
            if (!string.IsNullOrWhiteSpace(param?.CodigoCliente))
                filtros.Add(" AND tae.tb_cliente_org_codigo_cliente = @CodigoCliente ");

            // Filtro por quem agendou
            if (!string.IsNullOrWhiteSpace(param?.CodigoColaboradorAgendou))
                filtros.Add(" AND tae.tb_colaborador_codigo_interno_colaborador = @CodigoColaboradorAgendou ");

            // Filtro por gestor externo - DIRETO no alias 'p' (não usa EXISTS)
            if (!string.IsNullOrWhiteSpace(param?.CodigoGestorExterno))
                filtros.Add(" AND p.tb_gestor_externo_cod_gestor_externo = @CodigoGestorExterno ");

            return filtros.Count > 0 ? string.Join(" ", filtros) : "";
        }


        public async Task<AgendaVersaoDTO> AtualizaVersaoApp(string descricao, string versao)
        {
            var conn = _dapperConnection.GetConnection();

            var sql = @"
                INSERT INTO tb_agenda_versao (descricao, versao)
                VALUES (@descricao, @versao);
                
                SELECT 
                    id AS Id,
                    descricao AS Descricao,
                    versao AS Versao,
                    data_atualizacao AS DataAtualizacao
                FROM tb_agenda_versao
                WHERE id = LAST_INSERT_ID();
            ";

            var resultado = await conn.QueryFirstOrDefaultAsync<AgendaVersaoDTO>(sql, new { descricao, versao });
            return resultado;
        }

        public async Task<AgendaVersaoDTO> ListaVersaoApp()
        {
            var conn = _dapperConnection.GetConnection();

            var sql = @"
                SELECT 
                    id AS Id,
                    descricao AS Descricao,
                    versao AS Versao,
                    data_atualizacao AS DataAtualizacao
                FROM tb_agenda_versao
                ORDER BY data_atualizacao DESC
                LIMIT 1;
            ";

            var resultado = await conn.QueryFirstOrDefaultAsync<AgendaVersaoDTO>(sql);
            return resultado;
        }

    }
}

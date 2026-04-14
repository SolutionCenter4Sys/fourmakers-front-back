-- ####################################################################
-- INSERTS DE DADOS (VERSÃO UUID)
-- CORREÇÃO FINAL: Garante que os dados para Lowcode, Mainframe e Gerentes/Diretores
--                 sejam extraídos corretamente do CSV original.
-- ####################################################################

-- 1. LIMPEZA DAS TABELAS
SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE tb_equipamento_padrao_mapeamento;
TRUNCATE TABLE tb_equipamento_padrao_cargo_funcao;
TRUNCATE TABLE tb_equipamento_padrao_grupo_area;
TRUNCATE TABLE tb_equipamento_padrao_especificacao;
TRUNCATE TABLE tb_equipamento_padrao_categoria_requisito;
TRUNCATE TABLE tb_equipamento_padrao_upgrade_customizado;
TRUNCATE TABLE tb_equipamento_padrao_catalogo_modelo;
SET FOREIGN_KEY_CHECKS = 1;

-- 2. DEFINIÇÃO DE VARIÁVEIS UUID PARA OS DICIONÁRIOS

-- A. Grupos de Área
SET @grupo_administrativo = UUID();
SET @grupo_desenvolvedor = UUID();

-- B. Categorias
SET @cat_padrao_intel = UUID();
SET @cat_segunda_opcao_amd = UUID();
SET @cat_opcao_macbook = UUID();

-- C. Cargos (APENAS OS SOLICITADOS)
-- Grupo: Administrativo
SET @cargo_analistanegocios = UUID();
SET @cargo_gerentedeprojetos = UUID();
SET @cargo_po_sm_agile = UUID();
SET @cargo_design_motion = UUID();
SET @cargo_corporativocsc = UUID();
SET @cargo_gerente_diretores = UUID(); -- Para 'Gerentes e Diretores de Areas'
-- Grupo: Desenvolvedor
SET @cargo_frontend = UUID();
SET @cargo_backend = UUID();
SET @cargo_fullstack = UUID();
SET @cargo_lowcode = UUID(); -- Para 'Lowc Ode'
SET @cargo_mainframe = UUID(); -- Para 'Ma inframe'
SET @cargo_mobileios = UUID();
SET @cargo_mobileandroid = UUID();
SET @cargo_rpa = UUID();
SET @cargo_qa_automacao = UUID();
SET @cargo_data_analytics = UUID();
SET @cargo_arquiteto = UUID();
SET @cargo_ai = UUID(); -- Para 'AL' do CSV

-- D. Especificações (Baseadas nos dados únicos do CSV)
SET @spec_note_win_i5_int_16g_s256g = UUID(); -- Analista Negócios, Corporativo CSC, AI(AL) - Padrão
SET @spec_note_win_i7_int_16g_s256g = UUID(); -- Gerente Projetos, PO/SM - Padrão
SET @spec_mac_macos_m_int_32g_s512g = UUID(); -- Design/Motion, Mobile Android - Padrão/Opção
SET @spec_note_win_ult_rtx_64g_s512g = UUID(); -- Design/Motion - Secundário
SET @spec_note_win_r5_int_16g_s256g = UUID(); -- Analista Negócios, Corporativo CSC, AI(AL) - Secundário
SET @spec_note_win_r7_int_16g_s256g = UUID(); -- Gerente Projetos, PO/SM - Secundário
SET @spec_note_win_r7_int_32g_s256g = UUID(); -- Frontend, Backend, Fullstack, RPA, QA, Data/Analytics, Arquiteto, Lowcode, Mainframe - Secundário
SET @spec_note_win_i7_int_32g_s256g = UUID(); -- Frontend, Backend, Fullstack, RPA, QA, Data/Analytics, Arquiteto, Lowcode, Mainframe, Mobile IOS, Mobile Android - Padrão / Secundário
SET @spec_mac_ios_m_int_32g_s512g = UUID();   -- Mobile IOS - Padrão/Opção

-- E. Upgrades (Baseados nos dados únicos do CSV)
SET @upg_memate4tb = UUID();                  -- Mobile IOS
SET @upg_memate8tb = UUID();                  -- (Não usado no CSV original)
SET @upg_notei7ottgenssd512gb = UUID();      -- Analista Negocios
SET @upg_notei7ultgenssd512gbmem32gb = UUID(); -- Corporativo CSC, AI(AL)
SET @upg_ssd1tbmacbookpromax = UUID();        -- Design/Motion
SET @upg_ssd512gb = UUID();                  -- Gerente Projetos, PO/SM, RPA, QA, Arquiteto, Lowcode, Mainframe
SET @upg_ssd512gbmem32gb = UUID();            -- Frontend, Backend, Fullstack, Data/Analytics

-- F. Modelos (Baseados nos dados únicos do CSV)
SET @mod_apple_mackbookpro = UUID();
SET @mod_dell_latitude3450 = UUID();
SET @mod_lenovo_thinkpadp14 = UUID();
SET @mod_lenovo_thinkpadt14 = UUID();

-- 3. INSERTS NOS DICIONÁRIOS

-- A. tb_equipamento_padrao_grupo_area
INSERT INTO tb_equipamento_padrao_grupo_area (id, nome_grupo) VALUES
(@grupo_administrativo, 'Administrativo'),
(@grupo_desenvolvedor, 'Desenvolvedor');

-- B. tb_equipamento_padrao_categoria_requisito
INSERT INTO tb_equipamento_padrao_categoria_requisito (id, categoria_nome, descricao) VALUES
(@cat_padrao_intel, 'PADRAO_INTEL', 'Requisito Padrão (Geralmente Intel ou Plataforma Principal)'),
(@cat_segunda_opcao_amd, 'SEGUNDA_OPCAO_AMD', 'Segunda Opção de Equipamento (Geralmente AMD ou Plataforma Alternativa)'),
(@cat_opcao_macbook, 'OPCAO_MACBOOK', 'Opção Padrão para perfis que utilizam Apple/MacOS');

-- C. tb_equipamento_padrao_cargo_funcao (SOMENTE OS CARGOS SOLICITADOS COM GRAFIA CORRETA)
INSERT INTO tb_equipamento_padrao_cargo_funcao (id, nome_cargo_funcao, tb_equipamento_padrao_grupo_area_id) VALUES
-- Grupo: Administrativo
(@cargo_analistanegocios, 'Analista de Negocios', @grupo_administrativo),
(@cargo_gerentedeprojetos, 'Gerente de Projetos', @grupo_administrativo),
(@cargo_po_sm_agile, 'PO/SM/Agile Master', @grupo_administrativo),
(@cargo_design_motion, 'Design/Motion', @grupo_administrativo),
(@cargo_corporativocsc, 'Corporativo CSC', @grupo_administrativo),
(@cargo_gerente_diretores, 'Gerentes e Diretores de Areas', @grupo_administrativo), -- Grafia exata

-- Grupo: Desenvolvedor
(@cargo_frontend, 'Frontend', @grupo_desenvolvedor),
(@cargo_backend, 'Backend', @grupo_desenvolvedor),
(@cargo_fullstack, 'Fullstack', @grupo_desenvolvedor),
(@cargo_lowcode, 'Lowc Ode', @grupo_desenvolvedor), -- Grafia exata
(@cargo_mainframe, 'Ma inframe', @grupo_desenvolvedor), -- Grafia exata
(@cargo_mobileios, 'Mobile IOS', @grupo_desenvolvedor),
(@cargo_mobileandroid, 'Mobile Android', @grupo_desenvolvedor),
(@cargo_rpa, 'RPA', @grupo_desenvolvedor),
(@cargo_qa_automacao, 'QA/Automacao', @grupo_desenvolvedor),
(@cargo_data_analytics, 'Data/Analytics Platform', @grupo_desenvolvedor),
(@cargo_arquiteto, 'Arquiteto', @grupo_desenvolvedor),
(@cargo_ai, 'AI', @grupo_desenvolvedor);


-- D. tb_equipamento_padrao_especificacao
INSERT INTO tb_equipamento_padrao_especificacao (id, tipo_equipamento, sistema_operacional, cpu_geracao, gpu, memoria_ram, armazenamento_disco) VALUES
(@spec_note_win_i5_int_16g_s256g, 'NOTEBOOK', 'WINDOWS11PRO', 'INTELI5', 'INTEGRADA', '16GB', 'SSD256GB'),
(@spec_note_win_i7_int_16g_s256g, 'NOTEBOOK', 'WINDOWS11PRO', 'INTELI7', 'INTEGRADA', '16GB', 'SSD256GB'),
(@spec_mac_macos_m_int_32g_s512g, 'MACBOOK', 'MACOS', 'APPLESILICONM', 'INTEGRADA', '32GB', 'SSD512GB'),
(@spec_note_win_ult_rtx_64g_s512g, 'NOTEBOOK', 'WINDOWS11PRO', 'INTELCOREULTRA', 'NVIDEARTXDEDICADA', '64GB', 'SSD512GB'),
(@spec_note_win_r5_int_16g_s256g, 'NOTEBOOK', 'WINDOWS11PRO', 'AMDRYZEN5', 'INTEGRADA', '16GB', 'SD256GB'),
(@spec_note_win_r7_int_16g_s256g, 'NOTEBOOK', 'WINDOWS11PRO', 'AMDRYZEN7', 'INTEGRADA', '16GB', 'SD256GB'),
(@spec_note_win_r7_int_32g_s256g, 'NOTEBOOK', 'WINDOWS11PRO', 'AMDRYZEN7', 'INTEGRADA', '32GB', 'SD256GB'),
(@spec_note_win_i7_int_32g_s256g, 'NOTEBOOK', 'WINDOWS11PRO', 'INTELI7', 'INTEGRADA', '32GB', 'SSD256GB'),
(@spec_mac_ios_m_int_32g_s512g, 'MACBOOK', 'IOS', 'APPLESILICONM', 'INTEGRADA', '32GB', 'SSD512GB');

-- E. tb_equipamento_padrao_upgrade_customizado
INSERT INTO tb_equipamento_padrao_upgrade_customizado (id, descricao_upgrade) VALUES
(@upg_memate4tb, 'MEMATÉ4TB'),
(@upg_memate8tb, 'MEMATÉ8TB'),
(@upg_notei7ottgenssd512gb, 'NOTEBOOKI7OTTGENSSD512GB'),
(@upg_notei7ultgenssd512gbmem32gb, 'NOTEBOOKI7ULTGENSSD512GBMEM32GB'),
(@upg_ssd1tbmacbookpromax, 'SSD1TBMACBOOKPROMAX'),
(@upg_ssd512gb, 'SSD512GB'),
(@upg_ssd512gbmem32gb, 'SSD512GBMEM32GB');

-- F. tb_equipamento_padrao_catalogo_modelo
INSERT INTO tb_equipamento_padrao_catalogo_modelo (id, marca, nome_modelo, linha_produto) VALUES
(@mod_apple_mackbookpro, 'APPLE', 'MACKBOOKPRO', 'APPLE_SILICON'),
(@mod_dell_latitude3450, 'DELL', 'LATITUDE3450', 'INTEL'),
(@mod_lenovo_thinkpadp14, 'LENOVO', 'THINKPADP14', 'AMD'),
(@mod_lenovo_thinkpadt14, 'LENOVO', 'THINKPADT14', 'INTEL');

-- 4. INSERTS NA TABELA DE MAPEAMENTO (Com dados corretos do CSV para Lowcode, Mainframe, Gerentes)

-- Analista Negocios
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_analistanegocios, @cat_padrao_intel, @spec_note_win_i5_int_16g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_notei7ottgenssd512gb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_analistanegocios, @cat_segunda_opcao_amd, @spec_note_win_r5_int_16g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- Gerente de Projetos
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_gerentedeprojetos, @cat_padrao_intel, @spec_note_win_i7_int_16g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_ssd512gb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_gerentedeprojetos, @cat_segunda_opcao_amd, @spec_note_win_r7_int_16g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- PO/SM/Agile Master
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_po_sm_agile, @cat_padrao_intel, @spec_note_win_i7_int_16g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_ssd512gb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_po_sm_agile, @cat_segunda_opcao_amd, @spec_note_win_r7_int_16g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- Design/ Motion
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_design_motion, @cat_opcao_macbook, @spec_mac_macos_m_int_32g_s512g, @mod_apple_mackbookpro, NULL, NULL, NULL, @upg_ssd1tbmacbookpromax);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_design_motion, @cat_segunda_opcao_amd, @spec_note_win_ult_rtx_64g_s512g, NULL, NULL, NULL, NULL, NULL);

-- Corporativo CSC
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_corporativocsc, @cat_padrao_intel, @spec_note_win_i5_int_16g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_notei7ultgenssd512gbmem32gb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_corporativocsc, @cat_segunda_opcao_amd, @spec_note_win_r5_int_16g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- Gerentes e Diretores de Areas (Dados do CSV)
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_gerente_diretores, @cat_padrao_intel, @spec_note_win_i7_int_16g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_ssd512gb); -- Mesma spec/upg de Gerente de Projetos
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_gerente_diretores, @cat_segunda_opcao_amd, @spec_note_win_r7_int_16g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL); -- Mesma spec/upg de Gerente de Projetos

-- AI (Mapeado dos dados do 'AL' no CSV)
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_ai, @cat_padrao_intel, @spec_note_win_i5_int_16g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_notei7ultgenssd512gbmem32gb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_ai, @cat_segunda_opcao_amd, @spec_note_win_r5_int_16g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- Frontend
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_frontend, @cat_padrao_intel, @spec_note_win_i7_int_32g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_ssd512gbmem32gb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_frontend, @cat_segunda_opcao_amd, @spec_note_win_r7_int_32g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- Backend
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_backend, @cat_padrao_intel, @spec_note_win_i7_int_32g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_ssd512gbmem32gb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_backend, @cat_segunda_opcao_amd, @spec_note_win_r7_int_32g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- Fullstack
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_fullstack, @cat_padrao_intel, @spec_note_win_i7_int_32g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_ssd512gbmem32gb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_fullstack, @cat_segunda_opcao_amd, @spec_note_win_r7_int_32g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- Lowc Ode (Dados do CSV)
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_lowcode, @cat_padrao_intel, @spec_note_win_i7_int_32g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_ssd512gb); -- Mesma spec/upg de Arquiteto
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_lowcode, @cat_segunda_opcao_amd, @spec_note_win_r7_int_32g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL); -- Mesma spec/upg de Arquiteto

-- Ma inframe (Dados do CSV)
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_mainframe, @cat_padrao_intel, @spec_note_win_i7_int_32g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_ssd512gb); -- Mesma spec/upg de Arquiteto
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_mainframe, @cat_segunda_opcao_amd, @spec_note_win_r7_int_32g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL); -- Mesma spec/upg de Arquiteto

-- Mobile IOS
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_mobileios, @cat_opcao_macbook, @spec_mac_ios_m_int_32g_s512g, @mod_apple_mackbookpro, NULL, NULL, NULL, @upg_memate4tb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_mobileios, @cat_segunda_opcao_amd, @spec_note_win_i7_int_32g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- Mobile Android (Dados do CSV)
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_mobileandroid, @cat_opcao_macbook, @spec_mac_macos_m_int_32g_s512g, @mod_apple_mackbookpro, NULL, NULL, NULL, NULL);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_mobileandroid, @cat_segunda_opcao_amd, @spec_note_win_i7_int_32g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- RPA
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_rpa, @cat_padrao_intel, @spec_note_win_i7_int_32g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_ssd512gb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_rpa, @cat_segunda_opcao_amd, @spec_note_win_r7_int_32g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- QA/Automacao
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_qa_automacao, @cat_padrao_intel, @spec_note_win_i7_int_32g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_ssd512gb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_qa_automacao, @cat_segunda_opcao_amd, @spec_note_win_r7_int_32g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- Data/Analytics Platform
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_data_analytics, @cat_padrao_intel, @spec_note_win_i7_int_32g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_ssd512gbmem32gb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_data_analytics, @cat_segunda_opcao_amd, @spec_note_win_r7_int_32g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);

-- Arquiteto
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_arquiteto, @cat_padrao_intel, @spec_note_win_i7_int_32g_s256g, @mod_lenovo_thinkpadt14, @mod_dell_latitude3450, NULL, NULL, @upg_ssd512gb);
INSERT INTO tb_equipamento_padrao_mapeamento (id, tb_equipamento_padrao_cargo_funcao_id, tb_equipamento_padrao_categoria_requisito_id, tb_equipamento_padrao_especificacao_id, tb_equipamento_padrao_catalogo_modelo_id_aprovado_1, tb_equipamento_padrao_catalogo_modelo_id_aprovado_2, tb_equipamento_padrao_catalogo_modelo_id_aprovado_3, tb_equipamento_padrao_catalogo_modelo_id_aprovado_4, tb_equipamento_padrao_upgrade_customizado_id)
VALUES (UUID(), @cargo_arquiteto, @cat_segunda_opcao_amd, @spec_note_win_r7_int_32g_s256g, NULL, NULL, @mod_lenovo_thinkpadp14, NULL, NULL);
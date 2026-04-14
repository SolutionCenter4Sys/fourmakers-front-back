CREATE TABLE tb_template_pdf_admissao (
                                          id VARCHAR(36) NOT NULL,
                                          tb_candidato_vaga_id VARCHAR(36) NOT NULL,
                                          tb_vaga_id VARCHAR(36) NOT NULL,
                                          tb_cliente_org_codigo_cliente VARCHAR(45) NOT NULL,

                                          PRIMARY KEY (id),

                                          CONSTRAINT fk_template_pdf_candidato_vaga
                                              FOREIGN KEY (tb_candidato_vaga_id)
                                                  REFERENCES tb_candidato_vaga(id)
                                                  ON DELETE CASCADE
                                                  ON UPDATE CASCADE,
                                          CONSTRAINT fk_template_pdf_vaga
                                              FOREIGN KEY (tb_vaga_id)
                                                  REFERENCES tb_vaga(id)
                                                  ON DELETE CASCADE
                                                  ON UPDATE CASCADE,
                                          CONSTRAINT fk_template_pdf_cliente
                                              FOREIGN KEY (tb_cliente_org_codigo_cliente)
                                                  REFERENCES tb_cliente_org(codigo_cliente)
                                                  ON DELETE CASCADE
                                                  ON UPDATE CASCADE

);

CREATE TABLE tb_equipamentos_foursys_template_pdf (
                                                      id VARCHAR(36) NOT NULL,
                                                      tb_template_pdf_admissao_id VARCHAR(36) NOT NULL,
                                                      celular tinyint NOT NULL,
                                                      plano_dados tinyint NOT NULL,
                                                      quantidade_minutos_plano_dados double NOT NULL,
                                                      cartao_visitas tinyint NOT NULL,
                                                      quantidade_cartao_visitas tinyint NOT NULL,
                                                      outros TEXT,


                                                      PRIMARY KEY (id),

                                                      CONSTRAINT fk_template_pdf_admissao
                                                          FOREIGN KEY (tb_template_pdf_admissao_id)
                                                              REFERENCES tb_template_pdf_admissao(id)
                                                              ON DELETE CASCADE
                                                              ON UPDATE CASCADE
);

CREATE TABLE tb_profissional_template_pdf (
                                              id VARCHAR(36) NOT NULL,
                                              tb_template_pdf_admissao_id VARCHAR(36) NOT NULL,

                                              tb_equipamentos_foursys_template_pdf_id VARCHAR(36) NULL,
                                              nome VARCHAR(255) NOT NULL,
                                              rg VARCHAR(50),
                                              cpf VARCHAR(50),
                                              data_nascimento DATE,
                                              telefone VARCHAR(50),
                                              email_pessoal VARCHAR(255),
                                              tamanho_camiseta VARCHAR(10),
                                              superior_imediato VARCHAR(255),


                                              PRIMARY KEY (id),

                                              CONSTRAINT fk_profissional_template_pdf_template
                                                  FOREIGN KEY (tb_template_pdf_admissao_id)
                                                      REFERENCES tb_template_pdf_admissao(id)
                                                      ON DELETE CASCADE
                                                      ON UPDATE CASCADE,

                                              CONSTRAINT fk_profissional_template_pdf_equipamentos_foursys
                                                  FOREIGN KEY (tb_equipamentos_foursys_template_pdf_id)
                                                      REFERENCES tb_equipamentos_foursys_template_pdf(id)
                                                      ON DELETE SET NULL
                                                      ON UPDATE CASCADE
);

CREATE TABLE tb_vaga_admissao_template_pdf (
                                               id VARCHAR(36) NOT NULL,
                                               tb_template_pdf_admissao_id VARCHAR(36) NOT NULL,

                                               id_vaga INT,
                                               cliente VARCHAR(255),
                                               cargo VARCHAR(255),
                                               stack_principal VARCHAR(255),
                                               nivel_cargo VARCHAR(100),
                                               tipo_vaga VARCHAR(100),
                                               endereco VARCHAR(500),
                                               cep VARCHAR(20),
                                               data_inicio DATETIME,
                                               diretoria VARCHAR(255) NULL,
                                               solicitante VARCHAR(255),
                                               gestor_responsavel VARCHAR(255),
                                               analista_responsavel VARCHAR(255),
                                               unidade_trabalho VARCHAR(255),
                                               local_alocacao VARCHAR(255),
                                               area_staff VARCHAR(255),
                                               horario_trabalho VARCHAR(100),
                                               horas_fechadas BOOLEAN,

                                               PRIMARY KEY (id),

                                               CONSTRAINT fk_vaga_admissao_template_pdf_template
                                                   FOREIGN KEY (tb_template_pdf_admissao_id)
                                                       REFERENCES tb_template_pdf_admissao(id)
                                                       ON DELETE CASCADE
                                                       ON UPDATE CASCADE
);

CREATE TABLE tb_beneficios_template_pdf (
                                            id VARCHAR(36) NOT NULL,
                                            tb_template_pdf_admissao_id VARCHAR(36) NOT NULL,

                                            salario DECIMAL(12,2),
                                            vale_refeicao DECIMAL(12,2),
                                            vale_alimentacao DECIMAL(12,2),
                                            assistencia_medica DECIMAL(12,2),
                                            ajuda_de_custo DECIMAL(12,2),
                                            mobilidade DECIMAL(12,2),
                                            assistencia_educacional DECIMAL(12,2),
                                            remuneracao_total DECIMAL(12,2),

                                            PRIMARY KEY (id),

                                            CONSTRAINT fk_beneficios_template_pdf_template
                                                FOREIGN KEY (tb_template_pdf_admissao_id)
                                                    REFERENCES tb_template_pdf_admissao(id)
                                                    ON DELETE CASCADE
                                                    ON UPDATE CASCADE
);

CREATE TABLE tb_checklist_instalacao_template_pdf (
                                                      id VARCHAR(36) NOT NULL,
                                                      tb_template_pdf_admissao_id VARCHAR(36) NOT NULL,
                                                      nome_profissional VARCHAR(255),
                                                      descricao_maquina VARCHAR(255),
                                                      hardware VARCHAR(255),
                                                      outros_softwares TEXT,

                                                      PRIMARY KEY (id),

                                                      CONSTRAINT fk_checklist_instalacao_template_pdf_template
                                                          FOREIGN KEY (tb_template_pdf_admissao_id)
                                                              REFERENCES tb_template_pdf_admissao(id)
                                                              ON DELETE CASCADE
                                                              ON UPDATE CASCADE
);

CREATE TABLE tb_acessos_usuario_template_pdf (
                                                 id VARCHAR(36) NOT NULL,
                                                 tb_template_pdf_admissao_id VARCHAR(36) NOT NULL,
                                                 tb_profissional_template_pdf_id VARCHAR(36) NOT NULL,

                                                 email_foursys VARCHAR(255),
                                                 tipo_email VARCHAR(50),
                                                 login_rede VARCHAR(100),
                                                 observacao_acesso_usuario TEXT,

                                                 PRIMARY KEY (id),

                                                 CONSTRAINT fk_acessos_usuario_template_pdf_template
                                                     FOREIGN KEY (tb_template_pdf_admissao_id)
                                                         REFERENCES tb_template_pdf_admissao(id)
                                                         ON DELETE CASCADE
                                                         ON UPDATE CASCADE,

                                                 CONSTRAINT fk_acessos_usuario_tb_profissional_template_pdf_id
                                                     FOREIGN KEY (tb_profissional_template_pdf_id)
                                                         REFERENCES tb_profissional_template_pdf(id)
                                                         ON DELETE CASCADE
                                                         ON UPDATE CASCADE
);

CREATE TABLE tb_acessos_usuario_grupos_template_pdf (
                                                        id VARCHAR(36) NOT NULL,
                                                        tb_acessos_usuario_template_pdf_id VARCHAR(36) NOT NULL,
                                                        tb_template_pdf_admissao_id VARCHAR(36) NOT NULL,
                                                        todos_foursys tinyint NOT NULL,
                                                        foursys_alphavile tinyint NOT NULL,
                                                        foursys_paulista tinyint NOT NULL,
                                                        foursys_curitiba tinyint NOT NULL,
                                                        grupo_email_contrato VARCHAR(255) NOT NULL,
                                                        descricao_outros_grupos TEXT,
                                                        observacao_aprovador TEXT,

                                                        PRIMARY KEY (id),

                                                        CONSTRAINT fk_grupos_template_pdf_acessos
                                                            FOREIGN KEY (tb_acessos_usuario_template_pdf_id)
                                                                REFERENCES tb_acessos_usuario_template_pdf(id)
                                                                ON DELETE CASCADE
                                                                ON UPDATE CASCADE,

                                                        CONSTRAINT fk_grupos_template_pdf_template
                                                            FOREIGN KEY (tb_template_pdf_admissao_id)
                                                                REFERENCES tb_template_pdf_admissao(id)
                                                                ON DELETE CASCADE
                                                                ON UPDATE CASCADE
);


CREATE TABLE tb_sistemas_liberados_template_pdf (
                                                    id VARCHAR(36) NOT NULL,
                                                    nome_sistema VARCHAR(255) NOT NULL,
                                                    PRIMARY KEY(id)
);


INSERT INTO tb_sistemas_liberados_template_pdf (id, nome_sistema)
VALUES (UUID(), 'Portal de Projetos');

INSERT INTO tb_sistemas_liberados_template_pdf (id, nome_sistema)
VALUES (UUID(), 'Mapa de Alocação');

INSERT INTO tb_sistemas_liberados_template_pdf (id, nome_sistema)
VALUES (UUID(), 'SCC - Custo profissional');

INSERT INTO tb_sistemas_liberados_template_pdf (id, nome_sistema)
VALUES (UUID(), 'SCC - Necessário autorização diretoria');

INSERT INTO tb_sistemas_liberados_template_pdf (id, nome_sistema)
VALUES (UUID(), 'CRM');

INSERT INTO tb_sistemas_liberados_template_pdf (id, nome_sistema)
VALUES (UUID(), 'CCH - administrador');

INSERT INTO tb_sistemas_liberados_template_pdf (id, nome_sistema)
VALUES (UUID(), 'Controle de Gadget');

INSERT INTO tb_sistemas_liberados_template_pdf (id, nome_sistema)
VALUES (UUID(), 'Recrutamento e Seleção - Trabalhar as Vagas');

INSERT INTO tb_sistemas_liberados_template_pdf (id, nome_sistema)
VALUES (UUID(), 'Recrutamento e Seleção - Criação de Vagas');

INSERT INTO tb_sistemas_liberados_template_pdf (id, nome_sistema)
VALUES (UUID(), 'Recrutamento e Seleção - Aprovador de Vagas');


CREATE TABLE tb_sistemas_liberados_acessos_usuario_template_pdf (
                                                                    id VARCHAR(36) NOT NULL,
                                                                    tb_acessos_usuario_template_pdf_id VARCHAR(36) NOT NULL,
                                                                    tb_sistemas_liberados_template_pdf_id VARCHAR(36) NOT NULL,
                                                                    liberado tinyint NOT NULL,

                                                                    PRIMARY KEY (id),

                                                                    CONSTRAINT fk_sistemas_liberados_acessos_usuario
                                                                        FOREIGN KEY (tb_acessos_usuario_template_pdf_id)
                                                                            REFERENCES tb_acessos_usuario_template_pdf(id)
                                                                            ON DELETE CASCADE
                                                                            ON UPDATE CASCADE,

                                                                    CONSTRAINT fk_sistemas_liberados
                                                                        FOREIGN KEY (tb_sistemas_liberados_template_pdf_id)
                                                                            REFERENCES tb_sistemas_liberados_template_pdf(id)
                                                                            ON DELETE CASCADE
                                                                            ON UPDATE CASCADE
);

CREATE TABLE tb_acessos_pasta_rede_template_pdf (
                                                    id VARCHAR(36) NOT NULL,
                                                    diretorio VARCHAR(255) NOT NULL,
                                                    leitura tinyint NOT NULL,
                                                    escrita tinyint NOT NULL,
                                                    PRIMARY KEY(id)

);

INSERT INTO tb_acessos_pasta_rede_template_pdf (id, diretorio, leitura, escrita)
VALUES (UUID(), 'Diretório PMO - UN 1 (Bradesco) Diretório de Propostas', 0, 0);

INSERT INTO tb_acessos_pasta_rede_template_pdf (id, diretorio, leitura, escrita)
VALUES (UUID(), 'Diretório Faturamento_Bradesco', 0, 0);

INSERT INTO tb_acessos_pasta_rede_template_pdf (id, diretorio, leitura, escrita)
VALUES (UUID(), 'Diretório Com - UN 2, 3 e 4 (Executivos de Conta)', 0, 0);

INSERT INTO tb_acessos_pasta_rede_template_pdf (id, diretorio, leitura, escrita)
VALUES (UUID(), 'Diretorio de Pré Vendas - UN 2, 3 e 4 (gerentes de projetos e executivos de contas)', 0, 0);

INSERT INTO tb_acessos_pasta_rede_template_pdf (id, diretorio, leitura, escrita)
VALUES (UUID(), 'Diretorio de Marketing', 0, 0);

INSERT INTO tb_acessos_pasta_rede_template_pdf (id, diretorio, leitura, escrita)
VALUES (UUID(), 'Diretorio de RH', 0, 0);

INSERT INTO tb_acessos_pasta_rede_template_pdf (id, diretorio, leitura, escrita)
VALUES (UUID(), 'Diretório de Recrutamento e Seleção (RS)', 0, 0);

INSERT INTO tb_acessos_pasta_rede_template_pdf (id, diretorio, leitura, escrita)
VALUES (UUID(), 'Diretório de Infra', 0, 0);

INSERT INTO tb_acessos_pasta_rede_template_pdf (id, diretorio, leitura, escrita)
VALUES (UUID(), 'Diretório de Manutenção Predial', 0, 0);

CREATE TABLE tb_acessos_pasta_rede_template_pdf_rel (
                                                        id VARCHAR(36) NOT NULL,
                                                        tb_acessos_usuario_template_pdf_id VARCHAR(36) NOT NULL,
                                                        tb_acessos_pasta_rede_template_pdf_id VARCHAR(36) NOT NULL,

                                                        PRIMARY KEY (id),

                                                        CONSTRAINT fk_acessos_pasta_rede_template_pdf
                                                            FOREIGN KEY (tb_acessos_pasta_rede_template_pdf_id)
                                                                REFERENCES tb_acessos_pasta_rede_template_pdf(id)
                                                                ON DELETE CASCADE
                                                                ON UPDATE CASCADE,

                                                        CONSTRAINT fk_acessos_usuario_template_pdf
                                                            FOREIGN KEY (tb_acessos_usuario_template_pdf_id)
                                                                REFERENCES tb_acessos_usuario_template_pdf(id)
                                                                ON DELETE CASCADE
                                                                ON UPDATE CASCADE
);



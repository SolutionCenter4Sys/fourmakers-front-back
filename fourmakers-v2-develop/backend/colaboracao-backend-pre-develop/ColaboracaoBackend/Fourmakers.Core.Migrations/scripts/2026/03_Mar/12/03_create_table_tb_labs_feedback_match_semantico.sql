-- Feedback do usuário: qual resultado de match preferiu (RankCandidatesIds vs Match Semântico)
CREATE TABLE IF NOT EXISTS tb_labs_feedback_match_semantico (
  id CHAR(36) NOT NULL,
  id_log_rank_candidates_ids CHAR(36) NULL COMMENT 'FK lógica para tb_labs_log_rank_candidates_ids.id',
  id_log_match_semantico CHAR(36) NULL COMMENT 'FK lógica para tb_labs_log_match_semantico.id',
  match_semantico_melhor TINYINT(1) NOT NULL COMMENT '1 = usuário preferiu o Match Semântico, 0 = preferiu RankCandidatesIds',
  tb_org_id INT NOT NULL,
  codigo_interno_colaborador VARCHAR(36) NULL COMMENT 'Quem enviou o feedback',
  data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

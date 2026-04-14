-- ============================================================
-- Feedback 360 - Mural de reconhecimento: catálogo de emojis para reação
-- Emojis disponíveis para reagir aos cards do mural.
-- ============================================================

CREATE TABLE tb_feedback360_mural_emojis_reacao (
  id INT NOT NULL AUTO_INCREMENT,
  slug VARCHAR(30) NOT NULL COMMENT 'Identificador técnico da reação (love, rocket, clap)',
  emoji VARCHAR(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Emoji exibido no front',
  ativo TINYINT NOT NULL DEFAULT 1,
  data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY uk_slug (slug)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Opções de emoji para reação nos cards do mural de reconhecimento (coração, foguete, palmas, etc.)';

-- Dados iniciais: coração, foguete, palmas, divertido, fogo
INSERT INTO tb_feedback360_mural_emojis_reacao (slug, emoji) VALUES
('love', '❤️'),
('rocket', '🚀'),
('clap', '👏'),
('smile', '😄'),
('fire', '🔥');

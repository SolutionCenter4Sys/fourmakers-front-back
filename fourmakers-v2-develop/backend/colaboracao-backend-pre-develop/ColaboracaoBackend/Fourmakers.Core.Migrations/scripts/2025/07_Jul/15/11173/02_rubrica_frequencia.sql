CREATE TABLE `tb_rubrica_frequencia` (
  `id` int PRIMARY KEY,
  `descricao` varchar(100),
  `detalhe` varchar(255),
  `codigo_rubrica_frequencia` varchar(50),
  UNIQUE KEY uq_codigo_rubrica_frequencia (codigo_rubrica_frequencia)
);

INSERT INTO tb_rubrica_frequencia (id, descricao, detalhe, codigo_rubrica_frequencia) VALUES
(1, 'Única', 'Rubrica aplicada UNICAMENTE na vigência, sem repetição automática.', 'UNICA'),
(2, 'Mensal', 'Rubrica aplicada de forma recorrente com vigência final definida.', 'MENSAL_SEM_VIGENCIA_FINAL'),
(3, 'Mensal com Data Final', 'Rubrica aplicada de forma contínua, sem data final definida.', 'MENSAL_COM_VIGENCIA_FINAL');

CREATE TABLE tb_colaborador_pagamento_cnab_nota_fiscal (
   id CHAR(36) NOT NULL,
   tb_colaborador_pagamento_cnab_id CHAR(36) NOT NULL,
   tb_nota_fiscal_id CHAR(36) NOT NULL,
   valor_pagamento DECIMAL(15,2) NOT NULL,

   CONSTRAINT pk_tb_colab_pag_cnab_nf
       PRIMARY KEY (id),

   CONSTRAINT fk_tb_colab_pag_cnab_nf_cnab
       FOREIGN KEY (tb_colaborador_pagamento_cnab_id)
           REFERENCES tb_colaborador_pagamento_cnab (id),

   CONSTRAINT fk_tb_colab_pag_cnab_nf_nf
       FOREIGN KEY (tb_nota_fiscal_id)
           REFERENCES tb_nota_fiscal (id)
);
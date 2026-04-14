CREATE TABLE tb_vaga_candidato_status_log (
    id INT PRIMARY KEY AUTO_INCREMENT,  
    vaga_id INT NOT NULL,               
    candidato_id INT NOT NULL,          
    status_id INT NOT NULL,             
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,  
    FOREIGN KEY (status_id) REFERENCES tb_candidato_status(id)
);

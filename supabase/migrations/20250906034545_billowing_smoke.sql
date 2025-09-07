/*
  # Schema do Sistema de Estoque e PDV

  Este arquivo contém todo o schema necessário para o Sistema de Estoque e PDV.
  Execute este script no SQL Editor do seu projeto Supabase.

  ## Tabelas criadas:
  1. produtos - Armazena informações dos produtos
  2. vendas - Armazena informações das vendas
  3. itens_venda - Armazena os itens de cada venda

  ## Segurança:
  - RLS habilitado em todas as tabelas
  - Políticas para operações CRUD
*/

-- Limpar tabelas existentes se necessário (remova os comentários se precisar recriar)
-- DROP TABLE IF EXISTS itens_venda CASCADE;
-- DROP TABLE IF EXISTS vendas CASCADE;
-- DROP TABLE IF EXISTS produtos CASCADE;

-- Extensões necessárias
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Tabela de Produtos
CREATE TABLE IF NOT EXISTS produtos (
    id SERIAL PRIMARY KEY,
    codigo VARCHAR(50) UNIQUE NOT NULL,
    nome VARCHAR(200) NOT NULL,
    descricao TEXT,
    preco DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    quantidade_estoque INTEGER NOT NULL DEFAULT 0,
    estoque_minimo INTEGER NOT NULL DEFAULT 0,
    categoria VARCHAR(100),
    ativo BOOLEAN DEFAULT TRUE,
    data_criacao TIMESTAMPTZ DEFAULT NOW(),
    data_atualizacao TIMESTAMPTZ DEFAULT NOW(),
    
    CONSTRAINT produtos_preco_check CHECK (preco >= 0),
    CONSTRAINT produtos_estoque_check CHECK (quantidade_estoque >= 0),
    CONSTRAINT produtos_estoque_minimo_check CHECK (estoque_minimo >= 0)
);

-- Tabela de Vendas
CREATE TABLE IF NOT EXISTS vendas (
    id SERIAL PRIMARY KEY,
    numero_venda VARCHAR(50) UNIQUE NOT NULL,
    data_venda TIMESTAMPTZ DEFAULT NOW(),
    valor_total DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    desconto DECIMAL(10,2) DEFAULT 0.00,
    valor_final DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    forma_pagamento VARCHAR(50) NOT NULL,
    status VARCHAR(20) DEFAULT 'Finalizada',
    observacoes TEXT,
    
    CONSTRAINT vendas_valor_total_check CHECK (valor_total >= 0),
    CONSTRAINT vendas_desconto_check CHECK (desconto >= 0),
    CONSTRAINT vendas_valor_final_check CHECK (valor_final >= 0),
    CONSTRAINT vendas_status_check CHECK (status IN ('Finalizada', 'Cancelada'))
);

-- Tabela de Itens da Venda
CREATE TABLE IF NOT EXISTS itens_venda (
    id SERIAL PRIMARY KEY,
    venda_id INTEGER REFERENCES vendas(id) ON DELETE CASCADE,
    produto_id INTEGER REFERENCES produtos(id) ON DELETE RESTRICT,
    quantidade INTEGER NOT NULL,
    preco_unitario DECIMAL(10,2) NOT NULL,
    subtotal DECIMAL(10,2) NOT NULL,
    
    CONSTRAINT itens_quantidade_check CHECK (quantidade > 0),
    CONSTRAINT itens_preco_check CHECK (preco_unitario >= 0),
    CONSTRAINT itens_subtotal_check CHECK (subtotal >= 0)
);

-- Índices para melhor performance
CREATE INDEX IF NOT EXISTS idx_produtos_codigo ON produtos(codigo);
CREATE INDEX IF NOT EXISTS idx_produtos_nome ON produtos(nome);
CREATE INDEX IF NOT EXISTS idx_produtos_ativo ON produtos(ativo);
CREATE INDEX IF NOT EXISTS idx_vendas_numero ON vendas(numero_venda);
CREATE INDEX IF NOT EXISTS idx_vendas_data ON vendas(data_venda);
CREATE INDEX IF NOT EXISTS idx_vendas_status ON vendas(status);
CREATE INDEX IF NOT EXISTS idx_itens_venda_id ON itens_venda(venda_id);
CREATE INDEX IF NOT EXISTS idx_itens_produto_id ON itens_venda(produto_id);

-- Function para atualizar data_atualizacao automaticamente
CREATE OR REPLACE FUNCTION update_data_atualizacao()
RETURNS TRIGGER AS $$
BEGIN
    NEW.data_atualizacao = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger para atualizar data_atualizacao em produtos
DROP TRIGGER IF EXISTS trigger_update_produtos_data_atualizacao ON produtos;
CREATE TRIGGER trigger_update_produtos_data_atualizacao
    BEFORE UPDATE ON produtos
    FOR EACH ROW
    EXECUTE FUNCTION update_data_atualizacao();

-- Habilitar Row Level Security
ALTER TABLE produtos ENABLE ROW LEVEL SECURITY;
ALTER TABLE vendas ENABLE ROW LEVEL SECURITY;
ALTER TABLE itens_venda ENABLE ROW LEVEL SECURITY;

-- Políticas de segurança para produtos
CREATE POLICY "Permitir todas operações em produtos" ON produtos
    FOR ALL
    USING (true)
    WITH CHECK (true);

-- Políticas de segurança para vendas
CREATE POLICY "Permitir todas operações em vendas" ON vendas
    FOR ALL
    USING (true)
    WITH CHECK (true);

-- Políticas de segurança para itens_venda
CREATE POLICY "Permitir todas operações em itens_venda" ON itens_venda
    FOR ALL
    USING (true)
    WITH CHECK (true);

-- Inserir produtos de exemplo
INSERT INTO produtos (codigo, nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria) VALUES
('001', 'Coca-Cola 350ml', 'Refrigerante Coca-Cola lata 350ml', 4.50, 100, 20, 'Bebidas'),
('002', 'Pão Frances', 'Pão francês tradicional', 0.75, 50, 10, 'Padaria'),
('003', 'Leite Integral 1L', 'Leite integral UHT 1 litro', 4.99, 30, 5, 'Laticínios'),
('004', 'Arroz Tipo 1 5kg', 'Arroz branco tipo 1 pacote 5kg', 18.90, 25, 5, 'Grãos'),
('005', 'Feijão Preto 1kg', 'Feijão preto tipo 1 pacote 1kg', 8.50, 20, 3, 'Grãos'),
('006', 'Açúcar Cristal 1kg', 'Açúcar cristal refinado 1kg', 3.99, 15, 3, 'Mercearia'),
('007', 'Café em Pó 500g', 'Café torrado e moído 500g', 12.90, 12, 2, 'Bebidas'),
('008', 'Óleo de Soja 900ml', 'Óleo de soja refinado 900ml', 5.49, 18, 4, 'Mercearia'),
('009', 'Macarrão Espaguete 500g', 'Macarrão espaguete semolado 500g', 3.25, 40, 8, 'Massas'),
('010', 'Sabonete 90g', 'Sabonete em barra 90g', 2.10, 60, 15, 'Higiene')
ON CONFLICT (codigo) DO NOTHING;

-- Comentários nas tabelas
COMMENT ON TABLE produtos IS 'Tabela para armazenar informações dos produtos do estoque';
COMMENT ON TABLE vendas IS 'Tabela para armazenar informações das vendas realizadas';
COMMENT ON TABLE itens_venda IS 'Tabela para armazenar os itens de cada venda';

-- Comentários nas colunas principais
COMMENT ON COLUMN produtos.codigo IS 'Código único do produto (código de barras, SKU, etc.)';
COMMENT ON COLUMN produtos.quantidade_estoque IS 'Quantidade atual em estoque';
COMMENT ON COLUMN produtos.estoque_minimo IS 'Quantidade mínima para alerta de estoque baixo';
COMMENT ON COLUMN vendas.numero_venda IS 'Número único da venda gerado automaticamente';
COMMENT ON COLUMN vendas.forma_pagamento IS 'Forma de pagamento utilizada (Dinheiro, Cartão, PIX, etc.)';

-- Views úteis para relatórios
CREATE OR REPLACE VIEW view_produtos_baixo_estoque AS
SELECT 
    p.id,
    p.codigo,
    p.nome,
    p.quantidade_estoque,
    p.estoque_minimo,
    p.categoria,
    (p.estoque_minimo - p.quantidade_estoque) as quantidade_necessaria
FROM produtos p
WHERE p.ativo = true 
AND p.quantidade_estoque <= p.estoque_minimo
ORDER BY (p.estoque_minimo - p.quantidade_estoque) DESC;

CREATE OR REPLACE VIEW view_vendas_hoje AS
SELECT 
    v.*,
    COUNT(iv.id) as total_itens,
    SUM(iv.quantidade) as total_produtos_vendidos
FROM vendas v
LEFT JOIN itens_venda iv ON v.id = iv.venda_id
WHERE DATE(v.data_venda) = CURRENT_DATE
AND v.status != 'Cancelada'
GROUP BY v.id
ORDER BY v.data_venda DESC;

CREATE OR REPLACE VIEW view_produtos_mais_vendidos AS
SELECT 
    p.id,
    p.codigo,
    p.nome,
    p.categoria,
    COALESCE(SUM(iv.quantidade), 0) as total_vendido,
    COALESCE(SUM(iv.subtotal), 0) as total_faturado,
    COUNT(DISTINCT iv.venda_id) as vendas_participou
FROM produtos p
LEFT JOIN itens_venda iv ON p.id = iv.produto_id
LEFT JOIN vendas v ON iv.venda_id = v.id AND v.status != 'Cancelada'
WHERE p.ativo = true
GROUP BY p.id, p.codigo, p.nome, p.categoria
ORDER BY total_vendido DESC;

COMMENT ON VIEW view_produtos_baixo_estoque IS 'View que mostra produtos com estoque abaixo do mínimo';
COMMENT ON VIEW view_vendas_hoje IS 'View que mostra as vendas do dia atual';
COMMENT ON VIEW view_produtos_mais_vendidos IS 'View que mostra produtos ordenados por quantidade vendida';
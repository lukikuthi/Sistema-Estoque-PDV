# Sistema Estoque & PDV

## Summary  
Sistema completo de Estoque e Ponto de Venda (PDV) desenvolvido em **C#** usando console do Visual Studio, integrado ao **Supabase (PostgreSQL)**.  
Oferece gerenciamento de estoque, vendas, relatórios e interface interativa.

---

## Features
- Gerenciamento de Estoque: cadastrar, editar, remover e listar produtos; atualizar estoque; alertas de estoque baixo.
- Ponto de Venda (PDV): registrar novas vendas, consultar e cancelar vendas; aplicar descontos e múltiplas formas de pagamento (Dinheiro, Cartão, PIX).
- Relatórios: produtos em baixo estoque, vendas por período, produtos mais vendidos e resumo financeiro.

---

## Technologies  
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c#&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)



---

## 📹 Demo Video

[![Watch the video](https://img.youtube.com/vi/shgl9tRx8qk/hqdefault.jpg)](https://www.youtube.com/watch?v=shgl9tRx8qk)

---

### Pré-requisitos
- Visual Studio 2022 ou VS Code
- .NET 8.0 SDK
- Conta no Supabase
- Git (opcional)

### Configurar Supabase
1. Criar conta e projeto: `sistema-estoque-pdv`
2. Copiar Project URL e anon key
3. Rodar `supabase_schema.sql` no SQL Editor

### Configurar Projeto
1. Abrir `SistemaEstoquePDV.csproj` no Visual Studio
2. Inserir credenciais do Supabase em `Services/SupabaseService.cs`
3. Restaurar pacotes NuGet: `dotnet restore`

### Executar
- Compilar: `Ctrl + Shift + B`
- Rodar: `F5` ou `Ctrl + F5`  

"Conexão estabelecida com sucesso!" indica funcionamento correto.

### Arquivo Setup pronto
- Pasta Compactada (Arquivo .ZIP) dentro da pasta do projeto
- Extraia a pasta, instale e use o Sistema sem necessidade de executar nada

---

## License
Uso livre para aprendizado e projetos pessoais. Para uso comercial, implementar licenciamento, backup e segurança.

---

## Project developed by Lucas (github.com/lukikuthi)

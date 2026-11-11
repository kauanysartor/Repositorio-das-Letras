# Repositório das Letras

Sistema de gestão de biblioteca, construído com arquitetura de microsserviços.

**Disciplina:** Arquitetura de Software | Engenharia de Software | SATC

**Professor:** Eduardo Cizeski Meneghel

**Equipe:** *ALEXSANDER ANTUNES BARBOSA*, *KAUANY HAHN SARTOR*, *RAFAELLA DE SOUZA BENEDET*

**Links**
- Repositório: https://github.com/kauanysartor/Repositorio-das-Letras

---

## 1. Documento de requisitos

### 1.1 Propósito do sistema

O **Repositório das Letras** organiza o funcionamento de uma biblioteca: mantém o acervo de livros, o cadastro de leitores, o controle de empréstimos e devoluções e a cobrança de multas por atraso. O sistema substitui controles manuais (planilhas e fichas), evitando erros como emprestar um livro sem exemplares disponíveis ou emprestar para um leitor bloqueado por dívida.

### 1.2 Usuários

| Usuário           | Descrição                                                                                                           |
|-------------------|---------------------------------------------------------------------------------------------------------------------|
| **Bibliotecário** | Atende o balcão: cadastra livros e leitores, registra empréstimos e devoluções, lança multas e registra pagamentos. |
| **Administrador** | Gerencia o acervo e os leitores e consulta as informações do sistema.                                               |
| **Leitor**        | Consulta o acervo, seus empréstimos e suas multas.                                                                  |

### 1.3 Requisitos funcionais

**Livros**

- **RF01** Cadastrar, listar, buscar, atualizar e excluir livros (título, autor, ISBN, categoria e quantidade de exemplares).
- **RF02** Controlar a quantidade total de exemplares de cada livro.

**Leitores**

- **RF03** Cadastrar, listar, buscar, atualizar e inativar leitores (nome, CPF, e-mail e telefone).
- **RF04** Registrar o valor da dívida de cada leitor.
- **RF05** Bloquear automaticamente leitores com multas pendentes.

**Empréstimos**

- **RF06** Registrar o empréstimo de um livro a um leitor, informando a data de devolução prevista.
- **RF07** Impedir o empréstimo se o leitor estiver inativo ou bloqueado.
- **RF08** Impedir o empréstimo se o livro não tiver exemplar disponível (exemplares do livro menos empréstimos em aberto).
- **RF09** Registrar a devolução de um livro.
- **RF10** Exibir os dados do leitor e do livro ao consultar um empréstimo.
- **RF11** Consultar a disponibilidade de um livro.

**Multas**

- **RF12** Lançar uma multa para um leitor (valor, motivo e empréstimo de origem).
- **RF13** Ao lançar uma multa, atualizar a dívida do leitor e bloqueá-lo.
- **RF14** Registrar o pagamento da multa, liberando o leitor quando não houver mais dívida.
- **RF15** Listar as multas por leitor e por situação (pendente ou paga).

---

## 2. Descritivo técnico

### 2.1 Microsserviços

A aplicação é composta por **4 microsserviços independentes**. Cada serviço tem seu próprio banco de dados SQLite e se comunica com os demais somente por requisições HTTP REST.

| Microsserviço   | Função                                                                                                  | Porta local |
|-----------------|---------------------------------------------------------------------------------------------------------|-------------|
| **Livros**      | Gerencia o acervo: cadastro de livros e quantidade de exemplares.                                       | 5001        |
| **Leitores**    | Gerencia o cadastro de leitores, a situação (ativo, inativo, bloqueado) e o valor da dívida.            | 5002        |
| **Empréstimos** | Registra empréstimos e devoluções. Valida leitor e livro antes de emprestar.                            | 5003        |
| **Multas**      | Registra multas e pagamentos. Ao lançar ou pagar uma multa, altera a dívida do leitor.                  | 5004        |

```
                 +----------------+
                 |   Empréstimos  |
                 +----------------+
                  /              \
   busca leitor  /                \  busca livro
                /                  \
      +-----------+            +----------+
      |  Leitores |            |  Livros  |
      +-----------+            +----------+
            ^
            | altera dívida
      +-----------+
      |   Multas  |
      +-----------+
```

### 2.2 Integrações entre os microsserviços

| # | Tipo                         | Origem      | Destino  | Rota chamada                      | Descrição                                                                                                                                                  |
|---|------------------------------|-------------|----------|-----------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1 | Busca de dados simples       | Empréstimos | Leitores | `GET /api/leitores/{id}`          | Ao registrar um empréstimo (e ao consultá-lo), busca o leitor e valida se está ativo e sem bloqueio. Exibe os dados do leitor no empréstimo.               |
| 2 | Busca de dados simples       | Empréstimos | Livros   | `GET /api/livros/{id}`            | Ao registrar um empréstimo (e ao consultá-lo), busca o livro e valida se há exemplar disponível. Exibe os dados do livro no empréstimo.                    |
| 3 | Alteração de dados           | Multas      | Leitores | `PATCH /api/leitores/{id}/divida` | Ao lançar uma multa, soma o valor à dívida do leitor e muda a situação para bloqueado. No pagamento, reduz a dívida e libera o leitor se ela zerar.        |

Na integração 2, o serviço de Empréstimos busca o livro em Livros e compara a quantidade de exemplares com os empréstimos em aberto no seu próprio banco para saber se há exemplar disponível.

Cada serviço de origem possui uma classe cliente (`LeitorClient`, `LivroClient`) que concentra as chamadas HTTP ao serviço de destino, com a URL lida do `appsettings.json` (`LeitoresUrl` e `LivrosUrl`).

### 2.3 Endpoints principais

| Serviço     | Método | Rota                                                  | Descrição                                                |
|-------------|--------|-------------------------------------------------------|----------------------------------------------------------|
| Livros      | GET    | `/api/livros`                                         | Lista os livros                                          |
| Livros      | GET    | `/api/livros/{id}`                                    | Busca um livro                                           |
| Livros      | POST   | `/api/livros`                                         | Cadastra um livro                                        |
| Livros      | PUT    | `/api/livros/{id}`                                    | Atualiza um livro                                        |
| Livros      | DELETE | `/api/livros/{id}`                                    | Exclui um livro                                          |
| Leitores    | GET    | `/api/leitores`                                       | Lista os leitores                                        |
| Leitores    | GET    | `/api/leitores/{id}`                                  | Busca um leitor                                          |
| Leitores    | POST   | `/api/leitores`                                       | Cadastra um leitor                                       |
| Leitores    | PUT    | `/api/leitores/{id}`                                  | Atualiza um leitor                                       |
| Leitores    | PATCH  | `/api/leitores/{id}/inativar`                         | Inativa um leitor                                        |
| Leitores    | PATCH  | `/api/leitores/{id}/divida`                           | Altera dívida e situação (usado por Multas)              |
| Empréstimos | POST   | `/api/emprestimos`                                    | Registra empréstimo (aciona integrações 1 e 2)           |
| Empréstimos | GET    | `/api/emprestimos`                                    | Lista os empréstimos                                     |
| Empréstimos | GET    | `/api/emprestimos/{id}`                               | Consulta empréstimo com dados do leitor e do livro       |
| Empréstimos | PUT    | `/api/emprestimos/{id}/devolucao`                     | Registra devolução                                       |
| Empréstimos | GET    | `/api/emprestimos/livros/{codigoLivro}/disponibilidade` | Consulta a disponibilidade de um livro                 |
| Multas      | POST   | `/api/multas`                                         | Lança multa (aciona integração 3)                        |
| Multas      | PUT    | `/api/multas/{id}/pagamento`                          | Registra pagamento (aciona integração 3)                 |
| Multas      | GET    | `/api/multas?leitorId=&situacao=`                     | Lista multas por leitor e por situação                   |

### 2.4 Tecnologias

- .NET 10 (template da disciplina), Entity Framework Core e SQLite (um banco por microsserviço)
- API REST com troca de dados em JSON, documentada com Scalar (`/scalar`)
- Comunicação entre serviços via `HttpClient`
- Docker e publicação no Render (um Web Service por microsserviço)
- Código-fonte no GitHub

---

## 3. Como executar localmente

1. Clonar o repositório.
2. Em cada pasta de serviço (`livros`, `leitores`, `emprestimos`, `multas`), entrar na pasta `AppTemplate` e executar `dotnet run` (um terminal por serviço). Subir Livros e Leitores antes de Empréstimos e Multas.
3. Conferir nos arquivos `appsettings.json` de **Empréstimos** (`LivrosUrl` e `LeitoresUrl`) e **Multas** (`LeitoresUrl`) as URLs dos serviços de destino. Por padrão: `http://localhost:5001` (Livros) e `http://localhost:5002` (Leitores).
4. Testar pelas rotas do Scalar de cada serviço, em `http://localhost:<porta>/scalar`.

## 4. Estrutura do repositório

```
/livros
/leitores
/emprestimos
/multas
README.md
```

Cada pasta contém um serviço completo (solution, `Dockerfile` e o projeto em `AppTemplate/`).

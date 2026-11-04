# Repositório das Letras

Back-end de um sistema de gestão de biblioteca, construído com arquitetura de microsserviços.

**Disciplina:** Arquitetura de Software | Engenharia de Software | Centro Universitário SATC
**Professor:** Eduardo Cizeski Meneghel
**Avaliação:** Sistemas distribuídos

**Equipe:** _ALEXSANDER ANTUNES BARBOSA_, _KAUANY HAHN SARTOR_, _RAFAELLA DE SOUZA BENEDET_


---

## 1. Documento de requisitos

### 1.1 Propósito do sistema

O **Repositório das Letras** tem o objetivo de organizar o funcionamento de uma biblioteca: manter o acervo de livros, o cadastro de leitores, o controle de empréstimos e devoluções e a cobrança de multas por atraso. O sistema substitui controles manuais (planilhas e fichas), evitando erros como emprestar um livro sem exemplares disponíveis ou emprestar para um leitor bloqueado por dívida.

### 1.2 Usuários

| Usuário | Descrição |
|---|---|
| **Bibliotecário** | Atende o balcão: cadastra livros e leitores, registra empréstimos e devoluções, lança multas e registra pagamentos. |
| **Administrador** | Gerencia o acervo e os leitores, consulta relatórios e ajusta regras (prazo de empréstimo e valor da multa). |
| **Leitor** | Consulta o acervo, seus empréstimos e suas multas. |

### 1.3 Requisitos funcionais

**Livros**
- **RF01** O sistema deve permitir cadastrar, listar, buscar, atualizar e excluir livros (título, autor, ISBN, categoria e quantidade de exemplares).
- **RF02** O sistema deve controlar a quantidade de exemplares disponíveis de cada livro.
- **RF03** O sistema deve permitir consultar a disponibilidade de um livro.

**Leitores**
- **RF04** O sistema deve permitir cadastrar, listar, buscar, atualizar e inativar leitores (nome, CPF, e-mail e telefone).
- **RF05** O sistema deve registrar o valor da dívida de cada leitor.
- **RF06** O sistema deve bloquear automaticamente leitores com multas pendentes.

**Empréstimos**
- **RF07** O sistema deve permitir registrar o empréstimo de um livro a um leitor, informando a data de devolução prevista.
- **RF08** O sistema deve impedir o empréstimo se o leitor estiver inativo ou bloqueado.
- **RF09** O sistema deve impedir o empréstimo se o livro não tiver exemplar disponível.
- **RF10** O sistema deve permitir registrar a devolução de um livro.
- **RF11** O sistema deve exibir os dados do leitor e do livro ao consultar um empréstimo.

**Multas**
- **RF12** O sistema deve permitir lançar uma multa para um leitor (valor, motivo e empréstimo de origem).
- **RF13** Ao lançar uma multa, o sistema deve atualizar a dívida do leitor e bloqueá-lo.
- **RF14** O sistema deve permitir registrar o pagamento da multa, liberando o leitor quando não houver mais dívida.
- **RF15** O sistema deve permitir listar as multas por leitor e por situação (pendente ou paga).

---

## 2. Descritivo técnico

### 2.1 Visão geral

A aplicação é composta por **4 microsserviços independentes**, todos baseados no template de microsserviço da disciplina (.NET, SQLite, REST). Cada serviço tem seu próprio banco de dados SQLite e se comunica com os demais somente por requisições HTTP REST, nunca acessando o banco de outro serviço.

```
                 +----------------+
                 |   Empréstimos  |
                 +----------------+
                  /              \
     busca leitor/                \busca livro
                /                  \
      +-----------+            +----------+
      |  Leitores |            |  Livros  |
      +-----------+            +----------+
            ^
            | atualiza dívida / bloqueia
      +-----------+
      |   Multas  |
      +-----------+
```

### 2.2 Microsserviços

| Microsserviço | Função | Porta local |
|---|---|---|
| **Livros** | Gerencia o acervo (cadastro de livros) e o controle de exemplares disponíveis. | 5001 |
| **Leitores** | Gerencia o cadastro de leitores, a situação (ativo, inativo, bloqueado) e o valor da dívida. | 5002 |
| **Empréstimos** | Registra empréstimos e devoluções. Valida leitor e livro antes de emprestar. | 5003 |
| **Multas** | Registra multas e pagamentos. Ao lançar uma multa, atualiza a dívida do leitor. | 5004 |

### 2.3 Integrações entre os microsserviços

| # | Tipo | Origem | Destino | Descrição |
|---|---|---|---|---|
| 1 | Busca de dados | Empréstimos | Leitores | Ao registrar um empréstimo (e ao consultá-lo), busca o leitor e valida se está ativo e sem bloqueio. Exibe os dados do leitor no empréstimo. |
| 2 | Busca de dados | Empréstimos | Livros | Ao registrar um empréstimo, busca o livro e valida se há exemplar disponível. Exibe os dados do livro no empréstimo. |
| 3 | Alteração de dados | Multas | Leitores | Ao cadastrar uma multa, o serviço de Multas dispara uma alteração no serviço de Leitores: soma o valor à dívida do leitor e muda a situação para bloqueado. No pagamento, a dívida é reduzida e o leitor é liberado se zerar. |

### 2.4 Endpoints principais

| Serviço | Método | Rota | Descrição |
|---|---|---|---|
| Livros | GET | `/api/livros` | Lista os livros |
| Livros | GET | `/api/livros/{id}` | Busca um livro |
| Livros | POST | `/api/livros` | Cadastra um livro |
| Leitores | GET | `/api/leitores/{id}` | Busca um leitor |
| Leitores | POST | `/api/leitores` | Cadastra um leitor |
| Leitores | PATCH | `/api/leitores/{id}/divida` | Atualiza dívida e situação (usado por Multas) |
| Empréstimos | POST | `/api/emprestimos` | Registra empréstimo (aciona integrações 1 e 2) |
| Empréstimos | GET | `/api/emprestimos/{id}` | Consulta empréstimo com dados do leitor e do livro |
| Empréstimos | PUT | `/api/emprestimos/{id}/devolucao` | Registra devolução |
| Multas | POST | `/api/multas` | Lança multa (aciona integração 3) |
| Multas | PUT | `/api/multas/{id}/pagamento` | Registra pagamento |
| Multas | GET | `/api/multas?leitorId=` | Lista multas do leitor |

### 2.5 Tecnologias

- .NET (template da disciplina)
- SQLite (um banco por microsserviço)
- API REST com troca de dados em JSON
- Comunicação entre serviços via `HttpClient`
- Publicação no Render (um Web Service por microsserviço)
- Código-fonte no GitHub

---

## 4. Estrutura do repositório

```
/livros
/leitores
/emprestimos
/multas
README.md
```

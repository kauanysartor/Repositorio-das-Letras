# Passo a passo para rodar o Repositório das Letras

## 1. Pré-requisitos

- **SDK do .NET 10** instalado. Confira com `dotnet --version` (deve mostrar `10.x`).
- Internet na primeira execução (o `dotnet run` baixa os pacotes NuGet).
- Um terminal por serviço (4 no total).

## 2. Subir os 4 serviços

Descompacte o zip e, na pasta `Repositorio-das-Letras`, abra 4 terminais. Em cada um, rode um serviço:

```bash
# Terminal 1 - Livros (porta 5001)
cd livros/AppTemplate
dotnet run

# Terminal 2 - Leitores (porta 5002)
cd leitores/AppTemplate
dotnet run

# Terminal 3 - Empréstimos (porta 5003)
cd emprestimos/AppTemplate
dotnet run

# Terminal 4 - Multas (porta 5004)
cd multas/AppTemplate
dotnet run
```

Suba Livros e Leitores antes de Empréstimos e Multas. O banco SQLite (`Livros.db`, `Leitores.db`, `Emprestimos.db`, `Multas.db`) é criado sozinho na primeira execução, dentro da pasta `AppTemplate` de cada serviço.

Cada serviço fica disponível, com documentação interativa (Scalar), em:

| Serviço     | Documentação e testes            |
|-------------|----------------------------------|
| Livros      | http://localhost:5001/scalar     |
| Leitores    | http://localhost:5002/scalar     |
| Empréstimos | http://localhost:5003/scalar     |
| Multas      | http://localhost:5004/scalar     |


## 2.1 Abrindo no Visual Studio (ícone roxo)

Use o **Visual Studio 2022 (17.13 ou mais novo) ou o Visual Studio 2026**, com a carga de trabalho **ASP.NET e desenvolvimento para a Web** instalada (os arquivos `.slnx` precisam dessas versões).

1. Abra o Visual Studio e clique em **Abrir um projeto ou solução** (ou **Arquivo > Abrir > Projeto/Solução**).
2. Selecione o arquivo **`RepositorioDasLetras.slnx`** na pasta raiz. Ele já contém os 4 projetos.
3. Na barra de ferramentas, confirme que o perfil de execução é **http** (não IIS Express).
4. Clique com o botão direito na **Solução** (Gerenciador de Soluções) e escolha **Configurar projetos de inicialização...**.
5. Marque **Vários projetos de inicialização** e coloque **Iniciar** nos 4 projetos (`LivrosApp`, `LeitoresApp`, `EmprestimosApp`, `MultasApp`). Clique em **OK**.
6. Pressione **Ctrl+F5** (iniciar sem depuração) ou **F5** (com depuração). Abrem 4 janelas de console e 4 abas do navegador, cada uma já no `/scalar` do serviço.

Se preferir abrir um serviço só, use o `.slnx` dentro da pasta dele (por exemplo `livros/LivrosApp.slnx`).

## 3. Roteiro de teste (cobre as 3 integrações)

Use o Scalar de cada serviço (botão **Test Request**). Os `id` retornados nas respostas são usados nos passos seguintes.

1. **Livros** → `POST /api/livros`
   ```json
   { "titulo": "Dom Casmurro", "autor": "Machado de Assis", "isbn": "9788535910663", "categoria": "Romance", "quantidadeExemplares": 1 }
   ```
   Guarde o `id` do livro.

2. **Leitores** → `POST /api/leitores`
   ```json
   { "nome": "Maria Silva", "cpf": "12345678900", "email": "maria@email.com", "telefone": "48999990000" }
   ```
   Guarde o `id` do leitor (situação `Ativo`, dívida 0).

3. **Empréstimos** → `POST /api/emprestimos` (integrações 1 e 2: busca leitor e livro)
   ```json
   { "codigoLeitor": "<id do leitor>", "codigoLivro": "<id do livro>", "dataPrevistaDevolucao": "2026-12-15" }
   ```
   Use uma data de hoje em diante. Guarde o `id` do empréstimo.

4. **Empréstimos** → `GET /api/emprestimos/{id}`: mostra o empréstimo com os dados do leitor e do livro.

5. **Empréstimos** → repita o passo 3 com o mesmo livro: deve falhar com "O livro não possui exemplar disponível." (o livro só tem 1 exemplar).

6. **Empréstimos** → `GET /api/emprestimos/livros/{id do livro}/disponibilidade`: mostra exemplares total, em aberto e disponíveis.

7. **Multas** → `POST /api/multas` (integração 3: altera a dívida em Leitores)
   ```json
   { "codigoLeitor": "<id do leitor>", "codigoEmprestimo": "<id do empréstimo>", "valor": 15.50, "motivo": "Atraso de 5 dias" }
   ```
   Guarde o `id` da multa.

8. **Leitores** → `GET /api/leitores/{id}`: a dívida agora é `15.5` e a situação é `Bloqueado`.

9. **Empréstimos** → tente um novo empréstimo para esse leitor: deve falhar com "Leitor bloqueado por dívida...".

10. **Multas** → `PUT /api/multas/{id}/pagamento`.

11. **Leitores** → `GET /api/leitores/{id}`: dívida `0` e situação `Ativo` de novo.

12. **Empréstimos** → `PUT /api/emprestimos/{id}/devolucao`: devolve o livro. Repetir o passo 6 mostra o exemplar disponível outra vez.

## 4. Problemas comuns

| Sintoma | Causa provável | O que fazer |
|---|---|---|
| `dotnet` não é reconhecido | SDK não instalado | Instalar o SDK do .NET 10 |
| "Erro ao buscar leitor" ou "Erro ao buscar livro" | Serviço de destino fora do ar ou URL errada | Subir Livros e Leitores e conferir `LivrosUrl` e `LeitoresUrl` no `appsettings.json` |
| "address already in use" | Porta ocupada | Fechar o processo que usa a porta ou trocar a porta em `Properties/launchSettings.json` (e a URL correspondente no `appsettings.json`) |
| Quer zerar os dados | Banco com dados de teste | Parar o serviço e apagar o arquivo `.db` da pasta `AppTemplate` |
| Navegador tenta abrir https | Endereço digitado com https | Usar sempre `http://localhost:<porta>/scalar` (os serviços rodam só em http localmente) |

## 5. Publicar no Render (resumo)

O professor vai disponibilizar um manual; o resumo é:

1. Subir o projeto para o GitHub (repositório público ou compartilhado com `eduardocmeneghel`).
2. No Render, criar **4 Web Services** a partir do mesmo repositório, um por serviço, com **Root Directory** igual a `livros`, `leitores`, `emprestimos` e `multas`, e ambiente **Docker** (cada pasta já tem seu `Dockerfile`).
3. Publicar primeiro Livros e Leitores e anotar as URLs públicas (`https://....onrender.com`).
4. No serviço **Empréstimos**, criar as variáveis de ambiente `LivrosUrl` e `LeitoresUrl` com essas URLs. No serviço **Multas**, criar `LeitoresUrl`.
5. Colocar os 4 links do Render no README.

Observações: no plano gratuito, o primeiro acesso depois de um tempo parado demora (o serviço "acorda"), e o banco SQLite é perdido quando o serviço reinicia ou é reimplantado.

## 6. Subir para o GitHub

```bash
git clone https://github.com/kauanysartor/Repositorio-das-Letras.git
# copie o conteúdo desta pasta para dentro do repositório clonado
cd Repositorio-das-Letras
git add .
git commit -m "Adiciona os 4 microsserviços e o README"
git push origin main
```

# FecapStudy — Aula 9 (Oficina PBL) · C# + Windows Forms

Projeto completo das Missões 1 a 10: classes de domínio, tela gráfica e integração com o backend.

## Como rodar

Precisa do .NET SDK 8 no Windows (o do laboratório já serve).

    cd FecapStudy
    dotnet run

Também abre no Visual Studio: duplo-clique em `FecapStudy.csproj` e F5.

- Se o seu .NET for outro, abra `FecapStudy.csproj` e troque `net8.0-windows`
  por `net6.0-windows`, `net7.0-windows` ou `net9.0-windows`.
- Se o `dotnet build` reclamar de pasta do OneDrive, copie o projeto para
  `C:\Users\seu_usuario\ProjetosPOO\` (dica da seção "Telas sem Admin" do material).

## Usando a tela

1. Escolha disciplina, modalidade, duração e o dado extra (ciclos, acertos/questões ou páginas/resumo).
2. Clique em **Registrar Estudo**: o XP sobe, a barra de progresso anda e o feedback entra no histórico.
3. **Notificar Responsável**: escolha o canal (E-mail, WhatsApp ou Painel) e clique. O envio é simulado e mostra a mensagem.
4. **Redefinir meta**: o Responsável muda a meta semanal (o domínio trava entre 30 e 3000 min).

## Backend (Missão 10)

1. Rode `iniciar_backend.bat` (pasta `Backend` do material) e confirme http://localhost:5080.
2. Abra a tela: a barra de status embaixo mostra "Backend online".
3. Cada **Registrar Estudo** faz `POST /api/sessoes`. Veja a linha verde na janela preta do backend.
4. **Carregar histórico do servidor** faz `GET /api/sessoes` e preenche a lista da direita.
5. **Ver relatório** faz `GET /api/relatorio/Lucas Silva`.

Sem o backend a tela continua funcionando só em memória e avisa na barra de status.

## Onde está cada missão

| Missão | Conceito                  | Arquivo(s) |
|-------:|---------------------------|------------|
| 1  | Abstração, classe, objeto | `Dominio/Disciplina.cs` |
| 2  | Encapsulamento            | `Dominio/MetaEstudo.cs` |
| 3  | Classe abstrata e herança | `Dominio/Usuario.cs`, `Administrador.cs`, `Responsavel.cs`, `Discente.cs` |
| 4  | Polimorfismo dinâmico     | `Dominio/SessaoEstudo.cs`, `SessaoPomodoro.cs`, `SessaoExercicios.cs`, `SessaoLeitura.cs` |
| 5  | Polimorfismo estático     | `Dominio/Discente.cs` (3 sobrecargas de `RegistrarEstudo`) |
| 6  | Interfaces                | `Dominio/INotificador.cs`, `IExportavelJson.cs`, `Notificadores.cs` |
| 7  | Associação e agregação    | `Dominio/Responsavel.cs`, `TrilhaEstudo.cs` |
| 8  | Composição e dependência  | `Dominio/PlanoEstudo.cs`, `ServicoAlertaResponsavel.cs` |
| 9  | Tela gráfica              | `MainForm.cs` |
| 10 | Integração com o backend  | `Servicos/ClienteBackendEstudo.cs` (+ botões em `MainForm.cs`) |

## Personalize antes de entregar

No início do construtor de `MainForm` (arquivo `MainForm.cs`) troque as disciplinas, os nomes,
o RA e a meta pelos da sua equipe.

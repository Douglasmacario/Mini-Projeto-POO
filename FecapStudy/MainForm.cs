using System.Text.Json;
using System.Text.Encodings.Web;
using FecapStudy.Dominio;
using FecapStudy.Servicos;

namespace FecapStudy;

/// <summary>
/// Missão 9 — a tela. Regra de ouro (Aula 4): o clique do botão só LÊ os controles,
/// cria o objeto certo e pede para as classes calcularem. Nenhuma fórmula de XP aqui!
/// </summary>
public class MainForm : Form
{
    private static readonly Color VerdeEscuro = Color.FromArgb(2, 51, 39);
    private static readonly Color VerdeFecap = Color.FromArgb(0, 227, 135);

    // ───────── Objetos do domínio (Missões 1 a 8) ─────────
    private readonly Administrador admin;
    private readonly Responsavel responsavel;
    private readonly Discente discente;
    private readonly MetaEstudo meta;
    private readonly TrilhaEstudo trilha;
    private readonly PlanoEstudo plano;
    private readonly ServicoAlertaResponsavel servicoAlerta = new();

    // ───────── Missão 10 ─────────
    private readonly ClienteBackendEstudo cliente = new();

    // ───────── Controles ─────────
    private Label lblPerfilDiscente, lblPerfilResponsavel;
    private GroupBox grpFormulario;
    private ComboBox cmbDisciplina, cmbModalidade, cmbCanal;
    private NumericUpDown numDuracao, numExtra, numExtra2, numMeta;
    private Label lblExtra, lblExtra2, lblMeta, lblStatus;
    private CheckBox chkResumo;
    private Button btnRegistrar, btnNotificar, btnCarregar, btnRelatorio, btnRedefinirMeta;
    private ProgressBar progressMeta;
    private ListBox listBoxHistorico, listBoxServidor;

    public MainForm()
    {
        // 1) Cria os objetos do domínio
        admin = new Administrador(1, "Marina Costa", "marina@fecap.br", "admin123", "Coordenação PI");
        responsavel = new Responsavel(2, "Carlos Silva", "carlos@fecap.br", "resp123", "Pai");
        discente = new Discente(3, "Lucas Silva", "lucas@fecap.br", "aluno123", "RA-101");
        meta = new MetaEstudo(300);

        // Agregação: as disciplinas nascem FORA da trilha
        var poo = new Disciplina("ADS201", "Programação Orientada a Objetos", "Computação", 4);
        var bd = new Disciplina("ADS202", "Banco de Dados", "Dados", 4);
        var ed = new Disciplina("ADS203", "Estruturas de Dados", "Computação", 3);
        var emp = new Disciplina("NEG101", "Empreendedorismo", "Negócios", 2);
        trilha = new TrilhaEstudo("Trilha 2º Semestre ADS", admin);
        trilha.AdicionarDisciplina(poo);
        trilha.AdicionarDisciplina(bd);
        trilha.AdicionarDisciplina(ed);
        trilha.AdicionarDisciplina(emp);

        // Associação: o responsável acompanha o discente e sua meta
        responsavel.VincularDiscente(discente);
        responsavel.DefinirMeta(discente, meta);

        // Composição: o plano cria seus próprios itens
        plano = new PlanoEstudo();
        plano.AdicionarItem("Seg", 60);
        plano.AdicionarItem("Qua", 90);
        plano.AdicionarItem("Sex", 45);

        // 2) Monta a tela
        MontarTela();

        // 3) Preenche com os dados iniciais
        foreach (var d in trilha.GetDisciplinas()) cmbDisciplina.Items.Add(d);
        cmbDisciplina.SelectedIndex = 0;

        cmbModalidade.Items.AddRange(new object[] { "Pomodoro", "Exercicios", "Leitura" });
        cmbModalidade.SelectedIndex = 0;

        // Interfaces: três canais, todos tratados como INotificador
        cmbCanal.Items.Add(new NotificadorEmail(texto => MostrarNotificacao(texto)));
        cmbCanal.Items.Add(new NotificadorWhatsApp(texto => MostrarNotificacao(texto)));
        cmbCanal.Items.Add(new NotificadorPainel(texto => DefinirStatus("Painel: " + texto.Replace(Environment.NewLine, " "), false)));
        cmbCanal.SelectedIndex = 0;

        numMeta.Value = meta.GetMetaMinutosSemana();

        grpFormulario.Text = "Nova sessão de estudo — " + trilha.GerarResumo();
        AtualizarPerfil();
        AtualizarProgresso();

        Shown += async (s, e) => await VerificarBackendAsync();
    }

    // ═════════════════════════════════════════════════════════════
    //  Construção da tela (feita por código: sem Designer, sem surpresas)
    // ═════════════════════════════════════════════════════════════
    private void MontarTela()
    {
        Text = "FecapStudy — Plataforma de Apoio ao Estudo";
        ClientSize = new Size(1000, 760);
        MinimumSize = new Size(900, 720);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(241, 247, 244);

        var raiz = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(10)
        };
        raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));    // faixa de título
        raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));   // perfil
        raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 225));   // formulário
        raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));    // progresso + histórico
        Controls.Add(raiz);

        raiz.Controls.Add(CriarFaixaTitulo(), 0, 0);
        raiz.Controls.Add(CriarGrupoPerfil(), 0, 1);
        raiz.Controls.Add(CriarGrupoFormulario(), 0, 2);
        raiz.Controls.Add(CriarGrupoProgresso(), 0, 3);
    }

    private Control CriarFaixaTitulo()
    {
        return new Label
        {
            Text = "  FecapStudy · Plataforma de Apoio ao Estudo",
            Dock = DockStyle.Fill,
            BackColor = VerdeEscuro,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 16F),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(3, 3, 3, 6)
        };
    }

    // ───── Perfil ─────
    private Control CriarGrupoPerfil()
    {
        var grp = new GroupBox { Text = "Perfil (GerarPainelResumo de cada usuário)", Dock = DockStyle.Fill };

        var tabela = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        tabela.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tabela.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        lblPerfilDiscente = CriarPainelTexto();
        lblPerfilResponsavel = CriarPainelTexto();
        tabela.Controls.Add(lblPerfilDiscente, 0, 0);
        tabela.Controls.Add(lblPerfilResponsavel, 1, 0);

        grp.Controls.Add(tabela);
        return grp;
    }

    private static Label CriarPainelTexto() => new Label
    {
        Dock = DockStyle.Fill,
        BackColor = Color.White,
        BorderStyle = BorderStyle.FixedSingle,
        Padding = new Padding(8),
        Margin = new Padding(4),
        TextAlign = ContentAlignment.TopLeft
    };

    // ───── Formulário de nova sessão ─────
    private Control CriarGrupoFormulario()
    {
        grpFormulario = new GroupBox { Text = "Nova sessão de estudo", Dock = DockStyle.Fill };

        var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 5, Padding = new Padding(6, 4, 6, 4) };
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 5; i++) t.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        cmbDisciplina = CriarCombo();
        cmbModalidade = CriarCombo();
        numDuracao = CriarNumerico(1, 480, 25);       // a Missão 2 recusa > 480
        numExtra = CriarNumerico(0, 500, 1);
        numExtra2 = CriarNumerico(1, 500, 10);
        lblExtra = CriarRotulo("Ciclos completos:");
        lblExtra2 = CriarRotulo("Total de questões:");
        chkResumo = new CheckBox { Text = "Fez resumo do capítulo (+25 XP)", AutoSize = true, Anchor = AnchorStyles.Left, Checked = true };

        btnRegistrar = new Button
        {
            Text = "Registrar Estudo",
            Size = new Size(200, 30),
            Anchor = AnchorStyles.Right,
            FlatStyle = FlatStyle.Flat,
            BackColor = VerdeFecap,
            ForeColor = VerdeEscuro,
            Font = new Font("Segoe UI Semibold", 10.5F),
            Cursor = Cursors.Hand
        };
        btnRegistrar.FlatAppearance.BorderColor = VerdeEscuro;

        t.Controls.Add(CriarRotulo("Disciplina:"), 0, 0);
        t.Controls.Add(cmbDisciplina, 1, 0);
        t.Controls.Add(CriarRotulo("Modalidade:"), 2, 0);
        t.Controls.Add(cmbModalidade, 3, 0);

        t.Controls.Add(CriarRotulo("Duração (min):"), 0, 1);
        t.Controls.Add(numDuracao, 1, 1);
        t.Controls.Add(lblExtra, 2, 1);
        t.Controls.Add(numExtra, 3, 1);

        t.Controls.Add(lblExtra2, 2, 2);
        t.Controls.Add(numExtra2, 3, 2);

        t.Controls.Add(chkResumo, 2, 3);
        t.SetColumnSpan(chkResumo, 2);

        t.Controls.Add(btnRegistrar, 0, 4);
        t.SetColumnSpan(btnRegistrar, 4);

        cmbModalidade.SelectedIndexChanged += (s, e) => AtualizarCamposModalidade();
        btnRegistrar.Click += BtnRegistrar_Click;

        grpFormulario.Controls.Add(t);
        return grpFormulario;
    }

    // ───── Progresso, histórico e integração ─────
    private Control CriarGrupoProgresso()
    {
        var grp = new GroupBox { Text = "Progresso da meta e histórico", Dock = DockStyle.Fill };

        var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6, Padding = new Padding(6, 2, 6, 2) };
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));    // texto da meta
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));    // barra
        t.RowStyles.Add(new RowStyle(SizeType.Percent, 100));    // listas
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));    // botões (notificar / servidor)
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));    // meta do responsável
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));    // status

        lblMeta = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI Semibold", 10F) };
        progressMeta = new ProgressBar { Dock = DockStyle.Fill, Minimum = 0, Maximum = 100, Margin = new Padding(3, 2, 3, 2) };

        // Duas listas lado a lado: histórico local e histórico vindo do servidor
        var listas = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
        listas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        listas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        listas.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        listas.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        listas.Controls.Add(new Label { Text = "Histórico desta sessão (GerarFeedback):", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        listas.Controls.Add(new Label { Text = "Gravado no servidor (GET /api/sessoes):", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 1, 0);
        listBoxHistorico = new ListBox { Dock = DockStyle.Fill, HorizontalScrollbar = true, IntegralHeight = false };
        listBoxServidor = new ListBox { Dock = DockStyle.Fill, HorizontalScrollbar = true, IntegralHeight = false };
        listas.Controls.Add(listBoxHistorico, 0, 1);
        listas.Controls.Add(listBoxServidor, 1, 1);

        // Linha de botões 1
        var linha1 = CriarLinhaFlow();
        btnNotificar = CriarBotao("Notificar Responsável", 190);
        cmbCanal = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120, Margin = new Padding(3, 5, 12, 3) };
        btnCarregar = CriarBotao("Carregar histórico do servidor", 250);
        btnRelatorio = CriarBotao("Ver relatório", 130);
        linha1.Controls.AddRange(new Control[] { btnNotificar, cmbCanal, btnCarregar, btnRelatorio });

        // Linha de botões 2: o Responsável redefine a meta (Missão 2 trava entre 30 e 3000)
        var linha2 = CriarLinhaFlow();
        var lblRedef = new Label { Text = "Meta semanal (Responsável), min:", AutoSize = true, Margin = new Padding(3, 9, 3, 3) };
        numMeta = CriarNumerico(1, 5000, 300);
        numMeta.Margin = new Padding(3, 5, 8, 3);
        numMeta.Width = 90;
        btnRedefinirMeta = CriarBotao("Redefinir meta", 150);
        linha2.Controls.AddRange(new Control[] { lblRedef, numMeta, btnRedefinirMeta });

        lblStatus = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.DimGray };

        t.Controls.Add(lblMeta, 0, 0);
        t.Controls.Add(progressMeta, 0, 1);
        t.Controls.Add(listas, 0, 2);
        t.Controls.Add(linha1, 0, 3);
        t.Controls.Add(linha2, 0, 4);
        t.Controls.Add(lblStatus, 0, 5);

        btnNotificar.Click += BtnNotificar_Click;
        btnCarregar.Click += BtnCarregar_Click;
        btnRelatorio.Click += BtnRelatorio_Click;
        btnRedefinirMeta.Click += BtnRedefinirMeta_Click;

        grp.Controls.Add(t);
        return grp;
    }

    // ───── fábricas de controle (evitam repetição) ─────
    private static Label CriarRotulo(string texto) =>
        new Label { Text = texto, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };

    private static ComboBox CriarCombo() =>
        new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Left | AnchorStyles.Right };

    private static NumericUpDown CriarNumerico(int min, int max, int valor) =>
        new NumericUpDown { Minimum = min, Maximum = max, Value = valor, Anchor = AnchorStyles.Left | AnchorStyles.Right };

    private static FlowLayoutPanel CriarLinhaFlow() =>
        new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, FlowDirection = FlowDirection.LeftToRight };

    private static Button CriarBotao(string texto, int largura) =>
        new Button { Text = texto, Size = new Size(largura, 32), Margin = new Padding(3, 3, 8, 3), Cursor = Cursors.Hand };

    // ═════════════════════════════════════════════════════════════
    //  Atualização visual (só LÊ o domínio, não calcula nada)
    // ═════════════════════════════════════════════════════════════
    private void AtualizarPerfil()
    {
        // Polimorfismo: a tela só chama GerarPainelResumo(); cada classe monta o seu texto.
        lblPerfilDiscente.Text = discente.GerarPainelResumo() + Environment.NewLine + plano.GerarResumo();
        lblPerfilResponsavel.Text = responsavel.GerarPainelResumo();
    }

    private void AtualizarProgresso()
    {
        double pct = meta.CalcularPercentual();
        progressMeta.Value = (int)pct;
        lblMeta.Text = $"Meta semanal: {meta.GetMinutosCumpridos()} / {meta.GetMetaMinutosSemana()} min  ({pct:F0}%)"
                       + (meta.EstaCumprida() ? "  — META CUMPRIDA!" : "");
    }

    /// <summary>Mostra só os campos que fazem sentido para a modalidade escolhida.</summary>
    private void AtualizarCamposModalidade()
    {
        string modalidade = cmbModalidade.SelectedItem as string;

        lblExtra2.Visible = numExtra2.Visible = modalidade == "Exercicios";
        chkResumo.Visible = modalidade == "Leitura";

        switch (modalidade)
        {
            case "Pomodoro":
                lblExtra.Text = "Ciclos completos:";
                numExtra.Value = 1;
                break;
            case "Exercicios":
                lblExtra.Text = "Acertos:";
                numExtra.Value = 8;
                numExtra2.Value = 10;
                break;
            case "Leitura":
                lblExtra.Text = "Páginas lidas:";
                numExtra.Value = 10;
                break;
        }
    }

    private void DefinirStatus(string texto, bool erro)
    {
        lblStatus.Text = texto;
        lblStatus.ForeColor = erro ? Color.Firebrick : Color.FromArgb(10, 122, 76);
    }

    // ═════════════════════════════════════════════════════════════
    //  Eventos
    // ═════════════════════════════════════════════════════════════
    private async void BtnRegistrar_Click(object sender, EventArgs e)
    {
        try
        {
            if (cmbDisciplina.SelectedItem is not Disciplina disciplina)
            {
                MessageBox.Show(this, "Escolha uma disciplina.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int minutos = (int)numDuracao.Value;

            // 1) Cria a subclasse certa (a própria sessão valida seus dados)
            SessaoEstudo sessao = CriarSessao(disciplina, minutos);
            if (sessao == null) return;

            // 2) A meta valida (1 a 480 min). Se recusar, ninguém ganha XP.
            if (!meta.RegistrarAvanco(minutos))
            {
                MessageBox.Show(this, "Duração recusada pela meta (use de 1 a 480 minutos).",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3) O discente registra (soma o XP calculado pela própria sessão)
            discente.RegistrarEstudo(sessao);

            // 4) Atualiza a tela
            listBoxHistorico.Items.Add(sessao.GerarFeedback());
            listBoxHistorico.TopIndex = listBoxHistorico.Items.Count - 1;
            AtualizarPerfil();
            AtualizarProgresso();

            // 5) Missão 10: envia para o backend (se estiver offline, segue só local)
            DefinirStatus("Enviando para o servidor...", false);
            btnRegistrar.Enabled = false;
            bool salvou = await cliente.SalvarSessaoAsync(discente.GetNome(), sessao);
            btnRegistrar.Enabled = true;

            if (salvou) DefinirStatus("Sessão gravada no servidor (POST /api/sessoes).", false);
            else DefinirStatus("Salva só localmente. " + cliente.UltimoErro, true);
        }
        catch (Exception ex)
        {
            btnRegistrar.Enabled = true;
            MessageBox.Show(this, ex.Message, "Erro ao registrar estudo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>Decide QUAL subclasse instanciar. A fórmula de XP continua dentro das classes.</summary>
    private SessaoEstudo CriarSessao(Disciplina disciplina, int minutos)
    {
        switch (cmbModalidade.SelectedItem as string)
        {
            case "Pomodoro":
                return new SessaoPomodoro(disciplina, minutos, (int)numExtra.Value);

            case "Exercicios":
                int acertos = (int)numExtra.Value;
                int total = (int)numExtra2.Value;
                if (acertos > total)
                {
                    MessageBox.Show(this, "Os acertos não podem ser maiores que o total de questões.",
                        "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }
                return new SessaoExercicios(disciplina, minutos, total, acertos);

            case "Leitura":
                return new SessaoLeitura(disciplina, minutos, (int)numExtra.Value, chkResumo.Checked);

            default:
                MessageBox.Show(this, "Escolha uma modalidade.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
        }
    }

    private void BtnNotificar_Click(object sender, EventArgs e)
    {
        // A tela só conhece o contrato INotificador, não o canal concreto.
        if (cmbCanal.SelectedItem is not INotificador notificador) return;

        bool enviado = servicoAlerta.AvaliarDiscente(discente, meta, responsavel, notificador);
        if (!enviado)
            MessageBox.Show(this, "Não foi possível enviar o alerta.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void MostrarNotificacao(string texto)
    {
        MessageBox.Show(this, texto, "Alerta enviado ao responsável", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async void BtnRedefinirMeta_Click(object sender, EventArgs e)
    {
        try
        {
            meta.RedefinirMeta((int)numMeta.Value);
            numMeta.Value = meta.GetMetaMinutosSemana();   // mostra o valor já ajustado (30 a 3000)
            AtualizarProgresso();
            AtualizarPerfil();

            bool ok = await cliente.SalvarMetaAsync(discente.GetNome(), responsavel.GetNome(), meta);
            if (ok) DefinirStatus("Meta atualizada e enviada ao servidor (POST /api/metas).", false);
            else DefinirStatus("Meta atualizada só localmente. " + cliente.UltimoErro, true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ═════════════════════════════════════════════════════════════
    //  Missão 10 — integração com o backend
    // ═════════════════════════════════════════════════════════════
    private async Task VerificarBackendAsync()
    {
        bool online = await cliente.VerificarStatusAsync();
        if (online)
        {
            await cliente.SalvarMetaAsync(discente.GetNome(), responsavel.GetNome(), meta);
            DefinirStatus("Backend online em " + ClienteBackendEstudo.EnderecoBase, false);
        }
        else
        {
            DefinirStatus("Backend offline. A tela funciona, mas nada será gravado. Inicie o BackendEstudoAPI.exe.", true);
        }
    }

    private async void BtnCarregar_Click(object sender, EventArgs e)
    {
        try
        {
            btnCarregar.Enabled = false;
            DefinirStatus("Consultando o servidor...", false);

            string json = await cliente.ListarSessoesJsonAsync();
            if (json == null)
            {
                DefinirStatus("Falha ao carregar. " + cliente.UltimoErro, true);
                return;
            }

            listBoxServidor.Items.Clear();
            foreach (string linha in FormatarSessoesDoServidor(json))
                listBoxServidor.Items.Add(linha);

            DefinirStatus($"{listBoxServidor.Items.Count} registro(s) carregado(s) do servidor.", false);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnCarregar.Enabled = true;
        }
    }

    private async void BtnRelatorio_Click(object sender, EventArgs e)
    {
        try
        {
            btnRelatorio.Enabled = false;
            DefinirStatus("Buscando relatório...", false);

            string json = await cliente.ObterRelatorioJsonAsync(discente.GetNome());
            if (json == null)
            {
                DefinirStatus("Falha ao buscar o relatório. " + cliente.UltimoErro, true);
                return;
            }

            DefinirStatus("Relatório recebido.", false);
            MostrarTexto("Relatório de " + discente.GetNome(), EmbelezarJson(json));
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnRelatorio.Enabled = true;
        }
    }

    /// <summary>
    /// Transforma o JSON do servidor em linhas legíveis. Aceita uma lista na raiz ou um objeto
    /// que contenha uma lista; se o formato for outro, mostra o texto bruto (nunca quebra).
    /// </summary>
    private static List<string> FormatarSessoesDoServidor(string json)
    {
        var linhas = new List<string>();
        try
        {
            using var doc = JsonDocument.Parse(json);
            JsonElement? lista = EncontrarLista(doc.RootElement);
            if (lista == null)
            {
                linhas.Add(json);
                return linhas;
            }

            foreach (JsonElement item in lista.Value.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object
                    && item.TryGetProperty("disciplina", out _)
                    && item.TryGetProperty("xpGanho", out _))
                {
                    linhas.Add($"{Texto(item, "discenteNome")}: {Texto(item, "tipoModalidade")} em {Texto(item, "disciplina")} " +
                               $"({Texto(item, "duracaoMinutos")} min) -> +{Texto(item, "xpGanho")} XP");
                }
                else
                {
                    linhas.Add(item.GetRawText());
                }
            }
        }
        catch (JsonException)
        {
            linhas.Clear();
            linhas.Add(json);
        }
        return linhas;
    }

    private static JsonElement? EncontrarLista(JsonElement raiz)
    {
        if (raiz.ValueKind == JsonValueKind.Array) return raiz;
        if (raiz.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in raiz.EnumerateObject())
                if (prop.Value.ValueKind == JsonValueKind.Array) return prop.Value;
        }
        return null;
    }

    private static string Texto(JsonElement item, string campo) =>
        item.TryGetProperty(campo, out var v) ? v.ToString() : "?";

    private static string EmbelezarJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }
        catch (JsonException)
        {
            return json;
        }
    }

    private void MostrarTexto(string titulo, string texto)
    {
        using var janela = new Form
        {
            Text = titulo,
            Size = new Size(640, 480),
            StartPosition = FormStartPosition.CenterParent
        };
        var caixa = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            Dock = DockStyle.Fill,
            ScrollBars = ScrollBars.Both,
            WordWrap = false,
            Font = new Font("Consolas", 10F),
            Text = texto.Replace("\r\n", "\n").Replace("\n", "\r\n")
        };
        janela.Controls.Add(caixa);
        janela.ShowDialog(this);
    }
}

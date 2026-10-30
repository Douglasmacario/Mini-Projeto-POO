namespace FecapStudy.Dominio;

public class Discente : Usuario
{
    private string ra;
    private int xpTotal = 0;
    private List<SessaoEstudo> historico = new();   // Missão 5

    public Discente(int id, string nome, string email, string senha, string ra)
        : base(id, nome, email, senha)
    {
        this.ra = ra;
    }

    public string GetRa() => ra;
    public int GetXpTotal() => xpTotal;
    public IReadOnlyList<SessaoEstudo> GetHistorico() => historico.AsReadOnly();

    public void AdicionarXp(int pontos)
    {
        if (pontos > 0) xpTotal += pontos;
    }

    // ───────── Missão 5 — Polimorfismo estático (sobrecarga) ─────────

    /// <summary>Forma 1: já existe um objeto SessaoEstudo pronto.</summary>
    public void RegistrarEstudo(SessaoEstudo sessao)
    {
        if (sessao == null) throw new ArgumentNullException(nameof(sessao));
        historico.Add(sessao);
        AdicionarXp(sessao.CalcularXp());
    }

    /// <summary>Forma 2 (atalho rápido): disciplina + minutos viram um Pomodoro padrão.</summary>
    public void RegistrarEstudo(Disciplina d, int minutos)
    {
        int ciclos = Math.Max(1, minutos / 25);
        RegistrarEstudo(new SessaoPomodoro(d, minutos, ciclos));
    }

    /// <summary>Forma 3 (por horário): calcula os minutos pelo intervalo e cria uma Leitura com resumo.</summary>
    public void RegistrarEstudo(Disciplina d, int horaInicio, int horaFim, int paginasLidas)
    {
        int minutos = (horaFim - horaInicio) * 60;
        RegistrarEstudo(new SessaoLeitura(d, minutos, paginasLidas, true));
    }

    public override string ObterTipoPerfil() => "Discente";

    public override string GerarPainelResumo() =>
        $"Discente: {GetNome()} (RA {ra}){Environment.NewLine}XP total: {xpTotal} | Sessões registradas: {historico.Count}";
}

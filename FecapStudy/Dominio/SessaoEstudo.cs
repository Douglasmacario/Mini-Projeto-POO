using System.Text.Json;

namespace FecapStudy.Dominio;

/// <summary>
/// Missão 4 — Polimorfismo dinâmico. Cada modalidade sabe calcular o PRÓPRIO XP e
/// montar o PRÓPRIO feedback: quem usa a sessão nunca precisa de "if (tipo == ...)".
/// </summary>
public abstract class SessaoEstudo : IExportavelJson
{
    private Disciplina disciplina;
    private int duracaoMinutos;

    protected SessaoEstudo(Disciplina disciplina, int duracaoMinutos)
    {
        if (disciplina == null) throw new ArgumentNullException(nameof(disciplina));
        if (duracaoMinutos <= 0) throw new ArgumentException("A duração deve ser maior que zero.", nameof(duracaoMinutos));
        this.disciplina = disciplina;
        this.duracaoMinutos = duracaoMinutos;
    }

    public Disciplina GetDisciplina() => disciplina;
    public int GetDuracaoMinutos() => duracaoMinutos;

    public abstract string ObterModalidade();
    public abstract int CalcularXp();

    public virtual string GerarFeedback() =>
        $"{ObterModalidade()} em {disciplina.GetNome()} ({duracaoMinutos} min) -> +{CalcularXp()} XP";

    /// <summary>Missão 6 — campos no formato que o Backend espera em POST /api/sessoes.</summary>
    public string ParaJson() => JsonSerializer.Serialize(new
    {
        disciplina = disciplina.GetNome(),
        tipoModalidade = ObterModalidade(),
        duracaoMinutos,
        xpGanho = CalcularXp(),
        resumoFeedback = GerarFeedback()
    }, JsonOpcoes.Padrao);
}

namespace FecapStudy.Dominio;

public class SessaoExercicios : SessaoEstudo
{
    private int totalQuestoes;
    private int acertos;

    public SessaoExercicios(Disciplina disciplina, int duracaoMinutos, int totalQuestoes, int acertos)
        : base(disciplina, duracaoMinutos)
    {
        if (totalQuestoes < 0 || acertos < 0)
            throw new ArgumentException("Questões e acertos não podem ser negativos.");
        if (acertos > totalQuestoes)
            throw new ArgumentException("Os acertos não podem ser maiores que o total de questões.");

        this.totalQuestoes = totalQuestoes;
        this.acertos = acertos;
    }

    public int GetTotalQuestoes() => totalQuestoes;
    public int GetAcertos() => acertos;

    public override string ObterModalidade() => "Exercicios";

    // duracao + acertos * 10
    public override int CalcularXp() => GetDuracaoMinutos() + acertos * 10;

    public override string GerarFeedback()
    {
        double pct = totalQuestoes > 0 ? acertos * 100.0 / totalQuestoes : 0;
        return base.GerarFeedback() + $" | acerto: {pct:F0}% ({acertos}/{totalQuestoes})";
    }
}

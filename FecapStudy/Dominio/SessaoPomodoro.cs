namespace FecapStudy.Dominio;

public class SessaoPomodoro : SessaoEstudo
{
    private int ciclosCompletos;

    public SessaoPomodoro(Disciplina disciplina, int duracaoMinutos, int ciclosCompletos)
        : base(disciplina, duracaoMinutos)
    {
        this.ciclosCompletos = Math.Max(0, ciclosCompletos);
    }

    public int GetCiclosCompletos() => ciclosCompletos;

    public override string ObterModalidade() => "Pomodoro";

    // duracao * 2 + ciclos * 15
    public override int CalcularXp() => GetDuracaoMinutos() * 2 + ciclosCompletos * 15;

    public override string GerarFeedback() =>
        base.GerarFeedback() + $" | {ciclosCompletos} ciclo(s) concluído(s)";
}

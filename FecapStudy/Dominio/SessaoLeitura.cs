namespace FecapStudy.Dominio;

public class SessaoLeitura : SessaoEstudo
{
    private int paginasLidas;
    private bool fezResumo;

    public SessaoLeitura(Disciplina disciplina, int duracaoMinutos, int paginasLidas, bool fezResumo)
        : base(disciplina, duracaoMinutos)
    {
        this.paginasLidas = Math.Max(0, paginasLidas);
        this.fezResumo = fezResumo;
    }

    public int GetPaginasLidas() => paginasLidas;
    public bool FezResumo() => fezResumo;

    public override string ObterModalidade() => "Leitura";

    // duracao + paginas * 3 + (resumo ? 25 : 0)
    public override int CalcularXp() => GetDuracaoMinutos() + paginasLidas * 3 + (fezResumo ? 25 : 0);

    public override string GerarFeedback() =>
        base.GerarFeedback() + $" | {paginasLidas} página(s)" + (fezResumo ? " + resumo" : "");
}

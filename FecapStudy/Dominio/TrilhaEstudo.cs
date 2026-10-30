using System.Text;

namespace FecapStudy.Dominio;

/// <summary>
/// Missão 7 — Agregação (◇──): as Disciplinas nascem FORA da trilha e continuam existindo
/// se a trilha for apagada. Por isso AdicionarDisciplina recebe o objeto já pronto.
/// </summary>
public class TrilhaEstudo
{
    private string titulo;
    private Administrador responsavel;                   // associação
    private List<Disciplina> disciplinas = new();

    public TrilhaEstudo(string titulo, Administrador responsavel)
    {
        this.titulo = titulo;
        this.responsavel = responsavel;
    }

    public string GetTitulo() => titulo;
    public Administrador GetResponsavel() => responsavel;
    public IReadOnlyList<Disciplina> GetDisciplinas() => disciplinas.AsReadOnly();

    public void AdicionarDisciplina(Disciplina d)
    {
        if (d == null) throw new ArgumentNullException(nameof(d));
        if (!disciplinas.Contains(d)) disciplinas.Add(d);
    }

    public int CalcularCargaHorariaTotal()
    {
        int total = 0;
        foreach (var d in disciplinas) total += d.GetCargaHorariaSemanalHoras();
        return total;
    }

    public string GerarResumo() =>
        $"{titulo} — {disciplinas.Count} disciplina(s), {CalcularCargaHorariaTotal()}h/semana";
}

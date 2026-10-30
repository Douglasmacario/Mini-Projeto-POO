using System.Text;

namespace FecapStudy.Dominio;

public class Responsavel : Usuario
{
    private string parentesco;

    // Missão 7 — Associação: os Discentes existem fora do Responsável.
    private List<Discente> discentes = new();
    private Dictionary<Discente, MetaEstudo> metas = new();

    public Responsavel(int id, string nome, string email, string senha, string parentesco)
        : base(id, nome, email, senha)
    {
        this.parentesco = parentesco;
    }

    public string GetParentesco() => parentesco;
    public IReadOnlyList<Discente> GetDiscentes() => discentes.AsReadOnly();

    /// <summary>Recebe um Discente criado fora e o adiciona sem duplicar.</summary>
    public void VincularDiscente(Discente d)
    {
        if (d == null) throw new ArgumentNullException(nameof(d));
        if (!discentes.Contains(d)) discentes.Add(d);
    }

    public void DefinirMeta(Discente d, MetaEstudo meta)
    {
        VincularDiscente(d);
        metas[d] = meta;
    }

    public MetaEstudo ObterMeta(Discente d) => metas.TryGetValue(d, out var m) ? m : null;

    public override string ObterTipoPerfil() => "Responsável";

    public override string GerarPainelResumo()
    {
        var sb = new StringBuilder();
        sb.Append($"Responsável ({parentesco}): {GetNome()}");
        sb.Append($"{Environment.NewLine}Acompanha {discentes.Count} discente(s):");
        foreach (var d in discentes)
        {
            var meta = ObterMeta(d);
            string pct = meta != null ? $"{meta.CalcularPercentual():F0}%" : "sem meta";
            sb.Append($"{Environment.NewLine}  - {d.GetNome()}: XP {d.GetXpTotal()} | meta {pct}");
        }
        return sb.ToString();
    }
}

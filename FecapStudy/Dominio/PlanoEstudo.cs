using System.Text;

namespace FecapStudy.Dominio;

/// <summary>Missão 8 — item que só existe dentro de um PlanoEstudo (construtor internal).</summary>
public class ItemCronograma
{
    private string diaSemana;
    private int minutosPlanejados;

    internal ItemCronograma(string diaSemana, int minutosPlanejados)
    {
        this.diaSemana = diaSemana;
        this.minutosPlanejados = minutosPlanejados;
    }

    public string GetDiaSemana() => diaSemana;
    public int GetMinutosPlanejados() => minutosPlanejados;
}

/// <summary>
/// Missão 8 — Composição (◆──): o PlanoEstudo cria os ItemCronograma lá dentro
/// (recebe só dados primitivos) e nunca devolve a lista para fora.
/// </summary>
public class PlanoEstudo
{
    private List<ItemCronograma> itens = new();

    public void AdicionarItem(string diaSemana, int minutos)
    {
        if (string.IsNullOrWhiteSpace(diaSemana) || minutos <= 0) return;
        itens.Add(new ItemCronograma(diaSemana, minutos));   // o "new" fica DENTRO do plano
    }

    public int CalcularMinutosPlanejadosTotal()
    {
        int total = 0;
        foreach (var i in itens) total += i.GetMinutosPlanejados();
        return total;
    }

    public string GerarResumo()
    {
        if (itens.Count == 0) return "Plano de estudo: nenhum item planejado";

        var sb = new StringBuilder("Plano: ");
        for (int i = 0; i < itens.Count; i++)
        {
            if (i > 0) sb.Append(" · ");
            sb.Append($"{itens[i].GetDiaSemana()} {itens[i].GetMinutosPlanejados()} min");
        }
        sb.Append($" (total {CalcularMinutosPlanejadosTotal()} min)");
        return sb.ToString();
    }
}

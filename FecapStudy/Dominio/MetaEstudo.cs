using System.Text.Json;

namespace FecapStudy.Dominio;

/// <summary>Missão 2 — Encapsulamento: ninguém altera o estado interno sem passar pelas regras.</summary>
public class MetaEstudo : IExportavelJson
{
    private int metaMinutosSemana;
    private int minutosCumpridos;   // sempre começa em 0

    public MetaEstudo(int metaMinutosSemana)
    {
        minutosCumpridos = 0;
        RedefinirMeta(metaMinutosSemana);   // reaproveita a validação (sem repetir if/else)
    }

    public int GetMetaMinutosSemana() => metaMinutosSemana;
    public int GetMinutosCumpridos() => minutosCumpridos;

    /// <summary>Única porta de entrada para somar minutos. Recusa valores &lt;= 0 ou &gt; 480.</summary>
    public bool RegistrarAvanco(int minutos)
    {
        if (minutos <= 0 || minutos > 480) return false;
        minutosCumpridos += minutos;
        return true;
    }

    /// <summary>Usado pelo Responsável. Trava a meta entre 30 e 3000 minutos.</summary>
    public void RedefinirMeta(int novaMeta)
    {
        if (novaMeta < 30) novaMeta = 30;
        if (novaMeta > 3000) novaMeta = 3000;
        metaMinutosSemana = novaMeta;
    }

    /// <summary>De 0 a 100.0 — trava em 100 mesmo que o aluno estude além da meta.</summary>
    public double CalcularPercentual()
    {
        double pct = minutosCumpridos * 100.0 / metaMinutosSemana;
        return Math.Min(100.0, pct);
    }

    public bool EstaCumprida() => minutosCumpridos >= metaMinutosSemana;

    public string ParaJson() => JsonSerializer.Serialize(new
    {
        metaMinutosSemana,
        minutosCumpridos,
        percentual = Math.Round(CalcularPercentual(), 1)
    }, JsonOpcoes.Padrao);
}

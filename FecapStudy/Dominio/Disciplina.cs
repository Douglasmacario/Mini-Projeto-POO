namespace FecapStudy.Dominio;

/// <summary>Missão 1 — Abstração, Classe e Objeto.</summary>
public class Disciplina
{
    private string codigo;
    private string nome;
    private string categoria;
    private int cargaHorariaSemanalHoras;

    public Disciplina(string codigo, string nome, string categoria, int cargaHorariaSemanalHoras)
    {
        this.codigo = codigo;
        this.nome = nome;
        this.categoria = categoria;
        // Regra da missão: carga <= 0 vira o padrão (2 horas).
        this.cargaHorariaSemanalHoras = cargaHorariaSemanalHoras <= 0 ? 2 : cargaHorariaSemanalHoras;
    }

    public string GetCodigo() => codigo;
    public string GetNome() => nome;
    public string GetCategoria() => categoria;
    public int GetCargaHorariaSemanalHoras() => cargaHorariaSemanalHoras;

    public int CalcularCargaEmMinutos() => cargaHorariaSemanalHoras * 60;

    public string ExibirResumo() =>
        $"[{codigo}] {nome} ({categoria}) — {cargaHorariaSemanalHoras}h/semana ({CalcularCargaEmMinutos()} min)";

    // Usado pelo ComboBox da tela.
    public override string ToString() => $"[{codigo}] {nome}";
}

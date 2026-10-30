namespace FecapStudy.Dominio;

/// <summary>
/// Missão 3 — Classe abstrata: não existe "usuário genérico" no sistema,
/// toda pessoa é Administrador, Responsável ou Discente.
/// </summary>
public abstract class Usuario
{
    private int id;
    private string nome;
    private string email;
    private string senha;   // dado privado: NÃO existe getter de senha

    protected Usuario(int id, string nome, string email, string senha)
    {
        this.id = id;
        this.nome = nome;
        this.email = email;
        this.senha = senha;
    }

    public int GetId() => id;
    public string GetNome() => nome;
    public string GetEmail() => email;

    public bool Autenticar(string senhaDigitada) => senha == senhaDigitada;

    public abstract string ObterTipoPerfil();
    public abstract string GerarPainelResumo();
}

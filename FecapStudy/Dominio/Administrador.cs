namespace FecapStudy.Dominio;

public class Administrador : Usuario
{
    private string cargo;

    public Administrador(int id, string nome, string email, string senha, string cargo)
        : base(id, nome, email, senha)
    {
        this.cargo = cargo;
    }

    public string GetCargo() => cargo;

    public override string ObterTipoPerfil() => "Administrador";

    public override string GerarPainelResumo() =>
        $"Administrador: {GetNome()}{Environment.NewLine}Cargo: {cargo}";
}

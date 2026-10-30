namespace FecapStudy.Dominio;

/// <summary>Missão 6 — contrato: o sistema de alertas não sabe COMO a mensagem viaja.</summary>
public interface INotificador
{
    string ObterCanal();
    bool EnviarAlerta(string destinatario, string mensagem);
}

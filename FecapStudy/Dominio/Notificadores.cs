using System.Text.Json;
using System.Text.Json.Serialization;

namespace FecapStudy.Dominio;

/// <summary>Opções de JSON compartilhadas (mantém acentos legíveis).</summary>
internal static class JsonOpcoes
{
    public static readonly JsonSerializerOptions Padrao = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}

/// <summary>
/// Base dos notificadores. Aqui o envio é SIMULADO: em vez de chamar um servidor de e-mail
/// ou a API do WhatsApp, devolvemos o texto para quem quiser exibir (a tela, por exemplo).
/// O domínio continua sem saber que existe uma interface gráfica.
/// </summary>
public abstract class NotificadorBase : INotificador
{
    private readonly Action<string> aoEnviar;

    protected NotificadorBase(Action<string> aoEnviar = null)
    {
        this.aoEnviar = aoEnviar;
    }

    public abstract string ObterCanal();

    public bool EnviarAlerta(string destinatario, string mensagem)
    {
        if (string.IsNullOrWhiteSpace(destinatario) || string.IsNullOrWhiteSpace(mensagem))
            return false;

        aoEnviar?.Invoke($"Canal: {ObterCanal()}{Environment.NewLine}Para: {destinatario}{Environment.NewLine}{Environment.NewLine}{mensagem}");
        return true;
    }

    // Faz o canal aparecer com o nome certo dentro de um ComboBox.
    public override string ToString() => ObterCanal();
}

public class NotificadorEmail : NotificadorBase
{
    public NotificadorEmail(Action<string> aoEnviar = null) : base(aoEnviar) { }
    public override string ObterCanal() => "E-mail";
}

public class NotificadorWhatsApp : NotificadorBase
{
    public NotificadorWhatsApp(Action<string> aoEnviar = null) : base(aoEnviar) { }
    public override string ObterCanal() => "WhatsApp";
}

public class NotificadorPainel : NotificadorBase
{
    public NotificadorPainel(Action<string> aoEnviar = null) : base(aoEnviar) { }
    public override string ObterCanal() => "Painel";
}

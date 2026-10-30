namespace FecapStudy.Dominio;

/// <summary>
/// Missão 8 — Dependência (- - ->): usa Discente, MetaEstudo e INotificador só durante
/// o método. Não guarda nenhum deles em campo, então não há campos de instância.
/// </summary>
public class ServicoAlertaResponsavel
{
    public bool AvaliarDiscente(Discente d, MetaEstudo meta, Responsavel resp, INotificador notificador)
    {
        double pct = meta.CalcularPercentual();

        string mensagem = pct >= 100.0
            ? $"Parabéns! {d.GetNome()} atingiu 100% da meta semanal ({meta.GetMinutosCumpridos()} min estudados)."
            : $"{d.GetNome()} está com {pct:F0}% da meta semanal ({meta.GetMinutosCumpridos()} de {meta.GetMetaMinutosSemana()} min).";

        return notificador.EnviarAlerta(resp.GetEmail(), mensagem);
    }
}

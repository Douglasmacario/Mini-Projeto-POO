using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using FecapStudy.Dominio;

namespace FecapStudy.Servicos;

/// <summary>
/// Missão 10 — conversa com o BackendEstudoAPI.exe em http://localhost:5080.
/// Os métodos NUNCA estouram exceção por falta de rede: devolvem false/null e guardam
/// o motivo em UltimoErro. Assim a tela continua funcionando mesmo com o backend desligado.
/// </summary>
public class ClienteBackendEstudo
{
    public const string EnderecoBase = "http://localhost:5080";

    private static readonly HttpClient http = new()
    {
        BaseAddress = new Uri(EnderecoBase),
        Timeout = TimeSpan.FromSeconds(5)
    };

    private static readonly JsonSerializerOptions opcoes = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public string UltimoErro { get; private set; } = "";

    /// <summary>GET /api/status — o backend está no ar?</summary>
    public async Task<bool> VerificarStatusAsync()
    {
        return await ObterTextoAsync("/api/status") != null;
    }

    /// <summary>POST /api/sessoes — grava a sessão concluída (usa o ParaJson da própria sessão).</summary>
    public async Task<bool> SalvarSessaoAsync(string nomeDiscente, SessaoEstudo sessao)
    {
        // A sessão (IExportavelJson) já gera quase tudo; só acrescentamos quem estudou.
        JsonObject obj = JsonNode.Parse(sessao.ParaJson()).AsObject();
        obj["discenteNome"] = nomeDiscente;
        return await EnviarJsonAsync("/api/sessoes", obj.ToJsonString(opcoes));
    }

    /// <summary>POST /api/metas — cria ou atualiza a meta semanal do discente.</summary>
    public async Task<bool> SalvarMetaAsync(string nomeDiscente, string nomeResponsavel, MetaEstudo meta)
    {
        string json = JsonSerializer.Serialize(new
        {
            discenteNome = nomeDiscente,
            responsavelNome = nomeResponsavel,
            metaMinutosSemana = meta.GetMetaMinutosSemana()
        }, opcoes);
        return await EnviarJsonAsync("/api/metas", json);
    }

    /// <summary>GET /api/sessoes — devolve o JSON bruto (ou null se falhar).</summary>
    public Task<string> ListarSessoesJsonAsync() => ObterTextoAsync("/api/sessoes");

    /// <summary>GET /api/relatorio/{nome} — boletim consolidado do discente.</summary>
    public Task<string> ObterRelatorioJsonAsync(string nomeDiscente) =>
        ObterTextoAsync("/api/relatorio/" + Uri.EscapeDataString(nomeDiscente));

    // ───────── internos ─────────

    private async Task<bool> EnviarJsonAsync(string rota, string json)
    {
        try
        {
            using var conteudo = new StringContent(json, Encoding.UTF8, "application/json");
            using var resposta = await http.PostAsync(rota, conteudo);
            if (!resposta.IsSuccessStatusCode)
            {
                UltimoErro = $"O servidor respondeu HTTP {(int)resposta.StatusCode}.";
                return false;
            }
            UltimoErro = "";
            return true;
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            UltimoErro = "Não foi possível falar com o backend (ele está rodando?).";
            return false;
        }
    }

    private async Task<string> ObterTextoAsync(string rota)
    {
        try
        {
            using var resposta = await http.GetAsync(rota);
            if (!resposta.IsSuccessStatusCode)
            {
                UltimoErro = $"O servidor respondeu HTTP {(int)resposta.StatusCode}.";
                return null;
            }
            UltimoErro = "";
            return await resposta.Content.ReadAsStringAsync();
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            UltimoErro = "Não foi possível falar com o backend (ele está rodando?).";
            return null;
        }
    }
}

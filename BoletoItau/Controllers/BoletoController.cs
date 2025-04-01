using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

namespace BoletoItau.Controllers
{
    [ApiController]
    [Route("api/boleto")]
    public class BoletoController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BoletoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("emitir")]
        public async Task<IActionResult> EmitirBoleto()
        {
            var certificado = new X509Certificate2("certificado.pfx", "senha-certificado");

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificado);

            var client = new HttpClient(handler);

            // Passo 1: Autenticar
            var authContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", "SEU_CLIENT_ID"),
            new KeyValuePair<string, string>("client_secret", "SEU_CLIENT_SECRET")
        });

            var authResponse = await client.PostAsync("https://api.itau.com.br/auth/token", authContent);
            var authJson = await authResponse.Content.ReadFromJsonAsync<AuthResponse>();

            // Passo 2: Emitir Boleto
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authJson.access_token);

            var boletoPayload = new
            {
                numeroCarteira = "109",
                codigoEspecie = "01",
                dataVencimento = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd"),
                valorNominal = 150.75,
                pagador = new
                {
                    cpfCnpj = "12345678901",
                    nome = "João da Silva",
                    endereco = new
                    {
                        logradouro = "Rua Exemplo",
                        bairro = "Centro",
                        cidade = "São Paulo",
                        uf = "SP",
                        cep = "01010000"
                    }
                }
            };

            var response = await client.PostAsJsonAsync("https://api.itau.com.br/cobranca/v2/boletos", boletoPayload);
            var json = await response.Content.ReadAsStringAsync();

            return Ok(json);
        }
    }

    public class AuthResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}

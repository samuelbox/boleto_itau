using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoletoItau.Controllers
{
    [ApiController]
    [Route("webhook/itau")]
    public class WebhookController : ControllerBase
    {
        [HttpPost("boleto")]
        public IActionResult ReceberNotificacao([FromBody] ItauWebhook payload)
        {        
            Console.WriteLine($"Recebido: {payload.codigoEstadoTituloCobranca}, Linha: {payload.linhaDigitavel}, Pago: {payload.valorPago}");
            return Ok();
        }
    }

    public class ItauWebhook
    {
        public string codigoEstadoTituloCobranca { get; set; }
        public string linhaDigitavel { get; set; }
        public decimal valorPago { get; set; }
        public DateTime dataOcorrencia { get; set; }
    }
}

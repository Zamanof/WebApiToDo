using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TO_DO.HostedServices;

namespace TO_DO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly MessageQueue _queue;

        public TransactionController(ILogger<TransactionController> logger, MessageQueue queue)
        {
            _logger = logger;
            _queue = queue;
        }

        [HttpPost]
        public async Task<ActionResult> CreateTransaction(CreateTransactionRequest request)
        {
            _queue.Enqueue(request);
            //Task.Run(async () =>
            //{
            //    _logger.LogError("Transaction Begin");
            //    await Task.Delay(5000);
            //    _logger.LogError("Transaction End");
            //});
            return Ok();
        }
    }
}

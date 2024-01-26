using Microsoft.AspNetCore.Mvc;
using DownStream.Services;
using DownStream.Models;

namespace DownStream.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NodeController : ControllerBase
    {
        private readonly ILogger<NodeController> _logger;
        private readonly NodeServices _service;

        public NodeController(ILogger<NodeController> logger)
        {
            _logger = logger;
            _service = new NodeServices();
        }

        [HttpPost(Name = "GetDownSteamNodes")]
        public IActionResult Post(Request json)
        {
            var response = new Response
            {
                DownStreamCustomers = []
            };

            try
            {
                string error = string.Empty;
                if (_service.GenerateNodes(json.Network.Branches, json.Network.Customers, out error))
                {
                    var customers = _service.QueryCustomersFromNode(json.SelectedNode);
                    response.DownStreamCustomers = customers;
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message);

                response.Errors = new ErrorDetails
                {
                    StatusCode = ErrorCodes.GeneralException,
                    Message = ex.Message
                };

                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}

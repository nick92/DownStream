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
        private NodeServices _service;

        public NodeController(ILogger<NodeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post(Request json)
        {
            var response = new Response
            {
                DownStreamCustomers = []
            };

            _service = new NodeServices(json.Network.Branches, json.Network.Customers);

            try
            {
                string error = string.Empty;
                if (_service.GenerateNodes(out error))
                {
                    var customers = _service.QueryCustomersFromNode(json.SelectedNode, out error);
                    if (customers.Count.Equals(0) && !string.IsNullOrEmpty(error))
                    {
                        throw new Exception(error);
                    }

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

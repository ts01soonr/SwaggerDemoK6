using Microsoft.AspNetCore.Mvc;
using SwaggerDemo.Services;
using SwaggerDemo.Model;

namespace SwaggerDemo.Controllers
{
    [Route("api/v2")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _service;

        public ClientController(IClientService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var emps = _service.GetAll();
            return Ok(emps);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var emp = _service.GetById(id);

            if (emp == null)
                return NotFound();

            return Ok(emp);
        }

        [HttpPost]
        public ActionResult Post([FromBody] Client emp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newEmp = _service.Add(emp);
            return CreatedAtAction("Get", newEmp);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (_service.Delete(id))
                return Ok("The employee is deleted.");
            else
                return NotFound("The employee not found.");
        }
    }
}

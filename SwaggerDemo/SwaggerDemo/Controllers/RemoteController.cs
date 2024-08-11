using Microsoft.AspNetCore.Mvc;
using SwaggerDemo.Services;
using SwaggerDemo.Model;

namespace SwaggerDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemoteController : ControllerBase
    {
        private readonly IRemoteService _service;

        public RemoteController(IRemoteService service)
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
        public ActionResult Post([FromBody] Remote emp)
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
                return Ok("The remote record is deleted.");
            else
                return NotFound("The record not found.");
        }
    }
}

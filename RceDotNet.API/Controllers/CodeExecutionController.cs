using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RceDotNet.API.Models;
using RceDotNet.API.Repositories;

namespace RceDotNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeExecutionController : ControllerBase
    {
        private readonly ICodeExecution codeExecution;
        public CodeExecutionController(ICodeExecution codeExecution)
        {
            this.codeExecution = codeExecution;
        }
        [HttpPost]
        public async Task<IActionResult> ExecuteCode([FromBody] ExecuteCodeRequest request)
        {
            //Check for validity of model, as defined in the ExecuteCodeRequest DTO Model
            if (ModelState.IsValid)
            {
                List<string> response = await codeExecution.ExecuteCodeAsync(request.Code, request.Language,request.Input);
                if (response[1].Length!=0) return BadRequest(response[1]);
                return Ok(response[0]);
            }
            return BadRequest(ModelState.IsValid);
        }
    }
}

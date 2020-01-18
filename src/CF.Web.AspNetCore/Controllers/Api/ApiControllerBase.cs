using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CF.Web.AspNetCore.Controllers.Api
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {

    }
}
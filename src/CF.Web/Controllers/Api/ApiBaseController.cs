using Microsoft.AspNetCore.Mvc;

namespace CF.Web.Controllers.Api
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {

    }
}
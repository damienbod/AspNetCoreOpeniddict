using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceServer.Model;
using ResourceServer.Repositories;

namespace ResourceServer.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class DataEventRecordsController : Controller
{
    private readonly IDataEventRecordRepository _dataEventRecordRepository;

    public DataEventRecordsController(IDataEventRecordRepository dataEventRecordRepository)
    {
        _dataEventRecordRepository = dataEventRecordRepository;
    }

    [EndpointSummary("This is a summary from OpenApi attributes.")]
    [EndpointDescription("This is a description from OpenApi attributes.")]
    [Produces(typeof(List<DataEventRecord>))]
    [Authorize("dataEventRecordsUser")]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_dataEventRecordRepository.GetAll());
    }

    [Authorize("dataEventRecordsAdmin")]
    [HttpGet("{id}")]
    public IActionResult Get(long id)
    {
        return Ok(_dataEventRecordRepository.Get(id));
    }

    [Authorize("dataEventRecordsAdmin")]
    [HttpPost]
    public void Post([FromBody] DataEventRecord value)
    {
        _dataEventRecordRepository.Post(value);
    }

    [Authorize("dataEventRecordsAdmin")]
    [HttpPut("{id}")]
    public void Put(long id, [FromBody] DataEventRecord value)
    {
        _dataEventRecordRepository.Put(id, value);
    }

    [Authorize("dataEventRecordsAdmin")]
    [HttpDelete("{id}")]
    public void Delete(long id)
    {
        _dataEventRecordRepository.Delete(id);
    }
}

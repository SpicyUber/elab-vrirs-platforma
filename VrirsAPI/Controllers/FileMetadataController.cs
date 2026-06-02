using Application.Queries.FileMetadata;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VrirsAPI.Controllers
{
    [Route("api/downloads")]
    [ApiController]
    public class FileMetadataController : ControllerBase
    {
        private readonly IMediator mediator;

        public FileMetadataController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> Download(Guid fileId)
        {
            try
            {
                var download = await mediator.Send(new DownloadQuery() { FileId = fileId });
                return File(download.Stream, download.ContentType, download.FileNameWithExtension);
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

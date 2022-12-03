using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.Data.Models;
using InventoryManagement.Data.DataTransferObjects;
using InventoryManagement.Filters;

namespace InventoryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ExceptionHandler))]
    public class RecordController : ControllerBase
    {
        private readonly ILogger<RecordController> _log;
        private readonly InventoryContext _context;

        public RecordController(ILogger<RecordController> logger, InventoryContext context)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecordDto>>> Get()
        {
            var records = await _context.Record.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).ToListAsync().ConfigureAwait(false);
            if (records is null)
            {
                _log.LogWarning("Record > Get: Did not retrieve any Records");
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"Records {Constants.EntityNotFoundMessage}");
                return NotFound();
            }

            return Ok(records.Select(r => r.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Record>> Get(Guid id)
        {
            var record = await _context.Record.FindAsync(id);
            if (record is null)
            {
                _log.LogWarning("Record > Get{{id}}: Did not retrieve Record with Id:{RecordId}", id);
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"Record {Constants.EntityNotFoundMessage}");
                return NotFound();
            }

            return record;
        }

        [HttpPost]
        public async Task<ActionResult<Record>> Post(RecordDto recordDto)
        {
            if (recordDto is null)
            {
                _log.LogInformation("Record > Post: Record {ResponseMessage}", Constants.ObjectMissingMessage);
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"Record {Constants.ObjectMissingMessage}");
                return UnprocessableEntity();
            }

            var newRecord = new Record
            {
                Month = recordDto.Month,
                Year = recordDto.Year
            };

            _context.Record.Add(newRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = newRecord.Id }, newRecord);
        }

        [HttpPut]
        public async Task<IActionResult> Put(RecordDto recordDto)
        {
            if (recordDto is null)
            {
                _log.LogInformation("Record > Put: Record {ResponseMessage}", Constants.ObjectMissingMessage);
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"Record {Constants.ObjectMissingMessage}");
                return UnprocessableEntity();
            }

            var record = _context.Record.FirstOrDefaultAsync(r => r.Id == recordDto.Id).Result;
            if (record is null)
            {
                _log.LogWarning("Record > Put: Did not retrieve Record with Id:{RecordId}", recordDto.Id);
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"Record {Constants.EntityNotFoundMessage}");
                return NotFound();
            }

            _context.Entry(record).CurrentValues.SetValues(recordDto);
            await _context.SaveChangesAsync();

            return Ok();
        }

        #region HttpDelete Not Implemented
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteRecord(Guid id)
        //{
        //    throw new NotImplementedException();
        //    //var record = await _context.Record.FindAsync(id);
        //    //if (record == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    //if (_context.RecordItem.Any(r => r.RecordId == record.Id))
        //    //{
        //    //    return StatusCode(403, $"The Record marked '{record.Month}'/'{record.Year}' has children and cannot be deleted.");
        //    //}

        //    //_context.Record.Remove(record);
        //    //await _context.SaveChangesAsync();

        //    //return NoContent();
        //}
        #endregion

    }
}

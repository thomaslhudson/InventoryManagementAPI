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
    public class RecordItemController : ControllerBase
    {
        private readonly ILogger<RecordController> _log;
        private readonly InventoryContext _context;

        public RecordItemController(ILogger<RecordController> logger, InventoryContext context)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecordItem>>> Get()
        {
            var recordItems = await _context.RecordItem
                .Include(ri => ri.Record)
                .Include(ri => ri.Product)
                .Include(ri => ri.Product.Group)
                .OrderByDescending(ri => ri.Record.Year)
                .ThenBy(ri => ri.Record.Month)
                .ThenBy(ri => ri.Product.Name)
                .ToListAsync();

            return Ok(recordItems.Select(ri => ri.ToDto()));
            //return StatusCode(501, "Not Implemented");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecordItemDto>> Get(Guid id)
        {
            var recordItem = await _context.RecordItem
                .Where(ri => ri.Id == id)
                .Include(ri => ri.Record)
                .Include(ri => ri.Product)
                .Include(ri => ri.Product.Group)
                .FirstOrDefaultAsync();

            return Ok(recordItem.ToDto());
        }

        [HttpGet("Product/{productId}")]
        public async Task<ActionResult<IEnumerable<RecordItemDto>>> GetRecordItemsForProduct(Guid productId)
        {
            var recordItems = await _context.RecordItem
                .Include(ri => ri.Record)
                .Include(ri => ri.Record.RecordItems)
                .Include(ri => ri.Product)
                .Include(ri => ri.Product.Group)
                .Where(ri => ri.ProductId == productId)
                .OrderByDescending(ri => ri.Record.Year)
                .OrderByDescending(ri => ri.Record.Month).ToListAsync();

            return Ok(recordItems.Select(ri => ri.ToDto()));
        }

        [HttpGet("Record/{recordId}/Product/{productId}")]
        public async Task<ActionResult<RecordItemDto>> Get(Guid recordId, Guid productId)
        {
            var recordItem = await _context.RecordItem
                .Where(ri => ri.RecordId == recordId && ri.ProductId == productId)
                .Include(ri => ri.Record)
                .Include(ri => ri.Product)
                .Include(ri => ri.Product.Group)
                .FirstOrDefaultAsync();

            return Ok(recordItem.ToDto());
        }

        [HttpGet("Record/{recordId}")]
        public async Task<ActionResult<IEnumerable<RecordItemDto>>> GetRecordItemsByRecordId(Guid recordId)
        {
            var recordItems = await _context.RecordItem
                .Where(ri => ri.RecordId == recordId)
                .Include(ri => ri.Record)
                .Include(ri => ri.Product)
                .Include(ri => ri.Product.Group)
                .OrderBy(ri => ri.Product.Name)
                .ToListAsync();

            return Ok(recordItems.Select(ri => ri.ToDto()));
        }

        [HttpPost]
        public async Task<ActionResult<RecordItemDto>> Post(RecordItemDto recordItemDto)
        {
            if (recordItemDto is null)
            {
                _log.LogInformation("RecordItem > Post: RecordItem {ResponseMessage}", Constants.ObjectMissingMessage);
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"RecordItem {Constants.ObjectMissingMessage}");
                return UnprocessableEntity();
            }

            var recordItem = new RecordItem
            {
                Quantity = recordItemDto.Quantity,
                RecordId = recordItemDto.RecordId,
                ProductId = recordItemDto.ProductId,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            };

            _context.RecordItem.Add(recordItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = recordItem.Id }, recordItem);
        }

        [HttpPut]
        public async Task<IActionResult> Put(RecordItemDto recordItemDto)
        {
            if (recordItemDto is null)
            {
                _log.LogInformation("RecordItem > Put: RecordItem {ResponseMessage}", Constants.ObjectMissingMessage);
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"RecordItem {Constants.ObjectMissingMessage}");
                return UnprocessableEntity();
            }

            var recordItem = _context.RecordItem.FirstOrDefaultAsync(ri => ri.Id == recordItemDto.Id).Result;
            if (recordItem is null)
            {
                _log.LogWarning("RecordItem > Put: Did not retrieve RecordItem with RecordItem Id:{RecordItemId}", recordItemDto.Id);
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"RecordItem {Constants.EntityNotFoundMessage}");
                return NotFound();
            }

            recordItem.Quantity = recordItemDto.Quantity;
            recordItem.DateModified = DateTime.Now;

            _context.Entry(recordItem).Property(ri => ri.Quantity).IsModified = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        #region HttpDelete Not Implemented
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleterecordItem(Guid id)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}

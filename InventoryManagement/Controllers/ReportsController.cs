using InventoryManagement.Data;
using InventoryManagement.Data.Models;
using InventoryManagement.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace InventoryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ExceptionHandler))]
    public class ReportsController : ControllerBase
    {
        private readonly ILogger<RecordController> _log;
        private readonly InventoryContext _context;

        public ReportsController(ILogger<RecordController> logger, InventoryContext context)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("GroupTotalsByRecord/{recordId}")]
        public async Task<ActionResult<IEnumerable<GroupTotalsByRecord>>> GetGroupTotalsByRecord(Guid recordId)
        {
            var groupSubTotalsByRecord = _context.GroupSubTotalsByRecord.FromSqlRaw($"EXEC spGroupTotalsByRecord '{recordId}'").ToList();

            return Ok(groupSubTotalsByRecord);
        }

        //[HttpGet("ProductTotalsByRecord/{recordId}")]
        //public async Task<ActionResult<IEnumerable<decimal>>> GetRecordTotalsByProduct(Guid recordId)
        //{
        //    //var productTotalsByRecordOLDOLD = _context.Product.Select(p => p.Name == "Something");

        //    //.GroupBy(item => item.GroupKey)
        //    //.Select(group => group.Sum(item => item.Aggregate));

        //    //int[] a = new int[] { 1, 2 };
        //    //int[] b = new int[] { 3, 4 };
        //    //var c = a.SelectMany(i => b.Select(j => i * j));

        //    //var student = (from s in ctx.Students
        //    //               where s.StudentName == "Bill"
        //    //               select s).FirstOrDefault<Student>();

        //    //from prdt in db.Inventory
        //    //where prdt.Product.Location.City == "Dallas"
        //    //where prdt.Product.Sum(pri => pri.Price) > 100
        //    //select prdt

        //    //var productTotalsByRecord = _context.RecordItem
        //    //    .Include(ri => ri.Record)
        //    //    .Include(ri => ri.Product)
        //    //    .Include(ri => ri.Product.Group)
        //    //    .Where(ri => ri.RecordId == recordId)
        //    //    .GroupBy(ri => ri.Product.Group.Name).ToList()
        //    //    .SelectMany(ri => ri.Select(ri => ri.Product.UnitPrice * ri.Quantity))
        //    //    .ToList();

        //    var recordItemsByRecordId = _context.RecordItem
        //        .Include(ri => ri.Record)
        //        .Include(ri => ri.Product)
        //        .Include(ri => ri.Product.Group)
        //        .Where(ri => ri.RecordId == recordId).ToList();


        //    return Ok(productTotalsByRecord);
        //}
    }
}

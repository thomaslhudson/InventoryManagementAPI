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
    public class GroupController : ControllerBase
    {
        private readonly ILogger<GroupController> _log;
        private readonly InventoryContext _context;

        public GroupController(ILogger<GroupController> logger, InventoryContext context)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> Get()
        {
            var groups = await _context.Group.OrderBy(g => g.Name).ToListAsync();
            if (groups is null)
            {
                _log.LogWarning("Group > Get: Did not retrieve any Groups");
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"Groups {Constants.EntityNotFoundMessage}");
                return NotFound();
            }

            return Ok(groups.Select(g => g.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> Get(Guid id)
        {
            var group = await _context.Group.FindAsync(id);
            if (group is null)
            {
                _log.LogWarning("Group > Get{{id}}: Did not retrieve Group with Id:{GroupId}", id);
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"Group {Constants.EntityNotFoundMessage}");
                return NotFound();
            }

            return Ok(group);
        }

        [HttpPost]
        public async Task<ActionResult<Group>> Post(Group group)
        {
            if (group is null)
            {
                _log.LogInformation("Group > Post: Group {ResponseMessage}", Constants.ObjectMissingMessage);
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"Group {Constants.ObjectMissingMessage}");
                return UnprocessableEntity();
            }

            _context.Group.Add(group);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = group.Id }, group);
        }

        [HttpPut]
        public async Task<IActionResult> Put(GroupDto groupDto)
        {
            if (groupDto is null)
            {
                _log.LogInformation("Group > Put: Group {ResponseMessage}", Constants.ObjectMissingMessage);
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"Group {Constants.ObjectMissingMessage}");
                return UnprocessableEntity();
            }

            var group = _context.Group.FirstOrDefaultAsync(g => g.Id == groupDto.Id).Result;
            if (group is null)
            {
                _log.LogWarning("Group > Put: Did not retrieve Group with Id:{GroupId}", groupDto.Id);
                Response.Headers.Append(Constants.StatusReasonHeaderKey, $"Group {Constants.EntityNotFoundMessage}");
                return NotFound();
            }

            group.Name = groupDto.Name;
            _context.Entry(group).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }

        #region HttpDelete Not Implemented
        //[HttpDelete("{id}")]  /*  api/Group/{id}  */
        //public async Task<IActionResult> DeleteGroup(Guid id)
        //{
        //    var group = await _context.Group.FindAsync(id);
        //    if (group == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Group.Remove(group);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
        #endregion

        [HttpPost("PopulateGroups/")]
        public async Task<IActionResult> Post()
        {
            if (!_context.Group.Any())
            {
                // Add Groups
                var groups = new Group[]
                {
                    new Group{Name="Beer On Tap"},
                    new Group{Name="Bitters/Syrups"},
                    new Group{Name="Bottled Beer"},
                    new Group{Name="Brandy/Cognac/Armagnac/Pisco/Absinthe/Aquavit"},
                    new Group{Name="Cooking Wine"},
                    new Group{Name="Gin"},
                    new Group{Name="Grappa/Sambucca/Eau-de-Vie"},
                    new Group{Name="Kitchen Wine"},
                    new Group{Name="Liquers"},
                    new Group{Name="Non-Alcoholic Beverages"},
                    new Group{Name="Port/Sherry/Madeira/Dessert Wines"},
                    new Group{Name="Red Wine"},
                    new Group{Name="Rum/Cachaca"},
                    new Group{Name="Sparkling/Rose/Dessert/Fortified"},
                    new Group{Name="Tequila/Mezcal"},
                    new Group{Name="Vermouth"},
                    new Group{Name="Vodka"},
                    new Group{Name="Whiskey/Bourbon/Scotch/Irish"},
                    new Group{Name="White Wine"}
                };

                foreach (Group g in groups)
                {
                    _context.Group.Add(g);
                }

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}

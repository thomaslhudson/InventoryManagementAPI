using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.Data.Models;
using InventoryManagement.Data.DataTransferObjects;
using InventoryManagement.Filters;
using Microsoft.CodeAnalysis;
using System.Net;
using System;

namespace InventoryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ExceptionHandler))]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _log;
        private readonly InventoryContext _context;
        public ProductController(ILogger<ProductController> logger, InventoryContext context)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get()
        {
            var products = await _context.Product.Include(p => p.Group).OrderBy(p => p.Name).ToListAsync();
            return Ok(products.Select(p => p.ToDto()));
        }

        [HttpGet("Id/{id}")]
        public async Task<ActionResult<ProductDto>> Get(Guid id)
        {
            var product = await _context.Product.Include(p => p.Group).FirstOrDefaultAsync(p => p.Id == id);
            return Ok(product.ToDto());
        }

        [HttpGet("Name/{name}")]
        public async Task<ActionResult<ProductDto>> GetProductByName(string name)
        {
            var product = await _context.Product.Include(p => p.Group).FirstOrDefaultAsync(p => p.Name == name);
            return Ok(product.ToDto());
        }

        [HttpGet("Upc/{upc}")]
        public async Task<ActionResult<ProductDto>> GetProductByUpc(string upc)
        {
            var product = await _context.Product.Include(p => p.Group).FirstOrDefaultAsync(p => p.Upc == upc);
            return Ok(product.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Post(ProductDto productDto)
        {
            if (_context.Product.Any(p => p.Name == productDto.Name))
            {
                Response.Headers.Add(Constants.StatusReasonHeaderKey, "A product with that name already exists");
                return Conflict();
            }

            var prodcutDupUpc = _context.Product.FirstOrDefault(p => p.Upc == productDto.Upc);
            if (prodcutDupUpc is not null)
            {
                Response.Headers.Add(Constants.StatusReasonHeaderKey, $"The UPC is already in use by '{prodcutDupUpc.Name}'");
                return Conflict();
            }

            var product = new Product
            {
                Name = productDto.Name,
                Upc = productDto.Upc,
                UnitPrice = productDto.UnitPrice,
                IsActive = productDto.IsActive,
                GroupId = productDto.GroupId
            };

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = product.Id }, product);
        }

        [HttpPut]
        public async Task<IActionResult> Put(ProductDto productDto)
        {
            var product = _context.Product.FirstOrDefaultAsync(p => p.Id == productDto.Id).Result;
            //Product product = null;

            _context.Entry(product).CurrentValues.SetValues(productDto);
            await _context.SaveChangesAsync();
            return Ok();
        }

        #region HttpDelete Currently Not Available
        // Currently not available
        //[HttpDelete("{id}")]  /*  api/Product/{id}  */
        //public async Task<IActionResult> DeleteProduct(Guid id)
        //{
        //    // This method does not actually delete the Product
        //    // it changes the Product.IsActive to 'false'
        //    var product = await _context.Product.FindAsync(id);
        //    if (product is null)
        //        return NotFound();
        //    // _context.Product.Remove(product);  // This will attempt to DELETE the record
        //    product.IsActive = false;
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}
        #endregion

        [HttpPost("PopulateProducts/")]
        public async Task<IActionResult> Post()
        {
            var seqNumbers = Enumerable.Range(1, 999).ToList();
            string upcFormat = "000000000000";

            // Add Beer On Tap Products if needed
            Group groupBeerOnTap = _context.Group.FirstOrDefault(g => g.Name == "Beer On Tap");
            if (groupBeerOnTap is not null && !_context.Product.Any(p => p.GroupId == groupBeerOnTap.Id))
            {
                var productsBeerOnTap = new Product[]
                {
                    new Product { Name = "1903 (Henhouse)" },
                    new Product { Name = "Almanac Cherry Supernova" },
                    new Product { Name = "Annes Marie" },
                    new Product { Name = "Berryessa Mini Separaton Anxiety" },
                    new Product { Name = "Birthday Suit" },
                    new Product { Name = "Book of Ginger" },
                    new Product { Name = "Dark Sarcasm" },
                    new Product { Name = "Fieldwork 1502 Viena Lager" },
                    new Product { Name = "Fort Point KSA" },
                    new Product { Name = "Imaginary Circles  (Hen House)" },
                    new Product { Name = "Iron Springs Casey Jones" },
                    new Product { Name = "Leather Bound Books" },
                    new Product { Name = "Lucky Devil Manifeste" },
                    new Product { Name = "Lucky Devil Oude Kriek" },
                    new Product { Name = "Maui Waui rye IPA" },
                    new Product { Name = "Mighty Dry Cider Golden State" },
                    new Product { Name = "Mr Kites" },
                    new Product { Name = "Murker Junkies" },
                    new Product { Name = "Nutty Operator" },
                    new Product { Name = "Resolute (Hen House)" },
                    new Product { Name = "Rhuberbed Wire" },
                    new Product { Name = "Smoke N Dark" },
                    new Product { Name = "Social  Kitchen Puttin on the Spritz  (Hen House)" },
                    new Product { Name = "Summer Sumac (Henhouse)" },
                    new Product { Name = "WD-40" },
                    new Product { Name = "Play Hard" },
                    new Product { Name = "Ancient Mariner" },
                    new Product { Name = "Front & Follow" },
                    new Product { Name = "Captain Mast " }
                };

                foreach (Product p in productsBeerOnTap)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupBeerOnTap.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Add Bitters/Syrups Products if needed
            Group groupBitters = _context.Group.FirstOrDefault(g => g.Name == "Bitters/Syrups");
            if (groupBitters is not null && !_context.Product.Any(p => p.GroupId == groupBitters.Id))
            {
                var productsBitters = new Product[]
                {
                    new Product { Name = "Angostura Bitters" },
                    new Product { Name = "angostura orange" },
                    new Product { Name = "Barkeep Baked Apple" },
                    new Product { Name = "Barkeep Fennel" },
                    new Product { Name = "Barkeep Lavender Spice" },
                    new Product { Name = "Barkeep Swedish Herb" },
                    new Product { Name = "Bittermens Boston Bittahs" },
                    new Product { Name = "Bittermens Burlesque" },
                    new Product { Name = "Bittermens Elemakale Tiki" },
                    new Product { Name = "Bittermens Hopped Grapefruit" },
                    new Product { Name = "Bittermens Orange Cream Citrate" },
                    new Product { Name = "Bittermens Orchard St. Celery Shrub" },
                    new Product { Name = "Bittermens Xocalatl Mole" },
                    new Product { Name = "Branca Menta Bitter" },
                    new Product { Name = "Depaz Cane Syrup" },
                    new Product { Name = "Fee Brothers Grenadine" },
                    new Product { Name = "Fee Brothers Grapefruit Bitters" },
                    new Product { Name = "Fee Brothers Black Walnut Bitters" },
                    new Product { Name = "Peychaud Bitters" },
                    new Product { Name = "Scrappy Fire" },
                    new Product { Name = "Small Hands Orgeat" },
                    new Product { Name = "Velvet Falernum" }
                };

                foreach (Product p in productsBitters)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupBitters.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();

            }

            // Add Bottled Beer Products if needed
            Group groupBottledBeer = _context.Group.FirstOrDefault(g => g.Name == "Bottled Beer");
            if (groupBottledBeer is not null && !_context.Product.Any(p => p.GroupId == groupBottledBeer.Id))
            {
                var productsBottledBeer = new Product[]
                {
                        new Product { Name = "Hamms" },
                        new Product { Name = "PBR" }
                };

                foreach (Product p in productsBottledBeer)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupBottledBeer.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Add Brandy/Cognac/Armagnac/Pisco/Absinthe/Aquavit Products if needed 
            Group groupBrandy = _context.Group.FirstOrDefault(g => g.Name == "Brandy/Cognac/Armagnac/Pisco/Absinthe/Aquavit");
            if (groupBrandy is not null && !_context.Product.Any(p => p.GroupId == groupBrandy.Id))
            {

                var productsBrandy = new Product[]
                {
                        new Product { Name = "Alto De Carmen" },
                        new Product { Name = "Brennivin Anniversery Edition" },
                        new Product { Name = "Brennivin Cask" },
                        new Product { Name = "Busnel" },
                        new Product { Name = "Camus Cognac" },
                        new Product { Name = "Courvoisier vs" },
                        new Product { Name = "Courvoisier vsop" },
                        new Product { Name = "Delord Bas Armagnac Napolean" },
                        new Product { Name = "Delord Bas Armagnac XO" },
                        new Product { Name = "Frapin Cognac XO" },
                        new Product { Name = "Frapin First" },
                        new Product { Name = "La Muse Verte Absinthe" },
                        new Product { Name = "Marteau Absinthe" },
                        new Product { Name = "Oro Pisco" },
                        new Product { Name = "Paul Giroud Cognac Napolean" },
                        new Product { Name = "Pernod" },
                        new Product { Name = "Raynal Brandy  ---Kitchen" },
                        new Product { Name = "St. George Absinthe" }
                };

                foreach (Product p in productsBrandy)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupBrandy.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();

            }

            // Add Gin Products if needed
            Group groupGin = _context.Group.FirstOrDefault(g => g.Name == "Gin");
            if (groupGin is not null && !_context.Product.Any(p => p.GroupId == groupGin.Id))
            {
                var productsGin = new Product[]
                {
                        new Product { Name = "Bluecoat" },
                        new Product { Name = "Benham's" },
                        new Product { Name = "Bombay Sapphire" },
                        new Product { Name = "Botanist" },
                        new Product { Name = "City of london" },
                        new Product { Name = "Bummer and Lazarus" },
                        new Product { Name = "Crater Lake Gin" },
                        new Product { Name = "DeGeorge Benham's Sonoma Dry Gin" },
                        new Product { Name = "Diep 9 Old Genever" },
                        new Product { Name = "Diep 9 Young Genever" },
                        new Product { Name = "Distillery 209" },
                        new Product { Name = "Fords" },
                        new Product { Name = "Hayman's Old Tom Gin" },
                        new Product { Name = "Hendrick's" },
                        new Product { Name = "Junipero Gin" },
                        new Product { Name = "Magellan" },
                        new Product { Name = "Oakland Spirt Co. Sea Gin" },
                        new Product { Name = "Plymouth" },
                        new Product { Name = "Rusty Blade" },
                        new Product { Name = "Smooth Ambler Barrel aged" },
                        new Product { Name = "St. George Botanivore" },
                        new Product { Name = "St. George Terroir" },
                        new Product { Name = "St. George Dry Rye" },
                        new Product { Name = "saffron gin" },
                        new Product { Name = "Tanqueray Gin" },
                        new Product { Name = "Uncle Vals Botanical" },
                        new Product { Name = "Old Raj" },
                        new Product { Name = "OSCO Dry" },
                        new Product { Name = "OSCO #5" }
                };

                foreach (Product p in productsGin)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupGin.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();

            }

            // Add Grappa/Sambucca/Eau-de-Vie Products if needed
            Group groupGrappa = _context.Group.FirstOrDefault(g => g.Name == "Grappa/Sambucca/Eau-de-Vie");
            if (groupGrappa is not null && !_context.Product.Any(p => p.GroupId == groupGrappa.Id))
            {
                if (groupGrappa is not null)
                {
                    var productsGrappa = new Product[]
                    {
                        new Product { Name = "Antica Sambucca" },
                        new Product { Name = "Clear Creek Muscat Grappa" },
                        new Product { Name = "Clear Creek Pinot Grigio Grappa" },
                        new Product { Name = "Jacopo Poli Moscato" },
                        new Product { Name = "Jacopo Poli Torcolato" },
                        new Product { Name = "Lorenzo Inga Grappa di Gavi" },
                        new Product { Name = "Lorenzo Inga My Sambucca" },
                        new Product { Name = "Michele Chiarlo Grappa di Barolo" },
                        new Product { Name = "Nivole Grappa di Moscato d'Asti" },
                        new Product { Name = "Poli Miel" },
                        new Product { Name = "Poli Mirtillo" },
                        new Product { Name = "Singani" },
                        new Product { Name = "Sibona Reserva" }
                    };

                    foreach (Product p in productsGrappa)
                    {
                        var intFormatted = seqNumbers.First().ToString(upcFormat);
                        seqNumbers.Remove(seqNumbers.First());

                        p.Upc = intFormatted;
                        p.UnitPrice = 1.00m;
                        p.IsActive = true;
                        p.GroupId = groupGrappa.Id;

                        _context.Product.Add(p);
                    }

                    await _context.SaveChangesAsync();
                }

            }

            // Add Kitchen Wine Products if needed
            Group groupKitchenWine = _context.Group.FirstOrDefault(g => g.Name == "Kitchen Wine");
            if (groupKitchenWine is not null && !_context.Product.Any(p => p.GroupId == groupKitchenWine.Id))
            {
                var productsKitchenWine = new Product[]
                {
                        new Product { Name = "Labarthe Gaillac Blanc" },
                        new Product { Name = "Farnese Trebbiano" },
                        new Product { Name = "Farnese Montepulciano" },
                        new Product { Name = "Blandy Rainwater Madera" },
                        new Product { Name = "Fairbanks Sherry" },
                        new Product { Name = "Gallo Dry Vermouth" }
                };

                foreach (Product p in productsKitchenWine)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupKitchenWine.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Add Liqueurs Products if needed
            Group groupLiquers = _context.Group.FirstOrDefault(g => g.Name == "Liquers");
            if (groupLiquers is not null && !_context.Product.Any(p => p.GroupId == groupLiquers.Id))
            {
                var productsLiquers = new Product[]
                {
                        new Product { Name = "Paolucci Amaro CioCiaro" },
                        new Product { Name = "St Elizazbeth All Spice Dram" },
                        new Product { Name = "Amaro Montenegro" },
                        new Product { Name = "Amaro Nonino" },
                        new Product { Name = "Amaro Sfumato" },
                        new Product { Name = "Anco Reyes" },
                        new Product { Name = "Aperol" },
                        new Product { Name = "Averna" },
                        new Product { Name = "Bailey's" },
                        new Product { Name = "Benedictine" },
                        new Product { Name = "Bols Curacao" },
                        new Product { Name = "Boudier Vanilla Liq" },
                        new Product { Name = "Branca Menta" },
                        new Product { Name = "Brisson Pineau des Charentes" },
                        new Product { Name = "Café Amaro" },
                        new Product { Name = "Calisaya" },
                        new Product { Name = "Campari" },
                        new Product { Name = "Canton Ginger Liquer" },
                        new Product { Name = "chambord" },
                        new Product { Name = "Cointreau Orange Liquer" },
                        new Product { Name = "Combier cherry" },
                        new Product { Name = "Combier Kummel" },
                        new Product { Name = "Combier Orange" },
                        new Product { Name = "Cynar" },
                        new Product { Name = "Dillons Rose Liq" },
                        new Product { Name = "Drambuie" },
                        new Product { Name = "Emperor Norton Absinthe" },
                        new Product { Name = "Fernet Branca" },
                        new Product { Name = "Frangelico" },
                        new Product { Name = "Galliano L'autentico" },
                        new Product { Name = "Gioia Luisa Limoncello" },
                        new Product { Name = "Gran Classico Bitters" },
                        new Product { Name = "Grand Marnier" },
                        new Product { Name = "Green Chartreuse" },
                        new Product { Name = "Heering Cherry Liquer" },
                        new Product { Name = "Herbsaint Anis Liquer" },
                        new Product { Name = "Homebase Liqueur" },
                        new Product { Name = "Kahlua" },
                        new Product { Name = "Kronan Swedish Punsch" },
                        new Product { Name = "Laird's Applejack" },
                        new Product { Name = "Lillet" },
                        new Product { Name = "Lillet Rouge" },
                        new Product { Name = "Luxardo Amaretto" },
                        new Product { Name = "Luxardo Maraschino Liquer" },
                        new Product { Name = "Merlet Cassis" },
                        new Product { Name = "Merlet Triple Sec" },
                        new Product { Name = "Montenegro" },
                        new Product { Name = "Oakland Spirt Co. Lemon Grass" },
                        new Product { Name = "Oakland Spirt Co. Shiso" },
                        new Product { Name = "pages parfait amour" },
                        new Product { Name = "Rothman and Winter Creme de violet" },
                        new Product { Name = "Pamplemouse rose liqueur" },
                        new Product { Name = "Pimms" },
                        new Product { Name = "Pur Pear" },
                        new Product { Name = "S. Maria al Monte Amaro" },
                        new Product { Name = "Spirt Works Barrel Aged Sloe Gin" },
                        new Product { Name = "St. George Bruto Americano" },
                        new Product { Name = "St. Germain Elderflower" },
                        new Product { Name = "Tempus Fugit Quinovina" },
                        new Product { Name = "Tempus Fugit Crème de Menthe" },
                        new Product { Name = "Tuaca" },
                        new Product { Name = "Yellow Chartreuse" },
                        new Product { Name = "St. Elder elder flower liqerur" },
                        new Product { Name = "grand poppy liqerur" },
                        new Product { Name = "St. George NOLA" },
                        new Product { Name = "St. George Pear" }
                };

                foreach (Product p in productsLiquers)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupLiquers.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Non-Alcoholic Beverages Products
            Group groupNonAlcoholic = _context.Group.FirstOrDefault(g => g.Name == "Non-Alcoholic Beverages");
            if (groupNonAlcoholic is not null && !_context.Product.Any(p => p.GroupId == groupNonAlcoholic.Id))
            {
                var productsNonAlcoholic = new Product[]
                {
                        new Product { Name = "Abita Root Beer" },
                        new Product { Name = "Bundaberg Ginger Beer" },
                        new Product { Name = "Coke" },
                        new Product { Name = "Diet Coke" },
                        new Product { Name = "Fentimans Tonic Water" },
                        new Product { Name = "Fever tree ginger ale" },
                        new Product { Name = "San Pellegrino Aranciata" },
                        new Product { Name = "San Pellegrino Limonata" },
                        new Product { Name = "Saratoga  LARGE" },
                        new Product { Name = "SaratogaSparkling Water SMALL" },
                        new Product { Name = "Sprite" },
                        new Product { Name = "Alta Palla" },
                        new Product { Name = "Erdinger" }
                };

                foreach (Product p in productsNonAlcoholic)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupNonAlcoholic.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Add Port/Sherry/Madeira/Dessert Wines Products if needed
            Group groupPortSherry = _context.Group.FirstOrDefault(g => g.Name == "Port/Sherry/Madeira/Dessert Wines");
            if (groupPortSherry is not null && !_context.Product.Any(p => p.GroupId == groupPortSherry.Id))
            {
                var productsPortSherry = new Product[]
                {
                        new Product { Name = "Blandy's Madeira 10 yr" },
                        new Product { Name = "Blandy's Rainwater Madeira" },
                        new Product { Name = "Broadbent Boal" },
                        new Product { Name = "Broadbent Colheita 1996" },
                        new Product { Name = "Broadbent Malmsey 10yr" },
                        new Product { Name = "Broadbent Sercial" },
                        new Product { Name = "Broadbent Verdeiho" },
                        new Product { Name = "Chalk Hill Semillon" },
                        new Product { Name = "Croft Ruby Port" },
                        new Product { Name = "Ferreira 10 yr Port" },
                        new Product { Name = "Ferreira 20 yr Port" },
                        new Product { Name = "Fonseca Late Bottled Port" },
                        new Product { Name = "Graham's 6 Grapes Port" },
                        new Product { Name = "Graham's Late Bottled Port" },
                        new Product { Name = "Auraro Manzanilla" },
                        new Product { Name = "Noval Black" },
                        new Product { Name = "Quinta do Crasto LBV" },
                        new Product { Name = "Quinta do Nova LBV" },
                        new Product { Name = "Roche Late Harvest" },
                        new Product { Name = "Terra d'Oro Zinfandel Port" },
                        new Product { Name = "Tokaji Hetszolo" },
                        new Product { Name = "Taylor fladgate 20yr" }
                };

                foreach (Product p in productsPortSherry)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupPortSherry.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Add Red Wine Products if needed
            Group groupRedWine = _context.Group.FirstOrDefault(g => g.Name == "Red Wine");
            if (groupRedWine is not null && !_context.Product.Any(p => p.GroupId == groupRedWine.Id))
            {
                var productsRedWine = new Product[]
                {
                        new Product { Name = "Hirsch Pinot 1/2" },
                        new Product { Name = "Bloom Phase" },
                        new Product { Name = "Ardoisieres Rouge" },
                        new Product { Name = "Pecina Rioja" },
                        new Product { Name = "Pence Ranch" },
                        new Product { Name = "Parts + Labor" },
                        new Product { Name = "Grand Puy 97" },
                        new Product { Name = "Montelena \"Calistoga\"" },
                        new Product { Name = "Stonestreet 97" },
                        new Product { Name = "Poco a Poco Zinfandel" },
                        new Product { Name = "Chateau Vray Canon" },
                        new Product { Name = "Rion Chambolle Charmes" },
                        new Product { Name = "Gaunoux Beaune Mouches" },
                        new Product { Name = "girardin pommard epen" },
                        new Product { Name = "bv g latour 84" },
                        new Product { Name = "seghesio 97" },
                        new Product { Name = "A Conterno Langhe" },
                        new Product { Name = "Rolet Poulsard" },
                        new Product { Name = "Balgera" },
                        new Product { Name = "Paul Chapelle Santenay" },
                        new Product { Name = "Camus Bruchon" },
                        new Product { Name = "D. Hernandez Petite Sir" },
                        new Product { Name = "livio Sassetti Brunello" },
                        new Product { Name = "Tribute to Grace" },
                        new Product { Name = "Cheysson Chiroubles" },
                        new Product { Name = "tessier cab franc" },
                        new Product { Name = "Gilles Barge" },
                        new Product { Name = "ahlgren merlot" },
                        new Product { Name = "Cain Cuvee" },
                        new Product { Name = "edmunds st john syrah" },
                        new Product { Name = "Gut Oggau" },
                        new Product { Name = "Ryme Cellars " },
                        new Product { Name = "Bodegas Olarra" }
                };

                foreach (Product p in productsRedWine)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupRedWine.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Add Rum / Cachaca Products if needed
            Group groupRum = _context.Group.FirstOrDefault(g => g.Name == "Rum/Cachaca");
            if (groupRum is not null && !_context.Product.Any(p => p.GroupId == groupRum.Id))
            {
                var productsRum = new Product[]
                {
                        new Product { Name = "Avua Amburana" },
                        new Product { Name = "Avua Oaked" },
                        new Product { Name = "Appelton Estate" },
                        new Product { Name = "Avua Prata" },
                        new Product { Name = "Barbancourt 15yr" },
                        new Product { Name = "Barbancourt 4yr" },
                        new Product { Name = "Flor De Cana 4yr white" },
                        new Product { Name = "Flor De Cana 7yr " },
                        new Product { Name = "Flor De Cana Centenario 12yr" },
                        new Product { Name = "Capurro" },
                        new Product { Name = "Prichard's fine Rum" },
                        new Product { Name = "Hamilton Gold" },
                        new Product { Name = "Matusalem 10yr" },
                        new Product { Name = "Roaring Dans Maple" },
                        new Product { Name = "St. George Agricole" },
                        new Product { Name = "Three Sheets Barrel aged" },
                        new Product { Name = "Whaler's Dark " },
                        new Product { Name = "zaya" },
                        new Product { Name = "Mezan Jamaica" },
                        new Product { Name = "Mezan Panama" }
                };

                foreach (Product p in productsRum)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupRum.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Add Sparkling/Rose/Dessert/Fortified Products if needed
            Group groupSparklingRose = _context.Group.FirstOrDefault(g => g.Name == "Sparkling/Rose/Dessert/Fortified");
            if (groupSparklingRose is not null && !_context.Product.Any(p => p.GroupId == groupSparklingRose.Id))
            {
                var productsSparklingRose = new Product[]
                {
                        new Product { Name = "Inocente Fino Sherry" },
                        new Product { Name = "Jolie-Laide Trousseau" },
                        new Product { Name = "Goutorbe Special Club" },
                        new Product { Name = "Forest Marie" },
                        new Product { Name = "Avinyo Cava" },
                        new Product { Name = "P. Gimonnet 08" },
                        new Product { Name = "Tessier Rose" },
                        new Product { Name = "grahams lbv 94" },
                        new Product { Name = "clos peyraguey" },
                        new Product { Name = "Col Mesian Prosecco" },
                        new Product { Name = "JL Vergnon HLF" },
                        new Product { Name = "Huet Moelleux" },
                        new Product { Name = "Laherte Rose Meunier" }
                };

                foreach (Product p in productsSparklingRose)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupSparklingRose.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Add Tequila/Mezcal Products if needed
            Group groupTequila = _context.Group.FirstOrDefault(g => g.Name == "Tequila/Mezcal");
            if (groupTequila is not null && !_context.Product.Any(p => p.GroupId == groupTequila.Id))
            {
                var productsTequila = new Product[]
                {
                        new Product { Name = "Milagro Silver" },
                        new Product { Name = "7 Leguas Tequila Anejo" },
                        new Product { Name = "7 Leguas Tequila Reposado" },
                        new Product { Name = "7 Leguas Tequla Blanco" },
                        new Product { Name = "Alipus San Andres Mezcal" },
                        new Product { Name = "Cazadores" },
                        new Product { Name = "Cielo Rojo Blanco Mezcal" },
                        new Product { Name = "Cimarron Blanco" },
                        new Product { Name = "Del Maguey Chichicapa Mezcal" },
                        new Product { Name = "Del Maguey Santo Domingo ALbarradas" },
                        new Product { Name = "Del Maguey Cask Finish" },
                        new Product { Name = "Del Maguey Vida Mezcal" },
                        new Product { Name = "Don Abraham Blanco" },
                        new Product { Name = "Don Abraham Reposado" },
                        new Product { Name = "Don Abraham Xtra Anejo" },
                        new Product { Name = "Don Julio Anejo" },
                        new Product { Name = "Don Julio Reposado" },
                        new Product { Name = "Don Julio Silver" },
                        new Product { Name = "El Charro" },
                        new Product { Name = "El Silencio Espadin" },
                        new Product { Name = "El Silencio Joven" },
                        new Product { Name = "Hacienda Vieja Reposado Tequila" },
                        new Product { Name = "Casa Pacifica" },
                        new Product { Name = "Patron Anejo" },
                        new Product { Name = "Peloton de la muerte Mezcal" },
                        new Product { Name = "Tequila Ocho Anejo" },
                        new Product { Name = "Tequila Ocho Plata" },
                        new Product { Name = "Tequila Ocho Reposado" },
                        new Product { Name = "Legendario Domingo" }
                };

                foreach (Product p in productsTequila)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupTequila.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Add Vermouth Products if needed
            Group groupVermouth = _context.Group.FirstOrDefault(g => g.Name == "Vermouth");
            if (groupVermouth is not null && !_context.Product.Any(p => p.GroupId == groupVermouth.Id))
            {
                var productsVermouth = new Product[]
                {
                        new Product { Name = "Boissiere Dry Vermouth" },
                        new Product { Name = "Boissiere Sweet Vermouth" },
                        new Product { Name = "Carpano Antica" },
                        new Product { Name = "Dolin Blanc" },
                        new Product { Name = "Dolin Dry" },
                        new Product { Name = "Dolin Rouge" },
                        new Product { Name = "Dubonnet Rouge" },
                        new Product { Name = "Noilly Pratt Dry" },
                        new Product { Name = "Noilly Pratt Rouge" },
                        new Product { Name = "Punt e Mes" }
                };

                foreach (Product p in productsVermouth)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupVermouth.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Add Vodka Products if needed
            Group groupVodka = _context.Group.FirstOrDefault(g => g.Name == "Vodka");
            if (groupVodka is not null && !_context.Product.Any(p => p.GroupId == groupVodka.Id))
            {
                var productsVodka = new Product[]
                {
                        new Product { Name = "Belvedere" },
                        new Product { Name = "Charbay Grapefruit" },
                        new Product { Name = "Crater Lake Vodka" },
                        new Product { Name = "Golden State Vodka" },
                        new Product { Name = "Grey Goose" },
                        new Product { Name = "Hangar 1 Buddha\'s Hand Citroen" },
                        new Product { Name = "Hangar 1 Chipotle" },
                        new Product { Name = "Hangar 1 Kaffir Lime" },
                        new Product { Name = "Hangar 1 Mandarin" },
                        new Product { Name = "Hangar 1 Straight" },
                        new Product { Name = "Hanson" },
                        new Product { Name = "Ketel One" },
                        new Product { Name = "Seven Stills" },
                        new Product { Name = "Square One Cucumber" },
                        new Product { Name = "St. George all purpose vodka" },
                        new Product { Name = "St. George Citrus" },
                        new Product { Name = "St. George Green Chile" },
                        new Product { Name = "Stolichinaya" },
                        new Product { Name = "Tahoe" },
                        new Product { Name = "Tito\'s Handmade" },
                        new Product { Name = "Tru" },
                        new Product { Name = "Van Gogh Chocolate Vodka" },
                        new Product { Name = "Wheatley" }
                };

                foreach (Product p in productsVodka)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupVodka.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Add Whiskey/Bourbon/Scotch/Irish Products if needed
            Group groupWhiskey = _context.Group.FirstOrDefault(g => g.Name == "Whiskey/Bourbon/Scotch/Irish");
            if (groupWhiskey is not null && !_context.Product.Any(p => p.GroupId == groupWhiskey.Id))
            {
                var productsWhiskey = new Product[]
                {
                        new Product { Name = "1910 Canadian Rye" },
                        new Product { Name = "2nd chance cask strenght" },
                        new Product { Name = "2nd Chance wheat Sonoma" },
                        new Product { Name = "7 stills chocasmpke" },
                        new Product { Name = "Akashi grain malt" },
                        new Product { Name = "Alley 6 rye" },
                        new Product { Name = "Amador Whiskey" },
                        new Product { Name = "american born dixie shine" },
                        new Product { Name = "american born shine" },
                        new Product { Name = "Angel cask strenght" },
                        new Product { Name = "Angel's Envy Bourbon" },
                        new Product { Name = "Angel's Envy rye 100" },
                        new Product { Name = "Athol Brose Scotch Liquer" },
                        new Product { Name = "Amorik" },
                        new Product { Name = "Baker’s" },
                        new Product { Name = "Balcones Baby Blue" },
                        new Product { Name = "Balcones Blue Corn Whiskey" },
                        new Product { Name = "Balcones Brimstone" },
                        new Product { Name = "Balcones single malt" },
                        new Product { Name = "Balvenie 21yr" },
                        new Product { Name = "Balvenie Doublewood 12yr" },
                        new Product { Name = "Balvenie Doublewood 17yr" },
                        new Product { Name = "Balvenie 14yr" },
                        new Product { Name = "Banknote" },
                        new Product { Name = "Barrell Bourbon" },
                        new Product { Name = "Barrel Rye" },
                        new Product { Name = "Barrell Whiskey" },
                        new Product { Name = "Barterhouse 20yr" },
                        new Product { Name = "Basil Hayden’s " },
                        new Product { Name = "Belle Meade   " },
                        new Product { Name = "Belle meade 9yr Sherry" },
                        new Product { Name = "Bernheim 7yr" },
                        new Product { Name = "Big Bottom " },
                        new Product { Name = "Big Bottom Port Finished " },
                        new Product { Name = "Big Bottom Zinfandel Finished " },
                        new Product { Name = "Black Bottle Blended" },
                        new Product { Name = "black bull 21yr" },
                        new Product { Name = "Black Maple Hill " },
                        new Product { Name = "Black Saddle 12yr bourbon" },
                        new Product { Name = "Blade and Bow" },
                        new Product { Name = "Blanton's Bourbon" },
                        new Product { Name = "Bombergers" },
                        new Product { Name = "Booker’s" },
                        new Product { Name = "Breaking & Entering" },
                        new Product { Name = "Breucklen Dist. 77 Rye" },
                        new Product { Name = "Breucklen Dist. 77 Wheat " },
                        new Product { Name = "Buffalo Trace Bourbon" },
                        new Product { Name = "Buffalo Trace White Dog " },
                        new Product { Name = "Bulleit Bourbon" },
                        new Product { Name = "Bulleit Bourbon 10yr" },
                        new Product { Name = "Bulleit Rye" },
                        new Product { Name = "Bruichladdich Scottish Barley (Classic Laddie)" },
                        new Product { Name = "Bruichladdich 2009" },
                        new Product { Name = "Bruichladdich port charlotte" },
                        new Product { Name = "Bruichladdich Back art 1990" },
                        new Product { Name = "Bushmills" },
                        new Product { Name = "Charbay Whiskey" },
                        new Product { Name = "Charbay R5" },
                        new Product { Name = "Charles Goodnight" },
                        new Product { Name = "Clear Creek McCarthy single malt" },
                        new Product { Name = "corbin cash blended" },
                        new Product { Name = "Collier & McKeel" },
                        new Product { Name = "colorado gold" },
                        new Product { Name = "colorado ryte" },
                        new Product { Name = "compassbox spice tree" },
                        new Product { Name = "compassbox hedonism" },
                        new Product { Name = "corsair mosaic" },
                        new Product { Name = "Corsair Ryemageddon" },
                        new Product { Name = "Corsair Triple Smoke" },
                        new Product { Name = "Corsair Quinoa" },
                        new Product { Name = "Crater Lake Rye" },
                        new Product { Name = "Dad’s Hat " },
                        new Product { Name = "Dad’s Hat Cask Strength " },
                        new Product { Name = "Dad’s Hat Port Barrel Finished " },
                        new Product { Name = "Dad’s Hat Vermouth Barrel Finished " },
                        new Product { Name = "Deaths Door Whiskey" },
                        new Product { Name = "Desert wheat" },
                        new Product { Name = "Dewar's White Label" },
                        new Product { Name = "Dewars 12 yr" },
                        new Product { Name = "Dry Fly 101" },
                        new Product { Name = "Dry Fly Cask" },
                        new Product { Name = "Dry Fly Port Finished" },
                        new Product { Name = "Dry Fly Wheat" },
                        new Product { Name = "E.H. Taylor Rye" },
                        new Product { Name = "E.H. Taylor Single Barrel" },
                        new Product { Name = "E.H. Taylor small batch" },
                        new Product { Name = "E.H. Taylor Warehouse C" },
                        new Product { Name = "Eagle Rare 10yr" },
                        new Product { Name = "Eagle Rare 17 yr" },
                        new Product { Name = "EH Taylor Barrel Proof" },
                        new Product { Name = "Elijah Craig 12yr small batch" },
                        new Product { Name = "Elijah Craig 12yr Barrel Proof" },
                        new Product { Name = "Elijah Craig 20yr " },
                        new Product { Name = "Elijah Craig 21yr " },
                        new Product { Name = "Elmer T. Lee " },
                        new Product { Name = "Evan Williams 1783 Small Batch " },
                        new Product { Name = "Evan Williams Single Barrel " },
                        new Product { Name = "Everclear" },
                        new Product { Name = "Famous Grouse" },
                        new Product { Name = "Finians Irish" },
                        new Product { Name = "Four Roses limited" },
                        new Product { Name = "Four Roses Single Barrel " },
                        new Product { Name = "Four Roses Yellow" },
                        new Product { Name = "Four Roses Small Batch " },
                        new Product { Name = "Garrison Brothers Cowboy Bourbon" },
                        new Product { Name = "Garrison Brothers (Spring)" },
                        new Product { Name = "Garrison Brothers (Fall)" },
                        new Product { Name = "George Dickel Barrel Select " },
                        new Product { Name = "george dickel rye" },
                        new Product { Name = "George T. Stagg" },
                        new Product { Name = "George T. Stagg Jr. " },
                        new Product { Name = "Georgia Moonshine" },
                        new Product { Name = "Georgia Apple Pie" },
                        new Product { Name = "Glenfiddich 12yr" },
                        new Product { Name = "Glenfiddich 15yr" },
                        new Product { Name = "Glenfiddich 14yr" },
                        new Product { Name = "Glenfiddich 18yr" },
                        new Product { Name = "Glenfiddich 21yr" },
                        new Product { Name = "Glenmorangie 10yr" },
                        new Product { Name = "Glenmorangie 12yr lasanta" },
                        new Product { Name = "Glanmorangie 12 nectar" },
                        new Product { Name = "Glenmorangie 12yr quinta ruban" },
                        new Product { Name = "Gold Run Rye" },
                        new Product { Name = "Great King Artist's Blend" },
                        new Product { Name = "Greenbar Slow Hand Wood Whiskey" },
                        new Product { Name = "Hakushu 12yr" },
                        new Product { Name = "Handy Sazerac Rye" },
                        new Product { Name = "Henry McKenna 10yr" },
                        new Product { Name = "Henry McKenna Straight" },
                        new Product { Name = "High West 21yr " },
                        new Product { Name = "High West American Prairie" },
                        new Product { Name = "High West Bourye" },
                        new Product { Name = "High West Camp Fire" },
                        new Product { Name = "High West Double Rye " },
                        new Product { Name = "High West Midwinter Night's Dram" },
                        new Product { Name = "High West Rendezvous Rye" },
                        new Product { Name = "High West Silver Oat" },
                        new Product { Name = "High West Yipee Ki Yay" },
                        new Product { Name = "High West SOB " },
                        new Product { Name = "Highland Park 12 yr" },
                        new Product { Name = "Highland Park 18yr" },
                        new Product { Name = "Hirsch 25yr " },
                        new Product { Name = "Hirsch Straight Corn " },
                        new Product { Name = "Homebase Whiskey" },
                        new Product { Name = "Hooker House Pinot Noir Barrel Finished " },
                        new Product { Name = "Hooker House Zinfandel Barrel Finished " },
                        new Product { Name = "Hudson Baby Bourbon" },
                        new Product { Name = "Hudson Corn Whiskey" },
                        new Product { Name = "Hudson Four Grain Bourbon" },
                        new Product { Name = "Hudson Manhattan Rye" },
                        new Product { Name = "Hudson Single Malt" },
                        new Product { Name = "I.W. Harper" },
                        new Product { Name = "I.W. Harper 15yr" },
                        new Product { Name = "James E Pepper bourbon" },
                        new Product { Name = "James E Pepper rye" },
                        new Product { Name = "James E Pepper rye 100 proof" },
                        new Product { Name = "Jameson Irish" },
                        new Product { Name = "Jameson Black" },
                        new Product { Name = "Jameson 18yr" },
                        new Product { Name = "Jefferson’s reserve" },
                        new Product { Name = "Jefferson's ocean" },
                        new Product { Name = "Johnny Drum Private Stock" },
                        new Product { Name = "journeyman bourbon" },
                        new Product { Name = "journeyman rye" },
                        new Product { Name = "journeyman silver cross" },
                        new Product { Name = "Knob Creek " },
                        new Product { Name = "knob creek rye" },
                        new Product { Name = "Knob Creek Single Barrel " },
                        new Product { Name = "Lairds Applejack" },
                        new Product { Name = "Laphroaig 10yr" },
                        new Product { Name = "Laphroaig 18yr" },
                        new Product { Name = "Laphroaig quarter cask" },
                        new Product { Name = "Larceny Small Batch Bourbon" },
                        new Product { Name = "Last Feather Rye" },
                        new Product { Name = "Leopold Bro's Maryland Style Rye " },
                        new Product { Name = "Leopold Bro's Small Batch" },
                        new Product { Name = "Lock Stock & Barrel 13yr " },
                        new Product { Name = "Macallan 12 yr" },
                        new Product { Name = "Masterson's barley" },
                        new Product { Name = "Masterson's rye" },
                        new Product { Name = "Masterson's Wheat" },
                        new Product { Name = "Masterson's American" },
                        new Product { Name = "Masterson's French" },
                        new Product { Name = "McKenzie rye" },
                        new Product { Name = "Medley Brothers Bourbon" },
                        new Product { Name = "Michter;s Toasted barrel" },
                        new Product { Name = "Michter's American " },
                        new Product { Name = "Michter's Bourbon" },
                        new Product { Name = "Michter's Rye" },
                        new Product { Name = "Michter's Sour Mash" },
                        new Product { Name = "Michter’s 10yr Bourbon" },
                        new Product { Name = "Michter's 1oyr Rye" },
                        new Product { Name = "Michter's 20yr" },
                        new Product { Name = "Michter's 25yr" },
                        new Product { Name = "Michter's barrel rye" },
                        new Product { Name = "Monkey shoulder" },
                        new Product { Name = "Mortlach 15yr" },
                        new Product { Name = "Mosswood Apple brandy light whiskey" },
                        new Product { Name = "Mosswood Sour Beer" },
                        new Product { Name = "Mosswood Pinot Noir Barrel" },
                        new Product { Name = "Nikka Miyagikyo 12yr" },
                        new Product { Name = "Nikka coffey grain" },
                        new Product { Name = "Nikka coffey malt" },
                        new Product { Name = "Nikka Taketsru" },
                        new Product { Name = "noah's mill" },
                        new Product { Name = "Oban 14yr" },
                        new Product { Name = "Oban Distilers Edition" },
                        new Product { Name = "Oban Small Distillers" },
                        new Product { Name = "Old Bardstown Black " },
                        new Product { Name = "Old Bardstown Estate " },
                        new Product { Name = "Old Fitzgerald Bonded " },
                        new Product { Name = "old grandad" },
                        new Product { Name = "Old Medley 12yr" },
                        new Product { Name = "Old Medley Private Stock 10yr" },
                        new Product { Name = "Old Overholt Rye" },
                        new Product { Name = "Old Potrero " },
                        new Product { Name = "Old Potrero 18th Century Single Malt " },
                        new Product { Name = "Old Rip van Winkle 10yr" },
                        new Product { Name = "oola bourbon" },
                        new Product { Name = "Orphan Barrel Rhetoric" },
                        new Product { Name = "Orphan Barrel Barterhouse" },
                        new Product { Name = "Orphan Barrel Oak forged" },
                        new Product { Name = "Outlaw Moonshine " },
                        new Product { Name = "Pappy Van Winkle 13yr Rye" },
                        new Product { Name = "Pappy Von Winkle 15 yr" },
                        new Product { Name = "Pappy Von Winkle 20 yr" },
                        new Product { Name = "Pappy Von Winkle 23 yr" },
                        new Product { Name = "Pendleton" },
                        new Product { Name = "Pikesville Rye" },
                        new Product { Name = "Pow Wow" },
                        new Product { Name = "Prichard's Lightning" },
                        new Product { Name = "Prichard's malt" },
                        new Product { Name = "Prichard’s" },
                        new Product { Name = "Prichard’s Double Barrel " },
                        new Product { Name = "Prichard's Rye" },
                        new Product { Name = "R1" },
                        new Product { Name = "Re-Find" },
                        new Product { Name = "Redbreast Irish 12yr" },
                        new Product { Name = "Redbreast Irish 15yr" },
                        new Product { Name = "Redbreast Lustau Edition" },
                        new Product { Name = "Rhetoric 21yr" },
                        new Product { Name = "Redwood Empire" },
                        new Product { Name = "Revel Stoke" },
                        new Product { Name = "Rich and Rare Reserve" },
                        new Product { Name = "RIP Van Winkle 12yr Reserve" },
                        new Product { Name = "Rittenhouse Rye" },
                        new Product { Name = "Rock Hill" },
                        new Product { Name = "Rowans's Creek" },
                        new Product { Name = "russel's reserve rye" },
                        new Product { Name = "Russel’s Reserve Bourbon" },
                        new Product { Name = "Sacerac Rye" },
                        new Product { Name = "Sazerac High Proof Whiskey" },
                        new Product { Name = "7 stills fluxuate" },
                        new Product { Name = "7 stills whipnose " },
                        new Product { Name = "Slow and Low Rock & Rye" },
                        new Product { Name = "Slow Hand " },
                        new Product { Name = "Slow Hand Cask" },
                        new Product { Name = "Slow Hand White" },
                        new Product { Name = "Smooth Ambler Old Scout 10yr " },
                        new Product { Name = "Smooth Ambler Old Scout 7yr " },
                        new Product { Name = "Smooth Ambler Rye" },
                        new Product { Name = "Smooth Ambler Yearling Bourbon" },
                        new Product { Name = "Sonoma Rye" },
                        new Product { Name = "Sonoma Cherry Wood Rye" },
                        new Product { Name = "sonoma west of kentucky" },
                        new Product { Name = "Spirt Works Rye" },
                        new Product { Name = "Spirt Works Wheat " },
                        new Product { Name = "St. George Baller " },
                        new Product { Name = "St. George Lot 17" },
                        new Product { Name = "St. George 35th Anniversary" },
                        new Product { Name = "Stranahan's" },
                        new Product { Name = "Suntory Hibiki 12yr" },
                        new Product { Name = "Suntory Hibiki Harmony" },
                        new Product { Name = "Suntory Toki" },
                        new Product { Name = "Templeton Rye 6yr" },
                        new Product { Name = "Tin Cup" },
                        new Product { Name = "Tomatin Single Malt" },
                        new Product { Name = "Travers City Rye" },
                        new Product { Name = "Tullamore Dew" },
                        new Product { Name = "Van Winkle Family reserve 12yr" },
                        new Product { Name = "Virgina Black" },
                        new Product { Name = "Wathen's Single Barrel" },
                        new Product { Name = "Weller" },
                        new Product { Name = "westchester wheat whiskey" },
                        new Product { Name = "westland single malt" },
                        new Product { Name = "widow Jane bourbon 10 yr" },
                        new Product { Name = "Widow Jane Rye Mash Applewood" },
                        new Product { Name = "Wild Geese Irish Soldiers & Heroes" },
                        new Product { Name = "Wild Turkey 101" },
                        new Product { Name = "Wild Turkey Forgiven" },
                        new Product { Name = "Wild Turkey rye 81" },
                        new Product { Name = "Willett Family Estate 11yr " },
                        new Product { Name = "Willett Family Estate 22yr " },
                        new Product { Name = "Willett Family Estate 3yr" },
                        new Product { Name = "Willett Family Estate 7yr" },
                        new Product { Name = "Willett Family Estate 9yr" },
                        new Product { Name = "Willett Pot Still Bourbon" },
                        new Product { Name = "Willett Rye" },
                        new Product { Name = "Whistle Pig 10yr Rye" },
                        new Product { Name = "Whistle Pig 12yr" },
                        new Product { Name = "Woodford Reserve Bourbon" },
                        new Product { Name = "woodford's rye" },
                        new Product { Name = "Woodfords Double Oaked" },
                        new Product { Name = "Yamazaki 12yr" },
                        new Product { Name = "yamazaki 18yr" }
                };

                foreach (Product p in productsWhiskey)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupWhiskey.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            // Add White Wine Products if needed
            Group groupWhiteWine = _context.Group.FirstOrDefault(g => g.Name == "White Wine");
            if (groupWhiteWine is not null && !_context.Product.Any(p => p.GroupId == groupWhiteWine.Id))
            {
                var productsWhiteWine = new Product[]
                {
                        new Product { Name = "Von Winning Riesling" },
                        new Product { Name = "Folk Machine Chenin" },
                        new Product { Name = "Edmund St John Heart" },
                        new Product { Name = "Ardoisieres Blanc" },
                        new Product { Name = "Matrot Meursault Charmes" },
                        new Product { Name = "Berthet Bondet" },
                        new Product { Name = "Ceritas Chardonnay" },
                        new Product { Name = "Allimant Laugner Syl" },
                        new Product { Name = "C-S Wagner Spatlese" },
                        new Product { Name = "Donkey and Goat" },
                        new Product { Name = "Roche aux Moines" },
                        new Product { Name = "Nikolaihof GV Im Weing" },
                        new Product { Name = "Gilbert Chon Muscadet" },
                        new Product { Name = "delaunay touraine SB" },
                        new Product { Name = "Eladio Pineiro" },
                        new Product { Name = "Schloss Gruner" },
                        new Product { Name = "Michelot Bourgogne" }
                };

                foreach (Product p in productsWhiteWine)
                {
                    var intFormatted = seqNumbers.First().ToString(upcFormat);
                    seqNumbers.Remove(seqNumbers.First());

                    p.Upc = intFormatted;
                    p.UnitPrice = 1.00m;
                    p.IsActive = true;
                    p.GroupId = groupWhiteWine.Id;

                    _context.Product.Add(p);
                }

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
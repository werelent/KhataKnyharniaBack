using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeWebApp.Models;

namespace PracticeWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly DataContext _context;

        public OrdersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<OrderHistoryDTO>>> GetUserOrderHistory([FromQuery] string email)
        {
            var userOrders = await _context.Orders
                .Where(o => o.Email == email)
                .ToListAsync();

            var orderHistoryDTOs = new List<OrderHistoryDTO>();

            foreach (var order in userOrders)
            {
                var orderHistoryDTO = new OrderHistoryDTO
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    Name = order.Name,
                    Email = order.Email,
                    Address = order.Address,
                    Status = order.Status,
                    BookInfo = new Dictionary<string, int>()
                };

                foreach (var bookQuantity in order.BookQuantities)
                {
                    var book = await _context.Books.FindAsync(bookQuantity.Key);
                    if (book != null)
                    {
                        orderHistoryDTO.BookInfo.Add(book.Title, bookQuantity.Value);
                    }
                }

                orderHistoryDTOs.Add(orderHistoryDTO);
            }

            return Ok(orderHistoryDTOs);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            // Assign the current date to the OrderDate property
            order.OrderDate = DateTime.Now;

            // Perform any necessary validations
            if (string.IsNullOrEmpty(order.Name) || string.IsNullOrEmpty(order.Email) || string.IsNullOrEmpty(order.Address))
            {
                return BadRequest("Name, Email, and Address are required.");
            }

            // Save the order to the database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

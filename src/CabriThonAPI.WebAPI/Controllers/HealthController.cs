using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CabriThonAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CabriThonAPI.WebAPI.Controllers;

[Route("api/v1/health")]
[ApiController]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Check API health and database connectivity
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            var response = new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                database = "checking..."
            };

            // Try to connect to database
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                return Ok(new
                {
                    status = "unhealthy",
                    timestamp = DateTime.UtcNow,
                    database = "cannot connect",
                    message = "Unable to connect to database"
                });
            }

            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                database = "connected",
                message = "Database connection successful"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return Ok(new
            {
                status = "unhealthy",
                timestamp = DateTime.UtcNow,
                database = "error",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Check database tables and data
    /// </summary>
    [HttpGet("database")]
    public async Task<IActionResult> GetDatabaseInfo()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                return Ok(new
                {
                    connected = false,
                    message = "Cannot connect to database"
                });
            }

            // Count records in each table
            var clientCount = await _context.Clients.CountAsync();
            var productCount = await _context.Products.CountAsync();
            var promotionCount = await _context.Promotions.CountAsync();
            var suggestedOrderCount = await _context.SuggestedOrders.CountAsync();
            var inventoryCount = await _context.InventoryClients.CountAsync();

            return Ok(new
            {
                connected = true,
                database = "Supabase PostgreSQL",
                tables = new
                {
                    clients = clientCount,
                    products = productCount,
                    promotions = promotionCount,
                    suggested_orders = suggestedOrderCount,
                    inventory_records = inventoryCount
                },
                message = clientCount > 0 && productCount > 0 
                    ? "Database has data and is ready!" 
                    : "Database is connected but may need initial data"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database info check failed");
            return Ok(new
            {
                connected = false,
                error = ex.Message,
                innerError = ex.InnerException?.Message
            });
        }
    }
}


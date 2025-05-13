// Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ProductApi.Controllers;

// http://localhost:5104 <-- Lista de Endpoints disponibles con Swagger ya documentados
// https://localhost:5104/api/Productos <-- Acceso a la tabla


/// <summary>
/// Controlador para manejar las operaciones CRUD (Crear, Leer, Actualizar, Eliminar) sobre los productos.
/// </summary>
[ApiController]
[Route("api/[controller]")]


public class ProductosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    // TOKEN FICTICIO
    private const string TOKEN_FAKE = "TOKEN_FAKE";

    /// <summary>
    /// Inicializa una nueva instancia del controlador ProductosController.
    /// </summary>
    /// <param name="context">El contexto de base de datos.</param>
    /// <param name="config">La configuración de la aplicación.</param>
    public ProductosController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    /// <summary>
    /// Verifica si la solicitud está autorizada comparando el valor de un token en los headers.
    /// </summary>
    /// <param name="request">La solicitud HTTP actual.</param>
    /// <returns>True si la solicitud está autorizada, de lo contrario False.</returns>
    private bool IsAuthorized(HttpRequest request)
    {
        var key = request.Headers["X-MASTER-KEY"].ToString();
        return key == TOKEN_FAKE;
    }

    /// <summary>
    /// Obtiene todos los productos de la base de datos.
    /// </summary>
    /// <returns>Una lista de productos en formato JSON.</returns>
    /// <response code="200">Devuelve la lista de productos.</response>
    /// <response code="401">Si el usuario no está autorizado.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!IsAuthorized(Request))
        {
            return Unauthorized(new
            {
                success = false,
                message = "No estás autorizado para ver los productos.",
                status = 401
            });
        }

        var productos = await _context.Productos.ToListAsync();

        return Ok(new
        {
            success = true,
            message = "Productos obtenidos correctamente.",
            status = 200,
            data = productos
        });
    }


    /// <summary>
    /// Obtiene un producto por su ID.
    /// </summary>
    /// <param name="id">El ID del producto.</param>
    /// <returns>Un producto en formato JSON.</returns>
    /// <response code="200">Devuelve el producto encontrado.</response>
    /// <response code="401">Si el usuario no está autorizado.</response>
    /// <response code="404">Si no se encuentra el producto con el ID dado.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        if (!IsAuthorized(Request)) return Unauthorized();
        return Ok(await _context.Productos.FindAsync(id));
    }

    /// <summary>
    /// Crea un nuevo producto en la base de datos.
    /// </summary>
    /// <param name="producto">El objeto Producto a crear.</param>
    /// <returns>Un mensaje de éxito con el ID del producto creado.</returns>
    /// <response code="201">Producto creado exitosamente.</response>
    /// <response code="401">Si el usuario no está autorizado para crear el producto.</response>
    // CREACIÓN DE REGISTRO
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Producto producto)
    {
        if (!IsAuthorized(Request))
        {
            return Unauthorized(new
            {
                success = false,
                message = "No estás autorizado para crear un producto.",
                status = 401
            });
        }

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = producto.Id }, new
        {
            success = true,
            message = "El producto ha sido creado correctamente.",
            status = 201,
            productoId = producto.Id
        });
    }

    /// <summary>
    /// Actualiza un producto existente por su ID.
    /// </summary>
    /// <param name="id">El ID del producto a actualizar.</param>
    /// <param name="producto">El objeto Producto con los nuevos valores.</param>
    /// <returns>Un mensaje de éxito o error según corresponda.</returns>
    /// <response code="200">Producto actualizado correctamente.</response>
    /// <response code="400">Si el ID del producto no coincide con el proporcionado en la URL.</response>
    /// <response code="401">Si el usuario no está autorizado para actualizar el producto.</response>
    /// <response code="404">Si no se encuentra el producto con el ID dado.</response>
    // ACTUALIZACIÓN DE REGISTRO
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Producto producto)
    {
        if (!IsAuthorized(Request))
        {
            return Unauthorized(new
            {
                success = false,
                message = "No estás autorizado para actualizar el producto.",
                status = 401
            });
        }

        if (id != producto.Id)
        {
            return BadRequest(new
            {
                success = false,
                message = "El ID del producto no coincide con el ID proporcionado en la URL.",
                status = 400
            });
        }

        var existingProducto = await _context.Productos.FindAsync(id);
        if (existingProducto == null)
        {
            return NotFound(new
            {
                success = false,
                message = $"Producto con ID {id} no encontrado.",
                status = 404
            });
        }

        // ✅ Actualizás tus campos reales
        existingProducto.Nombre = producto.Nombre;
        existingProducto.Descripcion = producto.Descripcion;
        existingProducto.Precio = producto.Precio;
        existingProducto.Imagen = producto.Imagen;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            success = true,
            message = $"Producto con ID {id} actualizado correctamente.",
            status = 200
        });
    }



    /// <summary>
    /// Elimina un producto de la base de datos por su ID.
    /// </summary>
    /// <param name="id">El ID del producto a eliminar.</param>
    /// <returns>Un mensaje de éxito o error.</returns>
    /// <response code="204">Producto eliminado correctamente.</response>
    /// <response code="401">Si el usuario no está autorizado para eliminar el producto.</response>
    /// <response code="404">Si no se encuentra el producto con el ID dado.</response>
    // ELIMINACIÓN DE REGISTRO
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAuthorized(Request))
        {
            return Unauthorized(new
            {
                success = false,
                message = "No estás autorizado para eliminar el producto.",
                status = 401
            });
        }

        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
        {
            return NotFound(new
            {
                success = false,
                message = $"Producto con ID {id} no encontrado.",
                status = 404
            });
        }

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return NoContent(); // Este responde sin cuerpo, no es necesario un mensaje, pero podemos agregarlo si es necesario
    }

    /// <summary>
    /// Obtiene todos los productos utilizando Dapper (alternativa a Entity Framework).
    /// </summary>
    /// <returns>Una lista de productos obtenidos desde la base de datos.</returns>
    /// <response code="200">Devuelve la lista de productos.</response>
    /// <response code="401">Si el usuario no está autorizado para acceder a esta información.</response>
    // OBTENCIÓN DE REGISTRO UTILIZANDO DAPPER (ALTERNATIVA A ENTITY FRAMEWORK) 
    [HttpGet("dapper")]
    public async Task<IActionResult> GetAllDapper()
    {
        if (!IsAuthorized(Request))
        {
            return Unauthorized(new
            {
                success = false,
                message = "No estás autorizado para acceder a esta información",
                status = 401
            });
        }

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var productos = await connection.QueryAsync<Producto>("SELECT * FROM Productos");

        return Ok(new
        {
            success = true,
            message = $"Se encontraron {productos.Count()} productos.",
            data = productos
        });
    }
}

# Proyecto Backend de Productos en .NET

Este proyecto es un backend API RESTful construido utilizando **.NET 9**. Proporciona una API para gestionar productos, incluyendo operaciones como obtener, crear, actualizar y eliminar productos.

## Requisitos

- **.NET SDK 9.0** o superior
- **SQL Server** (puedes usar SQL Server Express, o cualquier otra versión)
- **Postman** o cualquier otra herramienta para hacer peticiones HTTP (opcional)
- **IDE**: Visual Studio o Visual Studio Code

## Configuración

### 1. Clonar el repositorio

Primero, clona el repositorio en tu máquina local:

```bash
git clone https://github.com/tu_usuario/tu_repositorio.git
cd tu_repositorio
```

### 2. Configurar la base de datos

En este proyecto se usa SQL Server como base de datos. Asegúrate de tener una base de datos configurada y crea las tablas necesarias (o usa los scripts de migración si están configurados).

En el archivo `appsettings.json`, configura la cadena de conexión de la base de datos:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=productos_db;User Id=sa;Password=TuContraseña;"
  }
}
```

### 3. Instalar dependencias

Si aún no has restaurado las dependencias del proyecto, ejecuta el siguiente comando en la terminal dentro de la carpeta del proyecto:

```bash
dotnet restore
```

### 4. Migraciones (si usas Entity Framework Core)

Si estás utilizando Entity Framework Core para manejar la base de datos, crea y aplica las migraciones necesarias con los siguientes comandos:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Ejecutar el proyecto

Para iniciar el proyecto, utiliza el siguiente comando:

```bash
dotnet run
```

Esto debería levantar el servidor en `http://localhost:5104` (o el puerto que tengas configurado).

## API Endpoints

### Obtener todos los productos

**GET** `/api/productos`

Obtiene todos los productos almacenados en la base de datos.

**Respuesta:**

```json
[
  {
    "id": 1,
    "nombre": "Producto A",
    "precio": 100
  },
  {
    "id": 2,
    "nombre": "Producto B",
    "precio": 150
  }
]
```

### Obtener un producto por ID

**GET** `/api/productos/{id}`

Obtiene un producto específico por su ID.

**Respuesta:**

```json
{
  "id": 1,
  "nombre": "Producto A",
  "precio": 100
}
```

### Crear un nuevo producto

**POST** `/api/productos`

Crea un nuevo producto.

**Cuerpo de la solicitud:**

```json
{
  "nombre": "Nuevo Producto",
  "precio": 200
}
```

**Respuesta:**

```json
{
  "id": 3,
  "nombre": "Nuevo Producto",
  "precio": 200
}
```

### Actualizar un producto

**PUT** `/api/productos/{id}`

Actualiza un producto existente.

**Cuerpo de la solicitud:**

```json
{
  "nombre": "Producto A Actualizado",
  "precio": 120
}
```

**Respuesta:**

```json
{
  "id": 1,
  "nombre": "Producto A Actualizado",
  "precio": 120
}
```

### Eliminar un producto

**DELETE** `/api/productos/{id}`

Elimina un producto existente.

**Respuesta:**

```json
{
  "success": true,
  "message": "Producto eliminado correctamente"
}
```

## Autenticación (Opcional)

Si se requiere autenticación, se debe agregar el siguiente encabezado en las peticiones a la API:

```
X-MASTER-KEY: [Tu API Key Aquí]
```

Asegúrate de que el valor de este encabezado esté configurado correctamente en el interceptor HTTP de tu frontend para que todas las peticiones tengan autorización.

## Errores Comunes

- **401 Unauthorized**: Esto ocurre si no se envía el encabezado `X-MASTER-KEY` en la petición o si el valor del encabezado es incorrecto.
- **500 Internal Server Error**: Esto puede suceder si hay un problema con la base de datos o la lógica del servidor.

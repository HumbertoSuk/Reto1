Este proyecto usa Entity Framework Core como ORM, lo que permite trabajar con la base de datos usando clases C# (entidades)
Se utiliza el enfoque Code-First: primero se definen los modelos y el DbContext en C#, luego se generan migraciones.

## Tech Stack

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core (SQL Server provider)
- Microsoft SQL Server (DB engine/admin via SSMS)
---
## Requisitos del reto
### Crud
- Crear pago
- Listar pagos
- Ver detalles
- Editar pago
- Eliminar pago
- bulk delete
### No permitir pagos duplicados
La app evita duplicados con dos capas:
- Validación del lado servidor (consulta antes de guardar)
- Índice UNIQUE en la base de datos como garantía final
Regla de duplicado elegida (campos únicos):
PaidOn + Amount + MerchantNormalized deben ser únicos.
---
### Estructura de Proyecto con MVC
---
### 1) Prevención de duplicados

Cuando se crea o edita un pago:

1. **Normalizar Merchant**
    - `MerchantNormalized = NormalizeMerchant(Merchant)`
2. **Validación antes de guardar (EF Core)**
    - Se consulta si ya existe un pago con:
        - misma fecha `PaidOn` (solo fecha)
        - mismo `Amount`
        - mismo `MerchantNormalized`
    - Si existe → se muestra error de validación.
3. **Garantía a nivel base de datos**
    - El índice UNIQUE impide duplicados incluso si llegan 2 requests al mismo tiempo.
    - Si ocurre `DbUpdateException`, se maneja mostrando mensaje de duplicado.

### 2) Eliminación masiva (Bulk Delete)

En la vista `Index`:

- El usuario marca varios registros (`selectedIds[]`)
- Se envía POST a `/Payments/BulkDelete`
- El servicio carga los registros y elimina con `RemoveRange`
- Se muestra un mensaje en `TempData` con el resultado.
---

Hay mejoras que se pueden hacer como 
Usar Interfaces y seperar responsabilidades del controller para que que sea mas limpio con inyecciones de dependencias, asi orquestar mejor. 


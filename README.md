# Alica System | Sistema de Gestión Bibliotecaria

Sistema web de gestión bibliotecaria desarrollado como proyecto final para la materia de **Desarrollo Web II**, carrera de Ingeniería de Software — **Instituto Cultural Dominico Americano (UNICDA)**.

> Calificación final: **100/100**

🎥 [Ver video de presentación](https://youtu.be/zewsCL4gQTY?si=3LOo52gyjSOr0w1m) &nbsp;|&nbsp;  

---

## Equipo

- **Alani Encarnación** - Acceso, Lector, Administrador (Libros, Categorías, Autores, Usuarios, Reportes)
- **Camila Hierro** - Bibliotecario, Administrador (Dashboard, Empleados, Roles)

**Profesor:** Roberto Pichardo

---

## Descripción

Alica System administra el préstamo, reserva y catálogo de libros de una biblioteca universitaria, con **tres roles de usuario** independientes:

| Rol | Puede |
|---|---|
| **Lector** | Consultar catálogo, reservar libros, ver/renovar préstamos, gestionar listas personales, ver multas |
| **Bibliotecario** | Registrar préstamos y devoluciones, atender reservas pendientes, gestionar multas |
| **Administrador** | Gestionar libros, autores, categorías, usuarios, empleados, roles y generar reportes |

---

## Stack técnico

- **Backend:** ASP.NET Core Razor Pages (C#, .NET 10)
- **Base de datos:** SQL Server / Azure SQL Database
- **Frontend:** HTML, CSS, JavaScript (sin frameworks de frontend)
- **Control de versiones:** Git / GitHub

---

## Arquitectura

Todo el acceso a datos pasa **exclusivamente por stored procedures** — no hay una sola consulta SQL embebida en el código C#. Esto centraliza la lógica de negocio y las validaciones directamente en la base de datos:

- **16 tablas** relacionales
- **70+ stored procedures**
- Validaciones críticas resueltas a nivel de base de datos (duplicados, límites de negocio, protección contra eliminación de registros en uso)

### Reglas de negocio destacadas

- Máximo **3 préstamos activos** y **3 reservas activas** por usuario
- Las reservas descuentan inventario al instante y **expiran automáticamente a las 48h**
- Los préstamos permiten hasta **5 renovaciones de 7 días**, solo habilitadas en los últimos 2 días antes del vencimiento
- Las multas se generan automáticamente por atraso, o manualmente por daño reportado
- Los correos institucionales y contraseñas iniciales de los lectores se generan automáticamente a partir de su matrícula
- Soft-delete (activar/desactivar) en catálogo, categorías, usuarios y empleados — protegido contra eliminar registros con relaciones activas

---

## Estructura del proyecto

```
AlicaSystem/
├── Pages/
│   ├── Login.cshtml, Recuperar.cshtml
│   ├── Lector/          → Catálogo, Ficha de libro, Reservas, Préstamos, Listas
│   ├── Bibliotecario/   → Registrar préstamo/devolución, Reservas pendientes, Multas
│   └── Administrador/   → Libros, Autores, Categorías, Usuarios, Empleados, Roles, Reportes
├── Datos/               → Clases de acceso a datos (una por entidad)
├── Models/              → Clases de modelo
├── Database/            → Scripts SQL organizados por módulo (SP_ACCESO, SP_LECTOR, SP_BIBLIOTECARIO, SP_ADMINISTRADOR)
└── wwwroot/             → CSS, JS, imágenes
```

---

## Cómo correrlo localmente

1. Clona el repositorio
2. Restaura la base de datos con el script en `Database/` (crea las 16 tablas + los stored procedures)
3. Configura tu cadena de conexión en `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "AlicaSystem": "Server=localhost,1433;Database=alica_system;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True;"
     }
   }
   ```
4. Restaura los paquetes y ejecuta:
   ```
   dotnet restore
   dotnet run
   ```

---

## Entregables del curso

- ✅ Etapa 1 — Propuesta
- ✅ Etapa 2 — Requerimientos y reglas de negocio
- ✅ Etapa 3 — Modelo de datos
- ✅ Etapa 4 — Mockups
- ✅ Etapa 5 — Frontend
- ✅ Etapa 6/7 — Backend y stored procedures
- ✅ Etapa 9 — Presentación final (video)

---

*Proyecto académico desarrollado con fines educativos - Desarrollo Web II, 6to cuatrimestre, UNICDA, 2026.*

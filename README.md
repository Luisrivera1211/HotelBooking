# HotelBooking

Sistema de **gestión de reservas de habitaciones de hotel**, desarrollado en C# / .NET 8 con ASP.NET Core MVC, una API REST, Entity Framework Core y SQL Server, con arquitectura en capas.

> Proyecto académico. Cubre el ciclo completo: estructura del hotel, huéspedes, disponibilidad, reservas, notificaciones por correo y auditoría.

---

## Características

- **Visitante sin cuenta:** puede ver los hoteles, buscar disponibilidad y ver las habitaciones. La cuenta solo se pide al confirmar la reserva.
- **Huésped con cuenta:** se registra con su correo, reserva a su nombre y consulta, modifica o cancela **solo sus propias** reservas.
- **Personal:** autenticación por cookies con dos roles, **Administrador** y **Recepcionista**. El rol Administrador exige un código de registro configurable.
- **Estructura del hotel:** Hotel → Bloque → Piso → Habitación, con tipos de habitación (capacidad y tarifa por noche).
- **Hoteles en tarjetas** con estrellas (1 a 5) definidas por el administrador.
- **Habitaciones** con descripción breve (se muestra al pasar el mouse) y características (Wi-Fi, vista al mar, etc.).
- **Huéspedes:** alta, edición, búsqueda por documento e historial. La recepción puede elegir un huésped ya registrado al reservar.
- **Disponibilidad:** búsqueda por hotel, fechas y número de huéspedes.
- **Reservas:** crear, modificar, confirmar y cancelar, con cálculo automático del total (noches × tarifa).
- **Notificaciones por correo** (confirmación, modificación, cancelación) con registro de estado (`Pending`, `Sent`, `Failed`). Si el correo falla, la reserva no se ve afectada.
- **Auditoría** (RNF-AUT-001): quién creó o modificó una reserva, una habitación o una notificación, y cuándo. Visible en el detalle de la reserva y en la pantalla de Auditoría (solo administrador).
- **Integridad de reservas:** no se permiten solapamientos de fechas en una misma habitación, validado dentro de una transacción serializable.
- **API REST** con Swagger que expone los servicios de negocio.

## Permisos por rol

| Funcionalidad | Visitante | Huésped | Recepcionista | Administrador |
|---|:---:|:---:|:---:|:---:|
| Ver hoteles y buscar disponibilidad | ✅ | ✅ | ✅ | ✅ |
| Crear reservas | ❌ (pide cuenta) | ✅ (a su nombre) | ✅ | ✅ |
| Consultar, modificar y cancelar reservas | ❌ | ✅ (solo las suyas) | ✅ | ✅ |
| Huéspedes | ❌ | ❌ | ✅ | ✅ |
| Hoteles, bloques, pisos, habitaciones y tipos | ❌ | ❌ | ❌ | ✅ |
| Auditoría | ❌ | ❌ | ❌ | ✅ |

## Arquitectura

Tres capas con dependencias en un solo sentido (la presentación depende de negocio y datos; negocio depende de datos). Hay dos proyectos de presentación sobre la misma lógica: la web MVC y la API.

```
HotelBooking.sln
├── HotelBooking.Data        Entidades, DbContext, repositorios, OperationResult<T>
├── HotelBooking.Business    Servicios, DTOs, reglas de negocio, canales de notificación
├── HotelBooking.Web         ASP.NET Core MVC: controladores, vistas, ViewModels, autenticación
└── HotelBooking.API         API REST: controladores y Swagger
```

### Decisiones de diseño

- **Inversión de dependencias:** los servicios y repositorios se consumen por interfaz; cada proyecto registra sus dependencias en su propio `DependencyInjection` (`AddDataDependencies`, `AddBusinessDependencies`).
- **Principio abierto/cerrado (OCP) en las notificaciones:** `NotificationService` depende de la interfaz `INotificationChannel` y recorre todos los canales registrados. Hoy solo existe el canal de correo (`EmailChannel`); un canal nuevo (por ejemplo SMS) se agrega con **una clase que implemente la interfaz y una línea en `AddBusinessDependencies`**, sin modificar el servicio. El SMS queda fuera del alcance actual: la interfaz es el punto de extensión.
- **Resultado uniforme:** todas las operaciones devuelven `OperationResult<T>` (`Success`, `Message`, `ErrorType`, `Data`). `ErrorType` distingue `Validation`, `NotFound`, `Conflict` y `Unexpected`.
- **Repositorio base genérico** (`IBaseRepository<T>` / `BaseRepository<T>`) con puntos de extensión (`Validate`, `SaveEntityAsync` y `UpdateEntityAsync` virtuales), `try/catch` en cada método y guardas de entrada.
- **Auditoría centralizada** en el `DbContext`: las entidades auditables (`IAuditable`) dejan `CreatedAt/By` y `UpdatedAt/By`, y cada cambio se anota en la tabla `AuditLogs` con el detalle campo por campo. Los registros de auditoría no se pueden modificar ni borrar.
- **Mensajes centralizados** (`DataMessages` en Data y `ErrorMessages` en Business).
- **DTOs sin anotaciones** en Business; la validación de formularios vive en los **ViewModels** de la capa web y Business revalida por su cuenta.
- **Sin `ViewData` para datos de pantalla:** cada vista recibe un ViewModel tipado.
- **Borrado restringido** (`DeleteBehavior.Restrict`) en todas las relaciones.

### Modelo de datos

`Hotels` → `Blocks` → `Floors` → `Rooms` (con `RoomTypes`), más `Guests`, `Employees`, `Reservations`, `Notifications` y `AuditLogs`. Los enums se guardan como texto y la base tiene `CHECK`, `UNIQUE` e índices.

## Reglas de negocio principales

- No puede haber dos pisos **activos** con el mismo número en un mismo bloque.
- El número de habitación es único dentro del hotel.
- Una reserva requiere habitación disponible, piso activo y capacidad suficiente para los huéspedes.
- Las fechas se normalizan al día; la estadía mínima es de una noche.
- Estados de reserva: `Pending` → `Confirmed`; se puede modificar o cancelar mientras esté `Pending` o `Confirmed`.
- No se permiten reservas que se solapen en la misma habitación (excluyendo las canceladas).
- **RN-NOT-03:** al modificar una reserva solo se notifica si cambió la habitación o las fechas.
- Un documento de huésped, un correo de empleado y un correo de cuenta de huésped no pueden repetirse.

## Seguridad

- Contraseñas con **PBKDF2-SHA256** (100 000 iteraciones, sal aleatoria por usuario); nunca se almacenan ni se exponen en texto plano.
- El inicio de sesión devuelve el mismo mensaje si el correo no existe o la contraseña es incorrecta.
- Cookies `HttpOnly`, política de autorización por defecto que exige sesión (con excepciones explícitas para el acceso público) y **antiforgery** automático en todos los `POST`.
- Un huésped solo accede a sus propias reservas; si pide otra, recibe "no encontrada".
- Redirección posterior al login solo hacia URLs locales.
- **Los secretos no se suben al repositorio.** La contraseña del correo y el código de administrador se guardan con *user-secrets* (ver abajo).

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (Express, LocalDB o instancia completa) y SQL Server Management Studio
- Visual Studio 2022 (opcional, recomendado)
- Cuenta de correo SMTP para las notificaciones (con Gmail se requiere una *contraseña de aplicación*)

## Puesta en marcha

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/Luisrivera1211/HotelBooking.git
   cd HotelBooking
   ```

2. **Crear la base de datos.** Ejecuta `Database/hotel_booking_db_completo.sql` en SQL Server. Crea la base `hotel_booking_db` con todas sus tablas.

3. **Revisar `appsettings.json`** en `HotelBooking.Web` (y en `HotelBooking.API`). Solo la cadena de conexión va en el archivo; los datos sensibles quedan vacíos:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=hotel_booking_db;Trusted_Connection=True;TrustServerCertificate=True"
     },
     "AdminRegistrationCode": "CAMBIAR-ESTE-CODIGO",
     "Email": {
       "Host": "smtp.gmail.com",
       "Port": 587,
       "User": "",
       "Password": "",
       "From": "",
       "EnableSsl": true
     }
   }
   ```

4. **Guardar los secretos en tu equipo** (una vez por proyecto: Web y API):
   ```bash
   cd HotelBooking.Web
   dotnet user-secrets init
   dotnet user-secrets set "Email:User" "<tu-correo>"
   dotnet user-secrets set "Email:Password" "<clave-de-aplicacion>"
   dotnet user-secrets set "Email:From" "<tu-correo>"
   dotnet user-secrets set "AdminRegistrationCode" "<codigo>"
   ```
   User Secrets se lee al ejecutar en el entorno `Development`. Si no configuras el correo, el sistema funciona, pero las notificaciones quedan como `Failed`.

5. **Ejecutar la web**
   ```bash
   dotnet run --project HotelBooking.Web
   ```
   O desde Visual Studio: establece `HotelBooking.Web` como proyecto de inicio y pulsa F5.

6. **Primer uso:** en *Regístrese* crea una cuenta de empleado con rol Administrador (con el código configurado) y otra con rol Recepcionista. Los huéspedes se registran desde *Crear cuenta*.

## API REST

La API reutiliza las capas de negocio y datos. Se ejecuta con `HotelBooking.API` como proyecto de inicio y la documentación interactiva queda en `/swagger`.

- Un controlador por servicio de negocio. Respuestas `200` con el `OperationResult` completo, o `400` con el mensaje de error.
- Los cambios hechos por la API quedan auditados con el usuario `api`.
- Esta versión **no incluye autenticación por token**: los endpoints de login devuelven los datos del usuario, no un token.

| Controlador | Endpoints |
|---|---|
| `Hotels` | `GET /api/Hotels`, `GET /api/Hotels/{id}`, `POST CreateHotel`, `POST ModifyHotel/{id}`, `POST SetHotelActive/{id}` |
| `Blocks` | `GET ByHotel/{hotelId}`, `GET {id}`, `POST CreateBlock`, `POST ModifyBlock/{id}`, `POST SetBlockActive/{id}` |
| `Floors` | `GET ByBlock/{blockId}`, `GET {id}`, `POST CreateFloor`, `POST SetFloorActive/{id}` |
| `RoomTypes` | `GET`, `GET {id}`, `POST CreateRoomType`, `POST ModifyRoomType/{id}` |
| `Rooms` | `GET ByFloor/{floorId}`, `GET {id}`, `POST CreateRoom`, `POST ModifyRoom/{id}`, `POST ChangeRoomStatus/{id}` |
| `Guests` | `GET`, `GET {id}`, `GET ByDocument/{document}`, `POST CreateGuest`, `POST ModifyGuest/{id}`, `POST RegisterGuestAccount`, `POST LoginGuest` |
| `Employees` | `POST RegisterEmployee`, `POST LoginEmployee` |
| `Reservations` | `GET {id}`, `GET ByGuest/{guestId}`, `POST CreateReservation`, `POST ModifyReservation/{id}`, `POST ConfirmReservation/{id}`, `POST CancelReservation/{id}` |
| `Availability` | `POST Search` |
| `Notifications` | `GET ByReservation/{reservationId}`, `POST Notify/{reservationId}?type=` |
| `Audit` | `GET Recent`, `GET ByEntity/{entityName}/{entityId}` |

Ejemplo, crear una reserva (`POST /api/Reservations/CreateReservation`):

```json
{ "roomId": 1, "guestId": 1, "checkIn": "2026-12-10", "checkOut": "2026-12-12", "guestCount": 2 }
```

## Flujo de demostración (web)

1. Administrador: crea un **hotel** (con sus estrellas), un **bloque**, un **piso**, un **tipo de habitación** y una **habitación** (con descripción y características).
2. Sin iniciar sesión, entra a *Disponibilidad*, elige el hotel, indica las fechas y revisa las habitaciones.
3. Al pulsar *Reservar*, el sistema pide **iniciar sesión o crear una cuenta**. Crea una cuenta de huésped y completa la reserva.
4. Recepcionista: también puede reservar eligiendo un huésped ya registrado.
5. **Confirma**, **modifica** o **cancela** la reserva y revisa el estado de las **notificaciones**.
6. Administrador: revisa el historial de cambios en el detalle de la reserva y en **Auditoría**.

## Estructura de carpetas

```
HotelBooking.Data/
  Base/  Entities/  Enums/  Results/  Context/  Interfaces/  Repositories/
HotelBooking.Business/
  DTOs/  Interfaces/  Services/  Services/Channels/  Helpers/  Mappers/
HotelBooking.Web/
  Controllers/  Models/  Helpers/  Constants/  Views/  wwwroot/
HotelBooking.API/
  Controllers/  Helpers/
Database/
  hotel_booking_db_completo.sql
```

## Limitaciones conocidas

- No hay listado general de reservas: se consultan desde la ficha del huésped o desde el panel de inicio.
- No existe recuperación de contraseña para los huéspedes.
- El canal de SMS no está implementado (fuera del alcance); solo el punto de extensión.
- La API no tiene autenticación por token.
- El historial de auditoría se escribe en un segundo guardado; fuera de la transacción de las reservas, un fallo entre ambos podría dejar un cambio sin su entrada de auditoría.

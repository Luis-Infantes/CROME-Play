​C.R.O.M.E. Play es una app diseñada para crear secciones de juegos y gestionar los distintos eventos de los mismos para una tienda, un evento, etc.

#ADMIN#

A través del perfil del "Admin" gestionaremos la creación de los roles "Masterclub" y "Member". 
Tendremos que crear los clubes que sean necesarios antes de que se registren con sus cuentas para una vez hecho el registro, se pueda asignar su rol de "MasterClub".
Por otra parte los socios se registran previeamente y estaran en una tabla de usuarios pendientes hasta que se les asignen el rol de "Member".

Podremos eliminar cualquiera de los perfiles citados para cuando lo sea necesario.

#MASTERCLUB#

A través del "Masterclub", gestionaremos cada club.
Podremos editar el perfil. Disponemos de un pequeño gestor de tareas para organizar los deberes para la creacion de eventos, juegos, etc.
Disponemos del acceso al listado de juegos y eventos, donde los podremos crear, editar o eliminar.

#MEMBER#

En el perfil del "Member", podrá completar sus datos personales en la edición de su perfil, donde además podrá ver los eventos a los que se ha apuntado y también podrá eliminarlos
por si no puede asistir.
En la página de inscripciones podrá inscribirse sólo una vez al evento que más le interese



#PAQUETES NECESARIOS E INSTALACION#

Microsoft.EntityFrameworkCore (version 8.0.20)
Microsoft.EntityFrameworkCore.SqlServer (version 8.0.20)
Microsoft.EntityFrameworkCore.Tools (version 8.0.20)
X.PagedList.Mvc.Core (version 10.5.9)

Crear migración y crear base de datos a través de la consola.




# ARCHIVO ACCESS#

Archivo de texto ACCESS donde encontramos los correos y contraseñas necesarios para entrar con la cuenta del admin. Y gestionas los perfiles del Masterclub y Member



#TECNOLOGIAS USADAS#

- C#
- ASP.NET MVC
- Entity Framework Core
- JavaScript para la lógica del cliente
- AJAX para la comunicación asíncrona entre vistas y controladores
- API interna creada desde cero para manejar eventos y procesar datos
- CSS y Bootstrap para el diseño de la interfaz




#ESTRUCTURA DEL PROYECTO#

/Properties

/wwwroot
  ├── css
  ├── images
  └── js

/Areas
  ├── Identity


/Controllers
  ├── Api
  │     └── CalendarController.cs
  │
  ├── AdminController.cs
  ├── BaseController.cs
  ├── ClubController.cs
  ├── HomeController.cs
  └── MemberController.cs

/Data
  ├── Migrations
  ├── ApplicationDbContext.cs

/Domain
  ├── Calendar
       └── CalendarEvent.cs

/Dtos
  ├── Calendar
       |── CreateEventDto.cs
       └── UpdateEventDto.cs


/Models
  ├── AppSettings.cs
  ├── Club.cs
  ├── Enrollment.cs
  ├── ErrorViewModel.cs
  ├── Game.cs
  |── Member.cs
  |── PendingUser.cs
  └── TaskItem.cs

/Services
  ├── Calendar
       |── CalendarService.cs
       └── ICalendarService.cs

/ViewModels

/Views

/ACCESS

/README

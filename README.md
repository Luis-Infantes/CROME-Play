C.R.O.M.E. Play is an app designed to create game sections and manage different events for a store, an event, etc.

#ADMIN#
Through the "Admin" profile, we will manage the creation of the roles "Masterclub" and "Member".
We must create the necessary clubs before they register with their accounts so that once registration is complete, their role as "MasterClub" can be assigned.
On the other hand, members register beforehand and will remain in a pending users table until they are assigned the "Member" role.
We will be able to delete any of the mentioned profiles whenever necessary.

#MASTERCLUB#
Through the "Masterclub", we will manage each club.
We can edit the profile. We have a small task manager to organize duties for creating events, games, etc.
We have access to the list of games and events, where we can create, edit, or delete them.

#MEMBER#
In the "Member" profile, they can complete their personal data in the profile editing section, where they can also see the events they have signed up for and remove them if they cannot attend.
On the registration page, they can register only once for the event they are most interested in.
#REQUIRED PACKAGES AND INSTALLATION#
Microsoft.EntityFrameworkCore (version 8.0.20)
Microsoft.EntityFrameworkCore.SqlServer (version 8.0.20)
Microsoft.EntityFrameworkCore.Tools (version 8.0.20)
X.PagedList.Mvc.Core (version 10.5.9)
Create migration and create database through the console.

#ACCESS FILE#
Text file ACCESS where we find the emails and passwords needed to log in with the admin account and manage the Masterclub and Member profiles.
#TECHNOLOGIES USED#

C#
ASP.NET MVC
Entity Framework Core
JavaScript for client-side logic
AJAX for asynchronous communication between views and controllers
Internal API created from scratch to handle events and process data
CSS and Bootstrap for interface design

#PROJECT STRUCTURE#
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
/README​




//-----------------------------------------------------

C.R.O.M.E. Play es una app diseñada para crear secciones de juegos y gestionar los distintos eventos de los mismos para una tienda, un evento, etc.

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

Archivo de texto ACCESS donde encontramos los correos y contraseñas necesarios para entrar con la cuenta del admin. 
Y gestionar los perfiles del Masterclub y Member.


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

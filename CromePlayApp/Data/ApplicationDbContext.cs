using CromePlayApp.Domain.Calendar;
using CromePlayApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CromePlayApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<Club> Clubes { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<PendingUser> PendingUsers { get; set; }

        public DbSet<CalendarEvent> CalendarEvents { get; set; } = default!;

        public DbSet<TaskItem> TaskItems { get; set; } = default!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            //DEFINIR NUEVAMENTE LAS RELACIONES ENTRE LOS MODELOS DE DATOS

            //Con Restrict evitamos además el borrado en cascada
            builder.Entity<Enrollment>()
                .HasOne(e => e.Member)
                .WithMany()
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            //Con Cascade todo lo contrario. Cuando se borra un registro, elimina en cascada
            builder.Entity<Enrollment>()
                .HasOne(e => e.Game)
                .WithMany()
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CalendarEvent>()
                .HasOne(e => e.Game)
                .WithMany(c => c.CalendarEvents)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<TaskItem>()
                .HasOne(e => e.Club)
                .WithMany(c => c.TaskItems)
                .HasForeignKey(e => e.ClubId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<PendingUser>()
                .HasOne(p => p.IdentityUser)
                .WithMany() 
                .HasForeignKey(p => p.IdentityUserId)
                .OnDelete(DeleteBehavior.Cascade); 
           
            builder.Entity<PendingUser>()
                .HasIndex(p => p.IdentityUserId)
                .IsUnique();

            builder.Entity<Member>()
                .HasOne(m => m.IdentityUser)
                .WithMany()
                .HasForeignKey(m => m.IdentityUserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Club>()
                .HasIndex(c => c.ClubEmail)
                .IsUnique();


            // Índice único filtrado por IdentityUserId (si permites null/empty, filtra para no chocar)
            builder.Entity<PendingUser>()
                .HasIndex(p => p.IdentityUserId)
                .HasFilter("[IdentityUserId] IS NOT NULL AND [IdentityUserId] <> ''")
                .IsUnique();


            builder.Entity<PendingUser>()
                .HasIndex(p => p.IdentityUserId)
                .HasFilter("[IdentityUserId] IS NOT NULL AND [IdentityUserId] <> ''")
                .IsUnique();


            // Índice único por email (si tu dominio lo permite; si no, al menos un índice normal)
            builder.Entity<PendingUser>()
                .HasIndex(p => p.PendingUserEmail)
                .IsUnique();


            //Configuración explícita de la entidad con campos requeridos para crear el evento
            builder.Entity<CalendarEvent>(e =>
            {
                e.ToTable("CalendarEvents");
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).IsRequired().HasMaxLength(100);
                e.Property(x => x.Start).IsRequired();
                e.Property(x => x.End).IsRequired();
                e.Property(x => x.Type).HasMaxLength(50);
                e.Property(x => x.IsAllDay).HasDefaultValue(false);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.GameId).IsRequired();
                // e.Property(x => x.UpdatedAt) // puede ser null, sin config especial
            });





            //SEMILLA DE DATOS BASE COMO EJEMPLO DEL PROYECTO

            builder.Entity<Club>().HasData(
                
                new Club {ClubId = 1, ClubName = "Juegos de cartas", ClubEmail= "CardGames@Contact.com", ClubDescription = "Adéntrate en un mundo donde la magia y la estrategia se entrelazan. Forja tu destino como héroe, invocando criaturas legendarias, lanzando poderosos hechizos y desafiando a tus rivales en épicas batallas. Cada carta es una historia, cada jugada, una leyenda."}
                
                );


            builder.Entity<Game>().HasData(
                
                new Game { GameId = 1, GameName = "Marvel Champions", GameDescription = "¡Entra en el universo Marvel y conviértete en tu héroe favorito! Marvel Champions es un juego cooperativo de cartas en el que tú y tus amigos uniréis fuerzas para enfrentarse a villanos icónicos y detener sus planes malvados. Cada jugador elige un héroe con habilidades únicas y construye su mazo para adaptarse a diferentes desafíos. Con partidas dinámicas, expansiones temáticas y una experiencia inmersiva, este juego te hará sentir parte de la acción. ¿Estás listo para salvar el mundo?", ClubId = 1 },

                new Game { GameId = 2, GameName = "Star Wars (Unlimited)", GameDescription = "¡Explora una galaxia muy, muy lejana en este juego de cartas coleccionable lleno de acción! Star Wars: Unlimited te permite construir tu propio mazo y enfrentarte en duelos estratégicos con héroes, villanos y naves icónicas de la saga. Con reglas dinámicas, ilustraciones espectaculares y expansiones continuas, cada partida es una nueva aventura. ¿Tienes lo necesario para liderar la batalla y cambiar el destino de la galaxia?", ClubId = 1 },

                new Game { GameId = 3, GameName = "The Lord of the Rings", GameDescription = "Embárcate en una épica aventura por la Tierra Media. Este juego cooperativo te permite liderar a héroes legendarios en misiones peligrosas contra las fuerzas oscuras de Sauron. Con mazos personalizables, escenarios desafiantes y una narrativa envolvente, The Lord of the Rings: El Juego de Cartas es la experiencia definitiva para los fans de Tolkien.", ClubId = 1 },

                new Game { GameId = 4, GameName = "Arkham Horror", GameDescription = "Sumérgete en los oscuros mitos de Lovecraft con este juego cooperativo narrativo. En Arkham Horror: El Juego de Cartas, asumirás el papel de un investigador que debe resolver misterios inquietantes y enfrentarse a horrores sobrenaturales. Cada decisión cuenta: construye tu mazo, explora escenarios llenos de tensión y lucha por mantener la cordura mientras desentrañas secretos prohibidos.", ClubId = 1 },

                new Game { GameId = 5, GameName = "vampire vtes", GameDescription = "En Vampire: The Eternal Struggle no eres un simple jugador: eres un Matusalén, un vampiro ancestral que mueve los hilos en la oscuridad. Recluta a tu cripta, conspira, negocia y traiciona para dominar la noche. Cada decisión cuenta, cada alianza puede romperse, y solo el más astuto sobrevivirá.\r\n¿Controlarás la sangre… o serás drenado por tus rivales? La lucha nunca termina. ¿Estás listo para jugar?", ClubId = 1 },

                new Game { GameId = 6, GameName = "MAGIC. The gathering", GameDescription = "En Magic: The Gathering te conviertes en un planeswalker, un poderoso hechicero capaz de viajar entre mundos y desatar fuerzas inimaginables. Invoca criaturas legendarias, lanza conjuros devastadores y construye estrategias únicas para derrotar a tus rivales. Cada partida es un duelo épico donde la inteligencia y la imaginación deciden el destino.", ClubId = 1 }


                );


            builder.Entity<CalendarEvent>().HasData(
                
                
                new CalendarEvent { Id= 1, Title= "Evento de Marvel Champions", Price = 5,AvailablePlace = 12, Start = new DateTime (2026,05,12) , End = new DateTime(2026, 05, 13),Type = "CardGames", IsAllDay = false, CreatedAt= new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null, GameId = 1 },


                new CalendarEvent { Id = 2, Title = "Evento de Star Wars (Unlimited)",Price = 3, AvailablePlace = 18, Start = new DateTime(2026, 04, 12), End = new DateTime(2026, 04, 13), Type = "CardGames", IsAllDay = false, CreatedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null, GameId = 2 },

                new CalendarEvent { Id = 3, Title = "Evento de The Lord of the Rings",Price = 0, AvailablePlace = 14, Start = new DateTime(2026, 03, 12), End = new DateTime(2026, 03, 13), Type = "CardGames", IsAllDay = false, CreatedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null, GameId = 3 },


                new CalendarEvent { Id = 4, Title = "Evento de Arkham Horror",Price = 3, AvailablePlace = 8, Start = new DateTime(2026, 02, 12), End = new DateTime(2026, 02, 13), Type = "CardGames", IsAllDay = false, CreatedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null, GameId = 4 },


                new CalendarEvent { Id = 5, Title = "Evento de vampire vtes",Price = 5, AvailablePlace = 10, Start = new DateTime(2026, 01, 12), End = new DateTime(2026, 01, 13), Type = "CardGames", IsAllDay = false, CreatedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null, GameId = 5 },


                new CalendarEvent { Id = 6, Title = "Evento de MAGIC. The gathering",Price = 0, AvailablePlace = 20, Start = new DateTime(2026, 05, 12), End = new DateTime(2026, 05, 13), Type = "CardGames", IsAllDay = false, CreatedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null, GameId = 6 }


                );


            builder.Entity<Enrollment>().HasData(


                new Enrollment { EnrollmentId = 1, GameId = 1, MemberId = 1 },
                new Enrollment { EnrollmentId = 2, GameId = 1, MemberId = 2 },
                new Enrollment { EnrollmentId = 3, GameId = 1, MemberId = 3 },
                new Enrollment { EnrollmentId = 4, GameId = 1, MemberId = 4 },
                new Enrollment { EnrollmentId = 5, GameId = 1, MemberId = 5 },

                new Enrollment { EnrollmentId = 6, GameId = 2, MemberId = 1 },
                new Enrollment { EnrollmentId = 7, GameId = 2, MemberId = 2 },
                new Enrollment { EnrollmentId = 8, GameId = 2, MemberId = 3 },
                new Enrollment { EnrollmentId = 9, GameId = 2, MemberId = 4 },


                new Enrollment { EnrollmentId = 11, GameId = 3, MemberId = 1 },
                new Enrollment { EnrollmentId = 12, GameId = 3, MemberId = 3 },
                new Enrollment { EnrollmentId = 13, GameId = 3, MemberId = 4 },

                new Enrollment { EnrollmentId = 14, GameId = 4, MemberId = 2 },
                new Enrollment { EnrollmentId = 15, GameId = 4, MemberId = 3 },
                new Enrollment { EnrollmentId = 16, GameId = 4, MemberId = 4 },
                new Enrollment { EnrollmentId = 17, GameId = 4, MemberId = 5 },

                new Enrollment { EnrollmentId = 18, GameId = 5, MemberId = 2 },
                new Enrollment { EnrollmentId = 19, GameId = 5, MemberId = 3 },
                new Enrollment { EnrollmentId = 20, GameId = 5, MemberId = 4 },


                new Enrollment { EnrollmentId = 21, GameId = 6, MemberId = 2 },
                new Enrollment { EnrollmentId = 22, GameId = 6, MemberId = 3 },
                new Enrollment { EnrollmentId = 23, GameId = 6, MemberId = 4 },
                new Enrollment { EnrollmentId = 24, GameId = 6, MemberId = 5 },
                new Enrollment { EnrollmentId = 25, GameId = 6, MemberId = 1 }
                


                );


            



            builder.Entity<Member>().HasData(



                new Member
                {
                    MemberId = 1,
                    MemberName = "Sophie Lefèvre",
                    MemberEmail = "SophieLeF@Contact.com",
                    MemberAddress = "Calle Gran Vía 12",
                    MemberPhone = "+34 612 345 678"
                },
                new Member
                {
                    MemberId = 2,
                    MemberName = "Alejandro García",
                    MemberEmail = "AlejandroG@Contact.com",
                    MemberAddress = "Avenida Diagonal 45",
                    MemberPhone = "+34 622 987 654"
                },
                new Member
                {
                    MemberId = 3,
                    MemberName = "Julien Moreau",
                    MemberEmail = "JulienM@Contact.com",
                    MemberAddress = "Calle Larios 8",
                    MemberPhone = "+34 633 456 789"
                },
                new Member
                {
                    MemberId = 4,
                    MemberName = "David Wilson",
                    MemberEmail = "DavidW@Contact.com",
                    MemberAddress = "Paseo de la Castellana 221",
                    MemberPhone = "+34 644 321 987"
                },
                new Member
                {
                    MemberId = 5,
                    MemberName = "Lukas Müller",
                    MemberEmail = "LukasM@Contact.com",
                    MemberAddress = "Calle Mayor 10",
                    MemberPhone = "+34 655 789 123"
                }



                );
        }
    }
}

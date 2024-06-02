
namespace WebApplication2
{
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var connectionString = builder.Configuration.GetConnectionString("MySqlConn");
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            //app.MapControllerRoute(
            //    name: "HostDashboard",
            //    pattern: "{action=HostDashboard}/{HostId?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}"
            );
            //app.MapControllerRoute(
            //    name: "hostDashboard",
            //    pattern: "HostDashboard",
            //    defaults: new { controller = "Host", action = "Index" }
            //);
            app.MapControllerRoute(
                name: "userWithId",
                pattern: "User/{user_id}",
                defaults: new { controller = "User", action = "Id" }
            );
            app.MapControllerRoute(
                name: "userWithIdWithHostId",
                pattern: "User/{user_id}/Host/{host_id}",
                defaults: new { controller = "Host", action = "Index" }
            );
            app.MapControllerRoute(
                name: "userWithId",
                pattern: "Slot/{invitation_code}",
                defaults: new { controller = "Slot", action = "InvitationCode" }
            );
            app.MapControllerRoute(
                name: "userWithId",
                pattern: "User/{user_id}/Slot/{slot_id}/calendar",
                defaults: new { controller = "Slot", action = "Calendar" }
            );
            //app.MapControllerRoute(
            //    name: "hostDashboardWithIdWithSlotWithId",
            //    pattern: "HostDashboard/{host_id}/Slot/{slot_id}",
            //    defaults: new { controller = "Host", action = "Slot" }
            //);
            //app.MapControllerRoute(
            //    name: "adminDashboardWithId",
            //    pattern: "AdminDashboard/{id}",
            //    defaults: new { controller = "Admin", action = "Id" }
            //);
            //app.MapControllerRoute(
            //    name: "adminDashboardWithIdWithSlotWithSlotNameSlotCode",
            //    pattern: "AdminDashboard/{admin_id}/Slot/{slot_name_slot_code}",
            //    defaults: new { controller = "Admin", action = "Slot" }
            //);
            //app.MapControllerRoute(
            //    name: "reserveeDashboardWithId",
            //    pattern: "ReserveeDashboard/{id}",
            //    defaults: new { controller = "Reservee", action = "Id" }
            //);
            //app.MapControllerRoute(
            //    name: "reserveeDashboardWithIdWithSlotWithId",
            //    pattern: "ReserveeDashboard/{reservee_id}/Slot/{slot_id}",
            //    defaults: new { controller = "Reservee", action = "Slot" }
            //);
            app.Run();
        }
    }
}
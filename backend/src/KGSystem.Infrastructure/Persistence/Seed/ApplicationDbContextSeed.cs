using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;
using KGSystem.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KGSystem.Infrastructure.Persistence.Seed;

public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context, ILogger logger, UserManager<IdentityUser>? userManager = null, RoleManager<IdentityRole>? roleManager = null)
    {
        try
        {
            await context.Database.MigrateAsync();

            // Seed Roles
            if (roleManager != null && !await roleManager.RoleExistsAsync("Manager"))
            {
                await roleManager.CreateAsync(new IdentityRole("Manager"));
                await roleManager.CreateAsync(new IdentityRole("Accountant"));
            }

            // Seed Users
            if (userManager != null && await userManager.FindByNameAsync("admin") == null)
            {
                var adminUser = new IdentityUser { UserName = "admin", Email = "admin@kgsystem.com", EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Manager");
                await userManager.AddToRoleAsync(adminUser, "Accountant");

                var accountantUser = new IdentityUser { UserName = "accountant", Email = "accountant@kgsystem.com", EmailConfirmed = true };
                await userManager.CreateAsync(accountantUser, "Accountant@123");
                await userManager.AddToRoleAsync(accountantUser, "Accountant");

                var managerUser = new IdentityUser { UserName = "manager", Email = "manager@kgsystem.com", EmailConfirmed = true };
                await userManager.CreateAsync(managerUser, "Manager@123");
                await userManager.AddToRoleAsync(managerUser, "Manager");
            }

            // Seed KGPhases
            if (!await context.KGPhases.AnyAsync())
            {
                context.KGPhases.AddRange(
                    new KGPhase("KG1", "الروضة الأولى", "First Kindergarten", 1),
                    new KGPhase("KG2", "الروضة الثانية", "Second Kindergarten", 2));
                await context.SaveChangesAsync();
            }

            // Seed AcademicYears
            if (!await context.AcademicYears.AnyAsync())
            {
                context.AcademicYears.AddRange(
                    new AcademicYear("2024-2025", "العام الدراسي 2024-2025", "Academic Year 2024-2025", new DateTime(2024, 9, 1), new DateTime(2025, 6, 30)),
                    new AcademicYear("2025-2026", "العام الدراسي 2025-2026", "Academic Year 2025-2026", new DateTime(2025, 9, 1), new DateTime(2026, 6, 30), true));
                await context.SaveChangesAsync();
            }

            // Seed MonthlyFees
            if (!await context.MonthlyFees.AnyAsync())
            {
                var years = await context.AcademicYears.ToListAsync();

                foreach (var year in years)
                {
                    for (var m = 1; m <= 12; m++)
                    {
                        var dueDate = new DateTime(year.StartDate.Year + (m < year.StartDate.Month ? 1 : 0), m, 5);
                        context.MonthlyFees.Add(
                            new MonthlyFee(year.Id, m, Money.Create(1500m, "EGP"), dueDate));
                    }
                }
                await context.SaveChangesAsync();
            }

            // Seed Sample Children
            if (!await context.Children.AnyAsync())
            {
                var kg1Phase = await context.KGPhases.FirstAsync(p => p.Code == "KG1");
                var kg2Phase = await context.KGPhases.FirstAsync(p => p.Code == "KG2");
                var activeYear = await context.AcademicYears.FirstAsync(y => y.IsActive);

                var children = new List<Child>
                {
                    new("أحمد", "Ahmed", "علي", "Ali", new DateTime(2019, 5, 15), Gender.Male, "علي محمد", "Ali Mohamed", "01001111111", "ali@example.com", "مصري", "القاهرة"),
                    new("سارة", "Sara", "حسن", "Hassan", new DateTime(2019, 8, 20), Gender.Female, "حسن أحمد", "Hassan Ahmed", "01002222222", "hassan@example.com", "مصري", "الجيزة"),
                    new("محمد", "Mohamed", "خالد", "Khaled", new DateTime(2018, 3, 10), Gender.Male, "خالد محمود", "Khaled Mahmoud", "01003333333", "khaled@example.com", "مصري", "الإسكندرية"),
                    new("فاطمة", "Fatma", "عمر", "Omar", new DateTime(2018, 11, 5), Gender.Female, "عمر عبدالله", "Omar Abdullah", "01004444444", "omar@example.com", "مصري", "القاهرة"),
                    new("يوسف", "Youssef", "إبراهيم", "Ibrahim", new DateTime(2019, 1, 25), Gender.Male, "إبراهيم محمد", "Ibrahim Mohamed", "01005555555", "ibrahim@example.com", "مصري", "مدينة نصر"),
                };

                foreach (var child in children)
                {
                    context.Children.Add(child);
                }
                await context.SaveChangesAsync();

                foreach (var child in children)
                {
                    child.ClearDomainEvents();
                }

                // Create enrollments for sample children
                foreach (var child in children)
                {
                    var phase = children.IndexOf(child) < 3 ? kg1Phase : kg2Phase;
                    var enrollment = new Enrollment(child.Id, phase.Id, activeYear.Id, "Seed enrollment");
                    context.Enrollments.Add(enrollment);
                }
                await context.SaveChangesAsync();

                // Create sample payments for current month
                var enrollments = await context.Enrollments.Include(e => e.Child).ToListAsync();
                var currentMonth = DateTime.UtcNow.Month;
                var currentYear = DateTime.UtcNow.Year;
                var random = new Random();

                foreach (var enrollment in enrollments)
                {
                    var fee = await context.MonthlyFees
                        .FirstOrDefaultAsync(f => f.AcademicYearId == enrollment.AcademicYearId && f.Month == currentMonth);

                    if (fee != null)
                    {
                        var paid = random.Next(2) == 0;
                        var dueDate = fee.DueDate ?? new DateTime(currentYear, currentMonth, 5);
                        var payment = new Payment(
                            enrollment.Id,
                            currentMonth,
                            currentYear,
                            fee.Amount.Amount,
                            dueDate);

                        if (paid)
                        {
                            payment.RecordPayment(fee.Amount.Amount, PaymentMethod.Cash, "system");
                        }

                        context.Payments.Add(payment);
                    }
                }
                await context.SaveChangesAsync();
            }

            // Seed BrandingSettings
            if (!await context.BrandingSettings.AnyAsync())
            {
                context.BrandingSettings.Add(
                    BrandingSetting.CreateDefault(
                        BrandingDefaults.AppName,
                        BrandingDefaults.AppNameAr,
                        BrandingDefaults.LogoUrl,
                        null,
                        BrandingDefaults.PrimaryColor,
                        BrandingDefaults.SecondaryColor,
                        BrandingDefaults.Currency));
                await context.SaveChangesAsync();
            }
            else
            {
                var existing = await context.BrandingSettings.FirstOrDefaultAsync();
                if (existing != null)
                {
                    var appNameAr = string.IsNullOrWhiteSpace(existing.AppNameAr) ? BrandingDefaults.AppNameAr : existing.AppNameAr;
                    var currency = string.IsNullOrWhiteSpace(existing.Currency) ? BrandingDefaults.Currency : existing.Currency;
                    existing.Update(existing.AppName, appNameAr, existing.LogoUrl, existing.LogoData, existing.PrimaryColor, existing.SecondaryColor, currency);
                    await context.SaveChangesAsync();
                }
            }

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}

using Backend.TopUp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backend.TopUp.Infrastructure.Configuration.Db
{
    public static class Seed
    {
        public static void EnsureDatabaseCreated(IHost app)
        {
            try
            {
                var serviceScopeFactory = app.Services.GetService<IServiceScopeFactory>();

                if (serviceScopeFactory is null) 
                    throw new NullReferenceException();

                using var serviceScope = serviceScopeFactory.CreateScope();
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();

                dbContext.Database.Migrate();

                // DONT USE THIS IN PRODUCTION
                if (!dbContext.TopUpOptions.Any(x => x.CurrencyAbbreviation == "AED")) 
                {
                    dbContext.TopUpOptions.AddRange(_aedTopUpOptionsSeed);
                    dbContext.SaveChanges();
                }

                serviceScope.Dispose();

            }
            catch (Exception ex)
            {
                // todo: put some log here
                throw;
            }

        }

        private static List<TopUpOption> _aedTopUpOptionsSeed
        {
            get 
            {  
                return [new("AED", 5, true),
                        new("AED", 10, true),
                        new("AED", 20, true),
                        new("AED", 30, true),
                        new("AED", 50, true),
                        new("AED", 75, true),
                        new("AED", 100, true)];
            }
        } 
    }
}

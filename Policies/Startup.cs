using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Policies.Services;
using Polly;
using Polly.Retry;

namespace Policies
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        private static AsyncRetryPolicy AsyncRetryPolicy => Policy
            .Handle<FlakyStartupException>(ex =>
            {
                Console.WriteLine(ex.Message);
                return true;
            })
            .WaitAndRetryForeverAsync(i => TimeSpan.FromMilliseconds(100));
        
        private static RetryPolicy RetryPolicy => Policy
            .Handle<FlakyStartupException>(ex =>
            {
                Console.WriteLine(ex.Message);
                return true;
            })
            .WaitAndRetryForever(i => TimeSpan.FromMilliseconds(100));

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IFlakyStartupService, FlakyStartupService>();
            services.AddSingleton<ISlowStartupService, SlowStartupService>();
            services.AddSingleton<IQuickStartupService, QuickStartupService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env,
            IFlakyStartupService flaky,
            IQuickStartupService quick,
            ISlowStartupService slow,
            ILogger<Startup> logger)
        {
            // Starting services in same order: Flaky -> Slow -> Quick
            //StartServicesAsyncWithAwait(flaky, quick, slow, logger).GetAwaiter().GetResult();
            //StartServicesAsyncInParallel(flaky, quick, slow, logger).GetAwaiter().GetResult();
            //StartServicesTasks(flaky, quick, slow, logger).GetAwaiter().GetResult();
            //StartServicesSync(flaky, quick, slow, logger);
            //StartServicesAsyncInSync(flaky, quick, slow, logger);
            
            Console.WriteLine("Doing other stuff....");
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private async Task StartServicesAsyncWithAwait(
            IFlakyStartupService flaky,
            IQuickStartupService quick,
            ISlowStartupService slow,
            ILogger<Startup> logger)
        {
            logger.LogInformation("Starting Polly Stuff");

            var cancellationTokenSource = new CancellationTokenSource();
            
            // Awaits block the executing thread.
            // Flaky has to finish before Slow starts
            // Slow has to finish before Quick starts
            
            await AsyncRetryPolicy.ExecuteAsync(() => flaky.Start(cancellationTokenSource.Token));
            await AsyncRetryPolicy.ExecuteAsync(() => slow.Start(cancellationTokenSource.Token));
            await AsyncRetryPolicy.ExecuteAsync(() => quick.Start(cancellationTokenSource.Token));
        }
        
        private Task StartServicesTasks(
            IFlakyStartupService flaky,
            IQuickStartupService quick,
            ISlowStartupService slow,
            ILogger<Startup> logger)
        {
            logger.LogInformation("Starting Polly Stuff");

            var cancellationTokenSource = new CancellationTokenSource();
            
            // Awaits block the executing thread.
            // Flaky has to finish before Slow starts
            // Slow has to finish before Quick starts
            // possible return values are not available due to missing await
            
            var t1 = AsyncRetryPolicy.ExecuteAsync(() => flaky.Start(cancellationTokenSource.Token));
            var t2 = AsyncRetryPolicy.ExecuteAsync(() => slow.Start(cancellationTokenSource.Token));
            var t3 = AsyncRetryPolicy.ExecuteAsync(() => quick.Start(cancellationTokenSource.Token));

            return Task.WhenAll(new[] { t1, t2, t3 });
        }
        
        private async Task StartServicesAsyncInParallel(
            IFlakyStartupService flaky,
            IQuickStartupService quick,
            ISlowStartupService slow,
            ILogger<Startup> logger)
        {
            logger.LogInformation("Starting Polly Stuff");

            var cancellationTokenSource = new CancellationTokenSource();
            
            // Execution tasks are started in parallel if thread has enough capacities
            // Flaky or Fast finish first
            // Slow and Flaky do not block
            
            var t1 = AsyncRetryPolicy.ExecuteAsync(() => flaky.Start(cancellationTokenSource.Token));
            var t2 = AsyncRetryPolicy.ExecuteAsync(() => slow.Start(cancellationTokenSource.Token));
            var t3 =  AsyncRetryPolicy.ExecuteAsync(() => quick.Start(cancellationTokenSource.Token));

            await t1;
            await t2;
            await t3;
        }
        
        private void StartServicesSync(
            IFlakyStartupService flaky,
            IQuickStartupService quick,
            ISlowStartupService slow,
            ILogger<Startup> logger)
        {
            logger.LogInformation("Starting Polly Stuff");

            var cancellationTokenSource = new CancellationTokenSource();
            
            // Calls block the executing thread.
            // Flaky has to finish before Slow starts
            // Slow has to finish before Quick starts
            
            RetryPolicy.Execute(() => flaky.Start(cancellationTokenSource.Token));
            RetryPolicy.Execute(() => slow.Start(cancellationTokenSource.Token));
            RetryPolicy.Execute(() => quick.Start(cancellationTokenSource.Token));
        }
        
        private void StartServicesAsyncInSync(
            IFlakyStartupService flaky,
            IQuickStartupService quick,
            ISlowStartupService slow,
            ILogger<Startup> logger)
        {
            logger.LogInformation("Starting Polly Stuff");

            var cancellationTokenSource = new CancellationTokenSource();
            
            // Starts tasks if function is really async (needs await)
            
            AsyncRetryPolicy.ExecuteAsync(() => flaky.Start(cancellationTokenSource.Token));
            AsyncRetryPolicy.ExecuteAsync(() => slow.Start(cancellationTokenSource.Token));
            AsyncRetryPolicy.ExecuteAsync(() => quick.Start(cancellationTokenSource.Token));
        }
    }
}

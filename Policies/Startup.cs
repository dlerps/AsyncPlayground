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
            //StartServicesAsyncWithAwait(flaky, quick, slow, logger).GetAwaiter().GetResult();
            //StartServicesAsyncInParallel(flaky, quick, slow, logger).GetAwaiter().GetResult();
            //StartServicesSync(flaky, quick, slow, logger);
            StartServicesAsyncInSync(flaky, quick, slow, logger);
            
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
            
            await AsyncRetryPolicy.ExecuteAsync(() => flaky.Start(cancellationTokenSource.Token));
            await AsyncRetryPolicy.ExecuteAsync(() => slow.Start(cancellationTokenSource.Token));
            await AsyncRetryPolicy.ExecuteAsync(() => quick.Start(cancellationTokenSource.Token));
        }
        
        private async Task StartServicesAsyncInParallel(
            IFlakyStartupService flaky,
            IQuickStartupService quick,
            ISlowStartupService slow,
            ILogger<Startup> logger)
        {
            logger.LogInformation("Starting Polly Stuff");

            var cancellationTokenSource = new CancellationTokenSource();
            
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
            
            AsyncRetryPolicy.ExecuteAsync(() => flaky.Start(cancellationTokenSource.Token));
            AsyncRetryPolicy.ExecuteAsync(() => slow.Start(cancellationTokenSource.Token));
            AsyncRetryPolicy.ExecuteAsync(() => quick.Start(cancellationTokenSource.Token));
        }
    }
}

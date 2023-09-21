using WorkerService;
using Shared;
using WorkerService.BackgroundServices;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.SetRabbitMQLifeTime();

        services.AddHostedService<FanoutWorker>();
        services.AddHostedService<DirectWorker>();
        services.AddHostedService<TopicWorker>();
        services.AddHostedService<HeaderWorker>();
        //services.AddHostedService<Worker>();
    })
    .Build();

host.Run();

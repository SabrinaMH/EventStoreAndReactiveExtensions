using System;
using System.Net;
using System.Threading;
using CommonLibrary;
using CommonLibrary.Domain;
using EventStore.ClientAPI;

namespace Projections
{
    class Program
    {
        static void Main(string[] args)
        {
            new Projection("ByProject").Create();
            new Projection("ByUser").Create();
            EmitEvents();

            var settings = ConnectionSettings.Create();
            settings.SetDefaultUserCredentials(EventStoreUtils.UserCredentials);
            using (
                var connection = EventStoreConnection.Create(settings,
                    new IPEndPoint(IPAddress.Loopback, EventStoreUtils.DefaultTcpPort)))
            {
                connection.ConnectAsync().Wait();

                //var catchUpSubscription = new CatchUpSubscription("$ce-Project");
                var catchUpSubscription = new CatchUpSubscription("ByProject");
                catchUpSubscription.Run(connection);
                Console.ReadKey();
            }
        }

        private static void EmitEvents()
        {
            int counter = 0;
            var projectRepo = new EventStoreRepository<Project>();
            var taskRepo = new EventStoreRepository<Task>();

            System.Threading.Tasks.Task.Run(() =>
            {
                while (true)
                {
                    var user = counter % 2 == 0 ? "Sabrina" : "Teddy";
                    counter++;

                    var project = new Project(Guid.NewGuid().ToString(), user);
                    project.MoveDeadline(DateTime.UtcNow);
                    projectRepo.Save(project);

                    var task = new Task(project.Id, Guid.NewGuid().ToString(), user);
                    task.ChangeTitle(Guid.NewGuid().ToString());
                    taskRepo.Save(task);

                    Thread.Sleep(1000);
                }
            });
        }
    }
}

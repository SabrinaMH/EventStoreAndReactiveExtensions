using System;
using System.Net;
using System.Reactive.Linq;
using CommonLibrary;
using CommonLibrary.Domain;
using EventStore.ClientAPI;

namespace ReactiveExtensions
{
    class Program
    {
        public static int NumberOfEventsProcessed = 0;

        static void Main(string[] args)
        {
            ReactiveExtensionsTest();
        }

        static void ReactiveExtensionsTest()
        {

            var settings = ConnectionSettings.Create();
            settings.SetDefaultUserCredentials(EventStoreUtils.UserCredentials);
            using (
                var connection =
                    EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Loopback,
                        EventStoreUtils.DefaultTcpPort)))
            {
                connection.ConnectAsync().Wait();
                var catchUpSubscriptionObservable = new CatchUpSubscriptionObservable<ProjectRegistered>(connection);
                var observable = catchUpSubscriptionObservable.Buffer(TimeSpan.FromSeconds(5));
                observable.Subscribe(projectsRegistered =>
                {
                    Console.WriteLine($"Number of events processed: {NumberOfEventsProcessed}");
                    Console.WriteLine("Time: {0}. Number of registered projects: {1}", DateTime.UtcNow,
                        projectsRegistered.Count);
                });

                //var catchUpSubscriptionObservable = new CatchUpSubscriptionObservable<ProjectRegistered>(connection);
                //var observer = new Observer<ProjectRegistered>();
                //catchUpSubscriptionObservable.Subscribe(observer);
                Console.ReadKey();
            }
        }
    }
}

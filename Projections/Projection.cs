using System;
using System.IO;
using System.Linq;
using System.Net;
using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.Projections;

namespace Projections
{
    public class Projection
    {
        private readonly string _projectionName;

        public Projection(string projectionName)
        {
            _projectionName = projectionName;
        }

        public void Create()
        {
            Console.WriteLine($"Creating projection {_projectionName}");
            var projectionsManager = new ProjectionsManager(new ConsoleLogger(),
                new IPEndPoint(IPAddress.Loopback, CommonLibrary.EventStoreUtils.DefaultHttpPort), TimeSpan.FromSeconds(10));

            var existingProjections = projectionsManager.ListAllAsync(CommonLibrary.EventStoreUtils.UserCredentials).Result;

            if (existingProjections.Any(x => x.Name.Equals(_projectionName)))
                return;

            var projectionCode = ReadProjectionCode(_projectionName);
            projectionsManager.CreateContinuousAsync(_projectionName, projectionCode, CommonLibrary.EventStoreUtils.UserCredentials).Wait();
        }

        private string ReadProjectionCode(string projectionFileName)
        {
            var fullName = string.Format("{0}.ProjectionSources.{1}.js", typeof(Projection).Namespace, projectionFileName);
            using (var stream = ReadStream(fullName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private Stream ReadStream(string fullName)
        {
            var assembly = typeof(Program).Assembly;
            var stream = assembly.GetManifestResourceStream(fullName);

            if (stream == null) throw new InvalidOperationException(string.Format("Stream '{0}' not found!", fullName));

            return stream;
        }
    }
}
using System;
using System.Collections.Generic;

namespace OpenTerrariaClient.Client.Service
{
    // https://github.com/RogueException/Discord.Net/blob/master/src/Discord.Net/ServiceManager.cs
    public class ServiceManager
    {
        private readonly Dictionary<Type, IService> _services;

        internal TerrariaClient Client { get; }

        internal ServiceManager(TerrariaClient listener)
        {
            Client = listener;
            _services = new Dictionary<Type, IService>();
        }

        public T Add<T>(T service)
            where T : class, IService
        {
            _services.Add(typeof (T), service);
            service.Install(Client);
            return service;
        }

        public T Get<T>()
            where T : class, IService
        {
            IService service;
            T singletonT = null;

            if (_services.TryGetValue(typeof (T), out service))
                singletonT = service as T;

            return singletonT;
        }
    }

    public interface IService
    {
        void Install(TerrariaClient client);
    }
}

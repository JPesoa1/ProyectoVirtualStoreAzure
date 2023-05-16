using StackExchange.Redis;

namespace ProyectoVirtualStore.Helpers
{
    public static class HelperCacheMultiplexer
    {
        private static Lazy<ConnectionMultiplexer> CreateConnection =
            new Lazy<ConnectionMultiplexer>(() =>
            {
                string cnn = "bbddtiendaredisjpv.redis.cache.windows.net:6380,password=AvCu32mnWH5BKfGF2q1Bqva69JJGB98b6AzCaCNvkbU=,ssl=True,abortConnect=False";
                return ConnectionMultiplexer.Connect(cnn);
            });



        public static ConnectionMultiplexer GetConnection
        {
            get
            {
                return CreateConnection.Value;
            }

        }
    }
}

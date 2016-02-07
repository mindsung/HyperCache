using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MindSung.HyperCache.Test
{
    using Providers;

    public class StorageProviderTests
    {
        class TestObject
        {
        }

        static IStorageProviderFactory factory = new FileSystemStorageProviderFactory("C:/hypercache");

        static IStorageProvider<TestObject> provider = factory.Create<TestObject>("app", "test");

        async Task<string> AddKey(string key = null)
        {
            key = key ?? Guid.NewGuid().ToString();
            await provider.AddOrUpdate(key, ObjectProxy<TestObject>.FromSerialized(key, key));
            return key;
        }

        [Fact]
        public async Task AddOrUpdate()
        {
            var key = await AddKey();
            await AddKey(key);
        }

        [Fact]
        public async Task Get()
        {
            var key = await AddKey();
            var readback = await provider.Get(key);
            Assert.True(readback.Key == key && readback.Serialized == key, "The serialized value read back was not the expected value.");
        }

        [Fact]
        public async Task GetAllAndDelete()
        {
            await AddKey();
            var keys = (await provider.GetAll()).Select(proxy => proxy.Key).ToList();
            Assert.True(keys.Count > 0, "Expected at least one object, found none.");
            foreach (var key in keys)
            {
                await provider.Remove(key);
            }
            keys = (await provider.GetAll()).Select(proxy => proxy.Serialized).ToList();
            Assert.True(keys.Count == 0, "Not all objects were deleted.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace MindSung.HyperCache.Providers
{
    public class FileSystemStorageProviderFactory : IStorageProviderFactory
    {
        class FileSystemStorageProvider<T> : IStorageProvider<T>
        {
            public FileSystemStorageProvider(string applicationScope, string typeScope, string basePath)
            {
                this.applicationScope = applicationScope ?? "default";
                this.typeScope = typeScope ?? "default";
                path = basePath + this.applicationScope + "/" + this.typeScope + "/";
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }

            string applicationScope;
            string typeScope;
            string path;

            public async Task<ObjectProxy<T>> Get(string key)
            {
                var fileName = path + key;
                if (!File.Exists(fileName)) { return null; }
                try
                {
                    using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    using (var reader = new StreamReader(stream))
                    {
                        return ObjectProxy<T>.FromSerialized(key, await reader.ReadToEndAsync());
                    }
                }
                // May have been removed between checking existence and opening.
                catch (FileNotFoundException) { return null; }
            }

            public async Task<IEnumerable<ObjectProxy<T>>> GetAll()
            {
                var all = new List<ObjectProxy<T>>();
                foreach (var filePath in Directory.EnumerateFiles(path))
                {
                    var key = Path.GetFileName(filePath);
                    var proxy = await Get(key);
                    if (proxy != null) { all.Add(proxy); }
                }
                return all;
            }

            public async Task AddOrUpdate(string key, ObjectProxy<T> value)
            {
                using (var stream = new FileStream(path + key, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(value.Serialized);
                }
            }

            public Task Remove(string key)
            {
                File.Delete(path + key);
                return Task.FromResult(true);
            }
        }

        public FileSystemStorageProviderFactory(string basePath = null)
        {
            this.basePath = basePath ?? "./data/";
            if (!(this.basePath.EndsWith("\\") || this.basePath.EndsWith("/")))
            {
                this.basePath = this.basePath + "/";
            }
        }

        string basePath;

        public IStorageProvider<T> Create<T>(string applicationScope, string typeScope)
        {
            return new FileSystemStorageProvider<T>(applicationScope, typeScope, basePath);
        }
    }
}

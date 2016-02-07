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
                this.path = basePath + this.applicationScope + "/" + this.typeScope + "/";
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
                    using (var reader = new StreamReader(fileName))
                    {
                        return new ObjectProxy<T>(await reader.ReadToEndAsync());
                    }
                }
                // May have been removed between checking existence and opening.
                catch (FileNotFoundException) { return null; }
            }

            public Task<IEnumerable<T>> GetAll()
            {
                // TODO
                return Task.FromResult(Enumerable.Empty<T>());
            }

            public async Task AddOrUpdate(string key, ObjectProxy<T> value)
            {
                using (var writer = new StreamWriter(path + key, false))
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
            this.basePath = basePath ??
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                + System.Reflection.Assembly.GetEntryAssembly().GetName().Name
                + "/data/";
            if (!this.basePath.EndsWith("\\") || this.basePath.EndsWith("/"))
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

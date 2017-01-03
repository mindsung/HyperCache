using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MindSung.HyperState;
using MindSung.HyperState.AspNetCore;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace MindSung.Test.HyperState.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public ValuesController(IWebObjectProxyFactory<string> factory)
        {
            this.factory = factory;

            if (simpleCache == null)
            {
                simpleCache = new ConcurrentDictionary<int, string>();
                var val1 = factory.FromObject(new MyValues { Id = 1234, Value = "hello" });
                var val2 = factory.FromObject(new MyValues { Id = 2345, Value = "there" });
                simpleCache[val1.Object.Id] = val1.Serialized;
                simpleCache[val2.Object.Id] = val2.Serialized;
            }
        }

        IWebObjectProxyFactory<string> factory;

        public class MyValues
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }

        static ConcurrentDictionary<int, string> simpleCache;

        // GET api/values
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(simpleCache.Values.Select(s => factory.FromSerialized<MyValues>(s)));
        }

        // GET api/values/{id}
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            string s;
            if (!simpleCache.TryGetValue(id, out s))
            {
                return NotFound();
            }
            return Ok(factory.FromSerialized<MyValues>(s));
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody]IObjectProxy<MyValues, string> proxy)
        {
            if (!simpleCache.TryAdd(proxy.Object.Id, proxy.Serialized))
            {
                throw new Exception("An item with the specified ID already exists.");
            }
            return Ok(proxy);
        }

        // PUT api/values/{id}
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody]IObjectProxy<MyValues, string> proxy)
        {
            if (!simpleCache.ContainsKey(id))
            {
                return NotFound();
            }
            if (id != proxy.Object.Id)
            {
                throw new Exception("The request update ID doesn't match that of the item.");
            }
            simpleCache[proxy.Object.Id] = proxy.Serialized;
            return Ok(proxy);
        }

        // POST api/values/put/{id}
        [HttpPost("put/{id}")]
        public ActionResult PostPut(int id, [FromBody]IObjectProxy<MyValues, string> proxy)
        {
            return Put(id, proxy);
        }

        // PATCH api/values
        [HttpPatch]
        public ActionResult Patch([FromBody]IEnumerable<IObjectProxy<MyValues, string>> values)
        {
            foreach (var proxy in values)
            {
                simpleCache[proxy.Object.Id] = proxy.Serialized;
            }
            return Ok(values);
        }

        // POST api/values/patch
        [HttpPost("patch")]
        public ActionResult PostPatch([FromBody]IEnumerable<IObjectProxy<MyValues, string>> values)
        {
            return Patch(values);
        }

        // DELETE api/values/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            string _;
            if (!simpleCache.TryRemove(id, out _))
            {
                return NotFound();
            }
            return Ok();
        }

        // GET api/values/delete/{id}
        [HttpGet("delete/{id}")]
        public ActionResult GetDelete(int id)
        {
            return Delete(id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MindSung.HyperState;
using MindSung.HyperState.AspNetCore;

namespace MindSung.Test.HyperState.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class JsonValuesController : Controller
    {
        public JsonValuesController(JsonWebObjectProxyFactory factory)
        {
            this.factory = factory;

            var val1 = factory.FromObject(new MyValues { IntValue = 1234, StringValue = "hello" });
            var val2 = factory.FromObject(new MyValues { IntValue = 2345, StringValue = "there" });
            myValues[val1.Object.IntValue] = val1;
            myValues[val2.Object.IntValue] = val2;
        }

        JsonWebObjectProxyFactory factory;

        public class MyValues
        {
            public int IntValue { get; set; }
            public string StringValue { get; set; }
        }

        static Dictionary<int, JsonObjectProxy<MyValues>> myValues = new Dictionary<int, JsonObjectProxy<MyValues>>();

        // GET api/values
        [HttpGet]
        public IEnumerable<MyValues> Get()
        {
            return myValues.Values.Select(v => v.Object);
        }

        // GET api/values/{id}
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            JsonObjectProxy<MyValues> val;
            if (!myValues.TryGetValue(id, out val))
            {
                return NotFound();
            }
            return Ok(val);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]JsonObjectProxy<MyValues> proxy)
        {
            var val = proxy.Object;
            myValues[val.IntValue] = proxy;
        }

        // PUT api/values/{id}
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody]JsonObjectProxy<MyValues> proxy)
        {
            if (!myValues.ContainsKey(id))
            {
                return NotFound();
            }
            var val = proxy.Object;
            myValues[val.IntValue] = proxy;
            return Ok();
        }

        // PATCH api/values
        [HttpPatch]
        public ActionResult Patch([FromBody]IEnumerable<JsonObjectProxy<MyValues>> values)
        {
            foreach (var proxy in values)
            {
                myValues[proxy.Object.IntValue] = proxy;
            }
            return Ok(values);
        }

        // POST api/values/patch
        [HttpPost("patch")]
        public ActionResult PostPatch([FromBody]IEnumerable<JsonObjectProxy<MyValues>> values)
        {
            return Patch(values);
        }

        // DELETE api/values/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (!myValues.Remove(id))
            {
                return NotFound();
            }
            return Ok();
        }
    }
}

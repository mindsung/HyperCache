using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MindSung.HyperState;

namespace MindSung.Test.HyperState.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public ValuesController()
        {
            var val1 = new ObjectProxy<MyValues>();
            val1.SetObject(new MyValues { IntValue = 1234, StringValue = "hello" });
            var val2 = new ObjectProxy<MyValues>();
            val2.SetObject(new MyValues { IntValue = 2345, StringValue = "there" });
            myValues[val1.GetObject().IntValue] = val1;
            myValues[val2.GetObject().IntValue] = val2;
        }

        public class MyValues
        {
            public int IntValue { get; set; }
            public string StringValue { get; set; }
        }

        static Dictionary<int, ObjectProxy<MyValues>> myValues = new Dictionary<int, ObjectProxy<MyValues>>();

        // GET api/values
        [HttpGet]
        public IEnumerable<MyValues> Get()
        {
            return myValues.Values.Select(v => v.GetObject());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            ObjectProxy<MyValues> val;
            if (!myValues.TryGetValue(id, out val))
            {
                return NotFound();
            }
            return Ok(val);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]ObjectProxy<MyValues> proxy)
        {
            var val = proxy.GetObject();
            myValues[val.IntValue] = proxy;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody]ObjectProxy<MyValues> proxy)
        {
            if (!myValues.ContainsKey(id))
            {
                return NotFound();
            }
            var val = proxy.GetObject();
            myValues[val.IntValue] = proxy;
            return Ok();
        }

        // DELETE api/values/5
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

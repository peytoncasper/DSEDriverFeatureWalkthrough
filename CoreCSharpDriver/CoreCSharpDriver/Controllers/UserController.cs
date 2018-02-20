using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dse;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreCSharpDriver.Controllers
{
    public class User {
        public Guid id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            IDseCluster cluster = DseCluster.Builder()
                .AddContactPoint("127.0.0.1")
                .Build();
            IDseSession session = cluster.Connect();

            var get_users = session.Prepare("SELECT * FROM driver_test.user");
            RowSet results = session.Execute(get_users.Bind());

            List<string> final_results = new List<string>();

            foreach(Row row in results.AsEnumerable()){
                final_results.Add(row.GetValue<Guid>("id").ToString() + " " + 
                                     row.GetValue<String>("first_name") + " " + 
                                     row.GetValue<String>("last_name") + "\n" );
                
            }


            return final_results;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(Guid id)
        {
            IDseCluster cluster = DseCluster.Builder()
                .AddContactPoint("127.0.0.1")
                .Build();
            IDseSession session = cluster.Connect();

            var get_user = session.Prepare("SELECT * FROM driver_test.user where id = ?");
            RowSet results = session.Execute(get_user.Bind(id));

            if (results.Any())
            {
                session.Dispose();
                Row user = results.First();
                return user.GetValue<String>("first_name") + user.GetValue<String>("last_name");
            }
            else
            {
                session.Dispose();
                return "User not found";
            }

        }

        // POST api/values
        [HttpPost]
        public Guid Post([FromBody]User user)
        {
            IDseCluster cluster = DseCluster.Builder()
                .AddContactPoint("127.0.0.1")
                .Build();
            IDseSession session = cluster.Connect();
            Guid user_id = Guid.NewGuid();
            var insert_user = session.Prepare(
                "INSERT INTO driver_test.user (id, first_name, last_name) VALUES(?,?,?)"
            );
            RowSet results = session.Execute(insert_user.Bind(user_id, user.first_name, user.last_name));
            return user_id;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

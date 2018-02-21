using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dse;
using Dse.Mapping;
using Dse.Data.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreCSharpDriver.Controllers
{
    public class User {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public User(Guid Id, string FirstName, string LastName){
            this.Id = id;
            this.FirstName = FirstName;
            this.LastName = LastName;
        }
    }

    [Route("api/[controller]")]
    public class MapperUserController : Controller
    {
        public Table<User> GetTable(){
            IDseCluster cluster = DseCluster.Builder()
                .AddContactPoint("127.0.0.1")
                .Build();
            IDseSession session = cluster.Connect();
            MappingConfiguration.Global.Define(
               new Map<User>()
                    .TableName("user")
                    .PartitionKey(u => u.Id)
                    .Column(u => u.Id, cm => cm.WithName("id"))
                    .Column(u => u.FirstName, cm => cm.WithName("first_name"))
                    .Column(u => u.LastName, cm => cm.WithName("last_name")));
            var users = new Table<User>(session);

            return users;

        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {


            Table<User> users = GetTable();
            IEnumerable<User> results = users.Execute();

            List<string> final_results = new List<string>();

            foreach(User user in results.AsEnumerable()){
                final_results.Add(user.Id + " " + 
                                  user.FirstName + " " + 
                                  user.LastName + "\n" );
                
            }


            return final_results;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(Guid id)
        {
            Table<User> users = GetTable();
            IEnumerable<User> results = users.Where(u => u.Id == id).Execute();

            if (results.Any())
            {
                User user = results.First();
                return user.FirstName + user.LastName;
            }
            return "User not found";
        }

        // POST api/values
        [HttpPost]
        public Guid Post([FromBody]User user)
        {
            Table<User> users = GetTable();

            Guid user_id = Guid.NewGuid();
            users.Insert(new User(
                user_id,
                user.first_name,
                user.last_name
            ));
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

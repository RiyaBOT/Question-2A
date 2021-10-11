using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using producer.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {

        [HttpPost]
        public UnauthorizedResult Post([FromBody] TaskItem task)
        {

            var r = new Request("/api/login", Method.POST);
            r.AddJsonBody(task)
            var cl = new RestClient("https://reqres.in/api/login");

            

            var response = cl.Execute(r);
            TokenDTO tokenobtained = new JsonDeserializer().Deserialize(response);
            //obtain the token from the response and then check

            if (tokenobtained == null)
                return Unauthorized(); //401 error code

            var factory = new ConnectionFactory()
            {   //HostName = "localhost" , 
                //Port = 30724
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
            };

            Console.WriteLine(factory.HostName + ":" + factory.Port);
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "TaskQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = task.task;
                string email = task.email;
                string password = task.password;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "TaskQueue",
                                     basicProperties: null,
                                     body: body);
            }
        }

        
        
            




        

    }
}

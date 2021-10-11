﻿using Microsoft.AspNetCore.Mvc;
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
        public void Post([FromBody] TaskItem task)
        {
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
                channel.QueueDeclare(queue: "tasks",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = task.task;
                string email = task.email;
                string password = task.password;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "tasks",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
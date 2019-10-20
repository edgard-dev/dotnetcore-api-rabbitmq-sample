using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Payments.API.Models;
using Payments.API.RabbitMQ;

namespace Payments.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "payment1", "payment2", "payment3" };
        }

        [HttpPost]        
        public ActionResult SendPayment([FromBody] Payment payment)
        {
            try
            {
                RabbitMQPublisher client = new RabbitMQPublisher();
                client.SendPayment(payment);
                client.Close();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok(payment);
        }

    }
}
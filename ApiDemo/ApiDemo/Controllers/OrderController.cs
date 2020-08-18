using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDemo.Models;
using DataLibrary.Data;
using DataLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]")] // Route = 'api/order'
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IFoodData foodData;
        private readonly IOrderData orderData;

        public OrderController(IFoodData foodData, IOrderData orderData)
        {
            this.foodData = foodData;
            this.orderData = orderData;
        }

        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status200OK)] //possible outcomes for the method - helpful for swagger etc.
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(OrderModel order) 
        {
            var food = await foodData.GetFood();
            order.Total = order.Quantity * food.Where(x => x.Id == order.FoodId).First().Price;
            int id = await orderData.CreateOrder(order);
            return Ok(new { Id = id }); //returning a new anonymous object just containing the Id
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            if (id == 0) { return BadRequest(); } //if default value for int - 0 - then the request is invalid

            var order = await orderData.GetOrderById(id); //get order

            if (order != null) //if there is an order
            {
                var food = await foodData.GetFood();
                var output = new //anon object
                {
                    Order = order,
                    ItemPurchased = food.Where(x => x.Id == order.FoodId).FirstOrDefault()?.Title
                };

                return Ok(output);
            }
            else { return NotFound(); } //if there is no order
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] OrderUpdateModel data)
        {
            await orderData.UpdateOrderName(data.Id, data.OrderName);

            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            await orderData.DeleteOrder(id);

            return Ok();
        }
    }
}

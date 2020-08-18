using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary.Data;
using DataLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodData foodData;

        public FoodController(IFoodData foodData)
        {
            this.foodData = foodData;
        }

        [HttpGet]
        //without status code attributes - status codes are returned automatically, but maybe not as relevant e.g. bad request if no db connection etc.
        public async Task<List<FoodModel>> Get() 
        {
            return await foodData.GetFood();
        }
    }
}

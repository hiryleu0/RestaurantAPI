using Microsoft.AspNetCore.Mvc;
using RestaurantAPI_ASP.NET_Core.Model;
using RestaurantAPI_ASP.NET_Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI_ASP.NET_Core.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private IDishService _dishService;
        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }
        [HttpPost]
        public ActionResult Post([FromRoute] int restaurantId, [FromBody] CreateDishDto dto)
        {
            var newDishId = _dishService.Create(restaurantId, dto);
            return Created($"api/restaurant/{restaurantId}/dish/{newDishId}", null);
        }

        [HttpGet("{dishId}")]
        public ActionResult<DishDto> Get([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var dish = _dishService.GetById(restaurantId, dishId);
            return Ok(dish);
        }
        [HttpGet]
        public ActionResult<List<DishDto>> GetAll([FromRoute] int restaurantId)
        {
            var dishes = _dishService.GetAll(restaurantId);
            return Ok(dishes);
        }
        [HttpDelete("{dishId}")]
        public ActionResult<DishDto> Delete([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            _dishService.Delete(restaurantId, dishId);
            return NoContent();
        }
        [HttpDelete]
        public ActionResult<DishDto> DeleteAll([FromRoute] int restaurantId)
        {
            _dishService.DeleteAll(restaurantId);
            return NoContent();
        }
    }
}

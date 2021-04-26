using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_ASP.NET_Core.Entities;
using RestaurantAPI_ASP.NET_Core.Model;
using RestaurantAPI_ASP.NET_Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI_ASP.NET_Core.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    [Authorize]
    public class RestaurantController : ControllerBase
    {
        private IRestaurantService _service;
        public RestaurantController(IRestaurantService service)
        {
            _service = service;
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            var restaurant = _service.GetById(id);
            return Ok(restaurant);
        }

        [HttpPost]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            var id = _service.Create(dto);
            return Created($"/api/restaurant/{id}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteRestaurant([FromRoute] int id)
        {
            _service.Delete(id);
            return NoContent(); 
        }
        [HttpPut("{id}")]
        public ActionResult UpdateRestaurant([FromBody] UpdateRestaurantDto dto, [FromRoute] int id)
        {
             _service.Update(dto, id);
            return Ok();
        }
    }
}

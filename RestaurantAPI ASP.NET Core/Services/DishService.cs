using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_ASP.NET_Core.Entities;
using RestaurantAPI_ASP.NET_Core.Exceptions;
using RestaurantAPI_ASP.NET_Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI_ASP.NET_Core.Services
{
    public class DishService : IDishService
    {
        private RestaurantDbContext _dbContext;
        private IMapper _mapper;
        public DishService(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public int Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dishEntity = _mapper.Map<Dish>(dto);
            dishEntity.RestaurantId = restaurantId;

            _dbContext.Dishes.Add(dishEntity);
            _dbContext.SaveChanges();

            return dishEntity.Id;
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dish = _dbContext
                .Dishes
                .FirstOrDefault(dish => dish.Id == dishId);

            if (dish is null || dish.RestaurantId != restaurantId)
                throw new NotFoundException("Dish not found");

            var dishDto = _mapper.Map<DishDto>(dish);
            return dishDto;
        }

        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dishes = restaurant.Dishes;
            var dishDtos = _mapper.Map<List<DishDto>>(dishes);
            return dishDtos;
        }

        public void Delete(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = _dbContext
                .Dishes
                .FirstOrDefault(d => d.Id == dishId);

            _dbContext.Dishes.Remove(dish);
            _dbContext.SaveChanges();
        }

        public void DeleteAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            _dbContext.Dishes.RemoveRange(restaurant.Dishes);
            _dbContext.SaveChanges();
        }

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == restaurantId);
            if (restaurant is null) throw new NotFoundException("Restaurant not found");
            return restaurant;
        }
    }
}

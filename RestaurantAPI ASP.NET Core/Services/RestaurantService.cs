﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI_ASP.NET_Core.Authorization;
using RestaurantAPI_ASP.NET_Core.Entities;
using RestaurantAPI_ASP.NET_Core.Exceptions;
using RestaurantAPI_ASP.NET_Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantAPI_ASP.NET_Core.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext  _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }
        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(restaurant => restaurant.Id == id);

            if (restaurant is null) throw new NotFoundException("Not found to get");
            return _mapper.Map<RestaurantDto>(restaurant);
        }
        public PageResult<RestaurantDto> GetAll(RestaurantQuery restaurantQuery)
        {

            var baseQuery = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => restaurantQuery.SearchPhase == null ||
                    (r.Name.ToLower().Contains(restaurantQuery.SearchPhase.ToLower()) || r.Description.ToLower().Contains(restaurantQuery.SearchPhase.ToLower())));

            if(string.IsNullOrEmpty(restaurantQuery.SortBy))
            {
                var columnSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    {nameof(Restaurant.Name), r => r.Name },
                    {nameof(Restaurant.Description), r => r.Description },
                    {nameof(Restaurant.Category), r => r.Category }
                };

                var selectedColumn = columnSelector[restaurantQuery.SortBy];

                baseQuery = restaurantQuery.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurants = baseQuery
                .Skip(restaurantQuery.ItemsPerPage * (restaurantQuery.PageNumber-1))
                .Take(restaurantQuery.ItemsPerPage)
                .ToList();

            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);
            var result = new PageResult<RestaurantDto>(restaurantsDtos, baseQuery.Count(), restaurantQuery.ItemsPerPage, restaurantQuery.PageNumber);
            return result;
        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }

        public void Delete(int id)
        {
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");

            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.Id == id);

            var authorizationresult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationresult.Succeeded)
            {
                throw new ForbidException("Forbidden");
            }

            if (restaurant == null) throw new NotFoundException("Not found to delete");

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        public void Update(UpdateRestaurantDto dto, int id)
        {

            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant == null) throw new NotFoundException("Not found to update");

            var authorizationresult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if(!authorizationresult.Succeeded)
            {
                throw new ForbidException("Forbidden");
            }

            if (dto.Name != null)
                restaurant.Name = dto.Name;
            
            if (dto.Description != null)
                restaurant.Description = dto.Description;
            
            if(dto.HasDelivery.HasValue)
                restaurant.HasDelivery = dto.HasDelivery.Value;

            _dbContext.SaveChanges();
        }
    }
}

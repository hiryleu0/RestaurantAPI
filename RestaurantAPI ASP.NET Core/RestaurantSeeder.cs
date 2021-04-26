using Microsoft.EntityFrameworkCore;
using RestaurantAPI_ASP.NET_Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI_ASP.NET_Core
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDbContext _context;
        public RestaurantSeeder(RestaurantDbContext context)
        {
            _context = context;
        }
        public void Seed()
        {
            if (_context.Database.CanConnect())
            {
                 var pendingMigrations = _context.Database.GetPendingMigrations();
                if(pendingMigrations != null && pendingMigrations.Any())
                {
                    _context.Database.Migrate();
                }

                if(!_context.Roles.Any())
                {
                    var roles = GetRoles();
                    _context.Roles.AddRange(roles);
                    _context.SaveChanges();
                }
                if (!_context.Restaurants.Any())
                {
                    var restaurants = GetRestaurants();
                    _context.Restaurants.AddRange(restaurants);
                    _context.SaveChanges();
                }
            }
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name="User"
                },
                new Role()
                {
                    Name="Manager"
                },
                new Role()
                {
                    Name="Admin"
                }
            };
            return roles;
        }

        private IEnumerable<Restaurant> GetRestaurants()
        {
            var restaurants = new List<Restaurant>()
            {
                new Restaurant()
                {
                    Name = "KFC",
                    Category = "Fast Food",
                    Description = "Not healthy food",
                    ContactEmail = "kfc@kfc.com",
                    ContactNumber = "55555",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name="Chicken",
                            Price = 10.50M
                        },
                        new Dish()
                        {
                            Name="Fries",
                            Price=5.90M
                        }
                    },
                    Address = new Address()
                    {
                        City="Warsaw",
                        Street="Akademicka 5",
                        PostalCode = "02-038"
                    }
                }
            };
            return restaurants;
        }
    }
}

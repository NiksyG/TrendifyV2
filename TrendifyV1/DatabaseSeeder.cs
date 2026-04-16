using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrendifyV1.Data.Entities;
using TrendifyV1.Data; 

namespace TrendifyV1.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedProductsAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<TrendifyV1DbContext>(); 


            var categoryNames = new[] { "Горнища", "Долнища", "Тениски", "Къси панталони", "Аксесоари" };
            var categories = new Dictionary<string, Category>();

            foreach (var name in categoryNames)
            {
                var category = await context.Categories.FirstOrDefaultAsync(c => c.Name == name);
                if (category == null)
                {
                    category = new Category { Name = name };
                    context.Categories.Add(category);
                    await context.SaveChangesAsync(); 
                }
                categories[name] = category;
            }

            var productsToSeed = new List<Product>
    {
        new Product {
            Name = "HYPER BLAZE STITCH HOODIE",
            Price = 66.47m,
            Description = "Размери: Ширина 58 см 66 см 61 см | Дължина 69 см 63 см 72 см.",
            ImageUrl = "https://hyperclothing.shop/cdn/shop/files/orange_HOODIE_F.png?v=1725204913&width=823",
            CategoryId = categories["Горнища"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 1 }, new ProductSize { Size = "M", Quantity = 2 }, new ProductSize { Size = "L", Quantity = 3 }
            }
        },
        new Product {
            Name = "HYPER EDITION PANDA BLACK HOODIE",
            Price = 71.58m,
            Description = "Размери: Ширина 58 см 66 см 61 см | Дължина 69 см 63 см 72 см.",
            ImageUrl = "https://hyperclothing.shop/cdn/shop/files/BLACK_HOODIE_F.png?v=1725194361&width=823",
            CategoryId = categories["Горнища"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 0 }, new ProductSize { Size = "M", Quantity = 0 }, new ProductSize { Size = "L", Quantity = 4 }
            }
        },
        new Product {
            Name = "BLACK KNIGHT HYPER HOODIE",
            Price = 71.58m,
            Description = "Размери: Ширина 58 см 66 см 61 см | Дължина 69 см 63 см 72 см.",
            ImageUrl = "https://hyperclothing.shop/cdn/shop/files/KON_BLACK_FRONT.png?v=1729001004&width=823",
            CategoryId = categories["Горнища"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 0 }, new ProductSize { Size = "M", Quantity = 0 }, new ProductSize { Size = "L", Quantity = 0 }
            }
        },

        new Product {
            Name = "HYPER EDITION PANDA BLACK SWEATPANTS",
            Price = 66.47m,
            Description = "Размери: Обиколка Талия 73 см 79 см 85 см | Дължина 107 см 109 см 111 см.",
            ImageUrl = "https://hyperclothing.shop/cdn/shop/files/BLACK_PANTS_F.png?v=1725194410&width=823",
            CategoryId = categories["Долнища"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 1 }, new ProductSize { Size = "M", Quantity = 1 }, new ProductSize { Size = "L", Quantity = 3 }
            }
        },
        new Product {
            Name = "HYPER BLAZE STITCH SWEATPANTS",
            Price = 61.35m,
            Description = "Размери: Обиколка Талия 73 см 79 см 85 см | Дължина 107 см 109 см 111 см.",
            ImageUrl = "https://hyperclothing.shop/cdn/shop/files/orange_pant_F.png?v=1725204993&width=823",
            CategoryId = categories["Долнища"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 1 }, new ProductSize { Size = "M", Quantity = 2 }, new ProductSize { Size = "L", Quantity = 3 }
            }
        },
        new Product {
            Name = "BLACK KNIGHT HYPER SWEATPANTS",
            Price = 66.47m,
            Description = "Размери: Обиколка Талия 73 см 79 см 85 см | Дължина 107 см 109 см 111 см.",
            ImageUrl = "https://hyperclothing.shop/cdn/shop/files/KON_BLACK_PANTS_FRONT.png?v=1729001004&width=823",
            CategoryId = categories["Долнища"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 0 }, new ProductSize { Size = "M", Quantity = 0 }, new ProductSize { Size = "L", Quantity = 0 }
            }
        },

        new Product {
            Name = "HYPER BLACK POCKET TEE",
            Price = 46.01m,
            Description = "Размери: Ширина 57 см 60 см 63 см | Дължина 63 см 65 см 67 см.",
            ImageUrl = "https://hyperclothing.shop/cdn/shop/files/BLACK_POCKET_TEE_FRONT_416f0702-8c17-437a-ab3f-e3d558d5cce8.png?v=1725192906&width=823",
            CategoryId = categories["Тениски"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 2 }, new ProductSize { Size = "M", Quantity = 0 }, new ProductSize { Size = "L", Quantity = 2 }
            }
        },
        new Product {
            Name = "HYPER WINNER BLACK TEE",
            Price = 40.90m,
            Description = "Размери: Ширина 57 см 60 см 63 см | Дължина 63 см 65 см 67 см.",
            ImageUrl = "https://hyperclothing.shop/cdn/shop/files/EJ25_ANGEL_BKTS_FLT_BACK.png?v=1738404801&width=823",
            CategoryId = categories["Тениски"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 0 }, new ProductSize { Size = "M", Quantity = 0 }, new ProductSize { Size = "L", Quantity = 0 }
            }
        },
        new Product {
            Name = "HYPER DARK KNIGHT TEE",
            Price = 46.01m,
            Description = "Размери: Ширина 57 см 60 см 63 см | Дължина 63 см 65 см 67 см.",
            ImageUrl = "https://hyperclothing.shop/cdn/shop/files/14_2.png?v=1744205219&width=823",
            CategoryId = categories["Тениски"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 5 }, new ProductSize { Size = "M", Quantity = 5 }, new ProductSize { Size = "L", Quantity = 5 }
            }
        },

        new Product {
            Name = "HYPER BLACK POCKET SHORTS",
            Price = 51.12m,
            Description = "Размери: Обиколка кръст 76 см 80 см 84 см | Дължина 48 см 49 см 50 см.",
            ImageUrl = "https://hyperclothing.shop/cdn/shop/files/BLACK_POCKET_SHORTS_FRONT_ad5cb593-0969-4244-92f5-22b55c3d3b87.png?v=1725192749&width=823",
            CategoryId = categories["Къси панталони"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 2 }, new ProductSize { Size = "M", Quantity = 0 }, new ProductSize { Size = "L", Quantity = 2 }
            }
        },
        new Product {
            Name = "HYPER STONE WASHED KNIGHT SHORTS",
            Price = 40.89m,
            Description = "Размери: Обиколка кръст 76 см 80 см 84 см | Дължина 48 см 49 см 50 см.",
            ImageUrl = "https://hyperclothing.shop/cdn/shop/files/WASHED_KNIGHT_SHORTS_FRONT.png?v=1725270865&width=823",
            CategoryId = categories["Къси панталони"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 5 }, new ProductSize { Size = "M", Quantity = 5 }, new ProductSize { Size = "L", Quantity = 5 }
            }
        },
        new Product {
            Name = "HYPER SKULL SHORTS",
            Price = 51.12m,
            Description = "Размери: Обиколка кръст 76 см 80 см 84 см | Дължина 48 см 49 см 50 см.",
            ImageUrl = "https://hyperclothing.shop/cdn/shop/files/SKULL_SHORTS_FRONT.png?v=1725268584&width=823",
            CategoryId = categories["Къси панталони"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 0 }, new ProductSize { Size = "M", Quantity = 0 }, new ProductSize { Size = "L", Quantity = 0 }
            }
        },

        new Product {
            Name = "XO ГЕРДАН",
            Price = 15.33m,
            Description = "Размери: 45 см 50 см 60 см.",
            ImageUrl = "https://vvs-shop.com/cdn/shop/files/S49b4d88f26eb465a8f8a14cdd8ef2dd0O_22.jpg?v=1721296787",
            CategoryId = categories["Аксесоари"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 5 }, new ProductSize { Size = "M", Quantity = 1 }, new ProductSize { Size = "L", Quantity = 3 }
            }
        },
        new Product {
            Name = "CACTUS JACK ГЕРДАН",
            Price = 15.33m,
            Description = "Размери: 45 см 50 см 60 см.",
            ImageUrl = "https://i.etsystatic.com/17005687/r/il/6e7f5b/2349688788/il_1588xN.2349688788_3hvb.jpg",
            CategoryId = categories["Аксесоари"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 1 }, new ProductSize { Size = "M", Quantity = 1 }, new ProductSize { Size = "L", Quantity = 0 }
            }
        },
        new Product {
            Name = "CROSS ГЕРДАН",
            Price = 15.33m,
            Description = "Размери: 45 см 50 см 60 см.",
            ImageUrl = "https://www.zalesoutlet.com/productimages/processed/V-20449973_0_800.jpg?pristine=true",
            CategoryId = categories["Аксесоари"].Id,
            ProductSizes = new List<ProductSize> {
                new ProductSize { Size = "S", Quantity = 3 }, new ProductSize { Size = "M", Quantity = 3 }, new ProductSize { Size = "L", Quantity = 3 }
            }
        }
    };

            foreach (var product in productsToSeed)
            {
                bool exists = await context.Products.AnyAsync(p => p.Name == product.Name);
                if (!exists)
                {
                    context.Products.Add(product);
                }
            }

            await context.SaveChangesAsync();
        }
    }

    }

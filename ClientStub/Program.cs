using ClientConsole.Model;
using ClientConsole.Persistence;
using Contracts.StubCommunicationClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace ClientConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //dotnet run --project GrpcServerStub
            //dotnet run --project ServerStub
            //^Запустит серверы

            Trace.AutoFlush = true;
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new ConsoleTraceListener());
            string logFileName = $"test-sms-console-app-{DateTime.UtcNow:yyyyMMdd}.log";
            Trace.Listeners.Add(new TextWriterTraceListener(logFileName));

            Trace.WriteLine("Hello, World!"); // <-- неприкосновенно

            try
            {
                #region БД
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseNpgsql(GetDbConnection());
                DbContextOptions<AppDbContext> options = optionsBuilder.Options;

                using var db = new AppDbContext(options);

                #endregion

                //Оба рабочие
                //using ICommunicationService comm = HttpServerStubCommunication.StubCommunication.GetCommunicationService(GetUserName(), GetUserPassword(), GetServerUri());
                using ICommunicationService comm = GrpcServerStubCommunication.StubCommunication.GetCommunicationService(GetServerUri(grpc: true));
                bool exit = false;
                while (!exit)
                {
                    try
                    {
                        Trace.WriteLine("1. Получить меню");
                        Trace.WriteLine("2. Загрузить меню с сервера");
                        Trace.WriteLine("3. Создать заказ");
                        Trace.WriteLine("4. Выход");

                        var action = Console.ReadLine();
                        Trace.Listeners[1].WriteLine("Пользователь ввел: " + action);

                        switch (action)
                        {
                            case "1":
                            case "2":
                                {
                                    var menu = await GetMenuOrPopulateDb(comm, db);

                                    foreach (var item in menu)
                                    {
                                        string price = item.Price == 0 ? "Нет цены" : item.Price.ToString();
                                        Trace.WriteLine($"{item.Name} – {item.Article} – {price}");
                                    }
                                    break;
                                }
                            case "3":
                                {
                                    try
                                    {
                                        var menu = await GetMenuOrPopulateDb(comm, db);

                                        var orderLine = Console.ReadLine();
                                        Trace.Listeners[1].WriteLine("Пользователь ввел: " + orderLine);

                                        var individualItems = orderLine!.Split(';');

                                        List<MenuItemQuantity> items = new(individualItems.Length);

                                        foreach (var item in individualItems)
                                        {
                                            if (string.IsNullOrEmpty(item))
                                            {
                                                continue;
                                            }

                                            var itemSplit = item.Split(':');

                                            var id = itemSplit.First();

                                            if (!menu.Any(x=> x.ServerId == Convert.ToInt32(id)))
                                            {
                                                throw new Exception($"Позиции меню id {id} нет в базе данных");
                                            }

                                            var quantity = Convert.ToDouble(itemSplit.Last());

                                            if(quantity <= 0)
                                            {
                                                throw new Exception($"В позиции меню {id} количество указано не больше нуля – так нельзя");
                                            }

                                            items.Add(new MenuItemQuantity() { Id = id, Quantity = quantity });
                                        }

                                        var order = new Contracts.StubCommunicationClasses.Order()
                                        {
                                            Id = Guid.NewGuid(),
                                            MenuItems = items
                                        };

                                        await comm.SendOrder(order);
                                        Trace.WriteLine("УСПЕХ");
                                    }
                                    catch (Exception ex)
                                    {
                                        Trace.WriteLine("Ошибка при попытке создания заказа: "+ ex.Message);
                                    }
                                    break;
                                }
                            case "4":
                                {
                                    exit = true;
                                    break;
                                }
                            default:
                                {
                                    Trace.WriteLine("Неизвестная команда");
                                    break;
                                }
                        }

                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                        if (ex.InnerException != null)
                        {
                            Trace.WriteLine(ex.InnerException.Message);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Произошла ошибка при инициализации: " + ex.Message);
            }
        }

        private static async Task<List<Model.MenuItem>> GetMenuOrPopulateDb(ICommunicationService service, AppDbContext db)
        {
            if (db.MenuItems.Any())
            {
                return await db.MenuItems.AsNoTracking().ToListAsync();
            }

            var menu = await service.GetMenu(true);
            var menuModel = menu
                .Select(x => new Model.MenuItem()
                {
                    ServerId = Convert.ToInt32(x.Id),
                    Article = x.Article,
                    FullPath = x.FullPath,
                    IsWeighted = x.IsWeighted,
                    Name = x.Name,
                    Price = x.Price,
                    Barcodes = [.. x.BarCodes.Select(x => new Barcode() { Code = x })]
                }).ToList();

            await db.MenuItems.AddRangeAsync(menuModel);
            db.SaveChanges();

            return await db.MenuItems.AsNoTracking().ToListAsync();
        }

        private static string? GetDbConnection()
        {
            var conf = GetConfig();
            return conf["Settings:DatabaseConnection"];
        }

        private static string GetServerUri(bool grpc = false)
        {
            var conf = GetConfig();

            if (grpc)
            {
                return conf["Settings:BaseUriGrpc"]!;
            }

            return conf["Settings:BaseUri"]!;
        }
        private static string GetUserName()
        {
            var conf = GetConfig();
            return conf["Settings:UserName"]!;
        }

        private static string GetUserPassword()
        {
            var conf = GetConfig();
            return conf["Settings:UserPassword"]!;
        }

        private static IConfigurationRoot GetConfig()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return configuration;
        }
    }
}

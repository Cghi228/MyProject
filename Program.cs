using Microsoft.EntityFrameworkCore;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public int Permission { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
}

public class AppDb : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;

    public AppDb() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder o)
        => o.UseSqlite("Data Source=shop.db");

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>().HasData(
            new User { Id = 1, Name = "Admin", Password = "12345", Permission = 3 },
            new User { Id = 2, Name = "Manager", Password = "111", Permission = 2 },
            new User { Id = 3, Name = "User1", Password = "222", Permission = 1 }
        );
        mb.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Хлеб", Count = 10, Price = 40 },
            new Product { Id = 2, Name = "Молоко", Count = 5, Price = 80 }
        );
    }
}

class Program
{
    static void Main()
    {
        using var db = new AppDb();

        User? user = null;
        while (user == null)
        {
            Console.Write("Логин: ");
            string? login = Console.ReadLine();
            Console.Write("Пароль: ");
            string? pass = Console.ReadLine();
            user = db.Users.FirstOrDefault(u => u.Name == login && u.Password == pass);
            if (user == null) Console.WriteLine("Неверно.\n");
        }
        Console.WriteLine($"\nПривет, {user.Name}!\n");

        while (true)
        {
            Console.WriteLine("\n1.Товары");
            if (user.Permission >= 2) Console.WriteLine("2.Добавить товар\n3.Изменить товар");
            if (user.Permission == 3) Console.WriteLine("4.Люди\n5.Добавить человека\n6.Изменить человека");
            Console.WriteLine("0.Выход");
            Console.Write("Выбор: ");
            string? v = Console.ReadLine();

            if (v == "1")
            {
                foreach (var p in db.Products.ToList())
                    Console.WriteLine($"{p.Id}.{p.Name} | {p.Count}шт | {p.Price}руб");
            }
            else if (v == "2" && user.Permission >= 2)
            {
                Console.Write("Имя: "); string? n = Console.ReadLine();
                Console.Write("Кол: "); int c = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Цена: "); decimal p = decimal.Parse(Console.ReadLine() ?? "0");
                db.Products.Add(new Product { Name = n, Count = c, Price = p });
                db.SaveChanges();
                Console.WriteLine("ОК");
            }
            else if (v == "3" && user.Permission >= 2)
            {
                foreach (var p in db.Products.ToList()) Console.WriteLine($"{p.Id}.{p.Name}");
                Console.Write("ID: "); int id = int.Parse(Console.ReadLine() ?? "0");
                var t = db.Products.FirstOrDefault(x => x.Id == id);
                if (t != null)
                {
                    Console.Write("Имя: "); t.Name = Console.ReadLine();
                    Console.Write("Кол: "); t.Count = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Цена: "); t.Price = decimal.Parse(Console.ReadLine() ?? "0");
                    db.SaveChanges();
                    Console.WriteLine("ОК");
                }
            }
            else if (v == "4" && user.Permission == 3)
            {
                foreach (var u in db.Users.ToList())
                    Console.WriteLine($"{u.Id}.{u.Name} | {u.Permission}");
            }
            else if (v == "5" && user.Permission == 3)
            {
                Console.Write("Имя: "); string? n = Console.ReadLine();
                Console.Write("Пароль: "); string? p = Console.ReadLine();
                Console.Write("Уровень: "); int l = int.Parse(Console.ReadLine() ?? "1");
                db.Users.Add(new User { Name = n, Password = p, Permission = l });
                db.SaveChanges();
                Console.WriteLine("ОК");
            }
            else if (v == "6" && user.Permission == 3)
            {
                foreach (var u in db.Users.ToList()) Console.WriteLine($"{u.Id}.{u.Name}");
                Console.Write("ID: "); int id = int.Parse(Console.ReadLine() ?? "0");
                var u2 = db.Users.FirstOrDefault(x => x.Id == id);
                if (u2 != null)
                {
                    Console.Write("Имя: "); u2.Name = Console.ReadLine();
                    Console.Write("Пароль: "); u2.Password = Console.ReadLine();
                    Console.Write("Уровень: "); u2.Permission = int.Parse(Console.ReadLine() ?? "1");
                    db.SaveChanges();
                    Console.WriteLine("ОК");
                }
            }
            else if (v == "0") break;
            else Console.WriteLine("Нельзя");
        }
    }
}

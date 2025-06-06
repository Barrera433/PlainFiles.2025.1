using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVSWithLibary;

public class UserHelper
{
    private const string UserFilePath = "users.txt";

    public List<User> ReadUsers()
    {
        if (!File.Exists(UserFilePath))
        {
            Console.WriteLine($"Advertencia: El Archivo {UserFilePath} no existe.");
            var exampleUsers = new List<User>();
            {
                new User { Username = "admin", Password = "adminPassword!", IsActive = true };
                new User { Username = "user1", Password = "user1Password!", IsActive = true };
                new User { Username = "blocked", Password = "BlockedPass!", IsActive = true };

            };
            WriteUsers(exampleUsers);
            return exampleUsers;
        }

        var users = new List<User>();
        try
        {
            using (var reader = new StreamReader(UserFilePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false, Delimiter = ","
            }))
            {
                csv.Context.RegisterClassMap<UserMap>();
                users = csv.GetRecords<User>().ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al leer el  usuario: {UserFilePath}: {ex.Message}");
        }
        return users;

    }

    public void WriteUsers(List<User> users)
    {
        try
        {
            using (var writer = new StreamWriter(UserFilePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            { 
                HasHeaderRecord = false, Delimiter = "," 
            }))
            {
                csv.Context.RegisterClassMap<UserMap>();
                csv.WriteRecords(users);
            }
            Console.WriteLine($"Usuarios guardados correctamente en {UserFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al escribir el  usuario: {UserFilePath} {ex.Message}");
        }
    }
}
public sealed class UserMap : ClassMap<User>
{
    public UserMap()
    {
        Map(m => m.Username).Index(0);
        Map(m => m.Password).Index(1);
        Map(m => m.IsActive).Index(2);
        Map(m => m.FailedLoginAttempts).Index(3);
        Map(m => m.FailedLoginAttempts).Ignore(); // Ignorar FailedLoginAttempts al escribir
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace CVSWithLibary;

public class AppManager
{
    private readonly CsvHelperExample? _csvpeopleHelper;
    private List<Person>? _people;
    private readonly User? _authenticatedUser; // usuario que inicia la seccion

    public AppManager(User authenticatedUser)
    {
        _csvpeopleHelper = new CsvHelperExample();
        _people = _csvpeopleHelper.Read("people.csv").ToList();
        _authenticatedUser = authenticatedUser;
    }

    public void Showpeople()
    {
        LogActivity("Mostrar Personas", "Visualizando la lista de personas.");
        if (_people!.Any())
        {
            Console.WriteLine("no hay personas para mostrar.");
            return;

        }
        foreach (var person in _people!)
        {
            Console.WriteLine(person);
        }
    }

    public void AddPerson()
    {
        Console.WriteLine("--- Añadir Nueva Persona ");
        int id;
        bool idExists;
        do
        {
            Console.Write("Enter the ID: ");
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("ID inválido, ingrese número entero.");
                idExists = true;
                continue;
            }
            idExists = _people.Any(p => p.Id == id);
            if (idExists)
            {
                Console.WriteLine($"Error: Ya existe una persona con el ID '{id}'.ingrese un único id.");

            }
        } while (idExists);


        string firstName; // nombres y apellidos no vacios 
        do
        {
            Console.Write("Enter the first name: ");
            firstName = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(firstName))
            {
                Console.WriteLine("El nombre no puede estar vacío.");

            }
        } while (string.IsNullOrWhiteSpace(firstName));

        string lastName;
        do
        {
            Console.Write("Enter the last name: ");
            lastName = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(lastName))
            {
                Console.WriteLine("El apellido no puede estar vacío.");

            }
        } while (string.IsNullOrWhiteSpace(lastName));

        string phone; // validación telefono
        do
        {
            Console.Write("Enter the phone:");
            phone = Console.ReadLine() ?? string.Empty; // validacion de 10 digitos 

            if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^[0-9]{10,}$"))
            {
                Console.WriteLine("Teléfono inavalido. ingrese nuevamente.");
            }
        } while (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^[0-9]{10,}$"));


        Console.Write("Enter the city: ");
        var city = Console.ReadLine() ?? string.Empty;

        decimal balance;
        do
        {
            Console.Write("Enter the balance: ");
            if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out balance)
                || balance < 0)
            {
                Console.WriteLine("Balance inválido.intente de nuevo.");

            }
        } while (balance < 0);


        var newperson = new Person
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            City = city,
            Balance = balance

        };

        _people.Add(newperson);

        Console.WriteLine("Person añadida exitosamente.");
        LogActivity("Añadir persona", $"Se añadio una nueva persona: ID {newperson.Id}," +
            $" Nombre : {newperson.FirstName}" +
            $"{newperson.LastName}");

    }

    public void EditPerson()
    {
        Console.WriteLine("--- Editar Persona ---");
        Console.Write("Ingrese el ID de la persona a editar: ");
        if (!int.TryParse(Console.ReadLine(), out int idToEdit))
        {
            Console.WriteLine("ID inválido, ingrese un número entero.");
            LogActivity("Editar Persona", $"ID inválido proporcionado. ´{idToEdit}´");
            return;
        }
        var personToEdit = _people.FirstOrDefault(p => p.Id == idToEdit);

        if (personToEdit == null)
        {
            Console.WriteLine($"No se encontró una persona con el ID {idToEdit}.");
            LogActivity("Editar Persona", $"No se encontró una persona con el ID {idToEdit}.");
            return;
        }

        Console.WriteLine($"Editando persona: {personToEdit}");
        Console.WriteLine("Ingrese el nuevo nombre (dejar en blanco para no cambiar): ");


        // Guardar Valores originales 
        String originalFirstName = personToEdit.FirstName;
        string originalLastName = personToEdit.LastName;
        string originalPhone = personToEdit.Phone;
        string originalCity = personToEdit.City;
        decimal originalBalance = personToEdit.Balance;

        Console.Write($"Primer nombre({personToEdit.FirstName}): ");
        String? newFirstName = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(newFirstName))
        {
            personToEdit.FirstName = newFirstName;
        }
        Console.Write($"Apellido({personToEdit.LastName}): ");
        String? newLastName = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(newLastName))
        {
            personToEdit.LastName = newLastName;
        }
        Console.Write($"Teléfono({personToEdit.Phone}): ");
        String? newPhone = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(newPhone) && Regex.IsMatch(newPhone, @"^[0-9]{10,}$"))
        {
            personToEdit.Phone = newPhone;
        }
        else if (!string.IsNullOrWhiteSpace(newPhone))
        {
            Console.WriteLine("Teléfono inválido. Debe contener al menos 10 dígitos.");
            LogActivity("Editar Persona", $"Teléfono inválido proporcionado: {newPhone}");
        }

        Console.Write($"Ciudad({personToEdit.City}): ");
        String? newCity = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(newCity))
        {
            personToEdit.City = newCity;
        }

        Console.Write($"Balance({personToEdit.Balance:C2}): ");
        String? newBalanceStr = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(newBalanceStr))
        {
            if (decimal.TryParse(newBalanceStr, NumberStyles.Any, CultureInfo.InvariantCulture,
            out decimal newBalance) && newBalance >= 0)
            {
                personToEdit.Balance = newBalance;
            }
            else
            {
                Console.WriteLine("Balance inválido. Debe ser un número no negativo.");
                LogActivity("Editar Persona", $"Balance inválido proporcionado: {newBalanceStr}");
            }
        }

        Console.WriteLine("Persona editada exitosamente.");
        LogActivity("Editar Persona",
            $"Se editó la persona con ID {idToEdit}." +
            $" Cambios: " +
            $"Nombre: {originalFirstName} -> {personToEdit.FirstName}, " +
            $"Apellido: {originalLastName} -> {personToEdit.LastName}, " +
            $"Teléfono: {originalPhone} -> {personToEdit.Phone}, " +
            $"Ciudad: {originalCity} -> {personToEdit.City}, " +
            $"Balance: {originalBalance:C2} -> {personToEdit.Balance:C2}");

    }
    public void DeletePerson()
    {
        Console.WriteLine("--- Eliminar Persona ---");
        Console.Write("Ingrese el ID de la persona a eliminar: ");
        if (!int.TryParse(Console.ReadLine(), out int idToDelete))
        {
            Console.WriteLine("ID inválido, ingrese un número entero.");
            LogActivity("Eliminar Persona", $"ID inválido proporcionado: {idToDelete}");
            return;
        }

        var personToDelete = _people.FirstOrDefault(p => p.Id == idToDelete);

        if (personToDelete == null)
        {
            Console.WriteLine($"No se encontró una persona con el ID {idToDelete}.");
            LogActivity("Eliminar Persona", $"No se encontró una persona con el ID {idToDelete}.");
            return;
        }

        Console.WriteLine("Persona encontra: ");
        Console.WriteLine(personToDelete);

        Console.Write("¿Está seguro de que desea eliminar esta persona? (S/N): ");
        string confirmation = Console.ReadLine()!.ToLower();

        if (confirmation == "s" || confirmation == "si")
        {
            _people.Remove(personToDelete);
            Console.WriteLine("Persona eliminada exitosamente.");
            LogActivity("Eliminar Persona", $"Se eliminó la persona con ID {personToDelete.Id}" +
                $"borrada. Nombre: {personToDelete.FirstName}{personToDelete.LastName} ");
        }
        else
        {
            Console.WriteLine("Operación cancelada. No se eliminó ninguna persona.");
            LogActivity("Eliminar Persona", $"Operación cancelada para el ID {idToDelete}.");
        }

    }

    public void GenerateReport()
    {
        LogActivity("Generar Reporte", "Generando reporte por ciudad.");
        Console.WriteLine("--- Reporte por Ciudad ---");

        if (!_people!.Any())
        {
            Console.WriteLine("No hay personas para generar un reporte.");
            return;
        }
        var groupedByCity = _people.GroupBy(p => p.City ?? "sin ciudad").OrderBy(g => g.Key);

        decimal totalGeneral = 0;

        foreach (var cityGroup in groupedByCity)
        {
            Console.WriteLine($"Ciudad: {cityGroup.Key}");
            Console.WriteLine("ID\tNombre\t\tTeléfono\tBalance");
            Console.WriteLine("--------------------------------------------------");

            decimal totalCityBalance = 0;
            foreach (var person in cityGroup.OrderBy(p => p.LastName).ThenBy(p => p.FirstName))

            {
                Console.WriteLine($"{person.Id}\t{person.FirstName} {person.LastName}\t{person.Phone}\t{person.Balance,10:C2}");
                totalCityBalance += person.Balance;
            }

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Total en {cityGroup.Key,-20}{totalCityBalance,10:C2}");
            totalGeneral += totalCityBalance;
        }

        Console.WriteLine("==================================================");
        Console.WriteLine($"Total General  {totalGeneral,23:C2}");
        Console.WriteLine("================================================");
    }

    public void SaveChanges()
    {
        _csvpeopleHelper!.Write("people.csv", _people!);
        Console.WriteLine("Cambios guardados exitosamente en people.csv.");
        LogActivity("Guardar Cambios", "Guardando cambios en el archivo CSV.");
    }
    private void LogActivity(string activivityType, string message) // Método para registrar actividades log
    {
        string LogFilepath = "log.txt";
        string Username = _authenticatedUser!.Username;
        try
        {
            File.AppendAllText(LogFilepath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} -" +
                $" {activivityType} - {message} - User: {Username}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al registrar la actividad: {ex.Message}");
        }
    }
}

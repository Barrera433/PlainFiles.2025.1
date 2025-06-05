using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CVSWithLibary;

public class AppManeger
{
    private readonly CsvHelperExample? _csvpeopleHelper;
    private List<Person>? _people;
    private readonly User? _authenticatedUser; // usuario que inicia la seccion

    public AppManeger(User authenticatedUser)
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
            Console.Write("Enter the ID");
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

    }

















    private void LogActivity(string v1, string v2)
    {
        throw new NotImplementedException();
    }
}

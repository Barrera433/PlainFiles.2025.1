using CVSWithLibary;
using System.Linq; // Asegúrate de tener esto

// Instancias de los helpers
var userHelper = new UserHelper();

// Carga inicial de usuarios
var users = userHelper.ReadUsers();

// Variable para el usuario autenticado
User? authenticatedUser = null;

// --- Lógica de Autenticación al inicio del programa ---
Console.WriteLine("=======================================");
Console.WriteLine("        BIENVENIDO AL SISTEMA        ");
Console.WriteLine("=======================================");

bool loggedIn = false;
while (!loggedIn)
{
    Console.Write("Usuario: ");
    string usernameInput = Console.ReadLine() ?? string.Empty;

    Console.Write("Contraseña: ");
    string passwordInput = ReadPassword(); // Función para leer contraseña sin mostrar

    User? user = users.FirstOrDefault(u => u.Username.Equals(usernameInput, StringComparison.OrdinalIgnoreCase));

    if (user == null)
    {
        Console.WriteLine("Usuario no encontrado.");
        // Usamos la función auxiliar estática para loguear antes de tener un usuario autenticado
        LogActivityExternal("Intento de Login Fallido", $"Usuario '{usernameInput}' no encontrado.", null);
    }
    else if (!user.IsActive)
    {
        Console.WriteLine("Usuario bloqueado. Contacte al administrador.");
        LogActivityExternal("Intento de Login Fallido", $"Usuario '{usernameInput}' bloqueado.", user);
    }
    else if (user.Password == passwordInput) // En un sistema real, usar hashing de contraseñas
    {
        Console.WriteLine($"¡Bienvenido, {user.Username}!");
        authenticatedUser = user;
        user.FailedLoginAttempts = 0; // Reinicia intentos fallidos al loguearse
        userHelper.WriteUsers(users); // Guarda el estado de los usuarios (reiniciar intentos)
        LogActivityExternal("Login Exitoso", $"Usuario '{user.Username}' ha iniciado sesión.", user);
        loggedIn = true;
    }
    else
    {
        user.FailedLoginAttempts++;
        LogActivityExternal("Intento de Login Fallido", $"Contraseña incorrecta para usuario '{user.Username}'. Intento {user.FailedLoginAttempts}", user);

        if (user.FailedLoginAttempts >= 3)
        {
            user.IsActive = false;
            Console.WriteLine($"Demasiados intentos fallidos. El usuario '{user.Username}' ha sido bloqueado.");
            LogActivityExternal("Usuario Bloqueado", $"Usuario '{user.Username}' bloqueado después de 3 intentos fallidos.", user);
        }
        else
        {
            Console.WriteLine($"Contraseña incorrecta. Te quedan {3 - user.FailedLoginAttempts} intentos.");
        }
        userHelper.WriteUsers(users); // Guarda el estado de los usuarios (intentos fallidos/bloqueo)
    }

    if (!loggedIn)
    {
        Console.WriteLine("\nPresiona cualquier tecla para reintentar...");
        Console.ReadKey();
        Console.Clear();
    }
}

Console.Clear(); // Limpia la consola después del login exitoso

// --- Instancia de AppManager para manejar las operaciones ---
// Ahora se le pasa el usuario autenticado
var appManager = new AppManager(authenticatedUser);

// --- Lógica del menú principal ---
var opc = "0";
do
{
    opc = Menu();
    Console.WriteLine("=======================================");
    switch (opc)
    {
        case "1":
            appManager.Showpeople();
            break;

        case "2":
            appManager.AddPerson();
            break;

        case "3":
            appManager.EditPerson();
            break;

        case "4":
            appManager.DeletePerson();
            break;

        case "5":
            appManager.GenerateReport();
            break;

        case "S":
        case "s":
            appManager.SaveChanges();
            break;

        case "0":
            Console.WriteLine("Saliendo del programa...");
            break;

        default:
            Console.WriteLine("Opción inválida. Por favor, elige una opción del menú.");
            break;
    }
    Console.WriteLine("\nPresiona cualquier tecla para continuar...");
    Console.ReadKey();
    Console.Clear();
} while (opc != "0");

// Guarda los cambios finales al salir
appManager.SaveChanges();
LogActivityExternal("Cierre de Sesión", $"Usuario '{authenticatedUser?.Username}' ha cerrado sesión.", authenticatedUser);


// --- Funciones auxiliares de Program.cs ---

// Esta función de log es específica para el Program.cs y para casos donde no hay un AppManager instanciado
// o un usuario logueado todavía. Es una versión "raw" y estática para casos tempranos.
static void LogActivityExternal(string activityType, string message, User? currentUser)
{
    string logFilePath = "log.txt";
    string username = currentUser?.Username ?? "N/A (No logueado)"; // Si no hay usuario logueado, muestra N/A

    try
    {
        File.AppendAllText(logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {activityType} - {message} - User: {username}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al escribir en el log (desde Program.cs): {ex.Message}");
    }
}


string ReadPassword()
{
    string password = "";
    ConsoleKeyInfo key;
    do
    {
        key = Console.ReadKey(true);
        if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
        {
            password += key.KeyChar;
            Console.Write("*");
        }
        else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
        {
            password = password.Remove(password.Length - 1);
            Console.Write("\b \b");
        }
    } while (key.Key != ConsoleKey.Enter);
    Console.WriteLine();
    return password;
}

string Menu()
{
    Console.WriteLine("=======================================");
    Console.WriteLine("1. Mostrar contenido");
    Console.WriteLine("2. Añadir persona");
    Console.WriteLine("3. Editar persona");
    Console.WriteLine("4. Borrar persona");
    Console.WriteLine("5. Generar informe");
    Console.WriteLine("S. Guardar cambios (explícito)");
    Console.WriteLine("0. Salir");
    Console.Write("Elige una opción: ");
    return Console.ReadLine() ?? "0";
}
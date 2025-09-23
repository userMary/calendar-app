// See https://aka.ms/new-console-template for more information
using System;
using Microsoft.AspNetCore.Identity;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Введите пароль для админа: ");
        string password = Console.ReadLine();

        var hasher = new PasswordHasher<object>();
        // объект нам не важен, поэтому передаем null
        string hash = hasher.HashPassword(null, password);

        Console.WriteLine("\nХэш пароля:");
        Console.WriteLine(hash);
    }
}


using JORM;
using Npgsql;

var source = new Source(() => null!);//new NpgsqlConnection(null));

int i = source.ExecuteNonQuery($"SELECT * FROM table WHERE id = {147:id}");













// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

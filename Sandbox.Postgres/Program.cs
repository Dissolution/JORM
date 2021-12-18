using System.Data.Common;
using JORM;
using Npgsql;

var source = new Source(() => null!);//new NpgsqlConnection(null));

int id = 147;
string name = "Jesus";

//int i = source.ExecuteNonQuery($"SELECT * FROM table WHERE id = {id} AND name = {name:NAM}");


var tuple = InterpolatedSqlHandler.Parse($"SELECT * FROM table WHERE id = {id} AND name = {name:NAM}");

// var factory = NpgsqlFactory.Instance;
//
// ISourceBuilder builder = default!;
// builder.Provider(NpgsqlFactory.Instance)
//        .Connection(f => (f.CreateConnection() as NpgsqlConnection)!)
//        .Transaction(cmd => cmd.BeginTransaction())
//        .Command(conn => conn.CreateCommand())
//        .Parameter(cmd => cmd.CreateParameter());
//
//
// NpgsqlConnection conn = default!;
// conn.begintra





// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

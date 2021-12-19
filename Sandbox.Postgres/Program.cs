using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using JORM;
using JORM.Text;
using Npgsql;
using TextWriter = JORM.Text.TextWriter;
using TextReader = JORM.Text.TextReader;


string str = "select * from table where id = {0} and name = {1:NAM}";
var reader = new TextReader(str);

//var pre = reader.TakeUntil(rem => rem.Length > 0 && rem[0] == '{' && rem.Length > 2 && rem[1] != '{');
//var hole = reader.TakeUntil(rem => rem.Length > 0 && rem[0] == '}' && (rem.Length == 1 || rem[1] != '}'));


Debugger.Break();

FormattableString fstr = $"select * from table where id = {0} and name = {1:NAM}";
Test(ref fstr);

string.Create(null, ref (DefaultInterpolatedStringHandler)fstr);


static void Test(ref DefaultInterpolatedStringHandler handler)
{
    throw new NotImplementedException();
}
//
// var source = new Source(() => null!);//new NpgsqlConnection(null));
//
// int id = 147;
// string name = "Jesus";
//
// //int i = source.ExecuteNonQuery($"SELECT * FROM table WHERE id = {id} AND name = {name:NAM}");
//
//
// var tuple = InterpolatedSqlHandler.Parse($"SELECT * FROM table WHERE id = {id} AND name = {name:NAM}");
//
// // var factory = NpgsqlFactory.Instance;
// //
// // ISourceBuilder builder = default!;
// // builder.Provider(NpgsqlFactory.Instance)
// //        .Connection(f => (f.CreateConnection() as NpgsqlConnection)!)
// //        .Transaction(cmd => cmd.BeginTransaction())
// //        .Command(conn => conn.CreateCommand())
// //        .Parameter(cmd => cmd.CreateParameter());
// //
// //
// // NpgsqlConnection conn = default!;
// // conn.begintra





// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

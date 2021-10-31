
using System.Diagnostics;
using JORM.Querying.SqlBuilding;

string statement = Clause.Process("SELECT * FROM entity WHERE id >= {0} AND id <= {1}", 1, 2);
//FormattableString format = $"SELECT * FROM entity WHERE id >= {1} AND id <= {2}";
//string statement = Clause.Process(format.Format, format.GetArguments());

Console.WriteLine(statement);

Debugger.Break();

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Press Enter to close.");
Console.ReadLine();

using System.Diagnostics;
using JORM.Querying.SqlBuilding;

var statement = Testing.Statement.Parse(@"
SELECT *    
FROM entity    
WHERE id >= {0}  AND  id <=  {1}   ", 2);

Console.WriteLine(statement);

Debugger.Break();

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Press Enter to close.");
Console.ReadLine();

using System.Diagnostics;
using ConsoleSandbox;
using JORM.Querying.Builder;
using JORM.Querying.SqlBuilding;
//
// var statement = Testing.Statement.Parse(@"
// SELECT *    
// FROM entity    
// WHERE id >= {0}  AND  id <=  {1}   ", 2);

var entity = new TestEntity();

var builder = Activator.CreateInstance<IPreFromSelectBuilder<ISelectQueryBuilder>>();
builder.From("table");
builder.From("table t");
builder.From("table", "t");
builder.From($"{entity}", "e");
builder.From(entity.GetType(), "e");
builder.From(() => entity, "e");
builder.From<TestEntity>("e")
    .Select(e => e.Id)
    .Select(e => e.Name)
    .Where(b => b.)


//Console.WriteLine(statement);

Debugger.Break();

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Press Enter to close.");
Console.ReadLine();
// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Diagnostics;
// using System.Linq;
// using System.Text;
//
// namespace JORM.Querying.SqlBuilding
// {
//     public class Testing
//     {
//         public enum ClauseType
//         {
//             Statement,
//             And,
//             Or,
//         }
//
//         public interface IRenderable
//         {
//             void Render(Source source, StringBuilder text);
//         }
//
//         public interface IDbCommandBuilder
//         {
//             StringBuilder Sql { get; }
//             IDbCommandBuilder AddParameter(string? name, object? value);
//         }
//
//         public interface IBCommandAddon
//         {
//             void Apply(IDbCommandBuilder dbCommand);
//         }
//
//         public abstract class Clause : IRenderable, IBCommandAddon
//         {
//             public ClauseType Type { get; }
//
//             protected Clause(ClauseType type)
//             {
//                 this.Type = type;
//             }
//
//             public abstract void Render(Source source, StringBuilder text);
//
//             public abstract void Apply(IDbCommandBuilder dbCommand);
//         }
//
//         public class Statement : Clause
//         {
//             public static Statement Parse(string sql, int argsCount)
//             {
//                 var text = new StringBuilder(1024);
//                 var argHoles = new List<(int, int)>();
//                 text statement = sql;
//                 statement.Trim();
//                 var reader = new TextReader(statement);
//                 // Skip all leading whitespace
//                 reader.SkipWhiteSpace();
//                 // Process
//                 bool prevWS = false;
//                 while (reader.CanRead)
//                 {
//                     var ch = reader.Character;
//                     if (char.IsWhiteSpace(ch))
//                     {
//                         if (!prevWS)
//                         {
//                             text.Append(' ');
//                             prevWS = true;
//                         }
//                         reader.MoveNext();
//                         continue;
//                     }
//                     prevWS = false;
//                     if (ch == '{')
//                     {
//                         if (!reader.TryPeek(out char nextChar) ||
//                             (nextChar != '{' && !char.IsDigit(nextChar)))
//                         {
//                             throw new SqlBuildException(statement, reader.Index, "Invalid argument hole start");
//                         }
//
//                         if (nextChar == '{')
//                         {
//                             text.Append('{');
//                             reader.MoveNext();
//                             continue;
//                         }
//
//                         reader.MoveNext();
//                         var digits = reader.TakeDigits();
//                         if (!reader.CanRead || reader.Character != '}')
//                         {
//                             throw new SqlBuildException(statement, reader.Index, "Invalid argument hole end");
//                         }
//
//                         int argHoleIndex = text.Length;
//                         int argIndex = int.Parse(digits);
//                         // TODO: Verify valid index
//                         argHoles.Add((argHoleIndex, argIndex));
//                         reader.MoveNext();
//                         continue;
//                     }
//
//                     if (ch == '}')
//                     {
//                         if (!reader.TryPeek(out var nextChar) ||
//                             nextChar != '}')
//                         {
//                             throw new SqlBuildException(statement, reader.Index, "Invalid argument hold end");
//                         }
//                     }
//
//                     text.Append(ch);
//                     reader.MoveNext();
//                 }
//
//                 return new Statement(text.ToString(), argHoles.ToArray());
//             }
//
//             internal string _sql;
//             internal (int Index, int ArgIndex)[] _argIndices;
//
//             private Statement(string sql, (int Index, int ArgIndex)[] argIndices)
//                 : base(ClauseType.Statement)
//             {
//                 _sql = sql;
//                 _argIndices = argIndices;
//             }
//
//             public override void Render(Source source, StringBuilder text)
//             {
//                 throw new NotImplementedException();
//             }
//
//             public override void Apply(IDbCommandBuilder dbCommand)
//             {
//                 throw new NotImplementedException();
//             }
//
//             public override string ToString()
//             {
//                 var text = new StringBuilder(_sql);
//                 foreach (var ai in _argIndices.Reverse())
//                 {
//                     text.Insert(ai.Index, '{' + ai.ArgIndex.ToString() + '}');
//                 }
//
//                 return text.ToString();
//             }
//         }
//
//         public class StatementClause : Clause
//         {
//             public Statement Statement { get; }
//             public object?[] Args { get; }
//
//             public StatementClause(string statement, object?[] args)
//                 : base(ClauseType.Statement)
//             {
//                 this.Args = args;
//                 this.Statement = Statement.Parse(statement, args.Length);
//             }
//
//             public override void Render(Source source, StringBuilder text)
//             {
//                 throw new NotImplementedException();
//             }
//
//             public override void Apply(IDbCommandBuilder dbCommand)
//             {
//                 throw new NotImplementedException();
//             }
//         }
//     }
//    
//
//     public class ClauseChain
//     {
//         /* Idea:
//          * public Clause
//          * {
//          * public enum ClauseType
//          * public List<Clause> Clauses
//          *
//          * public static Clause And(params clause[])
//          * public static Clause Or(params clause[])
//          * }
//          
//          */
//     }
//
//
//
// }
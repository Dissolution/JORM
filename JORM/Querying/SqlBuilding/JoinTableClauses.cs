using System.Collections.Generic;

namespace JORM.Querying.SqlBuilding
{
    internal class JoinTableClauses
    {
        public JoinType Join { get; }
        public Table Table { get; }
        public List<Clause> Clauses { get; }

        public JoinTableClauses(JoinType join, Table table)
        {
            this.Join = join;
            this.Table = table;
            this.Clauses = new List<Clause>(0);
        }
    }
}
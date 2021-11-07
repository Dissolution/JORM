namespace JORM.Querying.SqlBuilding
{
    public class Column
    {
        public Table Table { get; set; }
        public string Name { get; set; }

        public Column(Table table, string name)
        {
            this.Table = table;
            this.Name = name;
        }

        public Column(Table table, text name)
        {
            this.Table = table;
            this.Name = new string(name);
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Table.Alias))
            {
                return $"{Table.Name}.{Name}";
            }
            else
            {
                return $"{Table.Alias}.{Name}";
            }
        }
    }
}
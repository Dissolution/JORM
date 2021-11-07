namespace JORM.Querying.SqlBuilding;

public class TableRef
{
    public string Name { get; }
    public string? Alias { get; set; }

    public TableRef(string name, string? alias = null)
    {
        this.Name = name;
        this.Alias = alias;
    }

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(Alias))
            return Name;
        return $"{Name} {Alias}";
    }
}
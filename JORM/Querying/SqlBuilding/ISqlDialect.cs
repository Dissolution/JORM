namespace JORM.Querying.SqlBuilding
{
    public interface ISqlDialect
    {
        bool CaseSensitive { get; set; }
    }
}
namespace JORM.Querying.SqlBuilding
{
    public interface IInitialSqlBuilder<TBuilder>
        where TBuilder : ISqlBuilder<TBuilder>
    {
        TBuilder From(string tableName, string? alias = null);
        ITypeSqlBuilder<T> From<T>(string? alias = null);
    }
}
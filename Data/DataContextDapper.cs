using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

class DataContextDapper (IConfiguration config)
{
    private readonly IConfiguration _config = config;
    
    public IEnumerable<T> Query<T>(string query, object? parameters = null)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.Query<T>(query, parameters);
    }

    public T? QuerySingle<T>(string query, object? parameters = null)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.QuerySingleOrDefault<T>(query, parameters);
    }

    public bool Execute(string query, object? parameters = null)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.Execute(query, parameters) > 0;
    }
}
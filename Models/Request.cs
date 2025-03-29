namespace Travidia.Models;

public class Request 
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int RequestStepId { get; set; }
    public bool? Response { get; set; }
    public int DepartmentId { get; set; }
    public int RolId { get; set; }
    public int UserId { get; set; }
    
    public void ResponseRequest(int id, bool response)
    {
        try
        {
            
        }
        catch (Exception)
        {
            throw;
        }
    }
}
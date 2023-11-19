namespace PZ24.Clients;

public class Status
{
    public int StatusID { get; set; }
    public string StatusName { get; set; }
    
    public override string ToString()
    {
        return StatusName;
    }
}
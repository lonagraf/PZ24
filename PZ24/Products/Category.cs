namespace PZ24.Products;

public class Category
{
    public int CategoryID { get; set; }
    public string Name { get; set; }
    
    public override string ToString()
    {
        return Name;
    }
}
namespace Victor.Commons.Models;

public class BaseModel : IEntityGuid
{
    public Guid Id { get; protected set; } =  Guid.NewGuid();
    
    public DateTime LastUpdateTime { get; private set; }
    
    public string LastUpdateBy { get; private set;}
    
    public void SetLastUpdateTime()
    {
        LastUpdateTime = DateTime.UtcNow;
    }
    
    public void SetLastUpdateBy(string lastUpdateBy)
    {
        LastUpdateBy = lastUpdateBy;
    }
}
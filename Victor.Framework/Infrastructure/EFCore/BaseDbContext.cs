using Microsoft.EntityFrameworkCore;
using Victor.Commons.Models;

namespace Victor.Framework.Infrastructure.EFCore;

public abstract class BaseDbContext : DbContext
{
    protected void SetBaseEntityCommon<TEntity>(TEntity entity) where TEntity : BaseModel
    {
        entity.SetLastUpdateBy("");
        entity.SetLastUpdateTime();
    }
}
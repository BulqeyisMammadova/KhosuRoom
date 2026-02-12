namespace KhosuRoom.Core.Entities.Common;

public abstract class BaseAutitableEntity: BaseEntity
{
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public string CreateBy { get; set; } = string.Empty;

    public DateTime? UpdateDate { get; set; }
    public string? UpdateBy { get; set; }

    public DateTime? DeleteDate { get; set; }
    public string? DeleteBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

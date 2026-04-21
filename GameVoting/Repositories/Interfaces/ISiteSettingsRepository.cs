using GameVoting.Models.Entities;

namespace GameVoting.Repositories.Interfaces;

public interface ISiteSettingsRepository
{
    SiteSettings Get();
    void Update(SiteSettings settings);
}

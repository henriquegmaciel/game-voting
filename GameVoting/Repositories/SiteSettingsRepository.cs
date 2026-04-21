using GameVoting.Data;
using GameVoting.Models.Entities;
using GameVoting.Repositories.Interfaces;

namespace GameVoting.Repositories;

public class SiteSettingsRepository : ISiteSettingsRepository
{
    private readonly AppDbContext _context;

    public SiteSettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public SiteSettings Get()
    {
        SiteSettings? settings = _context.SiteSettings.FirstOrDefault();
        if (settings is null)
        {
            settings = new SiteSettings();
            _context.Add(settings);
            _context.SaveChanges();
        }

        return settings;
    }

    public void Update(SiteSettings settings)
    {
        _context.SiteSettings.Update(settings);
        _context.SaveChanges();
    }
}

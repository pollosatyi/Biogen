using Biogen.Common.Entities;
using Biogen.Dal.Repository.Contracts;

namespace Biogen.DAl.Repository;

public class Repository : IRepository
{
    private readonly Context _context;

    public Repository(Context context)
    {
        _context = context;
    }

    public async Task<bool> SaveDetectImage(ImageDetectionOutcome image)
    {
        try
        {
            await _context.ImageDetectionOutcomes.AddAsync(image);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

        
    }
}
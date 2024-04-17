using SpookifyApi.Models;
using SpookifyApi.Repositories;
using System.ComponentModel.DataAnnotations;

namespace SpookifyApi.Services;

public class StatService
{
    private readonly IStatRepository _statRepository;
    
    public StatService(IStatRepository statRepository)
    {
        _statRepository = statRepository;
    }

    // Check if statModel is valid
    private void ValidateSongModel(StatModel statModel, string errorString)
    {
        var validationContext = new ValidationContext(statModel);
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(statModel, validationContext, validationResults, true))
        {
            throw new ValidationException(errorString + '-' + validationResults);
        }
    }

    public async Task<int> Add(StatModel statModel)
    {
        ValidateSongModel(statModel, "Validation failed when trying to add stat.");
        return await _statRepository.Add(statModel);
    }
    
    public async Task<bool> Update(StatModel statModel)
    {
        ValidateSongModel(statModel, "Validation failed when trying to update stat.");
        return await _statRepository.Update(statModel);
    }
    
    public async Task<int> IncrementStreams(int songId)
    {
        var stats = await _statRepository.Get();
        var songStats = stats.Where(s => s.SongID == songId);
        var songStat = songStats.FirstOrDefault();
        if (songStat != null)
        {
            songStat.Streams++;
            return await _statRepository.Update(songStat) ? songStat.Streams : 0;
        }
        return 0;
    }
    
    public async Task<bool> Delete(int statId)
    {
        return await _statRepository.Delete(statId);
    }
    
    public async Task<bool> DeleteBySongId(int songId)
    {
        var stats = await _statRepository.Get();
        var songStats = stats.Where(s => s.SongID == songId);
        var songStat = songStats.FirstOrDefault();
        if (songStat != null)
        {
            return await _statRepository.Delete(songStat.StatID);
        }
        return false;
    }
    
    public async Task<StatModel> Get(int statId)
    {
        return await _statRepository.Get(statId);
    }
    
    public async Task<int> GetStreams(int songId)
    {
        var stats = await _statRepository.Get();
        var songStats = stats.Where(s => s.SongID == songId);
        var songStat = songStats.FirstOrDefault();
        return songStat?.Streams ?? 0;
    }
    
    public async Task<IEnumerable<StatModel>> Get()
    {
        return await _statRepository.Get();
    }
}
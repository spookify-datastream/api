using Microsoft.Extensions.Options;
using SpookifyApi.Configuration;
using SpookifyApi.Models;
using SpookifyApi.Repositories;
using System.ComponentModel.DataAnnotations;

namespace SpookifyApi.Services;

public class SongService
{
    private readonly ISongRepository _songRepository;
    private readonly string _songFilesPath;
    
    public SongService(ISongRepository songRepository, IOptions<FileSettings> fileSettings)
    {
        _songRepository = songRepository;
        _songFilesPath = fileSettings.Value.AudioFilesAbsPath;
    }

    // Check if songModel is valid
    private void ValidateSongModel(SongModel songModel, string errorString)
    {
        var validationContext = new ValidationContext(songModel);
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(songModel, validationContext, validationResults, true))
        {
            throw new ValidationException(errorString, validationResults);
        }
    }

    public async Task<int> Add(SongModel songModel)
    {
        ValidateSongModel(songModel, "Validation failed when trying to add song.");
        return await _songRepository.Add(songModel);
    }
    
    public async Task<bool> Update(SongModel songModel)
    {
        ValidateSongModel(songModel, "Validation failed when trying to update song.");
        return await _songRepository.Update(songModel);
    }
    
    public async Task<bool> Delete(int songId)
    {
        return await _songRepository.Delete(songId);
    }
    
    public async Task<SongModel> Get(int songId)
    {
        return await _songRepository.Get(songId);
    }
    
    public async Task<string> GetFilePath(int songId)
    {
        var song = await Get(songId);
        return song != null ? $"{_songFilesPath}/{song.Filename}" : null;
    }
    
    public async Task<IEnumerable<SongModel>> Get()
    {
        return await _songRepository.Get();
    }
}
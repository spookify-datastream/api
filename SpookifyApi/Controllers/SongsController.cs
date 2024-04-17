using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SpookifyApi.Configuration;
using SpookifyApi.Models;
using SpookifyApi.Repositories;
using SpookifyApi.Services;

namespace SpookifyApi.Controllers;


[ApiController]
[Route("[controller]")]
public class SongsController : ControllerBase
{
    private readonly SongService _songService;
    private readonly StatService _statService;

    public SongsController(SongService songService, StatService statService)
    {
        _songService = songService;
        _statService = statService;
    }

    [HttpPost]
    public async Task<IActionResult> Add(SongInputModel songModelInput)
    {
        var result = await _songService.Add(songModelInput.ToSongModel());
        //if the song was added successfully, add a stat entry for it
        if (result == 0) return BadRequest();

        var statModel = new StatModel
        {
            SongID = result,
            Streams = 0
        };
        result = await _statService.Add(statModel); //todo rollback :)

        return result > 0 ? Ok() : BadRequest();
    }
    
    [HttpPut]
    public async Task<IActionResult> Update(SongModel songModel)
    {
        var result = await _songService.Update(songModel);
        return result ? Ok() : BadRequest();
    }
    
    [HttpDelete("{songId}")]
    public async Task<IActionResult> Delete(int songId)
    {
        //first delete the associated stat, then the song
        var result = await _statService.DeleteBySongId(songId);
        if (!result) return BadRequest();
        result = await _songService.Delete(songId); //todo rollback 2
        return result ? Ok() : BadRequest();
    }
    
    [HttpGet("{songId}")] 
    public async Task<IActionResult> Get(int songId)
    {
        var song = await _songService.Get(songId);
        // append the streams to the song object
        var streams = await _statService.GetStreams(songId); 
        var response = new { song, streams }; //streams appended to this endpoint
        return song != null ? Ok(response) : NotFound();
    }
    
    [HttpGet]
    public async Task<IActionResult> Get() 
    {
        var songs = await _songService.Get(); //no streams here
        return songs != null ? Ok(songs) : NotFound();
    }
    
    [HttpGet("{id}/file")]
    public async Task<IActionResult> DownloadSongFile(int id)
    {
        //get the song filename from the repository
        var song = await _songService.Get(id);
        if (song == null) return NotFound("Song not found.");

        var filePath = await _songService.GetFilePath(id);
        if (!System.IO.File.Exists(filePath)) return NotFound("File not found.");

        var memoryStream = new MemoryStream();
        await using (var stream = new FileStream(filePath, FileMode.Open))
        {
            await stream.CopyToAsync(memoryStream);
        }
        memoryStream.Position = 0;
        
        await _statService.IncrementStreams(id);
        
        //get content type, not sure why this is needed
        var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(filePath, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return File(memoryStream, contentType, song.Filename);
    }
    
}
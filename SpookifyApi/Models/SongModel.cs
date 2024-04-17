using System.ComponentModel.DataAnnotations;

namespace SpookifyApi.Models;

public class SongModel
{
    public int SongID { get; set; }
    [Required]
    [StringLength(150)]
    public string Name { get; set; }
    [Required]
    [StringLength(150)]
    public string Artist { get; set; }
    [Required]
    [StringLength(150)]
    public string Album { get; set; }
    [Required]
    [StringLength(150)]
    public string Filename { get; set; }
}
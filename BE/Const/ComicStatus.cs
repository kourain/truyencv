using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
public enum ComicStatus
{
    [Display(Name = "Continuing")]
    Continuing = 1,
    [Display(Name = "Paused")]
    Paused = 2,
    [Display(Name = "Stopped")]
    Stopped = 3,
    [Display(Name = "Completed")]
    Completed = 4,
}
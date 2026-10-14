using Microsoft.AspNetCore.Mvc.RazorPages;

public class AboutModel : PageModel
{
    public string ServerName { get; set; } = "";
    public string Version { get; set; } = "";
    public string StartTime { get; set; } = "";

    public void OnGet()
    {
        ServerName = "Lesson1.WebServer";
        Version = "1.0";
        StartTime = System.Diagnostics.Process.GetCurrentProcess().StartTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
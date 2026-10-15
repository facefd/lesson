using Microsoft.AspNetCore.Mvc.RazorPages;

public class AboutModel : PageModel
{
    public string ServerName { get; set; } = "";
    public string Version { get; set; } = "";
    public string StartTime { get; set; } = "";

    public void OnGet()
    {
        ServerName = "lesson";
        Version = "1.0";
        StartTime = System.Diagnostics.Process.GetCurrentProcess().StartTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
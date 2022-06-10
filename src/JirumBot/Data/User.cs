using System.Collections.Generic;

namespace JirumBot.Data;

public class User
{
    public string UserId { get; set; }
    public string ChannelId { get; set; }
    public List<string> Keywords { get; set; }
}
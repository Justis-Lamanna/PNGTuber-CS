using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;

namespace PNGTuber
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class DiscordController : ControllerBase
    {
        private readonly DiscordClient client;
        private readonly ulong defaultChannel;

        public DiscordController(DiscordClient client, IConfiguration configuration)
        {
            this.client = client;
            this.defaultChannel = configuration.GetValue<ulong>("DefaultChannel");
        }
        
        [HttpGet("{user_id}")]
        public async Task<string> Name(ulong user_id)
        {
            DiscordChannel channel = await client.GetChannelAsync(defaultChannel);
            DiscordMember member = await channel.Guild.GetMemberAsync(user_id);
            return member.DisplayName;
        }
    }
}
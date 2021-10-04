using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PNGTuber.Controllers
{
    [ApiController]
    public class PNGTuberController : Controller
    {
        private readonly ILogger<PNGTuberController> _logger;
        private const ulong DEFAULT = 542131086000521221;

        public PNGTuberController(ILogger<PNGTuberController> logger)
        {
            _logger = logger;
        }

        [HttpGet("voice/{user_id}/{channel_id?}")]
        public async Task<State> GetVoiceState(ulong user_id, ulong? channel_id)
        {
            ulong channel_id_to_get = channel_id.GetValueOrDefault(DEFAULT);
            DiscordClient client = DiscordService.GetClient();
            DiscordChannel channel = await client.GetChannelAsync(channel_id_to_get);
            if(channel.GuildId.HasValue)
            {
                DiscordGuild guild = await client.GetGuildAsync(channel.GuildId.Value);
                DiscordMember member = await guild.GetMemberAsync(user_id);

                Response.ContentType = "text/event-stream";
                State initial = new State
                {
                    Online = IsOnline(member.VoiceState, channel_id_to_get),
                    Speaking = false
                };
            }
            throw new Exception("No such Guild");
        }

        private static bool IsOnline(DiscordVoiceState states, ulong channel_id)
        {
            return states != null && states.Channel != null && states.Channel.Id == channel_id;
        }
    }

    public class DiscordService
    {
        private static DiscordClient Client;

        public static DiscordClient GetClient()
        {
            if(Client == null)
            {
                Client = BuildClient().GetAwaiter().GetResult();
            }
            return Client;
        }

        private static async Task<DiscordClient> BuildClient()
        {
            DiscordClient NewClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = "MjY5NTIyMjgwOTc4MTg2MjQw.WHkSLw.h869PcMTZHeMxO0vKCluYmFnP6I",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            }) ;
            await NewClient.ConnectAsync();
            return NewClient;
        }
    }
}

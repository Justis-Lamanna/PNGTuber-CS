using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Threading;
using DSharpPlus;
using DSharpPlus.Entities;
using ExtensionMethods;
using Microsoft.Extensions.Configuration;

namespace PNGTuber
{
    public class VoiceHub : Hub
    {
        private readonly DiscordClient client;
        private readonly ulong defaultChannel;

        public VoiceHub(DiscordClient client, IConfiguration configuration)
        {
            this.client = client;
            this.defaultChannel = configuration.GetValue<ulong>("DefaultChannel");
        }
        
        public ChannelReader<bool> Online(ulong user_id, ulong? channel_id, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<bool>();
            _ = WriteOnline(channel.Writer, client, channel_id.GetValueOrDefault(this.defaultChannel), user_id, cancellationToken);
            return channel.Reader;
        }

        public async Task WriteOnline(ChannelWriter<bool> writer, DiscordClient client, ulong channel_id, ulong user_id, CancellationToken cancellationToken)
        {
            try
            {
                DiscordChannel channel = await client.GetChannelAsync(channel_id);
                if(channel.GuildId.HasValue)
                {
                    DiscordGuild guild = await client.GetGuildAsync(channel.GuildId.Value);
                    DiscordMember member = await guild.GetMemberAsync(user_id);
                
                    await writer.WriteAsync(IsOnline(member.VoiceState, channel_id), cancellationToken);

                    client.VoiceStateUpdated += async (s, e) =>
                    {
                        if(e.JoinedChannel(channel_id, user_id)) await writer.WriteAsync(true, cancellationToken);
                        else if(e.LeftChannel(channel_id, user_id)) await writer.WriteAsync(false, cancellationToken);
                        else await Task.CompletedTask;
                    };

                    await Task.Delay(-1, cancellationToken);
                } else
                {
                    throw new HubException();
                }
            } finally
            {
                writer.Complete();
            }
        }

        private static bool IsOnline(DiscordVoiceState states, ulong channel_id)
        {
            return states != null && states.Channel != null && states.Channel.Id == channel_id;
        }
    }
}

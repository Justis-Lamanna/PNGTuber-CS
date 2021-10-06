using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Threading;
using System;
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
        
        public ChannelReader<bool> Online(string user_id, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<bool>();
            _ = WriteOnline(channel.Writer, client, this.defaultChannel, Convert.ToUInt64(user_id), cancellationToken);
            return channel.Reader;
        }

        private async Task WriteOnline(ChannelWriter<bool> writer, DiscordClient client, ulong channel_id, ulong user_id, CancellationToken cancellationToken)
        {
            Exception local = null;
            try
            {
                DiscordChannel channel = await client.GetChannelAsync(channel_id);
                if (channel.GuildId.HasValue)
                {
                    DiscordMember member = await channel.Guild.GetMemberAsync(user_id);

                    await writer.WriteAsync(IsOnline(member.VoiceState, channel_id), cancellationToken);

                    client.VoiceStateUpdated += async (s, e) =>
                    {
                        if (e.JoinedChannel(channel_id, user_id)) await writer.WriteAsync(true, cancellationToken);
                        else if (e.LeftChannel(channel_id, user_id)) await writer.WriteAsync(false, cancellationToken);
                        else await Task.CompletedTask;
                    };

                    await Task.Delay(-1, cancellationToken);
                }
                else
                {
                    throw new HubException();
                }
            }
            catch (Exception ex)
            {
                local = ex;
            }
            finally
            {
                writer.Complete(local);
            }
        }

        public ChannelReader<bool> Speaking(string user_id, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<bool>();
            _ = WriteSpeaking(channel.Writer, cancellationToken);
            return channel.Reader;
        }

        private async Task WriteSpeaking(ChannelWriter<bool> writer, CancellationToken cancellationToken)
        {
            await writer.WriteAsync(false, cancellationToken);
            await Task.Delay(-1, cancellationToken);
        }

        private static bool IsOnline(DiscordVoiceState states, ulong channel_id)
        {
            return states != null && states.Channel != null && states.Channel.Id == channel_id;
        }
    }
}

using DSharpPlus.EventArgs;

namespace ExtensionMethods
{
    public static class Extensions
    {
        public static bool IsJoin(this VoiceStateUpdateEventArgs args)
        {
            return args.After != null && (args.Before == null || args.Before.Channel == null);
        }

        public static bool IsLeave(this VoiceStateUpdateEventArgs args)
        {
            return (args.After == null || args.After.Channel == null) && args.Before != null;
        }

        public static bool IsMove(this VoiceStateUpdateEventArgs args)
        {
            if (args.Before == null || args.Before.Channel == null || args.After == null || args.After.Channel == null) return false;
            return args.After.Channel.Id!= args.Before.Channel.Id;
        }

        public static bool LeftChannel(this VoiceStateUpdateEventArgs args, ulong channel_id, ulong user_id)
        {
            return (args.IsLeave() || args.IsMove()) && args.User.Id == user_id && args.Before.Channel.Id == channel_id;
        }

        public static bool JoinedChannel(this VoiceStateUpdateEventArgs args, ulong channel_id, ulong user_id)
        {
            return (args.IsJoin() || args.IsMove()) && args.User.Id == user_id && args.After.Channel.Id == channel_id;
        }
    }
}

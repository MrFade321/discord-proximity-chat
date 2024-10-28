using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DiscordRPC.Message
{
    public class SelectedVoiceChannel : IMessage
    {        
        public string Id { get; set; }
        public string Name { get; set; }
        public override MessageType Type { get { return MessageType.SelectedVoiceChannel; } }
        public string Topic { get; set; }
        public int Bitrate { get; set; }
        public int user_limit { get; set; }
        public string Guild_Id { get; set; }
        public int Position { get; set; }
        public List<string> messages { get; set; }
        public List<VoiceState> Voice_States { get; set; }
    }

    public class VoiceState
    {
        public string Nick { get; set; }
        public bool Mute { get; set; }
        public int Volume { get; set; }
        public Pan Pan { get; set; }
        public VoiceStateDetails VoiceStateDetails { get; set; }
        public user User { get; set; }
    }

    public class Pan
    {
        [JsonProperty("left")]
        public float Left { get; set; }

        [JsonProperty("right")]
        public float Right { get; set; }
    }

    public class VoiceStateDetails
    {
        public bool Mute { get; set; }
        public bool Deaf { get; set; }
        public bool SelfMute { get; set; }
        public bool SelfDeaf { get; set; }
        public bool Suppress { get; set; }
    }

    public class user
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string GlobalName { get; set; }
        public string Avatar { get; set; }
        public AvatarDecorationData AvatarDecorationData { get; set; }
        public bool Bot { get; set; }
        public int Flags { get; set; }
        public int PremiumType { get; set; }
    }

    public class AvatarDecorationData
    {
        public string Asset { get; set; }
        public string SkuId { get; set; }
    }

}

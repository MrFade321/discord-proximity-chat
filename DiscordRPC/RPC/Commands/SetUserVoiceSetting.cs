using DiscordRPC.Message;
using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordRPC.RPC.Commands
{
    internal class SetUserVoiceSetting : ICommand
    {
        [JsonProperty("user_id")]
        public string userID { get; set; }

        [JsonProperty("pan")]
        public Pan pan { get; set; }

        [JsonProperty("volume")]
        public int volume { get; set; }

        [JsonProperty("mute")]
        public bool mute { get; set; }

        public IPayload PreparePayload(long nonce)
        {
            return new ArgumentPayload(this, nonce)
            {
                Command = Command.SetUserVoiceSettings
            };
        }

    }
}

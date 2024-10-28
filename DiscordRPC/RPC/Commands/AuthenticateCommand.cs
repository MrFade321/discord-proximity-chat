using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordRPC.RPC.Commands
{
    internal class AuthenticateCommand : ICommand
    {
        ///// <summary>
        ///// AuthToken
        ///// </summary>
        [JsonProperty("access_token")]
        //[JsonProperty("code")]
        public string AuthToken { get; set; }

        ///// <summary>
        ///// The rich presence to be set. Can be null.
        ///// </summary>
        //[JsonProperty("activity")]
        //public RichPresence Presence { get; set; }

        public IPayload PreparePayload(long nonce)
        {
            return new ArgumentPayload(this, nonce)
            {
                Command = Command.Authenticate
            };
        }
    }
}


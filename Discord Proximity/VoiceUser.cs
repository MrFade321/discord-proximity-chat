using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Proximity
{
    public class VoiceUser
    {
        public bool LocalUser { get; set; }
        public DraggableCircle VisualCircle { get; set; }
        public string ID { get; set; }  
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public bool BOT { get; set; }
        public VoiceState voiceState { get; set; }

        public string GetAvatarLink(string format)
        {
            //https://cdn.discordapp.com/avatars/447287594309517323/85c8c6917c14d60435c7cc09a9a713fd.png

            string Link = "https://cdn.discordapp.com/avatars/";

            Link += ID + "/" + Avatar + $".{format}";

             return Link;
        }



    }

    public class VoiceState
    {
        public bool mute { get; set; }
        public bool deaf { get; set; }
        public int volume { get; set; }
        public Pan pan { get; set; }
    }

    public class Pan
    {
        public float left { get; set; }
        public float right { get; set; }
    }


}

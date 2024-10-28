using DiscordRPC;
using DiscordRPC.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using static Telerik.WinControls.UI.ValueMapper;

namespace Discord_Proximity
{
    public partial class Main : Telerik.WinControls.UI.RadForm
    {
        private static int discordPipe = -1;
        private static DiscordRPC.Logging.LogLevel logLevel = DiscordRPC.Logging.LogLevel.Trace;
        private static List<VoiceUser> voiceUsers = new List<VoiceUser>();
        private string LocalUserName = "";
        private List<(Point, double)> connections = new List<(Point, double)>();
        private DiscordRPC.DiscordRpcClient client;
        private float maxDistance = 1000;
        public DraggableCircle LocalUserCircle { get; set; } // Set this to the local user's position
        public Point localUserPosition { get; set; }
        public Main()
        {
            InitializeComponent();
        }

        private double CalculateDistance(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
        }

        private void AddControlToForm(Control control)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Control>(AddControlToForm), control);
            }
            else
            {
                this.Controls.Add(control);
            }
        }

        private void RadForm1_Load(object FormSender, EventArgs e)
        {
            //// Add a new circle at the mouse click location
            //DraggableCircle circle = new DraggableCircle
            //{
            //    Location = new Point(1,1),
            //    Size = new Size(60, 60) // Set the desired size for the circle
            //};

            //// Load an image (update the path accordingly)
            //// circle.LoadImage("path_to_your_image.png");

            //circle.LoadImageAsync("https://cdn.discordapp.com/avatars/447287594309517323/85c8c6917c14d60435c7cc09a9a713fd.png");


            //this.Controls.Add(circle); // Add the circle to the form

            SetupEvents();
        }
        private bool debuglineBool = false;
        public void DrawDebugLines()
        {
            while (debuglineBool)
            {
                connections.Clear(); // Clear previous connections
                if (LocalUserCircle != null)
                {
                    localUserPosition = new Point(LocalUserCircle.Location.X + (LocalUserCircle.Width / 2), LocalUserCircle.Location.Y + LocalUserCircle.Height / 2);

                    foreach (Control control in this.Controls)
                    {
                        if (control is DraggableCircle circle && circle != LocalUserCircle)
                        {
                            double distance = CalculateDistance(
                                localUserPosition,
                                new Point(circle.Location.X + (circle.Width / 2), circle.Location.Y + (circle.Height / 2))
                            );

                            connections.Add((new Point(circle.Location.X + (circle.Width / 2), circle.Location.Y + (circle.Height / 2)), distance));
                        }
                    }

                    // Trigger a redraw
                    this.Invalidate();
                }
                Thread.Sleep(10);
            }
        }
        private float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public float DistanceToVolume(float distance)
        {
            // Clamp distance to be between 0 and maxDistance
            if (distance < 0)
                distance = 0;
            if (distance > maxDistance)
                return 0; // Volume is 0% if distance exceeds maxDistance

            // Calculate volume percentage
            float volumePercentage = 200 * (1 - (distance / maxDistance));

            // Ensure the volume percentage is between 0 and 200
            return Clamp(volumePercentage, 0, 200);
        }

        public void UpdateUserVolumes()
        {
            foreach (Control control in this.Controls)
            {
                if (control is DraggableCircle circle && circle != LocalUserCircle)
                {
                    double distance = CalculateDistance(
                        localUserPosition,
                        new Point(circle.Location.X + (circle.Width / 2), circle.Location.Y + (circle.Height / 2))
                    );

                    var VolumePer = DistanceToVolume((float)distance); // Gross casting close your eyes child.


                    Console.WriteLine($"VolumePer returns ---------------------------------> {VolumePer}");

                    DiscordRPC.Message.Pan MyPan = new DiscordRPC.Message.Pan(); // This is also some gross stuff need to fix this.
                    MyPan.Left = 0.0f;
                    MyPan.Right = 1.0f; // Pretty sure discord does not honnor panning anymore?
                    client.SetUserVoiceSettings(circle._UserId, MyPan, (int)VolumePer, false);

                    Thread.Sleep(50); // Try not spam the RPC;
                }
            }
        }



        private bool UpdateThreadBool = false;
        public void Update()
        {
            while(UpdateThreadBool)
            {
                UpdateUserVolumes();
                client.GetCurrentVoiceChannel();
                Thread.Sleep(1000);
            }            
        }

        public void SetupEvents()
        {

            // == Create the client
            client = new DiscordRpcClient("1297861472273043517", pipe: discordPipe)
            {
                Logger = new DiscordRPC.Logging.ConsoleLogger(logLevel, true)
            };

            // == Initialize
            client.Initialize();

            Thread DrawLines = new Thread(DrawDebugLines);
            debuglineBool = true;
            DrawLines.Start();


            Thread UpdateTick = new Thread(Update);
            UpdateThreadBool = true;
            UpdateTick.Start();


            // == Subscribe to some events
            client.OnReady += (sender, msg) =>
            {
                //Create some events so we know things are happening
                Console.WriteLine("Connected to discord with user {0}", msg.User.Username);

                if(LocalUserName != msg.User.Username)
                    LocalUserName = msg.User.Username;

         
            };

            client.OnPresenceUpdate += (sender, msg) =>
            {
                //The presence has updated
                Console.WriteLine("Presence has been updated! ");
            };

            client.OnSelectVoiceChannel += async (sender, msg) =>
            {
                Console.WriteLine($"A selected voice channel event was fired!");
                Console.WriteLine($"{msg.Name}");
                Console.WriteLine($"<-Current Voices->");
                Console.WriteLine("<----------------------------->");

                foreach (var AVoiceState in msg.Voice_States)
                {

                    bool Exists = false;
                    foreach (var User in voiceUsers)
                    {
                        if (User.UserName == AVoiceState.User.Username || User.ID == AVoiceState.User.Id)
                        {
                            Exists = true;
                            break;
                        }                                                   
                    }

                    if (Exists)
                        continue;
                    
                    VoiceUser NewUser = new VoiceUser();

                    NewUser.UserName = AVoiceState.User.Username;
                    NewUser.BOT = AVoiceState.User.Bot;
                    NewUser.ID = AVoiceState.User.Id;
                    

                    NewUser.voiceState = new VoiceState { 
                        deaf = false, // broken? 
                        mute = false, // broken ?
                        volume = AVoiceState.Volume,                        
                            /// Pan is broken ?
                        };
                    NewUser.Avatar = AVoiceState.User.Avatar;

                    NewUser.LocalUser = (LocalUserName == NewUser.UserName);

    
      
                    DraggableCircle circle = new DraggableCircle
                    {
                        Location = new Point(1, 1),
                        Size = new Size(60, 60) // Set the desired size for the circle
                    };

                
                    Task.Run(async () =>
                    {
                        var avatarLink = NewUser.GetAvatarLink("png");
                        await circle.LoadImageAsync(avatarLink);
                    });

                    circle._UserId = AVoiceState.User.Id;  // The circle will store a copy of the userID 
                    NewUser.VisualCircle = circle; // last

                    if(NewUser.LocalUser)
                    LocalUserCircle = circle;

                    AddControlToForm(NewUser.VisualCircle);



                    voiceUsers.Add(NewUser);

                }
 

               Console.WriteLine("<----------------------------->");
            };
            

            //client.GetCurrentVoiceChannel();

            // == Do the rest of your program.
            //Simulated by a Console.ReadKey
            // etc...
            //Console.ReadKey();

            // == At the very end we need to dispose of it
            //client.Dispose();
        }

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            //base.OnPaint(e);

            // Draw connections from local user to other circles
            if (LocalUserCircle != null)
            {
                List<(Point,double)> CacheConnections = new List<(Point,double)>();
                CacheConnections.AddRange(connections);
                foreach (var (circleCenter, distance) in CacheConnections)
                {
                    // Draw the red line
                    e.Graphics.DrawLine(Pens.Red, localUserPosition, circleCenter);

                    // Draw the distance text below the line
                    string distanceText = $"{distance:F2} units"; // Format distance
                    e.Graphics.DrawString(distanceText, this.Font, Brushes.Black, (localUserPosition.X + circleCenter.X) / 2, (localUserPosition.Y + circleCenter.Y) / 2 + 10);
                }
            }
        }
    }
}

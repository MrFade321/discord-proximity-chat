using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DiscordRPC.Example
{
    public partial class Main : Form
    {
        private bool isDragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public Main()
        {
            InitializeComponent();
            InitializeDragablePanel();
        }

        private void InitializeDragablePanel()
        {
            Panel draggablePanel = new Panel()
            {
                Size = new Size(100,100),
                BackColor = Color.Blue,
                Location = new Point(50,50),
            };
            draggablePanel.MouseDown += DraggablePanel_MouseDown;
            draggablePanel.MouseMove += DraggablePanel_MouseMove;
            draggablePanel.MouseUp += DraggablePanel_MouseUp;
            draggablePanel.Show();
        }

        private void DraggablePanel_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void DraggablePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void DraggablePanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeDragablePanel();
        }
    }
}

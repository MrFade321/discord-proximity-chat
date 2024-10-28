using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

public class DraggableCircle : Control
{
    private Image _image;
    private bool _isDragging;
    private Point _dragStartPoint;
    public string _UserId;

    public DraggableCircle()
    {
        this.Size = new Size(100, 100); // Set the default size of the circle
        this.BackColor = Color.White; // Make the background transparent
        this.Cursor = Cursors.Hand; // Change cursor to hand when hovering
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // Draw the circle
        using (Brush brush = new SolidBrush(Color.Blue))
        {
            e.Graphics.FillEllipse(brush, 0, 0, this.Width, this.Height);
        }

        // Draw the image if it exists
        if (_image != null)
        {
            e.Graphics.DrawImage(_image, 0, 0, this.Width, this.Height);
        }
    }


    public void LoadImage(string imagePath)
    {
        _image = Image.FromFile(imagePath);
        this.Invalidate(); // Redraw the control
    }

    public async Task LoadImageAsync(string url)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                // Fetch the stream
                using (var stream = await client.GetStreamAsync(url))
                {
                    // Load the image on the background thread
                    _image = Image.FromStream(stream);
                }
            }

            // Now switch back to the UI thread to invalidate the control
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => this.Invalidate())); // Switch to UI thread
            }
            else
            {
                this.Invalidate(); // On the UI thread already
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading image: {ex.Message}");
        }
    }



    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left)
        {
            _isDragging = true;
            _dragStartPoint = e.Location;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_isDragging)
        {
            this.Left += e.X - _dragStartPoint.X;
            this.Top += e.Y - _dragStartPoint.Y;
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button == MouseButtons.Left)
        {
            _isDragging = false;
        }
    }
}

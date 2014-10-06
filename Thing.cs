using System.Drawing;

namespace MazeMasters
{
    public class Thing
    {
        private Bitmap image;

        public Thing(Bitmap image)
        {
            this.image = image;
            this.image.MakeTransparent(Color.White);
        }

        public void Draw(Graphics graphics, Rectangle bounds)
        {
            float widthRatio = (float)bounds.Width / (float)image.Width;
            float heightRatio = (float)bounds.Height / (float)image.Height;

            Rectangle rectangle = bounds;
            if (widthRatio > heightRatio)
            {
                rectangle.Width = (int)((float)image.Width * heightRatio);
                rectangle.X += (bounds.Width - rectangle.Width) / 2;
            }
            else
            {
                rectangle.Height = (int)((float)image.Height * widthRatio);
                rectangle.Y += (bounds.Height - rectangle.Height) / 2;
            }

            graphics.DrawImage(image, rectangle);
        }
    }
}

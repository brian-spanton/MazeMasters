using System.Drawing;

namespace MazeMasters
{
    public static class Extensions
    {
        public static void FillCircle(this Graphics graphics, Brush brush, Rectangle rectangle)
        {
            Rectangle adjusted = new Rectangle(
                rectangle.X,
                rectangle.Y - 1,
                rectangle.Width,
                rectangle.Height + 1);

            graphics.FillEllipse(brush, adjusted);
        }
    }
}

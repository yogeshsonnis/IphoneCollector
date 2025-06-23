using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphoneCollector.Helper
{
    public class DashedBorderDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Color.FromArgb("#ccc");
            canvas.StrokeSize = 2;
            canvas.StrokeDashPattern = new float[] { 6, 3 }; // dash, gap
            canvas.FillColor = Color.FromArgb("#f9f9f9");

            float cornerRadius = 8;
            canvas.FillRoundedRectangle(dirtyRect, cornerRadius);
            canvas.DrawRoundedRectangle(dirtyRect, cornerRadius);
        }
    }
}

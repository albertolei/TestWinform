using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace TestWinForm.Utils
{
    class AnnotationUtil
    {
        public const float ANNOTATIONEND = 10;
        internal static bool drawLengthAnnotation(Graphics graphics, PointF startPoint, PointF endPoint, string length)
        {
            Pen pen = new Pen(Color.LightBlue,1);
            SolidBrush brush = new SolidBrush(Color.FromArgb(108, 108, 108));
            System.Drawing.Font font = new System.Drawing.Font("Trebuchet MS", 8);
            graphics.DrawLine(pen, startPoint, endPoint);
            if (startPoint.Y == endPoint.Y) //水平标注
            {
                graphics.DrawLine(pen, startPoint.X - ANNOTATIONEND / (float)Math.Sqrt(2.0), startPoint.Y + ANNOTATIONEND / (float)Math.Sqrt(2.0), startPoint.X + ANNOTATIONEND / (float)Math.Sqrt(2.0), startPoint.Y - ANNOTATIONEND / (float)Math.Sqrt(2.0));
                graphics.DrawLine(pen, endPoint.X - ANNOTATIONEND / (float)Math.Sqrt(2.0), endPoint.Y + ANNOTATIONEND / (float)Math.Sqrt(2.0), endPoint.X + ANNOTATIONEND / (float)Math.Sqrt(2.0), endPoint.Y - ANNOTATIONEND / (float)Math.Sqrt(2.0));
                graphics.DrawLine(pen, startPoint.X, startPoint.Y -  2 * ANNOTATIONEND, startPoint.X, startPoint.Y + ANNOTATIONEND);
                graphics.DrawLine(pen, endPoint.X, endPoint.Y - 2 * ANNOTATIONEND, endPoint.X, endPoint.Y + ANNOTATIONEND);
                graphics.DrawString(length, font, brush, startPoint.X + (endPoint.X - startPoint.X) / 2 - graphics.MeasureString(length, font).Width / 2, endPoint.Y - graphics.MeasureString(length, font).Height);
            }
            else if (startPoint.X == endPoint.X) //垂直标注
            {
                graphics.DrawLine(pen, startPoint.X + ANNOTATIONEND / (float)Math.Sqrt(2.0), startPoint.Y + ANNOTATIONEND / (float)Math.Sqrt(2.0), startPoint.X - ANNOTATIONEND / (float)Math.Sqrt(2.0), startPoint.Y - ANNOTATIONEND / (float)Math.Sqrt(2.0));
                graphics.DrawLine(pen, endPoint.X + ANNOTATIONEND / (float)Math.Sqrt(2.0), endPoint.Y + ANNOTATIONEND / (float)Math.Sqrt(2.0), endPoint.X - ANNOTATIONEND / (float)Math.Sqrt(2.0), endPoint.Y - ANNOTATIONEND / (float)Math.Sqrt(2.0));
                graphics.DrawLine(pen, startPoint.X - 2 * ANNOTATIONEND, startPoint.Y, startPoint.X + ANNOTATIONEND, startPoint.Y);
                graphics.DrawLine(pen, endPoint.X - 2 * ANNOTATIONEND, endPoint.Y, endPoint.X + ANNOTATIONEND, endPoint.Y);
                graphics.TranslateTransform(startPoint.X - graphics.MeasureString(length, font).Height, (startPoint.Y + endPoint.Y) / 2 + graphics.MeasureString(length, font).Width / 2);
                graphics.RotateTransform(-90);
                graphics.DrawString(length, font, brush, 0, 0);
                graphics.RotateTransform(90);
                graphics.TranslateTransform(-startPoint.X + graphics.MeasureString(length, font).Height, -(startPoint.Y + endPoint.Y) / 2 - graphics.MeasureString(length, font).Width / 2);
            }
            else
            {
                return false;
            }
            return true;
        }

    }
}

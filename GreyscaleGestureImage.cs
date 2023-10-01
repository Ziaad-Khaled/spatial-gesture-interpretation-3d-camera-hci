using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class GreyscaleGestureImage : GestureImage
    {

        private Pen greyScalePen = new Pen(Color.FromArgb(255, 0, 0, 0), 10);



        public GreyscaleGestureImage(string pathToGesture, int gestureCounter)
        {

            base.Bitmap = new Bitmap(350, 350, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            base.Graphics = Graphics.FromImage(base.Bitmap);
            base.Path = pathToGesture + "\\CNN Images\\" + gestureCounter + ".png";
        }


        // for drawing the image for the machine learning approach
        public void DrawLines(Point[] nucleusPoints)
        {
            base.Graphics.DrawLines(greyScalePen, nucleusPoints);
        }

    }

}

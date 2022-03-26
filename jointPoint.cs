using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class jointPoint
    {
        private double x;
        private double y;
        private double z;

        public double GetX()
        {
            return x;
        }

        public void SetX(double value)
        {
            x = value;
        }

        public double GetY()
        {
            return y;
        }

        public void SetY(double value)
        {
            y = value;
        }

        public double GetZ()
        {
            return z;
        }

        public void SetZ(double value)
        {
            z = value;
        }
    }
}

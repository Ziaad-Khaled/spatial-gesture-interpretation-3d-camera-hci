using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class AccelerationPoint
    {
        private double xAcceleration;
        private double yAcceleration;
        private double zAcceleration;

        private Point position;

        public AccelerationPoint(double xAcceleration, double yAcceleration, double zAcceleration, Point point)
        {
            this.xAcceleration = xAcceleration;
            this.yAcceleration = yAcceleration;
            this.zAcceleration = zAcceleration;
            this.position = point;
        }
        public Point GetPosition()
        {
            return position;
        }

        public void SetPosition(Point value)
        {
            position = value;
        }

        public double GetXAcceleration()
        {
            return xAcceleration;
        }

        public void SetXAcceleration(double value)
        {
            xAcceleration = value;
        }

        public double GetYAcceleration()
        {
            return yAcceleration;
        }

        public void SetYAcceleration(double value)
        {
            yAcceleration = value;
        }

        public double GetZAcceleration()
        {
            return zAcceleration;
        }

        public void SetZAcceleration(double value)
        {
            zAcceleration = value;
        }
    }
}

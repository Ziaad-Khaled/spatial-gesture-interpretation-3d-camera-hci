using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class VelocityLine
    {
        VelocityPoint point1;
        VelocityPoint point2;

        public VelocityLine(VelocityPoint point1, VelocityPoint point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }

        public VelocityPoint GetPoint1()
        {
            return point1;
        }

        public void SetPoint1(VelocityPoint value)
        {
            point1 = value;
        }

        public VelocityPoint GetPoint2()
        {
            return point2;
        }

        public void SetPoint2(VelocityPoint value)
        {
            point2 = value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Tracking
{
    public class DataWork
    {
        public struct PointA
        {
            public double X, Y, Z;

            public PointA(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

        }

        public void dataWork(List<PointA> track)
        {
            double x0 = track.Min(e => e.X);
            double x1 = track.Max(e => e.X);
            double y0 = track.Min(e => e.Y);
            double y1 = track.Max(e => e.Y);

            Point point1 = new Point();
            Point point2 = new Point();
            Point point3 = new Point();
            Point point4 = new Point();
            //Ohraničení oblasti...
            point1.X = x1;
            point1.Y = y0;

            point2.X = x0;
            point2.Y = y0;

            point3.X = x1;
            point3.Y = y1;

            point4.X = x0;
            point4.Y = y1;

        }
    }
}

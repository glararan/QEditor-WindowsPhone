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
        private double getBigX(List<PointA> track)
        {
            double nej;
            if (track[0].X > track[1].X)
            {
                nej = track[0].X;
            }
            else
            {
                nej = track[1].X;
            }
            for (int i = 2; i < track.Count; i++)
            {
                if (nej < track[i].X)
                {
                    nej = track[i].X;
                }
            }
            return nej;
        }

        private double getLittleX(List<PointA> track)
        {
            double nej;
            if (track[0].X > track[1].X)
            {
                nej = track[1].X;
            }
            else
            {
                nej = track[0].X;
            }
            for (int i = 2; i < track.Count; i++)
            {
                if (nej > track[i].X)
                {
                    nej = track[i].X;
                }
            }
            return nej;
        }

        private double getBigY(List<PointA> track)
        {
            double nej;
            if (track[0].X > track[1].X)
            {
                nej = track[0].X;
            }
            else
            {
                nej = track[1].X;
            }
            for (int i = 2; i < track.Count; i++)
            {
                if (nej < track[i].X)
                {
                    nej = track[i].X;
                }
            }
            return nej;
        }

        private double getLittleY(List<PointA> track)
        {
            double nej;
            if (track[0].X > track[1].X)
            {
                nej = track[1].X;
            }
            else
            {
                nej = track[0].X;
            }
            for (int i = 2; i < track.Count; i++)
            {
                if (nej > track[i].X)
                {
                    nej = track[i].X;
                }
            }
            return nej;
        }

        public void dataWork(List<PointA> track)
        {
            double x0 = getLittleX(track);
            double x1 = getBigX(track);
            double y0 = getLittleY(track);
            double y1 = getBigY(track);

            Point point1=new Point();
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

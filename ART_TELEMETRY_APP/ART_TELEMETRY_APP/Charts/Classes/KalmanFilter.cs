using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.Charts.Classes
{
    public class KalmanFilter
    {
        public KalmanFilter(double A, double H, double Q, double R, double initial_P, double initial_x)
        {
            this.A = A;
            this.H = H;
            this.Q = Q;
            this.R = R;
            P = initial_P;
            x = initial_x;
        }

        private readonly double A;
        private readonly double H;
        private readonly double Q;
        private readonly double R;
        private double P;
        private double x;

        public double Output(double input)
        {
            // time update - prediction
            x = A * x;
            P = A * P * A + Q;

            // measurement update - correction
            double K = P * H / (H * P * H + R);
            x += K * (input - H * x);
            P = (1 - K * H) * P;

            return x;
        }
    }
}

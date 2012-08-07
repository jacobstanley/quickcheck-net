using System;

namespace QuickCheck.Random
{
    public static class RandomDistribution
    {
        /// <summary>
        /// Generate a normally distributed <see cref="double"/> with
        /// mean 0 and standard deviation 1.
        /// </summary>
        public static double Normal(this IRandom random)
        {
            // Use Box-Muller algorithm
            double u1 = random.Double();
            double u2 = random.Double();
            double r = Math.Sqrt(-2.0 * Math.Log(u1));
            double theta = 2.0 * Math.PI * u2;
            return r * Math.Sin(theta);
        }

        /// <summary>
        /// Generate a normally distributed <see cref="double"/>.
        /// </summary>
        public static double Normal(this IRandom random, double mean, double standardDeviation)
        {
            if (standardDeviation <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    "standardDeviation", standardDeviation, "Standard deviation must be positive.");
            }

            return mean + standardDeviation * random.Normal();
        }

        /// <summary>
        /// Generate an exponentially distributed <see cref="double"/> with mean 1.
        /// </summary>
        public static double Exponential(this IRandom random)
        {
            return -Math.Log(random.Double());
        }

        /// <summary>
        /// Generate a exponentially distributed <see cref="double"/>.
        /// </summary>
        public static double Exponential(this IRandom random, double mean)
        {
            if (mean <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    "mean", mean, "Mean must be positive.");
            }

            return mean * random.Exponential();
        }

        /// <summary>
        /// Generate a gamma distributed <see cref="double"/>.
        /// </summary>
        public static double Gamma(this IRandom random, double shape, double scale)
        {
            // Implementation based on "A Simple Method for Generating Gamma Variables"
            // by George Marsaglia and Wai Wan Tsang.  ACM Transactions on Mathematical Software
            // Vol 26, No 3, September 2000, pages 363-372.

            if (shape >= 1.0)
            {
                double d = shape - 1.0 / 3.0;
                double c = 1.0 / Math.Sqrt(9.0 * d);

                for (;;)
                {
                    double v;
                    double x;
                    do
                    {
                        x = random.Normal();
                        v = 1.0 + c * x;
                    } while (v <= 0.0);

                    v = v * v * v;
                    double u = random.Double();
                    double xsquared = x * x;

                    if (u < 1.0 - .0331 * xsquared * xsquared ||
                        Math.Log(u) < 0.5 * xsquared + d * (1.0 - v + Math.Log(v)))
                    {
                        return scale * d * v;
                    }
                }
            }

            if (shape <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    "shape", shape, "Shape must be positive.");
            }

            double g = random.Gamma(shape + 1.0, 1.0);
            double w = random.Double();
            return scale * g * Math.Pow(w, 1.0 / shape);
        }

        /// <summary>
        /// Generate a chi-squared distributed <see cref="double"/>.
        /// </summary>
        public static double ChiSquared(this IRandom random, double degreesOfFreedom)
        {
            // A chi squared distribution with n degrees of freedom
            // is a gamma distribution with shape n/2 and scale 2.
            return random.Gamma(0.5 * degreesOfFreedom, 2.0);
        }

        /// <summary>
        /// Generate an inverse-gamma distributed <see cref="double"/>.
        /// </summary>
        public static double InverseGamma(this IRandom random, double shape, double scale)
        {
            // If X is gamma(shape, scale) then
            // 1/Y is inverse gamma(shape, 1/scale)
            return 1.0 / random.Gamma(shape, 1.0 / scale);
        }

        /// <summary>
        /// Generate Weibull distributed <see cref="double"/>.
        /// </summary>
        public static double Weibull(this IRandom random, double shape, double scale)
        {
            if (shape <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    "shape", shape, "Shape must be positive.");
            }
            if (shape <= 0.0 || scale <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    "scale", scale, "Scale must be positive.");
            }
            return scale * Math.Pow(-Math.Log(random.Double()), 1.0 / shape);
        }

        /// <summary>
        /// Generate Cauchy distributed <see cref="double"/>.
        /// </summary>
        public static double Cauchy(this IRandom random, double median, double scale)
        {
            if (scale <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "scale", scale, "Scale must be positive.");
            }

            double p = random.Double();

            // Apply inverse of the Cauchy distribution function to a uniform
            return median + scale * Math.Tan(Math.PI * (p - 0.5));
        }

        /// <summary>
        /// Generate a t-distributed <see cref="double"/>.
        /// </summary>
        public static double StudentT(this IRandom random, double degreesOfFreedom)
        {
            if (degreesOfFreedom <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "degreesOfFreedom", degreesOfFreedom, "Degrees of freedom must be positive.");
            }

            // See Seminumerical Algorithms by Knuth
            double y1 = random.Normal();
            double y2 = random.ChiSquared(degreesOfFreedom);
            return y1 / Math.Sqrt(y2 / degreesOfFreedom);
        }

        /// <summary>
        /// Generate a Laplace distributed <see cref="double"/>.
        /// </summary>
        public static double Laplace(this IRandom random, double mean, double scale)
        {
            double u = random.Double();

            return (u < 0.5)
                ? mean + scale * Math.Log(2.0 * u)
                : mean - scale * Math.Log(2 * (1 - u));
        }

        /// <summary>
        /// Generate a log-normally distributed <see cref="double"/>.
        /// </summary>
        public static double LogNormal(this IRandom random, double mu, double sigma)
        {
            return Math.Exp(random.Normal(mu, sigma));
        }

        /// <summary>
        /// Generate a beta distributed <see cref="double"/>.
        /// </summary>
        public static double Beta(this IRandom random, double a, double b)
        {
            if (a <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "a", a, "Beta parameters must be positive.");
            }

            if (b <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "b", b, "Beta parameters must be positive.");
            }

            // There are more efficient methods for generating beta samples.
            // However such methods are a little more efficient and much more complicated.
            // For an explanation of why the following method works, see
            // http://www.johndcook.com/distribution_chart.html#gamma_beta

            double u = random.Gamma(a, 1.0);
            double v = random.Gamma(b, 1.0);
            return u / (u + v);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Services {
    static class Unitls {
        private static BigInteger ONE = BigInteger.One;
        private static BigInteger TWO = BigInteger.Parse("2");
        private static BigInteger ZERO = BigInteger.Zero;

        private static bool isEven(BigInteger bi) {
            return bi.IsEven;
        }

        private static BigInteger GetRandomBi(BigInteger n, Random rnd) {
            BigInteger r;
            do {
                int length = (int)Math.Ceiling(BigInteger.Log(n, 2));
                int numBytes = (int)Math.Ceiling(length / 8.0);
                byte[] data = new byte[numBytes];
                rnd.NextBytes(data);
                r = new BigInteger(data);
            } while (r.CompareTo(n) >= 0);
            return r;
        }
        public static void GenerateRSAKoef(ref BigInteger p,ref BigInteger q, BigInteger e, BigInteger n) {
            BigInteger d = new BigInteger(3);
            // Step 1: Let k = de – 1. If k is odd, then go to Step 4
            BigInteger k = d * e - (ONE);
            if (isEven(k)) {

                // Step 2 (express k as (2^t)r, where r is the largest odd integer
                // dividing k and t >= 1)
                BigInteger r = k;
                BigInteger t = ZERO;

                do {
                    r = r / TWO;
                    t = t + ONE;
                } while (isEven(r));

                // Step 3
                Random random = new Random();
                bool success = false;
                BigInteger y = BigInteger.Zero;

                for (int i = 1; i <= 100; i++) {

                    // 3a
                    BigInteger g = GetRandomBi(n, random);

                    // 3b
                    y = BigInteger.ModPow(g, r, n);

                    // 3c
                    if (y.Equals(ONE) || y.Equals(n - ONE)) {
                        // 3g
                        continue ;
                    }

                    // 3d
                    for (BigInteger j = ONE; j.CompareTo(t) <= 0; j = j + ONE) {
                        // 3d1
                        BigInteger x = BigInteger.ModPow(y, TWO, n);

                        // 3d2
                        if (x.Equals(ONE)) {
                            success = true;
                            break;
                        }

                        // 3d3
                        if (x.Equals(n - ONE)) {
                            // 3g
                            continue;
                        }

                        // 3d4
                        y = x;
                    }

                    // 3e
                    BigInteger x2 = BigInteger.ModPow(y, TWO, n);
                    if (x2.Equals(ONE)) {

                        success = true;
                        break;

                    }

                    // 3g
                    // (loop again)
                }

                if (success) {
                    // Step 5
                    p = GCD(y - ONE, n);
                    q = n / p;
                    return;
                }
            }
        }

        private static BigInteger GCD(BigInteger a, BigInteger b) {
            while (a != 0 && b != 0) {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }


        public static RSAParameters Create(byte[] p, byte[] q, byte[] exponent, byte[] modulus) {
            var addlParameters = GetFullPrivateParameters(
                p: new BigInteger(CopyAndReverse(p)),
                q: new BigInteger(CopyAndReverse(q)),
                e: new BigInteger(CopyAndReverse(exponent)),
                modulus: new BigInteger(CopyAndReverse(modulus)));

            return new RSAParameters {
                P = p,
                Q = q,
                Exponent = exponent,
                Modulus = modulus,
                D = addlParameters.D,
                DP = addlParameters.DP,
                DQ = addlParameters.DQ,
                InverseQ = addlParameters.InverseQ,
            };
        }

        private static RSAParameters GetFullPrivateParameters(BigInteger p, BigInteger q, BigInteger e, BigInteger modulus) {
            var n = p * q;
            var phiOfN = (p - 1) * (q - 1);

            var d = ModInverse(e, phiOfN);
            //Assert.Equal(1, (d * e) % phiOfN);

            var dp = d % (p - 1);
            var dq = d % (q - 1);

            var qInv = ModInverse(q, p);
            //Assert.Equal(1, (qInv * q) % p);

            return new RSAParameters {
                D = CopyAndReverse(d.ToByteArray()),
                DP = CopyAndReverse(dp.ToByteArray()),
                DQ = CopyAndReverse(dq.ToByteArray()),
                InverseQ = CopyAndReverse(qInv.ToByteArray()),
            };
        }


        /// <summary>
        /// Calculates the modular multiplicative inverse of <paramref name="a"/> modulo <paramref name="m"/>
        /// using the extended Euclidean algorithm.
        /// </summary>
        /// <remarks>
        /// This implementation comes from the pseudocode defining the inverse(a, n) function at
        /// https://en.wikipedia.org/wiki/Extended_Euclidean_algorithm
        /// </remarks>
        public static BigInteger ModInverse(BigInteger a, BigInteger n) {
            BigInteger t = 0, nt = 1, r = n, nr = a;

            if (n < 0) {
                n = -n;
            }

            if (a < 0) {
                a = n - (-a % n);
            }

            while (nr != 0) {
                var quot = r / nr;

                var tmp = nt; nt = t - quot * nt; t = tmp;
                tmp = nr; nr = r - quot * nr; r = tmp;
            }

            if (r > 1) throw new ArgumentException(nameof(a) + " is not convertible.");
            if (t < 0) t = t + n;
            return t;
        }

        private static byte[] CopyAndReverse(byte[] data) {
            byte[] reversed = new byte[data.Length];
            Array.Copy(data, 0, reversed, 0, data.Length);
            Array.Reverse(reversed);
            return reversed;
        }
    }
}

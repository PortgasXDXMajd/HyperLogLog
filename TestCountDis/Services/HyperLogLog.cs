using System.Security.Cryptography;
using System.Text;

public class HyperLogLog
{
    private int m;
    private int[] M;

    public HyperLogLog(int m)
    {
        this.m = m;
        M = new int[m];
    }

    public void Add(string value)
    {
        byte[] data = Encoding.UTF8.GetBytes(value);
        byte[] hash = SHA256.Create().ComputeHash(data);

        int index = (int)(BitConverter.ToUInt32(hash, 0) % m);
        int w = (int)(BitConverter.ToUInt32(hash, 4) % (1 << 31));

        M[index] = Math.Max(M[index], Rho(w));
    }

    public double Count()
    {
        double alphaM;

        if (m == 16)
        {
            alphaM = 0.673;
        }
        else if (m == 32)
        {
            alphaM = 0.697;
        }
        else if (m == 64)
        {
            alphaM = 0.709;
        }
        else
        {
            alphaM = 0.7213 / (1 + 1.079 / m);
        }

        double Z = 0;
        for (int i = 0; i < m; i++)
        {
            Z += 1.0 / (1 << M[i]);
        }

        double E = alphaM * m * m / Z;

        if (E <= 2.5 * m)
        {
            int V = 0;
            for (int i = 0; i < m; i++)
            {
                if (M[i] == 0)
                {
                    V++;
                }
            }
            if (V > 0)
            {
                E = m * Math.Log(m / (double)V);
            }
        }
        else if (E > (1 / 30.0) * Math.Pow(2, 32))
        {
            E = -Math.Pow(2, 32) * Math.Log(1 - E / Math.Pow(2, 32));
        }

        return (int)E;
    }

    private int Rho(int w)
    {
        int rho = 1;
        while ((w & 1) == 0 && rho <= 31)
        {
            w >>= 1;
            rho++;
        }
        return rho;
    }
}
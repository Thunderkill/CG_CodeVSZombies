namespace CG_CodeVsZombies2.Utils
{
    public static class NumberUtils
    {
        public static int Fibonacci(int n)
        {
            if (n <= 1) return n;
            int fib = 1;
            int prevFib = 1;

            for (int i = 2; i < n; i++)
            {
                int temp = fib;
                fib += prevFib;
                prevFib = temp;
            }

            return fib;
        }
    }
}
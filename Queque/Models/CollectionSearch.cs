internal static class CollectionSearch
{
    /// <summary>
    /// Busca un elemento en una colección ordenada y retorna el índice donde se encuentra ese elemento
    /// </summary>
    /// <param name="input">Colección de números</param>
    /// <param name="value">Valor a buscar</param>
    /// <returns>Retorna el la posición del elemento en casco contrario retorna -1</returns>
    public static int Binary(int[] input, int value)
    {
        int l = 0;
        int r = input.Length - 1;
        while (l <= r)
        {
            int mid = (l + r) / 2;
            if (input[mid] < value)
            {
                l = mid + 1;
            }
            else if (input[mid] > value)
            {
                r = mid - 1;
            }
            else
                return mid;
        }
        return -1;
    }
    /// <summary>
    /// Busca un elemento en una colección ordenada y retorna el índice donde se encuentra ese elemento de manera recursiva
    /// </summary>
    /// <param name="input">Colección de números</param>
    /// <param name="value">Valor a buscar</param>
    /// <param name="start">Posición de búsqueda inicial</param>
    /// <param name="length">Tamaño total de la colección -1</param>
    /// <returns></returns>
    public static int BinaryRecursive(int[] input, int value, int start, int length)
    {
        if (start > length)
        {
            return -1;
        }

        int mid = (start + length) / 2;
        return input[mid] == value ? mid : input[mid] < value ? BinaryRecursive(input, value, mid + 1, length) : BinaryRecursive(input, value, start, mid - 1);
    }
    /// <summary>
    /// Busca un elemento en una colección ordenada y retorna el índice donde se encuentra ese elemento
    /// </summary>
    /// <param name="input">Colección de números</param>
    /// <param name="value">Valor a buscar</param>
    /// <returns>Retorna el la posición del elemento en casco contrario retorna -1</returns>
    public static int Lineal(int[] input, int value)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == value) return value;
            else if (input[i] > value) break;
        }

        return -1;
    }
}
/* Búsqueda valor = 1 todo en un proceso
BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4046/22H2/2022Update)
Intel Core i3-2350M CPU 2.30GHz (Sandy Bridge), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.102
  [Host] : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX

Job=InProcess  Toolchain=InProcessEmitToolchain  InvocationCount=1 IterationCount=100_000  UnrollFactor=1

| Method                   | Mean     | Error   | StdDev   | Median      | Max        | Min         | Iterations | Allocated |
|------------------------- |---------:|--------:|---------:|------------:|-----------:|------------:|-----------:|----------:|
| BúsquedaLineal           | 192.4 ns | 2.90 ns | 275.1 ns |   0.0000 ns | 1,400.0 ns |   0.0000 ns |   97,370.0 |     688 B |
| BúsquedaBinaria          | 463.6 ns | 4.42 ns | 417.1 ns | 300.0000 ns | 1,800.0 ns |   0.0000 ns |   96,476.0 |     688 B |
| BúsquedaBinariaRecursiva | 617.5 ns | 4.29 ns | 403.4 ns | 400.0000 ns | 1,800.0 ns | 100.0000 ns |   95,656.0 |     352 B |
| BúsquedaBinariaNativa    | 578.8 ns | 5.06 ns | 477.2 ns | 500.0000 ns | 2,200.0 ns |   0.0000 ns |   96,387.0 |     352 B |
| BúsquedaIndexOf          | 616.5 ns | 4.77 ns | 450.9 ns | 500.0000 ns | 2,200.0 ns |   0.0000 ns |   96,880.0 |     400 B |
*/
/*                                  Multiproceso
BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4046/22H2/2022Update)
Intel Core i3-2350M CPU 2.30GHz (Sandy Bridge), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.102
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX
  Job-QSYWBZ : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX

InvocationCount=1  IterationCount=100_000  UnrollFactor=1

| Method                   | Mean      | Error    | StdDev    | Median      | Max        | Min         | Iterations | Allocated |
|------------------------- |----------:|---------:|----------:|------------:|-----------:|------------:|-----------:|----------:|
| BúsquedaLineal           |  83.44 ns | 1.444 ns | 136.60 ns |   0.0000 ns |   600.0 ns |   0.0000 ns |   96,887.0 |     400 B |
| BúsquedaBinaria          | 301.04 ns | 2.809 ns | 264.23 ns | 200.0000 ns | 1,100.0 ns |   0.0000 ns |   95,843.0 |     112 B |
| BúsquedaBinariaRecursiva | 387.72 ns | 2.817 ns | 265.22 ns | 300.0000 ns | 1,200.0 ns | 100.0000 ns |   96,010.0 |     400 B |
| BúsquedaBinariaNativa    | 499.17 ns | 2.861 ns | 267.66 ns | 400.0000 ns | 1,300.0 ns | 100.0000 ns |   94,757.0 |     400 B |
| BúsquedaIndexOf          | 230.11 ns | 2.528 ns | 239.10 ns | 200.0000 ns | 1,000.0 ns |   0.0000 ns |   96,885.0 |     400 B |
*/
/*     Búsqueda valor = 3920 multiprocesos
BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4046/22H2/2022Update)
Intel Core i3-2350M CPU 2.30GHz (Sandy Bridge), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.102
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX
  Job-XIJMBC : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX

InvocationCount=1  IterationCount=10_000  UnrollFactor=1

| Method                   | Mean       | Error    | StdDev     | Median     | Max         | Min           | Iterations | Allocated |
|------------------------- |-----------:|---------:|-----------:|-----------:|------------:|--------------:|-----------:|----------:|
| BúsquedaLineal           | 8,914.3 ns | 61.40 ns | 1,716.8 ns | 8,700.0 ns | 16,200.0 ns | 6,800.0000 ns |    8,471.0 |     400 B |
| BúsquedaBinaria          |   421.6 ns | 12.16 ns |   360.6 ns |   400.0 ns |  1,600.0 ns |     0.0000 ns |    9,536.0 |      64 B |
| BúsquedaBinariaRecursiva |   369.8 ns | 11.03 ns |   325.9 ns |   300.0 ns |  1,300.0 ns |     0.0000 ns |    9,459.0 |     400 B |
| BúsquedaBinariaNativa    | 1,075.0 ns | 14.17 ns |   417.6 ns | 1,100.0 ns |  2,300.0 ns |   400.0000 ns |    9,416.0 |     400 B |
| BúsquedaIndexOf          | 2,383.0 ns | 30.20 ns |   871.9 ns | 2,400.0 ns |  5,200.0 ns | 1,100.0000 ns |    9,030.0 |     400 B |
*/
/* Búsqueda valor = aleatorio por iteración
BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4046/22H2/2022Update)
Intel Core i3-2350M CPU 2.30GHz (Sandy Bridge), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.102
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX
  Job-JCOMFJ : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX

InvocationCount=1  IterationCount=10_000  UnrollFactor=1

| Method                   | Mean       | Error    | StdDev     | Median     | Max         | Min           | Iterations | Allocated |
|------------------------- |-----------:|---------:|-----------:|-----------:|------------:|--------------:|-----------:|----------:|
| BúsquedaLineal           |   146.8 ns |  6.12 ns |   176.4 ns |   100.0 ns |    700.0 ns |     0.0000 ns |    8,994.0 |     400 B |
| BúsquedaBinaria          |   666.3 ns | 13.76 ns |   404.7 ns |   600.0 ns |  1,900.0 ns |     0.0000 ns |    9,368.0 |     400 B |
| BúsquedaBinariaRecursiva |   682.4 ns | 12.15 ns |   356.5 ns |   700.0 ns |  1,700.0 ns |     0.0000 ns |    9,330.0 |     400 B |
| BúsquedaBinariaNativa    |   817.4 ns | 14.24 ns |   411.5 ns |   800.0 ns |  2,100.0 ns |     0.0000 ns |    9,043.0 |      64 B |
| BúsquedaIndexOf          | 5,430.0 ns | 66.32 ns | 1,943.7 ns | 4,500.0 ns | 11,600.0 ns | 3,700.0000 ns |    9,306.0 |      64 B |
*/
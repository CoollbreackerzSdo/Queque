//Matrices dispersas
//
//  [][][][][][][][]
//  [][][][][][][][]
//  [][][][][][][][]
//  [][][][][][][][]
//  [][][][][][][][]
//
using QueQue.Models;

SparseMatrix<int> matrix = new int[200, 5];

SparseMatrix<int> matrix2 = new(10, 5);

for (int i = 0; i < matrix.Columns; i++)
    for (int j = 0; j < matrix.Rows; j++)
        matrix[i, j] = Random.Shared.Next(0, 2000);

for (int i = 0; i < matrix2.Columns; i++)
    for (int j = 0; j < matrix2.Rows; j++)
        matrix2[i, j] = Random.Shared.Next(0, 20);

matrix.Transmute();

var matrix3 = matrix - matrix2;

Console.WriteLine(matrix3.GetEnumerator().Sum());
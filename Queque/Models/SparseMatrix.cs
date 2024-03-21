using System.Collections;

namespace Queque.Models;

//public sealed class SparseMatrix<TValue>(int _MaxColumns, int _MaxRows) : ICollection<MatrixNode<int, int, TValue>>
//{
//    public void Add(MatrixNode<int, int, TValue> item)
//    {
//        if (item.Index.Item1 > _MaxColumns || item.Index.Item2 > _MaxRows) return;

//    }
//    public void Clear()
//    {
//        throw new NotImplementedException();
//    }
//    public bool Contains(MatrixNode<int, int, TValue> item)
//    {
//        throw new NotImplementedException();
//    }

//    public void CopyTo(MatrixNode<int, int, TValue>[] array, int arrayIndex)
//    {
//        throw new NotImplementedException();
//    }

//    public IEnumerator<MatrixNode<int, int, TValue>> GetEnumerator()
//    {
//        throw new NotImplementedException();
//    }

//    public bool Remove(MatrixNode<int, int, TValue> item)
//    {
//        throw new NotImplementedException();
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        throw new NotImplementedException();
//    }
//    //public Dictionary<>
//    //public int Count { get; private set; }
//    public bool IsReadOnly { get; } = true;
//}
/// <summary>
/// Representa un nodo en una estructura de matriz.
/// </summary>
/// <typeparam name="TRow">El tipo del índice de fila.</typeparam>
/// <typeparam name="TColumn">El tipo del índice de columna.</typeparam>
/// <typeparam name="TValue">El tipo del valor almacenado en el nodo.</typeparam>
public interface IMatrixNode<TRow, TColumn, TValue>
{
    /// <summary>
    /// Obtiene el índice del nodo en la matriz.
    /// </summary>
    /// <value>El índice del nodo.</value>
    ValueTuple<TRow, TColumn> Index { get; }
    /// <summary>
    /// Obtiene el valor almacenado en el nodo.
    /// </summary>
    /// <value>El valor almacenado en el nodo.</value>
    TValue Value { get; }
}
/// <summary>
/// Representa una nodo de matriz base
/// </summary>
/// <typeparam name="TRow">El tipo del índice de fila.</typeparam>
/// <typeparam name="TColumn">El tipo del índice de columna.</typeparam>
/// <typeparam name="TValue">El tipo del valor almacenado en el nodo.</typeparam>
public class MatrixNode<TRow, TColumn, TValue> : IMatrixNode<TRow, TColumn, TValue?>
{
    public (TRow, TColumn) Index { get; set; }
    public TValue? Value { get; set; }
}
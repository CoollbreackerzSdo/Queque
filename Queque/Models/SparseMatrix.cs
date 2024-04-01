using System.Numerics;

namespace QueQue.Models;

public class SparseMatrix<T> where T : INumber<T?>
{
    public SparseMatrix(int _Columns, int _Rows)
    {
        Columns = _Columns;
        Rows = _Rows;
        _root = new T[_Columns, _Rows];
    }
    public SparseMatrix(T[,] values)
    {
        Columns = values.GetLength(0);
        Rows = values.GetLength(1);
        _root = values;
    }
    /// <summary>
    /// Elimina todos los elementos existentes en la colección
    /// </summary>
    public void Clear() => _root = new T[Columns, Rows];
    /// <summary>
    /// Busca si un elemento existe dentro de la colección
    /// </summary>
    /// <param name="item">El elemento a buscar</param>
    /// <returns>Retorna true si existe el elemento en caso contrario false</returns>
    public bool Contains(T item)
    {
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (this[i, j] is null) continue;
                else if (this[i, j] == item) return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Elimina un elemento especifico en la colección
    /// </summary>
    /// <param name="item">El elemento a eliminar</param>
    /// <returns>Retorna true si el elemento fue encontrado y eliminado en caso contrario false</returns>
    public bool Remove(T item)
    {
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (this[i, j] is null) continue;
                else if (this[i, j] == item)
                {
                    this[i, j] = default;
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// Elimina todos los elementos específicos en la colección
    /// </summary>
    /// <param name="item">El Elemento a eliminar</param>
    public void RemoveAll(T item)
    {
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (this[i, j] is null) continue;
                else if (this[i, j] == item)
                {
                    this[i, j] = default;
                }
            }
        }
    }
    /// <summary>
    /// Convierte las filas -> columnas y las columnas -> filas modificando la colección
    /// </summary>
    public void Transmute()
    {
        lock (this)
        {
            (Columns, Rows) = (Rows, Columns);
            var target = new T?[Columns, Rows];
            for (int i = 0; i < Columns; i++)  
            {
                for (int j = 0; j < Rows; j++)
                {
                    target[i, j] = this[j, i];
                }
            }

            _root = target;
        }
    }
    public IEnumerable<T?> GetEnumerator() => _root.Cast<T?>();
    public static implicit operator SparseMatrix<T>(T[,] values) => new(values);
    public static SparseMatrix<T> operator +(SparseMatrix<T> sourceOne, SparseMatrix<T> sourceTow)
    {
        var target = new SparseMatrix<T>(Math.Max(sourceOne.Columns, sourceTow.Columns), Math.Max(sourceOne.Rows, sourceTow.Rows));

        for (int i = 0; i < target.Columns; i++)
        {
            for (int j = 0; j < target.Rows; j++)
            {
                if (i < sourceOne.Columns && i < sourceTow.Columns && j < sourceTow.Rows && j < sourceOne.Rows)
                {
                    target[i, j] = sourceOne[i, j] + sourceTow[i, j];
                    continue;
                }
                else if (i < sourceOne.Columns && j < sourceOne.Rows)
                {
                    target[i, j] = sourceOne[i, j];
                    continue;
                }
                else if (i < sourceTow.Columns && j < sourceTow.Rows)
                {
                    target[i, j] = sourceTow[i, j];
                    continue;
                }
            }
        }
        return target;
    }
    public static SparseMatrix<T> operator *(SparseMatrix<T> sourceOne, SparseMatrix<T> sourceTow)
    {
        var target = new SparseMatrix<T>(sourceOne.Columns > sourceTow.Columns ? sourceOne.Columns : sourceTow.Columns,
            sourceOne.Rows > sourceTow.Rows ? sourceOne.Rows : sourceTow.Rows);

        for (int i = 0; i < target.Columns; i++)
        {
            for (int j = 0; j < target.Rows; j++)
            {
                if (i < sourceOne.Columns && i < sourceTow.Columns && j < sourceTow.Rows && j < sourceOne.Rows)
                {
                    target[i, j] = sourceOne[i, j] * sourceTow[i, j];
                    continue;
                }
                else if (i < sourceOne.Columns && j < sourceOne.Rows)
                {
                    target[i, j] = sourceOne[i, j];
                    continue;
                }
                else if (i < sourceTow.Columns && j < sourceTow.Rows)
                {
                    target[i, j] = sourceTow[i, j];
                    continue;
                }
            }
        }
        return target;
    }
    public static SparseMatrix<T> operator -(SparseMatrix<T> sourceOne, SparseMatrix<T> sourceTow)
    {
        var target = new SparseMatrix<T>(sourceOne.Columns > sourceTow.Columns ? sourceOne.Columns : sourceTow.Columns,
            sourceOne.Rows > sourceTow.Rows ? sourceOne.Rows : sourceTow.Rows);

        for (int i = 0; i < target.Columns; i++)
        {
            for (int j = 0; j < target.Rows; j++)
            {
                if (i < sourceOne.Columns && i < sourceTow.Columns && j < sourceTow.Rows && j < sourceOne.Rows)
                {
                    target[i, j] = sourceOne[i, j] - sourceTow[i, j];
                    continue;
                }
                else if (i < sourceOne.Columns && j < sourceOne.Rows)
                {
                    target[i, j] = sourceOne[i, j];
                    continue;
                }
                else if (i < sourceTow.Columns && j < sourceTow.Rows)
                {
                    target[i, j] = sourceTow[i, j];
                    continue;
                }
            }
        }
        return target;
    }
    public static SparseMatrix<T> operator /(SparseMatrix<T> sourceOne, SparseMatrix<T> sourceTow)
    {
        var target = new SparseMatrix<T>(sourceOne.Columns > sourceTow.Columns ? sourceOne.Columns : sourceTow.Columns,
            sourceOne.Rows > sourceTow.Rows ? sourceOne.Rows : sourceTow.Rows);

        for (int i = 0; i < target.Columns; i++)
        {
            for (int j = 0; j < target.Rows; j++)
            {
                if (i < sourceOne.Columns && i < sourceTow.Columns && j < sourceTow.Rows && j < sourceOne.Rows)
                {
                    if(sourceOne[i, j] ==  T.Zero || sourceTow[i, j] == T.Zero)
                    {
                        target[i, j] = T.Zero;
                        continue;
                    }
                    target[i, j] = sourceOne[i, j] / sourceTow[i, j];
                    continue;
                }
                else if (i < sourceOne.Columns && j < sourceOne.Rows)
                {
                    target[i, j] = sourceOne[i, j];
                    continue;
                }
                else if (i < sourceTow.Columns && j < sourceTow.Rows)
                {
                    target[i, j] = sourceTow[i, j];
                    continue;
                }
            }
        }
        return target;
    }
    public T? this[int column, int row]
    {
        get
        {
            return _root[column, row];
        }
        set
        {
            _root[column, row] = value;
        }
    }
    /// <summary>
    /// Numero de columnas en la collación
    /// </summary>
    public int Columns { get; private set; }
    /// <summary>
    /// Numero de filas en la collación
    /// </summary>
    public int Rows { get; private set; }
    /// <summary>
    /// Cantidad de elementos en la collación
    /// </summary>
    public int Count { get => _root.Length; }
    private T?[,] _root;
}
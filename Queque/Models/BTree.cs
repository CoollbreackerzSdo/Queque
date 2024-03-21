using System.Collections;

namespace Queque.Models;
/// <summary>
/// Representa un árbol b de tipo genérico
/// </summary>
/// <typeparam name="T"></typeparam>
public class BTreeV1<T> : IEnumerable<T>, ICollection<T>, IDisposable
    where T : IComparable
{
    public BTreeV1(int maxKeysPerNode)
    {
        if (maxKeysPerNode < 3) throw new Exception("El numero mínimo de claves permitida es de 3");

         _maxKeysPerNode = maxKeysPerNode;
        _minKeysPerNode = maxKeysPerNode / 2;
    }
    public void Add(T item)
    {
        //si el nodo es nulo inicializamos uno nuevo
        if (_root is null)
        {
            _root = new BTreeNodeV1<T>(_maxKeysPerNode, null) { Keys = { [0] = item } };
            _root.KeyCount++;
            Count++;
            return;
        }

        var leafToInsert = BTreeV1<T>.FindInsertionLeaf(_root, ref item);
        AddAndSplit(leafToInsert, ref item, null, null);
        Count++;
    }
    public void Clear() => Dispose();
    public bool Contains(T item)
    {
        if (_root is null) return false;
        return Exist(_root,ref item);
    }
    public void CopyTo(T[] array, int arrayIndex)
    {
        return;
    }
    public void Dispose()
    {
        // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    public IEnumerator<T> GetEnumerator() => new BTreeV1Enumerator<T>(_root!);
    public bool Remove(T item)
    {
        var node = BTreeV1<T>.FindDeletionNode(_root, ref item);
        if (node is null) return false;

        for (var i = 0; i < node.KeyCount; i++)
        {
            if (item.CompareTo(node.Keys[i]) != 0) continue;

            //si el nodo es hoja y no hay desbordamiento
            //luego simplemente eliminamos el nodo
            if (node.IsLeaf)
            {
                BTreeV1<T>.RemoveAt(node.Keys, i);
                node.KeyCount--;

                Balance(node);
            }
            else
            {
                //reemplazar con el nodo máximo del árbol izquierdo
                var maxNode = BTreeV1<T>.FindMaxNode(node.Children[i]);
                node.Keys[i] = maxNode.Keys[maxNode.KeyCount - 1];

                BTreeV1<T>.RemoveAt(maxNode.Keys, maxNode.KeyCount - 1);
                maxNode.KeyCount--;

                Balance(maxNode);
            }

            Count--;
        }
        return true;
    }
    /// <summary>
    /// Inserta un nuevo nodo y divide el mismo
    /// </summary>
    /// <param name="leafToInsert"></param>
    /// <param name="item"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void AddAndSplit(BTreeNodeV1<T> node, ref readonly T item, BTreeNodeV1<T>? nodeLeft, BTreeNodeV1<T>? nodeRight)
    {
        //si el nodo actual es nolo pues inicializamos la raiz siendo el nuevo nodo
        if (node is null)
        {
            node = new BTreeNodeV1<T>(_maxKeysPerNode, null);
            _root = node;
        }

        //Si el nodo tiene espacio para insertar uno nuevo pues inicializamos uno nuevo e insertamos el nuevo elemento y reordenamos el nodo
        if (node.KeyCount != _maxKeysPerNode)
        {
            BTreeV1<T>.InsertToNotFullNode(node, in item, nodeLeft, nodeRight);
            return;
        }

        //si el nodo está lleno entonces dividimos el nodo
        //y luego inserte una nueva mediana en el padre.

        //divide los valores del nodo actual + nuevo NodeV1 como sub nodos izquierdo y derecho
        var left = new BTreeNodeV1<T>(_maxKeysPerNode, null);
        var right = new BTreeNodeV1<T>(_maxKeysPerNode, null);

        //mediana del NodeV1 actual
        var currentMedianIndex = node.GetMedianIndex();

        //inicia el nodo actual en consideración a la izquierda
        var currentNode = left;
        var currentNodeIndex = 0;

        //la nueva mediana también incorpora el nuevo valor a la cuenta
        var newMedian = default(T);
        var newMedianSet = false;
        var itemInserted = false;

        //realizar un seguimiento de cada inserción
        var insertionCount = 0;

        //insertar elemento y valores existentes en orden
        //a los nodos izquierdo y derecho
        //establecer nueva mediana durante la clasificación
        for (var i = 0; i < node.KeyCount; i++)
        {
            //si el recuento de inserciones alcanzó la nueva mediana
            //establecemos la nueva mediana eligiendo el siguiente valor más pequeño
            if (!newMedianSet && insertionCount == currentMedianIndex)
            {
                newMedianSet = true;

                //la mediana puede ser el nuevo valor o node.keys[i] (siguiente clave de nodo)
                //lo que sea menor
                if (!itemInserted && item.CompareTo(node.Keys[i]) < 0)
                {
                    //la mediana es el nuevo valor a insertar
                    newMedian = item;
                    itemInserted = true;

                    if (nodeLeft != null) BTreeV1<T>.SetChild(currentNode, currentNode.KeyCount, nodeLeft);

                    //ahora llena el nodo derecho
                    currentNode = right;
                    currentNodeIndex = 0;

                    if (nodeRight != null) BTreeV1<T>.SetChild(currentNode, 0, nodeRight);

                    i--;
                    insertionCount++;
                    continue;
                }

                //la mediana es el siguiente nodo
                newMedian = node.Keys[i];

                //ahora llena el nodo derecho
                currentNode = right;
                currentNodeIndex = 0;

                continue;
            }

            //elige el más pequeño entre item y node.Keys[i]
            //e insertar en el nodo actual (nodos izquierdo y derecho)
            //si ya se insertó un nuevo valor, simplemente copie desde node.Keys en secuencia
            //dado que node.Keys ya está ordenado, debería estar bien
            if (itemInserted || node.Keys[i].CompareTo(item) < 0)
            {
                currentNode.Keys[currentNodeIndex] = node.Keys[i];
                currentNode.KeyCount++;

                //si el hijo está configurado, no lo vuelvas a configurar
                //el hijo ya fue configurado por el último nodo derecho o el último nodo
                if (currentNode.Children[currentNodeIndex] is null)
                    BTreeV1<T>.SetChild(currentNode, currentNodeIndex, node.Children[i]);

                BTreeV1<T>.SetChild(currentNode, currentNodeIndex + 1, node.Children[i + 1]);
            }
            else
            {
                currentNode.Keys[currentNodeIndex] = item;
                currentNode.KeyCount++;

                BTreeV1<T>.SetChild(currentNode, currentNodeIndex, nodeLeft!);
                BTreeV1<T>.SetChild(currentNode, currentNodeIndex + 1, nodeRight!);

                i--;
                itemInserted = true;
            }

            currentNodeIndex++;
            insertionCount++;
        }

        //podría ser que la nueva clave sea la mejor
        // entonces insertar al final
        if (!itemInserted)
        {
            currentNode.Keys[currentNodeIndex] = item;
            currentNode.KeyCount++;

            BTreeV1<T>.SetChild(currentNode, currentNodeIndex, nodeLeft!);
            BTreeV1<T>.SetChild(currentNode, currentNodeIndex + 1, nodeRight!);
        }

        //insertar elemento de desbordamiento (newMedian) al padre
        var parent = node.Parent;
        AddAndSplit(parent!, ref newMedian!, left, right);
    }
    private static void SetChild(BTreeNodeV1<T> currentNode, int keyCount, BTreeNodeV1<T> child)
    {
        currentNode.Children[keyCount] = child;

        if (child is null) return;

        child.Parent = currentNode;
        child.Index = keyCount;
    }
    private static void InsertToNotFullNode(BTreeNodeV1<T> node, ref readonly T item, BTreeNodeV1<T>? nodeLeft, BTreeNodeV1<T>? nodeRight)
    {
        var inserted = false;

        //insertar en orden
        for (var i = 0; i < node.KeyCount; i++)
        {
            if (item.CompareTo(node.Keys[i]) >= 0) continue;

            BTreeV1<T>.AddAt(node.Keys, i, item);
            node.KeyCount++;

            // Insertar hijos si los hay
            BTreeV1<T>.SetChild(node, i, nodeLeft!);
            BTreeV1<T>.InsertChild(node, i + 1, nodeRight);


            inserted = true;
            break;
        }

        //nuevoValor es el mayor
        //el elemento debe insertarse al final y luego
        if (inserted) return;

        node.Keys[node.KeyCount] = item;
        node.KeyCount++;

        BTreeV1<T>.SetChild(node, node.KeyCount - 1, nodeLeft!);
        BTreeV1<T>.SetChild(node, node.KeyCount, nodeRight!);
    }
    /// <summary>
    /// Cambie la matriz a la derecha en el índice para dejar espacio para una nueva inserción
    /// Y luego insertar en el índice
    /// Asume que la matriz tiene al menos un índice vacío al final
    /// </summary>
    /// <typeparam name="Ts"></typeparam>
    /// <param name="keys"></param>
    /// <param name="index"></param>
    /// <param name="item"></param>
    private static void AddAt<Ts>(Ts[] keys, int index, Ts item)
    {
        //desplaza los elementos a la derecha un índice desde el índice
        Array.Copy(keys, index, keys, index + 1, keys.Length - index - 1);
        //ahora establecemos el valor
        keys[index] = item;
    }
    /// <summary>
    /// Inserta un nodo hijo en el indice especificado
    /// </summary>
    /// <param name="node"></param>
    /// <param name="childIndex"></param>
    /// <param name="child"></param>
    private static void InsertChild(BTreeNodeV1<T> node, int childIndex, BTreeNodeV1<T>? child)
    {
        BTreeV1<T>.AddAt(node.Children, childIndex, child);

        if (child != null) child.Parent = node;

        //actualizar los indices
        for (var i = childIndex; i <= node.KeyCount; i++)
            if (node.Children[i] != null)
                node.Children[i].Index = i;
    }
    /// <summary>
    /// Busca un espacio donde insertar un nodo como hoja de manera recursiva
    /// </summary>
    /// <param name="root"></param>
    /// <param name="item"></param>
    /// <returns></returns>                                                     
    /// <exception cref="NotImplementedException"></exception>
    private static BTreeNodeV1<T> FindInsertionLeaf(BTreeNodeV1<T> node, ref readonly T item)
    {
        //Si ya estamos en una hoja retornamos la referencia del nodo
        if (node.IsLeaf) return node;

        for (var i = 0; i < node.KeyCount; i++)
        {
            //Busca el nodo menor a la izquierda
            if (item.CompareTo(node.Keys[i]) < 0) return BTreeV1<T>.FindInsertionLeaf(node.Children[i], in item);
            //Verifica si el nodo es mayor a la derecha
            if (node.KeyCount == i + 1) return BTreeV1<T>.FindInsertionLeaf(node.Children[i + 1], in item);
        }

        return node;
    }
    /// <summary>
    /// Busaca si existe un elemento en un nodo
    /// </summary>
    /// <param name="node"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private static bool Exist(BTreeNodeV1<T> node, ref readonly T item)
    {
        if (node.IsLeaf)
        {
            for (var i = 0; i < node.KeyCount; i++)
                if (item.CompareTo(node.Keys[i]) == 0)
                    return true;
        }
        else
        {                                             
            for (var i = 0; i < node.KeyCount; i++)
            {
                if (item.CompareTo(node.Keys[i]) == 0) return true;

                if (item.CompareTo(node.Keys[i]) < 0) return BTreeV1<T>.Exist(node.Children[i],in item);

                if (node.KeyCount == i + 1) return BTreeV1<T>.Exist(node.Children[i + 1],in item);
            }
        }

        return false;
    }
    /// <summary>
    /// Equilibra un nodo al que le faltan claves mediante rotaciones o fusión
    /// </summary>
    /// <param name="node"></param>
    private void Balance(BTreeNodeV1<T> node)
    {
        if (node == _root || node.KeyCount >= _minKeysPerNode) return;

        var rightSibling = BTreeV1<T>.GetRightSibling(node);

        if (rightSibling != null
            && rightSibling.KeyCount > _minKeysPerNode)
        {
            BTreeV1<T>.LeftRotate(node, rightSibling);
            return;
        }

        var leftSibling = BTreeV1<T>.GetLeftSibling(node);

        if (leftSibling != null
            && leftSibling.KeyCount > _minKeysPerNode)
        {
            BTreeV1<T>.RightRotate(leftSibling, node);
            return;
        }

        if (rightSibling != null)
            Sandwich(node, rightSibling);
        else
            Sandwich(leftSibling!, node);
    }
    /// <summary>
    /// Fusionar dos hermanos adyacentes a un nodo
    /// </summary>
    /// <param name="leftSibling"></param>
    /// <param name="rightSibling"></param>
    private void Sandwich(BTreeNodeV1<T> leftSibling, BTreeNodeV1<T> rightSibling)
    {
        var separatorIndex = BTreeV1<T>.GetNextSeparatorIndex(leftSibling);
        var parent = leftSibling.Parent;

        var newNode = new BTreeNodeV1<T>(_maxKeysPerNode, leftSibling.Parent);
        var newIndex = 0;

        for (var i = 0; i < leftSibling.KeyCount; i++)
        {
            newNode.Keys[newIndex] = leftSibling.Keys[i];

            if (leftSibling.Children[i] != null) BTreeV1<T>.SetChild(newNode, newIndex, leftSibling.Children[i]);

            if (leftSibling.Children[i + 1] != null) BTreeV1<T>.SetChild(newNode, newIndex + 1, leftSibling.Children[i + 1]);

            newIndex++;
        }

        // Caso especial cuando la hermana izquierda está vacía
        if (leftSibling.KeyCount == 0 && leftSibling.Children[0] != null)
            BTreeV1<T>.SetChild(newNode, newIndex, leftSibling.Children[0]);

        newNode.Keys[newIndex] = parent!.Keys[separatorIndex];
        newIndex++;

        for (var i = 0; i < rightSibling.KeyCount; i++)
        {
            newNode.Keys[newIndex] = rightSibling.Keys[i];

            if (rightSibling.Children[i] != null) BTreeV1<T>.SetChild(newNode, newIndex, rightSibling.Children[i]);

            if (rightSibling.Children[i + 1] != null) BTreeV1<T>.SetChild(newNode, newIndex + 1, rightSibling.Children[i + 1]);

            newIndex++;
        }

        // Caso especial cuando la hermana izquierda está vacía 
        if (rightSibling.KeyCount == 0 && rightSibling.Children[0] != null)
            BTreeV1<T>.SetChild(newNode, newIndex, rightSibling.Children[0]);

        newNode.KeyCount = newIndex;
        BTreeV1<T>.SetChild(parent, separatorIndex, newNode);
        RemoveAt(parent.Keys, separatorIndex);
        parent.KeyCount--;

        BTreeV1<T>.RemoveChild(parent, separatorIndex + 1);


        if (parent.KeyCount == 0
            && parent == _root)
        {
            _root = newNode;
            _root.Parent = null;

            if (_root.KeyCount == 0) _root = null;

            return;
        }

        if (parent.KeyCount < _minKeysPerNode) Balance(parent);
    }
    /// <summary>
    /// Remueve un elemento del indice especificado
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="childIndex"></param>
    private static void RemoveChild(BTreeNodeV1<T> parent, int childIndex)
    {
        RemoveAt(parent.Children, childIndex);

        //update indices
        for (var i = childIndex; i <= parent.KeyCount; i++)
            if (parent.Children[i] != null)
                parent.Children[i].Index = i;
    }
    private static int GetNextSeparatorIndex(BTreeNodeV1<T> node)
    {
        var parent = node.Parent;

        if (node.Index == 0) return 0;

        if (node.Index == parent!.KeyCount) return node.Index - 1;

        return node.Index;
    }
    /// <summary>
    /// hacer una rotación a la derecha
    /// </summary>
    /// <param name="leftSibling"></param>
    /// <param name="node"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void RightRotate(BTreeNodeV1<T> leftSibling, BTreeNodeV1<T> rightSibling)
    {
        var parentIndex = BTreeV1<T>.GetNextSeparatorIndex(leftSibling);

        AddAt(rightSibling.Keys, 0, rightSibling.Parent!.Keys[parentIndex]);
        rightSibling.KeyCount++;

        BTreeV1<T>.InsertChild(rightSibling, 0, leftSibling.Children[leftSibling.KeyCount]);

        rightSibling.Parent.Keys[parentIndex] = leftSibling.Keys[leftSibling.KeyCount - 1];

        RemoveAt(leftSibling.Keys, leftSibling.KeyCount - 1);
        leftSibling.KeyCount--;

        BTreeV1<T>.RemoveChild(leftSibling, leftSibling.KeyCount + 1);
    }
    /// <summary>
    /// hacer una rotación a la izquierda
    /// </summary>
    /// <param name="leftSibling"></param>
    /// <param name="rightSibling"></param>
    private static void LeftRotate(BTreeNodeV1<T> leftSibling, BTreeNodeV1<T> rightSibling)
    {
        var parentIndex = BTreeV1<T>.GetNextSeparatorIndex(leftSibling);
        leftSibling.Keys[leftSibling.KeyCount] = leftSibling.Parent!.Keys[parentIndex];
        leftSibling.KeyCount++;

        BTreeV1<T>.SetChild(leftSibling, leftSibling.KeyCount, rightSibling.Children[0]);


        leftSibling.Parent.Keys[parentIndex] = rightSibling.Keys[0];

        RemoveAt(rightSibling.Keys, 0);
        rightSibling.KeyCount--;

        BTreeV1<T>.RemoveChild(rightSibling, 0);
    }
    /// <summary>
    /// obtener el nodo hermano izquierdo
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static BTreeNodeV1<T>? GetLeftSibling(BTreeNodeV1<T> node)
        => node.Index == 0 ? null : node.Parent.Children[node.Index - 1];
    /// <summary>
    /// Obtener el nodo hermano correcto
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static BTreeNodeV1<T>? GetRightSibling(BTreeNodeV1<T> node)
    {
        var parent = node.Parent;

        return node.Index == parent!.KeyCount ? null : parent.Children[node.Index + 1];
    }
    private static void RemoveAt<Ts>(Ts[] keys, int index) 
        => Array.Copy(keys, index + 1, keys, index, keys.Length - index - 1);
    private static BTreeNodeV1<T>? FindDeletionNode(BTreeNodeV1<T>? node, ref readonly T item)
    {
        if (node is null) return null;

        //si es hoja entonces es hora de insertarla
        if (node.IsLeaf)
        {
            for (var i = 0; i < node.KeyCount; i++)
                if (item.CompareTo(node.Keys[i]) == 0)
                    return node;
        }
        else
        {
            //si no es una hoja, profundice hasta la hoja
            for (var i = 0; i < node.KeyCount; i++)
            {
                if (item.CompareTo(node.Keys[i]) == 0) return node;

                //el valor actual es menor que el nuevo valor
                //profundizar hasta el hijo izquierdo del valor actual
                if (item.CompareTo(node.Keys[i]) < 0) return BTreeV1<T>.FindDeletionNode(node.Children[i], in item);
                //el valor actual es mayor que el nuevo valor
                //y el valor actual es el último elemento
                if (node.KeyCount == i + 1) return BTreeV1<T>.FindDeletionNode(node.Children[i + 1], in item);
            }
        }

        return null;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    /// <summary>
    /// Busca el nodo mas pequeño del árbol
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static BTreeNodeV1<T> FindMinNode(BTreeNodeV1<T> node)
    {
        //Si el nodo es un nodo hoja pues ya estamos en final y lo retornamos
        return node.IsLeaf ? node : BTreeV1<T>.FindMinNode(node.Children[0]);
    }
    /// <summary>
    /// Busca el nodo mas grade del árbol
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static BTreeNodeV1<T> FindMaxNode(BTreeNodeV1<T> node)
    {
        //Si el nodo es un nodo hoja pues ya estamos en final y lo retornamos
        return node.IsLeaf ? node : BTreeV1<T>.FindMaxNode(node.Children[node.KeyCount]);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: eliminar el estado administrado (objetos administrados)
                _root = null;
                Count = 0;
            }

            // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
            // TODO: establecer los campos grandes como NULL
            disposedValue = true;
        }
    }
    // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
    // ~BTreeV1()
    // {
    //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
    //     Dispose(disposing: false);
    // }
    /// <summary>
    /// Nodo mas grande
    /// </summary>
    public T Max
    {
        get
        {
            if (_root is null) return default!;

            var maxNode = BTreeV1<T>.FindMaxNode(_root);
            return maxNode.Keys[maxNode.KeyCount - 1];
        }
    }
    /// <summary>
    /// Nodo mas pequeño 
    /// </summary>
    public T Min
    {
        get
        {
            if (_root is null) return default!;

            var minNode = BTreeV1<T>.FindMinNode(_root);
            return minNode.Keys[0];
        }
    }
    public int Count { get; private set; }
    public bool IsReadOnly { get; } = false;
    private readonly int _maxKeysPerNode;
    private readonly int _minKeysPerNode;
    internal BTreeNodeV1<T>? _root;
    private bool disposedValue;
}
/// <summary>
/// Representa una colección de enumeración de un nodo de árbol
/// </summary>
/// <typeparam name="T"></typeparam>
internal class BTreeV1Enumerator<T> : IEnumerator<T>, IDisposable
    where T : IComparable
{

    /// <summary>
    /// Inicializa el enumerator y establece la raiz del mismo
    /// </summary>
    /// <param name="root"></param>
    public bool MoveNext()
    {
        //Si la raiz es nul no nos podemos mover
        if (_root is null) return false;
        //Si el proceso es nulo pues lo inicializamos
        if (_progress is null)
        {
            //Hacemos que current sea una referencia de la raiz pra no tocar la raiz directamente 
            _current = _root;
            //inicializamos el proceso en un stack para los elementos no nulos
            _progress = new Stack<BTreeNodeV1<T>>(_root.Children.Take(_root.KeyCount + 1).Where(x => x != null));
            //Retornamos si la cantidad de elementos en el current es lo suficiente para movernos
            return _current.KeyCount > 0;
        }
        //Si el nodo actual current no es nulo y el índice es menor a la cantidad de claves pues nos movemos
        if (_current != null && _index + 1 < _current.KeyCount)
        {
            _index++;
            return true;
        }
        //Si el proceso aun es mayor a 0 ósea que queden elementos pues iteramos
        if (_progress.Count > 0)
        {
            _index = 0;

            _current = _progress.Pop();

            foreach (var child in _current.Children.Take(_current.KeyCount + 1).Where(x => x != null))
                _progress.Push(child);

            return true;
        }

        return false;
    }
    public void Reset()
    {
        _progress = null;
        _current = null;
        _index = 0;
    }
    // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
    // ~BTreeV1Enumerator()
    // {
    //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
    //     Dispose(disposing: false);
    // }
    public void Dispose()
    {
        // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: eliminar el estado administrado (objetos administrados)
                _progress = null;
            }

            // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
            // TODO: establecer los campos grandes como NULL
            disposedValue = true;
        }
    }
    public T Current => _current!.Keys[_index];
    private bool disposedValue;
    /// <summary>
    /// Nodo Raiz del árbol enumerator
    /// </summary>
    private readonly BTreeNodeV1<T>? _root;
    /// <summary>
    /// Elemento concurrente de enumerator
    /// </summary>
    private BTreeNodeV1<T>? _current;
    /// <summary>
    /// Índice primacía del enumerador
    /// </summary>
    private int _index;
    /// <summary>
    /// 
    /// </summary>
    private Stack<BTreeNodeV1<T>>? _progress;
    object IEnumerator.Current => Current;
    internal BTreeV1Enumerator(BTreeNodeV1<T> root)
    {
        _root = root;
    }
}
/// <summary>
/// Representa un nodo de árbol b genérico
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Inicializa el nodo de árbol con su máximo de claves por nodo mas si nodo pariente
/// </remarks>
/// <param name="_maxKeysPerNode"></param>
/// <param name="parent"></param>
internal class BTreeNodeV1<T>(int _maxKeysPerNode, BTreeNodeV1<T>? parent) : NodeV1<T>(_maxKeysPerNode)
    where T : IComparable
{
    /// <summary>
    /// Nodo Pariente
    /// </summary>
    internal BTreeNodeV1<T>? Parent { get; set; } = parent;
    /// <summary>
    /// Nodos Hijos del Node B
    /// </summary>
    internal BTreeNodeV1<T>[] Children { get; set; } = new BTreeNodeV1<T>[_maxKeysPerNode + 1];
    /// <summary>
    /// Especifica si un nodo es hoja
    /// </summary>
    internal bool IsLeaf => Children[0] is null;
    internal override NodeV1<T>[] GetChildren()
    {
        return Children;
    }
    internal override NodeV1<T>? GetParent()
    {
        return Parent;
    }
}
/// <summary>
/// Representa una base para los nodos de tipo genérico
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class NodeV1<T> where T : IComparable
{
    /// <summary>
    /// Index del nodo
    /// </summary>
    internal int Index;
    /// <summary>
    /// Cantidad de claves
    /// </summary>
    internal int KeyCount;
    /// <summary>
    /// Inicializa el nodo con un numero máximo de elementos por nodo
    /// </summary>
    /// <param name="_maxKeysPerNode"></param>
    internal NodeV1(int _maxKeysPerNode)
    {
        Keys = new T[_maxKeysPerNode];
    }
    /// <summary>
    /// Claves del nodo
    /// </summary>
    internal T[] Keys { get; set; }
    /// <summary>
    /// Retorna el nodo arbitrario alterno
    /// </summary>
    /// <returns></returns>
    internal abstract NodeV1<T>? GetParent();
    /// <summary>
    /// Retorna los sub nodos del nodo
    /// </summary>
    /// <returns></returns>
    internal abstract NodeV1<T>[] GetChildren();
    /// <summary>
    /// Retorna la media del índice del nodo
    /// </summary>
    /// <returns></returns>
    internal int GetMedianIndex()
    {
        return KeyCount / 2 + 1;
    }
}
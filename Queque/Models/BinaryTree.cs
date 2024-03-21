using System.Numerics;
namespace QueQue.Models;
/// <summary>
/// Representa un árbol numérico genérico
/// </summary>
/// <typeparam name="T">Es el tipo numérico en el que se base el árbol</typeparam>
/// <param name="_Mode">Modo en el que se graduaran los elementos</param>
public sealed class BinaryTree<T>(BinaryTreeMode _Mode)
    where T : INumber<T>
{
    /// <summary>
    /// Representa el modelo en el que se basa un nodo para existir
    /// </summary>
    /// <param name="_Value">Valor numérico del nodo</param>
    class Node(T _Value)
    {
        /// <summary>
        /// Dato del nodo
        /// </summary>
        public T Value = _Value;
        /// <summary>
        /// Nodo hijo Izquierdo
        /// </summary>
        public Node? LeftNode { get; set; }
        /// <summary>
        /// Nodo Hijo Derecho
        /// </summary>
        public Node? RightNode { get; set; }
    }
    /// <summary>
    /// Nodo raiz del BinaryTree
    /// </summary>
    Node? _raiz;
    /// <summary>
    /// Cantidad total de nodos
    /// </summary>
    public int? Length { get; set; } = 1;
    /// <summary>
    /// Agrega un nuevo nodo en el BinaryTree en base al modo
    /// </summary>
    /// <param name="value">Valor que se va a insertar en el BinaryTree</param>
    public void Add(T value)
    {
        if (_Mode == BinaryTreeMode.Strict && Constrains(value)) return;
        else
        {
            Node tempNode = new(value);

            if (_raiz == null)
                _raiz = tempNode;
            else
            {
                Length++;
                Node? previous = null, reco = _raiz;
                while (reco is not null)
                {
                    previous = reco;
                    if (value < reco.Value)
                        reco = reco.LeftNode;
                    else
                        reco = reco.RightNode;
                }
                if (value < previous!.Value)
                    previous.LeftNode = tempNode;
                else
                    previous!.RightNode = tempNode;
            }
        }
    }
    /// <summary>
    /// Busca en elemento en el BinaryTree
    /// </summary>
    /// <param name="value">Valor a buscar si se encuentra en el BinaryTree</param>
    /// <returns></returns>
    public bool Constrains(T value)
    {
        Node? reco = _raiz;
        while (reco is not null)
        {
            if (value == reco.Value)  
                return true;
            else
                if (value > reco.Value)
                reco = reco.RightNode;
            else
                reco = reco.LeftNode;
        }
        return false;
    }
    /// <summary>
    /// Remueve un nodo en especifico del BinaryTree :
    /// si el nodo tiene 2 hijos se mantendrá el nodo mas a la izquierda si el valor es menor y en caso contrario el de la derecha 
    /// </summary>
    /// <param name="value">Valor a buscar para eliminar en el BinaryTree</param>
    /// <returns>Retorna true si el valor a sido encontrado y eleminado en caso contrario false</returns>
    /// <exception cref="NullReferenceException">Activa una excepción cuando se accede a un árbol sin elementos</exception>
    public bool Remove(T value)
    {
        if (_raiz is null) throw new NullReferenceException();
        else if (_raiz.Value == value)
        {
            _raiz = null;
            return true;
        }
        Node? preview = null, reco = _raiz;
        while (reco is not null)
        {
            if (value > reco.Value)
                reco = reco.RightNode;
            else
                reco = reco.LeftNode;

            if (reco is not null && reco!.Value == value)
            {
                if (preview is not null)
                {
                    if (preview.Value > value)
                        preview.LeftNode = reco.LeftNode;
                    else
                        preview.RightNode = reco.RightNode;
                    Length--;
                    return true;
                }
            }

            preview = reco;
        }

        return false;
    }
    /// <summary>
    /// Elimina todo los nodos del BinaryTree
    /// </summary>
    public void Clean() => _raiz = null;
    /// <summary>
    /// Remueve el nodo mas pequeño en el BinaryTree
    /// </summary>
    /// <exception cref="ArgumentNullException">Activa una excepción cuando se accede a un árbol sin elementos</exception>
    public void RemoveMin()
    {
        if (_raiz is null) throw new ArgumentNullException();

        if (_raiz.LeftNode is null)
            _raiz = _raiz.RightNode;
        else
        {
            Length--;
            Node anterior = _raiz;
            Node? reco = _raiz.LeftNode;
            while (reco?.LeftNode is not null)
            {
                anterior = reco;
                reco = reco.LeftNode;
            }
            anterior.LeftNode = reco?.RightNode;
        }
    }
    /// <summary>
    /// Remueve el nodo mas grange en el BinaryTree
    /// </summary>
    /// <exception cref="ArgumentNullException">Activa una excepción cuando se accede a un árbol sin elementos</exception>
    public void RemoveMax()
    {
        if (_raiz is null) throw new ArgumentNullException();

        if (_raiz.RightNode is null)
            _raiz = _raiz.LeftNode;
        else
        {
            Length--;
            Node anterior = _raiz;
            Node? reco = _raiz.RightNode;
            while (reco?.RightNode is not null)
            {
                anterior = reco;
                reco = reco.RightNode;
            }
            anterior.RightNode = reco?.LeftNode;
        }
    }
    /// <summary>
    /// Busca el valor mínimo del BinaryTree
    /// </summary>
    /// <returns>Retorna el valor mas pequeño encontrado en el BinaryTree</returns>
    /// <exception cref="ArgumentNullException">Activa una excepción cuando se accede a un árbol sin elementos</exception>
    public T? Min()
    {
        if (_raiz is null) throw new ArgumentNullException();

        var reco = _raiz;
        while (reco?.LeftNode is not null)
            reco = reco.LeftNode;

        return reco!.Value;
    }
    /// <summary>
    /// Busca el valor máximo del BinaryTree
    /// </summary>
    /// <returns>Retorna el valor mas grande encontrado en el BinaryTree</returns>
    /// <exception cref="ArgumentNullException">Activa una excepción cuando no se accede a un árbol sin elementos</exception>
    public T? Max()
    {
        if (_raiz is null) throw new ArgumentNullException();

        var reco = _raiz;
        while (reco?.RightNode is not null)
            reco = reco.RightNode;

        return reco!.Value;
    }
}
/// <summary>
/// Representa el modo de almacenamiento en el que se almacenan los elementos en el BinaryTree
/// </summary>
public enum BinaryTreeMode
{
    /// <summary>
    /// Establece un modo donde los elementos se almacenan sin repeticiones 
    /// </summary>
    Strict,
    /// <summary>
    /// Establece un modo donde los elementos se almacenan sin presentar excepciones
    /// </summary>
    Normal
}
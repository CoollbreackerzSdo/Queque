using Queque.Models;
Console.WriteLine();

//      Arbol B
//    [n]-[n]-[n] 
//     |  |  |  |
//    [n][n][n][n]
//

var numbers = Enumerable.Range(-200, 1000).ToArray();
var bTree = new BTreeV1<int>(3);
foreach (var item in numbers)
{
    bTree.Add(item);
}

foreach (var item in bTree)
{
    Console.WriteLine(item);
}
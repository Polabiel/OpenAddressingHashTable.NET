using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Interface for hash table implementations that support clearing operation
/// </summary>
public interface IHashTable
{
    /// <summary>
    /// Clears all data from the hash table without requiring parameters
    /// </summary>
    void Limpar();
}
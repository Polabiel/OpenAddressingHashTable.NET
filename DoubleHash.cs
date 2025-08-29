using System;
using System.Collections.Generic;

public class DoubleHash<T> : IHashing<T> where T : IRegistro<T>, new()
{
    private const int TAMANHO_PADRAO = 101; // primo para melhor distribuição
    private T[] tabelaDeHash;
    private bool[] ocupado;
    private bool[] removido;
    private int totalElementos;
    private int R; // Número primo menor que o tamanho da tabela para segunda função hash

    public DoubleHash() : this(TAMANHO_PADRAO) { }

    public DoubleHash(int tamanhoDesejado)
    {
        // Garantir que o tamanho seja primo
        int tamanho = ProximoPrimo(tamanhoDesejado);
        tabelaDeHash = new T[tamanho];
        ocupado = new bool[tamanho];
        removido = new bool[tamanho];
        totalElementos = 0;
        
        // R deve ser um primo menor que o tamanho da tabela
        R = MaiorPrimoMenorQue(tamanho);
    }

    private int Hash1(string chave)
    {
        long hash = 0;
        for (int i = 0; i < chave.Length; i++)
            hash = (hash * 31 + chave[i]) % tabelaDeHash.Length;
        
        if (hash < 0)
            hash += tabelaDeHash.Length;
            
        return (int)hash;
    }

    // Segunda função hash: h2(x) = R - (x mod R)
    // onde R é um primo menor que o tamanho da tabela
    private int Hash2(string chave)
    {
        long hash = 0;
        for (int i = 0; i < chave.Length; i++)
            hash = (hash * 37 + chave[i]);
        
        if (hash < 0)
            hash = -hash;
            
        int resultado = R - (int)(hash % R);
        
        // Garantir que o resultado seja sempre positivo e diferente de 0
        if (resultado <= 0)
            resultado = 1;
            
        return resultado;
    }

    private int ProximoPrimo(int numero)
    {
        if (numero <= 1) return 2;
        
        while (!EhPrimo(numero))
            numero++;
            
        return numero;
    }

    private int MaiorPrimoMenorQue(int numero)
    {
        for (int i = numero - 1; i >= 2; i--)
        {
            if (EhPrimo(i))
                return i;
        }
        return 2; // fallback
    }

    private bool EhPrimo(int numero)
    {
        if (numero <= 1) return false;
        if (numero <= 3) return true;
        if (numero % 2 == 0 || numero % 3 == 0) return false;
        
        for (int i = 5; i * i <= numero; i += 6)
        {
            if (numero % i == 0 || numero % (i + 2) == 0)
                return false;
        }
        
        return true;
    }

    public bool Incluir(T novoDado)
    {
        if (totalElementos >= tabelaDeHash.Length * 0.7) // Fator de carga
            return false;

        int hash1 = Hash1(novoDado.Chave);
        int hash2 = Hash2(novoDado.Chave);
        int posicao = hash1;
        
        // Double Hashing: incrementa com hash2
        for (int i = 0; i < tabelaDeHash.Length; i++)
        {
            if (!ocupado[posicao] || removido[posicao])
            {
                tabelaDeHash[posicao] = novoDado;
                ocupado[posicao] = true;
                removido[posicao] = false;
                totalElementos++;
                return true;
            }
            
            // Verifica se já existe
            if (tabelaDeHash[posicao] != null && tabelaDeHash[posicao].Equals(novoDado))
                return false;

            // Próxima posição usando double hashing
            posicao = (posicao + hash2) % tabelaDeHash.Length;
        }

        return false; // Não conseguiu inserir
    }

    public bool Excluir(T dado)
    {
        int posicao;
        if (!Existe(dado, out posicao))
            return false;

        tabelaDeHash[posicao] = default(T);
        ocupado[posicao] = false;
        removido[posicao] = true;
        totalElementos--;
        return true;
    }

    public bool Existe(T dado, out int onde)
    {
        int hash1 = Hash1(dado.Chave);
        int hash2 = Hash2(dado.Chave);
        onde = hash1;
        
        // Double Hashing para busca
        for (int i = 0; i < tabelaDeHash.Length; i++)
        {
            if (!ocupado[onde])
            {
                if (!removido[onde])
                    break; // Posição nunca foi usada
            }
            else if (tabelaDeHash[onde] != null && tabelaDeHash[onde].Equals(dado))
            {
                return true;
            }

            // Próxima posição usando double hashing
            onde = (onde + hash2) % tabelaDeHash.Length;
        }

        onde = -1;
        return false;
    }

    public List<T> Conteudo()
    {
        List<T> resultado = new List<T>();
        
        for (int i = 0; i < tabelaDeHash.Length; i++)
        {
            if (ocupado[i] && !removido[i] && tabelaDeHash[i] != null)
                resultado.Add(tabelaDeHash[i]);
        }
        
        return resultado;
    }
}
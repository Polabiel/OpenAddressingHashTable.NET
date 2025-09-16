using System;
using System.Collections.Generic;

public class DoubleHashing<T> : IHashing<T> where T : IRegistro<T>, new()
{
    private T[] tabela;
    private bool[] tombstone;
    private int R; // número primo menor que o tamanho da tabela para segunda função hash

    // Tamanho otimizado para demonstrar duplo hash eficientemente
    public DoubleHashing() : this(307) { } // primo adequado para duplo hash
    public DoubleHashing(int tamanho)
    {
        // Garantir que o tamanho seja primo
        tamanho = GetNextPrime(tamanho);
        tabela = new T[tamanho];
        tombstone = new bool[tamanho];
        
        // R deve ser um primo menor que o tamanho da tabela para h2(x) = R - (x mod R)
        R = GetPreviousPrime(tamanho);
        if (R == tamanho) R = GetPreviousPrime(tamanho - 1);
    }

    private int GetNextPrime(int number)
    {
        if (number <= 2) return 2;
        if (number % 2 == 0) number++;
        
        while (!IsPrime(number))
            number += 2;
        return number;
    }
    
    private int GetPreviousPrime(int n)
    {
        if (n <= 2) return 2;
        if (n % 2 == 0) n--;
        
        while (n >= 2 && !IsPrime(n))
            n -= 2;
        return n >= 2 ? n : 2;
    }
    
    private bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number <= 3) return true;
        if (number % 2 == 0 || number % 3 == 0) return false;
        
        for (int i = 5; i * i <= number; i += 6)
        {
            if (number % i == 0 || number % (i + 2) == 0)
                return false;
        }
        return true;
    }

    private int HashAprimorado(string chave)
    {
        long tot = 0;
        for (int i = 0; i < chave.Length; i++)
            tot = 37 * tot + (int)chave[i];
        tot = tot % tabela.Length;
        if (tot < 0) tot += tabela.Length;
        return (int)tot;
    }

    // Segunda função hash: h2(x) = R - (x mod R), onde R é primo menor que tamanho da tabela
    private int SecondHash(string chave)
    {
        long x = 0;
        for (int i = 0; i < chave.Length; i++)
            x = 37 * x + (int)chave[i];
        
        int h2 = R - (int)(Math.Abs(x) % R); // Implementação da fórmula h2(x) = R - (x mod R)
        if (h2 == 0) h2 = 1; // garantir que nunca seja 0 (passo mínimo = 1)
        return h2;
    }

    public bool Incluir(T novoDado)
    {
        int h1 = HashAprimorado(novoDado.Chave);
        int h2 = SecondHash(novoDado.Chave);
        int primeiroTombstone = -1;

        // Duplo hash: usa duas funções hash para determinar posição e passo
        for (int i = 0; i < tabela.Length; i++)
        {
            int pos = (h1 + i * h2) % tabela.Length; // posição = hash1 + i * hash2

            // Verifica se é tombstone
            if (tombstone[pos])
            {
                if (primeiroTombstone == -1)
                    primeiroTombstone = pos;
                continue;
            }

            // Verifica se está vazio
            if (tabela[pos] == null)
            {
                if (primeiroTombstone != -1)
                {
                    // Reusa o tombstone
                    tabela[primeiroTombstone] = novoDado;
                    tombstone[primeiroTombstone] = false;
                    return true;
                }
                else
                {
                    tabela[pos] = novoDado;
                    return true;
                }
            }

            // Verifica se é duplicata
            if (tabela[pos].Equals(novoDado))
                return false;
        }

        // A tabela está cheia mas pode ter tombstone para reuso
        if (primeiroTombstone != -1)
        {
            tabela[primeiroTombstone] = novoDado;
            tombstone[primeiroTombstone] = false;
            return true;
        }

        return false; // tabela cheia
    }

    public bool Existe(T dado, out int onde)
    {
        int h1 = HashAprimorado(dado.Chave);
        int h2 = SecondHash(dado.Chave);
        onde = -1;

        for (int i = 0; i < tabela.Length; i++)
        {
            int pos = (h1 + i * h2) % tabela.Length;

            // Se encontrou null sem tombstone, elemento não existe
            if (tabela[pos] == null && !tombstone[pos])
                return false;

            // Se é tombstone, continua procurando
            if (tombstone[pos])
                continue;

            // Se encontrou o elemento
            if (tabela[pos] != null && tabela[pos].Equals(dado))
            {
                onde = pos;
                return true;
            }
        }
        return false;
    }

    public bool Excluir(T dado)
    {
        int pos;
        if (!Existe(dado, out pos)) return false;
        tabela[pos] = default(T);
        tombstone[pos] = true;
        return true;
    }

    public List<T> Conteudo()
    {
        var saida = new List<T>();
        for (int i = 0; i < tabela.Length; i++)
            if (tabela[i] != null && !tombstone[i])
                saida.Add(tabela[i]);
        return saida;
    }

    public void Limpar()
    {
        for (int i = 0; i < tabela.Length; i++)
        {
            tabela[i] = default(T);
            tombstone[i] = false;
        }
    }
}
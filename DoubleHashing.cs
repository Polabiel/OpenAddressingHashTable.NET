using System;
using System.Collections.Generic;

public class DoubleHashing<T> : IHashing<T> where T : IRegistro<T>, new()
{
    private T[] tabela;
    private bool[] tombstone;
    private int R; // prime smaller than table size for second hash

    public DoubleHashing() : this(10007) { }
    public DoubleHashing(int tamanho)
    {
        tabela = new T[tamanho];
        tombstone = new bool[tamanho];
        R = PreviousPrime(tamanho);
        if (R == tamanho) R = PreviousPrime(tamanho - 1);
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

    private int SecondHash(string chave)
    {
        long x = 0;
        for (int i = 0; i < chave.Length; i++)
            x = 37 * x + (int)chave[i];
        int h2 = R - (int)(x % R);
        if (h2 == 0) h2 = 1;
        return h2;
    }

    private int PreviousPrime(int n)
    {
        if (n <= 2) return 2;
        for (int i = n - 1; i >= 2; i--)
        {
            bool primo = true;
            for (int j = 2; j * j <= i; j++)
                if (i % j == 0) { primo = false; break; }
            if (primo) return i;
        }
        return 2;
    }

    public bool Incluir(T novoDado)
    {
        int h1 = HashAprimorado(novoDado.Chave);
        int h2 = SecondHash(novoDado.Chave);
        int primeiroTombstone = -1;

        for (int i = 0; i < tabela.Length; i++)
        {
            int pos = (h1 + i * h2) % tabela.Length;
            if (tabela[pos] == null)
            {
                if (primeiroTombstone != -1)
                {
                    tabela[primeiroTombstone] = novoDado;
                    tombstone[primeiroTombstone] = false;
                    return true;
                }

                tabela[pos] = novoDado;
                return true;
            }

            if (tombstone[pos])
            {
                if (primeiroTombstone == -1) primeiroTombstone = pos;
                continue;
            }

            if (tabela[pos].Equals(novoDado))
                return false; // já existe
        }

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
            if (tabela[pos] == null && !tombstone[pos])
                return false; // slot vazio real => não existe

            if (!tombstone[pos] && tabela[pos] != null && tabela[pos].Equals(dado))
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
            if (tabela[i] != null)
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
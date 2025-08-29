using System;
using System.Collections.Generic;

public class LinearProbingHash<T> : IHashing<T> where T : IRegistro<T>, new()
{
    private T[] tabela;
    private bool[] tombstone;

    public LinearProbingHash() : this(10007) { }
    public LinearProbingHash(int tamanho)
    {
        tabela = new T[tamanho];
        tombstone = new bool[tamanho];
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

    public bool Incluir(T novoDado)
    {
        int h = HashAprimorado(novoDado.Chave);
        int primeiroTombstone = -1;
        for (int i = 0; i < tabela.Length; i++)
        {
            int pos = (h + i) % tabela.Length;
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
        int h = HashAprimorado(dado.Chave);
        onde = -1;
        for (int i = 0; i < tabela.Length; i++)
        {
            int pos = (h + i) % tabela.Length;
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

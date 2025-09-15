using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Xml.Linq;

public class HashSimples<T> : IHashing<T> where T : IRegistro<T>, new()
{
    const int tamanhoPadrao = 10007;  // primo
    T[] tabelaDeHash;
    public HashSimples() : this(tamanhoPadrao) { }
    public HashSimples(int tamanhoDesejado)
    {
        tabelaDeHash = new T[tamanhoDesejado];
    }

    private int HashAprimorado(string chave)
    {
        long tot = 0;
        for (int i = 0; i < chave.Length; i++)
            tot += 37 * tot + (int)chave[i];
        tot = tot % tabelaDeHash.Length;
        if (tot < 0)
            tot += tabelaDeHash.Length;
        return (int)tot;
    }

    private int Hash(string chave)
    {
        int tot = 0;
        for (int i = 0; i < chave.Trim().Length; i++)
            tot += (int)chave[i];
        return tot % tabelaDeHash.Length;
    }

    public bool Incluir(T novoDado)
    {
        int valorDeHash = HashAprimorado(novoDado.Chave);
        if (tabelaDeHash[valorDeHash] != null)
            return false;
        tabelaDeHash[valorDeHash] = novoDado;
        return true;
    }

    public bool Existe(T dado, out int posicao)
    {
        posicao = HashAprimorado(dado.Chave);
        return tabelaDeHash[posicao] != null && tabelaDeHash[posicao].Equals(dado);
    }

    public List<T> Conteudo()
    {
        var saida = new List<T>();
        for (int i = 0; i < tabelaDeHash.Length; i++)
            if (tabelaDeHash[i] != null)
                saida.Add(tabelaDeHash[i]);
        return saida;
    }

    public void Limpar()
    {
        for (int i = 0; i < tabelaDeHash.Length; i++)
            tabelaDeHash[i] = default(T);
    }

    public bool Excluir(T dado)
    {
        int ondeAchou;
        if (Existe(dado, out ondeAchou))
        {
            tabelaDeHash[ondeAchou] = default(T);
            return true;
        }
        return false;
    }
}
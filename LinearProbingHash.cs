using System;
using System.Collections.Generic;

public class LinearProbingHash<T> : IHashing<T> where T : IRegistro<T>, new()
{
    private const int TAMANHO_PADRAO = 101; // primo para melhor distribuição
    private T[] tabelaDeHash;
    private bool[] ocupado;
    private bool[] removido;
    private int totalElementos;

    public LinearProbingHash() : this(TAMANHO_PADRAO) { }

    public LinearProbingHash(int tamanhoDesejado)
    {
        // Garantir que o tamanho seja primo (simplificado)
        int tamanho = ProximoPrimo(tamanhoDesejado);
        tabelaDeHash = new T[tamanho];
        ocupado = new bool[tamanho];
        removido = new bool[tamanho];
        totalElementos = 0;
    }

    private int Hash(string chave)
    {
        long hash = 0;
        for (int i = 0; i < chave.Length; i++)
            hash = (hash * 31 + chave[i]) % tabelaDeHash.Length;
        
        if (hash < 0)
            hash += tabelaDeHash.Length;
            
        return (int)hash;
    }

    private int ProximoPrimo(int numero)
    {
        if (numero <= 1) return 2;
        
        while (!EhPrimo(numero))
            numero++;
            
        return numero;
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
        if (totalElementos >= tabelaDeHash.Length * 0.75) // Fator de carga
            return false;

        int posicao = Hash(novoDado.Chave);
        int posicaoOriginal = posicao;

        // Sondagem Linear: incrementa de 1 em 1
        do
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

            posicao = (posicao + 1) % tabelaDeHash.Length;
        } while (posicao != posicaoOriginal);

        return false; // Tabela cheia
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
        onde = Hash(dado.Chave);
        int posicaoOriginal = onde;

        // Sondagem Linear para busca
        do
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

            onde = (onde + 1) % tabelaDeHash.Length;
        } while (onde != posicaoOriginal);

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
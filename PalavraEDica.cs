// ra23600 ra23305
// Gabriel - Andrew
using Microsoft.Win32;
using System;
using System.IO;
namespace apListaLigada
{
    internal class PalavraEDica : IComparable<PalavraEDica>, IRegistro<PalavraEDica>
    {
        public string Palavra { get; set; }
        public string Dica { get; set; }

        public string Chave => Palavra;

        public PalavraEDica(string palavra, string dica)
        {
            Palavra = palavra;
            Dica = dica;
        }

        public int CompareTo(PalavraEDica other)
        {
            return string.Compare(Palavra, other?.Palavra, StringComparison.OrdinalIgnoreCase);
        }

        public string ParaArquivo()
        {
            return $"{Palavra.PadRight(30)}{Dica}";
        }

        public string FormatoDeArquivo()
        {
            return ParaArquivo();
        }

        public override string ToString()
        {
            return $"{Palavra} - {Dica}";
        }

        public override bool Equals(object obj)
        {
            if (obj is PalavraEDica other)
            {
                return string.Equals(Palavra, other.Palavra, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(Dica, other.Dica, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Palavra?.ToLowerInvariant().GetHashCode() ?? 0);
                hash = hash * 23 + (Dica?.ToLowerInvariant().GetHashCode() ?? 0);
                return hash;
            }
        }

        public void LerRegistro(StreamReader arquivo)
        {
            string linha = arquivo.ReadLine();
            if (linha != null)
            {
                Palavra = linha.Substring(0, 30).Trim();
                Dica = linha.Substring(30).Trim();
            }
            else
            {
                Palavra = string.Empty;
                Dica = string.Empty;
            }
        }

        public void EscreverRegistro(StreamWriter arquivo)
        {
            arquivo.WriteLine(ParaArquivo());
        }

        public bool Equals(PalavraEDica outroRegistro)
        {
            if (ReferenceEquals(null, outroRegistro)) return false;
            if (ReferenceEquals(this, outroRegistro)) return true;
            return string.Equals(Palavra, outroRegistro.Palavra, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Dica, outroRegistro.Dica, StringComparison.OrdinalIgnoreCase);
        }
        public PalavraEDica()
        {
            Palavra = string.Empty;
            Dica = string.Empty;
        }
    }
}
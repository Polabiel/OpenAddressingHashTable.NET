using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IHashing<T> where T : IRegistro<T>, new()
{
    bool Incluir(T novoDado);
    bool Excluir(T dado);
    bool Existe(T dado, out int onde);
    List<T> Conteudo();
}
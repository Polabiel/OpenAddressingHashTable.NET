# OpenAddressingHashTable.NET

Este projeto implementa diferentes técnicas de resolução de colisões em tabelas hash para fins educacionais.

<img width="543" height="747" alt="image" src="https://github.com/user-attachments/assets/bb613dde-69d5-4c34-9cbe-324181ee6e61" />

## Implementações Disponíveis

### 1. Bucket Hash (BucketHash.cs)
Implementação usando **chaining** (encadeamento) para resolução de colisões. Cada posição da tabela contém uma lista (ArrayList) que armazena múltiplos elementos quando ocorrem colisões.

**Características:**
- Utiliza ArrayList para armazenar colisões
- Tamanho fixo da tabela (37 posições)
- Adequado quando há muitas colisões esperadas

### 2. Sondagem Linear (LinearProbingHash.cs)
Implementação de **open addressing** usando sondagem linear. Quando uma colisão ocorre, busca a próxima posição disponível incrementando de 1 em 1.

**Características:**
- Busca: posição + 1, posição + 2, posição + 3...
- **Problema:** Causa agrupamento primário (primary clustering)
- Fator de carga recomendado: máximo 75%

### 3. Sondagem Quadrática (QuadraticProbingHash.cs)
Implementação de **open addressing** usando sondagem quadrática. Reduz o agrupamento primário, mas pode causar agrupamento secundário.

**Características:**
- Busca: posição + 1², posição + 2², posição + 3²...
- **Requisito:** Tamanho da tabela deve ser um número primo
- Fator de carga recomendado: máximo 50%
- Reduz agrupamento primário comparado à sondagem linear

### 4. Duplo Hash (DoubleHash.cs)
Implementação de **open addressing** usando dupla hash. Utiliza uma segunda função hash para determinar o passo de sondagem.

**Características:**
- Função hash principal: h1(x) para posição inicial
- Segunda função hash: h2(x) = R - (x mod R), onde R é primo < tamanho da tabela
- Minimiza tanto agrupamento primário quanto secundário
- Fator de carga recomendado: máximo 70%

## Interface Comum

Todas as implementações seguem a interface `IHashing<T>`:

```csharp
public interface IHashing<T> where T : IRegistro<T>, new()
{
    bool Incluir(T novoDado);      // Inserir elemento
    bool Excluir(T dado);          // Remover elemento
    bool Existe(T dado, out int onde);  // Verificar existência
    List<T> Conteudo();           // Listar todos os elementos
}
```

## Comparação de Técnicas

| Técnica | Vantagens | Desvantagens | Uso Recomendado |
|---------|-----------|--------------|-----------------|
| **Bucket Hash** | Simples, não há limite de elementos por posição | Uso extra de memória, acesso indireto | Muitas colisões esperadas |
| **Sondagem Linear** | Cache-friendly, simples implementação | Agrupamento primário severo | Baixo fator de carga |
| **Sondagem Quadrática** | Reduz agrupamento primário | Agrupamento secundário, requer tamanho primo | Fator de carga médio |
| **Duplo Hash** | Melhor distribuição, minimiza agrupamentos | Mais complexo, requer duas funções hash | Alto desempenho necessário |

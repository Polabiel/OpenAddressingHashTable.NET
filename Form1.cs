using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using apListaLigada;

namespace OpenAddressingHashTable.NET
{
    public partial class Form1 : Form
    {
        private IHashing<PalavraEDica> hashTable;
        private const string DATA_FILE_PATH = "dados.txt";
        private List<PalavraEDica> cachedData;
        
        public Form1()
        {
            InitializeComponent();
            InitializeHashTable();
            AttachEventHandlers();
        }
        
        private void InitializeHashTable()
        {
            // Removido carregamento automático de dados.txt
            // Agora os dados serão carregados apenas quando o usuário selecionar um arquivo
            hashTable = new BucketHash<PalavraEDica>();
            
            radio_BucketHash.Checked = true;
            
            AtualizarListagem(); 
        }
        
        private void AttachEventHandlers()
        {
            radio_BucketHash.CheckedChanged += RadioButton_CheckedChanged;
            radio_Linear.CheckedChanged += RadioButton_CheckedChanged;
            radio_Quadratica.CheckedChanged += RadioButton_CheckedChanged;
            radio_duploHash.CheckedChanged += RadioButton_CheckedChanged;
            
            btn_Incluir.Click += Btn_Incluir_Click;
            btn_Excluir.Click += Btn_Excluir_Click;
            btn_Alterar.Click += Btn_Alterar_Click;
            btn_Listar.Click += Btn_Listar_Click;
            
            lsbListagem.SelectionChanged += LsbListagem_SelectionChanged;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (radio != null && radio.Checked)
            {
                textBox_palavra.Clear();
                textBox_Dica.Clear();
                lsbListagem.ClearSelection();
                
                lsbListagem.Rows.Clear();
                
                if (hashTable != null)
                {
                    ClearCurrentHashTable();
                }
                
                if (radio == radio_BucketHash)
                    hashTable = new BucketHash<PalavraEDica>();
                else if (radio == radio_Linear)
                    hashTable = new LinearProbingHash<PalavraEDica>();
                else if (radio == radio_Quadratica)
                    hashTable = new QuadraticProbingHash<PalavraEDica>();
                else if (radio == radio_duploHash)
                    hashTable = new DoubleHashing<PalavraEDica>();

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt|Todos os arquivos (*.*)|*.*";
                openFileDialog.Title = "Selecionar arquivo de dados";
                openFileDialog.InitialDirectory = Application.StartupPath;
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        LoadDataFromFile(openFileDialog.FileName);
                        LoadDataIntoHashTable();
                        AtualizarListagem();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao carregar arquivo: {ex.Message}", "Erro", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    AtualizarListagem();
                }
            }
        }

        private void Btn_Incluir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_palavra.Text) || string.IsNullOrWhiteSpace(textBox_Dica.Text))
            {
                MessageBox.Show("Por favor, preencha tanto a palavra quanto a dica.", "Dados incompletos", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var novaPalavra = new PalavraEDica(textBox_palavra.Text.Trim(), textBox_Dica.Text.Trim());
            
            if (hashTable.Incluir(novaPalavra))
            {
                MessageBox.Show("Você incluiu um novo dado", "Inclusão", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox_palavra.Clear();
                textBox_Dica.Clear();
                AtualizarListagem();
            }
            else
            {
                MessageBox.Show("Não foi possível incluir o dado. Palavra já existe.", "Erro na inclusão", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void Btn_Excluir_Click(object sender, EventArgs e)
        {
            if (lsbListagem.SelectedRows.Count == 0)
            {
                MessageBox.Show("Você precisa selecionar um dado para excluir", "Nenhum dado selecionado", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var selectedRow = lsbListagem.SelectedRows[0];
            var palavra = selectedRow.Cells["Palavra"].Value?.ToString();
            var dica = selectedRow.Cells["Dica"].Value?.ToString();
            
            if (string.IsNullOrEmpty(palavra) || string.IsNullOrEmpty(dica))
            {
                MessageBox.Show("Dados inválidos selecionados.", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            var result = MessageBox.Show("Você deseja mesmo excluir esse dado?", "Confirmar exclusão", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                var palavraParaExcluir = new PalavraEDica(palavra, dica);
                if (hashTable.Excluir(palavraParaExcluir))
                {
                    AtualizarListagem();
                }
                else
                {
                    MessageBox.Show("Não foi possível excluir o dado.", "Erro na exclusão", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void Btn_Alterar_Click(object sender, EventArgs e)
        {
            if (lsbListagem.SelectedRows.Count == 0)
            {
                MessageBox.Show("Você precisa selecionar um dado para alterar", "Nenhum dado selecionado", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(textBox_palavra.Text) || string.IsNullOrWhiteSpace(textBox_Dica.Text))
            {
                MessageBox.Show("Por favor, preencha tanto a palavra quanto a dica para alteração.", "Dados incompletos", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var selectedRow = lsbListagem.SelectedRows[0];
            var palavraOriginal = selectedRow.Cells["Palavra"].Value?.ToString();
            var dicaOriginal = selectedRow.Cells["Dica"].Value?.ToString();
            
            if (string.IsNullOrEmpty(palavraOriginal) || string.IsNullOrEmpty(dicaOriginal))
            {
                MessageBox.Show("Dados inválidos selecionados.", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            var result = MessageBox.Show("Você deseja alterar mesmo esse dado?", "Confirmar alteração", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                var palavraOriginalObj = new PalavraEDica(palavraOriginal, dicaOriginal);
                var novaPalavraObj = new PalavraEDica(textBox_palavra.Text.Trim(), textBox_Dica.Text.Trim());

                if (hashTable.Excluir(palavraOriginalObj))
                {
                    if (hashTable.Incluir(novaPalavraObj))
                    {
                        textBox_palavra.Clear();
                        textBox_Dica.Clear();
                        AtualizarListagem();
                    }
                    else
                    {
                        hashTable.Incluir(palavraOriginalObj);
                        MessageBox.Show("Não foi possível alterar o dado. Palavra já existe.", "Erro na alteração", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Não foi possível alterar o dado.", "Erro na alteração", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void Btn_Listar_Click(object sender, EventArgs e)
        {
            AtualizarListagem();
            MessageBox.Show("Os dados foram atualizados com sucesso", "Listagem", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void AtualizarListagem()
        {
            try
            {
                if (hashTable == null)
                {
                    return;
                }
                
                ConfigurarColunas();
                
                lsbListagem.Rows.Clear();
                
                ExibirOrdemHashing();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar listagem: {ex.Message}", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ConfigurarColunas()
        {
            if (lsbListagem.Columns.Count != 3)
            {
                lsbListagem.Columns.Clear();
                lsbListagem.AutoGenerateColumns = false;
                lsbListagem.Columns.Add("Indice", "Índice");
                lsbListagem.Columns.Add("Palavra", "Palavra");
                lsbListagem.Columns.Add("Dica", "Dica");
                lsbListagem.Columns["Indice"].Width = 80;
                lsbListagem.Columns["Palavra"].Width = 200;
                lsbListagem.Columns["Dica"].Width = 350;
                lsbListagem.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                lsbListagem.MultiSelect = false;
                lsbListagem.ReadOnly = true;
            }
        }
        
        private void ExibirOrdemHashing()
        {
            if (hashTable is BucketHash<PalavraEDica> bucketHash)
            {
                ExibirOrdemBucketHash(bucketHash);
            }
            else if (hashTable is LinearProbingHash<PalavraEDica> linearHash)
            {
                ExibirOrdemLinearProbing(linearHash);
            }
            else if (hashTable is QuadraticProbingHash<PalavraEDica> quadraticHash)
            {
                ExibirOrdemQuadraticProbing(quadraticHash);
            }
            else if (hashTable is DoubleHashing<PalavraEDica> doubleHash)
            {
                ExibirOrdemDoubleHashing(doubleHash);
            }
        }
        
        private void ExibirOrdemBucketHash(BucketHash<PalavraEDica> bucketHash)
        {
            try
            {
                var dadosField = typeof(BucketHash<PalavraEDica>).GetField("dados", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (dadosField != null)
                {
                    var dados = (System.Collections.ArrayList[])dadosField.GetValue(bucketHash);
                    int totalOcupados = 0;
                    int bucketsUsados = 0;
                    
                    for (int i = 0; i < dados.Length; i++)
                    {
                        if (dados[i].Count > 0)
                        {
                            bucketsUsados++;
                            for (int j = 0; j < dados[i].Count; j++)
                            {
                                var item = dados[i][j] as PalavraEDica;
                                if (item != null)
                                {
                                    totalOcupados++;
                                    string bucketInfo = j == 0 ? i.ToString() : $"{i}.{j}";
                                    lsbListagem.Rows.Add(bucketInfo, item.Palavra ?? "", item.Dica ?? "");
                                }
                            }
                        }
                        else
                        {
                            lsbListagem.Rows.Add(i.ToString(), "-- vazio --", "");
                        }
                    }
                    
                    // Atualiza informações no footer para BucketHash
                    AtualizarFooterInfoBucket(dados.Length, bucketsUsados, totalOcupados);
                }
                else
                {
                    var dados = hashTable.Conteudo();
                    foreach (var item in dados)
                    {
                        if (item != null)
                        {
                            lsbListagem.Rows.Add("N/A", item.Palavra ?? "", item.Dica ?? "");
                        }
                    }
                    AtualizarFooterInfoBucket(0, 0, dados.Count);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exibir ordem BucketHash: {ex.Message}\nUsando exibição padrão.", 
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    
                var dados = hashTable.Conteudo();
                foreach (var item in dados)
                {
                    if (item != null)
                    {
                        lsbListagem.Rows.Add("N/A", item.Palavra ?? "", item.Dica ?? "");
                    }
                }
                AtualizarFooterInfoBucket(0, 0, dados.Count);
            }
        }
        
        private void ExibirOrdemLinearProbing(LinearProbingHash<PalavraEDica> linearHash)
        {
            try
            {
                var tabelaField = typeof(LinearProbingHash<PalavraEDica>).GetField("tabela",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var tombstoneField = typeof(LinearProbingHash<PalavraEDica>).GetField("tombstone",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                if (tabelaField != null && tombstoneField != null)
                {
                    var tabela = (PalavraEDica[])tabelaField.GetValue(linearHash);
                    var tombstone = (bool[])tombstoneField.GetValue(linearHash);
                    
                    // Encontra os índices ocupados primeiro
                    var indicesOcupados = new List<int>();
                    for (int i = 0; i < tabela.Length; i++)
                    {
                        if (tabela[i] != null || tombstone[i])
                        {
                            indicesOcupados.Add(i);
                        }
                    }
                    
                    // Se não há dados, mostra apenas os primeiros slots
                    if (indicesOcupados.Count == 0)
                    {
                        for (int i = 0; i < Math.Min(20, tabela.Length); i++)
                        {
                            lsbListagem.Rows.Add(i.ToString(), "-- vazio --", "");
                        }
                        if (tabela.Length > 20)
                        {
                            lsbListagem.Rows.Add("...", $"... mais {tabela.Length - 20} slots vazios", "...");
                        }
                        
                        // Atualiza informações no footer
                        AtualizarFooterInfo(tabela.Length, 0, 0);
                        return;
                    }
                    
                    // Mostra slots ocupados e alguns contextuais ao redor
                    var exibidos = new HashSet<int>();
                    
                    foreach (int indice in indicesOcupados.OrderBy(x => x))
                    {
                        // Mostra alguns slots antes e depois do ocupado para contexto
                        for (int j = Math.Max(0, indice - 2); j <= Math.Min(tabela.Length - 1, indice + 2); j++)
                        {
                            if (!exibidos.Contains(j))
                            {
                                exibidos.Add(j);
                            }
                        }
                    }
                    
                    // Exibe os slots em ordem crescente
                    foreach (int j in exibidos.OrderBy(x => x))
                    {
                        if (tabela[j] != null)
                        {
                            lsbListagem.Rows.Add(j.ToString(), tabela[j].Palavra ?? "", tabela[j].Dica ?? "");
                        }
                        else if (tombstone[j])
                        {
                            lsbListagem.Rows.Add(j.ToString(), "-- removido --", "(slot com tombstone)");
                        }
                        else
                        {
                            lsbListagem.Rows.Add(j.ToString(), "-- vazio --", "");
                        }
                    }
                    
                    // Calcula e atualiza informações no footer
                    int totalOcupados = indicesOcupados.Count(i => tabela[i] != null);
                    int totalTombstones = indicesOcupados.Count(i => tombstone[i]);
                    AtualizarFooterInfo(tabela.Length, totalOcupados, totalTombstones);
                }
                else
                {
                    var dados = hashTable.Conteudo();
                    foreach (var item in dados)
                    {
                        if (item != null)
                        {
                            lsbListagem.Rows.Add("N/A", item.Palavra ?? "", item.Dica ?? "");
                        }
                    }
                    AtualizarFooterInfo(0, dados.Count, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exibir ordem LinearProbing: {ex.Message}\nUsando exibição padrão.", 
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    
                var dados = hashTable.Conteudo();
                foreach (var item in dados)
                {
                    if (item != null)
                    {
                        lsbListagem.Rows.Add("N/A", item.Palavra ?? "", item.Dica ?? "");
                    }
                }
                AtualizarFooterInfo(0, dados.Count, 0);
            }
        }
        
        private void ExibirOrdemQuadraticProbing(QuadraticProbingHash<PalavraEDica> quadraticHash)
        {
            try
            {
                var tabelaField = typeof(QuadraticProbingHash<PalavraEDica>).GetField("tabela",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var tombstoneField = typeof(QuadraticProbingHash<PalavraEDica>).GetField("tombstone",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                if (tabelaField != null && tombstoneField != null)
                {
                    var tabela = (PalavraEDica[])tabelaField.GetValue(quadraticHash);
                    var tombstone = (bool[])tombstoneField.GetValue(quadraticHash);

                    // Encontra os índices ocupados primeiro
                    var indicesOcupados = new List<int>();
                    for (int i = 0; i < tabela.Length; i++)
                    {
                        if (tabela[i] != null || tombstone[i])
                        {
                            indicesOcupados.Add(i);
                        }
                    }
                    
                    // Se não há dados, mostra apenas os primeiros slots
                    if (indicesOcupados.Count == 0)
                    {
                        for (int i = 0; i < Math.Min(20, tabela.Length); i++)
                        {
                            lsbListagem.Rows.Add(i.ToString(), "-- vazio --", "");
                        }
                        if (tabela.Length > 20)
                        {
                            lsbListagem.Rows.Add("...", $"... mais {tabela.Length - 20} slots vazios", "...");
                        }
                        
                        // Atualiza informações no footer
                        AtualizarFooterInfo(tabela.Length, 0, 0);
                        return;
                    }
                    
                    // Mostra slots ocupados e alguns contextuais ao redor
                    var exibidos = new HashSet<int>();
                    
                    foreach (int indice in indicesOcupados.OrderBy(x => x))
                    {
                        // Mostra alguns slots antes e depois do ocupado para contexto
                        for (int j = Math.Max(0, indice - 2); j <= Math.Min(tabela.Length - 1, indice + 2); j++)
                        {
                            if (!exibidos.Contains(j))
                            {
                                exibidos.Add(j);
                            }
                        }
                    }
                    
                    // Exibe os slots em ordem crescente
                    foreach (int j in exibidos.OrderBy(x => x))
                    {
                        if (tabela[j] != null)
                        {
                            lsbListagem.Rows.Add(j.ToString(), tabela[j].Palavra ?? "", tabela[j].Dica ?? "");
                        }
                        else if (tombstone[j])
                        {
                            lsbListagem.Rows.Add(j.ToString(), "-- removido --", "(slot com tombstone)");
                        }
                        else
                        {
                            lsbListagem.Rows.Add(j.ToString(), "-- vazio --", "");
                        }
                    }
                    
                    // Calcula e atualiza informações no footer
                    int totalOcupados = indicesOcupados.Count(i => tabela[i] != null);
                    int totalTombstones = indicesOcupados.Count(i => tombstone[i]);
                    AtualizarFooterInfo(tabela.Length, totalOcupados, totalTombstones);
                }
                else
                {
                    var dados = hashTable.Conteudo();
                    foreach (var item in dados)
                    {
                        if (item != null)
                        {
                            lsbListagem.Rows.Add("N/A", item.Palavra ?? "", item.Dica ?? "");
                        }
                    }
                    AtualizarFooterInfo(0, dados.Count, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exibir ordem QuadraticProbing: {ex.Message}\nUsando exibição padrão.", 
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    
                var dados = hashTable.Conteudo();
                foreach (var item in dados)
                {
                    if (item != null)
                    {
                        lsbListagem.Rows.Add("N/A", item.Palavra ?? "", item.Dica ?? "");
                    }
                }
                AtualizarFooterInfo(0, dados.Count, 0);
            }
        }
        
        private void ExibirOrdemDoubleHashing(DoubleHashing<PalavraEDica> doubleHash)
        {
            try
            {
                var tabelaField = typeof(DoubleHashing<PalavraEDica>).GetField("tabela",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var tombstoneField = typeof(DoubleHashing<PalavraEDica>).GetField("tombstone",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                if (tabelaField != null && tombstoneField != null)
                {
                    var tabela = (PalavraEDica[])tabelaField.GetValue(doubleHash);
                    var tombstone = (bool[])tombstoneField.GetValue(doubleHash);

                    // Encontra os índices ocupados primeiro
                    var indicesOcupados = new List<int>();
                    for (int i = 0; i < tabela.Length; i++)
                    {
                        if (tabela[i] != null || tombstone[i])
                        {
                            indicesOcupados.Add(i);
                        }
                    }
                    
                    // Se não há dados, mostra apenas os primeiros slots
                    if (indicesOcupados.Count == 0)
                    {
                        for (int i = 0; i < Math.Min(20, tabela.Length); i++)
                        {
                            lsbListagem.Rows.Add(i.ToString(), "-- vazio --", "");
                        }
                        if (tabela.Length > 20)
                        {
                            lsbListagem.Rows.Add("...", $"... mais {tabela.Length - 20} slots vazios", "...");
                        }
                        
                        // Atualiza informações no footer
                        AtualizarFooterInfo(tabela.Length, 0, 0);
                        return;
                    }
                    
                    // Mostra slots ocupados e alguns contextuais ao redor
                    var exibidos = new HashSet<int>();
                    
                    foreach (int indice in indicesOcupados.OrderBy(x => x))
                    {
                        // Mostra alguns slots antes e depois do ocupado para contexto
                        for (int j = Math.Max(0, indice - 2); j <= Math.Min(tabela.Length - 1, indice + 2); j++)
                        {
                            if (!exibidos.Contains(j))
                            {
                                exibidos.Add(j);
                            }
                        }
                    }
                    
                    // Exibe os slots em ordem crescente
                    foreach (int j in exibidos.OrderBy(x => x))
                    {
                        if (tabela[j] != null)
                        {
                            lsbListagem.Rows.Add(j.ToString(), tabela[j].Palavra ?? "", tabela[j].Dica ?? "");
                        }
                        else if (tombstone[j])
                        {
                            lsbListagem.Rows.Add(j.ToString(), "-- removido --", "(slot com tombstone)");
                        }
                        else
                        {
                            lsbListagem.Rows.Add(j.ToString(), "-- vazio --", "");
                        }
                    }
                    
                    // Calcula e atualiza informações no footer
                    int totalOcupados = indicesOcupados.Count(i => tabela[i] != null);
                    int totalTombstones = indicesOcupados.Count(i => tombstone[i]);
                    AtualizarFooterInfo(tabela.Length, totalOcupados, totalTombstones);
                }
                else
                {
                    var dados = hashTable.Conteudo();
                    foreach (var item in dados)
                    {
                        if (item != null)
                        {
                            lsbListagem.Rows.Add("N/A", item.Palavra ?? "", item.Dica ?? "");
                        }
                    }
                    AtualizarFooterInfo(0, dados.Count, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exibir ordem DoubleHashing: {ex.Message}\nUsando exibição padrão.", 
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    
                var dados = hashTable.Conteudo();
                foreach (var item in dados)
                {
                    if (item != null)
                    {
                        lsbListagem.Rows.Add("N/A", item.Palavra ?? "", item.Dica ?? "");
                    }
                }
                AtualizarFooterInfo(0, dados.Count, 0);
            }
        }
        
        private void LsbListagem_SelectionChanged(object sender, EventArgs e)
        {
            if (lsbListagem.SelectedRows.Count > 0)
            {
                var selectedRow = lsbListagem.SelectedRows[0];
                var palavra = selectedRow.Cells["Palavra"].Value?.ToString() ?? "";
                var dica = selectedRow.Cells["Dica"].Value?.ToString() ?? "";
                
                if (palavra != "-- vazio --" && palavra != "-- removido --" && !palavra.StartsWith("..."))
                {
                    textBox_palavra.Text = palavra;
                    textBox_Dica.Text = dica;
                }
                else
                {
                    textBox_palavra.Clear();
                    textBox_Dica.Clear();
                }
            }
        }

        private void LoadDataFromFile(string filePath = null)
        {
            cachedData = new List<PalavraEDica>();
            
            try
            {
                string fileToLoad = filePath;
                
                if (string.IsNullOrEmpty(fileToLoad))
                {
                    // Fallback para o arquivo padrão se nenhum caminho for especificado
                    string[] possiblePaths = {
                        Path.Combine(Application.StartupPath, DATA_FILE_PATH),
                        Path.Combine(Directory.GetCurrentDirectory(), DATA_FILE_PATH),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DATA_FILE_PATH),
                        DATA_FILE_PATH
                    };
                    
                    foreach (string path in possiblePaths)
                    {
                        if (File.Exists(path))
                        {
                            fileToLoad = path;
                            break;
                        }
                    }
                }
                
                if (!string.IsNullOrEmpty(fileToLoad) && File.Exists(fileToLoad))
                {
                    using (StreamReader reader = new StreamReader(fileToLoad))
                    {
                        while (!reader.EndOfStream)
                        {
                            var palavra = new PalavraEDica();
                            palavra.LerRegistro(reader);
                            
                            // Verifica se leu dados válidos
                            if (!string.IsNullOrWhiteSpace(palavra.Palavra) && !string.IsNullOrWhiteSpace(palavra.Dica))
                            {
                                cachedData.Add(palavra);
                            }
                        }
                    }
                }
                else
                {
                    string fileName = Path.GetFileName(fileToLoad ?? DATA_FILE_PATH);
                    MessageBox.Show($"Arquivo '{fileName}' não encontrado.", 
                        "Arquivo não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados do arquivo: {ex.Message}", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LoadDataIntoHashTable()
        {
            if (cachedData != null && hashTable != null)
            {
                foreach (var item in cachedData)
                {
                    hashTable.Incluir(item);
                }
            }
        }

        private void ClearCurrentHashTable()
        {
            if (hashTable != null)
            {
                // Obtém todo o conteúdo da tabela hash
                var conteudo = hashTable.Conteudo();
                
                // Remove cada item usando o método Excluir da interface IHashing
                foreach (var item in conteudo)
                {
                    hashTable.Excluir(item);
                }
            }
        }

        private void AtualizarFooterInfo(int tamanhoTabela, int slotsOcupados, int slotsRemovidos)
        {
            int slotsVazios = tamanhoTabela - slotsOcupados - slotsRemovidos;
            
            lblTamanhoTabela.Text = $"Tamanho da tabela: {tamanhoTabela:N0}";
            lblSlotsOcupados.Text = $"Slots ocupados: {slotsOcupados}";
            lblSlotsRemovidos.Text = $"Slots removidos: {slotsRemovidos}";
            lblSlotsVazios.Text = $"Slots vazios: {slotsVazios:N0}";
        }

        private void AtualizarFooterInfoBucket(int totalBuckets, int bucketsUsados, int totalItens)
        {
            int bucketsVazios = totalBuckets - bucketsUsados;
            
            lblTamanhoTabela.Text = $"Total de buckets: {totalBuckets}";
            lblSlotsOcupados.Text = $"Buckets usados: {bucketsUsados}";
            lblSlotsRemovidos.Text = $"Total de itens: {totalItens}";
            lblSlotsVazios.Text = $"Buckets vazios: {bucketsVazios}";
        }
        
        private void LimparFooterInfo()
        {
            lblTamanhoTabela.Text = "Tamanho da tabela: -";
            lblSlotsOcupados.Text = "Slots ocupados: -";
            lblSlotsRemovidos.Text = "Slots removidos: -";
            lblSlotsVazios.Text = "Slots vazios: -";
        }
    }
}

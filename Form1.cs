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
            // Load data from file first
            LoadDataFromFile();
            
            // Start with BucketHash as default
            hashTable = new BucketHash<PalavraEDica>();
            LoadDataIntoHashTable();
            
            // Display loaded data automatically
            AtualizarListagem(); 
            
            // Set default radio button
            radio_BucketHash.Checked = true;
            
            // Optional: Show status message for debugging
            #if DEBUG
            int dataCount = cachedData?.Count ?? 0;
            int hashCount = hashTable?.Conteudo()?.Count ?? 0;
            MessageBox.Show($"Dados carregados: {dataCount} do arquivo, {hashCount} na tabela hash", 
                "Status de Inicialização", MessageBoxButtons.OK, MessageBoxIcon.Information);
            #endif
        }
        
        private void AttachEventHandlers()
        {
            // Attach radio button events
            radio_BucketHash.CheckedChanged += RadioButton_CheckedChanged;
            radio_Linear.CheckedChanged += RadioButton_CheckedChanged;
            radio_Quadratica.CheckedChanged += RadioButton_CheckedChanged;
            radio_duploHash.CheckedChanged += RadioButton_CheckedChanged;
            
            // Attach CRUD button events
            btn_Incluir.Click += Btn_Incluir_Click;
            btn_Excluir.Click += Btn_Excluir_Click;
            btn_Alterar.Click += Btn_Alterar_Click;
            btn_Listar.Click += Btn_Listar_Click;
            
            // Attach DataGridView selection event
            lsbListagem.SelectionChanged += LsbListagem_SelectionChanged;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (radio != null && radio.Checked)
            {
                // Clear information from previous operation
                textBox_palavra.Clear();
                textBox_Dica.Clear();
                lsbListagem.ClearSelection();

                // ✅ SOLUÇÃO: Simplesmente cria uma NOVA instância da hash table
                // Não precisa "limpar" a anterior, basta substituí-la
                if (radio == radio_BucketHash)
                    hashTable = new BucketHash<PalavraEDica>();
                else if (radio == radio_Linear)
                    hashTable = new LinearProbingHash<PalavraEDica>();
                else if (radio == radio_Quadratica)
                    hashTable = new QuadraticProbingHash<PalavraEDica>();
                else if (radio == radio_duploHash)
                    hashTable = new DoubleHashing<PalavraEDica>();

                // Load the cached data into the NEW hash table
                LoadDataIntoHashTable();

                // Refresh the list
                AtualizarListagem();
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
                
                // Remove the old entry and add the new one
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
                        // If inclusion fails, try to restore the original
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
                // Ensure hash table exists
                if (hashTable == null)
                {
                    return;
                }
                
                var dados = hashTable.Conteudo();
                
                // Configure DataGridView if not already configured
                if (lsbListagem.Columns.Count == 0)
                {
                    lsbListagem.AutoGenerateColumns = false;
                    lsbListagem.Columns.Add("Palavra", "Palavra");
                    lsbListagem.Columns.Add("Dica", "Dica");
                    lsbListagem.Columns["Palavra"].Width = 200;
                    lsbListagem.Columns["Dica"].Width = 400;
                    lsbListagem.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    lsbListagem.MultiSelect = false;
                    lsbListagem.ReadOnly = true;
                }
                
                // Clear existing rows
                lsbListagem.Rows.Clear();
                
                // Add data to DataGridView
                if (dados != null)
                {
                    foreach (var palavra in dados)
                    {
                        if (palavra != null)
                        {
                            lsbListagem.Rows.Add(palavra.Palavra ?? "", palavra.Dica ?? "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar listagem: {ex.Message}", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LsbListagem_SelectionChanged(object sender, EventArgs e)
        {
            if (lsbListagem.SelectedRows.Count > 0)
            {
                var selectedRow = lsbListagem.SelectedRows[0];
                textBox_palavra.Text = selectedRow.Cells["Palavra"].Value?.ToString() ?? "";
                textBox_Dica.Text = selectedRow.Cells["Dica"].Value?.ToString() ?? "";
            }
        }
        
        /// <summary>
        /// Carrega dados do arquivo .txt usando StreamReader e a classe PalavraEDica
        /// </summary>
        private void LoadDataFromFile()
        {
            cachedData = new List<PalavraEDica>();
            
            try
            {
                // Try multiple possible paths for the data file
                string[] possiblePaths = {
                    Path.Combine(Application.StartupPath, DATA_FILE_PATH),
                    Path.Combine(Directory.GetCurrentDirectory(), DATA_FILE_PATH),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DATA_FILE_PATH),
                    DATA_FILE_PATH  // Relative path as fallback
                };
                
                string filePath = null;
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        filePath = path;
                        break;
                    }
                }
                
                if (filePath != null)
                {
                    using (StreamReader reader = new StreamReader(filePath))
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
                    MessageBox.Show($"Arquivo '{DATA_FILE_PATH}' não encontrado. Verificar se o arquivo está no diretório da aplicação.", 
                        "Arquivo não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados do arquivo: {ex.Message}", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Carrega os dados em cache na tabela hash atual
        /// </summary>
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

        /// <summary>
        /// Limpa a tabela hash atual usando reflexão para chamar o método Limpar se disponível
        /// </summary>
        private void ClearCurrentHashTable()
        {
            if (hashTable != null)
            {
                var limparMethod = hashTable.GetType().GetMethod("Excluir");
                if (limparMethod != null)
                {
                    limparMethod.Invoke(hashTable, null);
                }
            }
        }
    }
}

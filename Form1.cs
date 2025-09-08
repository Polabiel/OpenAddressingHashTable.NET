using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenAddressingHashTable.NET
{
    public partial class Form1 : Form
    {
        private IHashing<PalavraEDica> hashTable;

        public Form1()
        {
            InitializeComponent();
            InitializeEventHandlers();
            InitializeHashTable();
            SetupDataGrid();
        }

        private void InitializeEventHandlers()
        {
            btn_Incluir.Click += Btn_Incluir_Click;
            btn_Alterar.Click += Btn_Alterar_Click;
            btn_Excluir.Click += Btn_Excluir_Click;
            btn_Listar.Click += Btn_Listar_Click;
            
            radio_BucketHash.CheckedChanged += RadioButton_CheckedChanged;
            radio_Linear.CheckedChanged += RadioButton_CheckedChanged;
            radio_Quadratica.CheckedChanged += RadioButton_CheckedChanged;
            radio_duploHash.CheckedChanged += RadioButton_CheckedChanged;
        }

        private void InitializeHashTable()
        {
            // Set default hash table to Bucket Hash
            radio_BucketHash.Checked = true;
            hashTable = new BucketHash<PalavraEDica>();
        }

        private void SetupDataGrid()
        {
            lsbListagem.AutoGenerateColumns = false;
            lsbListagem.Columns.Clear();
            
            var colPalavra = new DataGridViewTextBoxColumn
            {
                HeaderText = "Palavra",
                DataPropertyName = "Palavra",
                Width = 200
            };
            
            var colDica = new DataGridViewTextBoxColumn
            {
                HeaderText = "Dica",
                DataPropertyName = "Dica",
                Width = 400
            };
            
            lsbListagem.Columns.Add(colPalavra);
            lsbListagem.Columns.Add(colDica);
            
            // Add double-click event to populate text boxes
            lsbListagem.CellDoubleClick += LsbListagem_CellDoubleClick;
        }

        private void LsbListagem_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && lsbListagem.DataSource is List<PalavraEDica> lista)
            {
                var palavraSelecionada = lista[e.RowIndex];
                textBox_palavra.Text = palavraSelecionada.Palavra;
                textBox_Dica.Text = palavraSelecionada.Dica;
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton radio && radio.Checked)
            {
                // Save current data before switching
                var currentData = hashTable?.Conteudo() ?? new List<PalavraEDica>();
                
                // Create new hash table based on selection
                if (radio == radio_BucketHash)
                    hashTable = new BucketHash<PalavraEDica>();
                else if (radio == radio_Linear)
                    hashTable = new LinearProbingHash<PalavraEDica>();
                else if (radio == radio_Quadratica)
                    hashTable = new QuadraticProbingHash<PalavraEDica>();
                else if (radio == radio_duploHash)
                    hashTable = new DoubleHashing<PalavraEDica>();
                
                // Restore data to new hash table
                foreach (var item in currentData)
                {
                    hashTable.Incluir(item);
                }
                
                // Refresh display
                AtualizarListagem();
            }
        }

        private void Btn_Incluir_Click(object sender, EventArgs e)
        {
            try
            {
                string palavra = textBox_palavra.Text.Trim();
                string dica = textBox_Dica.Text.Trim();
                
                if (string.IsNullOrEmpty(palavra))
                {
                    MessageBox.Show("Por favor, digite uma palavra.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox_palavra.Focus();
                    return;
                }
                
                if (string.IsNullOrEmpty(dica))
                {
                    MessageBox.Show("Por favor, digite uma dica.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox_Dica.Focus();
                    return;
                }
                
                var novaPalavra = new PalavraEDica(palavra, dica);
                
                if (hashTable.Incluir(novaPalavra))
                {
                    MessageBox.Show("Palavra incluída com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparCampos();
                    AtualizarListagem();
                }
                else
                {
                    MessageBox.Show("Esta palavra já existe na tabela.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao incluir palavra: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Alterar_Click(object sender, EventArgs e)
        {
            try
            {
                string palavra = textBox_palavra.Text.Trim();
                string novaDica = textBox_Dica.Text.Trim();
                
                if (string.IsNullOrEmpty(palavra))
                {
                    MessageBox.Show("Por favor, digite a palavra que deseja alterar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox_palavra.Focus();
                    return;
                }
                
                if (string.IsNullOrEmpty(novaDica))
                {
                    MessageBox.Show("Por favor, digite a nova dica.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox_Dica.Focus();
                    return;
                }
                
                var palavraParaBuscar = new PalavraEDica(palavra, "");
                int onde;
                
                if (hashTable.Existe(palavraParaBuscar, out onde))
                {
                    // Remove the old entry and add the new one
                    var palavraAntiga = hashTable.Conteudo().FirstOrDefault(p => 
                        string.Equals(p.Palavra, palavra, StringComparison.OrdinalIgnoreCase));
                    
                    if (palavraAntiga != null)
                    {
                        hashTable.Excluir(palavraAntiga);
                        var palavraAtualizada = new PalavraEDica(palavra, novaDica);
                        hashTable.Incluir(palavraAtualizada);
                        
                        MessageBox.Show("Palavra alterada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparCampos();
                        AtualizarListagem();
                    }
                }
                else
                {
                    MessageBox.Show("Palavra não encontrada na tabela.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao alterar palavra: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Excluir_Click(object sender, EventArgs e)
        {
            try
            {
                string palavra = textBox_palavra.Text.Trim();
                
                if (string.IsNullOrEmpty(palavra))
                {
                    MessageBox.Show("Por favor, digite a palavra que deseja excluir.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox_palavra.Focus();
                    return;
                }
                
                var palavraParaExcluir = hashTable.Conteudo().FirstOrDefault(p => 
                    string.Equals(p.Palavra, palavra, StringComparison.OrdinalIgnoreCase));
                
                if (palavraParaExcluir != null)
                {
                    var result = MessageBox.Show($"Tem certeza que deseja excluir a palavra '{palavra}'?", 
                        "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        if (hashTable.Excluir(palavraParaExcluir))
                        {
                            MessageBox.Show("Palavra excluída com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LimparCampos();
                            AtualizarListagem();
                        }
                        else
                        {
                            MessageBox.Show("Erro ao excluir palavra.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Palavra não encontrada na tabela.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir palavra: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Listar_Click(object sender, EventArgs e)
        {
            AtualizarListagem();
        }

        private void LimparCampos()
        {
            textBox_palavra.Clear();
            textBox_Dica.Clear();
            textBox_palavra.Focus();
        }

        private void AtualizarListagem()
        {
            try
            {
                var conteudo = hashTable.Conteudo();
                lsbListagem.DataSource = conteudo.OrderBy(p => p.Palavra).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar listagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

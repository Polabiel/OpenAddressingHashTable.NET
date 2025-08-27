namespace OpenAddressingHashTable.NET
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.radio_BucketHash = new System.Windows.Forms.RadioButton();
            this.radio_Linear = new System.Windows.Forms.RadioButton();
            this.radio_Quadratica = new System.Windows.Forms.RadioButton();
            this.radio_duploHash = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_palavra = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Dica = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_Excluir = new System.Windows.Forms.Button();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_Alterar = new System.Windows.Forms.Button();
            this.btn_Incluir = new System.Windows.Forms.Button();
            this.btn_Listar = new System.Windows.Forms.Button();
            this.lsbListagem = new System.Windows.Forms.DataGridView();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lsbListagem)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.radio_BucketHash);
            this.flowLayoutPanel1.Controls.Add(this.radio_Linear);
            this.flowLayoutPanel1.Controls.Add(this.radio_Quadratica);
            this.flowLayoutPanel1.Controls.Add(this.radio_duploHash);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(135, 115);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Técnicas de Hashing";
            // 
            // radio_BucketHash
            // 
            this.radio_BucketHash.AutoSize = true;
            this.radio_BucketHash.Location = new System.Drawing.Point(3, 16);
            this.radio_BucketHash.Name = "radio_BucketHash";
            this.radio_BucketHash.Size = new System.Drawing.Size(87, 17);
            this.radio_BucketHash.TabIndex = 1;
            this.radio_BucketHash.TabStop = true;
            this.radio_BucketHash.Text = "Bucket Hash";
            this.radio_BucketHash.UseVisualStyleBackColor = true;
            // 
            // radio_Linear
            // 
            this.radio_Linear.AutoSize = true;
            this.radio_Linear.Location = new System.Drawing.Point(3, 39);
            this.radio_Linear.Name = "radio_Linear";
            this.radio_Linear.Size = new System.Drawing.Size(108, 17);
            this.radio_Linear.TabIndex = 2;
            this.radio_Linear.TabStop = true;
            this.radio_Linear.Text = "Sondagem Linear";
            this.radio_Linear.UseVisualStyleBackColor = true;
            // 
            // radio_Quadratica
            // 
            this.radio_Quadratica.AutoSize = true;
            this.radio_Quadratica.Location = new System.Drawing.Point(3, 62);
            this.radio_Quadratica.Name = "radio_Quadratica";
            this.radio_Quadratica.Size = new System.Drawing.Size(131, 17);
            this.radio_Quadratica.TabIndex = 3;
            this.radio_Quadratica.TabStop = true;
            this.radio_Quadratica.Text = "Sondagem Quadrática";
            this.radio_Quadratica.UseVisualStyleBackColor = true;
            // 
            // radio_duploHash
            // 
            this.radio_duploHash.AutoSize = true;
            this.radio_duploHash.Location = new System.Drawing.Point(3, 85);
            this.radio_duploHash.Name = "radio_duploHash";
            this.radio_duploHash.Size = new System.Drawing.Size(81, 17);
            this.radio_duploHash.TabIndex = 4;
            this.radio_duploHash.TabStop = true;
            this.radio_duploHash.Text = "Duplo Hash";
            this.radio_duploHash.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.label2);
            this.flowLayoutPanel2.Controls.Add(this.textBox_palavra);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(173, 14);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(480, 31);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Palavra:";
            // 
            // textBox_palavra
            // 
            this.textBox_palavra.Location = new System.Drawing.Point(55, 3);
            this.textBox_palavra.Name = "textBox_palavra";
            this.textBox_palavra.Size = new System.Drawing.Size(411, 20);
            this.textBox_palavra.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Dica:";
            // 
            // textBox_Dica
            // 
            this.textBox_Dica.Location = new System.Drawing.Point(41, 3);
            this.textBox_Dica.Name = "textBox_Dica";
            this.textBox_Dica.Size = new System.Drawing.Size(425, 20);
            this.textBox_Dica.TabIndex = 1;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.label3);
            this.flowLayoutPanel3.Controls.Add(this.textBox_Dica);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(173, 51);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(480, 31);
            this.flowLayoutPanel3.TabIndex = 2;
            // 
            // btn_Excluir
            // 
            this.btn_Excluir.Location = new System.Drawing.Point(3, 3);
            this.btn_Excluir.Name = "btn_Excluir";
            this.btn_Excluir.Size = new System.Drawing.Size(75, 23);
            this.btn_Excluir.TabIndex = 3;
            this.btn_Excluir.Text = "Excluir";
            this.btn_Excluir.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.btn_Excluir);
            this.flowLayoutPanel4.Controls.Add(this.btn_Alterar);
            this.flowLayoutPanel4.Controls.Add(this.btn_Incluir);
            this.flowLayoutPanel4.Controls.Add(this.btn_Listar);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(173, 97);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(328, 34);
            this.flowLayoutPanel4.TabIndex = 4;
            // 
            // btn_Alterar
            // 
            this.btn_Alterar.Location = new System.Drawing.Point(84, 3);
            this.btn_Alterar.Name = "btn_Alterar";
            this.btn_Alterar.Size = new System.Drawing.Size(75, 23);
            this.btn_Alterar.TabIndex = 4;
            this.btn_Alterar.Text = "Alterar";
            this.btn_Alterar.UseVisualStyleBackColor = true;
            // 
            // btn_Incluir
            // 
            this.btn_Incluir.Location = new System.Drawing.Point(165, 3);
            this.btn_Incluir.Name = "btn_Incluir";
            this.btn_Incluir.Size = new System.Drawing.Size(75, 23);
            this.btn_Incluir.TabIndex = 5;
            this.btn_Incluir.Text = "Incluir";
            this.btn_Incluir.UseVisualStyleBackColor = true;
            // 
            // btn_Listar
            // 
            this.btn_Listar.Location = new System.Drawing.Point(246, 3);
            this.btn_Listar.Name = "btn_Listar";
            this.btn_Listar.Size = new System.Drawing.Size(75, 23);
            this.btn_Listar.TabIndex = 6;
            this.btn_Listar.Text = "Listar";
            this.btn_Listar.UseVisualStyleBackColor = true;
            // 
            // lsbListagem
            // 
            this.lsbListagem.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lsbListagem.Location = new System.Drawing.Point(18, 147);
            this.lsbListagem.Name = "lsbListagem";
            this.lsbListagem.Size = new System.Drawing.Size(635, 207);
            this.lsbListagem.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 376);
            this.Controls.Add(this.lsbListagem);
            this.Controls.Add(this.flowLayoutPanel4);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.flowLayoutPanel3);
            this.Name = "Form1";
            this.Text = "Manuteção de palavras e Dicas ";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lsbListagem)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radio_BucketHash;
        private System.Windows.Forms.RadioButton radio_Linear;
        private System.Windows.Forms.RadioButton radio_Quadratica;
        private System.Windows.Forms.RadioButton radio_duploHash;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_palavra;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Dica;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button btn_Excluir;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Button btn_Alterar;
        private System.Windows.Forms.Button btn_Incluir;
        private System.Windows.Forms.Button btn_Listar;
        private System.Windows.Forms.DataGridView lsbListagem;
    }
}


// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SWTaskpaneHost.Designer.cs" company="da">
//   da
// </copyright>
// <summary>
//   Defines the SWTaskpaneHost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AngelSix
{
    /// <summary>
    /// The taskpane drawn on SolidWorks.
    /// </summary>
    public partial class SWTaskpaneHost
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SWTaskpaneHost));
            this.bOttieniDatiSelezione = new System.Windows.Forms.Button();
            this.bSimilarity = new System.Windows.Forms.Button();
            this.bProporzioni = new System.Windows.Forms.Button();
            this.bEsatto = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BIncastro = new System.Windows.Forms.Button();
            this.Resu = new System.Windows.Forms.ListBox();
            this.CartellaModelli = new System.Windows.Forms.FolderBrowserDialog();
            this.Cancella = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.FolderSelection = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bOttieniDatiSelezione
            // 
            this.bOttieniDatiSelezione.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bOttieniDatiSelezione.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.bOttieniDatiSelezione.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bOttieniDatiSelezione.Location = new System.Drawing.Point(59, 108);
            this.bOttieniDatiSelezione.Name = "bOttieniDatiSelezione";
            this.bOttieniDatiSelezione.Padding = new System.Windows.Forms.Padding(5, 0, 10, 0);
            this.bOttieniDatiSelezione.Size = new System.Drawing.Size(125, 34);
            this.bOttieniDatiSelezione.TabIndex = 7;
            this.bOttieniDatiSelezione.Text = "Load selection";
            this.bOttieniDatiSelezione.UseVisualStyleBackColor = true;
            this.bOttieniDatiSelezione.Click += new System.EventHandler(this.BOttieniDatiSelezioneClick);
            // 
            // bSimilarity
            // 
            this.bSimilarity.Location = new System.Drawing.Point(60, 184);
            this.bSimilarity.Name = "bSimilarity";
            this.bSimilarity.Size = new System.Drawing.Size(124, 49);
            this.bSimilarity.TabIndex = 8;
            this.bSimilarity.Text = "Search for similar shape";
            this.bSimilarity.UseVisualStyleBackColor = true;
            this.bSimilarity.Click += new System.EventHandler(this.BSimilarityClick);
            // 
            // bProporzioni
            // 
            this.bProporzioni.Location = new System.Drawing.Point(60, 250);
            this.bProporzioni.Name = "bProporzioni";
            this.bProporzioni.Size = new System.Drawing.Size(124, 49);
            this.bProporzioni.TabIndex = 9;
            this.bProporzioni.Text = "Search for similar shape with size constraints";
            this.bProporzioni.UseVisualStyleBackColor = true;
            this.bProporzioni.Click += new System.EventHandler(this.BProporzioniClick);
            // 
            // bEsatto
            // 
            this.bEsatto.Location = new System.Drawing.Point(224, 250);
            this.bEsatto.Name = "bEsatto";
            this.bEsatto.Size = new System.Drawing.Size(124, 49);
            this.bEsatto.TabIndex = 10;
            this.bEsatto.Text = "Search for mating shape with size constraints";
            this.bEsatto.UseVisualStyleBackColor = true;
            this.bEsatto.Click += new System.EventHandler(this.BEsattoClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 16);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(118, 46);
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(225, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(168, 59);
            this.pictureBox2.TabIndex = 17;
            this.pictureBox2.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Location = new System.Drawing.Point(0, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(395, 68);
            this.panel1.TabIndex = 18;
            // 
            // BIncastro
            // 
            this.BIncastro.Location = new System.Drawing.Point(224, 184);
            this.BIncastro.Name = "BIncastro";
            this.BIncastro.Size = new System.Drawing.Size(124, 49);
            this.BIncastro.TabIndex = 21;
            this.BIncastro.Text = "Search for mating shape";
            this.BIncastro.UseVisualStyleBackColor = true;
            this.BIncastro.Click += new System.EventHandler(this.BIncastroClick);
            // 
            // Resu
            // 
            this.Resu.FormattingEnabled = true;
            this.Resu.HorizontalExtent = 1000;
            this.Resu.HorizontalScrollbar = true;
            this.Resu.Location = new System.Drawing.Point(28, 317);
            this.Resu.Name = "Resu";
            this.Resu.Size = new System.Drawing.Size(343, 238);
            this.Resu.TabIndex = 22;
            this.Resu.SelectedIndexChanged += new System.EventHandler(this.ListaRisultatiSelectedIndexChanged);
            // 
            // CartellaModelli
            // 
            this.CartellaModelli.SelectedPath = "C:\\Users\\SoullessPG\\Desktop";
            this.CartellaModelli.HelpRequest += new System.EventHandler(this.CartellaModelliHelpRequest);
            // 
            // Cancella
            // 
            this.Cancella.Location = new System.Drawing.Point(28, 554);
            this.Cancella.Name = "Cancella";
            this.Cancella.Size = new System.Drawing.Size(84, 21);
            this.Cancella.TabIndex = 23;
            this.Cancella.Text = "Erase";
            this.Cancella.UseVisualStyleBackColor = true;
            this.Cancella.Click += new System.EventHandler(this.CancellaClick);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.Location = new System.Drawing.Point(224, 108);
            this.button2.Name = "button2";
            this.button2.Padding = new System.Windows.Forms.Padding(5, 0, 10, 0);
            this.button2.Size = new System.Drawing.Size(125, 34);
            this.button2.TabIndex = 27;
            this.button2.Text = "Load full model";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FolderSelection
            // 
            this.FolderSelection.Location = new System.Drawing.Point(61, 148);
            this.FolderSelection.Name = "FolderSelection";
            this.FolderSelection.Size = new System.Drawing.Size(288, 23);
            this.FolderSelection.TabIndex = 28;
            this.FolderSelection.Text = "Choose folder including comparison models";
            this.FolderSelection.UseVisualStyleBackColor = true;
            this.FolderSelection.Click += new System.EventHandler(this.FolderSelection_Click);
            // 
            // SWTaskpaneHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.Controls.Add(this.FolderSelection);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.Cancella);
            this.Controls.Add(this.Resu);
            this.Controls.Add(this.BIncastro);
            this.Controls.Add(this.bEsatto);
            this.Controls.Add(this.bProporzioni);
            this.Controls.Add(this.bSimilarity);
            this.Controls.Add(this.bOttieniDatiSelezione);
            this.Controls.Add(this.panel1);
            this.Name = "SWTaskpaneHost";
            this.Size = new System.Drawing.Size(398, 578);
            this.Load += new System.EventHandler(this.SWTaskpaneHostLoad);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bOttieniDatiSelezione;
        private System.Windows.Forms.Button bSimilarity;
        private System.Windows.Forms.Button bProporzioni;
        private System.Windows.Forms.Button bEsatto;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BIncastro;
        private System.Windows.Forms.FolderBrowserDialog CartellaModelli;
        private System.Windows.Forms.Button Cancella;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox Resu;
        private System.Windows.Forms.Button FolderSelection;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}

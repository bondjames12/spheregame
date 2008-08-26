namespace X_Editor
{
    partial class AddContentWindow
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label5 = new System.Windows.Forms.Label();
            this.filename = new System.Windows.Forms.TextBox();
            this.browse = new System.Windows.Forms.Button();
            this.build = new System.Windows.Forms.Button();
            this.importer = new System.Windows.Forms.ComboBox();
            this.processor = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Content Importer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Content Processor";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Input File";
            // 
            // filename
            // 
            this.filename.Location = new System.Drawing.Point(112, 12);
            this.filename.Name = "filename";
            this.filename.Size = new System.Drawing.Size(197, 20);
            this.filename.TabIndex = 6;
            // 
            // browse
            // 
            this.browse.Location = new System.Drawing.Point(315, 10);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(75, 23);
            this.browse.TabIndex = 8;
            this.browse.Text = "Browse";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // build
            // 
            this.build.Location = new System.Drawing.Point(12, 102);
            this.build.Name = "build";
            this.build.Size = new System.Drawing.Size(98, 23);
            this.build.TabIndex = 9;
            this.build.Text = "Add Content";
            this.build.UseVisualStyleBackColor = true;
            this.build.Click += new System.EventHandler(this.build_Click);
            // 
            // importer
            // 
            this.importer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.importer.FormattingEnabled = true;
            this.importer.Items.AddRange(new object[] {
            "Importer",
            "TextureImporter",
            "FbxImporter",
            "XImporter",
            "EffectImporter",
            "None"});
            this.importer.Location = new System.Drawing.Point(112, 39);
            this.importer.MaxDropDownItems = 25;
            this.importer.Name = "importer";
            this.importer.Size = new System.Drawing.Size(278, 21);
            this.importer.TabIndex = 10;
            // 
            // processor
            // 
            this.processor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.processor.FormattingEnabled = true;
            this.processor.Items.AddRange(new object[] {
            "Processor",
            "TextureProcessor",
            "EffectProcessor",
            "ModelProcessor",
            "None"});
            this.processor.Location = new System.Drawing.Point(112, 65);
            this.processor.MaxDropDownItems = 25;
            this.processor.Name = "processor";
            this.processor.Size = new System.Drawing.Size(278, 21);
            this.processor.TabIndex = 11;
            // 
            // AddContentWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 137);
            this.Controls.Add(this.processor);
            this.Controls.Add(this.importer);
            this.Controls.Add(this.build);
            this.Controls.Add(this.browse);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.filename);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddContentWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Add Content";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox filename;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.Button build;
        public System.Windows.Forms.ComboBox importer;
        public System.Windows.Forms.ComboBox processor;
    }
}
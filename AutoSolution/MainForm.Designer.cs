namespace Axantum.AutoSolution
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRootDirectory = new System.Windows.Forms.TextBox();
            this.checkBoxIncludeCs = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControlFeature = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBoxMode = new System.Windows.Forms.GroupBox();
            this.radioButtonSuperSolution = new System.Windows.Forms.RadioButton();
            this.radioButtonRecursive = new System.Windows.Forms.RadioButton();
            this.buttonRun = new System.Windows.Forms.Button();
            this.checkBoxNoForceAuto = new System.Windows.Forms.CheckBox();
            this.checkBoxIncludeVb = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBoxCleanAutoSolutions = new System.Windows.Forms.CheckBox();
            this.checkBoxCleanObj = new System.Windows.Forms.CheckBox();
            this.checkBoxBin = new System.Windows.Forms.CheckBox();
            this.buttonClean = new System.Windows.Forms.Button();
            this.textBoxProgress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxIgnoreFolders = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSuperSolutionNameBase = new System.Windows.Forms.TextBox();
            this.tabControlFeature.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBoxMode.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Start Directory";
            // 
            // textBoxRootDirectory
            // 
            this.textBoxRootDirectory.Location = new System.Drawing.Point(92, 13);
            this.textBoxRootDirectory.Name = "textBoxRootDirectory";
            this.textBoxRootDirectory.Size = new System.Drawing.Size(450, 20);
            this.textBoxRootDirectory.TabIndex = 15;
            this.textBoxRootDirectory.TextChanged += new System.EventHandler(this.textBoxRootDirectory_TextChanged);
            // 
            // checkBoxIncludeCs
            // 
            this.checkBoxIncludeCs.AutoSize = true;
            this.checkBoxIncludeCs.Location = new System.Drawing.Point(6, 7);
            this.checkBoxIncludeCs.Name = "checkBoxIncludeCs";
            this.checkBoxIncludeCs.Size = new System.Drawing.Size(118, 17);
            this.checkBoxIncludeCs.TabIndex = 8;
            this.checkBoxIncludeCs.Text = "Include C# projects";
            this.checkBoxIncludeCs.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(548, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabControlFeature
            // 
            this.tabControlFeature.Controls.Add(this.tabPage1);
            this.tabControlFeature.Controls.Add(this.tabPage2);
            this.tabControlFeature.Location = new System.Drawing.Point(12, 80);
            this.tabControlFeature.Name = "tabControlFeature";
            this.tabControlFeature.SelectedIndex = 0;
            this.tabControlFeature.Size = new System.Drawing.Size(433, 216);
            this.tabControlFeature.TabIndex = 19;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBoxMode);
            this.tabPage1.Controls.Add(this.buttonRun);
            this.tabPage1.Controls.Add(this.checkBoxNoForceAuto);
            this.tabPage1.Controls.Add(this.checkBoxIncludeVb);
            this.tabPage1.Controls.Add(this.checkBoxIncludeCs);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(425, 190);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Solutions";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBoxMode
            // 
            this.groupBoxMode.Controls.Add(this.textBoxSuperSolutionNameBase);
            this.groupBoxMode.Controls.Add(this.label2);
            this.groupBoxMode.Controls.Add(this.radioButtonSuperSolution);
            this.groupBoxMode.Controls.Add(this.radioButtonRecursive);
            this.groupBoxMode.Location = new System.Drawing.Point(6, 79);
            this.groupBoxMode.Name = "groupBoxMode";
            this.groupBoxMode.Size = new System.Drawing.Size(413, 76);
            this.groupBoxMode.TabIndex = 13;
            this.groupBoxMode.TabStop = false;
            this.groupBoxMode.Text = "Mode";
            // 
            // radioButtonSuperSolution
            // 
            this.radioButtonSuperSolution.AutoSize = true;
            this.radioButtonSuperSolution.Checked = true;
            this.radioButtonSuperSolution.Location = new System.Drawing.Point(7, 43);
            this.radioButtonSuperSolution.Name = "radioButtonSuperSolution";
            this.radioButtonSuperSolution.Size = new System.Drawing.Size(112, 17);
            this.radioButtonSuperSolution.TabIndex = 13;
            this.radioButtonSuperSolution.TabStop = true;
            this.radioButtonSuperSolution.Text = "One Root Solution";
            this.radioButtonSuperSolution.UseVisualStyleBackColor = true;
            // 
            // radioButtonRecursive
            // 
            this.radioButtonRecursive.AutoSize = true;
            this.radioButtonRecursive.Location = new System.Drawing.Point(6, 19);
            this.radioButtonRecursive.Name = "radioButtonRecursive";
            this.radioButtonRecursive.Size = new System.Drawing.Size(73, 17);
            this.radioButtonRecursive.TabIndex = 12;
            this.radioButtonRecursive.Text = "Recursive";
            this.radioButtonRecursive.UseVisualStyleBackColor = true;
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(6, 161);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 11;
            this.buttonRun.Text = "Run!";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // checkBoxNoForceAuto
            // 
            this.checkBoxNoForceAuto.AutoSize = true;
            this.checkBoxNoForceAuto.Location = new System.Drawing.Point(6, 53);
            this.checkBoxNoForceAuto.Name = "checkBoxNoForceAuto";
            this.checkBoxNoForceAuto.Size = new System.Drawing.Size(260, 17);
            this.checkBoxNoForceAuto.TabIndex = 10;
            this.checkBoxNoForceAuto.Text = "Do not generate new projects if references are ok";
            this.checkBoxNoForceAuto.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncludeVb
            // 
            this.checkBoxIncludeVb.AutoSize = true;
            this.checkBoxIncludeVb.Location = new System.Drawing.Point(6, 30);
            this.checkBoxIncludeVb.Name = "checkBoxIncludeVb";
            this.checkBoxIncludeVb.Size = new System.Drawing.Size(143, 17);
            this.checkBoxIncludeVb.TabIndex = 9;
            this.checkBoxIncludeVb.Text = "Include VB.NET projects";
            this.checkBoxIncludeVb.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.checkBoxCleanAutoSolutions);
            this.tabPage2.Controls.Add(this.checkBoxCleanObj);
            this.tabPage2.Controls.Add(this.checkBoxBin);
            this.tabPage2.Controls.Add(this.buttonClean);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(425, 190);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Clean";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBoxCleanAutoSolutions
            // 
            this.checkBoxCleanAutoSolutions.AutoSize = true;
            this.checkBoxCleanAutoSolutions.Checked = true;
            this.checkBoxCleanAutoSolutions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCleanAutoSolutions.Location = new System.Drawing.Point(25, 56);
            this.checkBoxCleanAutoSolutions.Name = "checkBoxCleanAutoSolutions";
            this.checkBoxCleanAutoSolutions.Size = new System.Drawing.Size(234, 17);
            this.checkBoxCleanAutoSolutions.TabIndex = 11;
            this.checkBoxCleanAutoSolutions.Text = "Clean *.Auto.sln, *.Auto.vbproj, *.Auto.csproj";
            this.checkBoxCleanAutoSolutions.UseVisualStyleBackColor = true;
            this.checkBoxCleanAutoSolutions.CheckedChanged += new System.EventHandler(this.CleanPresenterParameterChanged);
            // 
            // checkBoxCleanObj
            // 
            this.checkBoxCleanObj.AutoSize = true;
            this.checkBoxCleanObj.Checked = true;
            this.checkBoxCleanObj.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCleanObj.Location = new System.Drawing.Point(25, 32);
            this.checkBoxCleanObj.Name = "checkBoxCleanObj";
            this.checkBoxCleanObj.Size = new System.Drawing.Size(79, 17);
            this.checkBoxCleanObj.TabIndex = 10;
            this.checkBoxCleanObj.Text = "Clean obj\\*";
            this.checkBoxCleanObj.UseVisualStyleBackColor = true;
            this.checkBoxCleanObj.CheckedChanged += new System.EventHandler(this.CleanPresenterParameterChanged);
            // 
            // checkBoxBin
            // 
            this.checkBoxBin.AutoSize = true;
            this.checkBoxBin.Checked = true;
            this.checkBoxBin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBin.Location = new System.Drawing.Point(25, 8);
            this.checkBoxBin.Name = "checkBoxBin";
            this.checkBoxBin.Size = new System.Drawing.Size(79, 17);
            this.checkBoxBin.TabIndex = 9;
            this.checkBoxBin.Text = "Clean bin\\*";
            this.checkBoxBin.UseVisualStyleBackColor = true;
            this.checkBoxBin.CheckedChanged += new System.EventHandler(this.CleanPresenterParameterChanged);
            // 
            // buttonClean
            // 
            this.buttonClean.Location = new System.Drawing.Point(25, 90);
            this.buttonClean.Name = "buttonClean";
            this.buttonClean.Size = new System.Drawing.Size(75, 23);
            this.buttonClean.TabIndex = 8;
            this.buttonClean.Text = "Clean!";
            this.buttonClean.UseVisualStyleBackColor = true;
            this.buttonClean.Click += new System.EventHandler(this.buttonClean_Click);
            // 
            // textBoxProgress
            // 
            this.textBoxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProgress.Location = new System.Drawing.Point(12, 302);
            this.textBoxProgress.MinimumSize = new System.Drawing.Size(200, 100);
            this.textBoxProgress.Multiline = true;
            this.textBoxProgress.Name = "textBoxProgress";
            this.textBoxProgress.ReadOnly = true;
            this.textBoxProgress.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxProgress.Size = new System.Drawing.Size(581, 229);
            this.textBoxProgress.TabIndex = 18;
            this.textBoxProgress.WordWrap = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Ignore folders";
            // 
            // textBoxIgnoreFolders
            // 
            this.textBoxIgnoreFolders.Location = new System.Drawing.Point(93, 40);
            this.textBoxIgnoreFolders.Name = "textBoxIgnoreFolders";
            this.textBoxIgnoreFolders.Size = new System.Drawing.Size(449, 20);
            this.textBoxIgnoreFolders.TabIndex = 22;
            this.textBoxIgnoreFolders.TextChanged += new System.EventHandler(this.textBoxIgnoreFolders_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(134, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Solution Name Base:";
            // 
            // textBoxSuperSolutionNameBase
            // 
            this.textBoxSuperSolutionNameBase.Location = new System.Drawing.Point(246, 42);
            this.textBoxSuperSolutionNameBase.Name = "textBoxSuperSolutionNameBase";
            this.textBoxSuperSolutionNameBase.Size = new System.Drawing.Size(161, 20);
            this.textBoxSuperSolutionNameBase.TabIndex = 15;
            this.textBoxSuperSolutionNameBase.TextChanged += new System.EventHandler(this.textBoxSuperSolutionNameBase_TextChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 543);
            this.Controls.Add(this.textBoxIgnoreFolders);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxRootDirectory);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControlFeature);
            this.Controls.Add(this.textBoxProgress);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Axantum Automatic Solution and Project Reference Generator";
            this.tabControlFeature.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBoxMode.ResumeLayout(false);
            this.groupBoxMode.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxRootDirectory;
        private System.Windows.Forms.CheckBox checkBoxIncludeCs;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabControlFeature;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.CheckBox checkBoxNoForceAuto;
        private System.Windows.Forms.CheckBox checkBoxIncludeVb;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox checkBoxCleanAutoSolutions;
        private System.Windows.Forms.CheckBox checkBoxCleanObj;
        private System.Windows.Forms.CheckBox checkBoxBin;
        private System.Windows.Forms.Button buttonClean;
        private System.Windows.Forms.TextBox textBoxProgress;
        private System.Windows.Forms.GroupBox groupBoxMode;
        private System.Windows.Forms.RadioButton radioButtonSuperSolution;
        private System.Windows.Forms.RadioButton radioButtonRecursive;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxIgnoreFolders;
        private System.Windows.Forms.TextBox textBoxSuperSolutionNameBase;
        private System.Windows.Forms.Label label2;
    }
}


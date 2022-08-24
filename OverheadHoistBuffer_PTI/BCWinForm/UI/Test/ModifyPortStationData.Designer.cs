namespace com.mirle.ibg3k0.bc.winform.UI.Test
{
    partial class ModifyPortStationData
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
            this.dgvAPORTSTATIONData = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnGetData = new System.Windows.Forms.Button();
            this.btnToLoadReq = new System.Windows.Forms.Button();
            this.btnToInServ = new System.Windows.Forms.Button();
            this.btnToErrOn = new System.Windows.Forms.Button();
            this.btnToUnloadReq = new System.Windows.Forms.Button();
            this.btnToNoReq = new System.Windows.Forms.Button();
            this.btnToOutServ = new System.Windows.Forms.Button();
            this.btnToErrOff = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAPORTSTATIONData)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAPORTSTATIONData
            // 
            this.dgvAPORTSTATIONData.AllowUserToAddRows = false;
            this.dgvAPORTSTATIONData.AllowUserToDeleteRows = false;
            this.dgvAPORTSTATIONData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAPORTSTATIONData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dgvAPORTSTATIONData.Location = new System.Drawing.Point(33, 204);
            this.dgvAPORTSTATIONData.MultiSelect = false;
            this.dgvAPORTSTATIONData.Name = "dgvAPORTSTATIONData";
            this.dgvAPORTSTATIONData.ReadOnly = true;
            this.dgvAPORTSTATIONData.RowTemplate.Height = 24;
            this.dgvAPORTSTATIONData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAPORTSTATIONData.Size = new System.Drawing.Size(908, 150);
            this.dgvAPORTSTATIONData.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "PORT_ID";
            this.Column1.HeaderText = "Port ID";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "PORT_TYPE";
            this.Column2.HeaderText = "Request Status";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "PORT_SERVICE_STATUS";
            this.Column3.HeaderText = "Service Status";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "ERROR_FLAG";
            this.Column4.HeaderText = "Error";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // btnGetData
            // 
            this.btnGetData.Location = new System.Drawing.Point(33, 24);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(82, 32);
            this.btnGetData.TabIndex = 1;
            this.btnGetData.Text = "Get Data";
            this.btnGetData.UseVisualStyleBackColor = true;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // btnToLoadReq
            // 
            this.btnToLoadReq.Location = new System.Drawing.Point(171, 24);
            this.btnToLoadReq.Name = "btnToLoadReq";
            this.btnToLoadReq.Size = new System.Drawing.Size(90, 32);
            this.btnToLoadReq.TabIndex = 2;
            this.btnToLoadReq.Text = "LoadReq";
            this.btnToLoadReq.UseVisualStyleBackColor = true;
            this.btnToLoadReq.Click += new System.EventHandler(this.btnToLoadReq_Click);
            // 
            // btnToInServ
            // 
            this.btnToInServ.Location = new System.Drawing.Point(267, 24);
            this.btnToInServ.Name = "btnToInServ";
            this.btnToInServ.Size = new System.Drawing.Size(90, 32);
            this.btnToInServ.TabIndex = 3;
            this.btnToInServ.Text = "InService";
            this.btnToInServ.UseVisualStyleBackColor = true;
            this.btnToInServ.Click += new System.EventHandler(this.btnToInServ_Click);
            // 
            // btnToErrOn
            // 
            this.btnToErrOn.Location = new System.Drawing.Point(363, 24);
            this.btnToErrOn.Name = "btnToErrOn";
            this.btnToErrOn.Size = new System.Drawing.Size(90, 32);
            this.btnToErrOn.TabIndex = 4;
            this.btnToErrOn.Text = "ErrorOn";
            this.btnToErrOn.UseVisualStyleBackColor = true;
            this.btnToErrOn.Click += new System.EventHandler(this.btnToErrOn_Click);
            // 
            // btnToUnloadReq
            // 
            this.btnToUnloadReq.Location = new System.Drawing.Point(171, 73);
            this.btnToUnloadReq.Name = "btnToUnloadReq";
            this.btnToUnloadReq.Size = new System.Drawing.Size(90, 32);
            this.btnToUnloadReq.TabIndex = 5;
            this.btnToUnloadReq.Text = "UnloadReq";
            this.btnToUnloadReq.UseVisualStyleBackColor = true;
            this.btnToUnloadReq.Click += new System.EventHandler(this.btnToUnloadReq_Click);
            // 
            // btnToNoReq
            // 
            this.btnToNoReq.Location = new System.Drawing.Point(171, 122);
            this.btnToNoReq.Name = "btnToNoReq";
            this.btnToNoReq.Size = new System.Drawing.Size(90, 32);
            this.btnToNoReq.TabIndex = 6;
            this.btnToNoReq.Text = "NoReq";
            this.btnToNoReq.UseVisualStyleBackColor = true;
            this.btnToNoReq.Click += new System.EventHandler(this.btnToNoReq_Click);
            // 
            // btnToOutServ
            // 
            this.btnToOutServ.Location = new System.Drawing.Point(267, 73);
            this.btnToOutServ.Name = "btnToOutServ";
            this.btnToOutServ.Size = new System.Drawing.Size(90, 32);
            this.btnToOutServ.TabIndex = 7;
            this.btnToOutServ.Text = "OutOfService";
            this.btnToOutServ.UseVisualStyleBackColor = true;
            this.btnToOutServ.Click += new System.EventHandler(this.btnToOutServ_Click);
            // 
            // btnToErrOff
            // 
            this.btnToErrOff.Location = new System.Drawing.Point(363, 73);
            this.btnToErrOff.Name = "btnToErrOff";
            this.btnToErrOff.Size = new System.Drawing.Size(90, 32);
            this.btnToErrOff.TabIndex = 8;
            this.btnToErrOff.Text = "ErrorOff";
            this.btnToErrOff.UseVisualStyleBackColor = true;
            this.btnToErrOff.Click += new System.EventHandler(this.btnToErrOff_Click);
            // 
            // ModifyPortStationData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(971, 387);
            this.Controls.Add(this.btnToErrOff);
            this.Controls.Add(this.btnToOutServ);
            this.Controls.Add(this.btnToNoReq);
            this.Controls.Add(this.btnToUnloadReq);
            this.Controls.Add(this.btnToErrOn);
            this.Controls.Add(this.btnToInServ);
            this.Controls.Add(this.btnToLoadReq);
            this.Controls.Add(this.btnGetData);
            this.Controls.Add(this.dgvAPORTSTATIONData);
            this.Name = "ModifyPortStationData";
            this.Text = "EQ Port Data";
            this.Load += new System.EventHandler(this.ModifyPortStationData_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAPORTSTATIONData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAPORTSTATIONData;
        private System.Windows.Forms.Button btnGetData;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.Button btnToLoadReq;
        private System.Windows.Forms.Button btnToInServ;
        private System.Windows.Forms.Button btnToErrOn;
        private System.Windows.Forms.Button btnToUnloadReq;
        private System.Windows.Forms.Button btnToNoReq;
        private System.Windows.Forms.Button btnToOutServ;
        private System.Windows.Forms.Button btnToErrOff;
    }
}
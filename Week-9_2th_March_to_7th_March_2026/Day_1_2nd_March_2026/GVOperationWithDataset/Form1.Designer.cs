namespace GVOperationWithDataset
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            txtEmpId = new TextBox();
            txtEmpName = new TextBox();
            txtEmpDesig = new TextBox();
            txtEmpDOJ = new TextBox();
            txtEmpSal = new TextBox();
            txtDeptNo = new TextBox();
            dataGridView1 = new DataGridView();
            btnInsert = new Button();
            btnFind = new Button();
            btnClear = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            btnClose = new Button();
            btnUpdateDB = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();

            label1.AutoSize = true;
            label1.Location = new Point(70, 51);
            label1.Text = "Enter EmpId";

            label2.AutoSize = true;
            label2.Location = new Point(70, 101);
            label2.Text = "Enter EmpName";

            label3.AutoSize = true;
            label3.Location = new Point(70, 157);
            label3.Text = "Enter Designation";

            label4.AutoSize = true;
            label4.Location = new Point(70, 202);
            label4.Text = "Emp DOJ";

            label5.AutoSize = true;
            label5.Location = new Point(70, 243);
            label5.Text = "Enter Salary";

            label6.AutoSize = true;
            label6.Location = new Point(70, 296);
            label6.Text = "Emp DeptNo";

            txtEmpId.Location = new Point(276, 51);
            txtEmpId.Name = "txtEmpId";

            txtEmpName.Location = new Point(276, 101);
            txtEmpName.Name = "txtEmpName";

            txtEmpDesig.Location = new Point(276, 157);
            txtEmpDesig.Name = "txtEmpDesig";

            txtEmpDOJ.Location = new Point(276, 202);
            txtEmpDOJ.Name = "txtEmpDOJ";

            txtEmpSal.Location = new Point(276, 243);
            txtEmpSal.Name = "txtEmpSal";

            txtDeptNo.Location = new Point(276, 288);
            txtDeptNo.Name = "txtDeptNo";

            dataGridView1.Location = new Point(642, 51);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(622, 260);

            btnInsert.Location = new Point(88, 400);
            btnInsert.Name = "btnInsert";
            btnInsert.Text = "Insert";
            btnInsert.Click += btnInsert_Click;

            btnFind.Location = new Point(208, 400);
            btnFind.Name = "btnFind";
            btnFind.Text = "Find";
            btnFind.Click += btnFind_Click;

            btnClear.Location = new Point(353, 400);
            btnClear.Name = "btnClear";
            btnClear.Text = "Clear";
            btnClear.Click += btnClear_Click;

            btnUpdate.Location = new Point(88, 498);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Text = "Update";
            btnUpdate.Click += btnUpdate_Click;

            btnDelete.Location = new Point(208, 498);
            btnDelete.Name = "btnDelete";
            btnDelete.Text = "Delete";
            btnDelete.Click += btnDelete_Click;

            btnClose.Location = new Point(353, 498);
            btnClose.Name = "btnClose";
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;

            btnUpdateDB.Location = new Point(664, 484);
            btnUpdateDB.Name = "btnUpdateDB";
            btnUpdateDB.Size = new Size(246, 37);
            btnUpdateDB.Text = "Update into Database";
            btnUpdateDB.Click += btnUpdateDB_Click;

            ClientSize = new Size(1402, 706);
            Controls.Add(btnUpdateDB);
            Controls.Add(btnClose);
            Controls.Add(btnDelete);
            Controls.Add(btnUpdate);
            Controls.Add(btnClear);
            Controls.Add(btnFind);
            Controls.Add(btnInsert);
            Controls.Add(dataGridView1);
            Controls.Add(txtDeptNo);
            Controls.Add(txtEmpSal);
            Controls.Add(txtEmpDOJ);
            Controls.Add(txtEmpDesig);
            Controls.Add(txtEmpName);
            Controls.Add(txtEmpId);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;

            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox txtEmpId;
        private TextBox txtEmpName;
        private TextBox txtEmpDesig;
        private TextBox txtEmpDOJ;
        private TextBox txtEmpSal;
        private TextBox txtDeptNo;
        private DataGridView dataGridView1;
        private Button btnInsert;
        private Button btnFind;
        private Button btnClear;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClose;
        private Button btnUpdateDB;
    }
}
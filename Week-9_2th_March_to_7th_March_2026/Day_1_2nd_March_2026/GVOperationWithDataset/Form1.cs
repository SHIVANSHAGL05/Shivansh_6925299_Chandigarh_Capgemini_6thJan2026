using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace GVOperationWithDataset
{
    public partial class Form1 : Form
    {
        SqlConnection con;
        SqlDataAdapter da;
        DataSet ds;
        SqlCommandBuilder bldr;
        DataRow rec;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string conStr =
                @"Server=Blackpearl\SQLEXPRESS;
                  Database=EmployeeWindowdb;
                  Integrated Security=True;
                  TrustServerCertificate=True;";

            con = new SqlConnection(conStr);

            da = new SqlDataAdapter("SELECT * FROM Employeetb", con);
            ds = new DataSet();

            da.Fill(ds, "Employeetb");

            // set primary key
            ds.Tables["Employeetb"].PrimaryKey =
                new DataColumn[] { ds.Tables["Employeetb"].Columns["EmpId"] };

            // bind grid
            dataGridView1.DataSource = ds.Tables["Employeetb"];

            // auto-generate insert/update/delete commands
            bldr = new SqlCommandBuilder(da);
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            // safety check
            if (ds == null || ds.Tables.Count == 0)
            {
                MessageBox.Show("Dataset not loaded. Check Form Load.");
                return;
            }

            rec = ds.Tables["Employeetb"].NewRow();

            rec["EmpId"] = int.Parse(txtEmpId.Text);
            rec["EmpName"] = txtEmpName.Text;
            rec["EmpDesg"] = txtEmpDesig.Text;
            rec["EmpDOJ"] = DateTime.Parse(txtEmpDOJ.Text);
            rec["EmpSal"] = int.Parse(txtEmpSal.Text);
            rec["EmpDept"] = int.Parse(txtDeptNo.Text);

            ds.Tables["Employeetb"].Rows.Add(rec);

            MessageBox.Show("Record inserted into Dataset");
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (ds == null || ds.Tables.Count == 0)
            {
                MessageBox.Show("Dataset not loaded");
                return;
            }

            if (!int.TryParse(txtEmpId.Text, out int empId))
            {
                MessageBox.Show("Enter valid EmpId");
                return;
            }

            rec = ds.Tables["Employeetb"].Rows.Find(empId);

            if (rec != null)
            {
                txtEmpName.Text = rec["EmpName"].ToString();
                txtEmpDesig.Text = rec["EmpDesg"].ToString();
                txtEmpDOJ.Text = rec["EmpDOJ"].ToString();
                txtEmpSal.Text = rec["EmpSal"].ToString();
                txtDeptNo.Text = rec["EmpDept"].ToString();
            }
            else
            {
                MessageBox.Show("Record not found");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (rec == null) return;

            rec["EmpName"] = txtEmpName.Text;
            rec["EmpDesg"] = txtEmpDesig.Text;
            rec["EmpDOJ"] = DateTime.Parse(txtEmpDOJ.Text);
            rec["EmpSal"] = int.Parse(txtEmpSal.Text);
            rec["EmpDept"] = int.Parse(txtDeptNo.Text);

            MessageBox.Show("Record updated in Dataset");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            rec = ds.Tables["Employeetb"].Rows.Find(int.Parse(txtEmpId.Text));

            if (rec != null)
            {
                rec.Delete();
                MessageBox.Show("Record deleted from Dataset");
            }
        }

        private void btnUpdateDB_Click(object sender, EventArgs e)
        {
            da.Update(ds, "Employeetb");
            MessageBox.Show("Changes saved to database");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
                if (c is TextBox)
                    c.Text = "";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
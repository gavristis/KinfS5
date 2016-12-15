using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KInfS5.DAL;
using Oracle.ManagedDataAccess.Client;

namespace KInfS5
{
    public partial class Form1 : Form
    {
        private readonly DatabaseHelper _databaseHelper;

        public Form1()
        {
            InitializeComponent();
            _databaseHelper = new DatabaseHelper();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var tableName = textBox1.Text;
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }

            var query = "select * from " + tableName;

            try
            {
                ExecuteQuery(query, dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Error Has Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var procName = textBox2.Text;
            if (string.IsNullOrEmpty(procName))
            {
                return;
            }

            try
            {
                var parameters = new Dictionary<string, string>();
                foreach (DataGridViewRow row in dataGridView3.Rows)
                {
                    if (row.Cells[0].Value == null || row.Cells[1].Value == null)
                    {
                        continue;
                    }
                    parameters.Add(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
                }
                ExecuteProcedure(procName, parameters);
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 20001:
                        ShowErrorMessage("Specified flower could not be found");
                        break;
                    case 20002:
                        ShowErrorMessage("Values cannot be reduced below 0");
                        break;
                    case 20003:
                        ShowErrorMessage("Dangerous or impossible conditions");
                        break;
                    default:
                        ShowErrorMessage(string.Format("\n{1}", ex.Procedure, ex.Message));
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Error Has Occured", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var funcName = textBox4.Text;
            var parameters = textBox5.Text;

            if (string.IsNullOrEmpty(funcName))
            {
                return;
            }

            var function = "select " + funcName + "(" + parameters + ") from dual";

            try
            {
                ExecuteQuery(function, dataGridView2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Error Has Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "An Error Has Occured ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void FillResult(DataGridView dataGridView, DbDataReader reader)
        {
            var table = new DataTable();
            table.Load(reader);
            var source = new BindingSource { DataSource = table };
            dataGridView.DataSource = source;
            dataGridView.Refresh();
        }

        private void ExecuteQuery(string query, DataGridView dataGridView)
        {
            using (var connect = _databaseHelper.Connection)
            {
                connect.Open();
                using (var command = connect.CreateCommand())
                {
                    command.CommandText = query;
                    var reader = command.ExecuteReader();
                    FillResult(dataGridView, reader);
                }
            }
        }

        private void ExecuteProcedure(string procName, Dictionary<string, string> parameters)
        {
            using (var conn = _databaseHelper.Connection)
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procName;
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(_databaseHelper.CreateParameter(parameter.Key, parameter.Value));
                    }

                    command.ExecuteNonQuery();
                    MessageBox.Show("Prodecure Has Been Successfully Completed", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}

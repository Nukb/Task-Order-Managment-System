﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApp1.Forms
{
    public partial class Power_User : Form
    {
        DBConnection dbcon = new DBConnection();
        SqlCommand cdcom;
        SqlDataReader rdr;
        string sql_str;
        string st_t;
        string power_string;
        string power_temp;
        int user_type;

        public Power_User()
        {
            InitializeComponent();
        }

        private void Power_User2_Load(object sender, EventArgs e)
        {
            fill_grid();
            searchOptions.SelectedIndex = 0;
            comboUser_type.SelectedIndex = 0;
        }



        private void fill_grid(string sql_TS, byte fl_cl)
        {
            sql_str = sql_TS;

            cdcom = new SqlCommand(sql_str, dbcon.conn_db());
            rdr = cdcom.ExecuteReader();

            if (fl_cl == 0)
            {
                dataGridUser.Rows.Clear();
            }
            while (rdr.Read() == true)
            {
                dataGridUser.Rows.Add();
                dataGridUser.Rows[dataGridUser.Rows.Count - 1].Cells[0].Value = rdr["Nu_Emp"];
                dataGridUser.Rows[dataGridUser.Rows.Count - 1].Cells[1].Value = rdr["Na_emp"];
                dataGridUser.Rows[dataGridUser.Rows.Count - 1].Cells[2].Value = rdr["Username"];
            }

            rdr.Close();
            cdcom.Dispose();

        }

        private void fill_grid()
        {

            sql_str = "SELECT [Nu_power] ,[Na_power],[Txt_power] FROM [dbo].[Ta_Power2] ORDER BY Nu_power";

            cdcom = new SqlCommand(sql_str, dbcon.conn_db());
            rdr = cdcom.ExecuteReader();

            dataGridPower.Rows.Clear();

            while (rdr.Read() == true)
            {
                dataGridPower.Rows.Add();
                dataGridPower.Rows[dataGridPower.Rows.Count - 1].Cells[0].Value = 0;
                dataGridPower.Rows[dataGridPower.Rows.Count - 1].Cells[1].Value = dataGridPower.Rows.Count;
                dataGridPower.Rows[dataGridPower.Rows.Count - 1].Cells[2].Value = rdr["Txt_power"];
                dataGridPower.Rows[dataGridPower.Rows.Count - 1].Cells[3].Value = rdr["Na_power"];
            }

            rdr.Close();
            cdcom.Dispose();

        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (nu_Em.Text == string.Empty)
            {
                MessageBox.Show("يجب إدخال رقم الموظف", "خطأ إدخال");
                nu_Em.Focus();
                return;
            }

            sql_str = "SELECT [Nu_Emp] ,[Na_emp] ,[Username] FROM [dbo].[View_emp_user] WHERE (Nu_Emp=" + nu_Em.Text + ")";

            // Execute the query
            cdcom = new SqlCommand(sql_str, dbcon.conn_db());
            rdr = cdcom.ExecuteReader();
            if (rdr.HasRows) // Check if there are any rows in the result set
            {
                fill_grid(sql_str, 0); // Call fill_grid if a user is found
            }
            else
            {
                MessageBox.Show("رقم الموظف غير موجود", "تنبيه");
                newButton_Click(sender,e);
            }
        }

        private void dataGridUser_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                nu_Em.Text = Convert.ToString(dataGridUser.Rows[e.RowIndex].Cells[0].Value);
                na_Em.Text = Convert.ToString(dataGridUser.Rows[e.RowIndex].Cells[1].Value);
                num_textBox.Text = Convert.ToString(dataGridUser.Rows[e.RowIndex].Cells[0].Value);
                usename_textBox.Text = Convert.ToString(dataGridUser.Rows[e.RowIndex].Cells[2].Value);

                //retrive the User_power and change the check boxes
                sql_str = "SELECT [User_power] FROM [dbo].[Ta_Users] WHERE (Nu_Emp=" + Convert.ToString(dataGridUser.Rows[e.RowIndex].Cells[0].Value) + ")";

                cdcom = new SqlCommand(sql_str, dbcon.conn_db());
                rdr = cdcom.ExecuteReader();

                if (rdr.Read() == true)
                {
                    power_temp = Convert.ToString(rdr["User_power"]);
                    rdr.Close();
                    cdcom.Dispose();

                    // To read the power chain from database and check the powers/access in data gridview 
                    for (int i = 0; i <= dataGridPower.Rows.Count - 1; i++)
                    {

                        if (power_temp.Substring(i, 1) == "1")
                        {
                            dataGridPower.Rows[i].Cells[0].Value = 1;
                        }
                        else
                        {
                            dataGridPower.Rows[i].Cells[0].Value = 0;
                        }

                    }
                }
                else
                {
                    rdr.Close();
                    cdcom.Dispose();
                }

                //retrive the User_type and change the combo box value
                sql_str = "SELECT [User_type] FROM [dbo].[Ta_Users] WHERE (Nu_Emp=" + Convert.ToString(dataGridUser.Rows[e.RowIndex].Cells[0].Value) + ")";
                cdcom = new SqlCommand(sql_str, dbcon.conn_db());
                rdr = cdcom.ExecuteReader();

                if (rdr.Read())
                {
                    user_type = Convert.ToInt32(rdr["User_type"]);
                }


                // Set the combo box value based on the retrieved user_type
                if (user_type == 0)
                {
                    comboUser_type.SelectedIndex = 0; // Set "مستخدم" as the selected value
                }
                else if (user_type == 1)
                {
                    comboUser_type.SelectedIndex = 1; // Set "مدير نظام" as the selected value
                }
            }
        }

        private void na_Em_TextChanged(object sender, EventArgs e)
        {
            if (na_Em.Text != string.Empty)
            {

                if (searchOptions.SelectedIndex == 0)
                {
                    st_t = " WHERE(Na_emp LIKE N'" + na_Em.Text + "%')";
                }
                else if (searchOptions.SelectedIndex == 1)
                {
                    st_t = " WHERE(Na_emp LIKE N'%" + na_Em.Text + "%')";
                }
                else if (searchOptions.SelectedIndex == 2)
                {
                    st_t = " WHERE(Na_emp LIKE N'%" + na_Em.Text + "')";
                }

                sql_str = "SELECT [Nu_Emp] ,[Na_emp] ,[Username] FROM [dbo].[View_emp_user] " + st_t;

                fill_grid(sql_str, 0);

                //cdcom = new SqlCommand(sql_str, dbcon.conn_db());
                //rdr = cdcom.ExecuteReader();

                //dataGridPower.Rows.Clear();

                //while (rdr.Read() == true)
                //{
                //    dataGridUser.Rows.Add();
                //    dataGridUser.Rows[dataGridUser.Rows.Count - 1].Cells[0].Value = rdr["Nu_Emp"];
                //    dataGridUser.Rows[dataGridUser.Rows.Count - 1].Cells[1].Value = rdr["Na_emp"];
                //    dataGridUser.Rows[dataGridUser.Rows.Count - 1].Cells[2].Value = rdr["Username"];
                //}
                //rdr.Close();
                //cdcom.Dispose();
            }
        }

        private void comboUser_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if the user state is Admin so all checkboxes is chacked to give all the permissions 
            if (comboUser_type.SelectedIndex == 1)
            {
                for (int i = 0; i <= dataGridPower.Rows.Count - 1; i++)
                {
                    dataGridPower.Rows[i].Cells[0].Value = 1;
                }
            }
            else
            {
                for (int i = 1; i <= dataGridUser.Rows.Count - 1; i++)
                {
                    dataGridPower.Rows[i].Cells[0].Value = 0;
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            //To check if the checkbox cell is checked then give the power 1 value else 0 value to build the chain with 1's and 0's thet equal the number of powers/access
            if (num_textBox.Text != string.Empty)
            {
                //set power_string=1 to indicate to the change password power always equal 1 for all users
                power_string = "1";

                for (int i = 0; i <= dataGridPower.Rows.Count - 1; i++)
                {
                    if (Convert.ToByte(dataGridPower.Rows[i].Cells[0].Value) == 1)
                    {
                        power_string += "1";
                    }
                    else
                    {
                        power_string += "0";
                    }
                }


                dbcon.change_val_user("[User_power]", power_string, Convert.ToInt32(num_textBox.Text));

                dbcon.change_val_user("[User_type]", Convert.ToString(comboUser_type.SelectedIndex), Convert.ToInt32(num_textBox.Text));

                MessageBox.Show("تمت عملية الحفظ بنجاح", "حفظ");

                nu_Em.Clear();
                na_Em.Clear();
                dataGridUser.Rows.Clear();
                num_textBox.Clear();
                usename_textBox.Clear();

                // Clear and set all checkboxes in dataGridPower to 0 (unchecked)
                for (int i = 0; i <= dataGridPower.Rows.Count - 1; i++)
                {
                    dataGridPower.Rows[i].Cells[0].Value = 0; // Set checkbox value to 0 (unchecked)
                }
            }
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            nu_Em.Clear();
            na_Em.Clear();
            dataGridUser.Rows.Clear();
            num_textBox.Clear();
            usename_textBox.Clear();

            // Clear and set all checkboxes in dataGridPower to 0 (unchecked)
            for (int i = 0; i <= dataGridPower.Rows.Count - 1; i++) 
            {
               dataGridPower.Rows[i].Cells[0].Value = 0; // Set checkbox value to 0 (unchecked)
            }
        }
    }
}

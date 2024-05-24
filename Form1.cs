using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // MySQL 네임스페이스 추가

namespace Project13_4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string connStr;
        MySqlConnection conn;
        MySqlCommand cmd;

        private void Form1_Load(object sender, EventArgs e)
        {
            connStr = 
            @"
            Server=localhost;
            Database=naverDB;
            Uid=root;
            Pwd=1234;
            "; // MySQL 연결 문자열, 비밀번호 수정 필요
            conn = new MySqlConnection(connStr);
            conn.Open();

            cmd = new MySqlCommand();
            cmd.Connection = conn;

            list_result.View = View.Details;
            list_result.GridLines = true;
            int listWidth = list_result.Width;
            list_result.Columns.Add("아이디", (int)(listWidth * 0.2));
            list_result.Columns.Add("이름", (int)(listWidth * 0.3));
            list_result.Columns.Add("이메일", (int)(listWidth * 0.3));
            list_result.Columns.Add("출생연도", (int)(listWidth * 0.2));
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            conn.Close();
            MessageBox.Show("DB 연결을 종료합니다~");
        }

        private void SelectedView(object sender, EventArgs e)
        {
            if (list_result.SelectedItems.Count > 0)
            {
                int selectedIndex = list_result.SelectedIndices[0];
                tb_id.Text = list_result.Items[selectedIndex].SubItems[0].Text;
                tb_name.Text = list_result.Items[selectedIndex].SubItems[1].Text;
                tb_email.Text = list_result.Items[selectedIndex].SubItems[2].Text;
                tb_birth.Text = list_result.Items[selectedIndex].SubItems[3].Text;
            }
        }


        private void btn_insert_Click(object sender, EventArgs e)
        {
            string data1 = tb_id.Text.Trim();
            string data2 = tb_name.Text.Trim();
            string data3 = tb_email.Text.Trim();
            string data4 = tb_birth.Text.Trim();

            // 삽입할 SQL 쿼리를 매개변수화하여 SQL 인젝션 방지
            string sql = "INSERT INTO member (id, name, email, birthYear) VALUES (@id, @name, @email, @birthYear)";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", data1);
            cmd.Parameters.AddWithValue("@name", data2);
            cmd.Parameters.AddWithValue("@email", data3);
            cmd.Parameters.AddWithValue("@birthYear", data4);
            cmd.CommandText = sql;

            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("데이터를 추가하였습니다.");
                RefreshListView();  // ListView 새로고침 함수 호출
            }
            catch (Exception ex)
            {
                MessageBox.Show("데이터 추가에 실패했습니다: " + ex.Message);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            string data1 = tb_id.Text.Trim();
            string data2 = tb_name.Text.Trim();
            string data3 = tb_email.Text.Trim();
            string data4 = tb_birth.Text.Trim();

            string sql = @"
            DELETE FROM
                member 
            WHERE
                id = @id AND name = @name AND email = @email AND birthYear = @birthYear
            ";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", data1);
            cmd.Parameters.AddWithValue("@name", data2);
            cmd.Parameters.AddWithValue("@email", data3);
            cmd.Parameters.AddWithValue("@birthYear", data4);
            cmd.CommandText = sql;

            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0) {
                    MessageBox.Show("데이터를 삭제했습니다.");
                    RefreshListView();  // ListView 새로고침 함수 호출
                }
                else
                {
                    MessageBox.Show("해당하는 레코드가 없습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("해당하는 레코드가 없습니다: " + ex.Message);
            }
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            string data1 = tb_id.Text.Trim();
            string data2 = tb_name.Text.Trim();
            string data3 = tb_email.Text.Trim();
            string data4 = tb_birth.Text.Trim();

            string sql = @"
            UPDATE
                member 
            SET
                name = @name,
                email = @email,
                birthYear = @birthYear
            WHERE
                id = @id
            ";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", data1);
            cmd.Parameters.AddWithValue("@name", data2);
            cmd.Parameters.AddWithValue("@email", data3);
            cmd.Parameters.AddWithValue("@birthYear", int.Parse(data4));
            cmd.CommandText = sql;

            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0) {
                    MessageBox.Show("아이디(" + data1 + ")가 잘 수정되었습니다.");
                    RefreshListView();  // ListView 새로고침 함수 호출
                }
                else
                {
                    MessageBox.Show("해당하는 레코드가 없습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("해당하는 레코드가 없습니다: " + ex.Message);
            }
        }
        // ListView를 새로고침하는 메소드
        private void RefreshListView()
        {
            cmd.CommandText = "SELECT * FROM member";
            MySqlDataReader reader = cmd.ExecuteReader();

            list_result.Items.Clear();
            while (reader.Read())
            {
                var item = new ListViewItem(reader.GetString("id"));
                item.SubItems.Add(reader.GetString("name"));
                item.SubItems.Add(reader.GetString("email"));
                item.SubItems.Add(reader.GetInt32("birthYear").ToString());

                list_result.Items.Add(item);
            }
            reader.Close();
        }


        private void btn_select_Click(object sender, EventArgs e)
        {
            string data1, data2, data3, data4;

            cmd.CommandText = "SELECT * FROM member";
            MySqlDataReader reader = cmd.ExecuteReader();

            list_result.Items.Clear();
            ListViewItem item;
            while (reader.Read())
            {
                data1 = reader.GetString("id");
                data2 = reader.GetString("name");
                data3 = reader.GetString("email");
                data4 = reader.GetInt32("birthYear").ToString();

                item = new ListViewItem(data1);
                item.SubItems.Add(data2);
                item.SubItems.Add(data3);
                item.SubItems.Add(data4);

                list_result.Items.Add(item);
            }
            reader.Close();
        }
    }
}

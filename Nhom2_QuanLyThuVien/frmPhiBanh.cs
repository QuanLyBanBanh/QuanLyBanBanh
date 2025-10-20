using BLL_QuanLyBanBanh;
using DTO_QuanLyBanBanh;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GUI_QuanLyThuVien
{
    public partial class frmPhiBanh : Form
    {
        private readonly BusPhiBanh bus = new BusPhiBanh();

        public frmPhiBanh()
        {
            InitializeComponent();
        }

        private void frmPhiBanh_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadComboBoxMaBanh();
            txtMaPhiSach.Text = TaoMaPhiTuDong(); // 🔹 Gán mã tự động khi mở form
            txtMaPhiSach.Enabled = false; // Không cho nhập tay
        }

        private void LoadData()
        {
            dgvPhiBanh.DataSource = bus.GetAllPhiBanh();

            if (dgvPhiBanh.Columns.Count > 0)
            {
                dgvPhiBanh.Columns["MaPhi"].HeaderText = "Mã Phiếu";
                dgvPhiBanh.Columns["MaBanh"].HeaderText = "Mã Bánh";
                dgvPhiBanh.Columns["PhiBan"].HeaderText = "Phí Bán (VNĐ)";
                dgvPhiBanh.Columns["NgayTao"].HeaderText = "Ngày Tạo";
            }

            dgvPhiBanh.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPhiBanh.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvPhiBanh.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvPhiBanh.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // 🔹 Hàm tạo mã tự động PB001, PB002, ...
        private string TaoMaPhiTuDong()
        {
            var list = bus.GetAllPhiBanh();
            if (list == null || list.Count == 0)
                return "PB001";

            int max = list.Select(p =>
            {
                if (p.MaPhi != null && p.MaPhi.StartsWith("PB"))
                    return int.TryParse(p.MaPhi.Substring(2), out int so) ? so : 0;
                return 0;
            }).DefaultIfEmpty(0).Max();

            return "PB" + (max + 1).ToString("D3");
        }

        private void ClearForm()
        {
            txtMaPhiSach.Text = TaoMaPhiTuDong(); // 🔹 Cập nhật mã mới
            txtPhiBan.Clear();
            txtTimKiem.Clear();
            cbxMaBanh.SelectedIndex = -1;
            dtpNgayTao.Value = DateTime.Now;
        }

        private void LoadComboBoxMaBanh()
        {
            try
            {
                var listBanh = bus.GetAllPhiBanh(); // ⚠️ Gọi từ BLL chuyên biệt (nếu có)
                cbxMaBanh.DataSource = listBanh;
                cbxMaBanh.DisplayMember = "TenBanh";
                cbxMaBanh.ValueMember = "MaBanh";
                cbxMaBanh.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load danh sách bánh: " + ex.Message);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (cbxMaBanh.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn bánh!");
                return;
            }

            PhiBanh pb = new PhiBanh
            {
                MaPhi = txtMaPhiSach.Text.Trim(),
                MaBanh = cbxMaBanh.SelectedValue.ToString(),
                PhiBan = decimal.TryParse(txtPhiBan.Text, out decimal phiBan) ? phiBan : 0,
                NgayTao = dtpNgayTao.Value
            };

            string result = bus.AddPhiBanh(pb);
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("Thêm thành công!");
                LoadData();
                ClearForm();
            }
            else
                MessageBox.Show(result, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void dgvPhiBanh_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPhiBanh.Rows[e.RowIndex];
                txtMaPhiSach.Text = row.Cells["MaPhi"].Value?.ToString();
                cbxMaBanh.Text = row.Cells["MaBanh"].Value?.ToString();
                txtPhiBan.Text = row.Cells["PhiBan"].Value?.ToString();

                if (row.Cells["NgayTao"].Value != DBNull.Value)
                    dtpNgayTao.Value = Convert.ToDateTime(row.Cells["NgayTao"].Value);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            PhiBanh pb = new PhiBanh
            {
                MaPhi = txtMaPhiSach.Text.Trim(),
                MaBanh = cbxMaBanh.Text.Trim(),
                PhiBan = decimal.TryParse(txtPhiBan.Text, out decimal phiBan) ? phiBan : 0,
                NgayTao = dtpNgayTao.Value
            };

            string result = bus.UpdatePhiBanh(pb);
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("Cập nhật thành công!");
                LoadData();
                ClearForm();
            }
            else
                MessageBox.Show(result, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvPhiBanh.CurrentRow == null) return;

            string maPhi = dgvPhiBanh.CurrentRow.Cells["MaPhi"].Value.ToString();
            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string result = bus.DeletePhiBanh(maPhi);
                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("Xóa thành công!");
                    LoadData();
                    ClearForm();
                }
                else
                    MessageBox.Show(result, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadData();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            dgvPhiBanh.DataSource = bus.SearchPhiBanh(keyword);
        }
    }
}

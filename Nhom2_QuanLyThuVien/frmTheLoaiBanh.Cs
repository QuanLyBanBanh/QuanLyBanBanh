using BLL_QuanLyBanBanh;
using DTO_QuanLyBanBanh;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI_QuanLyThuVien
{
    public partial class frmTheLoaiBanh : Form
    {
        // khai báo BLL
        private BusTheLoaiBanh busLoaiBanh = new BusTheLoaiBanh();

        public frmTheLoaiBanh()
        {
            InitializeComponent();
            this.Load += TheLoaiBanh_Load;

            // Gán event nếu bạn chưa gán trong Designer
            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            btnTimKiem.Click += btnTimKiem_Click;
            dgvTheLoaiBanh.CellClick += dgvTheLoaiBanh_CellDoubleClick;

        }

        private void TheLoaiBanh_Load(object sender, EventArgs e)
        {
            LoadData();
            txtMaLoai.Text = busLoaiBanh.TaoMaLoaiBanhTuDong();
            txtMaLoai.Enabled = false;
            dtpNgayTao.Value = DateTime.Now;
            btnSua.Enabled = false;
        }
        private void LoadData()
        {
            var list = busLoaiBanh.GetAllTheLoaiBanh();
            dgvTheLoaiBanh.DataSource = list
                .Select(l => new
                {
                    l.MaLoai,
                    l.TenLoai,
                    NgayTao = l.NgayTao?.ToString("dd/MM/yyyy")
                }).ToList();

            if (dgvTheLoaiBanh.Columns["MaLoai"] != null)
                dgvTheLoaiBanh.Columns["MaLoai"].HeaderText = "Mã loại bánh";
            if (dgvTheLoaiBanh.Columns["TenLoai"] != null)
                dgvTheLoaiBanh.Columns["TenLoai"].HeaderText = "Tên loại bánh";
            if (dgvTheLoaiBanh.Columns["NgayTao"] != null)
                dgvTheLoaiBanh.Columns["NgayTao"].HeaderText = "Ngày tạo";

            dgvTheLoaiBanh.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTheLoaiBanh.ClearSelection();
        }
        private void ClearForm()
        {
            txtMaLoai.Text = busLoaiBanh.TaoMaLoaiBanhTuDong();
            txtTenLoai.Clear();
            txtTimKiem.Clear();
            dtpNgayTao.Value = DateTime.Now;
            btnThem.Enabled = true;
            btnSua.Enabled = false;
            dgvTheLoaiBanh.ClearSelection();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            var loai = new TheLoaiBanh
            {
                MaLoai = txtMaLoai.Text.Trim(),
                TenLoai = txtTenLoai.Text.Trim(),
                NgayTao = DateTime.Now
            };

            string res = busLoaiBanh.AddTheLoaiBanh(loai);
            if (string.IsNullOrEmpty(res))
            {
                MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearForm();
            }
            else
            {
                MessageBox.Show(res, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvTheLoaiBanh_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvTheLoaiBanh.Rows[e.RowIndex];
            txtMaLoai.Text = row.Cells["MaLoai"].Value?.ToString();
            txtTenLoai.Text = row.Cells["TenLoai"].Value?.ToString();

            string ngay = row.Cells["NgayTao"].Value?.ToString();
            if (DateTime.TryParse(ngay, out DateTime dt))
                dtpNgayTao.Value = dt;
            else
                dtpNgayTao.Value = DateTime.Now;

            btnThem.Enabled = false;
            btnSua.Enabled = true;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadData();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            var result = busLoaiBanh.SearchTheLoaiBanh(keyword);
            dgvTheLoaiBanh.DataSource = result
                .Select(l => new
                {
                    l.MaLoai,
                    l.TenLoai,
                    NgayTao = l.NgayTao?.ToString("dd/MM/yyyy")
                }).ToList();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaLoai.Text))
            {
                MessageBox.Show("Chọn loại bánh cần xóa.", "Thông báo");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string res = busLoaiBanh.DeleteTheLoaiBanh(txtMaLoai.Text.Trim());
                if (string.IsNullOrEmpty(res))
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(res, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaLoai.Text))
            {
                MessageBox.Show("Chọn loại bánh cần sửa.", "Thông báo");
                return;
            }

            var loai = new TheLoaiBanh
            {
                MaLoai = txtMaLoai.Text.Trim(),
                TenLoai = txtTenLoai.Text.Trim(),
                NgayTao = dtpNgayTao.Value
            };

            string res = busLoaiBanh.UpdateTheLoaiBanh(loai);
            if (string.IsNullOrEmpty(res))
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearForm();
            }
            else
            {
                MessageBox.Show(res, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

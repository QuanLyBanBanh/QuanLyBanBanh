using BLL_QuanLyBanBanh;
using DTO_QuanLyBanBanh;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;


namespace GUI_QuanLyThuVien
{
    public partial class frmNhaCungCap : Form
    {
        private readonly BusNhaCungCap bus = new BusNhaCungCap();

        public frmNhaCungCap()
        {
            InitializeComponent();
            this.Load += frmNhaCungCap_Load;
        }

        private void frmNhaCungCap_Load(object sender, EventArgs e)
        {
            LoadDanhSachNhaCungCap();
            txtMaNCC.Text = TaoMaNhaCungCapTuDong();
            btnSua.Enabled = false;
            txtMaNCC.Enabled = false;
        }

        private void LoadDanhSachNhaCungCap()
        {
            HienThiDuLieuTrenDGV(bus.GetAllNhaCungCap());
        }

        private void HienThiDuLieuTrenDGV(List<NhaCungCap> list)
        {
            dgvNhaCungCap.DataSource = list.Select(n => new
            {
                n.MaNCC,
                n.TenNCC,
                n.SDT,
                n.Email,
                n.DiaChi,
                TrangThai = n.TrangThai ?? "Không rõ",
                NgayTao = n.NgayTao?.ToString("dd/MM/yyyy")
            }).ToList();

            // 🔽 Đổi tiêu đề cột sang tiếng Việt
            dgvNhaCungCap.Columns["MaNCC"].HeaderText = "Mã nhà cung cấp";
            dgvNhaCungCap.Columns["TenNCC"].HeaderText = "Tên nhà cung cấp";
            dgvNhaCungCap.Columns["SDT"].HeaderText = "Số điện thoại";
            dgvNhaCungCap.Columns["Email"].HeaderText = "Email";
            dgvNhaCungCap.Columns["DiaChi"].HeaderText = "Địa chỉ";
            dgvNhaCungCap.Columns["TrangThai"].HeaderText = "Trạng thái";
            dgvNhaCungCap.Columns["NgayTao"].HeaderText = "Ngày tạo";

            // (Tuỳ chọn) căn giữa & tự giãn đều cột
            dgvNhaCungCap.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNhaCungCap.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }


        private string TaoMaNhaCungCapTuDong()
        {
            var list = bus.GetAllNhaCungCap();
            int max = list.Select(n => int.TryParse(n.MaNCC?.Replace("NCC", ""), out int so) ? so : 0).DefaultIfEmpty().Max();
            return "NCC" + (max + 1).ToString("D2");
        }

        private void ClearForm()
        {
            txtMaNCC.Text = TaoMaNhaCungCapTuDong();
            txtTenNCC.Clear();
            txtSDT.Clear();
            txtEmail.Clear();
            txtDiaChi.Clear();
            rbtDangHoatDong.Checked = false;
            rbtTamNgung.Checked = false;
            btnThem.Enabled = true;
            btnSua.Enabled = false;
        }

        private string LayTrangThaiTuRadio()
        {
            if (rbtDangHoatDong.Checked) return "Hoạt động";
            if (rbtTamNgung.Checked) return "Tạm ngưng";
            return "Không rõ";
        }

        private void DatTrangThaiLenRadio(string trangThai)
        {
            rbtDangHoatDong.Checked = trangThai == "Hoạt động" || trangThai == "Đang hoạt động";
            rbtTamNgung.Checked = trangThai == "Tạm ngưng";
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenNCC.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp!");
                return;
            }

            var ncc = new NhaCungCap
            {
                MaNCC = txtMaNCC.Text,
                TenNCC = txtTenNCC.Text,
                SDT = txtSDT.Text,
                Email = txtEmail.Text,
                DiaChi = txtDiaChi.Text,
                TrangThai = LayTrangThaiTuRadio(),
                NgayTao = DateTime.Now
            };

            string result = bus.AddNhaCungCap(ncc);
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("✅ Thêm thành công!");
                ClearForm();
                LoadDanhSachNhaCungCap();
            }
            else
                MessageBox.Show(result, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            var ncc = new NhaCungCap
            {
                MaNCC = txtMaNCC.Text,
                TenNCC = txtTenNCC.Text,
                SDT = txtSDT.Text,
                Email = txtEmail.Text,
                DiaChi = txtDiaChi.Text,
                TrangThai = LayTrangThaiTuRadio(),
                NgayTao = DateTime.Now
            };

            string result = bus.UpdateNhaCungCap(ncc);
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("Cập nhật thành công!");
                ClearForm();
                LoadDanhSachNhaCungCap();
            }
            else
                MessageBox.Show(result, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string result = bus.DeleteNhaCungCap(txtMaNCC.Text);
                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("Đã xóa!");
                    ClearForm();
                    LoadDanhSachNhaCungCap();
                }
                else MessageBox.Show(result, "Lỗi");
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            var result = bus.SearchNhaCungCap(keyword);
            HienThiDuLieuTrenDGV(result);
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadDanhSachNhaCungCap();
        }

        private void dgvNhaCungCap_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvNhaCungCap.Rows[e.RowIndex];
                txtMaNCC.Text = row.Cells["MaNCC"].Value?.ToString();
                txtTenNCC.Text = row.Cells["TenNCC"].Value?.ToString();
                txtSDT.Text = row.Cells["SDT"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString();

                string tt = row.Cells["TrangThai"].Value?.ToString();
                DatTrangThaiLenRadio(tt);

                btnThem.Enabled = false;
                btnSua.Enabled = true;
            }
        }
    }
}

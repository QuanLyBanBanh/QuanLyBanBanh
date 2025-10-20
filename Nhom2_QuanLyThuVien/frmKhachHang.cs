using BLL_QuanLyBanBanh;
using DTO_QuanLyBanBanh;
using System;
using System.Linq;
using System.Windows.Forms;

namespace GUI_QuanLyBanBanh
{
    public partial class frmKhachHang : Form
    {
        private readonly BusKhachHang bus = new BusKhachHang();

        public frmKhachHang()
        {
            InitializeComponent();
            this.Load += frmKhachHang_Load;
        }
        //Lê Hoàng Namabc

        private void frmKhachHang_Load(object sender, EventArgs e)
        {
            LoadData();
            txtMaKH.Text = AutoGenerateMaKH();
            txtMaKH.Enabled = false;
            btnSua.Enabled = false;
        }

        // ======================== LOAD DỮ LIỆU ========================
        private void LoadData()
        {
            var list = bus.GetAll();
            dgvKhachHang.Rows.Clear();
            dgvKhachHang.Columns.Clear();

            dgvKhachHang.Columns.Add("MaKH", "Mã KH");
            dgvKhachHang.Columns.Add("TenKH", "Tên KH");
            dgvKhachHang.Columns.Add("SDT", "SĐT");
            dgvKhachHang.Columns.Add("Email", "Email");
            dgvKhachHang.Columns.Add("DiaChi", "Địa chỉ");
            dgvKhachHang.Columns.Add("NgayTao", "Ngày tạo");

            foreach (var kh in list)
            {
                dgvKhachHang.Rows.Add(
                    kh.MaKH,
                    kh.TenKH,
                    kh.SDT,
                    kh.Email,
                    kh.DiaChi,
                    kh.NgayTao?.ToString("dd/MM/yyyy")
                );
            }

            dgvKhachHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // ======================== TỰ ĐỘNG SINH MÃ KH ========================
        private string AutoGenerateMaKH()
        {
            var list = bus.GetAll();
            int max = list.Select(x => int.TryParse(x.MaKH?.Replace("KH", ""), out int so) ? so : 0)
                          .DefaultIfEmpty().Max();
            return "KH" + (max + 1).ToString("D3");
        }

        // ======================== NÚT THÊM ========================
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!");
                return;
            }

            var kh = new KhachHang
            {
                MaKH = txtMaKH.Text,
                TenKH = txtTenKH.Text,
                SDT = txtSDT.Text,
                Email = txtEmail.Text,
                DiaChi = txtDiaChi.Text,
                NgayTao = DateTime.Now
            };

            string result = bus.Add(kh);
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("✅ Thêm khách hàng thành công!");
                ClearForm();
                LoadData();
            }
            else MessageBox.Show(result);
        }

        // ======================== NÚT SỬA ========================
        private void btnSua_Click(object sender, EventArgs e)
        {
            var kh = new KhachHang
            {
                MaKH = txtMaKH.Text,
                TenKH = txtTenKH.Text,
                SDT = txtSDT.Text,
                Email = txtEmail.Text,
                DiaChi = txtDiaChi.Text,
                NgayTao = DateTime.Now
            };

            string result = bus.Update(kh);
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("📝 Cập nhật khách hàng thành công!");
                ClearForm();
                LoadData();
            }
            else MessageBox.Show(result);
        }

        // ======================== NÚT XÓA ========================
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa khách hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string result = bus.Delete(txtMaKH.Text);
                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("🗑️ Đã xóa khách hàng!");
                    ClearForm();
                    LoadData();
                }
                else MessageBox.Show(result);
            }
        }

        // ======================== NÚT TÌM KIẾM ========================
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            var result = bus.Search(keyword);

            dgvKhachHang.Rows.Clear();
            foreach (var kh in result)
            {
                dgvKhachHang.Rows.Add(
                    kh.MaKH,
                    kh.TenKH,
                    kh.SDT,
                    kh.Email,
                    kh.DiaChi,
                    kh.NgayTao?.ToString("dd/MM/yyyy")
                );
            }

            if (result.Count == 0)
                MessageBox.Show("❌ Không tìm thấy khách hàng phù hợp!");
        }

        // ======================== NÚT LÀM MỚI ========================
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadData();
        }

        // ======================== DOUBLE CLICK TRÊN DGV ========================
        private void dgvKhachHang_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvKhachHang.Rows[e.RowIndex];
                txtMaKH.Text = row.Cells["MaKH"].Value?.ToString();
                txtTenKH.Text = row.Cells["TenKH"].Value?.ToString();
                txtSDT.Text = row.Cells["SDT"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString();

                btnThem.Enabled = false;
                btnSua.Enabled = true;
            }
        }

        // ======================== CLEAR FORM ========================
        private void ClearForm()
        {
            txtMaKH.Text = AutoGenerateMaKH();
            txtTenKH.Clear();
            txtSDT.Clear();
            txtEmail.Clear();
            txtDiaChi.Clear();
            txtTimKiem.Clear();
            btnThem.Enabled = true;
            btnSua.Enabled = false;
        }
    }
}

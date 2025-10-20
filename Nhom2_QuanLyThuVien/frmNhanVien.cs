using BLL_QuanLyBanBanh;
using DTO_QuanLyBanBanh;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GUI_QuanLyBanBanh
{
    public partial class frmNhanVien : Form
    {
        private readonly BusNhanVien bus = new BusNhanVien();
        private string imageFolder = Application.StartupPath + "\\Images\\";

        public frmNhanVien()
        {
            InitializeComponent();
            this.Load += frmNhanVien_Load; 
        }

        private void frmNhanVien_Load(object sender, EventArgs e)
        {
            LoadData();
            txtMaNV.Text = AutoGenerateMaNV();
            txtMaNV.Enabled = false;
            btnSua.Enabled = false;
            if (!Directory.Exists(imageFolder)) Directory.CreateDirectory(imageFolder);
        }

        // ======================== LOAD DỮ LIỆU ========================
        private void LoadData()
        {
            var list = bus.GetAll();
            dgvNhanVien.Rows.Clear();
            dgvNhanVien.Columns.Clear();

            // Cột văn bản
            dgvNhanVien.Columns.Add("MaNV", "Mã NV");
            dgvNhanVien.Columns.Add("TenNV", "Tên NV");
            dgvNhanVien.Columns.Add("GioiTinh", "Giới tính");
            dgvNhanVien.Columns.Add("NgaySinh", "Ngày sinh");
            dgvNhanVien.Columns.Add("SDT", "SĐT");
            dgvNhanVien.Columns.Add("ChucVu", "Chức vụ");
            dgvNhanVien.Columns.Add("TrangThai", "Trạng thái");
            dgvNhanVien.Columns.Add("MatKhau", "Mật khẩu");
            dgvNhanVien.Columns.Add("NgayTao", "Ngày tạo");
            dgvNhanVien.Columns.Add("Email", "Email");


            // Cột hình ảnh
            DataGridViewImageColumn imgCol = new DataGridViewImageColumn
            {
                Name = "HinhAnh",
                HeaderText = "Hình ảnh",
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            dgvNhanVien.Columns.Add(imgCol);

            dgvNhanVien.Columns.Add("TenFileAnh", "Tên file ảnh");
            dgvNhanVien.Columns["TenFileAnh"].Visible = false;

            // Duyệt danh sách nhân viên
            foreach (var nv in list)
            {
                Image img = null;
                string path = Path.Combine(imageFolder, nv.HinhAnh ?? "");
                if (File.Exists(path))
                    img = Image.FromFile(path);

                dgvNhanVien.Rows.Add(
                    nv.MaNV,
                    nv.TenNV,
                    nv.GioiTinh,
                    nv.NgaySinh?.ToString("dd/MM/yyyy"),
                    nv.SDT,
                    nv.ChucVu,
                    nv.TrangThai,
                    nv.MatKhau,
                    nv.NgayTao?.ToString("dd/MM/yyyy"),
                                        nv.Email,     // thêm dòng này
                    img,
                    nv.HinhAnh    // Cột ẩn tên file ảnh

                );
            }

            dgvNhanVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNhanVien.RowTemplate.Height = 70;
        }

        private string AutoGenerateMaNV()
        {
            var list = bus.GetAll();
            int max = list.Select(x => int.TryParse(x.MaNV?.Replace("NV", ""), out int so) ? so : 0).DefaultIfEmpty().Max();
            return "NV" + (max + 1).ToString("D2");
        }

        // ======================== CHỌN ẢNH ========================
        private void btnChonAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Ảnh (*.jpg;*.png)|*.jpg;*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.GetFileName(ofd.FileName);
                string dest = Path.Combine(imageFolder, fileName);
                if (!File.Exists(dest)) File.Copy(ofd.FileName, dest);
                picAnh.Image = Image.FromFile(dest);
                picAnh.Tag = fileName;
            }
        }

        // ======================== LẤY GIÁ TRỊ RADIO ========================
        private string LayGioiTinh()
        {
            if (rbtNam.Checked) return "Nam";
            if (rbtNu.Checked) return "Nữ";
            return "Không rõ";
        }

        private string LayTrangThai()
        {
            if (rbtDangLam.Checked) return "Đang làm";
            if (rbtNghi.Checked) return "Nghỉ";
            return "Không rõ";
        }

        // ======================== GÁN RADIO TỪ GIÁ TRỊ ========================
        private void SetGioiTinh(string gt)
        {
            rbtNam.Checked = gt == "Nam";
            rbtNu.Checked = gt == "Nữ";
        }

        private void SetTrangThai(string tt)
        {
            rbtDangLam.Checked = tt == "Đang làm";
            rbtNghi.Checked = tt == "Nghỉ";
        }

        // ======================== NÚT THÊM ========================
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenNV.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhân viên!");
                return;
            }

            var nv = new NhanVien
            {
                MaNV = txtMaNV.Text,
                TenNV = txtTenNV.Text,
                GioiTinh = LayGioiTinh(),
                NgaySinh = dtpNgaySinh.Value,
                SDT = txtSDT.Text,
                ChucVu = txtChucVu.Text,
                HinhAnh = picAnh.Tag?.ToString(),
                TrangThai = LayTrangThai(),
                MatKhau = txtMatKhau.Text,
                NgayTao = DateTime.Now,
                Email = txtEmail.Text,

            };

            string email = txtEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập email!");
                return;
            }
            else if (!email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Email phải có định dạng @gmail.com!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string result = bus.Add(nv);
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("✅ Thêm thành công!");
                ClearForm();
                LoadData();
            }
            else MessageBox.Show(result, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ======================== NÚT SỬA ========================
        private void btnSua_Click(object sender, EventArgs e)
        {
            var nv = new NhanVien
            {
                MaNV = txtMaNV.Text,
                TenNV = txtTenNV.Text,
                GioiTinh = LayGioiTinh(),
                NgaySinh = dtpNgaySinh.Value,
                SDT = txtSDT.Text,
                ChucVu = txtChucVu.Text,
                HinhAnh = picAnh.Tag?.ToString(),
                TrangThai = LayTrangThai(),
                MatKhau = txtMatKhau.Text,
                NgayTao = DateTime.Now,
                Email = txtEmail.Text,

            };

            string email = txtEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập email!");
                return;
            }
            else if (!email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Email phải có định dạng @gmail.com!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string result = bus.Update(nv);
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("📝 Cập nhật thành công!");
                ClearForm();
                LoadData();
            }
            else MessageBox.Show(result);
        }

        // ======================== NÚT XÓA ========================
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa nhân viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string result = bus.Delete(txtMaNV.Text);
                if (string.IsNullOrEmpty(result))
                {
                    MessageBox.Show("🗑️ Đã xóa!");
                    ClearForm();
                    LoadData();
                }
                else MessageBox.Show(result);
            }
        }

        // ======================== CLEAR FORM ========================
        private void ClearForm()
        {
            txtMaNV.Text = AutoGenerateMaNV();
            txtTenNV.Clear();
            txtChucVu.Clear();
            txtSDT.Clear();
            txtMatKhau.Clear();
            picAnh.Image = null;
            picAnh.Tag = null;
            rbtNam.Checked = false;
            rbtNu.Checked = false;
            rbtDangLam.Checked = false;
            rbtNghi.Checked = false;
            btnThem.Enabled = true;
            btnSua.Enabled = false;
        }

        // ======================== CLICK TRÊN DGV ========================
        private void dgvNhanVien_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvNhanVien.Rows[e.RowIndex];
                txtMaNV.Text = row.Cells["MaNV"].Value?.ToString();
                txtTenNV.Text = row.Cells["TenNV"].Value?.ToString();
                SetGioiTinh(row.Cells["GioiTinh"].Value?.ToString());
                txtChucVu.Text = row.Cells["ChucVu"].Value?.ToString();
                txtSDT.Text = row.Cells["SDT"].Value?.ToString();
                txtMatKhau.Text = row.Cells["MatKhau"].Value?.ToString();
                SetTrangThai(row.Cells["TrangThai"].Value?.ToString());

                // Load ảnh
                string imgName = row.Cells["HinhAnh"].Value?.ToString();
                string path = Path.Combine(imageFolder, imgName ?? "");
                if (File.Exists(path))
                {
                    picAnh.Image = Image.FromFile(path);
                    picAnh.Tag = imgName;
                }

                btnThem.Enabled = false;
                btnSua.Enabled = true;
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();

            // Lấy kết quả từ tầng BUS
            var result = bus.Search(keyword);

            // Xóa dữ liệu cũ trong DataGridView
            dgvNhanVien.Rows.Clear();

            // Hiển thị danh sách kết quả tìm kiếm
            foreach (var nv in result)
            {
                Image img = null;
                string path = Path.Combine(imageFolder, nv.HinhAnh ?? "");
                if (File.Exists(path))
                    img = Image.FromFile(path);

                dgvNhanVien.Rows.Add(
                    nv.MaNV,
                    nv.TenNV,
                    nv.GioiTinh,
                    nv.NgaySinh?.ToString("dd/MM/yyyy"),
                    nv.SDT,
                    nv.ChucVu,
                    nv.TrangThai,
                    nv.MatKhau,
                    nv.NgayTao?.ToString("dd/MM/yyyy"),
                                        nv.Email,
                    img,
                    nv.HinhAnh      // Cột ẩn tên file ảnh

                );
            }

            dgvNhanVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNhanVien.RowTemplate.Height = 70;

            if (result.Count == 0)
            {
                MessageBox.Show("❌ Không tìm thấy nhân viên nào phù hợp!", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvNhanVien_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvNhanVien.Rows[e.RowIndex];

                txtMaNV.Text = row.Cells["MaNV"].Value?.ToString();
                txtTenNV.Text = row.Cells["TenNV"].Value?.ToString();
                SetGioiTinh(row.Cells["GioiTinh"].Value?.ToString());

                // Ngày sinh
                DateTime ngaySinh;
                if (DateTime.TryParse(row.Cells["NgaySinh"].Value?.ToString(), out ngaySinh))
                    dtpNgaySinh.Value = ngaySinh;

                txtSDT.Text = row.Cells["SDT"].Value?.ToString();
                txtChucVu.Text = row.Cells["ChucVu"].Value?.ToString();
                txtMatKhau.Text = row.Cells["MatKhau"].Value?.ToString();

                SetTrangThai(row.Cells["TrangThai"].Value?.ToString());

                txtEmail.Text = row.Cells["Email"].Value?.ToString();


                // Ảnh
                string imgName = row.Cells["TenFileAnh"].Value?.ToString();
                string path = Path.Combine(imageFolder, imgName ?? "");
                if (File.Exists(path))
                {
                    picAnh.Image = Image.FromFile(path);
                    picAnh.Tag = imgName;
                }
                else
                {
                    picAnh.Image = null;
                    picAnh.Tag = null;
                }

                btnThem.Enabled = false;
                btnSua.Enabled = true;
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearForm();

            // Load lại toàn bộ danh sách nhân viên từ DB
            LoadData();

            // Thông báo nhẹ cho người dùng
            MessageBox.Show("🔄 Dữ liệu đã được làm mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

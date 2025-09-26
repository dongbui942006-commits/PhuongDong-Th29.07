Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports System.Xml
Imports System.FormDangNhap
Public Class FormChinh
    Dim conn As SqlConnection
    Dim cmd As SqlCommand
    Dim da As SqlDataAdapter
    Dim dt As DataTable
    Dim tongtien As Double = 0

    Private Sub FormChinh_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Application.Exit()
    End Sub
    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Kết nối SQL
        conn = New SqlConnection("Data Source=LAPTOP-JEVPB7CE\SQLEXPRESS;Initial Catalog=QLBanHang;Integrated Security=True;TrustServerCertificate=True")
        conn.Open()

        ' Load dữ liệu cho các combobox
        LoadComboBox("SELECT MaKH FROM KhachHang", cboMaKH)
        LoadComboBox("SELECT MaNV FROM NhanVien", cboMaNV)
        LoadComboBox("SELECT MaHang FROM HangHoa", cboMaHang)
    End Sub

    Private Sub LoadComboBox(sql As String, cbo As ComboBox)
        da = New SqlDataAdapter(sql, conn)
        dt = New DataTable
        da.Fill(dt)
        cbo.DataSource = dt
        cbo.DisplayMember = dt.Columns(0).ColumnName
        cbo.ValueMember = dt.Columns(0).ColumnName
    End Sub

    ' Khi chọn mã khách hàng
    Private Sub cboMaKH_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboMaKH.SelectedIndexChanged
        Dim sql As String = "SELECT TenKH, DiaChi, DienThoai FROM KhachHang WHERE MaKH='" & cboMaKH.Text & "'"
        cmd = New SqlCommand(sql, conn)
        Dim rd As SqlDataReader = cmd.ExecuteReader()
        If rd.Read() Then
            txtTenKhachHang.Text = rd("TenKH").ToString()
            txtDiaChi.Text = rd("DiaChi").ToString()
            txtDienThoai.Text = rd("DienThoai").ToString()
        End If
        rd.Close()
    End Sub

    ' Khi chọn mã nhân viên
    Private Sub cboMaNV_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboMaNV.SelectedIndexChanged
        Dim sql As String = "SELECT TenNV FROM NhanVien WHERE MaNV='" & cboMaNV.Text & "'"
        cmd = New SqlCommand(sql, conn)
        Dim rd As SqlDataReader = cmd.ExecuteReader()
        If rd.Read() Then
            txtTenNV.Text = rd("TenNV").ToString()
        End If
        rd.Close()
    End Sub

    ' Khi chọn mã hàng
    Private Sub cboMaHang_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboMaHang.SelectedIndexChanged
        Dim sql As String = "SELECT TenHang, DonGia FROM HangHoa WHERE MaHang='" & cboMaHang.Text & "'"
        cmd = New SqlCommand(sql, conn)
        Dim rd As SqlDataReader = cmd.ExecuteReader()
        If rd.Read() Then
            txtTenHang.Text = rd("TenHang").ToString()
            TxtDonGia.Text = rd("DonGia").ToString()
        End If
        rd.Close()
    End Sub

    ' Tính thành tiền
    Private Sub txtSoLuong_TextChanged(sender As Object, e As EventArgs) Handles txtSoLuong.TextChanged, txtGiamGia.TextChanged
        If txtSoLuong.Text <> "" And TxtDonGia.Text <> "" Then
            Dim sl As Integer = Val(txtSoLuong.Text)
            Dim dg As Double = Val(TxtDonGia.Text)
            Dim giam As Double = Val(txtGiamGia.Text) / 100
            Dim tt As Double = sl * dg * (1 - giam)
            txtThanhTien.Text = tt.ToString("N0")
        End If
    End Sub

    ' Thêm vào ListBox
    ' ======================= NÚT THÊM HÓA ĐƠN =========================
    Private Sub btThemHD_Click(sender As Object, e As EventArgs) Handles btThemHD.Click
        cboMaHD.SelectedIndex = 1
        dtNgay.Value = DateTime.Now
        cboMaKH.SelectedIndex = -1
        cboMaNV.SelectedIndex = -1
        cboMaHang.SelectedIndex = -1
        txtTenKhachHang.Clear()
        txtDiaChi.Clear()
        txtDienThoai.Clear()
        txtTenNV.Clear()
        txtTenHang.Clear()
        TxtDonGia.Clear()
        txtSoLuong.Clear()
        txtGiamGia.Clear()
        txtThanhTien.Clear()
        txtTongTien.Clear()
        tongtien = 0
    End Sub

    ' ======================= NÚT LƯU HÓA ĐƠN =========================
    Private Sub btLuuHD_Click(sender As Object, e As EventArgs) Handles btLuuHD.Click
        Try
            ' 1. Lưu vào bảng HoaDon
            Dim sqlHD As String = "INSERT INTO HoaDon(MaHD, NgayLap, MaKH, MaNV, TongTien) 
                               VALUES(@mahd, @ngay, @makh, @manv, @tong)"
            Using cmd As New SqlCommand(sqlHD, conn)
                cmd.Parameters.AddWithValue("@mahd", cboMaHD.Text)
                cmd.Parameters.AddWithValue("@ngay", dtNgay.Value)
                cmd.Parameters.AddWithValue("@makh", cboMaKH.Text)
                cmd.Parameters.AddWithValue("@manv", cboMaNV.Text)
                cmd.Parameters.AddWithValue("@tong", tongtien)
                cmd.ExecuteNonQuery()
            End Using

            ' 2. Lưu các chi tiết hóa đơn
            For Each item As String In lstDanhSach.Items
                ' Ví dụ item = "MH01 - Bút - SL:2 - DG:5000 - GG:0% - TT:10000"
                Dim parts() As String = item.Split("-"c)
                Dim maHang As String = parts(0).Trim()
                Dim soLuong As Integer = CInt(parts(2).Replace("SL:", "").Trim())
                Dim donGia As Double = CDbl(parts(3).Replace("DG:", "").Trim())
                Dim giamGia As Double = CDbl(parts(4).Replace("GG:", "").Replace("%", "").Trim())
                Dim thanhTien As Double = CDbl(parts(5).Replace("TT:", "").Trim())

                Dim sqlCT As String = "INSERT INTO ChiTietHD(MaHD, MaHang, SoLuong, DonGia, GiamGia, ThanhTien) 
                                   VALUES(@mahd,@mahang,@sl,@dg,@gg,@tt)"
                Using cmd As New SqlCommand(sqlCT, conn)
                    cmd.Parameters.AddWithValue("@mahd", cboMaHD.Text)
                    cmd.Parameters.AddWithValue("@mahang", maHang)
                    cmd.Parameters.AddWithValue("@sl", soLuong)
                    cmd.Parameters.AddWithValue("@dg", donGia)
                    cmd.Parameters.AddWithValue("@gg", giamGia)
                    cmd.Parameters.AddWithValue("@tt", thanhTien)
                    cmd.ExecuteNonQuery()
                End Using
            Next

            MessageBox.Show("Lưu hóa đơn thành công!", "Thông báo")

        Catch ex As Exception
            MessageBox.Show("Lỗi lưu hóa đơn: " & ex.Message)
        End Try
    End Sub

    ' ======================= NÚT HỦY HÓA ĐƠN =========================
    Private Sub btHuyHD_Click(sender As Object, e As EventArgs) Handles btHuyHD.Click
        Try
            Dim mahd As String = cboMaHD.Text
            If mahd = "" Then
                MessageBox.Show("Chưa chọn hóa đơn để hủy")
                Return
            End If

            ' Xóa chi tiết trước
            Dim sqlCT As String = "DELETE FROM ChiTietHD WHERE MaHD=@mahd"
            Using cmd As New SqlCommand(sqlCT, conn)
                cmd.Parameters.AddWithValue("@mahd", mahd)
                cmd.ExecuteNonQuery()
            End Using

            ' Xóa hóa đơn
            Dim sqlHD As String = "DELETE FROM HoaDon WHERE MaHD=@mahd"
            Using cmd As New SqlCommand(sqlHD, conn)
                cmd.Parameters.AddWithValue("@mahd", mahd)
                cmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Đã hủy hóa đơn " & mahd)
            btThemHD.PerformClick() ' reset form sau khi hủy

        Catch ex As Exception
            MessageBox.Show("Lỗi hủy hóa đơn: " & ex.Message)
        End Try
    End Sub

    Private Sub PrintHoaDon(sender As Object, e As Printing.PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim font As New Font("Arial", 12)

        g.DrawString("HÓA ĐƠN BÁN HÀNG", New Font("Arial", 16, FontStyle.Bold), Brushes.Black, 200, 50)
        g.DrawString("Mã HĐ: " & cboMaHD.Text, font, Brushes.Black, 50, 100)
        g.DrawString("Khách hàng: " & txtTenKhachHang.Text, font, Brushes.Black, 50, 130)
        g.DrawString("Địa chỉ: " & txtDiaChi.Text, font, Brushes.Black, 50, 160)
        g.DrawString("Điện thoại: " & txtDienThoai.Text, font, Brushes.Black, 50, 190)
        g.DrawString("Ngày lập: " & dtNgay.Value.ToShortDateString(), font, Brushes.Black, 50, 220)

        Dim y As Integer = 260
        For Each item As String In lstDanhSach.Items
            g.DrawString(item, font, Brushes.Black, 50, y)
            y += 30
        Next

        g.DrawString("Tổng tiền: " & txtTongTien.Text, font, Brushes.Black, 50, y + 20)

    End Sub


    Private Sub btnInHoaDon_Click(sender As Object, e As EventArgs) Handles btInHD.Click
        ' Sao chép toàn bộ nội dung hiện tại ra list tạm
        Dim temp As New List(Of String)
        For i As Integer = 0 To lstDanhSach.Items.Count - 1
            temp.Add(lstDanhSach.Items(i).ToString())
        Next

        ' Xoá toàn bộ trong ListBox chính
        lstDanhSach.Items.Clear()

        ' Thêm tiêu đề hoá đơn
        lstDanhSach.Items.Add("========== HÓA ĐƠN BÁN HÀNG ==========")
        lstDanhSach.Items.Add("Mã HD      : " & cboMaHD.Text)
        lstDanhSach.Items.Add("Ngày lập   : " & dtNgay.Value.ToString("dd/MM/yyyy"))
        lstDanhSach.Items.Add("Khách hàng : " & txtTenKhachHang.Text)
        lstDanhSach.Items.Add("Địa chỉ    : " & txtDiaChi.Text)
        lstDanhSach.Items.Add("Điện thoại : " & txtDienThoai.Text)
        lstDanhSach.Items.Add("======================================")
        lstDanhSach.Items.Add(String.Format("{0,-10}{1,-20}{2,6}{3,12}{4,12}", "Mã", "Tên hàng", "SL", "Đơn giá", "Thành tiền"))

        ' Add lại danh sách hàng từ mảng tạm
        For Each dong As String In temp
            lstDanhSach.Items.Add(dong)
        Next

        ' Thêm dòng tổng cộng
        lstDanhSach.Items.Add("======================================")
        lstDanhSach.Items.Add("TỔNG CỘNG: " & txtTongTien.Text)
    End Sub


    Private Sub btnTimKiem_Click(sender As Object, e As EventArgs) Handles btTimKiem.Click
        Dim tuKhoa As String = txtTimKiem.Text.Trim().ToLower()
        Dim timThay As Boolean = False

        ' Xóa chọn cũ
        lstDanhSach.ClearSelected()

        ' Duyệt tất cả các dòng trong ListBox
        For i As Integer = 0 To lstDanhSach.Items.Count - 1
            If lstDanhSach.Items(i).ToString().ToLower().Contains(tuKhoa) Then
                lstDanhSach.SetSelected(i, True)
                timThay = True
                Exit For ' Nếu muốn tìm nhiều kết quả thì bỏ dòng này
            End If
        Next

        If Not timThay Then
            MessageBox.Show("Không tìm thấy kết quả!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub


    Private Sub TinhTongTien()
        Dim tong As Decimal = 0

        For Each dong As String In lstDanhSach.Items
            Dim parts() As String = dong.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
            If parts.Length >= 5 Then
                Dim thanhTien As Decimal
                If Decimal.TryParse(parts(parts.Length - 1), thanhTien) Then
                    tong += thanhTien
                End If
            End If
        Next

        ' Hiển thị vào textbox
        txtTongTien.Text = tong.ToString("N0")
    End Sub

    Private Sub btInTien_Click(sender As Object, e As EventArgs) Handles btInTien.Click
        ' Lấy dữ liệu
        Dim maHang As String = cboMaHang.Text
        Dim tenHang As String = txtTenHang.Text
        Dim soLuong As Integer = Val(txtSoLuong.Text)
        Dim donGia As Decimal = Val(TxtDonGia.Text)
        Dim giamGia As Decimal = Val(txtGiamGia.Text)

        ' Tính thành tiền
        Dim thanhTien As Decimal = soLuong * donGia * (1 - giamGia / 100)

        ' Thêm vào ListBox
        Dim dong As String = String.Format("{0,-8}{1,-15}{2,5}{3,12:N0}{4,12:N0}", maHang, tenHang, soLuong, donGia, thanhTien)
        lstDanhSach.Items.Add(dong)

        ' Gọi hàm tính tổng tiền luôn
        TinhTongTien()
    End Sub
    Private Sub btnDong_Click(sender As Object, e As EventArgs) Handles btnDong.Click
        Me.Close()   ' Đóng form hiện tại
    End Sub

End Class
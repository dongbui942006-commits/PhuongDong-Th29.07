Imports QuanLyBanHang

Public Class FormDangNhap

    Private Sub btDangNhap_Click(sender As Object, e As EventArgs) Handles btDangNhap.Click
        Dim user As String = txtTenDangNhap.Text.Trim()
        Dim pass As String = txtMatKhau.Text.Trim()

        ' Kiểm tra TenTk/MK 
        If user = "PhuongDong" AndAlso pass = "123456" Then
            MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Mở Form chính
            Dim frm As New FormChinh()
            frm.Show()
            Me.Hide()
        Else
            MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtTenDangNhap.Clear()
            txtMatKhau.Focus()
        End If
    End Sub

    Private Sub btThoat_Click(sender As Object, e As EventArgs) Handles btThoat.Click
        ' Thoát toàn bộ ứng dụng
        Application.Exit()
    End Sub

End Class
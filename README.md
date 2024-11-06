Điều kiền sử dụng: dotnet 8, có địa chỉ ip puplic, mở nat port trên router là 8081 đi ra và đi vào, nếu địa chỉ ip động nên cần dịch vụ ddns có sẵn thỉ mới sử dụng được   

**Chức năng:
  + Tự động phân giải tên miền thành địa chỉ ip  puplic động để các Client từ bên ngoài có thể truy cập vào
  + Khi có người kết nối Nhận tên người dùng từ Client gửi lên kiểm tra có bị trùng tên hay không nếu trùng thì báo lỗi
  + Thêm tên người dùng vào danh sách client
  + Khi có người ngắt kết nối thì thông báo lên client đã tắt và xóa đi các soket liên quan 
  + Forward các gói tin theo yêu cầu của người dùng để 
    

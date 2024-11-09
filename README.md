Điều kiền sử dụng:
  + Dotnet 8
  + Mở nat port trên router là 8081 đi ra và đi vào
  + Địa chỉ ip động nên cần dịch vụ ddns để client có thể kết nối vào server
Cách sử dụng
  + Nhập đúng địa chỉ mà router cấp cho cho thiết bị đóng vai trò làm server
  + Thực hiện NAT PORT trỏ đúng internal port và external port là 8081 chọn đúng máy được chọn làm sever
Chức năng:
  + Tự động phân giải tên miền thành địa chỉ ip  puplic động để các Client từ bên ngoài có thể truy cập vào
  + Khi có người kết nối Nhận tên người dùng từ Client gửi lên kiểm tra có bị trùng tên hay không nếu trùng thì báo lỗi
  + Thêm tên người dùng vào danh sách client
  + Khi có người ngắt kết nối thì thông báo lên client đã tắt và xóa đi các soket liên quan 
  + Forward các gói tin theo yêu cầu của người dùng để 
    

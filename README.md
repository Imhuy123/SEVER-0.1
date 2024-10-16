Điều kiền sử dụng: dotnet 8, có địa chỉ ip puplic, mở port trên router trên 8081 ,vì là địa chỉ ip động nên cần dịch vụ ddns có sẵn thỉ mới sử dụng được  
Sử dụng thư viện json để lưu trữ tin nhắn  
**Chức năng:
  + Tự động phân giải tên miền thành địa chỉ ip  puplic động để các Client từ bên ngoài có thể truy cập vào
  + Khi có người kết nối Nhận tên người dùng từ Client gửi lên kiểm tra có bị trùng tên hay không nếu trùng thì báo lỗi erro
  + Thêm tên người dùng vào danh sách client
  + Khi có người ngắt kết nối thì thông báo lên client đã tắt và xóa đi các soket liên quan 
  + Gửi lịch sử nhắn tin nếu có khi người dùng kết nối lần đầu
  + Nhận tin nhắn từ người gửi ,sau đó gửi về chat fom cho người gửi và người nhận
    

# BackendGreenIot

## Hướng dẫn cài đặt và chạy API quản lý cảm biến và thiết bị sử dụng **Firebase**

### 1. **Yêu cầu hệ thống**
Trước khi chạy dự án, hãy đảm bảo rằng bạn đã cài đặt các công cụ sau:
- **.NET 6 SDK**: [Tải tại đây](https://dotnet.microsoft.com/download/dotnet/6.0)
- **Firebase**: Đăng ký tài khoản Firebase và tạo một dự án [Firebase Console](https://console.firebase.google.com/).
- **Git**: [Tải Git](https://git-scm.com/downloads)
- **Postman** hoặc các công cụ test API khác (tùy chọn, dùng để kiểm tra các endpoint).

---

### 2. **Clone dự án**
Dùng Git để clone repository về máy tính của bạn:
```bash
https://github.com/yamakazeAnkk/BackendGreenIot.git
```
### 3. Khôi phục thư viện
```bash
dotnet restore
```
### 4. Cấu hình Firebase
tải về file google-services.json và đặt trong thư mục gốc của dự án.

### 5. Chạy API
Khởi động ứng dụng bằng lệnh:
```bash
dotnet build
dotnet run
```
API sẽ chạy tại:
Địa chỉ chính: http://localhost:8080 (port mặc định)
 ```Plain Text
BookStore/
├── Controllers/      # Chứa các controller xử lý yêu cầu API
├── DTOs/             # Chứa các Data Transfer Objects (DTO) để trao đổi dữ liệu
├── Data/             # Chứa các lớp và cấu hình liên quan đến database
├── Helper/           # Chứa các helper class và phương thức hỗ trợ
├── Migrations/       # Chứa các file migration được tạo bởi Entity Framework
├── Models/           # Chứa các model đại diện cho dữ liệu trong ứng dụng
├── Properties/       # Chứa các file cấu hình dự án
├── Repositories/     # Chứa các lớp thực hiện các tác vụ truy vấn cơ sở dữ liệu
├── Services/         # Chứa các lớp xử lý logic nghiệp vụ
├── bin/
│   └── Debug/
│       └── net6.0/   # Thư mục build dự án ở chế độ debug (tự động sinh)
└── obj/              # Thư mục tạm tự động sinh khi biên dịch

```


# Hệ thống quản lý thiết bị và mượn trả (C#)

Bài thực hành Lab tuần 6 - OOP, kết tập, interface, đa hình và ngoại lệ nghiệp vụ.

- Mã sinh viên: 202418901
- Họ tên: Nguyễn Việt Hoàng

## Yêu cầu

- .NET 8 SDK

## Cách chạy

```bash
dotnet run
```

Chương trình chạy ba phần kiểm thử và in kết quả ra màn hình:

- A. Device và ClassRoom (kết tập, đa hình, ràng buộc dữ liệu)
- B. Luồng thành công (mục 11.1 của đề)
- C. Luồng phát sinh ngoại lệ (mục 11.2 của đề: 7 tình huống, cộng 1 tình huống bổ sung)

## Cấu trúc mã nguồn

| File | Nội dung |
|---|---|
| `DeviceStatus.cs` | enum Available, Borrowed, UnderMaintenance |
| `Device.cs` | lớp trừu tượng thiết bị |
| `TeachingDevice.cs` | thiết bị cho mượn (số ngày, số tiết, phí muộn) |
| `AirConditionerRemote.cs`, `WirelessMicrophone.cs`, `ProjectorRemote.cs` | ba loại thiết bị |
| `ClassRoom.cs` | phòng học, kết tập thiết bị |
| `IBorrower.cs`, `Lecturer.cs`, `TeachingAssistant.cs` | người mượn |
| `BorrowItem.cs`, `BorrowingSlip.cs` | dòng mượn và phiếu mượn |
| `BorrowingExceptions.cs` | hệ phân cấp ngoại lệ nghiệp vụ |
| `Program.cs` | lớp ứng dụng, chạy kiểm thử |

## Ghi chú

Các thông số nghiệp vụ không có trong đề (phí trả muộn mỗi ngày, số tiết tối đa, tỉ lệ chi phí bảo trì) là giả định của sinh viên, xem báo cáo mục 2.3 và 2.4.

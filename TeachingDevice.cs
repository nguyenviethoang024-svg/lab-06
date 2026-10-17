/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>
/// Thiết bị phục vụ giảng dạy, có thể cho mượn.
/// Bổ sung các quy tắc mượn: số ngày tối đa, số tiết tối đa và phí trả muộn.
/// Các lớp dẫn xuất chỉ cần khai báo thông số của riêng mình.
/// </summary>
public abstract class TeachingDevice : Device
{
    /// <summary>Tên loại thiết bị, dùng khi hiển thị.</summary>
    public abstract string DeviceTypeName { get; }

    /// <summary>Số ngày được phép mượn tối đa.</summary>
    public abstract int MaxBorrowDays { get; }

    /// <summary>Số tiết được phép mượn tối đa.</summary>
    public abstract int MaxPeriods { get; }

    /// <summary>Phí phạt cho mỗi ngày trả muộn.</summary>
    public abstract decimal LateFeePerDay { get; }

    protected TeachingDevice(string deviceId, string name, int yearInService, decimal purchasePrice)
        : base(deviceId, name, yearInService, purchasePrice)
    {
    }

    public override string ToString() =>
        $"{DeviceTypeName} {base.ToString()} | Tối đa: {MaxBorrowDays} ngày, {MaxPeriods} tiết";
}

/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>
/// Trạng thái của một thiết bị trong hệ thống.
/// </summary>
public enum DeviceStatus
{
    /// <summary>Sẵn sàng cho mượn.</summary>
    Available,

    /// <summary>Đang được mượn (thuộc một phiếu mượn chưa trả).</summary>
    Borrowed,

    /// <summary>Đang bảo trì, không được phép mượn.</summary>
    UnderMaintenance
}

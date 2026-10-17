/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>
/// Hợp đồng chung cho người mượn thiết bị.
/// Phiếu mượn chỉ phụ thuộc vào interface này, không phụ thuộc lớp cụ thể.
/// </summary>
public interface IBorrower
{
    /// <summary>Mã người mượn.</summary>
    string BorrowerId { get; }

    /// <summary>Họ tên người mượn.</summary>
    string FullName { get; }

    /// <summary>Số thiết bị tối đa được mượn trên một phiếu.</summary>
    int GetMaximumDevices();
}

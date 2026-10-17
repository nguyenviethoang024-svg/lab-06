/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>Giảng viên: được mượn tối đa 3 thiết bị trên một phiếu.</summary>
public class Lecturer : IBorrower
{
    private const int MaxDevicesPerSlip = 3;

    public string BorrowerId { get; }
    public string FullName { get; }

    public Lecturer(string borrowerId, string fullName)
    {
        if (string.IsNullOrWhiteSpace(borrowerId))
            throw new ArgumentException("Mã người mượn không được rỗng.", nameof(borrowerId));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Họ tên không được rỗng.", nameof(fullName));

        BorrowerId = borrowerId.Trim();
        FullName = fullName.Trim();
    }

    public int GetMaximumDevices() => MaxDevicesPerSlip;

    public override string ToString() => $"Giảng viên {FullName} ({BorrowerId})";
}

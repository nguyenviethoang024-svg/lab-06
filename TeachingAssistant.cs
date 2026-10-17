/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>Trợ giảng: được mượn tối đa 2 thiết bị trên một phiếu.</summary>
public class TeachingAssistant : IBorrower
{
    private const int MaxDevicesPerSlip = 2;

    public string BorrowerId { get; }
    public string FullName { get; }

    public TeachingAssistant(string borrowerId, string fullName)
    {
        if (string.IsNullOrWhiteSpace(borrowerId))
            throw new ArgumentException("Mã người mượn không được rỗng.", nameof(borrowerId));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Họ tên không được rỗng.", nameof(fullName));

        BorrowerId = borrowerId.Trim();
        FullName = fullName.Trim();
    }

    public int GetMaximumDevices() => MaxDevicesPerSlip;

    public override string ToString() => $"Trợ giảng {FullName} ({BorrowerId})";
}

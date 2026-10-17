/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>Điều khiển máy chiếu.</summary>
public class ProjectorRemote : TeachingDevice
{
    public override string DeviceTypeName => "Điều khiển máy chiếu";
    public override int MaxBorrowDays => 3;
    public override int MaxPeriods => 6;
    public override decimal LateFeePerDay => 15_000m;

    public ProjectorRemote(string deviceId, string name, int yearInService, decimal purchasePrice)
        : base(deviceId, name, yearInService, purchasePrice)
    {
    }

    /// <summary>Chi phí bảo trì: 7% giá mua mỗi năm.</summary>
    public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.07m;
}

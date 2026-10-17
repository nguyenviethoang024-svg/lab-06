/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>Micro không dây.</summary>
public class WirelessMicrophone : TeachingDevice
{
    /// <summary>Chi phí cố định hằng năm để thay pin/đầu thu.</summary>
    private const decimal BatteryReplacementCost = 150_000m;

    public override string DeviceTypeName => "Micro không dây";
    public override int MaxBorrowDays => 1;
    public override int MaxPeriods => 4;
    public override decimal LateFeePerDay => 20_000m;

    public WirelessMicrophone(string deviceId, string name, int yearInService, decimal purchasePrice)
        : base(deviceId, name, yearInService, purchasePrice)
    {
    }

    /// <summary>Chi phí bảo trì: 10% giá mua cộng chi phí thay pin cố định.</summary>
    public override decimal CalculateAnnualMaintenanceCost() =>
        PurchasePrice * 0.10m + BatteryReplacementCost;
}

/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>Điều khiển điều hòa.</summary>
public class AirConditionerRemote : TeachingDevice
{
    public override string DeviceTypeName => "Điều khiển điều hòa";
    public override int MaxBorrowDays => 5;
    public override int MaxPeriods => 6;
    public override decimal LateFeePerDay => 10_000m;

    public AirConditionerRemote(string deviceId, string name, int yearInService, decimal purchasePrice)
        : base(deviceId, name, yearInService, purchasePrice)
    {
    }

    /// <summary>Chi phí bảo trì: 5% giá mua mỗi năm (thay pin, vệ sinh).</summary>
    public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.05m;
}

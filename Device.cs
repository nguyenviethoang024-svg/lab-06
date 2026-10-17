/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>
/// Lớp trừu tượng cho mọi thiết bị của trường.
/// Mọi ràng buộc dữ liệu (mã, giá, năm sử dụng) được kiểm tra ngay tại đây
/// nên đối tượng Device luôn ở trạng thái hợp lệ.
/// </summary>
public abstract class Device
{
    private string _name = string.Empty;
    private int _yearInService;
    private decimal _purchasePrice;

    /// <summary>Mã thiết bị. Chỉ được thiết lập khi khởi tạo (không có setter).</summary>
    public string DeviceId { get; }

    /// <summary>Tên thiết bị (không rỗng).</summary>
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tên thiết bị không được rỗng.", nameof(Name));
            _name = value.Trim();
        }
    }

    /// <summary>Năm đưa vào sử dụng (không lớn hơn năm hiện tại).</summary>
    public int YearInService
    {
        get => _yearInService;
        set
        {
            if (value > DateTime.Now.Year)
                throw new ArgumentOutOfRangeException(nameof(YearInService), value,
                    "Năm đưa vào sử dụng không được lớn hơn năm hiện tại.");
            _yearInService = value;
        }
    }

    /// <summary>Giá mua (phải lớn hơn 0).</summary>
    public decimal PurchasePrice
    {
        get => _purchasePrice;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(PurchasePrice), value,
                    "Giá mua phải lớn hơn 0.");
            _purchasePrice = value;
        }
    }

    /// <summary>Trạng thái hoạt động. Chỉ đổi qua các phương thức bên dưới để bảo đảm hợp lệ.</summary>
    public DeviceStatus Status { get; private set; }

    protected Device(string deviceId, string name, int yearInService, decimal purchasePrice)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Mã thiết bị không được rỗng.", nameof(deviceId));

        DeviceId = deviceId.Trim();
        Name = name;
        YearInService = yearInService;
        PurchasePrice = purchasePrice;
        Status = DeviceStatus.Available;
    }

    /// <summary>Số năm đã sử dụng, tính đến năm hiện tại (hoặc năm truyền vào để dễ kiểm thử).</summary>
    public int GetAge(int? currentYear = null) => (currentYear ?? DateTime.Now.Year) - YearInService;

    /// <summary>Tính chi phí bảo trì dự kiến trong một năm. Mỗi loại thiết bị tự cài đặt (đa hình).</summary>
    public abstract decimal CalculateAnnualMaintenanceCost();

    /// <summary>Chuyển thiết bị sang Borrowed. Chỉ hợp lệ khi đang Available.</summary>
    /// <exception cref="DeviceUnavailableException">Thiết bị không ở trạng thái Available.</exception>
    public void MarkAsBorrowed()
    {
        if (Status != DeviceStatus.Available)
            throw new DeviceUnavailableException(DeviceId, Status);
        Status = DeviceStatus.Borrowed;
    }

    /// <summary>Đưa thiết bị về Available sau khi trả. Nếu đang bảo trì thì giữ nguyên.</summary>
    public void MarkAsReturned()
    {
        if (Status == DeviceStatus.Borrowed)
            Status = DeviceStatus.Available;
    }

    /// <summary>Đưa thiết bị đi bảo trì. Không thể bảo trì khi thiết bị đang được mượn.</summary>
    public void SendToMaintenance()
    {
        if (Status == DeviceStatus.Borrowed)
            throw new InvalidOperationException(
                $"Không thể bảo trì thiết bị '{DeviceId}' khi đang được mượn.");
        Status = DeviceStatus.UnderMaintenance;
    }

    /// <summary>Kết thúc bảo trì, thiết bị sẵn sàng trở lại.</summary>
    public void FinishMaintenance()
    {
        if (Status == DeviceStatus.UnderMaintenance)
            Status = DeviceStatus.Available;
    }

    /// <summary>Biểu diễn thông tin thiết bị dưới dạng chuỗi.</summary>
    public override string ToString() =>
        $"[{DeviceId}] {Name} | Năm SD: {YearInService} | Giá: {PurchasePrice:N0} | Trạng thái: {Status}";
}

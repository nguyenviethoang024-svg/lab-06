/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>
/// Phòng học, kết tập (aggregation) danh sách thiết bị.
/// Thiết bị tồn tại độc lập: xóa thiết bị khỏi phòng không hủy đối tượng thiết bị.
/// </summary>
public class ClassRoom
{
    /// <summary>Số năm sử dụng vượt quá ngưỡng này thì thiết bị cần bảo trì.</summary>
    private const int MaintenanceAgeThreshold = 5;

    private readonly List<Device> _devices = new();

    public string RoomId { get; }
    public string RoomName { get; }
    public int Capacity { get; }

    /// <summary>Danh sách thiết bị chỉ đọc, bên ngoài không thể thêm/xóa trực tiếp.</summary>
    public IReadOnlyList<Device> Devices => _devices.AsReadOnly();

    public ClassRoom(string roomId, string roomName, int capacity)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new ArgumentException("Mã phòng không được rỗng.", nameof(roomId));
        if (string.IsNullOrWhiteSpace(roomName))
            throw new ArgumentException("Tên phòng không được rỗng.", nameof(roomName));
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Sức chứa phải lớn hơn 0.");

        RoomId = roomId.Trim();
        RoomName = roomName.Trim();
        Capacity = capacity;
    }

    /// <summary>Thêm thiết bị vào phòng.</summary>
    /// <exception cref="ArgumentNullException">Thiết bị là null.</exception>
    /// <exception cref="InvalidOperationException">Trùng mã thiết bị trong phòng.</exception>
    public void AddDevice(Device device)
    {
        ArgumentNullException.ThrowIfNull(device);

        if (FindDevice(device.DeviceId) != null)
            throw new InvalidOperationException(
                $"Phòng '{RoomId}' đã có thiết bị mã '{device.DeviceId}'.");

        _devices.Add(device);
    }

    /// <summary>Xóa thiết bị khỏi phòng theo mã. Trả về true nếu xóa được.</summary>
    public bool RemoveDevice(string deviceId)
    {
        var device = FindDevice(deviceId);
        return device != null && _devices.Remove(device);
    }

    /// <summary>Tìm thiết bị theo mã, không thấy trả về null.</summary>
    public Device? FindDevice(string deviceId) =>
        _devices.FirstOrDefault(d => d.DeviceId == deviceId);

    /// <summary>
    /// Tổng chi phí bảo trì dự kiến một năm của cả phòng.
    /// Dùng đa hình: gọi CalculateAnnualMaintenanceCost() của từng thiết bị,
    /// không kiểm tra kiểu bằng if/switch.
    /// </summary>
    public decimal CalculateAnnualMaintenanceCost() =>
        _devices.Sum(d => d.CalculateAnnualMaintenanceCost());

    /// <summary>Thiết bị đang bảo trì hoặc đã dùng trên 5 năm.</summary>
    public List<Device> GetDevicesRequiringMaintenance() =>
        _devices
            .Where(d => d.Status == DeviceStatus.UnderMaintenance
                        || d.GetAge() > MaintenanceAgeThreshold)
            .ToList();
}

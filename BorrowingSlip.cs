/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>
/// Phiếu mượn thiết bị của một người mượn, gồm nhiều BorrowItem (composition).
/// Xóa phiếu không xóa thiết bị và không xóa người mượn.
/// </summary>
public class BorrowingSlip
{
    private readonly List<BorrowItem> _items = new();

    public string SlipId { get; }
    public IBorrower Borrower { get; }
    public string TeachingRoom { get; }
    public DateTime BorrowedDate { get; }

    /// <summary>Danh sách dòng mượn chỉ đọc; chỉ thay đổi qua AddDevice/ReturnDevice.</summary>
    public IReadOnlyList<BorrowItem> Items => _items.AsReadOnly();

    public BorrowingSlip(string slipId, IBorrower borrower, string teachingRoom, DateTime borrowedDate)
    {
        if (string.IsNullOrWhiteSpace(slipId))
            throw new ArgumentException("Mã phiếu không được rỗng.", nameof(slipId));
        ArgumentNullException.ThrowIfNull(borrower);
        if (string.IsNullOrWhiteSpace(teachingRoom))
            throw new ArgumentException("Phòng dạy không được rỗng.", nameof(teachingRoom));

        SlipId = slipId.Trim();
        Borrower = borrower;
        TeachingRoom = teachingRoom.Trim();
        BorrowedDate = borrowedDate;
    }

    /// <summary>
    /// Thêm thiết bị vào phiếu (mượn theo ngày, mặc định tiết 1).
    /// </summary>
    public void AddDevice(TeachingDevice device, int borrowDays) =>
        AddDevice(device, borrowDays, 1, 1);

    /// <summary>
    /// Thêm thiết bị vào phiếu kèm tiết mượn.
    /// Quy trình an toàn: kiểm tra toàn bộ -> tạo item -> đổi trạng thái thiết bị -> thêm item.
    /// Nếu có lỗi ở bước kiểm tra hoặc tạo item thì thiết bị và phiếu chưa bị thay đổi.
    /// </summary>
    public void AddDevice(TeachingDevice device, int borrowDays, int fromPeriod, int toPeriod)
    {
        // Bước 1: kiểm tra mọi điều kiện, chưa thay đổi gì.
        ValidateBorrowing(device, borrowDays, fromPeriod, toPeriod);

        // Bước 2: tạo item (có thể ném ngoại lệ, nhưng vẫn chưa đổi trạng thái thiết bị).
        var item = new BorrowItem(device, BorrowedDate, BorrowedDate.AddDays(borrowDays), fromPeriod, toPeriod);

        // Bước 3: chỉ khi mọi thứ hợp lệ mới thay đổi trạng thái.
        device.MarkAsBorrowed();
        _items.Add(item);
    }

    /// <summary>
    /// Trả thiết bị theo mã. Trả về phí trả muộn của thiết bị đó.
    /// </summary>
    /// <exception cref="ArgumentException">Mã thiết bị rỗng.</exception>
    /// <exception cref="DeviceNotInSlipException">Thiết bị không thuộc phiếu.</exception>
    /// <exception cref="DeviceAlreadyReturnedException">Thiết bị đã trả trước đó.</exception>
    /// <exception cref="InvalidReturnDateException">Ngày trả trước ngày mượn.</exception>
    public decimal ReturnDevice(string deviceId, DateTime returnedDate)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Mã thiết bị không được rỗng.", nameof(deviceId));

        var item = _items.FirstOrDefault(i => i.Device.DeviceId == deviceId.Trim())
                   ?? throw new DeviceNotInSlipException(deviceId);

        // item.Return kiểm tra "đã trả" và "ngày trả" rồi mới ghi nhận.
        item.Return(returnedDate);

        // Chỉ khi ghi nhận thành công mới đưa thiết bị về Available.
        item.Device.MarkAsReturned();

        return item.CalculateLateFee();
    }

    /// <summary>Tổng phí trả muộn của tất cả thiết bị đã trả trong phiếu.</summary>
    public decimal CalculateTotalLateFee() => _items.Sum(i => i.CalculateLateFee());

    /// <summary>Phiếu có ít nhất một thiết bị trả muộn.</summary>
    public bool HasLateItems() => _items.Any(i => i.GetLateDays() > 0);

    /// <summary>Phiếu còn ít nhất một thiết bị chưa trả.</summary>
    public bool HasUnreturnedItems() => _items.Any(i => !i.IsReturned);

    /// <summary>
    /// Kiểm tra điều kiện thêm thiết bị theo thứ tự trong đề:
    /// null -> số ngày -> tiết -> trùng thiết bị -> giới hạn người mượn -> trạng thái.
    /// Phương thức này không thay đổi dữ liệu.
    /// </summary>
    private void ValidateBorrowing(TeachingDevice device, int borrowDays, int fromPeriod, int toPeriod)
    {
        ArgumentNullException.ThrowIfNull(device);

        if (borrowDays <= 0 || borrowDays > device.MaxBorrowDays)
            throw new InvalidBorrowPeriodException(device.DeviceId, borrowDays, device.MaxBorrowDays);

        BorrowItem.ValidatePeriods(device, fromPeriod, toPeriod);

        if (_items.Any(i => i.Device.DeviceId == device.DeviceId))
            throw new DuplicateDeviceException(device.DeviceId);

        if (_items.Count >= Borrower.GetMaximumDevices())
            throw new BorrowLimitExceededException(Borrower.BorrowerId, Borrower.GetMaximumDevices());

        if (device.Status != DeviceStatus.Available)
            throw new DeviceUnavailableException(device.DeviceId, device.Status);
    }

    public override string ToString() =>
        $"Phiếu {SlipId} | {Borrower} | Phòng {TeachingRoom} | Ngày mượn {BorrowedDate:dd/MM/yyyy} | {_items.Count} thiết bị";
}

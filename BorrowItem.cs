/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>
/// Một dòng trong phiếu mượn: ghi nhận việc mượn một thiết bị.
/// BorrowItem chỉ tham chiếu đến TeachingDevice, không sở hữu thiết bị.
/// </summary>
public class BorrowItem
{
    /// <summary>Số tiết lớn nhất trong một ngày học.</summary>
    public const int MaxPeriodNumber = 12;

    private int _fromPeriod;
    private int _toPeriod;

    /// <summary>Thiết bị được mượn (không null).</summary>
    public TeachingDevice Device { get; }

    /// <summary>Ngày mượn.</summary>
    public DateTime BorrowedDate { get; }

    /// <summary>Hạn trả (phải sau hoặc bằng ngày mượn).</summary>
    public DateTime DueDate { get; }

    /// <summary>Mượn từ tiết. Mỗi lần gán đều được kiểm tra hợp lệ.</summary>
    public int FromPeriod
    {
        get => _fromPeriod;
        set
        {
            ValidatePeriods(Device, value, _toPeriod);
            _fromPeriod = value;
        }
    }

    /// <summary>Mượn đến tiết. Mỗi lần gán đều được kiểm tra hợp lệ.</summary>
    public int ToPeriod
    {
        get => _toPeriod;
        set
        {
            ValidatePeriods(Device, _fromPeriod, value);
            _toPeriod = value;
        }
    }

    /// <summary>Ngày trả thực tế; null nếu chưa trả.</summary>
    public DateTime? ReturnedDate { get; private set; }

    /// <summary>Đã trả hay chưa.</summary>
    public bool IsReturned => ReturnedDate.HasValue;

    /// <exception cref="ArgumentNullException">Device là null.</exception>
    /// <exception cref="ArgumentException">Hạn trả trước ngày mượn.</exception>
    /// <exception cref="InvalidPeriodRangeException">Tiết mượn không hợp lệ.</exception>
    public BorrowItem(TeachingDevice device, DateTime borrowedDate, DateTime dueDate,
                      int fromPeriod = 1, int toPeriod = 1)
    {
        ArgumentNullException.ThrowIfNull(device);

        if (dueDate.Date < borrowedDate.Date)
            throw new ArgumentException("Hạn trả (DueDate) phải sau hoặc bằng ngày mượn.", nameof(dueDate));

        ValidatePeriods(device, fromPeriod, toPeriod);

        Device = device;
        BorrowedDate = borrowedDate;
        DueDate = dueDate;
        _fromPeriod = fromPeriod;
        _toPeriod = toPeriod;
        ReturnedDate = null;
    }

    /// <summary>
    /// Kiểm tra khoảng tiết: nằm trong [1, MaxPeriodNumber], từ tiết &lt;= đến tiết
    /// và số tiết mượn không vượt giới hạn của thiết bị.
    /// Để public static để BorrowingSlip kiểm tra trước khi tạo item.
    /// </summary>
    public static void ValidatePeriods(TeachingDevice device, int fromPeriod, int toPeriod)
    {
        bool outOfRange = fromPeriod < 1 || toPeriod > MaxPeriodNumber || fromPeriod > toPeriod;
        bool tooMany = !outOfRange && (toPeriod - fromPeriod + 1) > device.MaxPeriods;

        if (outOfRange || tooMany)
            throw new InvalidPeriodRangeException(device.DeviceId, fromPeriod, toPeriod, device.MaxPeriods);
    }

    /// <summary>
    /// Ghi nhận trả thiết bị. Mọi kiểm tra diễn ra trước khi thay đổi trạng thái,
    /// nên nếu lỗi thì item giữ nguyên.
    /// </summary>
    /// <exception cref="DeviceAlreadyReturnedException">Đã trả trước đó.</exception>
    /// <exception cref="InvalidReturnDateException">Ngày trả trước ngày mượn.</exception>
    public void Return(DateTime returnedDate)
    {
        if (IsReturned)
            throw new DeviceAlreadyReturnedException(Device.DeviceId);

        if (returnedDate.Date < BorrowedDate.Date)
            throw new InvalidReturnDateException(Device.DeviceId, returnedDate, BorrowedDate);

        ReturnedDate = returnedDate;
    }

    /// <summary>Số ngày trả muộn (0 nếu chưa trả hoặc trả đúng hạn).</summary>
    public int GetLateDays()
    {
        if (!ReturnedDate.HasValue)
            return 0;

        int lateDays = (ReturnedDate.Value.Date - DueDate.Date).Days;
        return lateDays > 0 ? lateDays : 0;
    }

    /// <summary>Phí trả muộn = số ngày muộn x phí mỗi ngày của thiết bị.</summary>
    public decimal CalculateLateFee() => GetLateDays() * Device.LateFeePerDay;
}

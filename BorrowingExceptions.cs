/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
namespace DeviceManagement;

/// <summary>
/// Ngoại lệ cơ sở của mọi lỗi nghiệp vụ mượn/trả thiết bị.
/// Lớp ứng dụng chỉ cần bắt lớp này để xử lý chung mọi lỗi nghiệp vụ.
/// </summary>
public class BorrowingException : Exception
{
    public BorrowingException(string message)
        : base(message)
    {
    }

    public BorrowingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>Phát sinh khi thiết bị không ở trạng thái Available.</summary>
public class DeviceUnavailableException : BorrowingException
{
    /// <summary>Mã thiết bị gây lỗi.</summary>
    public string DeviceId { get; }

    /// <summary>Trạng thái hiện tại của thiết bị.</summary>
    public DeviceStatus CurrentStatus { get; }

    public DeviceUnavailableException(string deviceId, DeviceStatus currentStatus)
        : base($"Thiết bị '{deviceId}' không sẵn sàng để mượn (trạng thái hiện tại: {currentStatus}).")
    {
        DeviceId = deviceId;
        CurrentStatus = currentStatus;
    }
}

/// <summary>Phát sinh khi người mượn đã đạt số thiết bị tối đa trên một phiếu.</summary>
public class BorrowLimitExceededException : BorrowingException
{
    /// <summary>Mã người mượn.</summary>
    public string BorrowerId { get; }

    /// <summary>Số thiết bị tối đa người mượn được phép.</summary>
    public int MaximumDevices { get; }

    public BorrowLimitExceededException(string borrowerId, int maximumDevices)
        : base($"Người mượn '{borrowerId}' đã đạt giới hạn {maximumDevices} thiết bị trên một phiếu.")
    {
        BorrowerId = borrowerId;
        MaximumDevices = maximumDevices;
    }
}

/// <summary>Phát sinh khi số ngày mượn không dương hoặc vượt giới hạn của thiết bị.</summary>
public class InvalidBorrowPeriodException : BorrowingException
{
    /// <summary>Mã thiết bị.</summary>
    public string DeviceId { get; }

    /// <summary>Số ngày mượn được yêu cầu.</summary>
    public int RequestedDays { get; }

    /// <summary>Số ngày mượn tối đa của thiết bị.</summary>
    public int MaximumDays { get; }

    public InvalidBorrowPeriodException(string deviceId, int requestedDays, int maximumDays)
        : base($"Số ngày mượn {requestedDays} không hợp lệ với thiết bị '{deviceId}' " +
               $"(phải từ 1 đến {maximumDays} ngày).")
    {
        DeviceId = deviceId;
        RequestedDays = requestedDays;
        MaximumDays = maximumDays;
    }
}

/// <summary>
/// Ngoại lệ bổ sung: phát sinh khi tiết mượn (từ tiết - đến tiết) không hợp lệ
/// hoặc vượt số tiết tối đa của thiết bị. Tách riêng khỏi InvalidBorrowPeriodException
/// vì đơn vị là "tiết" chứ không phải "ngày", và cần mang thông tin khác.
/// </summary>
public class InvalidPeriodRangeException : BorrowingException
{
    public string DeviceId { get; }
    public int FromPeriod { get; }
    public int ToPeriod { get; }
    public int MaximumPeriods { get; }

    public InvalidPeriodRangeException(string deviceId, int fromPeriod, int toPeriod, int maximumPeriods)
        : base($"Tiết mượn từ {fromPeriod} đến {toPeriod} không hợp lệ với thiết bị '{deviceId}' " +
               $"(tiết hợp lệ từ 1 đến {BorrowItem.MaxPeriodNumber}, từ tiết <= đến tiết, tối đa {maximumPeriods} tiết).")
    {
        DeviceId = deviceId;
        FromPeriod = fromPeriod;
        ToPeriod = toPeriod;
        MaximumPeriods = maximumPeriods;
    }
}

/// <summary>Phát sinh khi cùng một thiết bị đã tồn tại trong phiếu.</summary>
public class DuplicateDeviceException : BorrowingException
{
    public string DeviceId { get; }

    public DuplicateDeviceException(string deviceId)
        : base($"Thiết bị '{deviceId}' đã có trong phiếu mượn.")
    {
        DeviceId = deviceId;
    }
}

/// <summary>Phát sinh khi yêu cầu trả một thiết bị không thuộc phiếu.</summary>
public class DeviceNotInSlipException : BorrowingException
{
    public string DeviceId { get; }

    public DeviceNotInSlipException(string deviceId)
        : base($"Thiết bị '{deviceId}' không thuộc phiếu mượn này.")
    {
        DeviceId = deviceId;
    }
}

/// <summary>Phát sinh khi thiết bị trong phiếu đã được trả trước đó.</summary>
public class DeviceAlreadyReturnedException : BorrowingException
{
    public string DeviceId { get; }

    public DeviceAlreadyReturnedException(string deviceId)
        : base($"Thiết bị '{deviceId}' đã được trả trước đó.")
    {
        DeviceId = deviceId;
    }
}

/// <summary>
/// Ngoại lệ bổ sung: phát sinh khi ngày trả không hợp lệ (trước ngày mượn).
/// Đề yêu cầu "ngày trả không hợp lệ" phải báo lỗi nhưng không liệt kê lớp riêng,
/// nên tách thành lớp riêng để lớp ứng dụng phân biệt được với các lỗi khác.
/// </summary>
public class InvalidReturnDateException : BorrowingException
{
    public string DeviceId { get; }
    public DateTime ReturnedDate { get; }
    public DateTime BorrowedDate { get; }

    public InvalidReturnDateException(string deviceId, DateTime returnedDate, DateTime borrowedDate)
        : base($"Ngày trả {returnedDate:dd/MM/yyyy} của thiết bị '{deviceId}' " +
               $"không được trước ngày mượn {borrowedDate:dd/MM/yyyy}.")
    {
        DeviceId = deviceId;
        ReturnedDate = returnedDate;
        BorrowedDate = borrowedDate;
    }
}

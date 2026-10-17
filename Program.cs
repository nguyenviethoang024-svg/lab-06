/****************
 * Mã sinh viên: 202418901
 * Họ tên: Nguyễn Việt Hoàng
 ****************/
using System.Globalization;
using System.Text;
using DeviceManagement;

/// <summary>
/// Lớp ứng dụng: chạy các tình huống kiểm thử.
/// Lớp nghiệp vụ chỉ ném ngoại lệ; việc bắt (catch) và thông báo nằm ở đây.
/// </summary>
public static class Program
{
    private static readonly DateTime SlipDate = new(2026, 10, 12);

    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        try
        {
            TestDeviceAndClassRoom();
            TestSuccessFlow();
            TestExceptionFlows();
        }
        catch (Exception ex)
        {
            // Lưới an toàn cuối cùng: lỗi ngoài dự kiến không làm chương trình sập im lặng.
            Console.WriteLine($"[LỖI NGOÀI DỰ KIẾN] {ex.GetType().Name}: {ex.Message}");
        }
        finally
        {
            Console.WriteLine();
            Console.WriteLine("=== Kết thúc chương trình ===");
        }
    }

    // ------------------------------------------------------------------
    // 1. Kiểm thử Device và ClassRoom (kết tập, đa hình)
    // ------------------------------------------------------------------
    private static void TestDeviceAndClassRoom()
    {
        PrintTitle("A. Kiểm thử Device và ClassRoom");

        var room = new ClassRoom("P-301", "Phòng học 301", 60);
        room.AddDevice(new AirConditionerRemote("AC01", "Điều khiển điều hòa Daikin", 2022, 150_000m));
        room.AddDevice(new WirelessMicrophone("MIC01", "Micro không dây Shure", 2023, 1_800_000m));
        room.AddDevice(new ProjectorRemote("PJ01", "Điều khiển máy chiếu Epson", 2021, 220_000m));
        // Thiết bị cũ (> 5 năm) và thiết bị đang bảo trì để thử GetDevicesRequiringMaintenance.
        room.AddDevice(new WirelessMicrophone("MIC02", "Micro không dây cũ", 2018, 1_200_000m));
        var broken = new AirConditionerRemote("AC02", "Điều khiển điều hòa hỏng", 2024, 150_000m);
        broken.SendToMaintenance();
        room.AddDevice(broken);

        Console.WriteLine($"Phòng {room.RoomName}, sức chứa {room.Capacity}, {room.Devices.Count} thiết bị:");
        foreach (var d in room.Devices)
            Console.WriteLine($"  {d}  -> bảo trì/năm: {d.CalculateAnnualMaintenanceCost():N0} VND");

        Console.WriteLine($"Tổng chi phí bảo trì dự kiến/năm: {room.CalculateAnnualMaintenanceCost():N0} VND");
        Console.WriteLine("Thiết bị cần bảo trì (đang bảo trì hoặc > 5 năm):");
        foreach (var d in room.GetDevicesRequiringMaintenance())
            Console.WriteLine($"  {d.DeviceId} ({d.Status}, {d.GetAge()} năm)");

        // Các quy tắc của ClassRoom và Device.
        ExpectArgumentError("Thêm thiết bị null vào phòng", () => room.AddDevice(null!));
        ExpectArgumentError("Thêm thiết bị trùng mã vào phòng",
            () => room.AddDevice(new ProjectorRemote("PJ01", "Trùng mã", 2022, 100_000m)));
        ExpectArgumentError("Tạo thiết bị với mã rỗng",
            () => _ = new ProjectorRemote("  ", "Không mã", 2022, 100_000m));
        ExpectArgumentError("Tạo thiết bị với giá mua = 0",
            () => _ = new ProjectorRemote("PJ99", "Giá 0", 2022, 0m));
        ExpectArgumentError("Tạo thiết bị với năm sử dụng ở tương lai",
            () => _ = new ProjectorRemote("PJ98", "Năm tương lai", DateTime.Now.Year + 1, 100_000m));

        Console.WriteLine($"Xóa MIC02 khỏi phòng: {room.RemoveDevice("MIC02")}; xóa lần nữa: {room.RemoveDevice("MIC02")}");
    }

    // ------------------------------------------------------------------
    // 2. Luồng thành công (mục 11.1)
    // ------------------------------------------------------------------
    private static void TestSuccessFlow()
    {
        PrintTitle("B. Luồng thành công");

        // 1. Tạo giảng viên
        var lecturer = new Lecturer("GV001", "Nguyễn Văn An");

        // 2. Tạo 3 thiết bị
        var projector = new ProjectorRemote("PJ01", "Điều khiển máy chiếu Epson", 2021, 220_000m);
        var acRemote = new AirConditionerRemote("AC01", "Điều khiển điều hòa Daikin", 2022, 150_000m);
        var mic = new WirelessMicrophone("MIC01", "Micro không dây Shure", 2023, 1_800_000m);

        // 3. Tạo phiếu mượn cho một phòng dạy
        var slip1 = new BorrowingSlip("PM001", lecturer, "P-301", SlipDate);
        var slip2 = new BorrowingSlip("PM002", lecturer, "P-302", SlipDate);

        try
        {
            // 4. Thêm điều khiển máy chiếu và micro vào phiếu
            slip1.AddDevice(projector, 2, 1, 3);   // mượn 2 ngày, tiết 1-3
            slip1.AddDevice(mic, 1, 1, 2);         // mượn 1 ngày, tiết 1-2
            // Phiếu thứ hai mượn điều khiển điều hòa và để chưa trả, phục vụ bước 7.
            slip2.AddDevice(acRemote, 3, 4, 6);

            Console.WriteLine("Đã thêm thiết bị vào phiếu:");
            PrintSlipDetail(slip1);
            PrintSlipDetail(slip2);

            // 5. Trả điều khiển máy chiếu đúng hạn (hạn 14/10)
            decimal fee1 = slip1.ReturnDevice("PJ01", new DateTime(2026, 10, 14));
            Console.WriteLine($"Trả PJ01 đúng hạn 14/10 -> phí trả muộn: {fee1:N0} VND");

            // 6. Trả micro muộn 2 ngày (hạn 13/10, trả 15/10)
            decimal fee2 = slip1.ReturnDevice("MIC01", new DateTime(2026, 10, 15));
            Console.WriteLine($"Trả MIC01 ngày 15/10 (hạn 13/10) -> phí trả muộn: {fee2:N0} VND");

            Console.WriteLine($"Tổng phí trả muộn phiếu {slip1.SlipId}: {slip1.CalculateTotalLateFee():N0} VND");
        }
        catch (BorrowingException ex)
        {
            Console.WriteLine($"[LỖI NGHIỆP VỤ] {ex.GetType().Name}: {ex.Message}");
        }

        // 7. Hiển thị các phiếu trả muộn, chưa trả
        var slips = new List<BorrowingSlip> { slip1, slip2 };

        Console.WriteLine();
        Console.WriteLine("Các phiếu có thiết bị trả muộn:");
        foreach (var s in slips.Where(s => s.HasLateItems()))
        {
            Console.WriteLine($"  {s}");
            foreach (var i in s.Items.Where(i => i.GetLateDays() > 0))
                Console.WriteLine($"    - {i.Device.DeviceId}: muộn {i.GetLateDays()} ngày, phí {i.CalculateLateFee():N0} VND");
        }

        Console.WriteLine("Các phiếu còn thiết bị chưa trả:");
        foreach (var s in slips.Where(s => s.HasUnreturnedItems()))
        {
            Console.WriteLine($"  {s}");
            foreach (var i in s.Items.Where(i => !i.IsReturned))
                Console.WriteLine($"    - {i.Device.DeviceId} (hạn trả {i.DueDate:dd/MM/yyyy}), trạng thái thiết bị: {i.Device.Status}");
        }

        Console.WriteLine($"Trạng thái sau khi chạy: PJ01={projector.Status}, MIC01={mic.Status}, AC01={acRemote.Status}");
    }

    // ------------------------------------------------------------------
    // 3. Luồng phát sinh ngoại lệ (mục 11.2)
    // ------------------------------------------------------------------
    private static void TestExceptionFlows()
    {
        PrintTitle("C. Luồng phát sinh ngoại lệ");

        // --- 1. Mượn điều khiển điều hòa đang UnderMaintenance
        {
            var ac = new AirConditionerRemote("AC01", "Điều khiển điều hòa Daikin", 2022, 150_000m);
            ac.SendToMaintenance();
            var slip = new BorrowingSlip("PM101", new Lecturer("GV001", "Nguyễn Văn An"), "P-301", SlipDate);

            Expect<DeviceUnavailableException>(
                "1. Mượn điều khiển điều hòa đang UnderMaintenance",
                () => slip.AddDevice(ac, 2),
                ex => Console.WriteLine($"     DeviceId={ex.DeviceId}, CurrentStatus={ex.CurrentStatus}"),
                () => Console.WriteLine($"     Kiểm tra nhất quán: AC01={ac.Status} (giữ nguyên), số item trong phiếu={slip.Items.Count}"));
        }

        // --- 2. Trợ giảng mượn thiết bị thứ ba
        {
            var ta = new TeachingAssistant("TG001", "Trần Thị Bình");
            var ac = new AirConditionerRemote("AC01", "Điều khiển điều hòa Daikin", 2022, 150_000m);
            var mic = new WirelessMicrophone("MIC01", "Micro không dây Shure", 2023, 1_800_000m);
            var pj = new ProjectorRemote("PJ01", "Điều khiển máy chiếu Epson", 2021, 220_000m);
            var slip = new BorrowingSlip("PM102", ta, "P-301", SlipDate);
            slip.AddDevice(ac, 2);
            slip.AddDevice(mic, 1);

            Expect<BorrowLimitExceededException>(
                "2. Trợ giảng mượn thiết bị thứ ba",
                () => slip.AddDevice(pj, 2),
                ex => Console.WriteLine($"     BorrowerId={ex.BorrowerId}, MaximumDevices={ex.MaximumDevices}"),
                () => Console.WriteLine($"     Kiểm tra nhất quán: PJ01={pj.Status} (vẫn Available), số item trong phiếu={slip.Items.Count}"));
        }

        // --- 3. Thêm cùng một micro hai lần
        {
            var mic = new WirelessMicrophone("MIC01", "Micro không dây Shure", 2023, 1_800_000m);
            var slip = new BorrowingSlip("PM103", new Lecturer("GV001", "Nguyễn Văn An"), "P-301", SlipDate);
            slip.AddDevice(mic, 1);

            Expect<DuplicateDeviceException>(
                "3. Thêm cùng một micro hai lần",
                () => slip.AddDevice(mic, 1),
                ex => Console.WriteLine($"     DeviceId={ex.DeviceId}"),
                () => Console.WriteLine($"     Kiểm tra nhất quán: MIC01={mic.Status}, số item trong phiếu={slip.Items.Count} (vẫn 1)"));
        }

        // --- 4. Mượn micro 3 ngày dù giới hạn là 1 ngày
        {
            var mic = new WirelessMicrophone("MIC01", "Micro không dây Shure", 2023, 1_800_000m);
            var slip = new BorrowingSlip("PM104", new Lecturer("GV001", "Nguyễn Văn An"), "P-301", SlipDate);

            Expect<InvalidBorrowPeriodException>(
                "4. Mượn micro 3 ngày dù giới hạn là 1 ngày",
                () => slip.AddDevice(mic, 3),
                ex => Console.WriteLine($"     DeviceId={ex.DeviceId}, RequestedDays={ex.RequestedDays}, MaximumDays={ex.MaximumDays}"),
                () => Console.WriteLine($"     Kiểm tra nhất quán: MIC01={mic.Status} (vẫn Available), số item trong phiếu={slip.Items.Count}"));
        }

        // --- 5. Trả một điều khiển máy chiếu không thuộc phiếu
        {
            var mic = new WirelessMicrophone("MIC01", "Micro không dây Shure", 2023, 1_800_000m);
            var slip = new BorrowingSlip("PM105", new Lecturer("GV001", "Nguyễn Văn An"), "P-301", SlipDate);
            slip.AddDevice(mic, 1);

            Expect<DeviceNotInSlipException>(
                "5. Trả điều khiển máy chiếu PJ01 không thuộc phiếu",
                () => slip.ReturnDevice("PJ01", SlipDate.AddDays(1)),
                ex => Console.WriteLine($"     DeviceId={ex.DeviceId}"),
                () => Console.WriteLine($"     Kiểm tra nhất quán: MIC01={mic.Status} (vẫn Borrowed), chưa trả={slip.Items.Count(i => !i.IsReturned)}"));
        }

        // --- 6. Trả cùng một thiết bị hai lần
        {
            var pj = new ProjectorRemote("PJ01", "Điều khiển máy chiếu Epson", 2021, 220_000m);
            var slip = new BorrowingSlip("PM106", new Lecturer("GV001", "Nguyễn Văn An"), "P-301", SlipDate);
            slip.AddDevice(pj, 2);
            slip.ReturnDevice("PJ01", SlipDate.AddDays(1));
            DateTime? firstReturn = slip.Items[0].ReturnedDate;

            Expect<DeviceAlreadyReturnedException>(
                "6. Trả cùng một thiết bị PJ01 hai lần",
                () => slip.ReturnDevice("PJ01", SlipDate.AddDays(2)),
                ex => Console.WriteLine($"     DeviceId={ex.DeviceId}"),
                () => Console.WriteLine($"     Kiểm tra nhất quán: PJ01={pj.Status}, ngày trả vẫn là {slip.Items[0].ReturnedDate:dd/MM/yyyy} (không bị ghi đè, lần đầu {firstReturn:dd/MM/yyyy})"));
        }

        // --- 7. Trả thiết bị với ngày trả trước ngày mượn
        {
            var pj = new ProjectorRemote("PJ01", "Điều khiển máy chiếu Epson", 2021, 220_000m);
            var slip = new BorrowingSlip("PM107", new Lecturer("GV001", "Nguyễn Văn An"), "P-301", SlipDate);
            slip.AddDevice(pj, 2);

            Expect<InvalidReturnDateException>(
                "7. Trả PJ01 với ngày trả 11/10 trước ngày mượn 12/10",
                () => slip.ReturnDevice("PJ01", SlipDate.AddDays(-1)),
                ex => Console.WriteLine($"     ReturnedDate={ex.ReturnedDate:dd/MM/yyyy}, BorrowedDate={ex.BorrowedDate:dd/MM/yyyy}"),
                () => Console.WriteLine($"     Kiểm tra nhất quán: PJ01={pj.Status} (vẫn Borrowed), IsReturned={slip.Items[0].IsReturned}"));
        }

        // --- 8. Thêm: tiết mượn vượt giới hạn (ngoại lệ bổ sung)
        {
            var mic = new WirelessMicrophone("MIC01", "Micro không dây Shure", 2023, 1_800_000m);
            var slip = new BorrowingSlip("PM108", new Lecturer("GV001", "Nguyễn Văn An"), "P-301", SlipDate);

            Expect<InvalidPeriodRangeException>(
                "8. (Bổ sung) Mượn micro từ tiết 1 đến tiết 8 trong khi tối đa 4 tiết",
                () => slip.AddDevice(mic, 1, 1, 8),
                ex => Console.WriteLine($"     DeviceId={ex.DeviceId}, FromPeriod={ex.FromPeriod}, ToPeriod={ex.ToPeriod}, MaximumPeriods={ex.MaximumPeriods}"),
                () => Console.WriteLine($"     Kiểm tra nhất quán: MIC01={mic.Status} (vẫn Available), số item trong phiếu={slip.Items.Count}"));
        }
    }

    // ------------------------------------------------------------------
    // Hàm hỗ trợ
    // ------------------------------------------------------------------

    /// <summary>
    /// Chạy một thao tác dự kiến sẽ ném ngoại lệ kiểu TEx.
    /// try: chạy thao tác; catch: thông báo lỗi; finally: in trạng thái để chứng minh dữ liệu nhất quán.
    /// </summary>
    private static void Expect<TEx>(string title, Action action, Action<TEx> showDetail, Action showState)
        where TEx : BorrowingException
    {
        Console.WriteLine($"--- {title}");
        try
        {
            action();
            Console.WriteLine("  [SAI] Thao tác thành công, đáng lẽ phải có ngoại lệ!");
        }
        catch (TEx ex)
        {
            Console.WriteLine($"  [OK] Bắt được {typeof(TEx).Name}: {ex.Message}");
            showDetail(ex);
        }
        catch (BorrowingException ex)
        {
            Console.WriteLine($"  [SAI] Ngoại lệ nghiệp vụ khác kiểu mong đợi: {ex.GetType().Name}: {ex.Message}");
        }
        finally
        {
            showState();
        }
    }

    /// <summary>Chạy thao tác dự kiến ném ArgumentException/InvalidOperationException (lỗi dữ liệu đầu vào).</summary>
    private static void ExpectArgumentError(string title, Action action)
    {
        try
        {
            action();
            Console.WriteLine($"  [SAI] {title}: không có ngoại lệ!");
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            // ArgumentNullException và ArgumentOutOfRangeException đều kế thừa ArgumentException.
            string firstLine = ex.Message.Split('\n')[0].Trim();
            Console.WriteLine($"  [OK] {title} -> {ex.GetType().Name}: {firstLine}");
        }
    }

    private static void PrintSlipDetail(BorrowingSlip slip)
    {
        Console.WriteLine($"  {slip}");
        foreach (var i in slip.Items)
            Console.WriteLine($"    - {i.Device.DeviceId} {i.Device.Name}: mượn {i.BorrowedDate:dd/MM}, hạn {i.DueDate:dd/MM}, tiết {i.FromPeriod}-{i.ToPeriod}, thiết bị: {i.Device.Status}");
    }

    private static void PrintTitle(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 72));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', 72));
    }
}

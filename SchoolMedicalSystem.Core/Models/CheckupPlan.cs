using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMedicalSystem.Core.Models
{
    public enum CheckupPlanStatus
    {
        Planned,    // Đã lên kế hoạch
        InProgress, // Đang tiến hành
        Completed   // Đã hoàn thành
    }

    public class CheckupPlan
    {
        public int Id { get; set; }
        public string PlanName { get; set; } // Ví dụ: "Khám sức khỏe định kỳ Lớp 5A - Tháng 7"
        public string ClassName { get; set; }    // Lớp được khám
        public DateTime CheckupDate { get; set; }  // Ngày dự kiến khám
        public CheckupPlanStatus Status { get; set; } // Sử dụng Enum ở trên
    }
}

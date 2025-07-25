using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMedicalSystem.Core.Models
{
    public class HealthRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int NurseId { get; set; }
        public int? CheckupPlanId { get; set; }
        public DateTime CheckupDate { get; set; }

        // --- CÁC TRƯỜNG DỮ LIỆU ĐÃ CẬP NHẬT ---
        public double Height { get; set; } // Chiều cao (cm)
        public double Weight { get; set; } // Cân nặng (kg)
        public string Vision { get; set; } // Thị lực
        public string DentalStatus { get; set; } // Tình trạng răng miệng

        // THAY ĐỔI Ở ĐÂY
        public string BloodPressure { get; set; } // Huyết áp (ví dụ: "110/70 mmHg")
        public int HeartRate { get; set; } // Nhịp tim (lần/phút)

        public string? NurseNotes { get; set; } // Ghi chú chung vẫn giữ lại
    }
}

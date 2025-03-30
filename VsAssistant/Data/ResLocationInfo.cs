using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsAssistant.Data
{
    internal class ResLocationInfo
    {
        public ResLocationStatus Status { get; set; } = ResLocationStatus.Free;
        public DateTime? NextAttack { get; set; }

        public ResLocationInfo()
        {
        }

        public ResLocationInfo(ResLocationStatus status, DateTime? nextAttack)
        {
            Status = status;
            NextAttack = nextAttack;
        }
    }
}

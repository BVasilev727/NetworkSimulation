using NetworkSimulation.Enums;
using NetworkSimulation.Modulation.QAMModulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation.Modulation
{
    public static class ModulatorFactory
    {
        public static Modulator GetModulator(GenType type)
        {
            if (type == GenType.G1) return new AmplitudeModulator(5000, 1.0, 0.5);
            else if (type == GenType.G2) return new FrequencyModulator();
            else if (type == GenType.G3) return new PSKModulator();
            else if (type == GenType.G4) return new QAMModulator(64);
            else if (type == GenType.G5) return new QAMModulator(256);

            throw new Exception("Unknown network standard");
        }
    }
}

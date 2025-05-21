using System;
using System.Collections.Generic;
using System.Linq;

namespace TurnistleControlGui
{
    public static class ModbusCommandLibrary
    {
        // Komut sözlüğü (CRC'siz)
        public static readonly Dictionary<string, byte[]> Commands = new Dictionary<string, byte[]>
        {
            { "MotorEnable",    new byte[] { 0x01, 0x06, 0x08, 0x0C, 0x00, 0x0B } },
            { "MotorDisable",   new byte[] { 0x01, 0x06, 0x08, 0x0C, 0x00, 0x0C } },
            { "ForwardOpen",    new byte[] { 0x01, 0x06, 0x08, 0x0C, 0x00, 0x02 } },
            { "ReverseOpen",    new byte[] { 0x01, 0x06, 0x08, 0x0C, 0x00, 0x03 } },
            { "Stop",           new byte[] { 0x01, 0x06, 0x08, 0x0C, 0x00, 0x04 } },
            { "CloseDoor",      new byte[] { 0x01, 0x06, 0x08, 0x0C, 0x00, 0x01 } },
            { "Start",          new byte[] { 0x01, 0x06, 0x08, 0x0C, 0x00, 0x05 } },
            { "Reset",          new byte[] { 0x01, 0x06, 0x08, 0x0C, 0x00, 0x07 } },
        };

        // Komuta CRC ekleyen yardımcı fonksiyon
        public static byte[] GetCommandWithCRC(string commandKey)
        {
            if (!Commands.ContainsKey(commandKey))
                throw new ArgumentException("Tanımsız komut: " + commandKey);

            var data = Commands[commandKey];
            return AppendCRC(data);
        }

        private static byte[] AppendCRC(byte[] data)
        {
            ushort crc = 0xFFFF;

            foreach (byte b in data)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x0001) != 0)
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    else
                        crc >>= 1;
                }
            }

            byte crcLo = (byte)(crc & 0xFF);
            byte crcHi = (byte)((crc >> 8) & 0xFF);

            return data.Concat(new byte[] { crcLo, crcHi }).ToArray();
        }
    }
}

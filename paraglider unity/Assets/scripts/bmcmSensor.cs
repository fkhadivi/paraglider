using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Devices
{
    public class BmcmSensor
    {
        public BmcmSensor(string _type)
        {
            type = _type;
        }

        public const int AD_CHA_TYPE_ANALOG_IN = 0x01000000;
        public const int AD_CHA_TYPE_ANALOG_OUT = 0x02000000;
        public const int AD_CHA_TYPE_DIGITAL_IO = 0x03000000;
        public const int AD_CHA_TYPE_SYNC = 0x05000000;
        public const int AD_CHA_TYPE_ROUTE = 0x06000000;
        public const int AD_CHA_TYPE_CAN = 0x07000000;
        public const int AD_CHA_TYPE_COUNTER = 0x08000000;
        public const int AD_CHA_TYPE_ANALOG_COUNTER = 0x09000000;
        const int NUM_ANALOG_INPUTS = 16;

        #region Variables
        private const string dllname = "libad4.dll";
        private Int32 adh = -1;
        private string type = "none";
        private bool hardwareConnected = false;
        private bool digitalOutIsUsed = false;
        private int channel = 1;
        private float[][] interpolBuffer;                   //!< Array holding last Values for interpolation, reducing noise FOR EACH LINE
        private int numInterpolation = 5;                  //!< How many values to be interpolated
        private bool enableCounterReset = false;
        #endregion

        #region DllImport
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 ad_open([MarshalAs(UnmanagedType.LPStr)]string _name);
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 ad_close(Int32 _name);
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 ad_set_line_direction(Int32 adh, int cha, Int32 mask);
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 ad_set_digital_line(Int32 adh, Int32 cha, Int32 line, UInt32 flag);
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 ad_get_digital_line(Int32 adh, Int32 cha, Int32 line, ref UInt32 flag);
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 ad_digital_in(Int32 adh, Int32 cha, ref UInt32 data);
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ad_discrete_in(int adh, int cha, int range, ref uint data);
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 ad_digital_out(Int32 adh, Int32 cha, UInt32 data);
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 ad_analog_in(Int32 adh, Int32 cha, Int32 range, ref float volt);
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 ad_analog_out(Int32 adh, Int32 cha, Int32 range, float volt);

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        private struct ad_par { }

        [DllImport("libad4.dll")]
        private static extern int ad_ioctl(int adh, uint ioc, ref ad_par par, int size);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        public static int errno
        {
            get { return (int)GetLastError(); }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct ad_counter_mode
        {
            public int cha;                 // counter channel
            public byte mode;               // counter mode
            public byte mux_a;              // a input mux setting
            public byte mux_b;              // b input mux setting
            public byte mux_rst;            // reset input mux setting

            public ushort flags;            // control flags

            public uint capt_a;             // capture registers
            public uint capt_b;

            public byte eact_a;             // event action on capt_a match
            public byte eact_b;             // event action on capt_b match
        }

        private const uint AD_SET_COUNTER_MODE = 0xb3800001;
        private const uint AD_GET_COUNTER_MODE = 0xb3800002;

        [DllImport("libad4.dll", EntryPoint = "ad_ioctl")]
        private static extern int ad_ioctl_counter_mode(int adh, uint ioc, ref ad_counter_mode par, int size);

        public static int ad_set_counter_mode(int adh, ref ad_counter_mode mode)
        {
            return ad_ioctl_counter_mode(adh, AD_SET_COUNTER_MODE, ref mode, Marshal.SizeOf(mode));
        }

        public static int ad_get_counter_mode(int adh, ref ad_counter_mode mode)
        {
            return ad_ioctl_counter_mode(adh, AD_GET_COUNTER_MODE, ref mode, Marshal.SizeOf(mode));
        }

        public const byte AD_CNT_COUNTER = 0;         // counter mode
        public const byte AD_CNT_UPDOWN = 1;         // up/down counter mode
        public const byte AD_CNT_QUAD_DECODER = 4;         // quadrature decoder
        public const byte AD_CNT_PULSE_TIME = 5;

        public const ushort AD_CNT_INV_A = 0x0001;   // invert A input
        public const ushort AD_CNT_INV_B = 0x0002;   // invert B input
        public const ushort AD_CNT_INV_RST = 0x0004;   // invert reset input
        public const ushort AD_CNT_ENABLE_RST = 0x0008;   // enable reset input
        #endregion

        #region PRIVATE

        private Int32 Open(string _type)
        {
            try
            {
                adh = ad_open(_type);
                // init interpolBuffer
                interpolBuffer = new float[NUM_ANALOG_INPUTS][];
                for (int i = 0; i < NUM_ANALOG_INPUTS; i++)
                    interpolBuffer[i] = new float[numInterpolation];
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);

            }
            return adh;
        }

        #endregion

        #region PUBLIC
        public bool Connected
        {
            get { return hardwareConnected; }
            set { hardwareConnected = value; }
        }

        public void Init()
        {
            Debug.Log("USB AD INIT");
            try
            {
                if (Open(type) >= 0)
                {
                    hardwareConnected = true;
                    Debug.Log(type + " is valid");
                }
                else {
                    hardwareConnected = false;
                    Debug.LogError("No hardware " + type + " found. Error-Nr: " + errno);

                }
            }
            catch (Exception ex)
            {
                Debug.LogError("bmcmSensor: " + ex.Message);
            }
        }

        public bool Reconnect()
        {
            Close();
            if (Open(type) >= 0)
            {
                hardwareConnected = true;
                Log.Message(type + " is reconnected.");
            }
            else { hardwareConnected = false; Log.Error("No hardware " + type + " found"); }

            return hardwareConnected;
        }

        /// <summary>
        /// Close the device
        /// </summary>
        public void Close()
        {
            if (adh != -1)
            {
                if (digitalOutIsUsed) SetAllDigitalOut(false);
                ad_close(adh);
            }
            hardwareConnected = false;
            Log.Message("The hardware " + type + " was turned OFF. ");
        }

        /// <summary>
        /// Set sensor type and open this device after another device was closed.
        /// </summary>
        /// <param name="_type"></param>
        public void SetTypeOfSensor(string _type)
        {
            try
            {
                Close();
                if (Open(_type) != -1)
                {
                    hardwareConnected = true;
                    Log.Message("The hardware " + _type + " was turned ON.");
                }
                else { Log.Error("No hardware " + type + " found"); }
            }
            catch (Exception ex) { Log.Error(ex.Message); };
        }

        /// <summary>
        /// Set width of interpolation
        /// </summary>
        /// <param name="_num"></param>
        public void SetInterpolationWidth(int _num)
        {
            numInterpolation = _num;
            for (int i = 0; i < NUM_ANALOG_INPUTS; i++)
                interpolBuffer[i] = new float[numInterpolation];
        }

        /// <summary>
        /// Fill interpolation with current values
        /// </summary>
        /// <param name="_ch">Channel</param>
        /// <param name="_line">Line</param>
        public void FillInterpolationWithCurrentValues(int _ch, int _line)
        {
            float volt = 0;
            UInt32 ad = ad_analog_in(adh, AD_CHA_TYPE_ANALOG_IN | (_line + 1), 33, ref volt);

            // fill array
            for (int i = 0; i < (interpolBuffer[_line].Length - 1); i++)
            {
                interpolBuffer[_line][i] = volt;
            }
        }

        /// <summary>
        /// Check whether the device is connected.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return hardwareConnected;
        }

        public void SetPortDirection(int port, string direction)
        {
            if (direction.ToLower() == "input")
            {
                ad_set_line_direction(adh, port, 0xFF);
            }
            else
            {
                ad_set_line_direction(adh, port, 0x00);
            }
        }
        /// <summary>
        /// Setting Line 1 or 2 in the input lines to be a counter
        /// </summary>
        /// <param name="mode">
        /// AD_CNT_COUNTER = 0;     
        /// AD_CNT_UPDOWN = 1;      
        /// AD_CNT_QUAD_DECODER = 4;
        /// AD_CNT_PULSE_TIME = 5;
        /// </param>
        /// <param name="enable_rst"></param>
        public void SetCounter(int mode, bool enable_rst, int line)
        {
            // Set counter mode 
            enableCounterReset = enable_rst;

            int rc;
            if (mode >= 0)
            {
                ad_counter_mode par;
                /* Setup counter mode, using Port A/1 and Port A/2 as
                 * both counter inputs. The counter's reset input
                 * gets connected to Port A/3 (if enabled).
                 */
                par = new ad_counter_mode();
                par.cha = AD_CHA_TYPE_COUNTER | line;
                par.mode = (byte)(mode & 0xff);
                par.mux_a = 0;
                par.mux_b = 1;
                if (enable_rst)
                {
                    par.mux_rst = 2;
                    par.flags |= AD_CNT_ENABLE_RST;
                }
                rc = ad_set_counter_mode(adh, ref par);
                if (rc != 0)
                {
                    Log.Error("error: failed to setup BMCM counter. Error = " + rc);
                    return;
                }
            }
        }

        #region DIGITAL

        /// <summary>
        /// It returns a boolean of a selected line from a specified channel. Output: False = Line is turned OFF, True = Line is turned ON.
        /// </summary>
        /// <param name="_channel">Channel</param>
        /// <param name="_line">Line</param>
        /// <returns></returns>
        public bool GetDigitalLine(int _channel, int _line)
        {
            UInt32 data = 0;
            ad_set_line_direction(adh, _channel, 0xFFFF);
            UInt32 ad = ad_get_digital_line(adh, _channel, _line, ref data);
            if (data == 1)
                return true;
            else return false;
        }

        /// <summary>
        ///  Set a boolean to switch a specified line of a gaven channel from input-device.
        /// </summary>
        /// <param name="_channel">min. 1 and max. 3</param>
        /// <param name="_line">min. 0 and max. 7 or 15 for usb-oi16</param>
        /// <param name="_on">true or false</param>
        public void SetDigitalLine(int _channel, int _line, bool _on)
        {
            digitalOutIsUsed = true;
            UInt32 on = 0;
            if (_on) on = 1;

            ad_set_line_direction(adh, _channel, 0x0000);

            if (ad_set_digital_line(adh, _channel, _line, on) < 0)
            {
                Log.Error("SetDigitalLine() failed, because sensor is not connected, Try to reconnect");
                Reconnect();
                System.Threading.Thread.Sleep(1000);
                SetDigitalLine(_channel, _line, _on);
            }

        }

        /// <summary>
        /// Return binary value for whole port (e.g. 2 if second line is high) from input-device.
        /// </summary>
        /// <param name="_cha">Channel</param>
        /// <returns></returns>
        public int GetDigitalPort(int _cha)
        {
            UInt32 data = 0;
            UInt32 ad = ad_digital_in(adh, _cha, ref data);
            return (int)data;
        }

        /// <summary>
        /// Set a Integer that represents bit pattern to switch off/on  Lines of a specified digital channel
        /// </summary>
        /// <param name="_cha">min. 1 and max. 3</param>
        /// <param name="on">true or false</param>
        public void SetDigitalPortBin(int _cha, int _value)
        {
            Int32 ad = ad_digital_out(adh, _cha, (UInt32)_value);
        }

        /// <summary>
        /// Set a boolean to switch off/on all Lines of a specified digital channel from output device.
        /// </summary>
        /// <param name="_cha">min. 1 and max. 3</param>
        /// <param name="on">true or false</param>
        public void SetDigitalPort(int _cha, bool on)
        {
            digitalOutIsUsed = true;
            UInt32 value = 0x0000;
            if (on) value = 0xFFFF;
            Int32 ad = ad_digital_out(adh, _cha, value);
        }

        /// <summary>
        /// (Obsolete) See <see cref="SetDigitalPort(int, bool)"/>
        /// </summary>
        /// <param name="_cha"></param>
        /// <param name="on"></param>
        public void SetDigitalOut(int _cha, bool on)
        {
            SetDigitalPort(_cha, on);
        }

        /// <summary>
        /// Set a boolean to switch all digital channels at same time
        /// </summary>
        /// <param name="on"></param>
        public void SetAllDigitalOut(bool on)
        {
            SetDigitalPort(1, on);
            SetDigitalPort(2, on);
            SetDigitalPort(3, on);
        }

        public long GetCount(int line)
        {
            uint data = 0;
            ad_discrete_in(adh, AD_CHA_TYPE_COUNTER | line, 0, ref data);
            if (enableCounterReset)
            {
                if (data > Int32.MaxValue)
                    return (int)data + (0x80000000); // 0x80000000 is the counter's middle
                else
                    return (int)data - (0x80000000);
            }
            else
                return data;
        }

        #endregion

        #region ANALOG

        /// <summary>
        /// Get current voltage from analog device.
        /// </summary>
        /// <param name="_ch"></param>
        /// <param name="_line"></param>
        /// <returns></returns>
        public float GetAnalogIn(int _ch, int _line)
        {
            float volt = 0;
            UInt32 ad = ad_analog_in(adh, AD_CHA_TYPE_ANALOG_IN | (_line + 1), 33, ref volt);
            return volt;
        }

        /// <summary>
        /// Get current interpolated value from analog device. (default: Interpol Size = 10)
        /// </summary>
        /// <param name="_ch"></param>
        /// <param name="_line"></param>
        /// <returns></returns>
        public float GetAnalogInInterpolated(int _ch, int _line)
        {
            float volt = 0;
            UInt32 ad = ad_analog_in(adh, AD_CHA_TYPE_ANALOG_IN | (_line + 1), 33, ref volt);

            // shift array
            for (int i = interpolBuffer[_line].Length - 1; i > 0; i--)
            {
                interpolBuffer[_line][i] = interpolBuffer[_line][i - 1];
            }
            interpolBuffer[_line][0] = volt;

            // calculate average
            float interpolated = 0;
            for (int i = 0; i < interpolBuffer[_line].Length; i++)
            {
                interpolated += interpolBuffer[_line][i];
            }

            interpolated = interpolated / interpolBuffer[_line].Length;
#if DEBUG
            Log.Debug("Line: " + _line + ". Raw value: " + volt + ". Interpolated value: " + interpolated + ". INterpol Size: " + interpolBuffer[_line].Length);
#endif

            return interpolated;
        }

        /// <summary>
        /// (Obsolete) See <see cref="GetAnalogIn(int, int)"/> or <see cref="GetAnalogInInterpolated(int, int)"/>
        /// </summary>
        /// <param name="_line"></param>
        /// <returns></returns>
        public float GetRawValueFromAnalog(int _line = 0)
        {
            return GetAnalogIn(channel, _line);
        }

        /// <summary>
        /// Output the measured value direct as voltage of analog device
        /// </summary>
        /// <param name="_line"></param>
        /// <param name="_volt"></param>
        public void SetAnalogOut(int _line, float _volt)
        {
            Int32 a = ad_analog_out(adh, AD_CHA_TYPE_ANALOG_OUT | (_line + 1), 0, _volt);
        }


        #endregion

        #endregion
    }
}


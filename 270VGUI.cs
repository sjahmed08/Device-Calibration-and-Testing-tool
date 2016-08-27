using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Peak.Can.Basic;
using TPCANHandle = System.Byte;
using System.Timers;

namespace _270VGUI
{
    public partial class Form1 : Form
    {
        public static TPCANMsg CANMsgRead; // can message read
        public static TPCANMsg CANMsgWrite; // can message write
        public static TPCANMsg CANMsgWriteOnly;
        public static TPCANTimestamp TPCANTimestamp;
        public static TPCANHandle m_PcanHandle = 81;
        public static TPCANBaudrate m_Baudrate = TPCANBaudrate.PCAN_BAUD_1M;

        public static TPCANType m_HwType = TPCANType.PCAN_TYPE_ISA;
        public static uint ID1 = 418377642;
        public static uint ID2 = 418377643;

        private static System.Timers.Timer boardVoltageTimer;
        private static System.Timers.Timer statusUpdateTimer;
        private static System.Timers.Timer chnlSwitchTimer;

        
        public double[] inComingBrdData = new double[2];
        public double[,] inComingChnlData = new double[8, 9];

        public static byte chnlState = 0;
        public static byte groupState = 0;
        public static byte groupModeEnabled = 0x00;

        public static byte[] chnlArray = {0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70};
        public static byte[] groupArray = {0x01, 0x11, 0x21, 0x41};
        bool mainT_mode = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Initialize_Click(object sender, EventArgs e)
        {
            TPCANStatus stsResult;
            PCANBasic.Uninitialize(m_PcanHandle);
           
            stsResult = PCANBasic.Initialize(m_PcanHandle, m_Baudrate, m_HwType, Convert.ToUInt32(0x04),
            Convert.ToUInt16(3));

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                MessageBox.Show(Convert.ToString(stsResult));
            }
            else
            {
                MessageBox.Show("Connection Successful!");
                boardVoltageTimer = new System.Timers.Timer(350);
                boardVoltageTimer.Elapsed += boardVoltage_thread;
                boardVoltageTimer.Enabled = true;

                statusUpdateTimer = new System.Timers.Timer(100);
                statusUpdateTimer.Elapsed += statusUpdate_;
                statusUpdateTimer.Enabled = true;

                chnlSwitchTimer = new System.Timers.Timer(100);
                chnlSwitchTimer.Elapsed += chnlSwitchThread;
                chnlSwitchTimer.Enabled = true;

                toggleSwitch1.Enabled = false; ;
                porSWITCH.Enabled = false;
                crntRatingTBar.Enabled = false;
                Initialize.Enabled = false;
            }            
        }

        private void chnlSwitchThread(Object source, ElapsedEventArgs e)
        {
                if (chnlState == 0)
                {
                    Chnl0.FlatAppearance.BorderColor = Color.Orange;
                }
                else
                {
                    Chnl0.FlatAppearance.BorderColor = Color.Black;
                }

                if (chnlState == 1)
                {
                    Chnl1.FlatAppearance.BorderColor = Color.Orange;
                }
                else
                {
                    Chnl1.FlatAppearance.BorderColor = Color.Black;
                }

                if (chnlState == 2)
                {
                    Chnl2.FlatAppearance.BorderColor = Color.Orange;
                }
                else
                {
                    Chnl2.FlatAppearance.BorderColor = Color.Black;
                }

                if (chnlState == 3)
                {
                    Chnl3.FlatAppearance.BorderColor = Color.Orange;
                }
                else
                {
                    Chnl3.FlatAppearance.BorderColor = Color.Black;
                }

                if (chnlState == 4)
                {
                    Chnl4.FlatAppearance.BorderColor = Color.Orange;
                }
                else
                {
                    Chnl4.FlatAppearance.BorderColor = Color.Black;
                }

                if (chnlState == 5)
                {
                    Chnl5.FlatAppearance.BorderColor = Color.Orange;
                }
                else
                {
                    Chnl5.FlatAppearance.BorderColor = Color.Black;
                }

                if (chnlState == 6)
                {
                    Chnl6.FlatAppearance.BorderColor = Color.Orange;
                }
                else
                {
                    Chnl6.FlatAppearance.BorderColor = Color.Black;
                }

                if (chnlState == 7)
                {
                    Chnl7.FlatAppearance.BorderColor = Color.Orange;
                }
                else
                {
                    Chnl7.FlatAppearance.BorderColor = Color.Black;
                }
                if (groupState == 0)
                {
                    group1.FlatAppearance.BorderColor = Color.Blue;
                }
                else
                {
                    group1.FlatAppearance.BorderColor = Color.Black;
                }

                if (groupState == 1)
                {
                    group2.FlatAppearance.BorderColor = Color.Blue;
                }
                else
                {
                    group2.FlatAppearance.BorderColor = Color.Black;
                }

                if (groupState == 2)
                {
                    group3.FlatAppearance.BorderColor = Color.Blue;
                }
                else
                {
                    group3.FlatAppearance.BorderColor = Color.Black;
                }

                if (groupState == 3)
                {
                    group4.FlatAppearance.BorderColor = Color.Blue;
                }
                else
                {
                    group4.FlatAppearance.BorderColor = Color.Black;
                }





        }

        private void statusUpdate_(Object source, ElapsedEventArgs e)
        {
            statusUpdate_thread(inComingBrdData, inComingChnlData);
        }

        delegate void statusUpdateDelegate(double[] inComingBrdData, double[,] inComingChnlData);

        private void statusUpdate_thread(double[] inComingBrdData, double[,] inComingChnlData)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new statusUpdateDelegate(statusUpdate_thread), inComingBrdData, inComingChnlData);
                }
                catch (System.ObjectDisposedException)
                {
                }
            }
            else
            {
                
                channelVoltageNeedle.Value = Convert.ToSingle(inComingChnlData[chnlState, 1]);
                boardcurrentNeedle.Value = Convert.ToSingle(inComingChnlData[chnlState, 2] / 50);
                chnlCurntReading.Text = (inComingChnlData[chnlState, 2] / 50).ToString("0.0");

                digitalChannelGauge.Text = inComingChnlData[chnlState, 1].ToString("0.0");
                arcScaleNeedleComponent5.Value = Convert.ToSingle(inComingBrdData[1]);
                arcScaleNeedleComponent1.Value = Convert.ToSingle(inComingBrdData[0]);
                mainBoardcurrent.Text = inComingBrdData[1].ToString("0.0");
                digitalBrdVGauge.Text = inComingBrdData[0].ToString("0.0");
              
                if (inComingChnlData[chnlState, 3] != 0)
                {
                    OnOffIndicator.StateIndex = 1;
                  //  onOFFSwitch.IsOn = true;
                }
                else
                {
                    OnOffIndicator.StateIndex = 3;
                   // onOFFSwitch.IsOn = false;
                }

                if (inComingChnlData[chnlState, 5] != 0)
                {
                    bitIndicator.StateIndex = 1;
                }
                else
                {
                    bitIndicator.StateIndex = 3;
                }

                if (inComingChnlData[chnlState, 4] != 0)
                {
                    tripStatIndicator.StateIndex = 0;
                    OnOffIndicator.StateIndex = 2;
                }
                else
                {
                    tripStatIndicator.StateIndex = 3;
                }
                if (mainT_mode != true)
                {
                    if (inComingChnlData[chnlState, 7] != 0)
                    {
                        toggleSwitch1.IsOn = true;
                    }
                    else
                    {
                        toggleSwitch1.IsOn = false;
                    }

                    if (inComingChnlData[chnlState, 6] != 0)
                    {
                        porSWITCH.IsOn = true;
                    }
                    else
                    {
                        porSWITCH.IsOn = false;
                    }

                    crntRatingTBar.Value = Convert.ToUInt16(inComingChnlData[chnlState, 8]);
                }
            }

        }

        private void boardVoltage_thread(Object source, ElapsedEventArgs e)
        {
            TPCANStatus stsResult;
            CANMsgWrite = new TPCANMsg();
            CANMsgWrite.LEN = 8;
            CANMsgWrite.DATA = new byte[8];
            CANMsgWrite.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_EXTENDED;
            CANMsgWrite.ID = ID1;
            CANMsgWrite.DATA = new byte[8];

            CANMsgRead.ID = 0;
            CANMsgRead.LEN = 8;
            TPCANTimestamp.micros = 185;
            TPCANTimestamp.millis = 158903185;
            TPCANTimestamp.millis_overflow = 0;
            byte counTer = 0;

            byte[] writeMSG = new byte[8];
            byte[] dataRead = new byte[8];
            byte[] dataReadchnl = new byte[8];
            byte chnlCount = 0;
            byte groupCount = 0;
            bool skipProcess = false;
            int[] dataInPUT = new int[15];

            try
            {
                CANMsgWrite.ID = 418377632;
                CANMsgWrite.DATA[0] = 0x3D;
                CANMsgWrite.DATA[1] = 0x00;
                CANMsgWrite.DATA[2] = 0x00;
                CANMsgWrite.DATA[3] = 0x00;
                CANMsgWrite.DATA[4] = 0x00;
                CANMsgWrite.DATA[5] = 0x00;
                CANMsgWrite.DATA[6] = 0x00;
                CANMsgWrite.DATA[7] = 0x00;
            }
            catch (System.NullReferenceException)
            {
            }

            stsResult = PCANBasic.Write(m_PcanHandle, ref CANMsgWrite);

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
           //     MessageBox.Show(Convert.ToString(stsResult));
            }
            System.Threading.Thread.Sleep(5);
            while (CANMsgRead.ID != 418357487)
            {
                if (counTer > 5)
                {
                    counTer = 0;
                    skipProcess = true;
                    break;
                }
                stsResult = PCANBasic.Read(m_PcanHandle, out CANMsgRead, out TPCANTimestamp);
                counTer++;
            }

            if ((skipProcess == false) && (CANMsgRead.DATA[0] == 0x3E))
            {
                for (int i = 0; i < CANMsgRead.LEN; i++)
                {
                    dataInPUT[i] = CANMsgRead.DATA[i];
                }
                inComingBrdData[0] = (dataInPUT[2] + dataInPUT[3] * 256) / 10;
            }


            skipProcess = false;
            counTer = 0;


            CANMsgWrite.ID = 418377633;
            CANMsgWrite.DATA[0] = 0xA3;
            CANMsgWrite.DATA[1] = 0x00;
            CANMsgWrite.DATA[2] = 0x00;
            CANMsgWrite.DATA[3] = 0x00;
            CANMsgWrite.DATA[4] = 0x00;
            CANMsgWrite.DATA[5] = 0x00;
            CANMsgWrite.DATA[6] = 0x00;
            CANMsgWrite.DATA[7] = 0x00;

            stsResult = PCANBasic.Write(m_PcanHandle, ref CANMsgWrite);
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
        
            }

            System.Threading.Thread.Sleep(5);
            stsResult = PCANBasic.Read(m_PcanHandle, out CANMsgRead, out TPCANTimestamp);
            if ((CANMsgRead.DATA[0] == 0xA4) && (CANMsgRead.DATA[4] + CANMsgRead.DATA[5] < 255))
            {
                for (int i = 0; i < CANMsgRead.LEN; i++)
                {
                    dataInPUT[i] = CANMsgRead.DATA[i];
                }
                inComingBrdData[1] = ((dataInPUT[4] + (dataInPUT[5] * 256) + (dataInPUT[6] * 65536) + (dataInPUT[7] * 16777216)) / 100);
            }

            CANMsgWrite.ID = 418377635;
            CANMsgWrite.DATA[0] = 0x05;
            CANMsgWrite.DATA[1] = 0x00;
            CANMsgWrite.DATA[2] = 0x00;
            CANMsgWrite.DATA[3] = 0x00;
            CANMsgWrite.DATA[4] = 0x00;
            CANMsgWrite.DATA[5] = 0x00;
            CANMsgWrite.DATA[6] = 0x00;
            CANMsgWrite.DATA[7] = 0x00;

            stsResult = PCANBasic.Write(m_PcanHandle, ref CANMsgWrite);
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
         
            }

            System.Threading.Thread.Sleep(5);
            stsResult = PCANBasic.Read(m_PcanHandle, out CANMsgRead, out TPCANTimestamp);
            if (CANMsgRead.DATA[0] == 06)
            {
                for (int i = 0; i < CANMsgRead.LEN; i++)
                {
                    dataInPUT[i] = CANMsgRead.DATA[i];
                }
                inComingChnlData[0, 3] = dataInPUT[2] & 0x01;
                inComingChnlData[1, 3] = dataInPUT[2] & 0x02;
                inComingChnlData[2, 3] = dataInPUT[2] & 0x04;
                inComingChnlData[3, 3] = dataInPUT[2] & 0x08;
                inComingChnlData[4, 3] = dataInPUT[2] & 0x10;
                inComingChnlData[5, 3] = dataInPUT[2] & 0x20;
                inComingChnlData[6, 3] = dataInPUT[2] & 0x40;
                inComingChnlData[7, 3] = dataInPUT[2] & 0x80;
            }

            CANMsgWrite.DATA[0] = 0x0D;
            CANMsgWrite.DATA[1] = 0x00;
            CANMsgWrite.DATA[2] = 0x00;
            CANMsgWrite.DATA[3] = 0x00;
            CANMsgWrite.DATA[4] = 0x00;
            CANMsgWrite.DATA[5] = 0x00;
            CANMsgWrite.DATA[6] = 0x00;
            CANMsgWrite.DATA[7] = 0x00;
            stsResult = PCANBasic.Write(m_PcanHandle, ref CANMsgWrite);
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
        
            }

            System.Threading.Thread.Sleep(5);
            stsResult = PCANBasic.Read(m_PcanHandle, out CANMsgRead, out TPCANTimestamp);

            if (CANMsgRead.DATA[0] == 0x0E)
            {
                for (int i = 0; i < CANMsgRead.LEN; i++)
                {
                    dataInPUT[i] = CANMsgRead.DATA[i];
                }
                inComingChnlData[0, 4] = dataInPUT[2] & 0x01;
                inComingChnlData[1, 4] = dataInPUT[2] & 0x02;
                inComingChnlData[2, 4] = dataInPUT[2] & 0x04;
                inComingChnlData[3, 4] = dataInPUT[2] & 0x08;
                inComingChnlData[4, 4] = dataInPUT[2] & 0x10;
                inComingChnlData[5, 4] = dataInPUT[2] & 0x20;
                inComingChnlData[6, 4] = dataInPUT[2] & 0x40;
                inComingChnlData[7, 4] = dataInPUT[2] & 0x80;
            }

            CANMsgWrite.DATA[0] = 0x07;
            CANMsgWrite.DATA[1] = 0x00;
            CANMsgWrite.DATA[2] = 0x00;
            CANMsgWrite.DATA[3] = 0x00;
            CANMsgWrite.DATA[4] = 0x00;
            CANMsgWrite.DATA[5] = 0x00;
            CANMsgWrite.DATA[6] = 0x00;
            CANMsgWrite.DATA[7] = 0x00;

            stsResult = PCANBasic.Write(m_PcanHandle, ref CANMsgWrite);
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
          
            }

            System.Threading.Thread.Sleep(5);
            stsResult = PCANBasic.Read(m_PcanHandle, out CANMsgRead, out TPCANTimestamp);

            if (CANMsgRead.DATA[0] == 0x08)
            {
                for (int i = 0; i < CANMsgRead.LEN; i++)
                {
                    dataInPUT[i] = CANMsgRead.DATA[i];
                }
                inComingChnlData[0, 5] = dataInPUT[2] & 0x01;
                inComingChnlData[1, 5] = dataInPUT[2] & 0x02;
                inComingChnlData[2, 5] = dataInPUT[2] & 0x04;
                inComingChnlData[3, 5] = dataInPUT[2] & 0x08;
                inComingChnlData[4, 5] = dataInPUT[2] & 0x10;
                inComingChnlData[5, 5] = dataInPUT[2] & 0x20;
                inComingChnlData[6, 5] = dataInPUT[2] & 0x40;
                inComingChnlData[7, 5] = dataInPUT[2] & 0x80;
            }

            CANMsgWrite.DATA[0] = 0x49;
            CANMsgWrite.DATA[1] = 0x00;
            CANMsgWrite.DATA[2] = 0x00;
            CANMsgWrite.DATA[3] = 0x00;
            CANMsgWrite.DATA[4] = 0x00;
            CANMsgWrite.DATA[5] = 0x00;
            CANMsgWrite.DATA[6] = 0x00;
            CANMsgWrite.DATA[7] = 0x00;

            stsResult = PCANBasic.Write(m_PcanHandle, ref CANMsgWrite);
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
             
            }
            System.Threading.Thread.Sleep(5);
            stsResult = PCANBasic.Read(m_PcanHandle, out CANMsgRead, out TPCANTimestamp);

            if (CANMsgRead.DATA[0] == 0x4A)
            {
                for (int i = 0; i < CANMsgRead.LEN; i++)
                {
                    dataInPUT[i] = CANMsgRead.DATA[i];
                }

                inComingChnlData[0, 6] = dataInPUT[2] & 0x01;
                inComingChnlData[1, 6] = dataInPUT[2] & 0x02;
                inComingChnlData[2, 6] = dataInPUT[2] & 0x04;
                inComingChnlData[3, 6] = dataInPUT[2] & 0x08;
                inComingChnlData[4, 6] = dataInPUT[2] & 0x10;
                inComingChnlData[5, 6] = dataInPUT[2] & 0x20;
                inComingChnlData[6, 6] = dataInPUT[2] & 0x40;
                inComingChnlData[7, 6] = dataInPUT[2] & 0x80;
            }

            CANMsgWrite.DATA[0] = 0x43;
            CANMsgWrite.DATA[1] = 0x00;
            CANMsgWrite.DATA[2] = 0x00;
            CANMsgWrite.DATA[3] = 0x00;
            CANMsgWrite.DATA[4] = 0x00;
            CANMsgWrite.DATA[5] = 0x00;
            CANMsgWrite.DATA[6] = 0x00;
            CANMsgWrite.DATA[7] = 0x00;

            stsResult = PCANBasic.Write(m_PcanHandle, ref CANMsgWrite);
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
           
            }
            System.Threading.Thread.Sleep(5);
            stsResult = PCANBasic.Read(m_PcanHandle, out CANMsgRead, out TPCANTimestamp);

            if (CANMsgRead.DATA[0] == 0x44)
            {
                for (int i = 0; i < CANMsgRead.LEN; i++)
                {
                    dataInPUT[i] = CANMsgRead.DATA[i];
                }
                inComingChnlData[0, 7] = dataInPUT[2] & 0x01;
                inComingChnlData[1, 7] = dataInPUT[2] & 0x02;
                inComingChnlData[2, 7] = dataInPUT[2] & 0x04;
                inComingChnlData[3, 7] = dataInPUT[2] & 0x08;
                inComingChnlData[4, 7] = dataInPUT[2] & 0x10;
                inComingChnlData[5, 7] = dataInPUT[2] & 0x20;
                inComingChnlData[6, 7] = dataInPUT[2] & 0x40;
                inComingChnlData[7, 7] = dataInPUT[2] & 0x80;
            }


            while (chnlCount < 0x08)
            {
                if (groupModeSwitch.IsOn == true)
                {
                    try
                    {
                        CANMsgWrite.ID = 418377634;
                        CANMsgWrite.DATA[0] = 0x35;
                        CANMsgWrite.DATA[1] = groupArray[groupCount];
                        CANMsgWrite.DATA[2] = 0x00;
                        CANMsgWrite.DATA[3] = 0x00;
                        CANMsgWrite.DATA[4] = 0x00;
                        CANMsgWrite.DATA[5] = 0x00;
                        CANMsgWrite.DATA[6] = 0x00;
                        CANMsgWrite.DATA[7] = 0x00;
                    }
                    catch (System.NullReferenceException)
                    {
                    }
                }
                else
                {
                    try
                    {
                        CANMsgWrite.ID = 418377634;
                        CANMsgWrite.DATA[0] = 0x35;
                        CANMsgWrite.DATA[1] = chnlArray[chnlCount];
                        CANMsgWrite.DATA[2] = 0x00;
                        CANMsgWrite.DATA[3] = 0x00;
                        CANMsgWrite.DATA[4] = 0x00;
                        CANMsgWrite.DATA[5] = 0x00;
                        CANMsgWrite.DATA[6] = 0x00;
                        CANMsgWrite.DATA[7] = 0x00;
                    }
                    catch (System.NullReferenceException)
                    {
                    }
                }
                stsResult = PCANBasic.Write(m_PcanHandle, ref CANMsgWrite);
                if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                {
              
                }
                System.Threading.Thread.Sleep(20);
                stsResult = PCANBasic.Read(m_PcanHandle, out CANMsgRead, out TPCANTimestamp);

                if ((CANMsgRead.DATA[0] == 0x36) && ((CANMsgRead.DATA[1] == chnlArray[chnlCount]) || (CANMsgRead.DATA[1] == groupArray[groupCount])) && (CANMsgRead.DATA[2] + CANMsgRead.DATA[3] < 255))
                {
                    for (int i = 0; i < CANMsgRead.LEN; i++)
                    {
                        dataInPUT[i] = CANMsgRead.DATA[i];
                    }                   
                        inComingChnlData[chnlCount, 0] = chnlCount;
                        inComingChnlData[chnlCount, 1] = ((dataInPUT[2]) + ((dataInPUT[3]) * 256)) / 10;                  
                }

                if (groupModeSwitch.IsOn == true)
                {
                    try
                    {
                        CANMsgWrite.ID = 418377636;
                        CANMsgWrite.DATA[0] = 0x0F;
                        CANMsgWrite.DATA[1] = groupArray[groupCount];
                        CANMsgWrite.DATA[2] = 0x00;
                        CANMsgWrite.DATA[3] = 0x00;
                        CANMsgWrite.DATA[4] = 0x00;
                        CANMsgWrite.DATA[5] = 0x00;
                        CANMsgWrite.DATA[6] = 0x00;
                        CANMsgWrite.DATA[7] = 0x00;
                    }
                    catch (System.NullReferenceException)
                    {
                    }
                }
                else
                {
                    try
                    {
                        CANMsgWrite.ID = 418377636;
                        CANMsgWrite.DATA[0] = 0x0F;
                        CANMsgWrite.DATA[1] = chnlArray[chnlCount];
                        CANMsgWrite.DATA[2] = 0x00;
                        CANMsgWrite.DATA[3] = 0x00;
                        CANMsgWrite.DATA[4] = 0x00;
                        CANMsgWrite.DATA[5] = 0x00;
                        CANMsgWrite.DATA[6] = 0x00;
                        CANMsgWrite.DATA[7] = 0x00;
                    }
                    catch (System.NullReferenceException)
                    {
                    }
                }
                      
                    stsResult = PCANBasic.Write(m_PcanHandle, ref CANMsgWrite);
                    if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                    {
           
                    }

                    System.Threading.Thread.Sleep(20);
                    stsResult = PCANBasic.Read(m_PcanHandle, out CANMsgRead, out TPCANTimestamp);

                    if ((CANMsgRead.DATA[0] == 0x10) && ((CANMsgRead.DATA[1] == chnlArray[chnlCount]) || (CANMsgRead.DATA[1] == groupArray[groupCount])) && (CANMsgRead.DATA[2] + CANMsgRead.DATA[3] < 250))
                    {
                        for (int i = 0; i < CANMsgRead.LEN; i++)
                        {
                            dataInPUT[i] = CANMsgRead.DATA[i];
                        }
                        inComingChnlData[chnlCount, 0] = chnlCount;
                        inComingChnlData[chnlCount, 2] = (((dataInPUT[2]) + (dataInPUT[3])));
                    }
                    if (groupModeSwitch.IsOn == true)
                    {
                        try
                        {
                            CANMsgWrite.ID = 418377639;
                            CANMsgWrite.DATA[0] = 0x03;
                            CANMsgWrite.DATA[1] = groupArray[groupCount];
                            CANMsgWrite.DATA[2] = 0x00;
                            CANMsgWrite.DATA[3] = 0x00;
                            CANMsgWrite.DATA[4] = 0x00;
                            CANMsgWrite.DATA[5] = 0x00;
                            CANMsgWrite.DATA[6] = 0x00;
                            CANMsgWrite.DATA[7] = 0x00;
                        }
                        catch (System.NullReferenceException)
                        {
                        }                    
                    }
                    else
                    {
                        try
                        {
                            CANMsgWrite.ID = 418377639;
                            CANMsgWrite.DATA[0] = 0x03;
                            CANMsgWrite.DATA[1] = chnlArray[chnlCount];
                            CANMsgWrite.DATA[2] = 0x00;
                            CANMsgWrite.DATA[3] = 0x00;
                            CANMsgWrite.DATA[4] = 0x00;
                            CANMsgWrite.DATA[5] = 0x00;
                            CANMsgWrite.DATA[6] = 0x00;
                            CANMsgWrite.DATA[7] = 0x00;
                        }
                        catch (System.NullReferenceException)
                        {
                        }
                    }

                stsResult = PCANBasic.Write(m_PcanHandle, ref CANMsgWrite);
                if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                {
           
                }

                System.Threading.Thread.Sleep(10);
                stsResult = PCANBasic.Read(m_PcanHandle, out CANMsgRead, out TPCANTimestamp);
                if ((CANMsgRead.DATA[0] == 0x04) && (CANMsgRead.DATA[1] == chnlArray[chnlCount]))
                {
                    for (int i = 0; i < CANMsgRead.LEN; i++)
                    {
                        dataInPUT[i] = CANMsgRead.DATA[i];
                    }
                    inComingChnlData[chnlCount, 8] = ((dataInPUT[2] * (10 - 1) / 255) + 1);
                }
                if (groupModeSwitch.IsOn == true)
                {
                    groupCount++;
                    if (groupCount > 0x03)
                    {
                        groupCount = 0;
                    }
                }
                else
                {
                chnlCount++;
                }
            }
        }
      
        private void VoltageRead_Click(object sender, EventArgs e)
        {
            byte[] writeMSG = new byte[8];
            byte[] readData = new byte[8];

            writeMSG[0] = 0x3D;
            writeMSG[1] = 0x00;
            writeMSG[2] = 0x00;
            writeMSG[3] = 0x00;
            writeMSG[4] = 0x00;
            writeMSG[5] = 0x00;
            writeMSG[6] = 0x00;
            writeMSG[7] = 0x00;

            readData = SSPC_Ctrl(writeMSG, true);
        }

        private byte[] SSPC_Ctrl(byte[] msg, bool msginProg)
        {
            TPCANStatus stsResult;
            CANMsgWrite = new TPCANMsg();
            CANMsgWrite.LEN = 8;
            CANMsgWrite.DATA = new byte[8];
            CANMsgWrite.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_EXTENDED;
            CANMsgWrite.ID = ID1;
            CANMsgWrite.DATA = msg;
            TPCANTimestamp.micros = 185;
            TPCANTimestamp.millis = 158903185;
            TPCANTimestamp.millis_overflow = 0;
            
            byte[] inComingData = new byte[8];

            stsResult = PCANBasic.GetStatus(m_PcanHandle);


            stsResult = PCANBasic.Write(m_PcanHandle, ref CANMsgWrite);
         
                if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                {
                    MessageBox.Show(Convert.ToString(stsResult));
                }
                else
                {
                    System.Threading.Thread.Sleep(10);
                    stsResult = PCANBasic.Read(m_PcanHandle, out CANMsgRead, out TPCANTimestamp);
                    if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                    {
                        MessageBox.Show(Convert.ToString(stsResult));
                    }
                    else
                    {                                            
                       stsResult = PCANBasic.Read(m_PcanHandle, out CANMsgRead, out TPCANTimestamp);
                       
                        for (int i = 0; i < CANMsgRead.LEN; i++)
                            {
                                inComingData[i] = CANMsgRead.DATA[i];
                            }
                        }
                    }
            return inComingData;
        }
                           
        private void CAN_write_only(byte[] msg)
        {
            TPCANStatus stsResult;
            CANMsgWriteOnly = new TPCANMsg();
            CANMsgWriteOnly.LEN = 8;
            CANMsgWriteOnly.DATA = msg;
            CANMsgWriteOnly.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_EXTENDED;
            CANMsgWriteOnly.ID = 418377643;

            stsResult = PCANBasic.Write(m_PcanHandle, ref CANMsgWriteOnly);
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                MessageBox.Show(Convert.ToString(stsResult));
            }
        }

        private void Chnl0_Click(object sender, EventArgs e)
        {
            chnlState = 0x00;
            if (inComingChnlData[0, 3] == 0)

            {
                onOFFSwitch.IsOn = false;
            }
            else
            {
                onOFFSwitch.IsOn = true;
            }

            if (inComingChnlData[0, 6] == 0)
            {
                porSWITCH.IsOn = false;
            }
            else
            {
                porSWITCH.IsOn = true;
            }

            if (inComingChnlData[0, 7] == 0)
            {
                toggleSwitch1.IsOn = false;
            }
            else
            {
                toggleSwitch1.IsOn = true;
            }

            if (mainT_mode == true)
            {
                crntRatingTBar.Value = Convert.ToUInt16(inComingChnlData[chnlState, 8]);
            }
        }

        private void Chnl1_Click(object sender, EventArgs e)
        {
            chnlState = 0x01;
            if (inComingChnlData[1, 3] == 0)
            {
                onOFFSwitch.IsOn = false;
            }
            else
            {
                onOFFSwitch.IsOn = true;
            }

            if (inComingChnlData[1, 6] == 0)
            {
                porSWITCH.IsOn = false;
            }
            else
            {
                porSWITCH.IsOn = true;
            }

            if (inComingChnlData[1, 7] == 0)
            {
                toggleSwitch1.IsOn = false;
            }
            else
            {
                toggleSwitch1.IsOn = true;
            }

            if (mainT_mode == true)
            {
                crntRatingTBar.Value = Convert.ToUInt16(inComingChnlData[chnlState, 8]);
            }
        }

        private void Chnl2_Click(object sender, EventArgs e)
        {
            chnlState = 0x02;
            if (inComingChnlData[2, 3] == 0)
            {
                onOFFSwitch.IsOn = false;
            }
            else
            {
                onOFFSwitch.IsOn = true;
            }

            if (inComingChnlData[2, 6] == 0)
            {
                porSWITCH.IsOn = false;
            }
            else
            {
                porSWITCH.IsOn = true;
            }

            if (inComingChnlData[2, 7] == 0)
            {
                toggleSwitch1.IsOn = false;
            }
            else
            {
                toggleSwitch1.IsOn = true;
            }

            if (mainT_mode == true)
            {
                crntRatingTBar.Value = Convert.ToUInt16(inComingChnlData[chnlState, 8]);
            }

        }

        private void Chnl3_Click(object sender, EventArgs e)
        {
            chnlState = 0x03;
            if (inComingChnlData[3, 3] == 0)
            {
                onOFFSwitch.IsOn = false;
            }
            else
            {
                onOFFSwitch.IsOn = true;
            }

            if (inComingChnlData[3, 6] == 0)
            {
                porSWITCH.IsOn = false;
            }
            else
            {
                porSWITCH.IsOn = true;
            }

            if (inComingChnlData[3, 7] == 0)
            {
                toggleSwitch1.IsOn = false;
            }
            else
            {
                toggleSwitch1.IsOn = true;
            }


            if (mainT_mode == true)
            {
                crntRatingTBar.Value = Convert.ToUInt16(inComingChnlData[chnlState, 8]);
            }

        }

        private void Chnl4_Click(object sender, EventArgs e)
        {
            chnlState = 0x04;
            if (inComingChnlData[4, 3] == 0)
            {
                onOFFSwitch.IsOn = false;
            }
            else
            {
                onOFFSwitch.IsOn = true;
            }

            if (inComingChnlData[4, 6] == 0)
            {
                porSWITCH.IsOn = false;
            }
            else
            {
                porSWITCH.IsOn = true;
            }

            if (inComingChnlData[4, 7] == 0)
            {
                toggleSwitch1.IsOn = false;
            }
            else
            {
                toggleSwitch1.IsOn = true;
            }
            
            if (mainT_mode == true)
            {
                crntRatingTBar.Value = Convert.ToUInt16(inComingChnlData[chnlState, 8]);
            }
        }

        private void Chnl5_Click(object sender, EventArgs e)
        {
            chnlState = 0x05;
            if (inComingChnlData[5, 3] == 0)
            {
                onOFFSwitch.IsOn = false;
            }
            else
            {
                onOFFSwitch.IsOn = true;
            }

            if (inComingChnlData[5, 6] == 0)
            {
                porSWITCH.IsOn = false;
            }
            else
            {
                porSWITCH.IsOn = true;
            }

            if (inComingChnlData[5, 7] == 0)
            {
                toggleSwitch1.IsOn = false;
            }
            else
            {
                toggleSwitch1.IsOn = true;
            }

            if (mainT_mode == true)
            {
                crntRatingTBar.Value = Convert.ToUInt16(inComingChnlData[chnlState, 8]);
            }
        }

        private void Chnl6_Click(object sender, EventArgs e)
        {
            chnlState = 0x06;
            if (inComingChnlData[6, 3] == 0)
            {
                onOFFSwitch.IsOn = false;
            }
            else
            {
                onOFFSwitch.IsOn = true;
            }

            if (inComingChnlData[6, 6] == 0)
            {
                porSWITCH.IsOn = false;
            }
            else
            {
                porSWITCH.IsOn = true;
            }

            if (inComingChnlData[6, 7] == 0)
            {
                toggleSwitch1.IsOn = false;
            }
            else
            {
                toggleSwitch1.IsOn = true;
            }

            if (mainT_mode == true)
            {
                crntRatingTBar.Value = Convert.ToUInt16(inComingChnlData[chnlState, 8]);
            }
        }

        private void Chnl7_Click(object sender, EventArgs e)
        {
            chnlState = 0x07;
            if (inComingChnlData[7, 3] == 0)
            {
                onOFFSwitch.IsOn = false;
            }
            else
            {
                onOFFSwitch.IsOn = true;
            }

            if (inComingChnlData[7, 6] == 0)
            {
                porSWITCH.IsOn = false;
            }
            else
            {
                porSWITCH.IsOn = true;
            }

            if (inComingChnlData[7, 7] == 0)
            {
                toggleSwitch1.IsOn = false;
            }
            else
            {
                toggleSwitch1.IsOn = true;
            }

            if (mainT_mode == true)
            {
                crntRatingTBar.Value = Convert.ToUInt16(inComingChnlData[chnlState, 8]);
            }
        }

        private void onOFFSwitch_Toggled(object sender, EventArgs e)
        {
            byte[] writeMSG = new byte[8];

            if (groupModeSwitch.IsOn == true)
            {
                if (onOFFSwitch.IsOn == true)
                {
                    writeMSG[0] = 0x01;
                    writeMSG[1] = groupArray[groupState];
                    writeMSG[2] = 0x01;
                    writeMSG[3] = 0x00;
                    writeMSG[4] = 0x00;
                    writeMSG[5] = 0x00;
                    writeMSG[6] = 0x00;
                    writeMSG[7] = 0x00;
                    CAN_write_only(writeMSG);
                }
                else
                {
                    writeMSG[0] = 0x01;
                    writeMSG[1] = groupArray[groupState];
                    writeMSG[2] = 0x00;
                    writeMSG[3] = 0x00;
                    writeMSG[4] = 0x00;
                    writeMSG[5] = 0x00;
                    writeMSG[6] = 0x00;
                    writeMSG[7] = 0x00;
                    CAN_write_only(writeMSG);
                }
            }
            else
            {
                if (onOFFSwitch.IsOn == true)
                {
                    writeMSG[0] = 0x01;
                    writeMSG[1] = chnlArray[chnlState];
                    writeMSG[2] = 0x01;
                    writeMSG[3] = 0x00;
                    writeMSG[4] = 0x00;
                    writeMSG[5] = 0x00;
                    writeMSG[6] = 0x00;
                    writeMSG[7] = 0x00;
                    CAN_write_only(writeMSG);
                }
                else
                {
                    writeMSG[0] = 0x01;
                    writeMSG[1] = chnlArray[chnlState];
                    writeMSG[2] = 0x00;
                    writeMSG[3] = 0x00;
                    writeMSG[4] = 0x00;
                    writeMSG[5] = 0x00;
                    writeMSG[6] = 0x00;
                    writeMSG[7] = 0x00;
                    CAN_write_only(writeMSG);
                }
            }
        }

        private void maintnc_Mode_Click(object sender, EventArgs e)
        {
            if (mainT_mode == false)
            {
                mainT_mode = true;
                maintnc_Mode.BackColor = Color.Green;
                toggleSwitch1.Enabled = true;
                porSWITCH.Enabled = true;
                crntRatingTBar.Enabled = true;
                byte[] writeMSG = new byte[8];
                writeMSG[0] = 0x25;
                writeMSG[1] = 0x01;
                writeMSG[2] = 0xDC;
                writeMSG[3] = 0x0D;
                writeMSG[4] = 0x00;
                writeMSG[5] = 0x00;
                writeMSG[6] = 0x00;
                writeMSG[7] = 0x00;
                CAN_write_only(writeMSG);
                Flash.Enabled = true;
                currentIndicatorGroup.Enabled = true;
            }
            else
            {
                maintnc_Mode.BackColor = Color.Gray;
                mainT_mode = false;
                toggleSwitch1.Enabled = false;
                porSWITCH.Enabled = false;
                crntRatingTBar.Enabled = false;
                byte[] writeMSG = new byte[8];
                writeMSG[0] = 0x25;
                writeMSG[1] = 0x00;
                writeMSG[2] = 0xDC;
                writeMSG[3] = 0x0D;
                writeMSG[4] = 0x00;
                writeMSG[5] = 0x00;
                writeMSG[6] = 0x00;
                writeMSG[7] = 0x00;
                CAN_write_only(writeMSG);
                Flash.Enabled = false;
                currentIndicatorGroup.Enabled = false;
            }          
        }

        private void Flash_Click(object sender, EventArgs e)
        {
            byte[] writeMSG = new byte[8];
            writeMSG[0] = 0x61;
            writeMSG[1] = 0x00;
            writeMSG[2] = 0x00;
            writeMSG[3] = 0x00;
            writeMSG[4] = 0xAB;
            writeMSG[5] = 0xAC;
            writeMSG[6] = 0x00;
            writeMSG[7] = 0x00;
            CAN_write_only(writeMSG);   
        }

        private void resEt_Click(object sender, EventArgs e)
        {
            boardVoltageTimer.Enabled = false;
            byte[] writeMSG = new byte[8];            
            writeMSG[0] = 0x1D;
            writeMSG[1] = 0x00;
            writeMSG[2] = 0x00;
            writeMSG[3] = 0x00;
            writeMSG[4] = 0x00;
            writeMSG[5] = 0x00;
            writeMSG[6] = 0x00;
            writeMSG[7] = 0x00;
            CAN_write_only(writeMSG);
            System.Threading.Thread.Sleep(20);
            boardVoltageTimer.Enabled = true;
        }

        private void toggleSwitch1_Toggled(object sender, EventArgs e)
        {
            byte[] writeMSG = new byte[8];

            if (groupModeSwitch.IsOn == true)
            {
                if (toggleSwitch1.IsOn == true)
                {
                    writeMSG[0] = 0x41;
                    writeMSG[1] = groupArray[groupState];
                    writeMSG[2] = 0x01;
                    writeMSG[3] = 0x00;
                    writeMSG[4] = 0x00;
                    writeMSG[5] = 0x00;
                    writeMSG[6] = 0x00;
                    writeMSG[7] = 0x00;
                    CAN_write_only(writeMSG);
                }
                else
                {
                    writeMSG[0] = 0x41;
                    writeMSG[1] = groupArray[groupState];
                    writeMSG[2] = 0x00;
                    writeMSG[3] = 0x00;
                    writeMSG[4] = 0x00;
                    writeMSG[5] = 0x00;
                    writeMSG[6] = 0x00;
                    writeMSG[7] = 0x00;
                    CAN_write_only(writeMSG);
                }
            }

            else
            {
                if (toggleSwitch1.IsOn == true)
                {
                    writeMSG[0] = 0x41;
                    writeMSG[1] = chnlArray[chnlState];
                    writeMSG[2] = 0x01;
                    writeMSG[3] = 0x00;
                    writeMSG[4] = 0x00;
                    writeMSG[5] = 0x00;
                    writeMSG[6] = 0x00;
                    writeMSG[7] = 0x00;
                    CAN_write_only(writeMSG);
                }
                else
                {
                    writeMSG[0] = 0x41;
                    writeMSG[1] = chnlArray[chnlState];
                    writeMSG[2] = 0x00;
                    writeMSG[3] = 0x00;
                    writeMSG[4] = 0x00;
                    writeMSG[5] = 0x00;
                    writeMSG[6] = 0x00;
                    writeMSG[7] = 0x00;
                    CAN_write_only(writeMSG);
                }
            }
        }

        private void porSWITCH_Toggled(object sender, EventArgs e)
        {
            byte[] writeMSG = new byte[8];

            if (groupModeSwitch.IsOn == true)
            {
                if (porSWITCH.IsOn == true)
                {
                    writeMSG[0] = 0x47;
                    writeMSG[1] = groupArray[groupState];
                    writeMSG[2] = 0x01;
                    writeMSG[3] = 0x00;
                    writeMSG[4] = 0x00;
                    writeMSG[5] = 0x00;
                    writeMSG[6] = 0x00;
                    writeMSG[7] = 0x00;
                    CAN_write_only(writeMSG);
                }
                else
                {
                    writeMSG[0] = 0x47;
                    writeMSG[1] = groupArray[groupState];
                    writeMSG[2] = 0x00;
                    writeMSG[3] = 0x00;
                    writeMSG[4] = 0x00;
                    writeMSG[5] = 0x00;
                    writeMSG[6] = 0x00;
                    writeMSG[7] = 0x00;
                    CAN_write_only(writeMSG);
                }
            }

            else
            {
                if (porSWITCH.IsOn == true)
                {
                    writeMSG[0] = 0x47;
                    writeMSG[1] = chnlArray[chnlState];
                    writeMSG[2] = 0x01;
                    writeMSG[3] = 0x00;
                    writeMSG[4] = 0x00;
                    writeMSG[5] = 0x00;
                    writeMSG[6] = 0x00;
                    writeMSG[7] = 0x00;
                    CAN_write_only(writeMSG);
                }
                else
                {
                    writeMSG[0] = 0x47;
                    writeMSG[1] = chnlArray[chnlState];
                    writeMSG[2] = 0x00;
                    writeMSG[3] = 0x00;
                    writeMSG[4] = 0x00;
                    writeMSG[5] = 0x00;
                    writeMSG[6] = 0x00;
                    writeMSG[7] = 0x00;
                    CAN_write_only(writeMSG);
                }
            }
        }

        private void crntRatingTBar_EditValueChanged(object sender, EventArgs e)
        {
            byte[] writeMSG = new byte[8];
            byte curntVal = Convert.ToByte(255*(crntRatingTBar.Value - 1) / (10 - 1));

            writeMSG[0] = 0x11;
            writeMSG[2] = curntVal;
            writeMSG[3] = 0x00;
            writeMSG[4] = 0x00;
            writeMSG[5] = 0x00;
            writeMSG[6] = 0x00;
            writeMSG[7] = 0x00;

            if (groupModeSwitch.IsOn == true)
            {
                writeMSG[1] = groupArray[groupState];
            }
            else
            {
                writeMSG[0] = 0x11;
                writeMSG[1] = chnlArray[chnlState];
                writeMSG[2] = curntVal;
                writeMSG[3] = 0x00;
                writeMSG[4] = 0x00;
                writeMSG[5] = 0x00;
                writeMSG[6] = 0x00;
                writeMSG[7] = 0x00;
            }
            CAN_write_only(writeMSG);           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Channel_Control_Enter(object sender, EventArgs e)
        {

        }

        private void group1_Click(object sender, EventArgs e)
        {
            groupState = 0x00;
        }

        private void group2_Click(object sender, EventArgs e)
        {
            groupState = 0x01;
        }

        private void group3_Click(object sender, EventArgs e)
        {
            groupState = 0x02;
        }

        private void group4_Click(object sender, EventArgs e)
        {
            groupState = 0x03;
        }

        private void chnl_0_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupCreate_Click(object sender, EventArgs e)
        {
            byte selectedChannels = 0x00;
            byte[] writeMSG = new byte[8];

            if (chnl_0.Checked == true)
            {
                selectedChannels =+ 0x01;
            }

            if (chnl_1.Checked == true)
            {
                selectedChannels =+ 0x02;
            }

            if (chnl_2.Checked == true)
            {
                selectedChannels =+ 0x04;
            }

            if (chnl_3.Checked == true)
            {
                selectedChannels =+ 0x08;
            }

            if (chnl_4.Checked == true)
            {
                selectedChannels =+  0x10;
            }

            if (chnl_5.Checked == true)
            {
                selectedChannels =+ 0x20;
            }

            if (chnl_6.Checked == true)
            {
                selectedChannels =+ 0x40;
            }

            if (chnl_7.Checked == true)
            {
                selectedChannels =+ 0x80;
            }

            writeMSG[0] = 0x37;
            writeMSG[1] = groupState;
            writeMSG[2] = selectedChannels;
            writeMSG[3] = 0x00;
            writeMSG[4] = 0x00;
            writeMSG[5] = 0x00;
            writeMSG[6] = 0x00;
            writeMSG[7] = 0x00;
            CAN_write_only(writeMSG);
        }

        private void groupModeSwitch_Toggled(object sender, EventArgs e)
        {
            if (groupModeSwitch.IsOn == false)
            {
                groupModeEnabled = 0x01;
            }
            else
            {
                groupModeEnabled = 0x00;
            }
        }
    }
}


    


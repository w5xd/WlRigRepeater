using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Soap;
using System.IO;

namespace RigRepeater
{
    /* FreqUpdateUdpListener
     * UDP listener class. Notifies delegate from "init" method on receipt of data
     */
    class FreqUpdateUdpListener
    {
        public delegate void OnFreqUpdated(EntryFrequencyUpdate e);

        OnFreqUpdated m_notify;
        public static int UDP_PORT = 
            int.Parse(System.Configuration.ConfigurationSettings.AppSettings["UDP_PORT"].ToString());

        UdpClient listener;
        IPEndPoint groupEP;

        public void init(OnFreqUpdated v)
        {
            listener = new UdpClient(UDP_PORT);
            m_notify = v;
            StartListener();
        }

        public void stop()
        {
            m_notify = null;
            listener.Close();
        }

        private void StartListener()
        {
            if (m_notify == null)
                return;
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, UDP_PORT);
            listener.BeginReceive(new AsyncCallback(ReceiveDone), null);
        }

        void ReceiveDone(IAsyncResult ar)
        {
            byte[] got;
            try
            {   // at Form.Close time, the last one sometimes fails
                got = listener.EndReceive(ar, ref groupEP);
            }
            catch
            {
                return;
            }
            OnFreqUpdated notify = m_notify;
            if (notify == null)
                return;
            using (MemoryStream stream = new MemoryStream(got))
            {
                SoapFormatter formatter = new SoapFormatter();
                for (;;)
                {
                    EntryFrequencyUpdate efu = null;
                    try
                    {
                        efu = formatter.Deserialize(stream) as EntryFrequencyUpdate;
                    }
                    catch { }
                    if (efu == null)
                        break;
                    notify(efu);
                }
            }
            StartListener();
        }
    }
}

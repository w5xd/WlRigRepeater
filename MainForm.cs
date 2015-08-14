using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Soap;

using WriteLogClrTypes;

/*
 * RigRepeater
 * 
 * Windows Forms application that publishes WriteLog's Entry Window
 * frequency state via UDP. 
 * It also listens for copies of itself on the local network(s).
 * It displays those rig states and, when you click the Link button,
 * forces local rigs to match the frequency state of remote rigs.
 */

namespace RigRepeater
{
    public partial class MainForm : Form
    {
        IWriteL m_wl;
        FreqUpdateUdpListener m_listener;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                Object o = Marshal.GetActiveObject("WriteLog.Document");
                m_wl = (IWriteL)o;
            }
            catch
            {
            }
            if (m_wl == null)
            {
                MessageBox.Show("WriteLog is not running");
                Close();
            }
            m_listener = new FreqUpdateUdpListener();
            m_listener.init(new FreqUpdateUdpListener.OnFreqUpdated( OnReceivedFreq));
            timer1.Enabled = true;
        }

        /* As we find Entry Window's locally (on WriteLog running on this machine)
        ** update the local ListBox */
        private void AddOrUpdateLocal(EntryFrequencyUpdate efu)
        {
            foreach (Object o in listBoxLocal.Items)
            {
                EntryFrequencyUpdate onScreen = o as EntryFrequencyUpdate;
                if (onScreen.SameRig(efu))
                {
                    bool same = onScreen.Equals(efu);
                    onScreen.UpdateFrom(efu);
                    if (!same)
                        listBoxLocal.Invalidate();
                    return;
                }
            }
            efu.ListIndex = listBoxLocal.Items.Add(efu);         
        }

        /* As we find Entry Window's remotely (via UDP)
        ** update the remote ListBox. Our UDP listener hears our own
        ** UDP sends. They are not filted out here, so they appear
        ** in both ListBox's. That is not really a problem.*/
        private void AddOrUpdateRemote(EntryFrequencyUpdate efu)
        {
            bool found = false;
            foreach (Object o in listBoxRemote.Items)
            {
                EntryFrequencyUpdate onScreen = o as EntryFrequencyUpdate;
                if (onScreen.SameRig(efu))
                {
                    found = true;
                    bool same = onScreen.Equals(efu);
                    onScreen.UpdateFrom(efu);
                    if (!same)
                        listBoxRemote.Invalidate();
                    break;
                }
            }
            if (!found)
                efu.ListIndex = listBoxRemote.Items.Add(efu);
            /* on hearing from a remote Entry Window rig,
            ** see if its linked, and, if so, update the local Entry Window */
            foreach (Object o in listBoxLocal.Items)
            {
                EntryFrequencyUpdate localEntry = o as EntryFrequencyUpdate;
                EntryFrequencyUpdate linked = localEntry.LinkUpdatesFrom;
                if (linked != null)
                {
                    if (linked.SameRig(efu) && localEntry.EntryWindow != null)
                    {
                        /* The SameFrequency/UpdateFrom routine is an optimization
                        ** that prevents multiple quick updates in the case
                        ** where the rig takes time to update */
                        if (!localEntry.SameFrequency(efu))
                        {
                            localEntry.EntryWindow.SetLogFrequencyEx(efu.Mode, efu.RxFreq, efu.TxFreq, efu.Split);
                            localEntry.UpdateFrom(efu);
                            listBoxLocal.Invalidate();
                        }
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            List<EntryFrequencyUpdate> toSend = new List<EntryFrequencyUpdate>();
            try
            {
                // iterate through the local Entry Window's
                short netLetter = m_wl.NetLetter;
                for (short i = 0; ; i++)
                {
                    ISingleEntry e1 = m_wl.GetEntry(i) as ISingleEntry;
                    if (e1 == null)
                        break;
                    short lr = e1.GetLeftRight();
                    short mode = 0;
                    double rx = 0;
                    double tx = 0;
                    short split = 0;
                    e1.GetLogFrequency(ref mode, ref rx, ref tx, ref split);
                    EntryFrequencyUpdate efu = new EntryFrequencyUpdate(netLetter, lr, tx, rx, split, mode);
                    efu.EntryWindow = e1;
                    AddOrUpdateLocal(efu);
                    toSend.Add(efu);
                }

            }
            catch
            {
                timer1.Enabled = false;
                MessageBox.Show("Failed to talk to WriteLog");
                Close();
            }
            if (toSend.Any())
            {
                // Send a UDP update (one message) for all our active Entry Window rigs.
                SoapFormatter formatter = new SoapFormatter();
                byte[] bytes;
                using (MemoryStream stream = new MemoryStream())
                {
                    foreach (EntryFrequencyUpdate efu in toSend)
                    {
                        formatter.Serialize(stream, efu);
                    }
                    bytes = stream.ToArray();
                }

                // want to broadcast, but can't just send to "255.255.255.255" cuz that goes everywhere on the internet.
                // Instead, iterate through all the interfaces on this PC and broadcast on each of them.
                // Only works for interfaces configured for IPv4...
                System.Net.NetworkInformation.NetworkInterface[]
                    locals = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                foreach (System.Net.NetworkInformation.NetworkInterface iface in locals)
                {
                    if (iface.NetworkInterfaceType ==
                            System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
                        continue;
                    if (iface.OperationalStatus !=
                        System.Net.NetworkInformation.OperationalStatus.Up)
                        continue;
                    System.Net.NetworkInformation.UnicastIPAddressInformationCollection col =
                        iface.GetIPProperties().UnicastAddresses;
                    foreach (System.Net.NetworkInformation.UnicastIPAddressInformation ad in col)
                    {
                        /* This calculation if IPv4 specific....   */
                        System.Net.IPAddress broadcast = ad.Address;
                        byte[] addr = broadcast.GetAddressBytes();
                        byte[] mask = ad.IPv4Mask.GetAddressBytes();

                        if (addr.Length != mask.Length)
                            continue;

                        uint total = 0;
                        for (int k = 0; k < addr.Length; k++)
                        {
                            total += addr[k];
                            addr[k] |= (byte)~mask[k];
                        }

                        if (total == 0)
                            continue;

                        IPAddress bc2 = new System.Net.IPAddress(addr);
                        IPEndPoint ep = new IPEndPoint(bc2, FreqUpdateUdpListener.UDP_PORT);
                        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                                 ProtocolType.Udp);
                        s.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket,
                            System.Net.Sockets.SocketOptionName.Broadcast, 1);
                        s.SendTo(bytes, ep);
                    }
                }
            }
        }

        delegate void InvokeDelegate(EntryFrequencyUpdate efu);
        // OnReceivedFreq is called from the FreqUpdateUdpListener threaed
        private void OnReceivedFreq(EntryFrequencyUpdate efu)
        {
            // transfer the notification to the Form's thread (Main)
            InvokeDelegate id = new InvokeDelegate(AddOrUpdateRemote);
            BeginInvoke(id, efu);
        }

        // We ownerdraw the listbox items in order to just update the
        // underlying .Items members. listBox seems to cache the ToString's if
        // we don't do ownerdraw
        private void listBoxLocal_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if ((e.Index >= 0) && (e.Index < listBoxLocal.Items.Count))
            {
                EntryFrequencyUpdate efu = listBoxLocal.Items[e.Index] as EntryFrequencyUpdate;
                e.Graphics.DrawString(efu.ToString(),
                    e.Font, 
                    efu.LinkUpdatesFrom == null 
                        ? new SolidBrush(e.ForeColor) :
                           new SolidBrush(Color.Red),  // Linked Entry windows are drawn Red.
                    e.Bounds);
            }
            e.DrawFocusRectangle();
        }

        private void listBoxRemote_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if ((e.Index >= 0) && (e.Index < listBoxRemote.Items.Count))
            {
                e.Graphics.DrawString(listBoxRemote.Items[e.Index].ToString(),
                    e.Font,
                    new SolidBrush(e.ForeColor),
                    e.Bounds);
            }
            e.DrawFocusRectangle();
        }

        // update the Link and Unlink button Enabled properties based on selections
        private void ListSelectionChanged()
        {
            int localSel = listBoxLocal.SelectedIndex;
            int remoteSel = listBoxRemote.SelectedIndex;
            EntryFrequencyUpdate localEfu = null;
            EntryFrequencyUpdate remoteEfu = null;

            if (localSel >= 0)
                localEfu = listBoxLocal.Items[localSel] as EntryFrequencyUpdate;

            if (remoteSel >= 0)
                remoteEfu = listBoxRemote.Items[remoteSel] as EntryFrequencyUpdate;

            if ((localEfu != null) && (localEfu.LinkUpdatesFrom != null))
            {
                buttonLink.Enabled = false;
                buttonUnlink.Enabled = true;
                return;
            }

            if ((localSel < 0) || (remoteSel < 0))
            {
                buttonLink.Enabled = false;
                buttonUnlink.Enabled = false;
                return;
            }

            buttonLink.Enabled = true;
            buttonUnlink.Enabled = false;

        }

        private void listBoxLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            int localSel = listBoxLocal.SelectedIndex;
            if (localSel >= 0)
            {
                // when changing the local list, force the selection in the remote list
                // to be the Linked-to item.
                EntryFrequencyUpdate localEfu = listBoxLocal.Items[localSel] as EntryFrequencyUpdate;
                if (localEfu.LinkUpdatesFrom != null)
                {
                    foreach (EntryFrequencyUpdate efu in listBoxRemote.Items)
                    {
                        if (efu.SameRig(localEfu.LinkUpdatesFrom))
                        {
                            listBoxRemote.SetSelected(efu.ListIndex, true);
                            break;
                        }
                    }
                }
            }
            ListSelectionChanged();
        }

        private void listBoxRemote_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListSelectionChanged();
        }

        // create a frequency link
        private void buttonLink_Click(object sender, EventArgs e)
        {
            int localSel = listBoxLocal.SelectedIndex;
            int remoteSel = listBoxRemote.SelectedIndex;
            EntryFrequencyUpdate localEfu = null;
            EntryFrequencyUpdate remoteEfu = null;

            if (localSel >= 0)
                localEfu = listBoxLocal.Items[localSel] as EntryFrequencyUpdate;

            if (remoteSel >= 0)
                remoteEfu = listBoxRemote.Items[remoteSel] as EntryFrequencyUpdate;

            if (localEfu != null && remoteEfu != null)
                localEfu.LinkUpdatesFrom = remoteEfu;
            ListSelectionChanged();
        }

        private void buttonUnlink_Click(object sender, EventArgs e)
        {
            int localSel = listBoxLocal.SelectedIndex;
            EntryFrequencyUpdate localEfu = null;
            if (localSel >= 0)
                localEfu = listBoxLocal.Items[localSel] as EntryFrequencyUpdate;
            if (localEfu != null)
                localEfu.LinkUpdatesFrom = null;
            ListSelectionChanged();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_listener != null)
                m_listener.stop();
        }
    }
}

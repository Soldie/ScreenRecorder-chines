using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
 
namespace GlobalMouseHook
{
    public class MouseHook
    {
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;
 
        //ȫ�ֵ��¼�  
        public event MouseEventHandler OnMouseActivity;
 
        static int hMouseHook = 0; //��깳�Ӿ��  
 
        //��곣��  
        public const int WH_MOUSE_LL = 14; //mouse hook constant  
 
        HookProc MouseHookProcedure; //������깳���¼�����.  
 
        //����һ��Point�ķ�������  
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }
 
        //������깳�ӵķ��ͽṹ����  
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }
 
        //װ�ù��ӵĺ���  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
 
        //ж�¹��ӵĺ���  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
 
        //��һ�����ҵĺ���  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
 
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
 
        /// <summary>  
        /// ī�ϵĹ��캯�����쵱ǰ���ʵ��.  
        /// </summary>  
        public MouseHook()
        {
        }
 
        //��������.  
        ~MouseHook()
        {
            Stop();
        }
 
        public void Start()
        {
            //��װ��깳��  
            if (hMouseHook == 0)
            {
                //����һ��HookProc��ʵ��.  
                MouseHookProcedure = new HookProc(MouseHookProc);
 
                hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProcedure, Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]), 0);
 
                //���װ��ʧ��ֹͣ����  
                if (hMouseHook == 0)
                {
                    Stop();
                    throw new Exception("SetWindowsHookEx failed.");
                }
            }
        }
 
        public void Stop()
        {
            bool retMouse = true;
            if (hMouseHook != 0)
            {
                retMouse = UnhookWindowsHookEx(hMouseHook);
                hMouseHook = 0;
            }
 
            //���ж�¹���ʧ��  
            if (!(retMouse)) throw new Exception("UnhookWindowsHookEx failed.");
        }
 
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //����������в����û�Ҫ����������Ϣ  
            if ((nCode >= 0) && (OnMouseActivity != null))
            {
                MouseButtons button = MouseButtons.None;
                int clickCount = 0;
 
                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        button = MouseButtons.Left;
                        clickCount = 10;
                        break;
                    case WM_LBUTTONUP:
                        button = MouseButtons.Left;
                        clickCount = 11;
                        break;
                    case WM_LBUTTONDBLCLK:
                        button = MouseButtons.Left;
                        clickCount = 15;
                        break;
                    case WM_RBUTTONDOWN:
                        button = MouseButtons.Right;
                        clickCount = 20;
                        break;
                    case WM_RBUTTONUP:
                        button = MouseButtons.Right;
                        clickCount = 21;
                        break;
                    case WM_RBUTTONDBLCLK:
                        button = MouseButtons.Right;
                        clickCount = 25;
                        break;
                    case WM_MBUTTONDOWN:
                        button = MouseButtons.Middle;
                        clickCount = 30;
                        break;
                    case WM_MBUTTONUP:
                        button = MouseButtons.Middle;
                        clickCount = 31;
                        break;
                    case WM_MBUTTONDBLCLK:
                        button = MouseButtons.Middle;
                        clickCount = 35;
                        break;
                    //case WM_MOUSEMOVE:
                    //    button = MouseButtons.Left;
                    //    clickCount = 10;
                    //    break;
                }
 
                //�ӻص������еõ�������Ϣ  
                MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                MouseEventArgs e = new MouseEventArgs(button, clickCount, MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y, 0);
                //if(e.X>700)return 1;//�����Ҫ�����������Ļ�е��ƶ���������ڴ˴�����  
                OnMouseActivity(this, e);
            }
            return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
        }
    }
}

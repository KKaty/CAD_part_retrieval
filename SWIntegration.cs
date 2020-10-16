// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SWIntegration.cs" company="NON LO SO :D">
//   NON LO SO :D
// </copyright>
// <summary>
//   The SolidWorks integration class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

/*
 cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319
regasm.exe "C:\Users\SoullessPG\Documents\Visual Studio 2010\Projects\MySolidWorkAddin\bin\Release\AngelSix-MenusAndToolbars.dll" /codebase
 
 */

namespace MySolidWorksAddIn
{
    using System;
    using System.Runtime.InteropServices;

    using AngelSix;

    using SolidWorks.Interop.sldworks;
    using SolidWorks.Interop.swpublished;
    using SWIntegration;
    using SWIntegration.Data_Structure;

    /// <summary>
    /// The SolidWorks integration class
    /// </summary>
    public class SwIntegration : ISwAddin
    {
        #region Private Members

        /// <summary>
        /// The current running SolidWorks instance.
        /// </summary>
        public SldWorks mySolidWorksApplication;

        /// <summary>
        /// The instance of the SolidWorks Cookie.
        /// </summary>
        private int mySolidWorksCookie;

        /// <summary>
        /// The instance of the taskpane
        /// </summary>
        private TaskpaneView myTaskpane;

        /// <summary>
        /// The m taskpane host.
        /// </summary>
        private SWTaskpaneHost myTaskpaneHost;


        #endregion

        #region SW Connection

        /// <summary>
        /// The connect to SolidWork.
        /// </summary>
        /// <param name="solidWork">
        /// The SolidWork's instance.
        /// </param>
        /// <param name="cookie">
        /// The SolidWork's cookie.
        /// </param>
        /// <returns>
        /// True if connected successfully <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// ArgumentNullException if solidWork instance is null
        /// </exception>
        public bool ConnectToSW(object solidWork, int cookie)
        {
            if (solidWork == null)
            {
                throw new ArgumentNullException("solidWork");
            }

            this.mySolidWorksApplication = (SldWorks)solidWork;
            this.mySolidWorksCookie = cookie;

            // Set-up add-in call back info
            bool result = this.mySolidWorksApplication.SetAddinCallbackInfo(0, this, cookie);   

            this.UiSetup();

            return result; 
        }

        /// <summary>
        /// The disconnect from SolidWorks.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DisconnectFromSW()
        {
            this.UiTeardown();
            return true;
        }

        #endregion

        #region COM Register

        /// <summary>
        /// The com register.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        [ComRegisterFunction]
        private static void ComRegister(Type t)
        {
            string keyPath = String.Format(@"SOFTWARE\SolidWorks\AddIns\{0:b}", t.GUID);

            using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(keyPath))
            {
                if (rk != null)
                {
                    rk.SetValue(null, 1); // Load at startup
                    rk.SetValue("Title", "Menus & Toolbars"); // Title
                    rk.SetValue("Description", string.Empty); // Description
                }
            }
        }

        /// <summary>
        /// The com unregister.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        [ComUnregisterFunction]
        private static void ComUnregister(Type t)
        {
            string keyPath = String.Format(@"SOFTWARE\SolidWorks\AddIns\{0:b}", t.GUID);
            Microsoft.Win32.Registry.LocalMachine.DeleteSubKeyTree(keyPath);
        }

        #endregion

        #region UI Setup

        /// <summary>
        /// The ui setup.
        /// </summary>
        private void UiSetup()
        {
            this.myTaskpane = this.mySolidWorksApplication.CreateTaskpaneView2(string.Empty, "Menus and Toolbars");
            this.myTaskpaneHost = (SWTaskpaneHost)this.myTaskpane.AddControl(SWTaskpaneHost.SwtaskpaneProgid, string.Empty);
            this.myTaskpaneHost.Connect(this.mySolidWorksApplication, this.mySolidWorksCookie);
        }

        /// <summary>
        /// The ui teardown.
        /// </summary>
        private void UiTeardown()
        {
            this.myTaskpaneHost = null;
            this.myTaskpane.DeleteView();
            Marshal.ReleaseComObject(this.myTaskpane);
            this.myTaskpane = null;
           
        }

        #endregion
    }
}

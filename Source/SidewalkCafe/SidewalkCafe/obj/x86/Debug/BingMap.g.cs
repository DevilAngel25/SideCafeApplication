﻿

#pragma checksum "\\ctstudent2\studentusers\100215286\Desktop\SidewalkCafe\SidewalkCafe\BingMap.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C470AA42A16B5DE43798804DD57EEFCE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SidewalkCafe
{
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 62 "..\..\..\BingMap.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.PushpinTapped;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 73 "..\..\..\BingMap.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.MapTapped;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 121 "..\..\..\BingMap.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnsave_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 97 "..\..\..\BingMap.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.flyoutBackButton_Click;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 132 "..\..\..\BingMap.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnAddCafe_Click;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 133 "..\..\..\BingMap.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnZoomIn_Click;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 134 "..\..\..\BingMap.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnZoomOut_Click;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 135 "..\..\..\BingMap.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnChangeMapType_Click;
                 #line default
                 #line hidden
                break;
            case 9:
                #line 136 "..\..\..\BingMap.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnTrafficView_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}



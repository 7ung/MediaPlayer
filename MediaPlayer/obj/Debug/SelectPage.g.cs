﻿

#pragma checksum "C:\7ung project\Winphone\MusicPlayer\MediaPlayer\SelectPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C9E482F131F275FA11645E0D2215A968"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MediaPlayer
{
    partial class SelectPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 31 "..\..\SelectPage.xaml"
                ((global::Windows.UI.Xaml.Controls.MediaElement)(target)).MediaEnded += this.mediaElement_MediaEnded;
                 #line default
                 #line hidden
                #line 31 "..\..\SelectPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.mediaElement_Loaded;
                 #line default
                 #line hidden
                #line 31 "..\..\SelectPage.xaml"
                ((global::Windows.UI.Xaml.Controls.MediaElement)(target)).MediaOpened += this.mediaElement_MediaOpened_1;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 59 "..\..\SelectPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Checked += this.repeatBtn_Checked;
                 #line default
                 #line hidden
                #line 59 "..\..\SelectPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Unchecked += this.repeatBtn_Unchecked;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 65 "..\..\SelectPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.previousBtn_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 70 "..\..\SelectPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.playBtn_Click;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 75 "..\..\SelectPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.nextBtn_Click;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 80 "..\..\SelectPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Checked += this.shufferBtn_Checked;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 48 "..\..\SelectPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).ManipulationDelta += this.timeBtn_ManipulationDelta;
                 #line default
                 #line hidden
                #line 48 "..\..\SelectPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).ManipulationCompleted += this.timeBtn_ManipulationCompleted;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 41 "..\..\SelectPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.RangeBase)(target)).ValueChanged += this.progressBar_ValueChanged;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


